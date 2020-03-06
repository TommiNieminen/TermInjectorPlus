using Sdl.LanguagePlatform.TranslationMemory;

namespace Transmunger
{
    public interface ITransProcessor
    {
        string Title { get; set; }
        string FileName { get; set; }
        TranslationUnit[] Transform(TranslationUnit[] input);
        string Serialize();
    }
}
