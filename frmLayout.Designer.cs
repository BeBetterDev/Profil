namespace Profil
{
    partial class frmLayout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.centerPanel = new Profil.CustomPanel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.customPanel1 = new Profil.CustomPanel();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.customPanelBottom = new Profil.CustomPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.customPanelTop = new Profil.CustomPanel();
            this.lblTopDescription = new System.Windows.Forms.Label();
            this.centerPanel.SuspendLayout();
            this.customPanel1.SuspendLayout();
            this.customPanelBottom.SuspendLayout();
            this.customPanelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // centerPanel
            // 
            this.centerPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.centerPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.centerPanel.Controls.Add(this.panelRight);
            this.centerPanel.Controls.Add(this.customPanel1);
            this.centerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.centerPanel.Location = new System.Drawing.Point(3, 49);
            this.centerPanel.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
            this.centerPanel.Name = "centerPanel";
            this.centerPanel.Padding = new System.Windows.Forms.Padding(2);
            this.centerPanel.Size = new System.Drawing.Size(944, 455);
            this.centerPanel.TabIndex = 11;
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.SystemColors.Control;
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(199, 2);
            this.panelRight.Margin = new System.Windows.Forms.Padding(0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.panelRight.Size = new System.Drawing.Size(743, 451);
            this.panelRight.TabIndex = 10;
            // 
            // customPanel1
            // 
            this.customPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.customPanel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.customPanel1.Controls.Add(this.btnRestore);
            this.customPanel1.Controls.Add(this.btnRefresh);
            this.customPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.customPanel1.Location = new System.Drawing.Point(2, 2);
            this.customPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.customPanel1.Name = "customPanel1";
            this.customPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.customPanel1.Size = new System.Drawing.Size(197, 451);
            this.customPanel1.TabIndex = 12;
            // 
            // btnRestore
            // 
            this.btnRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.btnRestore.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnRestore.Location = new System.Drawing.Point(3, 44);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(191, 41);
            this.btnRestore.TabIndex = 3;
            this.btnRestore.Text = "Przywróć profil";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.Control;
            this.btnRefresh.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnRefresh.Location = new System.Drawing.Point(3, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(191, 41);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Odśwież profil";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // customPanelBottom
            // 
            this.customPanelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.customPanelBottom.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.customPanelBottom.Controls.Add(this.btnClose);
            this.customPanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.customPanelBottom.Location = new System.Drawing.Point(3, 504);
            this.customPanelBottom.Name = "customPanelBottom";
            this.customPanelBottom.Padding = new System.Windows.Forms.Padding(1);
            this.customPanelBottom.Size = new System.Drawing.Size(944, 69);
            this.customPanelBottom.TabIndex = 8;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnClose.Location = new System.Drawing.Point(759, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(170, 41);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Zamknij";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // customPanelTop
            // 
            this.customPanelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.customPanelTop.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.customPanelTop.Controls.Add(this.lblTopDescription);
            this.customPanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.customPanelTop.Location = new System.Drawing.Point(3, 3);
            this.customPanelTop.Name = "customPanelTop";
            this.customPanelTop.Size = new System.Drawing.Size(944, 46);
            this.customPanelTop.TabIndex = 7;
            // 
            // lblTopDescription
            // 
            this.lblTopDescription.AutoSize = true;
            this.lblTopDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTopDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(91)))), ((int)(((byte)(135)))));
            this.lblTopDescription.Location = new System.Drawing.Point(10, 14);
            this.lblTopDescription.Name = "lblTopDescription";
            this.lblTopDescription.Size = new System.Drawing.Size(229, 16);
            this.lblTopDescription.TabIndex = 1;
            this.lblTopDescription.Text = "Odświeżanie profilu użytkownika";
            // 
            // frmLayout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(950, 576);
            this.Controls.Add(this.centerPanel);
            this.Controls.Add(this.customPanelBottom);
            this.Controls.Add(this.customPanelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLayout";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmLayout";
            this.centerPanel.ResumeLayout(false);
            this.customPanel1.ResumeLayout(false);
            this.customPanelBottom.ResumeLayout(false);
            this.customPanelTop.ResumeLayout(false);
            this.customPanelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CustomPanel customPanelTop;
        private System.Windows.Forms.Label lblTopDescription;
        private CustomPanel customPanelBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panelRight;
        private CustomPanel centerPanel;
        private CustomPanel customPanel1;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnRefresh;
    }
}