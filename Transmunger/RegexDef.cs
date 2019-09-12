namespace Transmunger
{
    
    public class RegexReplacementDef
    {
        public string Pattern { get; set; }
        public string Replacement { get; set; }


        public RegexReplacementDef()
        {

        }

        public RegexReplacementDef(string pattern, string replacement)
        {
            this.Pattern = pattern;
            this.Replacement = replacement;
        }
    }
}