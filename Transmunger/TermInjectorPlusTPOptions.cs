using System;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace TermInjectorPlus
{
    /// <summary>
    /// This class is used to hold the provider plug-in settings. 
    /// All settings are automatically stored in a URI.
    /// </summary>
    public class TermInjectorPlusTPOptions
    {
        #region "TranslationMethod"
        public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.Mixed;
        #endregion

        #region "TranslationProviderUriBuilder"
        TranslationProviderUriBuilder _uriBuilder;        

        public TermInjectorPlusTPOptions()
        {
            _uriBuilder = new TranslationProviderUriBuilder(TermInjectorPlusTP.TermInjectorPlusTranslationProviderScheme);
        }

        public TermInjectorPlusTPOptions(Uri uri)
        {
            _uriBuilder = new TranslationProviderUriBuilder(uri);
        }
        #endregion
        

        public string configGuid
        {
            get { return GetStringParameter("configGuid"); }
            set { SetStringParameter("configGuid", value); }
        }

        #region "SetStringParameter"
        private void SetStringParameter(string p, string value)
        {
            _uriBuilder[p] = value;
        }
        #endregion

        #region "GetStringParameter"
        private string GetStringParameter(string p)
        {
            string paramString = _uriBuilder[p];
            return paramString;
        }
        #endregion

        #region "Uri"
        public Uri Uri
        {            
            get
            {
                return _uriBuilder.Uri;                
            }
        }
        #endregion
    }
}
