using System.Collections.ObjectModel;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Transmunger
{
    internal class RegexProcessor : ITransProcessor
    {
        private ObservableCollection<RegexReplacementDef> regexCollection;

        public RegexProcessor()
        {
        }

        public RegexProcessor(ObservableCollection<RegexReplacementDef> regexCollection, string name="unnamed")
        {
            this.Title = name;
            this.RegexCollection = regexCollection;
        }

        public string Title { get; set; }

        public ObservableCollection<RegexReplacementDef> RegexCollection { get => regexCollection; set => regexCollection = value; }

        public TranslationUnit[] Munge(TranslationUnit[] input)
        {
            throw new System.NotImplementedException();
        }
    }
}