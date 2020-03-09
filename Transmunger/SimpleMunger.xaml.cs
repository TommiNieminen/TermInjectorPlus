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

        private ObservableCollection<IExtension> _translatorProviderPlugins;
        private System.Windows.Forms.IWin32Window owner;
        private ITranslationProviderCredentialStore credentialStore;
        private ViewModel viewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<IExtension> TranslationProviderPluginUis
        { get => _translatorProviderPlugins; set { _translatorProviderPlugins = value; NotifyPropertyChanged(); } }


        public SimpleMunger(System.Windows.Forms.IWin32Window owner, ITranslationProviderCredentialStore credentialStore, ViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.DataContext = this.viewModel;
            this.TranslationProviderPluginUis = new ObservableCollection<IExtension>();
            this.GetTranslationProvidersUis();
            this.owner = owner;
            this.credentialStore = credentialStore;
        }

        private void GetTranslationProvidersUis()
        {
            try
            {
                var plugins = PluginManager.DefaultPluginRegistry.Plugins;

                foreach (var tpPlugin in plugins)
                {
                    foreach (var extension in tpPlugin.Extensions)
                    {
                        if (extension.ExtensionPoint.ExtensionAttributeType.Name == "TranslationProviderWinFormsUiAttribute")
                        {
                            this.TranslationProviderPluginUis.Add(extension);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        private void TpSettingsClick(object sender, RoutedEventArgs e)
        {
            var selectedUiExtension = (IExtension)this.TpComboBox.SelectedItem;
            
            var winform = (ITranslationProviderWinFormsUI)selectedUiExtension.CreateInstance();

            this.viewModel.TranslationProvider = winform.Browse(this.owner, new LanguagePair[] {}, this.credentialStore).Single();
        }
    }
}
