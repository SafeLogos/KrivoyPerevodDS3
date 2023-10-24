using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KrivojPerevodDS3.XML
{
    [System.Xml.Serialization.XmlRoot("fmg")]
    public class XMLMainDocument
    {
        public string OriginalFileName { get; set; }
        public string ConvertedFileName { get; set; }

        [XmlElement]
        public string compression { get; set; }

        [XmlElement]
        public string version { get; set; }

        [XmlElement]
        public string bigendian { get; set; }


        [XmlArrayItem(typeof(XMLPhrase), ElementName = "text")]
        public XMLPhrase[] entries { get; set; }
    }
}
