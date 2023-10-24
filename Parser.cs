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
            string originalFolderPath = @"C:\Users\Logos\source\repos\KrivojPerevodDS3\Parcing\item_dlc1-msgbnd-dcx\msg\rusRU\64bit";
            string translatedFolderPath = @"C:\Games\GOTOVO\JSON\Items";

            string[] originalFilesPaths = Directory.GetFiles(originalFolderPath);
            string[] translatedFilesPaths = Directory.GetFiles(translatedFolderPath);

            FileInfo[] originalFiles = originalFilesPaths.Select(x => new FileInfo(x)).ToArray();
            FileInfo[] translatedFiles = translatedFilesPaths.Select(x => new FileInfo(x)).ToArray();

            foreach (var trFile in translatedFiles)
            {
                string originalFileName = trFile.Name.Replace("RU_", "").Replace(".json", "");
                string json = System.IO.File.ReadAllText(trFile.FullName);
                FileInfo originalFile = originalFiles.FirstOrDefault(o => o.Name == originalFileName);

                List<PhraseModel> phrases = JSON.DeserializeObject<List<PhraseModel>>(json);
                phrases = Translator.Filter(phrases, false);

                string originalText = File.ReadAllText(originalFile.FullName);

                foreach (var phrase in phrases)
                {
                    string text = phrase.TranslatedText.Replace("<", "").Replace(">", "");
                    originalText = originalText.Replace(phrase.OriginalText, text);
                }

                File.WriteAllText(originalFile.FullName, originalText);
            }
        }
    }
}
