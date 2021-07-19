using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace JsonIterator
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    throw new Exception("Path must be passed as an argument!");
                }

                var json = File.ReadAllText(args[0]);
                var parsedJson = JToken.Parse(json);

                Visit(parsedJson, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Visit(JToken jToken, string parentName)
        {
            if (jToken is JObject)
            {
                Visit(jToken as JObject, parentName);
            }
            else if (jToken is JArray)
            {
                Visit(jToken as JArray, parentName);
            }
            else
            {
                Console.WriteLine($"{parentName} - {jToken}");
            }
        }

        public static void Visit(JObject jObject, string parentName)
        {
            foreach (var property in jObject.Properties())
            {
                Visit(property.Value, $"{parentName}{(!string.IsNullOrWhiteSpace(parentName) ? ":" : string.Empty)}{property.Name}");
            }
        }

        public static void Visit(JArray jArray, string parentName)
        {
            var counter = 0;
            foreach (var child in jArray.Children())
            {
                Visit(child, $"{parentName}[{counter}]");
                counter++;
            }
        }
    }
}
