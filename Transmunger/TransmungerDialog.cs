using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Transmunger
{
    public partial class TransmungerDialog : Form
    {
        

        #region "ProviderConfDialog"
        public TransmungerDialog(TransmungerTPOptions translationOptions)
        {
            Options = translationOptions;
            InitializeComponent();
            UpdateDialog();
        }

        public TransmungerTPOptions Options
        {
            get;
            set;
        }
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
