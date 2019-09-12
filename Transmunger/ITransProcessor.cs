using Sdl.LanguagePlatform.TranslationMemory;

namespace Transmunger
{
    interface ITransProcessor
    {
        string Title { get; set; }
        TranslationUnit[] Munge(TranslationUnit[] input);
    }
}
