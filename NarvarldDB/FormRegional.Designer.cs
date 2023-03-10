namespace NarvarldDB
{
    partial class FormRegional
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
            this.LB_lan = new System.Windows.Forms.CheckedListBox();
            this.CloseButton = new System.Windows.Forms.Button();
            this.Edubutton = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.recruitbutton = new System.Windows.Forms.Button();
            this.foreignbutton = new System.Windows.Forms.Button();
            this.educationbutton = new System.Windows.Forms.Button();
            this.Transitionbutton = new System.Windows.Forms.Button();
            this.latlongbutton = new System.Windows.Forms.Button();
            this.agebutton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LB_lan
            // 
            this.LB_lan.FormattingEnabled = true;
            this.LB_lan.Location = new System.Drawing.Point(357, 11);
            this.LB_lan.Name = "LB_lan";
            this.LB_lan.Size = new System.Drawing.Size(120, 79);
            this.LB_lan.TabIndex = 5;
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(401, 427);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 49);
            this.CloseButton.TabIndex = 6;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // Edubutton
            // 
            this.Edubutton.Location = new System.Drawing.Point(358, 95);
            this.Edubutton.Margin = new System.Windows.Forms.Padding(2);
            this.Edubutton.Name = "Edubutton";
            this.Edubutton.Size = new System.Drawing.Size(118, 41);
            this.Edubutton.TabIndex = 7;
            this.Edubutton.Text = "Andel högutbildade per län";
            this.Edubutton.UseVisualStyleBackColor = true;
            this.Edubutton.Click += new System.EventHandler(this.Edubutton_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(16, 24);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(289, 292);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // recruitbutton
            // 
            this.recruitbutton.Location = new System.Drawing.Point(358, 140);
            this.recruitbutton.Margin = new System.Windows.Forms.Padding(2);
            this.recruitbutton.Name = "recruitbutton";
            this.recruitbutton.Size = new System.Drawing.Size(118, 38);
            this.recruitbutton.TabIndex = 9;
            this.recruitbutton.Text = "Tabell var rekryteras studenter?";
            this.recruitbutton.UseVisualStyleBackColor = true;
            this.recruitbutton.Click += new System.EventHandler(this.recruitbutton_Click);
            // 
            // foreignbutton
            // 
            this.foreignbutton.Location = new System.Drawing.Point(358, 182);
            this.foreignbutton.Margin = new System.Windows.Forms.Padding(2);
            this.foreignbutton.Name = "foreignbutton";
            this.foreignbutton.Size = new System.Drawing.Size(118, 54);
            this.foreignbutton.TabIndex = 10;
            this.foreignbutton.Text = "Andel utländsk bakgrund i rekryteringsbasen";
            this.foreignbutton.UseVisualStyleBackColor = true;
            this.foreignbutton.Click += new System.EventHandler(this.foreignbutton_Click);
            // 
            // educationbutton
            // 
            this.educationbutton.Location = new System.Drawing.Point(357, 240);
            this.educationbutton.Margin = new System.Windows.Forms.Padding(2);
            this.educationbutton.Name = "educationbutton";
            this.educationbutton.Size = new System.Drawing.Size(121, 46);
            this.educationbutton.TabIndex = 11;
            this.educationbutton.Text = "Andel lågutbildade i rekryteringsbasen";
            this.educationbutton.UseVisualStyleBackColor = true;
            this.educationbutton.Click += new System.EventHandler(this.educationbutton_Click);
            // 
            // Transitionbutton
            // 
            this.Transitionbutton.Location = new System.Drawing.Point(358, 291);
            this.Transitionbutton.Name = "Transitionbutton";
            this.Transitionbutton.Size = new System.Drawing.Size(120, 38);
            this.Transitionbutton.TabIndex = 12;
            this.Transitionbutton.Text = "Övergångstal i rekryteringsbasen";
            this.Transitionbutton.UseVisualStyleBackColor = true;
            this.Transitionbutton.Click += new System.EventHandler(this.Transitionbutton_Click);
            // 
            // latlongbutton
            // 
            this.latlongbutton.Location = new System.Drawing.Point(358, 335);
            this.latlongbutton.Name = "latlongbutton";
            this.latlongbutton.Size = new System.Drawing.Size(118, 42);
            this.latlongbutton.TabIndex = 13;
            this.latlongbutton.Text = "Var rekryteras studenter (lat/long)";
            this.latlongbutton.UseVisualStyleBackColor = true;
            this.latlongbutton.Click += new System.EventHandler(this.latlongbutton_Click);
            // 
            // agebutton
            // 
            this.agebutton.Location = new System.Drawing.Point(358, 383);
            this.agebutton.Name = "agebutton";
            this.agebutton.Size = new System.Drawing.Size(118, 38);
            this.agebutton.TabIndex = 14;
            this.agebutton.Text = "Åldersindex";
            this.agebutton.UseVisualStyleBackColor = true;
            this.agebutton.Click += new System.EventHandler(this.agebutton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(241, 383);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 38);
            this.button1.TabIndex = 15;
            this.button1.Text = "Marknadsandel per län/kommun";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormRegional
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 487);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.agebutton);
            this.Controls.Add(this.latlongbutton);
            this.Controls.Add(this.Transitionbutton);
            this.Controls.Add(this.educationbutton);
            this.Controls.Add(this.foreignbutton);
            this.Controls.Add(this.recruitbutton);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.Edubutton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.LB_lan);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormRegional";
            this.Text = "FormRegional";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox LB_lan;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button Edubutton;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button recruitbutton;
        private System.Windows.Forms.Button foreignbutton;
        private System.Windows.Forms.Button educationbutton;
        private System.Windows.Forms.Button Transitionbutton;
        private System.Windows.Forms.Button latlongbutton;
        private System.Windows.Forms.Button agebutton;
        private System.Windows.Forms.Button button1;
    }
}