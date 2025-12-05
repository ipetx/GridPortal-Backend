using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Nodes;

// Keep your original assumption of nested classes under Profile
using static PsseCase;

public static class InsertHandler
{
    // Toggle this to false if you want strict "insert-only" behavior.
    private const bool UPSERT = true;

    public static string Run(string jsonFilePath)
    {
        int caseNumber = 1; // default

        // Allow CLI override: dotnet run -- --case=2
        foreach (var a in Environment.GetCommandLineArgs())
        {
            if (a.StartsWith("--case=", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(a.Substring("--case=".Length), out var n))
            {
                caseNumber = n;
                break;
            }
        }

        // Optional: try read CaseNum from caseset_config.json if present
        try
        {
            var cfgPath = "caseset_config.json";
            if (File.Exists(cfgPath))
            {
                var json = JsonDocument.Parse(File.ReadAllText(cfgPath));
                if (json.RootElement.TryGetProperty("CaseInfo", out var ci) &&
                    ci.TryGetProperty("CaseNum", out var cn) &&
                    cn.TryGetInt32(out var n2))
                {
                    caseNumber = n2;
                }
                else if (json.RootElement.TryGetProperty("CaseSet", out var cs) &&
                        cs.TryGetProperty("CaseSetNum", out var csn) &&
                        csn.TryGetInt32(out var n3))
                {
                    // fallback: CaseSetNum if CaseInfo.CaseNum not provided
                    caseNumber = n3;
                }
            }
        }
        catch { /* non-fatal */ }

        Console.WriteLine($"[info] Active CaseNumber = {caseNumber}"); // Or make configurable if you like

        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Starting JSON parsing and ordered DB insertion...");

        var serviceProvider = new ServiceCollection()
            .AddDbContext<PsseDbContext>(options =>
                options.UseNpgsql("Host=localhost;Database=powerflow;Username=postgres;Password=2025"))
            .BuildServiceProvider();

        using var context = serviceProvider.GetRequiredService<PsseDbContext>();

        // Ensure CaseInfo exists
        var caseInfo = context.CaseInfos.Find(caseNumber);
        if (caseInfo == null)
        {
            caseInfo = new CaseInfo
            {
                CaseNum = caseNumber,
                //DOR = DateTime.Now,
                CaseYear = 2025,
                CaseSeason = "Summer",
                CaseType = "Planning",
                Comments = "Imported from JSON"
            };
            context.CaseInfos.Add(caseInfo);
            context.SaveChanges();
        }

        // Insert default "0" entities if missing (kept as-is)
        InsertDefaultEntities(context, caseNumber);

        // Parse JSON
        string jsonContent = File.ReadAllText(jsonFilePath);
        var root = JsonNode.Parse(jsonContent)?.AsObject();
        if (root == null || !root.TryGetPropertyValue("network", out var networkNode) || networkNode is not JsonObject network)
        {
            Console.WriteLine("❌ Invalid or missing 'network' section.");
            return "Success!";
        }

        // Flatten "network" into entityName -> (fields, rows[])
        var entityData = ExtractEntities(network);

        // FK mappings (kept names/properties matching your tnp.cs expectations)
        var fkMappings = BuildFkMappings();

        // Order: prioritize these types first, then the rest
        var priorityOrder = new[] {"caseset", "caseinfo", "owner", "zone", "area", "bus", "load", "sub", "acline", "facts" };
        var remaining = entityData.Keys.Except(priorityOrder, StringComparer.OrdinalIgnoreCase).ToList();
        var sorted = priorityOrder.Where(entityData.ContainsKey).ToList();
        sorted.AddRange(remaining.Where(entityData.ContainsKey));

        int totalInserted = 0;
        int totalUpdated = 0;
        var insertedPerType = new Dictionary<string, int>();
        var updatedPerType = new Dictionary<string, int>();

        foreach (var typeName in sorted)
        {
            var (fields, rows) = entityData[typeName];

            // Resolve entity CLR type based on name (e.g., "bus" -> Profile.Bus)
            var entityType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.Name.Equals(Capitalize(typeName), StringComparison.OrdinalIgnoreCase));

            if (entityType == null) continue;

            insertedPerType[typeName] = 0;
            updatedPerType[typeName] = 0;

            // Pre-fetch Set<T> & reflection methods once per type for performance
            var (dbSet, findMethod) = GetDbSetAndFind(context, entityType);

            foreach (var row in rows)
            {
                var obj = Activator.CreateInstance(entityType)!;

                // 1) Assign scalar properties from JSON
                AssignScalars(entityType, obj, fields, row);

                // 2) Resolve foreign keys, set CaseNumber on nav-case props when applicable
                ResolveForeignKeys(context, entityType, obj, fields, row, fkMappings, caseNumber);

                // 3) Ensure your Case reference is set
                CaseNumberHelper.SetCaseReference(obj, caseNumber, caseInfo);

                // 4) Compute entity keys for Find(...) (supports composite keys, adds CaseNumber if present)
                var keyProps = GetKeyProps(entityType);
                var caseProp = entityType.GetProperty("CaseNumber", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (caseProp != null && !keyProps.Contains(caseProp)) keyProps.Add(caseProp);

                // If there's no [Key], skip (matches original behavior that required a key)
                if (keyProps.Count == 0)
                {
                    // You had a guard: skip if keyValue is null/empty; here we skip entire row if no keys can be identified.
                    continue;
                }

                // Ensure CaseNumber is set on obj (if property exists)
                if (caseProp != null && caseProp.CanWrite)
                {
                    var currentCase = caseProp.GetValue(obj);
                    if (currentCase == null || (currentCase is int ci && ci == 0))
                        caseProp.SetValue(obj, caseNumber);
                }

                var keyValues = keyProps.Select(p => p.GetValue(obj)).ToArray();
                var existing = findMethod!.Invoke(dbSet, new object[] { keyValues });

                if (existing == null)
                {
                    // INSERT
                    try
                    {
                        context.Add(obj);
                        insertedPerType[typeName]++;
                        totalInserted++;
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("cannot be tracked because another instance with the same key value"))
                    {
                        // Already tracked within the same context with same key; treat as existing
                        if (UPSERT)
                        {
                            CopyScalars(entityType, obj, ref existing, skipProps: keyProps);
                            updatedPerType[typeName]++;
                            totalUpdated++;
                        }
                        else
                        {
                            Console.WriteLine($"❌ Duplicate key error inserting {typeName}: {ex.Message}");
                            LogKeyValues(keyProps, obj);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ General error inserting {typeName}: {ex.Message}");
                    }
                }
                else
                {
                    // UPDATE if UPSERT enabled; otherwise skip like your original
                    if (UPSERT)
                    {
                        CopyScalars(entityType, obj, ref existing, skipProps: keyProps);
                        updatedPerType[typeName]++;
                        totalUpdated++;
                    }
                    else
                    {
                        // skip + log (matches original intent of skipping duplicates)
                        Console.WriteLine($"↺ Exists, skipping update: {typeName} {string.Join(", ", keyValues)}");
                    }
                }
            }

            // Save per-type, with IDENTITY_INSERT if needed (kept your original behavior)
            try
            {
                string? identityColumn = GetIdentityColumnName(entityType);
                if (identityColumn != null)
                    context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT [{entityType.Name}] ON");

                context.SaveChanges();

                Console.WriteLine($"✅ {typeName}: {insertedPerType[typeName]} inserted"
                    + (UPSERT ? $", {updatedPerType[typeName]} updated." : "."));

                if (identityColumn != null)
                    context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT [{entityType.Name}] OFF");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"❌ Error saving {typeName}: {dbEx.Message}");
                if (dbEx.InnerException != null)
                    Console.WriteLine($"🔍 Inner exception: {dbEx.InnerException.Message}");
                foreach (var entry in dbEx.Entries)
                {
                    Console.WriteLine($"🔍 Failed entity: {entry.Entity.GetType().Name}");
                    foreach (var prop in entry.CurrentValues.Properties)
                    {
                        var val = entry.CurrentValues[prop];
                        Console.WriteLine($"  • {prop.Name} = {val}");
                    }
                }
            }
        }

        return $"\n🎯 Completed: {totalInserted} inserted{(UPSERT ? $", {totalUpdated} updated" : "")}.";
    }

    // ---- Helpers ----------------------------------------------------------------

    private static Dictionary<string, (List<string> fields, List<JsonArray> rows)> ExtractEntities(JsonObject network)
    {
        var entityData = new Dictionary<string, (List<string> fields, List<JsonArray> rows)>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in network)
        {
            if (entry.Value is not JsonObject obj) continue;
            if (!obj.TryGetPropertyValue("fields", out var fieldsNode) || !obj.TryGetPropertyValue("data", out var dataNode))
                continue;

            var fields = fieldsNode!.AsArray().Select(f => f!.ToString()).ToList();
            var rows = new List<JsonArray>();
            if (dataNode is JsonArray arr && arr.Count > 0 && arr[0] is JsonArray)
                rows.AddRange(arr.Select(i => i!.AsArray()));
            else if (dataNode is JsonArray single && single.Count > 0)
                rows.Add(single);
            else
                continue;

            entityData[entry.Key] = (fields, rows);
        }

        return entityData;
    }

    private static Dictionary<string, (string navProp, Type targetType, string keyName)> BuildFkMappings()
    {
        return new(StringComparer.OrdinalIgnoreCase)
        {
            { "owner", ("Owner", typeof(PsseCase.Owner), "Iowner") },
            { "iowner", ("Owner", typeof(PsseCase.Owner), "Iowner") },
            { "o1", ("O1", typeof(PsseCase.Owner), "Iowner") },
            { "o2", ("O2", typeof(PsseCase.Owner), "Iowner") },
            { "o3", ("O3", typeof(PsseCase.Owner), "Iowner") },
            { "o4", ("O4", typeof(PsseCase.Owner), "Iowner") },
            { "zone",  ("Zone",  typeof(PsseCase.Zone),  "Izone")  },
            { "izone",  ("Zone",  typeof(PsseCase.Zone),  "Izone")  },
            { "area",  ("Area",  typeof(PsseCase.Area),  "Iarea")  },
            { "iarea",  ("Area",  typeof(PsseCase.Area),  "Iarea")  },
            { "arfrom",  ("FromArea",  typeof(PsseCase.Area),  "Iarea")  },
            { "arto",  ("ToArea",  typeof(PsseCase.Area),  "Iarea")  },
            { "ibus",  ("FromBus", typeof(PsseCase.Bus), "Ibus")   },
            { "jbus",  ("ToBus",   typeof(PsseCase.Bus), "Jbus")   },
            { "kbus",  ("AuxiliaryBus", typeof(PsseCase.Bus), "Kbus") },
            { "isub",  ("FromSub", typeof(PsseCase.Sub), "Isub") },
            { "dum1", ("Dum1Bus", typeof(PsseCase.Bus), "Ibus") },
            { "dum2", ("Dum2Bus", typeof(PsseCase.Bus), "Ibus") },
            { "dum3", ("Dum3Bus", typeof(PsseCase.Bus), "Ibus") },
            { "dum4", ("Dum4Bus", typeof(PsseCase.Bus), "Ibus") },
            { "dum5", ("Dum5Bus", typeof(PsseCase.Bus), "Ibus") },
            { "dum6", ("Dum6Bus", typeof(PsseCase.Bus), "Ibus") },
            { "dum7", ("Dum7Bus", typeof(PsseCase.Bus), "Ibus") },
            { "dum8", ("Dum8Bus", typeof(PsseCase.Bus), "Ibus") },
            { "dum9", ("Dum9Bus", typeof(PsseCase.Bus), "Ibus") }
        };
    }

    private static void AssignScalars(Type entityType, object obj, List<string> fields, JsonArray row)
    {
        for (int i = 0; i < fields.Count && i < row.Count; i++)
        {
            var fld = fields[i];
            var val = ConvertJsonValue(row[i]);
            var prop = entityType.GetProperty(fld, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null && prop.CanWrite)
            {
                try
                {
                    var cv = Convert.ChangeType(val, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    prop.SetValue(obj, cv);
                }
                catch
                {
                    // ignore conversion errors, preserve original behavior
                }
            }
        }
    }

    private static void ResolveForeignKeys(
        PsseDbContext context,
        Type entityType,
        object obj,
        List<string> fields,
        JsonArray row,
        Dictionary<string, (string navProp, Type targetType, string keyName)> fkMappings,
        int caseNumber
    )
    {
        foreach (var fld in fields)
        {
            if (!fkMappings.TryGetValue(fld, out var info)) continue;

            int idx = fields.FindIndex(x => string.Equals(x, fld, StringComparison.OrdinalIgnoreCase));
            object? fkVal = ConvertJsonValue(row[idx]);

            // PATCH: For o2/o3/o4, map blank/null to zero (kept behavior)
            if ((fld.Equals("o2", StringComparison.OrdinalIgnoreCase) ||
                 fld.Equals("o3", StringComparison.OrdinalIgnoreCase) ||
                 fld.Equals("o4", StringComparison.OrdinalIgnoreCase)) &&
                (fkVal == null ||
                 (fkVal is string st && string.IsNullOrWhiteSpace(st))))
            {
                fkVal = 0;
                Console.WriteLine($"[PATCH] {entityType.Name}.{fld} was null/blank -- mapped to 0");
            }

            try
            {
                // Build DbSet<T> and Find(...)
                var (dbSet, find) = GetDbSetAndFind(context, info.targetType);

                var pkProp = info.targetType.GetProperty(info.keyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (pkProp == null) continue;

                var keyType = Nullable.GetUnderlyingType(pkProp.PropertyType) ?? pkProp.PropertyType;
                var typedKey = (fkVal == null) ? null : Convert.ChangeType(fkVal, keyType);

                // Composite (Id, CaseNumber) for these types:
                object[] compositeKey = info.targetType == typeof(PsseCase.Bus)
                                        || info.targetType == typeof(PsseCase.Area)
                                        || info.targetType == typeof(PsseCase.Owner)
                                        || info.targetType == typeof(PsseCase.Zone)
                    ? new object[] { typedKey!, caseNumber }
                    : new object[] { typedKey! };

                var refObj = (typedKey == null) ? null : find!.Invoke(dbSet, new object[] { compositeKey });

                var allProps = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // If FK id is present, also set "*CaseNumber" property for the nav
                var navCaseProp = allProps.FirstOrDefault(p =>
                    p.Name.Equals(info.navProp + "CaseNumber", StringComparison.OrdinalIgnoreCase));

                if (navCaseProp != null && navCaseProp.CanWrite)
                {
                    bool hasValidFK = fkVal switch
                    {
                        null => false,
                        int val => val != 0,
                        double val => val != 0,
                        string val => !string.IsNullOrWhiteSpace(val),
                        _ => true
                    };
                    if (hasValidFK) navCaseProp.SetValue(obj, caseNumber);
                }

                // Force some case refs you previously forced
                ForceCaseNumberIfExists(entityType, obj, "ToBusCaseNumber", caseNumber);
                ForceCaseNumberIfExists(entityType, obj, "AuxiliaryBusCaseNumber", caseNumber);
                ForceCaseNumberIfExists(entityType, obj, "FromSubCaseNumber", caseNumber);

                foreach (var ownerCasePropName in new[] { "Owner1CaseNumber", "Owner2CaseNumber", "Owner3CaseNumber", "Owner4CaseNumber" })
                    ForceCaseNumberIfExists(entityType, obj, ownerCasePropName, caseNumber);

                // Set FK id if found, or zero/null if missing (kept your behavior)
                if (refObj != null)
                {
                    var fkIdProp = allProps.FirstOrDefault(p =>
                        p.Name.Equals(info.navProp + "Id", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Equals(info.keyName, StringComparison.OrdinalIgnoreCase));

                    if (fkIdProp != null && fkIdProp.CanWrite)
                    {
                        var fkValue = Convert.ChangeType(fkVal, Nullable.GetUnderlyingType(fkIdProp.PropertyType) ?? fkIdProp.PropertyType);
                        fkIdProp.SetValue(obj, fkValue);
                    }
                }
                else
                {
                    if (fkVal == null ||
                        (fkVal is string str && string.IsNullOrWhiteSpace(str)) ||
                        (fkVal is double d && d == 0) ||
                        (fkVal is int i && i == 0))
                    {
                        fkVal = 0;

                        // Also set the actual property on the object
                        var fkIdProp = entityType.GetProperty(info.keyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (fkIdProp != null && fkIdProp.CanWrite)
                        {
                            var zeroValue = Convert.ChangeType(0, Nullable.GetUnderlyingType(fkIdProp.PropertyType) ?? fkIdProp.PropertyType);
                            fkIdProp.SetValue(obj, zeroValue);
                        }
                    }

                    Console.WriteLine($"⚠️ Could not resolve FK {entityType.Name}.{info.navProp} = {fkVal}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception resolving FK {entityType.Name}.{info.navProp}: {ex.Message}");
            }
        }
    }

    private static void ForceCaseNumberIfExists(Type entityType, object obj, string propName, int caseNumber)
    {
        var p = entityType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (p != null && p.CanWrite) p.SetValue(obj, caseNumber);
    }

    private static (object dbSet, MethodInfo? findMethod) GetDbSetAndFind(DbContext context, Type entityType)
    {
        var setMethod = context.GetType()
            .GetMethods()
            .First(m => m.Name == "Set" && m.IsGenericMethod && m.GetParameters().Length == 0)
            .MakeGenericMethod(entityType);
        var dbSet = setMethod.Invoke(context, null)!;
        var find = dbSet.GetType().GetMethod("Find", new[] { typeof(object[]) });
        return (dbSet, find);
    }

    private static List<PropertyInfo> GetKeyProps(Type entityType)
    {
        return entityType
            .GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Any())
            .ToList();
    }

    private static void CopyScalars(Type entityType, object source, ref object existing, IEnumerable<PropertyInfo> skipProps)
    {
        var skip = new HashSet<string>(skipProps.Select(p => p.Name), StringComparer.OrdinalIgnoreCase);

        foreach (var p in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!p.CanWrite || p.SetMethod?.IsPublic != true) continue;
            if (skip.Contains(p.Name)) continue; // don't alter keys

            // Avoid copying navigations if you have any (heuristic: skip class-type props except string)
            if (p.PropertyType != typeof(string) && !p.PropertyType.IsValueType && !IsNullableValueType(p.PropertyType))
                continue;

            try
            {
                var newVal = p.GetValue(source);
                p.SetValue(existing, newVal);
            }
            catch
            {
                // Ignore copy errors, keep robust behavior
            }
        }
    }

    private static bool IsNullableValueType(Type t) =>
        Nullable.GetUnderlyingType(t) != null;

    private static void LogKeyValues(IEnumerable<PropertyInfo> keyProps, object obj)
    {
        Console.WriteLine("🔍 Duplicate key values:");
        foreach (var prop in keyProps)
        {
            var val = prop.GetValue(obj);
            Console.WriteLine($" • {prop.Name} = {val}");
        }
    }

    // Insert your default "0" rows (unchanged except minor tidy-ups)
    private static void InsertDefaultEntities(PsseDbContext context, int caseNumber)
{
    // Area 0
    if (!context.Set<Area>().Any(a => a.Iarea == 0 && a.CaseNumber == caseNumber))
        context.Set<Area>().Add(new Area { Iarea = 0, CaseNumber = caseNumber, Arname = "Default Area", Isw = 0, Pdes = 0, Ptol = 0 });
    context.SaveChanges();

    // Owner 0
    if (!context.Set<Owner>().Any(o => o.Iowner == 0 && o.CaseNumber == caseNumber))
        context.Set<Owner>().Add(new Owner { Iowner = 0, CaseNumber = caseNumber, Owname = "Default Owner" });
    context.SaveChanges();

    // Zone 0
    if (!context.Set<Zone>().Any(z => z.Izone == 0 && z.CaseNumber == caseNumber))
        context.Set<Zone>().Add(new Zone { Izone = 0, CaseNumber = caseNumber, zoname = "Default Zone" });
    context.SaveChanges();

    // Bus 0
    if (!context.Set<Bus>().Any(b => b.Ibus == 0 && b.CaseNumber == caseNumber))
        context.Set<Bus>().Add(new Bus {
            Ibus = 0, CaseNumber = caseNumber, Name = "Default Bus",
            Iarea = 0, Iowner = 0, ZoneId = 0, Baskv = 0, Vm = 0, Va = 0,
            Nvlo = 0, Nvhi = 0, Evlo = 0, Evhi = 0, Ide = 0,
            AreaCaseNumber = caseNumber,
            OwnerCaseNumber = caseNumber,
            ZoneCaseNumber = caseNumber
        });
    context.SaveChanges();

    // Sub 0 (some datasets point Subnodes to Sub=0)
    if (!context.Set<Sub>().Any(s => s.Isub == 0 && s.CaseNumber == caseNumber))
        context.Set<Sub>().Add(new Sub { Isub = 0, CaseNumber = caseNumber, Name = "Default Sub" });
    context.SaveChanges();

    // (Optional) Load 0, if your data expects it:
    if (!context.Set<Load>().Any(l => l.Ibus == 0 && l.CaseNumber == caseNumber))
        context.Set<Load>().Add(new Load {
            Ibus = 0, FromBusCaseNumber = caseNumber, CaseNumber = caseNumber,
            Loadid = "Default", Stat = 1, Iarea = 0, ZoneId = 0, Iowner = 0, Scale = 1,
            AreaCaseNumber = caseNumber,
            OwnerCaseNumber = caseNumber,
            ZoneCaseNumber = caseNumber
        });
    context.SaveChanges();
}


    private static object? ConvertJsonValue(JsonNode? node)
    {
        if (node == null || node.ToString() == "null") return null;
        var s = node.ToString();

        // Try int first to avoid "2" -> double 2.0 when property expects int
        if (int.TryParse(s, out var i)) return i;
        if (double.TryParse(s, out var d)) return d;
        if (bool.TryParse(s, out var b)) return b;
        return s;
    }

    private static string Capitalize(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..];

    private static string? GetIdentityColumnName(Type entityType)
    {
        foreach (var prop in entityType.GetProperties())
        {
            var attr = prop.GetCustomAttribute<DatabaseGeneratedAttribute>();
            if (attr?.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                return prop.Name;
        }
        return null;
    }
}
