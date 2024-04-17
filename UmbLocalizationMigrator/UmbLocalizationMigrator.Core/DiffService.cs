using Newtonsoft.Json.Linq;

namespace UmbLocalizationMigrator.Core
{
    public interface IDiffService
    {
        void PrintJsonDiffs(string sourcePath, string destPath, string reportPath);
    }

    public class DiffService : IDiffService
    {
        /// <summary>
        /// Loads two json files, and prints out a list of properties
        /// present in the source file, but not in the destination file
        /// and vice versa. 
        /// 
        /// Prints the results into a report file
        /// </summary>
        /// <param name="v13JsonPath">the path for the source file</param>
        /// <param name="v14JsonPath">the path for the dest file</param>
        /// <param name="reportPath">the path for the report file</param>
        public void PrintJsonDiffs(string v13JsonPath, string v14JsonPath, string reportPath)
        {
            Console.WriteLine("Loading V13 file" + v13JsonPath);
            JObject jsonV13 = JObject.Parse(File.ReadAllText(v13JsonPath));
            Console.WriteLine("Loading V14 file: " + v14JsonPath);
            JObject jsonV14 = JObject.Parse(File.ReadAllText(v14JsonPath));

            HashSet<string> propertiesInV13 = GetJsonPropsNested(jsonV13);
            HashSet<string> propertiesInV14 = GetJsonPropsNested(jsonV14);

            HashSet<string> propertiesIn13_ButNotIn14 = propertiesInV13.Except(propertiesInV14).ToHashSet();
            HashSet<string> propertiesIn14_ButNotIn13 = propertiesInV14.Except(propertiesInV13).ToHashSet();

            File.WriteAllText(reportPath, "");// clears any existing file



            // Print results
            File.AppendAllText(reportPath, "Properties in V13 but not in V14\r\n\r\n");

            Console.WriteLine("Properties in V13 but not in V14:");
            foreach (string property in propertiesIn13_ButNotIn14)
            {
                Console.WriteLine(property);
                File.AppendAllText(reportPath, $"{property}\r\n");
            }

            File.AppendAllText(reportPath, "\r\n\r\n\r\n--------------------\r\n\r\n\r\n");
            File.AppendAllText(reportPath, "Properties in V14 but not in V13 \r\n\r\n");

            Console.WriteLine("\nProperties in V14 but not in V13:");
            foreach (string property in propertiesIn14_ButNotIn13)
            {
                Console.WriteLine(property);
                File.AppendAllText(reportPath, $"{property}\r\n");

            }
        }


        private HashSet<string> GetJsonPropsNested(JToken json)
        {
            HashSet<string> propertyPaths = new HashSet<string>();

            if (json is JObject obj)
            {
                foreach (JProperty property in obj.Properties())
                {
                    string propertyName = property.Name;
                    string propertyPath = property.Path;
                    propertyPaths.Add(propertyPath);
                    propertyPaths.UnionWith(GetJsonPropsNested(property.Value));
                }
            }
            else if (json is JArray array)
            {
                int index = 0;
                foreach (JToken item in array)
                {
                    string propertyPath = $"{json.Path}[{index}]";
                    propertyPaths.UnionWith(GetJsonPropsNested(item).Select(p => $"{propertyPath}.{p}"));
                    index++;
                }
            }

            return propertyPaths;
        }
    }
}
