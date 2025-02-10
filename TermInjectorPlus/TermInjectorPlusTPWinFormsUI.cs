using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace TermInjectorPlus
{
    [TranslationProviderWinFormsUi(Id = "Translation_Provider_Plug_inWinFormsUI",
                                   Name = "Translation_Provider_Plug_inWinFormsUI",
                                   Description = "Translation_Provider_Plug_inWinFormsUI")]
    class TermInjectorPlusTPWinFormsUI : ITranslationProviderWinFormsUI
    {
        #region ITranslationProviderWinFormsUI Members
        
        public ITranslationProvider[] Browse(
            IWin32Window owner, 
            LanguagePair[] languagePairs, 
            ITranslationProviderCredentialStore credentialStore)
        {
            
            TermInjectorPlusDialog dialog = new TermInjectorPlusDialog(
                new TermInjectorPlusTPOptions(),
                credentialStore,owner, 
                languagePairs);
            
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                TermInjectorPlusTP testProvider = 
                    new TermInjectorPlusTP(dialog.Options,credentialStore);
                return new ITranslationProvider[] { testProvider };
            }
            return null;
        }

        /// <summary>
        /// If the plug-in settings can be changed by the user,
        /// SDL Trados Studio will display a Settings button.
        /// By clicking this button, users raise the plug-in user interface,
        /// in which they can modify any applicable settings
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="translationProvider"></param>
        /// <param name="languagePairs"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>
        
        public bool Edit(IWin32Window owner, 
            ITranslationProvider translationProvider, 
            LanguagePair[] languagePairs, 
            ITranslationProviderCredentialStore credentialStore)
        {
            TermInjectorPlusTP editProvider = translationProvider as TermInjectorPlusTP;
            if (editProvider == null)
            {
                return false;
            }

            TermInjectorPlusDialog dialog = new TermInjectorPlusDialog(
                editProvider.Options,credentialStore,owner,languagePairs);
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                editProvider.Options = dialog.Options;
                return true;
            }

            return false;
        }
        

        public bool GetCredentialsFromUser(
            IWin32Window owner, 
            Uri translationProviderUri, 
            string translationProviderState, 
            ITranslationProviderCredentialStore credentialStore)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Used for displaying the plug-in info such as the plug-in name,
        /// tooltip, and icon.
        /// </summary>
        /// <param name="translationProviderUri"></param>
        /// <param name="translationProviderState"></param>
        /// <returns></returns>
        
        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderDisplayInfo info = new TranslationProviderDisplayInfo();
            info.Name = PluginResources.Plugin_NiceName;
            //info.TranslationProviderIcon = PluginResources.icon_icon;
            info.TooltipText = PluginResources.Plugin_Tooltip;

            //info.SearchResultImage = PluginResources.icon_symbol;

            return info;
        }
        

        public bool SupportsEditing
        {
            get { return true; }
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("URI not supported by the plug-in.");
            }
            return String.Equals(
                translationProviderUri.Scheme, 
                TermInjectorPlusTP.TermInjectorPlusTranslationProviderScheme, 
                StringComparison.CurrentCultureIgnoreCase);
        }

        public string TypeDescription
        {
            get { return PluginResources.Plugin_Description; }

        }

        public string TypeName
        {
            get { return PluginResources.Plugin_NiceName; }
        }

        #endregion
    }
}
