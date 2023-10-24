using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrivojPerevodDS3.Translation
{
    public class YandexRequest
    {
        public string folderId { get; set; }
        public List<string> texts { get; set; }
        public string targetLanguageCode { get; set; }
    }
}
