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
using System.Windows.Forms;
using System.Xaml;

namespace Transmunger
{
    public partial class TransmungerDialog : Form
    {
        private ViewModel viewModel;

        
        #region "ProviderConfDialog"

        public TransmungerDialog(TransmungerTPOptions translationOptions) : this(translationOptions,null,null)
        {

        }

        public TransmungerDialog(TransmungerTPOptions translationOptions, ITranslationProviderCredentialStore credentialStore, IWin32Window owner)
        {
            Options = translationOptions;
            InitializeComponent();
            UpdateDialog();
            this.viewModel = new ViewModel();
            this.wpfHost.Child = new SimpleMunger(owner, credentialStore,viewModel);
            this.viewModel.TranslationProvider = NestedTPFactory.InstantiateNestedTP(translationOptions.nestedTranslationProvider, credentialStore);
            this.viewModel.Preprocessors = new ObservableCollection<ITransProcessor>(TransprocessorFactory.DeserializeProcessors(translationOptions.preprocessors));
            this.viewModel.Postprocessors = new ObservableCollection<ITransProcessor>(TransprocessorFactory.DeserializeProcessors(translationOptions.postprocessors));
        }

        public TransmungerTPOptions Options
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

        private void Ok_Click(object sender, EventArgs e)
        {
            if (this.viewModel.TranslationProvider != null)
            {
                this.Options.nestedTranslationProvider = this.viewModel.TranslationProvider.Uri.ToString();
            }

            this.Options.preprocessors = String.Join("-",this.viewModel.Preprocessors.Select(x => x.Serialize()));
            this.Options.postprocessors = String.Join("-", this.viewModel.Postprocessors.Select(x => x.Serialize()));

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
    }
}
