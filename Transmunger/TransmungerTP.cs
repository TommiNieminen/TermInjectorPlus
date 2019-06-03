using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Transmunger
{
    class TransmungerTP : ITranslationProvider
    {

        public static readonly string TransmungerTranslationProviderScheme = "transmunger";

        #region "TranslationOptions"
        public TransmungerTPOptions Options
        {
            get;
            set;
        }

        //Make this into a dictionary of instantiated translation providers
        public static ITranslationProvider test_provider;

        public TransmungerTP(TransmungerTPOptions options, ITranslationProvider test_provider, ITranslationProviderCredentialStore credentialStore)
        {
            Options = options;
            if (test_provider != null)
            {
                TransmungerTP.test_provider = test_provider;
            }
            this.CredentialStore = credentialStore;
        }
        #endregion

        #region ITranslationProvider Members

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            return new TransmungerTPLanguageDirection(this,languageDirection);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void LoadState(string translationProviderState)
        {

        }

        public string Name
        {
            get { return PluginResources.Plugin_Name; }
        }

        public void RefreshStatusInfo()
        {
            
        }

        public string SerializeState()
        {
            return null;
        }

        public ProviderStatusInfo StatusInfo
        {
            get { return new ProviderStatusInfo(true, PluginResources.Plugin_NiceName); }
        }

        public bool SupportsConcordanceSearch
        {
            get { return true; }
        }

        public bool SupportsDocumentSearches
        {
            get { return true; }
        }

        public bool SupportsFilters
        {
            get { return true; }
        }

        public bool SupportsFuzzySearch
        {
            get { return false; }
        }

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            return true;
        }

        public bool SupportsMultipleResults
        {
            get { return true; }
        }

        public bool SupportsPenalties
        {
            get { return true; }
        }

        public bool SupportsPlaceables
        {
            get { return true; }
        }

        public bool SupportsScoring
        {
            get { return true; }
        }

        public bool SupportsSearchForTranslationUnits
        {
            get { return true; }
        }

        public bool SupportsSourceConcordanceSearch
        {
            get { return true; }
        }

        public bool SupportsStructureContext
        {
            get { return true; }
        }

        public bool SupportsTaggedInput
        {
            get { return true; }
        }

        public bool SupportsTargetConcordanceSearch
        {
            get { return true; }
        }

        public bool SupportsTranslation
        {
            get { return true; }
        }

        public bool SupportsUpdate
        {
            get { return false; }
        }

        public bool SupportsWordCounts
        {
            get { return false; }
        }

        public TranslationMethod TranslationMethod
        {
            get { return TransmungerTPOptions.ProviderTranslationMethod; }
        }

        public Uri Uri
        {
            get { return Options.Uri; }
        }

        public ITranslationProviderCredentialStore CredentialStore { get; }

        #endregion
    }
}

