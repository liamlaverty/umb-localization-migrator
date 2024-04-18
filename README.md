# Umb Localization Migrator
A tool to migrate Umbraco's v13 XML localization files into the v14 JSON format

## What is this?

Umbraco's localization files for v13 and lower are in XML formats. The localization files for v14 are in JSON objects, contained in TypeScript files. 

This application attempts to convert between the two formats. Where conversions are incompatible, the app creates an audit of the failure. 

## Before you run

- Make sure the files at `./IOFiles/InputXml` match the current Umbraco v13's XML localization files. You should be able to copy/paste them from the Umbraco repo: https://github.com/umbraco/Umbraco-CMS/tree/v13/contrib/src/Umbraco.Core/EmbeddedResources/Lang . Note that you may need to change the branch from `contrib`.

- Make sure the files at `./IOFiles/DiffFinder/v14-us-dataset.json` matches the current Umbraco v14's JSON localization file. You'll need to manually edit the file to remove the Typscript related content. The file should just contain the JSON object, with no `import/export as` typescript code. 

## Run
To run the project, open a terminal and run:

`dotnet run --project ./UmbLocalizationMigrator/UmbLocalizationMigrator/`

## Debug

- Open in VS Code
- Select the debug tool from the left menu
- Press run