using Sdl.Core.PluginFramework;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmunger
{
    public class NestedTPFactory
    {
        public static ITranslationProvider InstantiateNestedTP(string nestedTPUriString, ITranslationProviderCredentialStore credentialStore)
        {
            var plugins = PluginManager.DefaultPluginRegistry.Plugins;
            var nestedUri = new Uri(nestedTPUriString);
            foreach (var plugin in plugins)
            {
                foreach (var extension in plugin.Extensions)
                {
                    if (extension.ExtensionPoint.ExtensionAttributeType.Name == "TranslationProviderFactoryAttribute")
                    {
                        var factory = (ITranslationProviderFactory)extension.CreateInstance();
                        if (factory.SupportsTranslationProviderUri(nestedUri))
                        {
                            return factory.CreateTranslationProvider(nestedUri, "", credentialStore);
                        }
                    }
                }
            }

            return null;
        }

    }
}
