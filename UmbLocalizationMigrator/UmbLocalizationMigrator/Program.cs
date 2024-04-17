using UmbLocalizationMigrator.Core;

namespace UmbLocalizationMigrator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new LocalizationMigrator().Main(args);
        }
    }


    internal class LocalizationMigrator
    {
        private readonly IMigrator _migrator;

        private readonly string XmlDirectoryPath = @"C:\UmbLocalizationMigrator\XmlFiles\";
        private readonly string JsonDirectoryPath = @"C:\UmbLocalizationMigrator\JsonFiles\";

        public LocalizationMigrator()
        {
            _migrator = new Migrator();
        }


        public void Main(string[] args)
        {



            Console.WriteLine("Starting!");

            _migrator.MigrateDirectoryFromXmlToJson(XmlDirectoryPath, JsonDirectoryPath);

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
