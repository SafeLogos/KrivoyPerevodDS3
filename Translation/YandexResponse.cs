using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrivojPerevodDS3.Translation
{
    public class YandexResponse
    {
        public List<Translations> Translations { get; set; }
    }

    public class Translations 
    {
        public string Text { get; set; }
        public string DetectedLanguageCode { get; set; }
    }

}
