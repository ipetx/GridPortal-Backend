using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

public static class DynamicXmlParser
{
    /// <summary>
    /// Loads an XML file and converts it to a dynamic object.
    /// </summary>
    public static dynamic ParseXml(string filePath)
    {
        XDocument doc = XDocument.Load(filePath);
        return ParseElement(doc.Root);
    }

    /// <summary>
    /// Recursively converts an XElement into a dynamic ExpandoObject.
    /// This method also assigns each attribute as a property with an "Attr_" prefix.
    /// </summary>
    private static dynamic ParseElement(XElement element)
    {
        IDictionary<string, object> expando = new ExpandoObject();

        // Store the element's local name.
        expando["ElementName"] = element.Name.LocalName;

        // Add attributes as properties with an "Attr_" prefix.
        foreach (var attr in element.Attributes())
        {
            expando["Attr_" + attr.Name.LocalName] = attr.Value;
        }

        // Process child elements.
        if (element.HasElements)
        {
            // Group children by their local name.
            var groups = element.Elements().GroupBy(e => e.Name.LocalName);
            foreach (var group in groups)
            {
            // If there is more than one element with the same name, store them in a list.
            if (group.Count() > 1)
            {
                List<dynamic> list = new List<dynamic>();
                foreach (var child in group)
                    {
                        list.Add(ParseElement(child));
                    }
                expando[group.Key] = list;
            }
            else
            {
                expando[group.Key] = ParseElement(group.First());
            }
            }
        }
        else
        {
            // If no children exist, store the text content.
            expando["#text"] = element.Value;
        }

        return expando;
        }
}
