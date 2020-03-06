using System;
using System.Runtime.Serialization;

namespace Transmunger
{
    [DataContract]
    public class RegexReplacementDef
    {
        [DataMember]
        public string Pattern { get; set; }

        [DataMember]
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