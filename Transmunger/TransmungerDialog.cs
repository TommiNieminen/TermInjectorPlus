using Sdl.Core.PluginFramework;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xaml;

namespace Transmunger
{
    public partial class TransmungerDialog : Form
    {




        #region "ProviderConfDialog"

        public TransmungerDialog(TransmungerTPOptions translationOptions) : this(translationOptions,null,null)
        {
        }

        public TransmungerDialog(TransmungerTPOptions translationOptions, ITranslationProviderCredentialStore credentialStore, IWin32Window owner)
        {
            Options = translationOptions;
            InitializeComponent();
            UpdateDialog();
            this.wpfHost.Child = new SimpleMunger(owner, credentialStore);
            

            /*var interfacetypes = from type in test.GetTypes()
                          where typeof(ITranslationProviderFactory).IsAssignableFrom(type)
                          select type;
            ITranslationProviderFactory another = (ITranslationProviderFactory)Activator.CreateInstance(interfacetypes.Single());
            var test_provider = another.CreateTranslationProvider(_options.Uri, "", null);*/

            //TODO: This appears to work, next check if e.g. MT Enhanced can be accessed similarly.
            /*Assembly test = Assembly.LoadFile(@"C:\Users\anonyymi_\AppData\Roaming\SDL\SDL Trados Studio\15\Plugins\Unpacked\MT Enhanced Trados Plugin\Sdl.Community.MtEnhancedProvider.dll");

            var interfacetypes = from type in test.GetTypes()
                                 where typeof(ITranslationProviderWinFormsUI).IsAssignableFrom(type)
                                 select type;
            ITranslationProviderWinFormsUI another = (ITranslationProviderWinFormsUI)Activator.CreateInstance(interfacetypes.Single());
            ITranslationProvider[] providers = another.Browse(this, new LanguagePair[] { new LanguagePair("en-GB", "fi-FI") }, credentialStore);
            this.Test_provider = providers.Single();*/
        }

        public TransmungerTPOptions Options
        {
            get;
            set;
        }
        public ITranslationProvider Test_provider { get; }
        #endregion


        #region "UpdateDialog"
        private void UpdateDialog()
        {
            //TODO: add code to update the dialog, if necessary, from the options read in the plugin URI string
        }
        #endregion

        private void Ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
