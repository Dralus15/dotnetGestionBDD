using GestionBDDApp.data.dao;
using System;
using System.Linq;

namespace GestionBDDApp
{
    partial class AjoutForm
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
            this.BrandComboBox = new System.Windows.Forms.ComboBox();
            this.SubFamillyComboBox = new System.Windows.Forms.ComboBox();
            this.FamillyComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ValidateButton = new System.Windows.Forms.Button();
            this.NotValidateButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.DescriptionBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.PriceBox = new System.Windows.Forms.NumericUpDown();
            this.QuantityBox = new System.Windows.Forms.NumericUpDown();
            this.ReferenceBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize) (this.PriceBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.QuantityBox)).BeginInit();
            this.SuspendLayout();
            // 
            // BrandComboBox
            // 
            this.BrandComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BrandComboBox.FormattingEnabled = true;
            this.BrandComboBox.Location = new System.Drawing.Point(199, 151);
            this.BrandComboBox.Name = "BrandComboBox";
            this.BrandComboBox.Size = new System.Drawing.Size(137, 21);
            this.BrandComboBox.TabIndex = 2;
            // 
            // SubFamillyComboBox
            // 
            this.SubFamillyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SubFamillyComboBox.FormattingEnabled = true;
            this.SubFamillyComboBox.Location = new System.Drawing.Point(199, 255);
            this.SubFamillyComboBox.Name = "SubFamillyComboBox";
            this.SubFamillyComboBox.Size = new System.Drawing.Size(137, 21);
            this.SubFamillyComboBox.TabIndex = 4;
            // 
            // FamillyComboBox
            // 
            this.FamillyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FamillyComboBox.FormattingEnabled = true;
            this.FamillyComboBox.Location = new System.Drawing.Point(199, 203);
            this.FamillyComboBox.Name = "FamillyComboBox";
            this.FamillyComboBox.Size = new System.Drawing.Size(137, 21);
            this.FamillyComboBox.TabIndex = 3;
            this.FamillyComboBox.SelectedIndexChanged += new System.EventHandler(this.Familles_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(118, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Marques";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Familles";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(95, 258);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Sous-Familles";
            // 
            // ValidateButton
            // 
            this.ValidateButton.Location = new System.Drawing.Point(79, 410);
            this.ValidateButton.Name = "ValidateButton";
            this.ValidateButton.Size = new System.Drawing.Size(99, 23);
            this.ValidateButton.TabIndex = 7;
            this.ValidateButton.Text = "Valider";
            this.ValidateButton.UseVisualStyleBackColor = true;
            this.ValidateButton.Click += new System.EventHandler(this.ValidateButton_Click);
            // 
            // NotValidateButton
            // 
            this.NotValidateButton.Location = new System.Drawing.Point(257, 410);
            this.NotValidateButton.Name = "NotValidateButton";
            this.NotValidateButton.Size = new System.Drawing.Size(99, 23);
            this.NotValidateButton.TabIndex = 8;
            this.NotValidateButton.Text = "Anuler";
            this.NotValidateButton.UseVisualStyleBackColor = true;
            this.NotValidateButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(106, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Description";
            // 
            // DescriptionBox
            // 
            this.DescriptionBox.Location = new System.Drawing.Point(199, 80);
            this.DescriptionBox.Multiline = true;
            this.DescriptionBox.Name = "DescriptionBox";
            this.DescriptionBox.Size = new System.Drawing.Size(137, 54);
            this.DescriptionBox.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(115, 356);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Quantité";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(138, 304);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Prix";
            // 
            // PriceBox
            // 
            this.PriceBox.Location = new System.Drawing.Point(199, 302);
            this.PriceBox.Name = "PriceBox";
            this.PriceBox.Size = new System.Drawing.Size(137, 20);
            this.PriceBox.TabIndex = 5;
            // 
            // QuantityBox
            // 
            this.QuantityBox.Location = new System.Drawing.Point(199, 354);
            this.QuantityBox.Name = "QuantityBox";
            this.QuantityBox.Size = new System.Drawing.Size(137, 20);
            this.QuantityBox.TabIndex = 6;
            // 
            // ReferenceBox
            // 
            this.ReferenceBox.Location = new System.Drawing.Point(199, 36);
            this.ReferenceBox.Name = "ReferenceBox";
            this.ReferenceBox.Size = new System.Drawing.Size(137, 20);
            this.ReferenceBox.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(109, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Référence";
            // 
            // AjoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 484);
            this.Controls.Add(this.ReferenceBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.QuantityBox);
            this.Controls.Add(this.PriceBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.DescriptionBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NotValidateButton);
            this.Controls.Add(this.ValidateButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SubFamillyComboBox);
            this.Controls.Add(this.FamillyComboBox);
            this.Controls.Add(this.BrandComboBox);
            this.Name = "AjoutForm";
            this.Text = "Ajout/Modification";
            ((System.ComponentModel.ISupportInitialize) (this.PriceBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.QuantityBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ComboBox BrandComboBox;
        private System.Windows.Forms.TextBox DescriptionBox;
        private System.Windows.Forms.ComboBox FamillyComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button NotValidateButton;
        private System.Windows.Forms.NumericUpDown PriceBox;
        private System.Windows.Forms.NumericUpDown QuantityBox;
        private System.Windows.Forms.TextBox ReferenceBox;
        private System.Windows.Forms.ComboBox SubFamillyComboBox;
        private System.Windows.Forms.Button ValidateButton;

        #endregion
    }
}