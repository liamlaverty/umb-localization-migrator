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

            Console.WriteLine(configuration[AppConfigPaths.DirectoryPath]);

            new LocalizationMigrator(configuration).Main([]);

            new DifferenceFinder(configuration).Main([]);
        }
    }


    internal class DifferenceFinder
    {
        private readonly IDiffService _differ;
        private readonly IConfiguration _config;
        private readonly string DirectoryPath;

        public DifferenceFinder(IConfiguration configuration)
        {
            _differ = new DiffService();
            _config = configuration;
            DirectoryPath = _config[AppConfigPaths.DirectoryPath] ?? throw new ArgumentNullException(DirectoryPath);
        }

        public void Main(string[] args)
        {
            Console.WriteLine("Starting Diff Service!");

            _differ.PrintJsonDiffs(DirectoryPath + "v13-us-dataset.json", DirectoryPath + "v14-us-dataset.json", DirectoryPath + "report.txt");

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
            JsonDirectoryPath = _config[AppConfigPaths.JsonDirectoryPath] ?? throw new ArgumentNullException(TsDirectoryPath); ;
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
        public const string DirectoryPath = "DiffFinder:DirectoryPath";
        public const string XmlDirectoryPath = "LocalizationManager:XmlDirectoryPath";
        public const string JsonDirectoryPath = "LocalizationManager:JsonDirectoryPath";
        public const string TsDirectoryPath = "LocalizationManager:TsDirectoryPath";
    }
}
