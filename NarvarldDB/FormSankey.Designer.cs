namespace NarvarldDB
{
    partial class FormSankey
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.CloseButton = new System.Windows.Forms.Button();
            this.LB_lan = new System.Windows.Forms.CheckedListBox();
            this.Importbutton = new System.Windows.Forms.Button();
            this.oldsankeybutton = new System.Windows.Forms.Button();
            this.mellansverigebutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(15, 417);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(540, 322);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(597, 678);
            this.CloseButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(100, 60);
            this.CloseButton.TabIndex = 3;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // LB_lan
            // 
            this.LB_lan.FormattingEnabled = true;
            this.LB_lan.Location = new System.Drawing.Point(538, 28);
            this.LB_lan.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LB_lan.Name = "LB_lan";
            this.LB_lan.Size = new System.Drawing.Size(159, 106);
            this.LB_lan.TabIndex = 4;
            // 
            // Importbutton
            // 
            this.Importbutton.Location = new System.Drawing.Point(548, 152);
            this.Importbutton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Importbutton.Name = "Importbutton";
            this.Importbutton.Size = new System.Drawing.Size(149, 52);
            this.Importbutton.TabIndex = 5;
            this.Importbutton.Text = "Nettoimport per län";
            this.Importbutton.UseVisualStyleBackColor = true;
            this.Importbutton.Click += new System.EventHandler(this.Importbutton_Click);
            // 
            // oldsankeybutton
            // 
            this.oldsankeybutton.Location = new System.Drawing.Point(548, 212);
            this.oldsankeybutton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.oldsankeybutton.Name = "oldsankeybutton";
            this.oldsankeybutton.Size = new System.Drawing.Size(149, 50);
            this.oldsankeybutton.TabIndex = 6;
            this.oldsankeybutton.Text = "Sankey för viss examen";
            this.oldsankeybutton.UseVisualStyleBackColor = true;
            this.oldsankeybutton.Click += new System.EventHandler(this.oldsankeybutton_Click);
            // 
            // mellansverigebutton
            // 
            this.mellansverigebutton.Location = new System.Drawing.Point(548, 269);
            this.mellansverigebutton.Name = "mellansverigebutton";
            this.mellansverigebutton.Size = new System.Drawing.Size(149, 54);
            this.mellansverigebutton.TabIndex = 7;
            this.mellansverigebutton.Text = "Kompetensförsörjning Mellansverige";
            this.mellansverigebutton.UseVisualStyleBackColor = true;
            this.mellansverigebutton.Click += new System.EventHandler(this.mellansverigebutton_Click);
            // 
            // FormSankey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 753);
            this.Controls.Add(this.mellansverigebutton);
            this.Controls.Add(this.oldsankeybutton);
            this.Controls.Add(this.Importbutton);
            this.Controls.Add(this.LB_lan);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.richTextBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormSankey";
            this.Text = "FormSankey";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.CheckedListBox LB_lan;
        private System.Windows.Forms.Button Importbutton;
        private System.Windows.Forms.Button oldsankeybutton;
        private System.Windows.Forms.Button mellansverigebutton;
    }
}