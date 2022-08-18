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
            TermInjector2022Dialog dialog = new TermInjector2022Dialog(new TermInjector2022TPOptions());
            if (dialog.ShowDialog() == DialogResult.OK)
            {
            }
            Console.ReadLine();
        }
    }
}
