using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Sdl.Core.PluginFramework;
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

        public TransmungerTP(TransmungerTPOptions options, ITranslationProviderCredentialStore credentialStore)
        {
            Options = options;
            this.CredentialStore = credentialStore;

            //Instantiate the nested translation provider, if present
            if (this.Options.nestedTranslationProvider != null || this.Options.nestedTranslationProvider.Length > 0)
            {
                this.InstantiateNestedTP();
            }

            //Deserialize pre- and post-processors
            this.Preprocessors = TransprocessorFactory.DeserializeProcessors(this.Options.preprocessors);

        }
        #endregion

        private void InstantiateNestedTP()
        {
            var plugins = PluginManager.DefaultPluginRegistry.Plugins;
            var nestedUri = new Uri(Options.nestedTranslationProvider);
            foreach (var plugin in plugins)
            {
                foreach (var extension in plugin.Extensions)
                {
                    if (extension.ExtensionPoint.ExtensionAttributeType.Name == "TranslationProviderFactoryAttribute")
                    {
                        var factory = (ITranslationProviderFactory)extension.CreateInstance();
                        if (factory.SupportsTranslationProviderUri(nestedUri))
                        {
                            this.NestedTP = factory.CreateTranslationProvider(nestedUri, "", this.CredentialStore);
                        }    
                    }
                }
            }
        }

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
        public ITranslationProvider NestedTP { get; private set; }
        public List<RegexProcessor> Preprocessors { get; private set; }

        #endregion
    }
}

