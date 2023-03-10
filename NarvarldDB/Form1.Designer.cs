namespace NarvarldDB
{
    partial class Form1
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
            this.DBbutton = new System.Windows.Forms.Button();
            this.Readbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Quitbutton
            // 
            this.Quitbutton.Location = new System.Drawing.Point(349, 418);
            this.Quitbutton.Name = "Quitbutton";
            this.Quitbutton.Size = new System.Drawing.Size(152, 98);
            this.Quitbutton.TabIndex = 0;
            this.Quitbutton.Text = "Quit";
            this.Quitbutton.UseVisualStyleBackColor = true;
            this.Quitbutton.Click += new System.EventHandler(this.Quitbutton_Click);
            // 
            // DBbutton
            // 
            this.DBbutton.Location = new System.Drawing.Point(349, 269);
            this.DBbutton.Name = "DBbutton";
            this.DBbutton.Size = new System.Drawing.Size(152, 112);
            this.DBbutton.TabIndex = 1;
            this.DBbutton.Text = "Upload to database";
            this.DBbutton.UseVisualStyleBackColor = true;
            this.DBbutton.Click += new System.EventHandler(this.DBbutton_Click);
            // 
            // Readbutton
            // 
            this.Readbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Readbutton.Location = new System.Drawing.Point(349, 30);
            this.Readbutton.Name = "Readbutton";
            this.Readbutton.Size = new System.Drawing.Size(152, 139);
            this.Readbutton.TabIndex = 2;
            this.Readbutton.Text = "Display information";
            this.Readbutton.UseVisualStyleBackColor = true;
            this.Readbutton.Click += new System.EventHandler(this.Readbutton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 537);
            this.Controls.Add(this.Readbutton);
            this.Controls.Add(this.DBbutton);
            this.Controls.Add(this.Quitbutton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Quitbutton;
        private System.Windows.Forms.Button DBbutton;
        private System.Windows.Forms.Button Readbutton;
    }
}

