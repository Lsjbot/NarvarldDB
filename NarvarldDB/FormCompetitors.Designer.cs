namespace NarvarldDB
{
    partial class FormCompetitors
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
            this.Closebutton = new System.Windows.Forms.Button();
            this.LBprog = new System.Windows.Forms.ListBox();
            this.LBsubj = new System.Windows.Forms.ListBox();
            this.CBadvanced = new System.Windows.Forms.CheckBox();
            this.Allbutton = new System.Windows.Forms.Button();
            this.nichebutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Closebutton
            // 
            this.Closebutton.Location = new System.Drawing.Point(484, 389);
            this.Closebutton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Closebutton.Name = "Closebutton";
            this.Closebutton.Size = new System.Drawing.Size(56, 19);
            this.Closebutton.TabIndex = 0;
            this.Closebutton.Text = "Close";
            this.Closebutton.UseVisualStyleBackColor = true;
            this.Closebutton.Click += new System.EventHandler(this.Closebutton_Click);
            // 
            // LBprog
            // 
            this.LBprog.FormattingEnabled = true;
            this.LBprog.HorizontalScrollbar = true;
            this.LBprog.Location = new System.Drawing.Point(9, 53);
            this.LBprog.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.LBprog.Name = "LBprog";
            this.LBprog.Size = new System.Drawing.Size(454, 290);
            this.LBprog.Sorted = true;
            this.LBprog.TabIndex = 1;
            this.LBprog.SelectedIndexChanged += new System.EventHandler(this.LBprog_SelectedIndexChanged);
            // 
            // LBsubj
            // 
            this.LBsubj.FormattingEnabled = true;
            this.LBsubj.Location = new System.Drawing.Point(486, 53);
            this.LBsubj.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.LBsubj.Name = "LBsubj";
            this.LBsubj.Size = new System.Drawing.Size(118, 251);
            this.LBsubj.Sorted = true;
            this.LBsubj.TabIndex = 2;
            this.LBsubj.SelectedIndexChanged += new System.EventHandler(this.LBsubj_SelectedIndexChanged);
            // 
            // CBadvanced
            // 
            this.CBadvanced.AutoSize = true;
            this.CBadvanced.Location = new System.Drawing.Point(491, 328);
            this.CBadvanced.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.CBadvanced.Name = "CBadvanced";
            this.CBadvanced.Size = new System.Drawing.Size(101, 17);
            this.CBadvanced.TabIndex = 3;
            this.CBadvanced.Text = "Avancerad nivå";
            this.CBadvanced.UseVisualStyleBackColor = true;
            // 
            // Allbutton
            // 
            this.Allbutton.Location = new System.Drawing.Point(44, 381);
            this.Allbutton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Allbutton.Name = "Allbutton";
            this.Allbutton.Size = new System.Drawing.Size(125, 40);
            this.Allbutton.TabIndex = 4;
            this.Allbutton.Text = "All competition for all institutions";
            this.Allbutton.UseVisualStyleBackColor = true;
            this.Allbutton.Click += new System.EventHandler(this.Allbutton_Click);
            // 
            // nichebutton
            // 
            this.nichebutton.Location = new System.Drawing.Point(217, 383);
            this.nichebutton.Name = "nichebutton";
            this.nichebutton.Size = new System.Drawing.Size(75, 38);
            this.nichebutton.TabIndex = 5;
            this.nichebutton.Text = "Potential niches";
            this.nichebutton.UseVisualStyleBackColor = true;
            this.nichebutton.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormCompetitors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 440);
            this.Controls.Add(this.nichebutton);
            this.Controls.Add(this.Allbutton);
            this.Controls.Add(this.CBadvanced);
            this.Controls.Add(this.LBsubj);
            this.Controls.Add(this.LBprog);
            this.Controls.Add(this.Closebutton);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "FormCompetitors";
            this.Text = "FormCompetitors";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Closebutton;
        private System.Windows.Forms.ListBox LBprog;
        private System.Windows.Forms.ListBox LBsubj;
        private System.Windows.Forms.CheckBox CBadvanced;
        private System.Windows.Forms.Button Allbutton;
        private System.Windows.Forms.Button nichebutton;
    }
}