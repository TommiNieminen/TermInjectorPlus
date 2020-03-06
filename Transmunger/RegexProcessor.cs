using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Transmunger
{
    [DataContract]
    public class RegexProcessor : ITransProcessor, INotifyPropertyChanged
    {
        private ObservableCollection<RegexReplacementDef> regexCollection;
        private string _title;
        
        public RegexProcessor()
        {
            this.Title = "";
            this.RegexCollection = new ObservableCollection<RegexReplacementDef>();
        }

        public RegexProcessor(ObservableCollection<RegexReplacementDef> regexCollection, string name = "unnamed")
        {
            this.Title = name;
            this.RegexCollection = regexCollection;
        }

        [DataMember]
        public string Title { get => _title; set { _title = value; NotifyPropertyChanged(); } }

        [DataMember]
        public ObservableCollection<RegexReplacementDef> RegexCollection { get => regexCollection; set => regexCollection = value; }

        [DataMember]
        public string FileName { get => fileName; set => fileName = value; }

        private string fileName;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public TranslationUnit[] Transform(TranslationUnit[] input)
        {
            var plainText = input[0].TargetSegment.ToPlain();
            foreach (var regex in this.RegexCollection)
            {
                plainText = Regex.Replace(plainText, regex.Pattern, regex.Replacement);
            }

            Segment newTargetSegment = new Segment();
            newTargetSegment.Add(plainText);
            input[0].TargetSegment = newTargetSegment;
            return input;
        }

        public string Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer ser =
                new DataContractSerializer(typeof(RegexProcessor));
                ser.WriteObject(ms, this);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
        
    }
}