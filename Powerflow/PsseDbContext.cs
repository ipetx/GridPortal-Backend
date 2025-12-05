using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

public class PsseDbContext : DbContext
{
    public PsseDbContext(DbContextOptions<PsseDbContext> options) : base(options) { }

    public DbSet<PsseCase.CaseInfo> CaseInfos { get; set; }
    public DbSet<PsseCase.CaseSet> CaseSet { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var profileTypes = typeof(PsseCase).GetNestedTypes(BindingFlags.Public);

        // --- Define FK roots for all main reference types ---
        var fkRoots = new Dictionary<Type, string[]>
        {
            { typeof(PsseCase.Area), new[] { "Iarea", "Arfrom", "Arto" } },
            { typeof(PsseCase.Owner), new[] { "Iowner", "O1", "O2", "O3", "O4" } },
            { typeof(PsseCase.Load), new[] { "Iload" } },
            { typeof(PsseCase.Sub), new[] { "Isub" } },
            { typeof(PsseCase.Zone), new[] { "Izone" } },
            { typeof(PsseCase.Bus), new[] { "Ibus", "Jbus", "Kbus", "Dum1", "Dum2", "Dum3", "Dum4", "Dum5", "Dum6", "Dum7", "Dum8", "Dum9"} }
        };

        // 1. Add CaseNumber to composite primary keys
        foreach (var type in profileTypes)
        {
            if (!type.GetProperties().Any()) continue;

            var keyProps = type.GetProperties()
                .Where(p => p.GetCustomAttribute<KeyAttribute>() != null)
                .Select(p => p.Name)
                .ToList();

            if (!keyProps.Contains("CaseNumber"))
            {
                var caseProp = type.GetProperty("CaseNumber", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (caseProp != null) keyProps.Add("CaseNumber");
            }

            if (keyProps.Count > 0)
                modelBuilder.Entity(type).HasKey(keyProps.ToArray());
            else
                modelBuilder.Entity(type).HasNoKey();
        }

        // 2. Automatically configure composite foreign keys
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType == null) continue;

            var props = clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var caseProp = props.FirstOrDefault(p => p.Name.Equals("CaseNumber", StringComparison.OrdinalIgnoreCase));
            if (caseProp == null) continue;

            foreach (var navProp in props)
            {
                var navType = navProp.PropertyType;
                if (!fkRoots.ContainsKey(navType)) continue;

                string navName = navProp.Name;
                var fkCandidates = fkRoots[navType];
                string? idPropName = null;

                // -- Bus logic --
                if (navType == typeof(PsseCase.Bus))
                {
                    if (navName.StartsWith("From", StringComparison.OrdinalIgnoreCase))
                        idPropName = "Ibus";
                    else if (navName.StartsWith("To", StringComparison.OrdinalIgnoreCase))
                        idPropName = "Jbus";
                    else if (navName.StartsWith("Auxiliary", StringComparison.OrdinalIgnoreCase) || navName.StartsWith("K", StringComparison.OrdinalIgnoreCase))
                        idPropName = "Kbus";
                    else if (navName.Equals("Bus", StringComparison.OrdinalIgnoreCase))
                        idPropName = "Ibus";
                    else
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(navName, @"^Dum(\d+)Bus$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (match.Success)
                            idPropName = "Dum" + match.Groups[1].Value;
                    }
                    
                }
                // -- Area logic --
                else if (navType == typeof(PsseCase.Area))
                {
                    if (navName.StartsWith("FromArea", StringComparison.OrdinalIgnoreCase))
                        idPropName = "Arfrom";
                    else if (navName.StartsWith("ToArea", StringComparison.OrdinalIgnoreCase))
                        idPropName = "Arto";
                    else
                        idPropName = "Iarea";
                }

                // -- Zone logic --
                else if (navType == typeof(PsseCase.Zone))
                    idPropName = "Izone";
                else if (navType == typeof(PsseCase.Sub)) {
                    if (navName.StartsWith("FromSub", StringComparison.OrdinalIgnoreCase))
                        idPropName = "Isub";
                    else idPropName = "Isub";
                }
                    
                // -- Owner logic, now supporting Owner1, Owner2, etc. --
                else if (navType == typeof(PsseCase.Owner))
                {
                    if (navName.Equals("Owner", StringComparison.OrdinalIgnoreCase))
                        idPropName = "Iowner";
                    // For Owner1, Owner2, ..., map to O1, O2, ...
                    else
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(navName, @"^Owner(\d+)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (match.Success)
                            idPropName = "O" + match.Groups[1].Value;
                        else
                            idPropName = null;
                    }
                }

                if (idPropName == null) continue;

                // Now find the composite case number property, if present
                string navCasePropName = navName + "CaseNumber";
                var navCaseProp = props.FirstOrDefault(p => p.Name.Equals(navCasePropName, StringComparison.OrdinalIgnoreCase));

                string[] foreignKeyProps;
                string[] principalKeyProps;

                // If the navigation is to Bus and a ...CaseNumber property exists, use composite FK
                if (navType == typeof(PsseCase.Bus) && navCaseProp != null)
                {
                    foreignKeyProps = new[] { idPropName, navCaseProp.Name };
                    principalKeyProps = new[] { "Ibus", "CaseNumber" };
                }
                else if (navType == typeof(PsseCase.Area) && navCaseProp != null)
                {
                    foreignKeyProps = new[] { idPropName, navCaseProp.Name };
                    principalKeyProps = new[] { "Iarea", "CaseNumber" };
                }
                else if (navType == typeof(PsseCase.Owner) && navCaseProp != null)
                {
                    foreignKeyProps = new[] { idPropName, navCaseProp.Name };
                    principalKeyProps = new[] { "Iowner", "CaseNumber" };
                }
                else if (navType == typeof(PsseCase.Sub) && navCaseProp != null)
                {
                    foreignKeyProps = new[] { idPropName, navCaseProp.Name };
                    principalKeyProps = new[] { "Isub", "CaseNumber" };
                }
                else if (navType == typeof(PsseCase.Zone) && navCaseProp != null)
                {
                    foreignKeyProps = new[] { idPropName, navCaseProp.Name };
                    principalKeyProps = new[] { "Izone", "CaseNumber" };
                }
                else
                {
                    // Fallback: single FK (no case number)
                    // foreignKeyProps = new[] { idPropName };
                    // principalKeyProps = new[] { idPropName };
                    continue;
                }
Console.WriteLine($"Yun check navType = {navType}, navCaseProp = {navCaseProp?.Name}, principalKeyProps = [{string.Join(", ", principalKeyProps)}], foreignKeyProps = [{string.Join(", ", foreignKeyProps)}]");


                modelBuilder.Entity(clrType)
                    .HasOne(navType, navName)
                    .WithMany()
                    .HasForeignKey(foreignKeyProps)
                    .HasPrincipalKey(principalKeyProps)
                    .OnDelete(DeleteBehavior.Restrict);

                // Optionally, log what's being configured:
                // Console.WriteLine($"✔️ FK {clrType.Name}.{navName} => {navType.Name} [{string.Join(", ", foreignKeyProps)}]");
            }
        }

        // 3. Disable cascade delete for all FKs globally
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var fk in entityType.GetForeignKeys())
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        // 4. Optional: make Acline PK non-clustered (to avoid 900-byte PK limit)
        modelBuilder.Entity<PsseCase.Acline>()
            .HasKey(e => new { e.Ckt, e.Ibus, e.Jbus, e.CaseNumber })
            .IsClustered(false);

        // 5. Optional: shorten Acline.Ckt to avoid PK key size limit
        modelBuilder.Entity<PsseCase.Acline>()
            .Property(e => e.Ckt)
            .HasMaxLength(200); // Under 450 bytes (nvarchar(200) = 400 bytes)
    }
}