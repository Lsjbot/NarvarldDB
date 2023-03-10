namespace NarvarldDB
{
    partial class FormColor
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
            this.LBseries = new System.Windows.Forms.ListBox();
            this.OKbutton = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.LBpalette = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // LBseries
            // 
            this.LBseries.FormattingEnabled = true;
            this.LBseries.Location = new System.Drawing.Point(37, 61);
            this.LBseries.Name = "LBseries";
            this.LBseries.Size = new System.Drawing.Size(153, 290);
            this.LBseries.TabIndex = 0;
            this.LBseries.SelectedIndexChanged += new System.EventHandler(this.LBseries_SelectedIndexChanged);
            // 
            // OKbutton
            // 
            this.OKbutton.Location = new System.Drawing.Point(356, 402);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(75, 23);
            this.OKbutton.TabIndex = 1;
            this.OKbutton.Text = "OK";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Series";
            // 
            // LBpalette
            // 
            this.LBpalette.FormattingEnabled = true;
            this.LBpalette.Location = new System.Drawing.Point(256, 60);
            this.LBpalette.Name = "LBpalette";
            this.LBpalette.Size = new System.Drawing.Size(136, 290);
            this.LBpalette.TabIndex = 3;
            this.LBpalette.SelectedIndexChanged += new System.EventHandler(this.LBpalette_SelectedIndexChanged);
            // 
            // FormColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 450);
            this.Controls.Add(this.LBpalette);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.LBseries);
            this.Name = "FormColor";
            this.Text = "FormColor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox LBseries;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox LBpalette;
    }
}