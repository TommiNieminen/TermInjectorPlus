﻿using System;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Transmunger
{
    /// <summary>
    /// This class is used to hold the provider plug-in settings. 
    /// All settings are automatically stored in a URI.
    /// </summary>
    public class TransmungerTPOptions
    {
        #region "TranslationMethod"
        public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.Other;
        #endregion

        #region "TranslationProviderUriBuilder"
        TranslationProviderUriBuilder _uriBuilder;        

        public TransmungerTPOptions()
        {
            _uriBuilder = new TranslationProviderUriBuilder(TransmungerTP.TransmungerTranslationProviderScheme);
        }

        public TransmungerTPOptions(Uri uri)
        {
            _uriBuilder = new TranslationProviderUriBuilder(uri);
        }
        #endregion
        
        public string languageDirection
        {
            get { return GetStringParameter("languageDirection"); }
            set { SetStringParameter("languageDirection", value); }
        }
       
        public string nestedTranslationProvider
        {
            get { return GetStringParameter("nestedTranslationProvider"); }
            set { SetStringParameter("nestedTranslationProvider", value); }
        }

        public string preprocessors
        {
            get { return GetStringParameter("preprocessors"); }
            set { SetStringParameter("preprocessors", value); }
        }

        public string postprocessors
        {
            get { return GetStringParameter("postprocessors"); }
            set { SetStringParameter("postprocessors", value); }
        }

        public string port
        {
            get { return GetStringParameter("port"); }
            set { SetStringParameter("port", value); }
        }
        public string client
        {
            get { return GetStringParameter("client"); }
            set { SetStringParameter("client", value); }
        }
        public string subject
        {
            get { return GetStringParameter("subject"); }
            set { SetStringParameter("subject", value); }
        }

        public string otherFeatures
        {
            get { return GetStringParameter("other_features"); }
            set { SetStringParameter("other_features", value); }
        }

        public string languageDirectionSource
        {
            get { return GetStringParameter("languageDirectionSource"); }
            set { SetStringParameter("languageDirectionSource", value); }
        }
        public string languageDirectionTarget
        {
            get { return GetStringParameter("languageDirectionTarget"); }
            set { SetStringParameter("languageDirectionTarget", value); }
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
