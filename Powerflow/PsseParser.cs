using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Text;

public static class PsseCaseParser
{
    public static Dictionary<string, Dictionary<string, object>> ParseFromString(string xmlContent)
    {
        var doc = XDocument.Parse(xmlContent);
        return ParseAll(doc);
    }

    public static Dictionary<string, Dictionary<string, object>> ParseFromStream(StreamReader reader)
    {
        var doc = XDocument.Load(reader, LoadOptions.PreserveWhitespace);  // ✅ uses stream, avoids memory spike
        return ParseAll(doc);
    }

    public static Dictionary<string, Dictionary<string, object>> ParseAll(string rdfFilePath)
    {
        var xmlContent = File.ReadAllText(rdfFilePath, new UTF8Encoding(false));
        return ParseFromString(xmlContent);
    }

    public static Dictionary<string, Dictionary<string, object>> ParseAll(XDocument doc)
    {
        XNamespace rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        var instanceMap = new Dictionary<string, Dictionary<string, object>>();
        var idToObjectMap = new Dictionary<string, object>();

        foreach (var elem in doc.Root.Elements())
        {
            string localName = elem.Name.LocalName;
            string rdfId = elem.Attribute(rdf + "ID")?.Value;
            if (rdfId == null) continue;

            Type type = Type.GetType("Profile+" + localName);
            if (type == null) continue;

            object instance = Activator.CreateInstance(type);

            PropertyInfo idProp = type.GetProperty("RdfID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (idProp != null && idProp.CanWrite)
                idProp.SetValue(instance, rdfId);

            foreach (var child in elem.Elements())
            {
                string propName = child.Name.LocalName.Split('.').Last();
                PropertyInfo prop = type.GetProperty(propName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
                if (prop == null || !prop.CanWrite) continue;

                if (child.Attribute(rdf + "resource") == null)
                {
                    string text = child.Value;
                    object convertedValue = null;

                    try
                    {
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        if (targetType == typeof(string))
                        {
                            convertedValue = text;
                        }
                        else if (targetType == typeof(bool) && bool.TryParse(text, out var boolVal))
                        {
                            convertedValue = boolVal;
                        }
                        else if (targetType == typeof(int) && int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var intVal))
                        {
                            convertedValue = intVal;
                        }
                        else if (targetType == typeof(double) && double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleVal))
                        {
                            convertedValue = doubleVal;
                        }

                        // Only set if value is parsed successfully or it's a nullable type
                        if (convertedValue != null || Nullable.GetUnderlyingType(prop.PropertyType) != null)
                        {
                            prop.SetValue(instance, convertedValue);
                        }
                    }
                    catch
                    {
                        // Skip silently
                    }
                }
            }

            if (!instanceMap.ContainsKey(localName))
                instanceMap[localName] = new Dictionary<string, object>();

            instanceMap[localName][rdfId] = instance;
            idToObjectMap[rdfId] = instance;
        }

        // second pass: resolve references
        foreach (var elem in doc.Root.Elements())
        {

            string localName = elem.Name.LocalName;
            string rdfId = elem.Attribute(rdf + "ID")?.Value;
            if (rdfId == null || !instanceMap.ContainsKey(localName) || !instanceMap[localName].ContainsKey(rdfId)) continue;

            var instance = instanceMap[localName][rdfId];
            Type type = instance.GetType();

            foreach (var child in elem.Elements())
            {
                //Console.WriteLine($"  - {child.Name.LocalName}: '{child.Value}'");
                var resourceAttr = child.Attribute(rdf + "resource");
                if (resourceAttr == null) continue;

                string refId = resourceAttr.Value.TrimStart('#');
                if (!idToObjectMap.ContainsKey(refId)) continue;

                string propName = child.Name.LocalName.Split('.').Last();
                PropertyInfo prop = type.GetProperty(propName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null || !prop.CanWrite) continue;

                var referencedObj = idToObjectMap[refId];
                var propType = prop.PropertyType;

                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propType)
                    && propType != typeof(string)
                    && propType.IsGenericType)
                {
                    var list = prop.GetValue(instance);
                    if (list == null)
                    {
                        Type elementType = propType.GetGenericArguments()[0];
                        var listImpl = typeof(List<>).MakeGenericType(elementType);
                        list = Activator.CreateInstance(listImpl);
                        prop.SetValue(instance, list);
                    }
                    var addMethod = list.GetType().GetMethod("Add");
                    addMethod.Invoke(list, new[] { referencedObj });
                }
                else
                {
                    try { prop.SetValue(instance, referencedObj); }
                    catch (ArgumentException) { }
                }
            }
        }

        return instanceMap;
    }
}




// using System;
// using System.Collections.Generic;
// using System.Globalization;
// using System.Linq;
// using System.Reflection;
// using System.Xml.Linq;

// public static class RdfProfileParser
// {
//     public static Dictionary<string, List<object>> ParseAll(string rdfFilePath)
//     {
//         XDocument doc = XDocument.Load(rdfFilePath);
//         // Define namespaces from your RDF file.
//         XNamespace rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
//         XNamespace cim = "http://iec.ch/TC57/2006/CIM-schema-cim10#";
//         // (Add other namespaces as needed.)

//         // Dictionary to store lists of objects for each type.
//         Dictionary<string, List<object>> parsedObjects = new Dictionary<string, List<object>>();

//         // Process each element that is in the CIM namespace.
//         foreach (var elem in doc.Root.Elements())
//         {
//             if (elem.Name.Namespace != cim)
//                 continue;

//             // The local name should correspond to a class defined in Profile.
//             string localName = elem.Name.LocalName; // e.g. "AnalogLimit", "AssetInfo", etc.
//             // Our generated classes are nested inside Profile, so their full name is "Profile+{localName}".
//             Type type = Type.GetType("Profile+" + localName);
//             if (type == null)
//                 continue; // Skip elements that don't have a matching class.

//             // Create an instance.
//             object instance = Activator.CreateInstance(type);

//             // For each child element, attempt to set a property on the instance.
//             foreach (var child in elem.Elements())
//             {
//                 // Get the child element's local name.
//                 string childLocalName = child.Name.LocalName;
//                 // Often the element names include a prefix like "AnalogLimit.value". Remove the prefix.
//                 string propName = childLocalName;
//                 if (propName.Contains('.'))
//                 {
//                     string[] parts = propName.Split('.');
//                     if (parts.Length > 1)
//                         propName = parts[1];
//                 }

//                 // skip for now
//                 var resourceAttr = child.Attribute(rdf+"resource");
//                 if (resourceAttr != null){
//                     continue;
//                 }

//                 PropertyInfo prop = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
//                 if (prop != null && prop.CanWrite)
//                 {
//                     try
//                     {
//                         string text = child.Value;
//                         object convertedValue = null;
//                         if (prop.PropertyType == typeof(string))
//                         {
//                             convertedValue = text;
//                         }
//                         else if (prop.PropertyType == typeof(bool))
//                         {
//                             convertedValue = bool.Parse(text);
//                         }
//                         else if (prop.PropertyType == typeof(int))
//                         {
//                             convertedValue = int.Parse(text, CultureInfo.InvariantCulture);
//                         }
//                         else if (prop.PropertyType == typeof(double))
//                         {
//                             convertedValue = double.Parse(text, CultureInfo.InvariantCulture);
//                         }
//                         // Set the property value.
//                         prop.SetValue(instance, convertedValue);
//                     }
//                     catch
//                     {
//                         // Ignore conversion errors for now.
//                     }
//                 }
//             }

//             // Add the created instance to the dictionary.
//             if (!parsedObjects.ContainsKey(localName))
//                 parsedObjects[localName] = new List<object>();
//             parsedObjects[localName].Add(instance);
//         }
//         return parsedObjects;
//     }
// }
