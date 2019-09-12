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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Transmunger
{
    /// <summary>
    /// Interaction logic for ProcessorDialog.xaml
    /// </summary>
    public partial class ProcessorDialog : Window, INotifyPropertyChanged
    {
        private RegexProcessor _regexProcessor;

        //public ObservableCollection<RegexReplacementDef> RegexCollection { get; set; }
        public RegexProcessor RegexProcessor { get => _regexProcessor; set { _regexProcessor = value; NotifyPropertyChanged(); } }

        public ProcessorDialog() : this(new RegexProcessor())
        {

        }

        public ProcessorDialog(RegexProcessor regexProcessor)
        {
            this.DataContext = this;
            this.RegexProcessor = regexProcessor;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            this.RegexProcessor.Title = this.Title.Text;
            this.DialogResult = true;
            this.Close();
        }
    }
}
