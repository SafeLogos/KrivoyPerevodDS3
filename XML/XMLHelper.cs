using KrivojPerevodDS3.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KrivojPerevodDS3.XML
{
    public static class XMLHelper
    {

        public static int Original = 0;
        public static int filtered = 0;
        public static XMLMainDocument[] ReadFromFolder(string path)
        {
            string[] files = Directory.GetFiles(path, "*.xml");
            XMLMainDocument[] documents = new XMLMainDocument[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                documents[i] = ReadXml(files[i]);
                Original += documents[i].entries.Length;
                filtered += Translator.Filter(documents[i].entries.Select(e => new PhraseModel() { Id = e.id, OriginalText = e.text }).ToList()).Count;
            }

            return documents;
        }

        public static XMLMainDocument ReadXml(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLMainDocument));

            StreamReader reader = new StreamReader(path);
            //reader.ReadToEnd();
            XMLMainDocument document = (XMLMainDocument)xmlSerializer.Deserialize(reader);
            FileInfo info = new FileInfo(path);
            document.OriginalFileName = info.Name;
            document.ConvertedFileName = $"RU_{info.Name}";
            reader.Close();

            return document;
        }

        public static void WriteXml(string path, XMLMainDocument data)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLMainDocument));
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, data);
            }
        }

        public static void WriteXml(string path, List<PhraseModel> phrases)
        {
            XMLMainDocument data = new XMLMainDocument()
            {
                bigendian = "False",
                compression = "None",
                version = "DarkSouls3",
                entries = phrases.Select(p => new XMLPhrase() { id = p.Id, text = p.TranslatedText }).ToArray()
            };
            WriteXml(path, data);
        }
    }
}
