using Microsoft.Extensions.Configuration;
using UmbLocalizationMigrator.Core;

namespace UmbLocalizationMigrator
{


    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration configuration = builder.Build();

            Console.WriteLine(configuration[AppConfigPaths.v14SampleFileDirectoryPath]);

            // migrate each file from the V13 localization spec to the V14 spec
            new LocalizationMigrator(configuration).Main([]);

            // write a report for the migrated files, detailing properties present in the 
            // old files, but not the new ones
            new DifferenceFinder(configuration).Main([]);
        }
    }


    internal class DifferenceFinder
    {
        private readonly IDiffService _differ;
        private readonly IConfiguration _config;
        private readonly string v14SampleFileDirectoryPath;

        private readonly string JsonDirectoryPath;
        private readonly string GeneratedReportDirectoryPath;


        public DifferenceFinder(IConfiguration configuration)
        {
            _differ = new DiffService();
            _config = configuration;
            v14SampleFileDirectoryPath = _config[AppConfigPaths.v14SampleFileDirectoryPath] ?? throw new ArgumentNullException(v14SampleFileDirectoryPath);
            JsonDirectoryPath = _config[AppConfigPaths.JsonDirectoryPath] ?? throw new ArgumentNullException(JsonDirectoryPath); ;
            GeneratedReportDirectoryPath = _config[AppConfigPaths.GeneratedReportDirectoryPath] ?? throw new ArgumentNullException(GeneratedReportDirectoryPath); ;
        }

        public void Main(string[] args)
        {
            Console.WriteLine("Starting Diff Service!");

            _differ.WriteDifferenceReportsForGeneratedJson(GeneratedReportDirectoryPath, JsonDirectoryPath, v14SampleFileDirectoryPath + "v14-us-dataset.json", v14SampleFileDirectoryPath + "v14-dk-dataset.json");

            Console.WriteLine("Diff Service Done. Press any key to continue");
            Console.ReadLine();
        }
    }


    /// <summary>
    /// Opens the path to the XML files and converts them to TS files, storing them 
    /// </summary>
    internal class LocalizationMigrator
    {
        private readonly IMigratorService _migrator;
        private readonly IConfiguration _config;


        private readonly string XmlDirectoryPath;
        private readonly string TsDirectoryPath;
        private readonly string JsonDirectoryPath;

        public LocalizationMigrator(IConfiguration configuration)
        {
            _migrator = new MigratorService();
            _config = configuration; 

            XmlDirectoryPath = _config[AppConfigPaths.XmlDirectoryPath] ?? throw new ArgumentNullException(XmlDirectoryPath);
            TsDirectoryPath = _config[AppConfigPaths.TsDirectoryPath] ?? throw new ArgumentNullException(TsDirectoryPath); ;
            JsonDirectoryPath = _config[AppConfigPaths.JsonDirectoryPath] ?? throw new ArgumentNullException(JsonDirectoryPath); ;
        }


        public void Main(string[] args)
        {
            Console.WriteLine("Starting Migration Service!");

            _migrator.MigrateDirectoryFromXmlToJson(XmlDirectoryPath, TsDirectoryPath, JsonDirectoryPath);

            Console.WriteLine("Migration Service Done. Press any key to continue");
            Console.ReadLine();
        }
    }




    internal static class AppConfigPaths
    {
        public const string v14SampleFileDirectoryPath = "DiffFinder:DirectoryPath";
        public const string GeneratedReportDirectoryPath = "DiffFinder:GeneratedReportDirectoryPath";

        public const string XmlDirectoryPath = "LocalizationManager:XmlDirectoryPath";
        public const string JsonDirectoryPath = "LocalizationManager:JsonDirectoryPath";
        public const string TsDirectoryPath = "LocalizationManager:TsDirectoryPath";
    }
}
