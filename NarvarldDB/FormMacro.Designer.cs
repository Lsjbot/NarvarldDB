namespace NarvarldDB
{
    partial class FormMacro
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
            this.Quitbutton = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Runbutton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // Quitbutton
            // 
            this.Quitbutton.Location = new System.Drawing.Point(318, 343);
            this.Quitbutton.Name = "Quitbutton";
            this.Quitbutton.Size = new System.Drawing.Size(119, 67);
            this.Quitbutton.TabIndex = 0;
            this.Quitbutton.Text = "Quit";
            this.Quitbutton.UseVisualStyleBackColor = true;
            this.Quitbutton.Click += new System.EventHandler(this.Quitbutton_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(20, 26);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(242, 277);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // Runbutton
            // 
            this.Runbutton.Location = new System.Drawing.Point(318, 240);
            this.Runbutton.Name = "Runbutton";
            this.Runbutton.Size = new System.Drawing.Size(119, 63);
            this.Runbutton.TabIndex = 2;
            this.Runbutton.Text = "Run macro";
            this.Runbutton.UseVisualStyleBackColor = true;
            this.Runbutton.Click += new System.EventHandler(this.Runbutton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FormMacro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 441);
            this.Controls.Add(this.Runbutton);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.Quitbutton);
            this.Name = "FormMacro";
            this.Text = "FormMacro";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Quitbutton;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button Runbutton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}