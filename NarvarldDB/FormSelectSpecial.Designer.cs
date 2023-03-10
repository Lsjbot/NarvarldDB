namespace NarvarldDB
{
    partial class FormSelectSpecial
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
            this.Engbutton = new System.Windows.Forms.Button();
            this.Teacherbutton = new System.Windows.Forms.Button();
            this.Nursebutton = new System.Windows.Forms.Button();
            this.Quitbutton = new System.Windows.Forms.Button();
            this.Specsskbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Engbutton
            // 
            this.Engbutton.Location = new System.Drawing.Point(56, 47);
            this.Engbutton.Name = "Engbutton";
            this.Engbutton.Size = new System.Drawing.Size(183, 37);
            this.Engbutton.TabIndex = 0;
            this.Engbutton.Text = "Högskoleingenjör";
            this.Engbutton.UseVisualStyleBackColor = true;
            this.Engbutton.Click += new System.EventHandler(this.Engbutton_Click);
            // 
            // Teacherbutton
            // 
            this.Teacherbutton.Location = new System.Drawing.Point(56, 104);
            this.Teacherbutton.Name = "Teacherbutton";
            this.Teacherbutton.Size = new System.Drawing.Size(183, 37);
            this.Teacherbutton.TabIndex = 1;
            this.Teacherbutton.Text = "Lärare";
            this.Teacherbutton.UseVisualStyleBackColor = true;
            this.Teacherbutton.Click += new System.EventHandler(this.Teacherbutton_Click);
            // 
            // Nursebutton
            // 
            this.Nursebutton.Location = new System.Drawing.Point(56, 163);
            this.Nursebutton.Name = "Nursebutton";
            this.Nursebutton.Size = new System.Drawing.Size(183, 41);
            this.Nursebutton.TabIndex = 2;
            this.Nursebutton.Text = "Sjuksköterska";
            this.Nursebutton.UseVisualStyleBackColor = true;
            this.Nursebutton.Click += new System.EventHandler(this.Nursebutton_Click);
            // 
            // Quitbutton
            // 
            this.Quitbutton.Location = new System.Drawing.Point(66, 358);
            this.Quitbutton.Name = "Quitbutton";
            this.Quitbutton.Size = new System.Drawing.Size(173, 48);
            this.Quitbutton.TabIndex = 3;
            this.Quitbutton.Text = "Avbryt";
            this.Quitbutton.UseVisualStyleBackColor = true;
            this.Quitbutton.Click += new System.EventHandler(this.Quitbutton_Click);
            // 
            // Specsskbutton
            // 
            this.Specsskbutton.Location = new System.Drawing.Point(56, 221);
            this.Specsskbutton.Name = "Specsskbutton";
            this.Specsskbutton.Size = new System.Drawing.Size(183, 45);
            this.Specsskbutton.TabIndex = 4;
            this.Specsskbutton.Text = "Specialistsjuksköterska";
            this.Specsskbutton.UseVisualStyleBackColor = true;
            this.Specsskbutton.Click += new System.EventHandler(this.Specsskbutton_Click);
            // 
            // FormSelectSpecial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 450);
            this.Controls.Add(this.Specsskbutton);
            this.Controls.Add(this.Quitbutton);
            this.Controls.Add(this.Nursebutton);
            this.Controls.Add(this.Teacherbutton);
            this.Controls.Add(this.Engbutton);
            this.Name = "FormSelectSpecial";
            this.Text = "FormSelectSpecial";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Engbutton;
        private System.Windows.Forms.Button Teacherbutton;
        private System.Windows.Forms.Button Nursebutton;
        private System.Windows.Forms.Button Quitbutton;
        private System.Windows.Forms.Button Specsskbutton;
    }
}