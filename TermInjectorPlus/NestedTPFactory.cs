using Sdl.Core.PluginFramework;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TermInjectorPlus
{
    internal class NestedTPFactory
    {
        public static ITranslationProvider InstantiateNestedTP(
            string nestedTPUriString, 
            ITranslationProviderCredentialStore credentialStore, 
            Sdl.LanguagePlatform.Core.LanguagePair[] languagePairs)
        {
            IList<ITranslationProviderFactory> tpFactories;
            try
            {
                tpFactories = TranslationProviderManager.GetTranslationProviderFactories();
            }
            catch
            {
                tpFactories = new List<ITranslationProviderFactory>();
            }

            var nestedUri = new Uri(nestedTPUriString);
            
            foreach(var factory in tpFactories)
            {
                if (factory.SupportsTranslationProviderUri(nestedUri))
                {
                    var factoryInfo = factory.GetTranslationProviderInfo(nestedUri,"");
                    //TODO: this hangs Studio if e.g. the file referred to in file TM does not exist
                    //It seems to do a endless loop of this part of the code, so setting e.g. a static flag
                    //for URI failing might work here
                    ITranslationProvider nestedProvider = null;
                    try
                    {
                        nestedProvider = factory.CreateTranslationProvider(nestedUri, "", credentialStore);
                        if (!languagePairs.Any(nestedProvider.SupportsLanguageDirection))
                        {
                            MessageBox.Show($"Selected translation provider does not support any of the specified language directions.");
                            nestedProvider = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        nestedProvider = null;
                    }
                    return nestedProvider;
                }
            }

            return null;
        }
        
    }
}
