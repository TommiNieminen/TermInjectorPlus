using Sdl.Core.PluginFramework;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Transmunger
{
    /// <summary>
    /// Interaction logic for SimpleMunger.xaml
    /// </summary>
    public partial class SimpleMunger : UserControl, INotifyPropertyChanged
    {

        private ObservableCollection<IPlugin> _translatorProviderPlugins;
        private System.Windows.Forms.IWin32Window owner;
        private ITranslationProviderCredentialStore credentialStore;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<IPlugin> TranslationProviderPlugins
        { get => _translatorProviderPlugins; set { _translatorProviderPlugins = value; NotifyPropertyChanged(); } }


        public SimpleMunger(System.Windows.Forms.IWin32Window owner, ITranslationProviderCredentialStore credentialStore)
        {
            InitializeComponent();
            this.DataContext = new ViewModel();
            this.TranslationProviderPlugins = new ObservableCollection<IPlugin>();
            this.GetTranslationProviders();
            this.owner = owner;
            this.credentialStore = credentialStore;
        }

        private void GetTranslationProviders()
        {
            var plugins = PluginManager.DefaultPluginRegistry.Plugins;
            foreach (var tpPlugin in plugins.Where(x => x.Extensions.Any(y => y.ExtensionPoint.ExtensionAttributeType.Name == "TranslationProviderFactoryAttribute")))
            {
                this.TranslationProviderPlugins.Add(tpPlugin);
            }
        }


        private void TpSettingsClick(object sender, RoutedEventArgs e)
        {
            var selectedPlugin = (IPlugin)this.TpComboBox.SelectedItem;
            //Some tp's have multiple UI's?
            var extension = selectedPlugin.Extensions.Single(x => x.ExtensionPoint.ExtensionAttributeType.Name == "TranslationProviderWinFormsUiAttribute");
            var winform = (ITranslationProviderWinFormsUI)extension.CreateInstance();
            
            //Need to pass the owner and credential store from Transmunger instance
            winform.Browse(this.owner, new LanguagePair[] { new LanguagePair("en-GB", "fi-FI") }, this.credentialStore);
        }
    }
}
