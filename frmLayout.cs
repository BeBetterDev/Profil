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
    public partial class frmLayout : Form
    {
        private string computerName = null;
        public frmLayout(string _pcName)
        {
            computerName = _pcName;
            InitializeComponent();
            //btnRefresh.FlatAppearance.BorderColor = SystemColors.Control;
            btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(0, 200, 200, 200);

            panelRight.Controls.Clear();          

            ucRestoreProfile restoreProfile = new ucRestoreProfile(computerName);
            restoreProfile.Dock = DockStyle.Fill;
            

            if (panelRight != null)
            {
                panelRight.Controls.Clear(); // Czyszczenie panelu

                // Dodanie nowej kontrolki
                var userControl = new ucRefreshProfile(computerName);
                {
                    Dock = DockStyle.Fill; // Zadokowanie
                };

                panelRight.Controls.Add(userControl);
            }
            else
            {
                MessageBox.Show("Panel lub główny formularz nie jest poprawnie zainicjalizowany.");
            }

        }


        private void btnRestore_Click(object sender, EventArgs e)
        {
            //btnRestore.Dock = DockStyle.Top;
            //btnRestore.FlatAppearance.BorderColor = SystemColors.Control;
            //btnRestore.BackColor = SystemColors.Control;


            //btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(255, 200, 200, 200);
            //btnRefresh.BackColor = Color.FromArgb(0, 230, 230, 230);



            if (panelRight != null)
            {

                panelRight.Controls.Clear(); // Czyszczenie panelu

                // Dodanie nowej kontrolki
                var userControl = new ucRestoreProfile(computerName);
                {
                    Dock = DockStyle.Fill; // Zadokowanie
                };

                panelRight.Controls.Add(userControl);
            }
            else
            {
                MessageBox.Show("Panel lub główny formularz nie jest poprawnie zainicjalizowany.");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //btnRestore.Dock = DockStyle.Bottom;
            //btnRestore.FlatAppearance.BorderColor = Color.FromArgb(255, 200, 200, 200);
            ////btnRestore.FlatAppearance.BorderColor = SystemColors.Control;
            //btnRestore.BackColor = Color.FromArgb(0, 230, 230, 230);


            //btnRefresh.BackColor = SystemColors.Control;
            //btnRefresh.FlatAppearance.BorderColor = SystemColors.Control;
            ////btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(255, 200, 200, 200);



            if (panelRight != null)
            {

                panelRight.Controls.Clear(); // Czyszczenie panelu

                // Dodanie nowej kontrolki
                var userControl = new ucRefreshProfile(computerName);
                {
                    Dock = DockStyle.Fill; // Zadokowanie
                };

                panelRight.Controls.Add(userControl);
            }
            else
            {
                MessageBox.Show("Panel lub główny formularz nie jest poprawnie zainicjalizowany.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
