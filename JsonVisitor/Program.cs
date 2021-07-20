using JsonVisitor;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonIterator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    throw new Exception("JSON path and CSV path must be passed as an argument!");
                }
                else if(args.Length == 1)
                {
                    throw new Exception("CSV path must be passed as an argument!");
                }
                else if(args.Length > 2)
                {
                    throw new Exception("Invalid number of arguments passed!");
                }

                Console.WriteLine("Attempting to parse JSON file...\n");
                Console.ForegroundColor = ConsoleColor.Green;

                var json = File.ReadAllText(args[0]);
                var parsedJson = JArray.Parse(json);

                var result = Visit(parsedJson);
                var propertyNames = result.Select(item => item.Select(item => item.Name)).SelectMany(item => item).Distinct().ToList();
                var rows = result.Select(item => item.ToDictionary(item => item.Name, item => (object)item.Value)).ToList();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nJSON file parsed successfully...");

                CsvWriter.Write(rows, propertyNames, args[1]);

                Console.WriteLine("CSV file created successfully!\n\nPress any key to exit....");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
        }

        private static List<List<Property>> Visit(JArray jArray)
        {
            var rows = new List<List<Property>>();

            var counter = 0;
            foreach (var child in jArray.Children())
            {
                rows.Add(Visit(child, string.Empty));
                counter++;
            }

            return rows;
        }

        private static List<Property> Visit(JToken jToken, string parentName)
        {
            var properties = new List<Property>();

            if (jToken is JObject)
            {
                foreach (var property in (jToken as JObject).Properties())
                {
                    properties.AddRange(Visit(property.Value, $"{parentName}{(!string.IsNullOrWhiteSpace(parentName) ? ":" : string.Empty)}{property.Name}"));
                }
            }
            else if (jToken is JArray)
            {
                var counter = 0;
                foreach (var child in (jToken as JArray).Children())
                {
                    properties.AddRange(Visit(child, $"{parentName}[{counter}]"));
                    counter++;
                }
            }
            else
            {
                var property = new Property()
                {
                    Name = parentName,
                    Value = jToken.ToString()
                };

                properties.Add(property);
                Console.WriteLine($"\t{property}");
            }

            return properties;
        }
    }
}
