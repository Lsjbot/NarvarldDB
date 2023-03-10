namespace NarvarldDB
{
    partial class FormFinance
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
            this.LBpost = new System.Windows.Forms.ListBox();
            this.LBverksamhet = new System.Windows.Forms.ListBox();
            this.displaybutton = new System.Windows.Forms.Button();
            this.testbutton = new System.Windows.Forms.Button();
            this.fourfieldbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LBpost
            // 
            this.LBpost.FormattingEnabled = true;
            this.LBpost.Location = new System.Drawing.Point(431, 35);
            this.LBpost.Name = "LBpost";
            this.LBpost.Size = new System.Drawing.Size(171, 355);
            this.LBpost.TabIndex = 0;
            // 
            // LBverksamhet
            // 
            this.LBverksamhet.FormattingEnabled = true;
            this.LBverksamhet.Location = new System.Drawing.Point(608, 35);
            this.LBverksamhet.Name = "LBverksamhet";
            this.LBverksamhet.Size = new System.Drawing.Size(167, 186);
            this.LBverksamhet.TabIndex = 1;
            // 
            // displaybutton
            // 
            this.displaybutton.Location = new System.Drawing.Point(649, 262);
            this.displaybutton.Name = "displaybutton";
            this.displaybutton.Size = new System.Drawing.Size(116, 44);
            this.displaybutton.TabIndex = 2;
            this.displaybutton.Text = "Diagram post/verksamhet";
            this.displaybutton.UseVisualStyleBackColor = true;
            this.displaybutton.Click += new System.EventHandler(this.displaybutton_Click);
            // 
            // testbutton
            // 
            this.testbutton.Location = new System.Drawing.Point(654, 320);
            this.testbutton.Name = "testbutton";
            this.testbutton.Size = new System.Drawing.Size(75, 23);
            this.testbutton.TabIndex = 3;
            this.testbutton.Text = "Test";
            this.testbutton.UseVisualStyleBackColor = true;
            this.testbutton.Click += new System.EventHandler(this.testbutton_Click);
            // 
            // fourfieldbutton
            // 
            this.fourfieldbutton.Location = new System.Drawing.Point(654, 349);
            this.fourfieldbutton.Name = "fourfieldbutton";
            this.fourfieldbutton.Size = new System.Drawing.Size(111, 41);
            this.fourfieldbutton.TabIndex = 4;
            this.fourfieldbutton.Text = "Christers fyrfältare resultat";
            this.fourfieldbutton.UseVisualStyleBackColor = true;
            this.fourfieldbutton.Click += new System.EventHandler(this.fourfieldbutton_Click);
            // 
            // FormFinance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.fourfieldbutton);
            this.Controls.Add(this.testbutton);
            this.Controls.Add(this.displaybutton);
            this.Controls.Add(this.LBverksamhet);
            this.Controls.Add(this.LBpost);
            this.Name = "FormFinance";
            this.Text = "FormFinance";
            this.Load += new System.EventHandler(this.FormFinance_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox LBpost;
        private System.Windows.Forms.ListBox LBverksamhet;
        private System.Windows.Forms.Button displaybutton;
        private System.Windows.Forms.Button testbutton;
        private System.Windows.Forms.Button fourfieldbutton;
    }
}