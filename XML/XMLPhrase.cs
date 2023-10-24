using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KrivojPerevodDS3.XML
{
    public class XMLPhrase
    {
        [XmlAttribute]
        public int id { get; set; }

        [XmlText]
        public string text { get; set; }
    }
}
