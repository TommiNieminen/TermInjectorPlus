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
    /// Interaction logic for ListBoxWithControls.xaml
    /// </summary>
    public partial class ListBoxWithControls : UserControl
    {

        ObservableCollection<ITransProcessor> modelList;

        public ListBoxWithControls()
        {
            InitializeComponent();
            this.DataContextChanged += dataContextChanged;
        }

        private void dataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.modelList = this.DataContext as ObservableCollection<ITransProcessor>;
        }
        
        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessorDialog processorDialog = new ProcessorDialog();
            processorDialog.ShowDialog();
            if (processorDialog.DialogResult.Value == true)
            {
                this.modelList.Add(processorDialog.RegexProcessor);
            }
        }

        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnEditItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessorDialog processorDialog = new ProcessorDialog();
            processorDialog.RegexCollection = ((RegexProcessor)this.lbTodoList.SelectedItem).RegexCollection;
            processorDialog.ShowDialog();
        }

        private void LbTodoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
