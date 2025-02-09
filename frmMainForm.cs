using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Profil
{
    public partial class frmMainForm : Form
    {
        private string computerName = null;
        public frmMainForm(string _pcName)
        {
            InitializeComponent();
            computerName = _pcName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            frmProfil frmProfil = new frmProfil(computerName);
            frmProfil.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmLayout frmProfil = new frmLayout(computerName);
            frmProfil.ShowDialog();
        }
    }
}
