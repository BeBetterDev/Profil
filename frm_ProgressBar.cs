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
    public partial class frm_ProgressBar : Form
    {
        public frm_ProgressBar(string labelTop)
        {
            InitializeComponent();
            lblOperationName.Text = labelTop;
        }

        private void frm_ProgressBar_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
        }
    }
}
