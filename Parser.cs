using KrivojPerevodDS3.Translation;
using KrivojPerevodDS3.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON = Newtonsoft.Json.JsonConvert;

namespace KrivojPerevodDS3
{
    public static class Parser
    {
        public static void Parse()
        {
            string originalFolderPath = @"C:\Users\Logos\source\repos\KrivojPerevodDS3\Parcing\menu_dlc2-msgbnd-dcx\msg\rusRU\64bit";
            string translatedFolderPath = @"C:\Games\Dark Souls 3 Proklyatiy Perevod\JSON\Menu";

            string[] originalFilesPaths = Directory.GetFiles(originalFolderPath, "*.xml");
            string[] translatedFilesPaths = Directory.GetFiles(translatedFolderPath);

            FileInfo[] originalFiles = originalFilesPaths.Select(x => new FileInfo(x)).ToArray();
            FileInfo[] translatedFiles = translatedFilesPaths.Select(x => new FileInfo(x)).ToArray();

            foreach (var trFile in translatedFiles)
            {
                string originalFileName = trFile.Name.Replace("RU_", "").Replace(".json", "");
                string json = System.IO.File.ReadAllText(trFile.FullName);
                FileInfo originalFile = originalFiles.FirstOrDefault(o => o.Name == originalFileName);

                List<PhraseModel> phrases = JSON.DeserializeObject<List<PhraseModel>>(json);
                //phrases = Translator.Filter(phrases, false);

                //string originalText = File.ReadAllText(originalFile.FullName);
                XMLMainDocument document = XMLHelper.ReadXml(originalFile.FullName);

                foreach (var phrase in phrases)
                {
                    if (phrase.TranslatedText == null)
                        continue;

                    if (phrase.OriginalText.Any(s => Translator.ForbiddenSymbols.Contains(s)))
                        continue;

                    string text = phrase.TranslatedText.Replace("<", "").Replace(">", "");
                    XMLPhrase xmlPhrase = document.entries.FirstOrDefault(e => e.id == phrase.Id);
                    if(xmlPhrase != null)
                    {
                        xmlPhrase.text = text;
                    }
                    else
                    {

                    }
                    
                    //originalText = originalText.Replace(phrase.OriginalText, text);
                }

                XMLHelper.WriteXml2(originalFile.FullName, document);

                // xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema

                //File.WriteAllText(originalFile.FullName, originalText);
            }
        }
    }
}
