using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrivojPerevodDS3
{
    public class PhraseModel
    {
        public int Id { get; set; }
        public string OriginalText { get; set; }
        public string TranslatedText { get; set; }
        public int TranslationsCount { get; set; }
        public string CurrentLanguage { get; set; }
    }
}
