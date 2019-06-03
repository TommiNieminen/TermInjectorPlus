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
    [TranslationProviderFactory(Id = "Transmunger_Plug_inFactory",
                                Name = "Transmunger_Plug_inFactory",
                                Description = "Transmunger_Plug_inFactory")]
    class TransmungerTPFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            var uri = TransmungerTP.test_provider.Uri;
            Assembly test = Assembly.LoadFile(@"C:\Users\anonyymi_\AppData\Roaming\SDL\SDL Trados Studio\15\Plugins\Unpacked\MT Enhanced Trados Plugin\Sdl.Community.MtEnhancedProvider.dll");
            /*var interfacetypes = from type in test.GetTypes()
                          where typeof(ITranslationProviderFactory).IsAssignableFrom(type)
                          select type;
            ITranslationProviderFactory another = (ITranslationProviderFactory)Activator.CreateInstance(interfacetypes.Single());
            var test_provider = another.CreateTranslationProvider(_options.Uri, "", null);*/
            var interfacetypes = from type in test.GetTypes()
                                 where typeof(ITranslationProviderFactory).IsAssignableFrom(type)
                                 select type;
            ITranslationProviderFactory another = (ITranslationProviderFactory)Activator.CreateInstance(interfacetypes.Single());


            return new TransmungerTP(new TransmungerTPOptions(translationProviderUri), another.CreateTranslationProvider(TransmungerTP.test_provider.Uri, TransmungerTP.test_provider.SerializeState(), credentialStore),credentialStore);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderInfo info = new TranslationProviderInfo();

            #region "TranslationMethod"
            info.TranslationMethod = TransmungerTPOptions.ProviderTranslationMethod;
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
            return String.Equals(translationProviderUri.Scheme, TransmungerTP.TransmungerTranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
