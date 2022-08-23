using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace TermInjectorPlus
{
    [TranslationProviderFactory(Id = "TermInjectorPlus_Plug_inFactory",
                                Name = "TermInjectorPlus_Plug_inFactory",
                                Description = "TermInjectorPlus_Plug_inFactory")]
    class TermInjectorPlusTPFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            return new TermInjectorPlusTP(new TermInjectorPlusTPOptions(translationProviderUri),credentialStore);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderInfo info = new TranslationProviderInfo();

            #region "TranslationMethod"
            info.TranslationMethod = TermInjectorPlusTPOptions.ProviderTranslationMethod;
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
            return String.Equals(translationProviderUri.Scheme, TermInjectorPlusTP.TermInjectorPlusTranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
