using KrivojPerevodDS3.Translation;
using KrivojPerevodDS3.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrivojPerevodDS3
{
    public class ContentGenerator
    {
        const string _itemsToConvertFolderPath = @"..\..\..\FilesToConvert\item_dlc1-msgbnd-dcx\msg\rusRU\64bit";
        const string _itemsConvertedToXMLFolderPath = @"..\..\..\ConvertedFiles\XML\Items";
        const string _itemsConvertedToJSONFolderPath = @"..\..\..\ConvertedFiles\JSON\Items";

        const string _menuToConvertFolderPath = @"..\..\..\FilesToConvert\menu_dlc1-msgbnd-dcx\msg\rusRU\64bit";
        const string _menuConvertedToXMLFolderPath = @"..\..\..\ConvertedFiles\XML\Menu";
        const string _menuConvertedToJSONFolderPath = @"..\..\..\ConvertedFiles\JSON\Menu";



        public static void StartTextItemsGeneration()
        {
            XMLMainDocument[] documents = XMLHelper.ReadFromFolder(_itemsToConvertFolderPath);
            int cnt = 0;
            foreach (var item in documents)
            {
                List<PhraseModel> phrases = Translator.Translate(item.entries.ToList());
                XMLHelper.WriteXml($"{_itemsConvertedToXMLFolderPath}\\{item.ConvertedFileName}", item);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(phrases);

                string jsonFilePath = $"{_itemsConvertedToJSONFolderPath}\\{item.ConvertedFileName}.json";

                if(System.IO.File.Exists(jsonFilePath))
                    System.IO.File.Delete(jsonFilePath);

                System.IO.File.AppendAllLines(jsonFilePath, new string[] { json });
                Console.WriteLine($"Items completed {++cnt}/{documents.Length}");
            }

            Console.WriteLine("Original count: " + XMLHelper.Original);
            Console.WriteLine("Filtered count: " + XMLHelper.filtered);
        }

        public static void StartMenuItemsGeneration()
        {
            XMLMainDocument[] documents = XMLHelper.ReadFromFolder(_menuToConvertFolderPath);
            int cnt = 0;
            foreach (var item in documents)
            {
                List<PhraseModel> phrases = Translator.Translate(item.entries.ToList());
                XMLHelper.WriteXml($"{_menuConvertedToXMLFolderPath}\\{item.ConvertedFileName}", item);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(phrases);

                string jsonFilePath = $"{_menuConvertedToJSONFolderPath}\\{item.ConvertedFileName}.json";

                if (System.IO.File.Exists(jsonFilePath))
                    System.IO.File.Delete(jsonFilePath);

                System.IO.File.AppendAllLines(jsonFilePath, new string[] { json });
                Console.WriteLine($"Items completed {++cnt}/{documents.Length}");
            }

            Console.WriteLine("Original count: " + XMLHelper.Original);
            Console.WriteLine("Filtered count: " + XMLHelper.filtered);
        }
    }
}
