using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace TermInjector2022
{
    [TranslationProviderFactory(Id = "TermInjector2022_Plug_inFactory",
                                Name = "TermInjector2022_Plug_inFactory",
                                Description = "TermInjector2022_Plug_inFactory")]
    class TermInjector2022TPFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            return new TermInjector2022TP(new TermInjector2022TPOptions(translationProviderUri),credentialStore);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderInfo info = new TranslationProviderInfo();

            #region "TranslationMethod"
            info.TranslationMethod = TermInjector2022TPOptions.ProviderTranslationMethod;
            #endregion

            #region "Name"
            info.Name = PluginResources.Plugin_NiceName;
            #endregion

            return info;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("Translation provider URI not supported.");
            }
            return String.Equals(translationProviderUri.Scheme, TermInjector2022TP.TermInjector2022TranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
