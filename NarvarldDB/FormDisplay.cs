using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace NarvarldDB
{
    public partial class FormDisplay : Form
    {
        DbTGSAnalysTest db = null;
        public static string nvfolder = @"\\dustaff\home\sja\Documents\Närvärld\";


        int ihda;
        FormSelectData fs;


        public FormDisplay(DbTGSAnalysTest dbpar)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0,0);

            db = dbpar;

            ihda = (from c in db.OV_University where c.Name == "Högskolan Dalarna" select c.Id).FirstOrDefault();
            memo("ihda = " + ihda);
            // Add the chart title, to be filled with content
            chart1.Titles.Add("Title1");
            chart1.Titles["Title1"].Docking = Docking.Top;
            chart1.Titles["Title1"].Font = new Font("Arial", 12);
            chart1.Titles.Add("Source");
            chart1.Titles["Title2"].Docking = Docking.Bottom;
            chart1.Titles["Title2"].Text = "Källa:";
            chart1.Titles["Title2"].Font = new Font("Arial",7);

            TBwidth.Text = chart1.Width.ToString();
            TBheight.Text = chart1.Height.ToString();

            fs = new FormSelectData(db, chart1, this);
            fs.Show();
            fs.totalincome();

        }

        private void Quitbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void memo(string s)
        {
            richTextBox1.AppendText(s + "\n");
            richTextBox1.ScrollToCaret();
        }

        private void Databutton_Click(object sender, EventArgs e)
        {
            fs.BringToFront();
        }

        private void Copybutton_Click(object sender, EventArgs e)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                chart1.SaveImage(ms, ChartImageFormat.Bmp);
                Bitmap bm = new Bitmap(ms);
                Clipboard.SetImage(bm);
            }
        }

        public void Savebutton_Click(object sender, EventArgs e)
        {
            string fn = util.uniquefilename(nvfolder + @"Bilder\"+util.cleanfilename(chart1.Titles["Title1"].Text) + ".png");
            memo("Saving to " + fn);
            chart1.SaveImage(fn, ChartImageFormat.Png);
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Paint(object sender, PaintEventArgs e)
        {
            TBaxismax.Text = chart1.ChartAreas[0].AxisY.Maximum.ToString();
            TBaxismin.Text = chart1.ChartAreas[0].AxisY.Minimum.ToString();
        }

        private void Axisadjustbutton_Click(object sender, EventArgs e)
        {
            double xmax = util.tryconvertdouble(TBaxismax.Text);
            double xmin = util.tryconvertdouble(TBaxismin.Text);
            if (xmax > xmin)
            {
                chart1.ChartAreas[0].AxisY.Maximum = xmax;
                chart1.ChartAreas[0].AxisY.Minimum = xmin;
            }
        }

        private void FormDisplay_Load(object sender, EventArgs e)
        {

        }

        private void averagebutton_Click(object sender, EventArgs e)
        {
            List<Series> focusseries = new List<Series>();
            List<Series> riketseries = new List<Series>();
            Series meanseries = new Series("Genomsnitt övriga");
            Dictionary<int, double> pointdict = new Dictionary<int, double>();

            double nother = 0;// chart1.Series.Count - 2;

            foreach (Series ss in chart1.Series)
            {
                if (ss.Name.Contains(fs.focusname))
                {
                    focusseries.Add(ss);
                }
                else if (ss.Name.ToLower().Contains("riket"))
                {
                    riketseries.Add(ss);
                }
                else
                {
                    nother++;
                    foreach (DataPoint pp in ss.Points)
                    {
                        int year = (int)pp.XValue;
                        if (!pointdict.ContainsKey(year))
                            pointdict.Add(year, pp.YValues[0]);
                        else
                            pointdict[year] += pp.YValues[0];
                    }
                }
            }

            if (nother <= 0)
                return;

            if (focusseries.Count == 0)
                return;

            foreach (int year in pointdict.Keys)
            {
                meanseries.Points.AddXY(year, pointdict[year]/nother);
            }
            meanseries.ChartType = focusseries[0].ChartType;
            meanseries.Points.Last().Label = meanseries.Name; 
            chart1.Series.Clear();
            chart1.Series.Add(meanseries);
            foreach (Series ff in focusseries)
                chart1.Series.Add(ff);
            foreach (Series rr in riketseries)
                chart1.Series.Add(rr);
        }

        private Legend legend = null;

        private void CBlegend_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBlegend.Checked)
            {
                legend = chart1.Legends[0];
                chart1.Legends.Clear();
            }
            else
            {
                if (legend != null)
                    chart1.Legends.Add(legend);
            }
        }

        private void Resizebutton_Click(object sender, EventArgs e)
        {
            int w = util.tryconvert(TBwidth.Text);
            int h = util.tryconvert(TBheight.Text);
            if (w>0 && h>0)
            {
                chart1.Width = w;
                chart1.Height = h;
            }
        }

        private void colorbutton_Click(object sender, EventArgs e)
        {
            FormColor fc = new FormColor(this);
            fc.Show();
        }

        private void Copytextbutton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.Text);
        }
    }
}
