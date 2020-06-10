using System.ComponentModel;

namespace GestionBDDApp
{
    partial class ExporterMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.ExportCsvButton = new System.Windows.Forms.Button();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.FileChoosedBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ExportCsvButton
            // 
            this.ExportCsvButton.Enabled = false;
            this.ExportCsvButton.Location = new System.Drawing.Point(260, 83);
            this.ExportCsvButton.Name = "ExportCsvButton";
            this.ExportCsvButton.Size = new System.Drawing.Size(75, 23);
            this.ExportCsvButton.TabIndex = 1;
            this.ExportCsvButton.Text = "Export";
            this.ExportCsvButton.UseVisualStyleBackColor = true;
            this.ExportCsvButton.Click += new System.EventHandler(this.ExportCsvButton_Click);
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(263, 33);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 2;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // FileChoosedBox
            // 
            this.FileChoosedBox.Location = new System.Drawing.Point(37, 36);
            this.FileChoosedBox.Name = "FileChoosedBox";
            this.FileChoosedBox.ReadOnly = true;
            this.FileChoosedBox.Size = new System.Drawing.Size(220, 20);
            this.FileChoosedBox.TabIndex = 3;
            // 
            // ExporterMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 120);
            this.Controls.Add(this.FileChoosedBox);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.ExportCsvButton);
            this.Name = "ExporterMenu";
            this.Text = "ExporterMenu";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Button ExportCsvButton;
        private System.Windows.Forms.TextBox FileChoosedBox;

        #endregion
    }
}