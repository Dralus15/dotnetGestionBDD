namespace GestionBDDApp
{
    partial class ImporterMenu
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
            this.SelectCsvButton = new System.Windows.Forms.Button();
            this.PathChoosedFile = new System.Windows.Forms.TextBox();
            this.EreaseModeButton = new System.Windows.Forms.Button();
            this.AppendModeButton = new System.Windows.Forms.Button();
            this.ImportProgress = new System.Windows.Forms.ProgressBar();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SelectCsvButton
            // 
            this.SelectCsvButton.Location = new System.Drawing.Point(308, 21);
            this.SelectCsvButton.Name = "SelectCsvButton";
            this.SelectCsvButton.Size = new System.Drawing.Size(75, 23);
            this.SelectCsvButton.TabIndex = 0;
            this.SelectCsvButton.Text = "Browse";
            this.SelectCsvButton.UseVisualStyleBackColor = true;
            this.SelectCsvButton.Click += new System.EventHandler(this.SelectCsvButton_Click);
            // 
            // PathChoosedFile
            // 
            this.PathChoosedFile.Location = new System.Drawing.Point(26, 21);
            this.PathChoosedFile.Name = "PathChoosedFile";
            this.PathChoosedFile.ReadOnly = true;
            this.PathChoosedFile.Size = new System.Drawing.Size(260, 20);
            this.PathChoosedFile.TabIndex = 1;
            // 
            // EreaseModeButton
            // 
            this.EreaseModeButton.Enabled = false;
            this.EreaseModeButton.Location = new System.Drawing.Point(229, 47);
            this.EreaseModeButton.Name = "EreaseModeButton";
            this.EreaseModeButton.Size = new System.Drawing.Size(154, 69);
            this.EreaseModeButton.TabIndex = 2;
            this.EreaseModeButton.Text = "Importer en mode écrasement";
            this.EreaseModeButton.UseVisualStyleBackColor = true;
            this.EreaseModeButton.Click += new System.EventHandler(this.EreaseModeButton_Click);
            // 
            // AppendModeButton
            // 
            this.AppendModeButton.Enabled = false;
            this.AppendModeButton.Location = new System.Drawing.Point(26, 47);
            this.AppendModeButton.Name = "AppendModeButton";
            this.AppendModeButton.Size = new System.Drawing.Size(154, 69);
            this.AppendModeButton.TabIndex = 3;
            this.AppendModeButton.Text = "Importer en mode ajout";
            this.AppendModeButton.UseVisualStyleBackColor = true;
            this.AppendModeButton.Click += new System.EventHandler(this.AppendModeButton_Click);
            // 
            // ImportProgress
            // 
            this.ImportProgress.Location = new System.Drawing.Point(26, 122);
            this.ImportProgress.Name = "ImportProgress";
            this.ImportProgress.Size = new System.Drawing.Size(357, 23);
            this.ImportProgress.TabIndex = 4;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 150);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(399, 22);
            this.statusStrip1.TabIndex = 5;
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(118, 17);
            this.StatusText.Text = "Choisisez un fichier à importer";
            // 
            // ImporterMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 172);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ImportProgress);
            this.Controls.Add(this.AppendModeButton);
            this.Controls.Add(this.EreaseModeButton);
            this.Controls.Add(this.PathChoosedFile);
            this.Controls.Add(this.SelectCsvButton);
            this.Name = "ImporterMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Importer une base";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelectCsvButton;
        private System.Windows.Forms.TextBox PathChoosedFile;
        private System.Windows.Forms.Button EreaseModeButton;
        private System.Windows.Forms.Button AppendModeButton;
        private System.Windows.Forms.ProgressBar ImportProgress;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
    }
}