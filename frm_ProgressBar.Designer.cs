namespace Profil
{
    partial class frm_ProgressBar
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labelPleaseWait = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblAction = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblPcName = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblOperationName = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.ForeColor = System.Drawing.Color.Gray;
            this.progressBar.Location = new System.Drawing.Point(88, 102);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(333, 61);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 0;
            // 
            // labelPleaseWait
            // 
            this.labelPleaseWait.AutoSize = true;
            this.labelPleaseWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelPleaseWait.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelPleaseWait.Location = new System.Drawing.Point(162, 187);
            this.labelPleaseWait.Name = "labelPleaseWait";
            this.labelPleaseWait.Size = new System.Drawing.Size(171, 25);
            this.labelPleaseWait.TabIndex = 1;
            this.labelPleaseWait.Text = "Proszę czekać...";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblAction);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.labelPleaseWait);
            this.panel1.Controls.Add(this.progressBar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(504, 272);
            this.panel1.TabIndex = 2;
            // 
            // lblAction
            // 
            this.lblAction.AutoSize = true;
            this.lblAction.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblAction.Location = new System.Drawing.Point(184, 217);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(129, 13);
            this.lblAction.TabIndex = 56;
            this.lblAction.Text = "Inwentaryzuję Dokumenty";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.panel3.Controls.Add(this.lblPcName);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel3.Location = new System.Drawing.Point(5, 240);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(492, 25);
            this.panel3.TabIndex = 55;
            // 
            // lblPcName
            // 
            this.lblPcName.AutoSize = true;
            this.lblPcName.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblPcName.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblPcName.Location = new System.Drawing.Point(7, 4);
            this.lblPcName.Name = "lblPcName";
            this.lblPcName.Size = new System.Drawing.Size(53, 17);
            this.lblPcName.TabIndex = 0;
            this.lblPcName.Text = "CI OHD";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.panel2.Controls.Add(this.lblOperationName);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(5, 5);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.panel2.Size = new System.Drawing.Size(492, 40);
            this.panel2.TabIndex = 54;
            // 
            // lblOperationName
            // 
            this.lblOperationName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblOperationName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblOperationName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblOperationName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblOperationName.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblOperationName.Location = new System.Drawing.Point(0, 10);
            this.lblOperationName.Multiline = true;
            this.lblOperationName.Name = "lblOperationName";
            this.lblOperationName.ReadOnly = true;
            this.lblOperationName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblOperationName.Size = new System.Drawing.Size(492, 27);
            this.lblOperationName.TabIndex = 53;
            this.lblOperationName.TabStop = false;
            this.lblOperationName.Text = "Postęp instalacji";
            this.lblOperationName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // frm_ProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 272);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frm_ProgressBar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frm_ProgressBar_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labelPleaseWait;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox lblOperationName;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblPcName;
        public System.Windows.Forms.Label lblAction;
    }
}