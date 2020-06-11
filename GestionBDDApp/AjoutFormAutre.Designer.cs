namespace GestionBDDApp
{
    partial class AjoutFormAutre
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
            this.DescriptionBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.NotValidateButton = new System.Windows.Forms.Button();
            this.ValidateButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.FamillyComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // DescriptionBox
            // 
            this.DescriptionBox.Location = new System.Drawing.Point(162, 59);
            this.DescriptionBox.Multiline = true;
            this.DescriptionBox.Name = "DescriptionBox";
            this.DescriptionBox.Size = new System.Drawing.Size(137, 54);
            this.DescriptionBox.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(69, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Description";
            // 
            // NotValidateButton
            // 
            this.NotValidateButton.Location = new System.Drawing.Point(200, 191);
            this.NotValidateButton.Name = "NotValidateButton";
            this.NotValidateButton.Size = new System.Drawing.Size(99, 23);
            this.NotValidateButton.TabIndex = 14;
            this.NotValidateButton.Text = "Anuler";
            this.NotValidateButton.UseVisualStyleBackColor = true;
            this.NotValidateButton.Click += new System.EventHandler(this.NotValidateButton_Click);
            // 
            // ValidateButton
            // 
            this.ValidateButton.Location = new System.Drawing.Point(30, 191);
            this.ValidateButton.Name = "ValidateButton";
            this.ValidateButton.Size = new System.Drawing.Size(99, 23);
            this.ValidateButton.TabIndex = 13;
            this.ValidateButton.Text = "Valider";
            this.ValidateButton.UseVisualStyleBackColor = true;
            this.ValidateButton.Click += new System.EventHandler(this.ValidateButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(81, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Familles";
            this.label2.Visible = false;
            // 
            // FamillyComboBox
            // 
            this.FamillyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FamillyComboBox.FormattingEnabled = true;
            this.FamillyComboBox.Location = new System.Drawing.Point(162, 144);
            this.FamillyComboBox.Name = "FamillyComboBox";
            this.FamillyComboBox.Size = new System.Drawing.Size(137, 21);
            this.FamillyComboBox.TabIndex = 18;
            this.FamillyComboBox.Visible = false;
            // 
            // AjoutFormAutre
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 264);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FamillyComboBox);
            this.Controls.Add(this.DescriptionBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NotValidateButton);
            this.Controls.Add(this.ValidateButton);
            this.Name = "AjoutFormAutre";
            this.Text = "AjoutFormAutre";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox DescriptionBox;
        private System.Windows.Forms.ComboBox FamillyComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button NotValidateButton;
        private System.Windows.Forms.Button ValidateButton;

        #endregion
    }
}