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
        [XmlElement]
        public string OriginalFileName;

        [XmlElement]
        public string ConvertedFileName;

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
