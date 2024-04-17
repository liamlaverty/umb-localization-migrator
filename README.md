# umb-localization-migrator
A tool to migrate Umbraco's v13 XML localization files into the v14 JSON format

## What is this?

Umbraco's localization files for v13 and lower are in XML formats. The localization files for v14 are in JSON objects, contained in TypeScript files. 

This application attempts to convert between the two formats. Where conversions are incompatible, the app creates an audit of the failure. 


## Run
To run the project, open a terminal and run:

cd `dotnet run --project ./UmbLocalizationMigrator/UmbLocalizationMigrator/`

## Debug

- Open in VS Code
- Select the debug tool from the left menu
- Press run