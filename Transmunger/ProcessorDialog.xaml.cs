using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Transmunger
{
    /// <summary>
    /// Interaction logic for ProcessorDialog.xaml
    /// </summary>
    public partial class ProcessorDialog : Window
    {
        public ObservableCollection<RegexReplacementDef> RegexCollection { get; set; }
        internal RegexProcessor RegexProcessor { get; private set; }

        public ProcessorDialog()
        {
            
            InitializeComponent();
            this.DataContext = this;
            this.RegexCollection = new ObservableCollection<RegexReplacementDef>()
            {
                new RegexReplacementDef("test","test"),
                new RegexReplacementDef("test1","test1")
            };

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            this.RegexProcessor = new RegexProcessor(this.RegexCollection);
            this.DialogResult = true;
            this.Close();
        }
    }
}
