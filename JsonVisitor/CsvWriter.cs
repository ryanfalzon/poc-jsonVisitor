using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JsonVisitor
{
    public static class CsvWriter
    {
        public static void Write(List<Dictionary<string, object>> data, List<string> properties, string path)
        {
            try
            {
                var csvData = (from entry in data
                               select (from property in properties
                                       select entry.ContainsKey(property) ? entry[property]?.ToString() : null).ToList()).ToList();

                csvData.Insert(0, properties.Select(property => property).ToList());

                WriteToFile(csvData, path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void WriteToFile(List<List<string>> data, string path)
        {
            var stringBuilder = new StringBuilder();

            foreach (var @object in data)
            {
                stringBuilder.Append(string.Join(',', @object));
                stringBuilder.AppendLine();
            }

            File.WriteAllText(path, stringBuilder.ToString());
        }
    }
}