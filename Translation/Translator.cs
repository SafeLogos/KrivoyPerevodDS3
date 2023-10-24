using KrivojPerevodDS3.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using JSON = Newtonsoft.Json.JsonConvert;

namespace KrivojPerevodDS3.Translation
{
    public static class Translator
    {
        const string forbiddenSymbols = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM&<>[]()%";
        const string _folder = "b1gk2v7ejpmn4ch4adg5";
        const string _apiURL = "https://translate.api.cloud.yandex.net/translate/v2/translate";
        const string _audioApiUrl = "https://tts.api.cloud.yandex.net/speech/v1/tts:synthesize?folderId=b1gk2v7ejpmn4ch4adg5&text={0}&voice={1}&format=mp3";
        private static string[] _languages = new string[]
        {
            "gu",
            "ar",
            "tt",
            "te",

            "fi",
            "uz",
            "si",

            "fa",
            "mn",

            "ru"
        };

        private static Dictionary<string, string> _voices = new Dictionary<string, string>()
        {
            //30
            { "012",  "filipp" }, //40
            { "023", "jane" },
            { "024",  "filipp" },


            //31
            { "005",  "filipp" }, //40
            { "008",  "jane" }, //40
            { "014",  "filipp" }, //40
            { "016",  "jane" }, //40
            { "017",  "filipp" }, //40
            { "021",  "filipp" },// 35 37 39
            { "027",  "filipp" },
            { "031",  "filipp" },
            { "531",  "filipp" },

            //33
            { "007",  "filipp" }, //40
            { "013",  "filipp" }, //40
            { "019",  "jane" }, // в 34 37 38 40

            //34
            { "022",  "filipp" },

            //35
            { "009",  "filipp" }, //40
            { "020",  "filipp" }, //40
            { "051",  "filipp" }, //38 40 45

            //37
            { "006",  "jane" },
            { "030",  "jane" }, //41

            //39
            { "010",  "filipp" },
            { "015",  "jane" }, //40

            //40
            { "002",  "jane" }, //41
            { "003",  "filipp" },
            { "004",  "jane" }, //45
            { "011",  "jane" },
            { "029",  "jane" },


            //41
            { "001",  "jane" },

            //45
            { "052",  "filipp" },
            { "053",  "jane" },
            { "054",  "filipp" },
            { "055",  "jane" },
            { "057",  "jane" },
            { "059",  "filipp" },
            { "072",  "jane" },

            //50
            { "073",  "filipp" }, //51
            { "076",  "jane" },

            //51
            { "071",  "filipp" },
            { "074",  "jane" },
            { "075",  "filipp" },
            { "077",  "filipp" },
            { "078",  "filipp" }
        };

        private static HttpClient _httpClient;
        public static void Initialize()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer t1.9euelZqbk4qexpiZlpSPzMiNz8_Myu3rnpWamoyTkJrLlpCQipzJmJnHmI_l8_clFyVW-e9QLSxP_N3z92VFIlb571AtLE_8zef1656VmoqWx4yaz87Ml8_MiZiNkM2V7_zF656VmoqWx4yaz87Ml8_MiZiNkM2V.CIVipsfCA47Y42MKn2o1CFRAy-S6v3yacirZQWqPOPeK8wFKwHHe5ezrC52c73Ewzt5tnD6cYditbVSVcAnZAg");
        }

        public static List<PhraseModel> Translate(List<XMLPhrase> data)
        {
            List<PhraseModel> result = data.Select(d => new PhraseModel()
            {
                Id = d.id,
                OriginalText = d.text,
                TranslatedText = null,
                CurrentLanguage = "ru",
                TranslationsCount = 0
            }).ToList();

            List<PhraseModel> filtered = Filter(result);

            if (!filtered.Any())
                return new List<PhraseModel>();

            int cnt = 0;
            foreach (string language in _languages)
            {
                Console.WriteLine($"Start [{filtered.FirstOrDefault().CurrentLanguage}] -> [{language}]");
                int lastPhraseIndex = 0;

                List<Translations> translations = new List<Translations>();
                int phrasesDone = 0;

                while (lastPhraseIndex < filtered.Count)
                {
                    int charactersCount = 0;
                    List<string> phrases = new List<string>();
                    for (int i = lastPhraseIndex; i < filtered.Count; i++)
                    {
                        string text = string.IsNullOrEmpty(filtered[i].TranslatedText) ? filtered[i].OriginalText : filtered[i].TranslatedText;
                        phrases.Add(text);
                        charactersCount += text.Length;
                        lastPhraseIndex++;
                        if (charactersCount >= 8200)
                            break;
                    }

                    phrasesDone += phrases.Count;

                    YandexRequest request = new YandexRequest()
                    {
                        folderId = _folder,
                        texts = phrases,
                        targetLanguageCode = language
                    };
                    string json = JSON.SerializeObject(request);

                    HttpResponseMessage response = _httpClient.PostAsync(_apiURL, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    Console.Write($"({phrasesDone}/{filtered.Count})   ");
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        if (language == "ru")
                            throw new Exception($"HTTP ERROR {response.StatusCode}");
                        else
                            Console.WriteLine($"ERROR ON SENDING {language} | HTTP CODE {response.StatusCode}");
                    }

                    string resJson = response.Content.ReadAsStringAsync().Result;
                    YandexResponse yandexResponse = JSON.DeserializeObject<YandexResponse>(resJson);
                    translations.AddRange(yandexResponse.Translations);

                    Thread.Sleep(250);
                }

                if(translations.Count() != filtered.Count)
                {
                    Console.WriteLine("SOMETHING WENT WRONG");
                }

                for (int i = 0; i < filtered.Count; i++)
                {
                    filtered[i].TranslatedText = translations[i].Text;
                    filtered[i].CurrentLanguage = language;
                    filtered[i].TranslationsCount++;
                }

                Console.WriteLine($"completed translations {++cnt}/{_languages.Count()} [{language}]");
            }

            foreach (var item in result)
            {
                PhraseModel? model = filtered.FirstOrDefault(f => f.OriginalText == item.OriginalText);
                if (model == null)
                    item.TranslatedText = item.OriginalText;
                else
                {
                    item.TranslatedText = model.TranslatedText;
                    item.TranslationsCount = model.TranslationsCount;
                }
            }


            return result;
        }

        public static List<PhraseModel> Filter(List<PhraseModel> list, bool removeTranslated = true)
        {
            List<PhraseModel> result = list.Where(e =>
            !string.IsNullOrEmpty(e.OriginalText) &&
            !string.IsNullOrEmpty(e.OriginalText.Trim())).ToList();

            result = result.Where(e => !e.OriginalText.Any(s => forbiddenSymbols.Contains(s))).ToList();

            result = result.GroupBy(e => e.OriginalText).Select(e =>
                new PhraseModel()
                {
                    Id = e.First().Id,
                    OriginalText = e.Key,
                    TranslatedText = removeTranslated ? null : e.First().TranslatedText,
                    CurrentLanguage = "ru",
                    TranslationsCount = 0
                }).ToList();

            return result;
        }

        public static void DownloadAllAudio()
        {
            string filePath = @"C:\Games\GOTOVO\JSON\Menu\RU_会話_dlc1.fmg.xml.json";

            string audioFolderPath = @"C:\Games\GOTOTVO\Audio";

            string json = File.ReadAllText(filePath);
            List<PhraseModel> phrases = JSON.DeserializeObject<List<PhraseModel>>(json);
            phrases = Filter(phrases, false);
            //phrases = phrases.Where(p => p.Id == 11000200).ToList();
            int cnt = 0;
            foreach (var item in phrases)
            {
                Console.Write($"{item.Id} Started. ");
                Console.WriteLine($"FINISHED ({++cnt}/{phrases.Count})");
                DownloadAudio(item, audioFolderPath);
            }
        }

        public static void DownloadAudio(PhraseModel phrase, string folderPath) 
        {

            string fileName = $"{phrase.Id}";
            while (fileName.Length < 9)
                fileName = "0" + fileName;

            string firstDigits = new string(fileName.Take(3).ToArray());
            string voice = "filipp";
            if (_voices.ContainsKey(firstDigits))
                voice = _voices[firstDigits];
            else
                return;

            fileName = $"v{fileName}.mp3";

            string path = string.Format(_audioApiUrl, phrase.TranslatedText.Replace(";", ""), voice);
            string filePath = $"{folderPath}\\" + fileName;

            if (File.Exists(filePath))
                return;

            HttpResponseMessage msg = _httpClient.GetAsync(path).Result;
            if (msg.IsSuccessStatusCode)
            {
                
                HttpContent content = msg.Content;
                Stream contentStream = content.ReadAsStreamAsync().Result;
                

                using (FileStream stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    contentStream.CopyTo(stream);
                }
            }
            else
                Console.Write($"Error {msg.StatusCode}");
        }
    }
}
