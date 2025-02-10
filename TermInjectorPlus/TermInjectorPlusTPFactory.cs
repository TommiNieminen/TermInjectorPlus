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
        
        public ITranslationProvider CreateTranslationProvider(
            Uri translationProviderUri, 
            string translationProviderState, 
            ITranslationProviderCredentialStore credentialStore)
        {
            return new TermInjectorPlusTP(new TermInjectorPlusTPOptions(
                translationProviderUri),credentialStore);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(
            Uri translationProviderUri, 
            string translationProviderState)
        {
            TranslationProviderInfo info = new TranslationProviderInfo();

            info.TranslationMethod = TermInjectorPlusTPOptions.ProviderTranslationMethod;
            
            info.Name = PluginResources.Plugin_NiceName;
            
            return info;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("Translation provider URI not supported.");
            }
            return String.Equals(
                translationProviderUri.Scheme, 
                TermInjectorPlusTP.TermInjectorPlusTranslationProviderScheme, 
                StringComparison.OrdinalIgnoreCase);
        }

    }
}
