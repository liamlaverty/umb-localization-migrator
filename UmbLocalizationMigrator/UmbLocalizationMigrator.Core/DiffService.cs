using Newtonsoft.Json.Linq;

namespace UmbLocalizationMigrator.Core
{
    public interface IDiffService
    {
        void PrintJsonDiffs(string sourcePath, string destPath, string reportPath);
        void WriteDifferenceReportsForGeneratedJson(string reportPath, string jsonDirectoryPath, string v14UsDatasetJsonPath, string v14DkDatasetJsonPath);
    }

    public class DiffService : IDiffService
    {
        /// <summary>
        /// Finds all files in the JSON directory 
        /// 
        /// Loops through them, and then generates a difference report, describing all of the properties
        /// present in the v13, which are not in the new v14 file, and vice-versa
        /// </summary>
        /// <param name="reportPath">the path to the directory where report files will be written to</param>
        /// <param name="jsonDirectoryPath">the path to the new v14 spec json localization files</param>
        /// <param name="v14UsDatasetJsonPath">the path to the sample V14 en-US dataset</param>
        /// <param name="v14DkDatasetJsonPath">The path to the sample V14 dk dataset</param>
        public void WriteDifferenceReportsForGeneratedJson(string reportPath, string jsonDirectoryPath, string v14UsDatasetJsonPath, string v14DkDatasetJsonPath)
        {
            IEnumerable<string> jsonFiles = Directory.GetFiles(jsonDirectoryPath, "*.json");

            foreach (var file in jsonFiles)
            {
                string outputPath = $"{reportPath + Path.GetFileNameWithoutExtension(file)}-migration-report.md";
                Console.WriteLine($"Reporting on json file {file}. File will be generated at {outputPath}");

                // just use the US dataset instead of the DK one, they seem to be the same
                PrintJsonDiffs(file, v14UsDatasetJsonPath, outputPath);
            }
        }




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

            File.AppendAllText(reportPath, $"# Localization migration report for {Path.GetFileNameWithoutExtension(v13JsonPath)}");

            // Print results
            File.AppendAllText(reportPath, "\r\n\r\n## Properties in V13 but not in V14\r\n\r\n");

            foreach (string property in propertiesIn13_ButNotIn14)
            {
                File.AppendAllText(reportPath, $"- {property}\r\n");
            }

            File.AppendAllText(reportPath, "\r\n\r\n\r\n---\r\n\r\n\r\n");
            File.AppendAllText(reportPath, "## Properties in V14 but not in V13 \r\n\r\n");

            foreach (string property in propertiesIn14_ButNotIn13)
            {
                File.AppendAllText(reportPath, $"- {property}\r\n");
            }
        }


        /// <summary>
        /// Gets nested json properties in a HashSet, instead of ust the properties in the top-level of the json file
        /// </summary>
        /// <param name="json">the json object to return</param>
        /// <returns></returns>
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
