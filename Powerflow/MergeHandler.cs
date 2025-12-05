using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class MergeHandler
{
    public static void Run(string xmlFilePath)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Starting RDF merge into existing DB...");

        //string xmlFilePath = args[0];

        var serviceProvider = new ServiceCollection()
            .AddDbContext<PsseDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CimDb;Trusted_Connection=True;"))
            .BuildServiceProvider();

        var context = serviceProvider.GetRequiredService<PsseDbContext>();

        using var reader = new StreamReader(xmlFilePath, new UTF8Encoding(false));
        var allObjects = PsseCaseParser.ParseFromStream(reader);
        Console.WriteLine($"Parsed {allObjects.Count} entity types.");

        var insertedPerType = new Dictionary<string, int>();
        var updatedPerType = new Dictionary<string, int>();
        int totalInserted = 0, totalUpdated = 0, totalErrors = 0;

        foreach (var (typeName, objectsById) in allObjects)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PsseDbContext>();

            Console.WriteLine($"\n🔄 Merging {typeName}: {objectsById.Count} objects");
            insertedPerType[typeName] = 0;
            updatedPerType[typeName] = 0;
            int idx = 0;

            var entityType = Type.GetType($"Profile+{typeName}");
            if (entityType == null)
            {
                Console.WriteLine($"⚠️ Type {typeName} not found");
                totalErrors += objectsById.Count;
                continue;
            }

            foreach (var (rdfId, obj) in objectsById)
            {
                idx++;
                if (string.IsNullOrWhiteSpace(rdfId))
                    continue;

                try
                {
                    // var dbSet = db.GetType().GetMethod("Set", Type.EmptyTypes)!.MakeGenericMethod(entityType).Invoke(db, null);
                    // var findMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods()
                    //     .First(m => m.Name == "FindAsync" && m.GetParameters().Length == 2)
                    //     .MakeGenericMethod(entityType);
                    // var task = (dynamic)findMethod.Invoke(null, new object[] { dbSet, new object[] { rdfId } });
                    var existing = context.Find(entityType, rdfId);

                    if (existing == null)
                    {
                        db.Add(obj);
                        Console.WriteLine("adding " + entityType + rdfId);
                        insertedPerType[typeName]++;
                        totalInserted++;
                    }
                    else if (!AreObjectsEqual(existing, obj))
                    {
                        // CopyValues(existing, obj);
                        // db.Update(existing);
                        // updatedPerType[typeName]++;
                        // totalUpdated++;
                        var entry = db.Entry(existing);
                        entry.CurrentValues.SetValues(obj);

                        if (entry.Properties.Any(p => p.IsModified))
                        {
                            updatedPerType[typeName]++;
                            totalUpdated++;
                        }
                    }

                    if (idx % 1000 == 0 || idx == objectsById.Count)
                        Console.Write($"\rProgress: {idx}/{objectsById.Count} ({(idx * 100) / objectsById.Count}%)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n⚠️ Error merging {typeName} ({rdfId}): {ex.Message}");
                    totalErrors++;
                }
            }

            try
            {
                db.SaveChanges();
                Console.WriteLine($"\n✅ {insertedPerType[typeName]} inserted, {updatedPerType[typeName]} updated in {typeName}");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"❌ DbUpdateException on {typeName}: {dbEx.InnerException?.Message ?? dbEx.Message}");
                totalErrors++;
            }
        }

        Console.WriteLine($"\n🎯 Merge Completed: {totalInserted} inserted, {totalUpdated} updated, {totalErrors} errors.");
        Console.WriteLine("\n📊 Summary:");
        foreach (var kvp in insertedPerType.OrderByDescending(kvp => kvp.Value))
            Console.WriteLine($" - {kvp.Key}: {kvp.Value} inserted, {updatedPerType[kvp.Key]} updated");
    }

    private static bool AreObjectsEqual(object a, object b)
    {
        var type = a.GetType();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var aValue = prop.GetValue(a);
            var bValue = prop.GetValue(b);
            if (!object.Equals(aValue, bValue))
                return false;
        }
        return true;
    }

    private static void CopyValues(object target, object source)
    {
        var type = target.GetType();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.CanWrite)
            {
                var value = prop.GetValue(source);
                prop.SetValue(target, value);
            }
        }
    }
}