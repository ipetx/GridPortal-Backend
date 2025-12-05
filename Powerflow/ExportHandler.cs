using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Xml.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;


// public static class ExportHandler
// {
//     public static void Run(string outputPath)
//     {
//         Console.OutputEncoding = System.Text.Encoding.UTF8;
//         Console.WriteLine("Starting database export to XML...");

//         // 1) Ensure target directory exists
//         var folder = Path.GetDirectoryName(outputPath);
//         if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
//             Directory.CreateDirectory(folder);

//         // 2) Build service provider and get your DbContext
//         var serviceProvider = new ServiceCollection()
//             .AddDbContext<PsseDbContext>(options =>
//                 options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CimDb;Trusted_Connection=True;"))
//             .BuildServiceProvider();

//         using var context = serviceProvider.GetRequiredService<PsseDbContext>();

//         // 3) Prepare XML namespaces
//         XNamespace rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
//         XNamespace cim = "http://iec.ch/TC57/2006/CIM-schema-cim17#";

//         var doc = new XDocument(
//             new XDeclaration("1.0", "utf-8", "yes"),
//             new XElement(rdf + "RDF",
//                 new XAttribute(XNamespace.Xmlns + "rdf", rdf),
//                 new XAttribute(XNamespace.Xmlns + "cim", cim)
//             )
//         );

//         // 4) Locate the generic DbContext.Set<T>() method unambiguously
//         var setMethod = typeof(DbContext)
//             .GetMethods(BindingFlags.Public | BindingFlags.Instance)
//             .First(m => m.Name == "Set"
//                      && m.IsGenericMethod
//                      && m.GetParameters().Length == 0);

//         // 5) Iterate through all EF entity types
//         foreach (var et in context.Model.GetEntityTypes())
//         {
//             var clrType = et.ClrType;
//             var queryable = setMethod
//                 .MakeGenericMethod(clrType)
//                 .Invoke(context, null) as IQueryable;

//             if (queryable == null) 
//                 continue;

//             // 6) For each instance in the table
//             foreach (var entity in queryable.Cast<object>())
//             {
//                 // a) Fetch the rdf:ID
//                 var idProp = clrType.GetProperty("RdfID", BindingFlags.Public | BindingFlags.Instance);
//                 if (idProp == null) 
//                     continue;

//                 var rdfId = (string)idProp.GetValue(entity)!;

//                 // b) Create the <cim:ClassName rdf:ID="..."> element
//                 var elem = new XElement(cim + clrType.Name,
//                     new XAttribute(rdf + "ID", rdfId)
//                 );

//                 // c) Export each public property
//                 foreach (var prop in clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
//                 {
//                     if (prop.Name == "RdfID") 
//                         continue;

//                     var value = prop.GetValue(entity);
//                     if (value == null) 
//                         continue;

//                     // Determine which CLR type actually declared this prop
//                     var declaring = prop.DeclaringType?.Name ?? clrType.Name;
//                     // Lowercase the first letter of the property name
//                     var localName = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
//                     var tag = $"{declaring}.{localName}";

//                     // Handle collections
//                     if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) 
//                         && prop.PropertyType != typeof(string))
//                     {
//                         foreach (var item in (IEnumerable)value)
//                         {
//                             var itemIdProp = item.GetType().GetProperty("RdfID");
//                             if (itemIdProp == null) 
//                                 continue;

//                             var itemId = (string)itemIdProp.GetValue(item)!;
//                             elem.Add(new XElement(cim + tag,
//                                 new XAttribute(rdf + "resource", "#" + itemId)));
//                         }
//                     }
//                     // Handle single references
//                     else if (!prop.PropertyType.IsPrimitive && prop.PropertyType != typeof(string))
//                     {
//                         var refIdProp = prop.PropertyType.GetProperty("RdfID");
//                         if (refIdProp != null)
//                         {
//                             var refId = (string)refIdProp.GetValue(value)!;
//                             elem.Add(new XElement(cim + tag,
//                                 new XAttribute(rdf + "resource", "#" + refId)));
//                         }
//                         else
//                         {
//                             // Fallback to scalar if no RdfID found
//                             elem.Add(new XElement(cim + tag, value.ToString()));
//                         }
//                     }
//                     // Scalar
//                     else
//                     {
//                         elem.Add(new XElement(cim + tag, value.ToString()));
//                     }
//                 }

//                 doc.Root!.Add(elem);
//             }
//         }

//         // 7) Save — this will create (or overwrite) the file if the folder exists
//         doc.Save(outputPath);
//         Console.WriteLine($"✅ Export completed: {doc.Root!.Elements().Count()} elements → {outputPath}");
//     }
// }

public static class ExportHandler
{
    // Export ALL cases: writes one file per CaseInfo.CaseNum into outputDir
    public static void RunAll(string outputDir, string filePattern = "case_{0}.rawx")
    {
        Directory.CreateDirectory(outputDir);

        using var db = new PsseDbContextFactory().CreateDbContext(Array.Empty<string>());

        // CaseNum is non-nullable int in your model
        var allCases = db.Set<PsseCase.CaseInfo>()
                         .AsNoTracking()
                         .Select(ci => ci.CaseNum)
                         .Distinct()
                         .OrderBy(n => n)
                         .ToList();

        if (allCases.Count == 0)
        {
            Console.WriteLine("No cases found in CaseInfo.");
            return;
        }

        filePattern = NormalizePattern(filePattern);

        foreach (var cn in allCases)
        {
            var path = Path.Combine(outputDir, string.Format(filePattern, cn));
            path = EnsureUnique(path);
            RunOne(path, cn);
        }
    }

    // Export a single case into the given file
    public static void RunOne(string outputPath, int caseNumber = 1)
    {
        // Allow override via CLI flag: --case=#
        foreach (var a in Environment.GetCommandLineArgs())
        {
            if (a.StartsWith("--case=", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(a.Substring("--case=".Length), out var n))
            {
                caseNumber = n;
                break;
            }
        }

        Console.WriteLine($"[export] CaseNumber = {caseNumber}");

        using var db = new PsseDbContextFactory().CreateDbContext(Array.Empty<string>());

        // Discover all public nested entity types in Profile
        var entityTypes = typeof(PsseCase).GetNestedTypes(BindingFlags.Public)
            .Where(t => t.IsClass && t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any())
            .ToList();

        var sb = new StringBuilder(1 << 20);
        using var sw = new StringWriter(sb);

        // Root
        sw.WriteLine("{");
        sw.WriteLine("  \"network\": {");

        for (int ei = 0; ei < entityTypes.Count; ei++)
        {
            var et = entityTypes[ei];
            string entityKey = et.Name.ToLowerInvariant();

            // db.Set<T>()
            var setMethod = typeof(DbContext)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .First(m => m.Name == nameof(db.Set) && m.IsGenericMethod && m.GetParameters().Length == 0)
                .MakeGenericMethod(et);

            var set = (IQueryable<object>)setMethod.Invoke(db, null)!;

            // Pull rows (no tracking), then filter in-memory by CaseNumber (or CaseInfo.CaseNum)
            var rows = set.AsNoTracking().ToList();

            var caseProp = et.GetProperty("CaseNumber", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (caseProp != null)
            {
                rows = rows.Where(r => SafeInt(caseProp.GetValue(r)) == caseNumber).ToList();
            }
            else if (et == typeof(PsseCase.CaseInfo))
            {
                var caseNumProp = et.GetProperty("CaseNum", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (caseNumProp != null)
                    rows = rows.Where(r => SafeInt(caseNumProp.GetValue(r)) == caseNumber).ToList();
                else
                    rows.Clear();
            }

            // Filter out default rows for some entities where key == 0
            switch (entityKey)
            {
                case "bus":
                    rows = rows.Where(r => SafeInt(et.GetProperty("Ibus")?.GetValue(r)) != 0).ToList();
                    break;
                case "owner":
                    rows = rows.Where(r => SafeInt(et.GetProperty("Iowner")?.GetValue(r)) != 0).ToList();
                    break;
                case "area":
                    rows = rows.Where(r => SafeInt(et.GetProperty("Iarea")?.GetValue(r)) != 0).ToList();
                    break;
                case "zone":
                    rows = rows.Where(r => SafeInt(et.GetProperty("Izone")?.GetValue(r)) != 0).ToList();
                    break;
                case "sub":
                    rows = rows.Where(r => SafeInt(et.GetProperty("Isub")?.GetValue(r)) != 0).ToList();
                    break;
                case "load":
                    // adapt depending on your schema — if it’s "Iload" or "LoadId"
                    var loadProp = et.GetProperty("Iload") ?? et.GetProperty("Loadid");
                    if (loadProp != null)
                        rows = rows.Where(r => SafeInt(loadProp.GetValue(r)) != 0).ToList();
                    break;
            }


            // -------- property selection (IMPORTANT) ----------
            // Get all public instance props
            var allProps = et.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 🔴 Drop ANY property whose name contains "CaseNumber" (e.g., CaseNumber, FromBusCaseNumber, Owner4CaseNumber, ...)
            allProps = allProps
                .Where(p => !p.Name.Contains("CaseNumber", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            // Keep only scalar props (no navigations/collections)
            var scalarProps = allProps.Where(IsScalarProp).ToList();

            // Keys first (from filtered allProps), then other scalars
            var keyProps = allProps.Where(p => p.GetCustomAttribute<KeyAttribute>() != null).ToList();
            var nonKeyScalars = scalarProps.Except(keyProps).ToList();
            var orderedProps = keyProps.Concat(nonKeyScalars).ToList();

            // Build fields list from property NAMES (InsertHandler binds by property name)
            var fieldNames = orderedProps.Select(p => p.Name).ToList();

            // ----- write entity block -----
            sw.WriteLine($"    \"{entityKey}\": {{");

            // fields
            sw.Write("      \"fields\": [");
            for (int i = 0; i < fieldNames.Count; i++)
            {
                if (i > 0) sw.Write(", ");
                sw.Write(JsonSerializer.Serialize(fieldNames[i]));
            }
            sw.WriteLine("],");

            // data (always 2D; one row per line)
            sw.WriteLine("      \"data\": [");
            for (int r = 0; r < rows.Count; r++)
            {
                var row = rows[r];
                sw.Write("        [");
                for (int c = 0; c < orderedProps.Count; c++)
                {
                    if (c > 0) sw.Write(", ");
                    var val = orderedProps[c].GetValue(row);
                    sw.Write(JsonValueStringForCell(val));
                }
                sw.Write("]");
                if (r < rows.Count - 1) sw.Write(",");
                sw.WriteLine();
            }
            sw.Write("      ]");

            sw.WriteLine();
            sw.Write("    }");
            if (ei < entityTypes.Count - 1) sw.Write(",");
            sw.WriteLine();
        }

        sw.WriteLine("  }");
        sw.WriteLine("}");

        // Save file
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath)) ?? ".");
        
var content = sb.ToString().Replace("\n", "\r\n");
File.WriteAllText(outputPath, content, new UTF8Encoding(false));


        Console.WriteLine($"✅ Export complete → {Path.GetFullPath(outputPath)}");
    }

    // --------------- helpers ----------------

    private static int SafeInt(object? v)
    {
        if (v == null) return 0;
        if (v is int i) return i;
        try { return Convert.ToInt32(v); } catch { return 0; }
    }

    private static bool IsScalarProp(PropertyInfo p)
    {
        var t = p.PropertyType;

        // Collections → navigation
        if (typeof(IEnumerable).IsAssignableFrom(t) && t != typeof(string))
            return false;

        // Navigation to another Profile.* class → not scalar
        if (t.IsClass && t != typeof(string) && t.DeclaringType == typeof(PsseCase))
            return false;

        // value types, string, nullable, enums → scalar
        var nt = Nullable.GetUnderlyingType(t);
        t = nt ?? t;

        return t.IsPrimitive
            || t.IsEnum
            || t == typeof(string)
            || t == typeof(DateTime)
            || t == typeof(decimal)
            || t == typeof(Guid)
            || t == typeof(double)
            || t == typeof(float)
            || t == typeof(int)
            || t == typeof(long)
            || t == typeof(short)
            || t == typeof(byte)
            || t == typeof(bool);
    }

    private static string JsonValueStringForCell(object? v)
    {
        if (v is DateTime dt) return JsonSerializer.Serialize(dt.ToString("o")); // ISO string
        if (v is float f)     return JsonSerializer.Serialize((double)f);
        if (v is decimal m)   return JsonSerializer.Serialize((double)m);
        if (v is short s16)   return JsonSerializer.Serialize((int)s16);
        if (v is byte b8)     return JsonSerializer.Serialize((int)b8);
        return JsonSerializer.Serialize(v);
    }

    // Ensure the pattern contains "{0}" and ends with .json
    private static string NormalizePattern(string pattern)
    {
        if (!pattern.Contains("{0}"))
        {
            var ext = Path.GetExtension(pattern);
            if (string.IsNullOrEmpty(ext))
            {
                pattern = pattern.TrimEnd() + "{0}.rawx";
            }
            else
            {
                var name = Path.GetFileNameWithoutExtension(pattern);
                pattern = name + "{0}" + ext;
            }
        }
        if (string.IsNullOrEmpty(Path.GetExtension(pattern)))
            pattern = pattern + ".rawx";
        return pattern;
    }

    // If a file exists, add _1, _2, ... until unique
    private static string EnsureUnique(string path)
    {
        if (!File.Exists(path)) return path;

        var dir = Path.GetDirectoryName(path) ?? ".";
        var name = Path.GetFileNameWithoutExtension(path);
        var ext = Path.GetExtension(path);
        int i = 1;
        string candidate;
        do
        {
            candidate = Path.Combine(dir, $"{name}_{i}{ext}");
            i++;
        } while (File.Exists(candidate));
        return candidate;
    }
}
