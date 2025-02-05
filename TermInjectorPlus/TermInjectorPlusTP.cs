using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Sdl.Core.PluginFramework;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Serilog;

namespace TermInjectorPlus
{
    class TermInjectorPlusTP : ITranslationProvider
    {

        public static readonly string TermInjectorPlusTranslationProviderScheme = "terminjectorplus";

        #region "TranslationOptions"
        public TermInjectorPlusTPOptions Options
        {
            get;
            set;
        }

        private void SetupLogging()
        {
            var logDir = HelperFunctions.GetLocalAppDataPath(TermInjectorPlusSettings.Default.LogDir);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(Path.Combine(logDir, "terminjectorplus.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public TermInjectorPlusTP(TermInjectorPlusTPOptions options,
            ITranslationProviderCredentialStore credentialStore)
        {
            this.SetupLogging();
            
            Options = options;
            this.CredentialStore = credentialStore;

            Guid configGuid;
            //Check if the options contain a guid for an existing terminjector configuration
            if (Guid.TryParse(options.configGuid, out configGuid))
            {
                var pipelineConfigDir = new DirectoryInfo(
                    HelperFunctions.GetLocalAppDataPath(TermInjectorPlusSettings.Default.ConfigDir));
                var configInOptionFileInfo = pipelineConfigDir.GetFiles($"{configGuid}.yml").FirstOrDefault();
                if (configInOptionFileInfo != null)
                {
                    this.TerminjectorPipeline = TermInjectorPipeline.CreateFromFile(configInOptionFileInfo, credentialStore);
                }
            }

        }
        #endregion


        #region ITranslationProvider Members

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            return new TermInjectorPlusTPLanguageDirection(this,languageDirection);
        }

        public bool IsReadOnly
        {
            get { return false; }
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
            get { return true; }
        }

        public bool SupportsWordCounts
        {
            get { return false; }
        }

        public TranslationMethod TranslationMethod
        {
            get { return TermInjectorPlusTPOptions.ProviderTranslationMethod; }
        }

        public Uri Uri
        {
            get { return Options.Uri; }
        }

        public ITranslationProviderCredentialStore CredentialStore { get; }
        public TermInjectorPipeline TerminjectorPipeline { get; private set; }
        public ITranslationProvider NestedTP { get; private set; }

        #endregion
    }
}

