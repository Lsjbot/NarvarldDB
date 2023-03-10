namespace NarvarldDB
{
    partial class FormDisplay
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.Quitbutton = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Databutton = new System.Windows.Forms.Button();
            this.Copybutton = new System.Windows.Forms.Button();
            this.Savebutton = new System.Windows.Forms.Button();
            this.TBaxismax = new System.Windows.Forms.TextBox();
            this.TBaxismin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Axisadjustbutton = new System.Windows.Forms.Button();
            this.averagebutton = new System.Windows.Forms.Button();
            this.CBlegend = new System.Windows.Forms.CheckBox();
            this.TBwidth = new System.Windows.Forms.TextBox();
            this.TBheight = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Resizebutton = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.colorbutton = new System.Windows.Forms.Button();
            this.Copytextbutton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // Quitbutton
            // 
            this.Quitbutton.Location = new System.Drawing.Point(681, 756);
            this.Quitbutton.Margin = new System.Windows.Forms.Padding(2);
            this.Quitbutton.Name = "Quitbutton";
            this.Quitbutton.Size = new System.Drawing.Size(101, 47);
            this.Quitbutton.TabIndex = 0;
            this.Quitbutton.Text = "Quit";
            this.Quitbutton.UseVisualStyleBackColor = true;
            this.Quitbutton.Click += new System.EventHandler(this.Quitbutton_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(9, 576);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(351, 227);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(9, 10);
            this.chart1.Margin = new System.Windows.Forms.Padding(2);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedArea;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.YValuesPerPoint = 4;
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(909, 541);
            this.chart1.TabIndex = 2;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.chart1_Click);
            this.chart1.Paint += new System.Windows.Forms.PaintEventHandler(this.chart1_Paint);
            // 
            // Databutton
            // 
            this.Databutton.Location = new System.Drawing.Point(681, 668);
            this.Databutton.Margin = new System.Windows.Forms.Padding(2);
            this.Databutton.Name = "Databutton";
            this.Databutton.Size = new System.Drawing.Size(101, 72);
            this.Databutton.TabIndex = 3;
            this.Databutton.Text = "Select data";
            this.Databutton.UseVisualStyleBackColor = true;
            this.Databutton.Click += new System.EventHandler(this.Databutton_Click);
            // 
            // Copybutton
            // 
            this.Copybutton.Location = new System.Drawing.Point(681, 599);
            this.Copybutton.Margin = new System.Windows.Forms.Padding(2);
            this.Copybutton.Name = "Copybutton";
            this.Copybutton.Size = new System.Drawing.Size(101, 44);
            this.Copybutton.TabIndex = 4;
            this.Copybutton.Text = "Copy chart to clipboard";
            this.Copybutton.UseVisualStyleBackColor = true;
            this.Copybutton.Click += new System.EventHandler(this.Copybutton_Click);
            // 
            // Savebutton
            // 
            this.Savebutton.Location = new System.Drawing.Point(531, 599);
            this.Savebutton.Name = "Savebutton";
            this.Savebutton.Size = new System.Drawing.Size(113, 44);
            this.Savebutton.TabIndex = 5;
            this.Savebutton.Text = "Save chart to image file";
            this.Savebutton.UseVisualStyleBackColor = true;
            this.Savebutton.Click += new System.EventHandler(this.Savebutton_Click);
            // 
            // TBaxismax
            // 
            this.TBaxismax.Location = new System.Drawing.Point(524, 730);
            this.TBaxismax.Name = "TBaxismax";
            this.TBaxismax.Size = new System.Drawing.Size(100, 20);
            this.TBaxismax.TabIndex = 6;
            // 
            // TBaxismin
            // 
            this.TBaxismin.Location = new System.Drawing.Point(524, 756);
            this.TBaxismin.Name = "TBaxismin";
            this.TBaxismin.Size = new System.Drawing.Size(100, 20);
            this.TBaxismin.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(436, 734);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Axis maximum";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(436, 759);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Axis minimum";
            // 
            // Axisadjustbutton
            // 
            this.Axisadjustbutton.Location = new System.Drawing.Point(527, 790);
            this.Axisadjustbutton.Name = "Axisadjustbutton";
            this.Axisadjustbutton.Size = new System.Drawing.Size(86, 23);
            this.Axisadjustbutton.TabIndex = 10;
            this.Axisadjustbutton.Text = "Adjust axes";
            this.Axisadjustbutton.UseVisualStyleBackColor = true;
            this.Axisadjustbutton.Click += new System.EventHandler(this.Axisadjustbutton_Click);
            // 
            // averagebutton
            // 
            this.averagebutton.Location = new System.Drawing.Point(531, 649);
            this.averagebutton.Name = "averagebutton";
            this.averagebutton.Size = new System.Drawing.Size(113, 38);
            this.averagebutton.TabIndex = 12;
            this.averagebutton.Text = "Average non-focus";
            this.averagebutton.UseVisualStyleBackColor = true;
            this.averagebutton.Click += new System.EventHandler(this.averagebutton_Click);
            // 
            // CBlegend
            // 
            this.CBlegend.AutoSize = true;
            this.CBlegend.Checked = true;
            this.CBlegend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBlegend.Location = new System.Drawing.Point(532, 700);
            this.CBlegend.Name = "CBlegend";
            this.CBlegend.Size = new System.Drawing.Size(62, 17);
            this.CBlegend.TabIndex = 13;
            this.CBlegend.Text = "Legend";
            this.CBlegend.UseVisualStyleBackColor = true;
            this.CBlegend.CheckedChanged += new System.EventHandler(this.CBlegend_CheckedChanged);
            // 
            // TBwidth
            // 
            this.TBwidth.Location = new System.Drawing.Point(438, 630);
            this.TBwidth.Name = "TBwidth";
            this.TBwidth.Size = new System.Drawing.Size(87, 20);
            this.TBwidth.TabIndex = 14;
            // 
            // TBheight
            // 
            this.TBheight.Location = new System.Drawing.Point(438, 656);
            this.TBheight.Name = "TBheight";
            this.TBheight.Size = new System.Drawing.Size(87, 20);
            this.TBheight.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(436, 614);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Diagram size";
            // 
            // Resizebutton
            // 
            this.Resizebutton.Location = new System.Drawing.Point(446, 682);
            this.Resizebutton.Name = "Resizebutton";
            this.Resizebutton.Size = new System.Drawing.Size(75, 23);
            this.Resizebutton.TabIndex = 17;
            this.Resizebutton.Text = "Resize";
            this.Resizebutton.UseVisualStyleBackColor = true;
            this.Resizebutton.Click += new System.EventHandler(this.Resizebutton_Click);
            // 
            // colorbutton
            // 
            this.colorbutton.Location = new System.Drawing.Point(394, 790);
            this.colorbutton.Name = "colorbutton";
            this.colorbutton.Size = new System.Drawing.Size(75, 23);
            this.colorbutton.TabIndex = 18;
            this.colorbutton.Text = "Adjust colors";
            this.colorbutton.UseVisualStyleBackColor = true;
            this.colorbutton.Click += new System.EventHandler(this.colorbutton_Click);
            // 
            // Copytextbutton
            // 
            this.Copytextbutton.Location = new System.Drawing.Point(366, 576);
            this.Copytextbutton.Name = "Copytextbutton";
            this.Copytextbutton.Size = new System.Drawing.Size(75, 35);
            this.Copytextbutton.TabIndex = 19;
            this.Copytextbutton.Text = "Copy text to clipboard";
            this.Copytextbutton.UseVisualStyleBackColor = true;
            this.Copytextbutton.Click += new System.EventHandler(this.Copytextbutton_Click);
            // 
            // FormDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 823);
            this.Controls.Add(this.Copytextbutton);
            this.Controls.Add(this.colorbutton);
            this.Controls.Add(this.Resizebutton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TBheight);
            this.Controls.Add(this.TBwidth);
            this.Controls.Add(this.CBlegend);
            this.Controls.Add(this.averagebutton);
            this.Controls.Add(this.Axisadjustbutton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TBaxismin);
            this.Controls.Add(this.TBaxismax);
            this.Controls.Add(this.Savebutton);
            this.Controls.Add(this.Copybutton);
            this.Controls.Add(this.Databutton);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.Quitbutton);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormDisplay";
            this.Text = "FormDisplay";
            this.Load += new System.EventHandler(this.FormDisplay_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Quitbutton;
        private System.Windows.Forms.RichTextBox richTextBox1;
        public System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button Databutton;
        private System.Windows.Forms.Button Copybutton;
        private System.Windows.Forms.Button Savebutton;
        private System.Windows.Forms.TextBox TBaxismax;
        private System.Windows.Forms.TextBox TBaxismin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Axisadjustbutton;
        private System.Windows.Forms.Button averagebutton;
        private System.Windows.Forms.CheckBox CBlegend;
        private System.Windows.Forms.TextBox TBwidth;
        private System.Windows.Forms.TextBox TBheight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Resizebutton;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button colorbutton;
        private System.Windows.Forms.Button Copytextbutton;
    }
}