using System;

namespace TermInjectorPlus
{
    partial class TermInjectorPlusDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TermInjectorPlusDialog));
            this.wpfHost = new System.Windows.Forms.Integration.ElementHost();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SaveSettingsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // wpfHost
            // 
            this.wpfHost.Location = new System.Drawing.Point(3, 0);
            this.wpfHost.Name = "wpfHost";
            this.wpfHost.Size = new System.Drawing.Size(1492, 657);
            this.wpfHost.TabIndex = 1;
            this.wpfHost.Text = "elementHost1";
            this.wpfHost.Child = null;
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(1385, 674);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(103, 44);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SaveSettingsButton
            // 
            this.SaveSettingsButton.Location = new System.Drawing.Point(1213, 674);
            this.SaveSettingsButton.Name = "SaveSettingsButton";
            this.SaveSettingsButton.Size = new System.Drawing.Size(157, 44);
            this.SaveSettingsButton.TabIndex = 3;
            this.SaveSettingsButton.Text = "Save settings";
            this.SaveSettingsButton.UseVisualStyleBackColor = true;
            this.SaveSettingsButton.Click += new System.EventHandler(this.SaveSettingsButton_Click);
            // 
            // TermInjectorPlusDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1498, 730);
            this.Controls.Add(this.SaveSettingsButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.wpfHost);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TermInjectorPlusDialog";
            this.Text = "TermInjector+";
            this.ResumeLayout(false);

        }

        private void Ok_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
        private System.Windows.Forms.Integration.ElementHost wpfHost;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button SaveSettingsButton;
    }
}