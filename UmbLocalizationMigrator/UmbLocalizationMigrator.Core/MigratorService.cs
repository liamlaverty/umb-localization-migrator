﻿using Newtonsoft.Json;
using System.Xml.Serialization;
using UmbLocalizationMigrator.Core.Models;

namespace UmbLocalizationMigrator.Core
{

    public interface IMigratorService
    {
        void MigrateDirectoryFromXmlToJson(string xmlFilePath, string tsFolderPath, string jsonFolderPath);

    }

    public class MigratorService :IMigratorService
    {

        /// <summary>
        /// Opens a given directory, and then attempts to migrate each 
        /// XML file inside from Umbraco's V13 format to the JSON V14 format
        /// </summary>
        /// <param name="xmlFolderPath">the dirctory path to the XML input</param>
        /// <param name="tsFolderPath">the directory path to the TypeScript output</param>
        public void MigrateDirectoryFromXmlToJson(string xmlFolderPath, string tsFolderPath, string jsonFolderPath)
        {
            IEnumerable<string> xmlFiles = Directory.GetFiles(xmlFolderPath, "*.xml");

            foreach (var file in xmlFiles)
            {
                MigrateFileFromXmlToJson(file, jsonFolderPath, writeToJsonObj: true);
                MigrateFileFromXmlToJson(file, tsFolderPath, writeToJsonObj: false);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="outputPath"></param>
        /// <param name="writeToJsonObj"></param>
        private void MigrateFileFromXmlToJson(string file, string outputPath, bool writeToJsonObj)
        {
            Console.WriteLine($"Migrating to TS: file {file} ");

            XmlSerializer serializer = new XmlSerializer(typeof(language));
            language language = new language();    

            using (FileStream stream = new FileStream(file, FileMode.Open))
            {
                language = (language)serializer.Deserialize(stream);
            }

            string filePath = outputPath + language.culture.ToLower() + (writeToJsonObj ?  ".json" : ".ts");
            File.WriteAllText(filePath, "");// clears any existing file

            if (!writeToJsonObj){
                File.AppendAllText(filePath, @$"/** 
* Origin File: https://github.com/umbraco/Umbraco-CMS/tree/v13/contrib/src/Umbraco.Core/EmbeddedResources/Lang/{Path.GetFileName(file)} 
* Creator Name: {language.creator.name} 
* Creator Link: {language.creator.link} 
*
* Language Alias: {language.alias} 
* Language Int Name: {language.intName} 
* Language Local Name: {language.localName} 
* Language LCID: {language.lcid} 
* Language Culture: {language.culture} 
*/");


                bool includeManifestHeader = false;
                if (includeManifestHeader){
                    File.AppendAllText(filePath, @$"/** 
Manifest File Entry: 
{{
    type: 'localization',
    alias: 'Umb.Localization.{Char.ToUpper(language.culture[0]) + language.culture.Substring(1)}',
    weight: -100,
    name: '{language.localName}',
    meta: {{
        culture: '{language.culture.ToLower()}',
    }},
    js: () => import('../../../assets/lang/{language.culture.ToLower()}.js'),
}},
*/");
                }
                File.AppendAllText(filePath, "\r\nimport type { UmbLocalizationDictionary } from '@umbraco-cms/backoffice/localization-api';\r\n");
                File.AppendAllText(filePath, "export default");
            }
            File.AppendAllText(filePath, "{\r\n");

            foreach (var area in language.area)
            {
                File.AppendAllText(filePath, "\t");

                // Console.WriteLine($"Found area {area.alias}");
                File.AppendAllText(filePath, area.alias + ": {\r\n");

                foreach (var key in area.key.DistinctBy(c => c.alias))
                {
                    File.AppendAllText(filePath, "\t\t");

                    if (String.IsNullOrWhiteSpace(key.Value)) {
                        // intentionally do nothing
                    }
                    else if (IsSpecialKey(key.alias))
                    {
                        File.AppendAllText(filePath, GetSpecialKeyText(key.alias, key.Value));
                    }
                    else if (IsNowLowerCaseKey(key.alias))
                    {
                        File.AppendAllText(filePath, GetSpecialKeyText(key.alias.ToLower(), key.Value));
                    }
                    else if (IsIgnoredKey(area.alias, key.alias))
                    {
                        // intentionally do nothing
                    }
                    else
                    {
                        File.AppendAllText(filePath, $"{key.alias}: {JsonConvert.SerializeObject(key.Value)},\r\n");
                    }
                }

                File.AppendAllText(filePath, "\t},\r\n");

            }


            File.AppendAllText(filePath, "\t\r\n}");
            if (!writeToJsonObj)
            {
                File.AppendAllText(filePath, " as UmbLocalizationDictionary;");
            }


            Console.WriteLine($"Completed migrating file {file}");

        }

        private bool IsNowLowerCaseKey(string alias)
        {
            switch (alias)
            {
                case "editdictionary":
                case "editlanguage":
                    return true;
                default:
                    return false;
            }
        }

        private bool IsIgnoredKey(string area, string alias)
        {
            return false;
            switch ($"{area}.{alias}")
            {
                case "actions.toggleHideUnavailable":
                case "login.userFailedLogin":
                case "login.bottomText":
                case "login.forgottenPassword":
                case "login.forgottenPasswordInstruction":
                case "login.requestPasswordResetConfirmation":
                case "login.showPassword":
                case "login.hidePassword":
                case "login.returnToLogin":
                case "login.setPasswordInstruction":
                case "login.setPasswordConfirmation":
                case "login.resetCodeExpired":
                case "login.resetPasswordEmailCopySubject":
                case "login.resetPasswordEmailCopyFormat":
                case "login.mfaSecurityCodeSubject":
                case "login.mfaSecurityCodeMessage":
                case "login.2faTitle":
                case "login.2faText":
                case "login.2faMultipleText":
                case "login.2faCodeInput":
                case "login.2faCodeInputHelp":
                case "login.2faInvalidCode":
                case "notifications.mailBody":
                case "notifications.mailBodyVariantSummary":
                case "notifications.mailBodyHtml":
                case "notifications.mailBodyVariantHtmlSummary":
                case "notifications.mailSubject":
                case "contentPicker.specifyPickerRootTitle":
                case "contentPicker.defineXPathOrigin":
                case "contentPicker.configurationStartNodeTitle":
                case "contentPicker.configurationXPathTitle":
                case "dynamicRoot.cancelAndClearQuery":
                case "webhooks.addWebhook":
                case "webhooks.addWebhookHeader":
                case "webhooks.addDocumentType":
                case "webhooks.addMediaType":
                case "webhooks.createHeader":
                case "webhooks.deliveries":
                case "webhooks.noHeaders":
                case "webhooks.noEventsFound":
                case "webhooks.enabled":
                case "webhooks.events":
                case "webhooks.event":
                case "webhooks.url":
                case "webhooks.types":
                case "webhooks.webhookKey":
                case "webhooks.retryCount":
                case "webhooks.toggleDebug":
                case "webhooks.statusNotOk":
                case "webhooks.urlDescription":
                case "webhooks.eventDescription":
                case "webhooks.contentTypeDescription":
                case "webhooks.enabledDescription":
                case "webhooks.headersDescription":
                case "webhooks.contentType":
                case "webhooks.headers":
                case "webhooks.selectEventFirst":
                case "translation.mailBody":
                case "user.selectUserGroups":
                case "user.inviteEmailCopySubject":
                case "user.inviteEmailCopyFormat":
                case "healthcheck.scheduledHealthCheckEmailBody":
                case "healthcheck.scheduledHealthCheckEmailSubject":
                case "visuallyHiddenTexts.expandChildItems":
                case "visuallyHiddenTexts.openContextNode":
                case "settingsDashboard.start":
                case "settingsDashboard.startDescription":
                case "settingsDashboard.more":
                case "settingsDashboard.bulletPointOne":
                case "settingsDashboard.bulletPointTwo":
                case "settingsDashboard.bulletPointTutorials":
                case "settingsDashboard.bulletPointFour":
                case "settingsDashboard.bulletPointFive":
                case "blockEditor.insertBlock":
                case "blockEditor.labelInlineMode":
                    return true;
                default:
                    return false;
            }
        }

        private string GetSpecialKeyText(string key, string value)
        {
            return $"{JsonConvert.SerializeObject(key)}: {JsonConvert.SerializeObject(value)},\r\n";
        }

        private bool IsSpecialKey(string alias)
        {
            switch (alias)
            {
                case "2fa":
                case "2faDisableText":
                case "2faProviderIsEnabled":
                case "2faProviderIsDisabledMsg":
                case "2faProviderIsNotDisabledMsg":
                case "2faDisableForUser":
                case "2faTitle":
                case "2faText":
                case "2faMultipleText":
                case "2faCodeInput":
                case "2faCodeInputHelp":
                case "2faInvalidCode":
                    return true;
                default:
                    return false;
            }
        }
    }
}
