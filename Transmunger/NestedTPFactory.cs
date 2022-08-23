using Sdl.Core.PluginFramework;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermInjector2022
{
    public class NestedTPFactory
    {
        public static ITranslationProvider InstantiateNestedTP(string nestedTPUriString, ITranslationProviderCredentialStore credentialStore)
        {
            //TODO: USE TranslationProviderManager to create factory
            var tpFactories = TranslationProviderManager.GetTranslationProviderFactories();
            var nestedUri = new Uri(nestedTPUriString);
            
            foreach(var factory in tpFactories)
            {
                if (factory.SupportsTranslationProviderUri(nestedUri))
                {
                    return factory.CreateTranslationProvider(nestedUri, "", credentialStore);
                }
            }

            return null;
        }

    }
}
