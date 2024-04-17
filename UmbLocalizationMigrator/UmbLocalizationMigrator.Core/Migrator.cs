using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;
using UmbLocalizationMigrator.Core.Models;
using static System.Net.Mime.MediaTypeNames;

namespace UmbLocalizationMigrator.Core
{
    public interface IMigrator
    {
        void MigrateDirectoryFromXmlToJson(string xmlFilePath, string jsonFolderPath);

    }

    public class Migrator :IMigrator
    {
        public void MigrateDirectoryFromXmlToJson(string xmlFolderPath, string jsonFolderPath)
        {
            IEnumerable<string> xmlFiles = Directory.GetFiles(xmlFolderPath, "*.xml");

            foreach (var file in xmlFiles)
            {
                MigrateFileFromXmlToJson(file, jsonFolderPath);
            }

        }

        private void MigrateFileFromXmlToJson(string file, string outputPath)
        {
            Console.WriteLine($"Migrating file {file}");


            XmlSerializer serializer = new XmlSerializer(typeof(language));
            language language = new language();    

            using (FileStream stream = new FileStream(file, FileMode.Open))
            {
                language = (language)serializer.Deserialize(stream);
            }

            string filePath = outputPath + language.culture + ".ts";
            File.WriteAllText(filePath, "");// clears any existing file

            File.AppendAllText(filePath, $"/** \r\n * \r\n * Origin File: https://github.com/umbraco/Umbraco-CMS/tree/v13/contrib/src/Umbraco.Core/EmbeddedResources/Lang/{file} \r\n\r\n * Creator Name: {language.creator.name} \r\n * Creator Link: {language.creator.link} \r\n */\r\n\r\n");

            File.AppendAllText(filePath, "import type { UmbLocalizationDictionary } from '@umbraco-cms/backoffice/localization-api';\r\n");
            File.AppendAllText(filePath, "export default {\r\n");

            foreach (var area in language.area)
            {
                // Console.WriteLine($"Found area {area.alias}");
                File.AppendAllText(filePath, area.alias + ": {\r\n");

                foreach (var key in area.key)
                {
                    // Console.WriteLine($"Found key {key.alias}:{key.Value}");
                    File.AppendAllText(filePath, $"{key.alias}: {JsonConvert.SerializeObject(key.Value)},\r\n");
                }

                File.AppendAllText(filePath, "\r\n},\r\n");

            }
            File.AppendAllText(filePath, "\r\n} as UmbLocalizationDictionary;");



            Console.WriteLine($"Completed migrating file {file}");

        }



    }
}
