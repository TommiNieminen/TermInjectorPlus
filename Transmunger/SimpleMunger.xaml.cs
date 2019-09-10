using System;
using System.Collections.Generic;
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
    /// Interaction logic for SimpleMunger.xaml
    /// </summary>
    public partial class SimpleMunger : UserControl
    {
        
        public SimpleMunger()
        {
            this.DataContext = new ViewModel();
            InitializeComponent();
        }

    }
}
