using Sdl.Core.PluginFramework;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xaml;

namespace TermInjectorPlus
{
    public partial class TermInjectorPlusDialog : Form
    {
        private TermInjectorPipelineView termInjectorView;


        #region "ProviderConfDialog"

        //This is used for testing the dialog outside of Trados Studio (quicker to do)
        public TermInjectorPlusDialog(TermInjectorPlusTPOptions translationOptions) : this(translationOptions,null,null,null,true)
        {

        }

        public TermInjectorPlusDialog(
            TermInjectorPlusTPOptions translationOptions, 
            ITranslationProviderCredentialStore credentialStore, 
            IWin32Window owner, 
            LanguagePair[] languagePairs,
            bool testDialog=false)
        {
            Options = translationOptions;
            InitializeComponent();
            UpdateDialog();
            
            this.termInjectorView = new TermInjectorPipelineView(owner, credentialStore, Options, languagePairs, testDialog);
            this.termInjectorView.SaveSettingsButton.Click += SaveSettingsButton_Click;
            this.termInjectorView.CancelButton.Click += CancelButton_Click;
            this.wpfHost.Child = this.termInjectorView;
            //TODO: add event handlers for the child Save and Cancel buttons to implement window closing
            //termInjectorView.SaveButton.Clicked? +=
            
        }

        public TermInjectorPlusTPOptions Options
        {
            get;
            set;
        }
        
        #endregion


        #region "UpdateDialog"
        private void UpdateDialog()
        {
            //TODO: add code to update the dialog, if necessary, from the options read in the plugin URI string
        }
        #endregion

        //TODO: replace this with WPF child Save and Cancel handler
        /*private void Ok_Click(object sender, EventArgs e)
        {
            if (this.viewModel.TranslationProvider != null)
            {
                this.Options.nestedTranslationProvider = this.viewModel.TranslationProvider.Uri.ToString();
            }

            this.Options.preprocessors = String.Join("-",this.viewModel.Preprocessors.Select(x => x.Serialize()));
            this.Options.postprocessors = String.Join("-", this.viewModel.Postprocessors.Select(x => x.Serialize()));

            this.DialogResult = DialogResult.OK;
            this.Close();
        }*/

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Options = new TermInjectorPlusTPOptions();
            this.termInjectorView.SaveSettings();
            this.Options.configGuid = this.termInjectorView.TermInjectorConfig.PipelineGuid;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
