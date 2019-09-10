using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Transmunger;

namespace SettingsDialogTester
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            TransmungerDialog dialog = new TransmungerDialog(new TransmungerTPOptions());
            if (dialog.ShowDialog() == DialogResult.OK)
            {
            }
            Console.ReadLine();
        }
    }
}
