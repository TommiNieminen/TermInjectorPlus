using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TermInjector2022;

namespace SettingsDialogTester
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            TermInjectorPlusDialog dialog = new TermInjectorPlusDialog(new TermInjectorPlusTPOptions());
            if (dialog.ShowDialog() == DialogResult.OK)
            {
            }
            Console.ReadLine();
        }
    }
}
