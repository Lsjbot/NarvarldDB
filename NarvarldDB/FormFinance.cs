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


namespace NarvarldDB
{
    public partial class FormFinance : Form
    {
        private DbTGSAnalysTest db;
        FormSelectData select;
        FormDisplay parent;

        public FormFinance(DbTGSAnalysTest dbpar, FormSelectData selpar, FormDisplay parentpar)
        {
            db = dbpar;
            select = selpar;
            parent = parentpar;

            InitializeComponent();

            foreach (OV_financepost ofp in db.OV_financepost)
                LBpost.Items.Add(ofp.Name.PadRight(50)+"§"+ofp.Id);
            LBpost.SelectedIndex = 0;

            foreach (OV_financeverksamhet ofv in db.OV_financeverksamhet)
                LBverksamhet.Items.Add(ofv.Name.PadRight(50) + "§" + ofv.Id);
            LBverksamhet.SelectedIndex = 0;
        }

        private void FormFinance_Load(object sender, EventArgs e)
        {

        }

        private void displaybutton_Click(object sender, EventArgs e)
        {
            int ipost = util.tryconvert(LBpost.SelectedItem.ToString().Split('§')[1]);
            int iact = util.tryconvert(LBverksamhet.SelectedItem.ToString().Split('§')[1]);
            List<int> postlist = new List<int>() { ipost };
            List<int> actlist = new List<int>() { iact };

            if (ipost == 6) //myndighetskapital; lägg till årets förändring
                            //för att få utgående balans
                postlist.Add(7);

            select.totalfinance(postlist, actlist);
        }

        private Random rand = new Random(0);
        private double[] RandomWalk(int points = 5, double start = 100, double mult = 50)
        {
            // return an array of difting random numbers
            double[] values = new double[points];
            values[0] = start;
            for (int i = 1; i < points; i++)
                values[i] = values[i - 1] + (rand.NextDouble() - .5) * mult;
            return values;
        }

        private void scattertest()
        {
            // generate some random XY data
            int pointCount = 100;
            double[] xs1 = RandomWalk(pointCount,0,100);
            double[] ys1 = RandomWalk(pointCount, 0, 100);
            double[] xs2 = RandomWalk(pointCount);
            double[] ys2 = RandomWalk(pointCount);

            // create a series for each line
            Series series1 = new Series("Group A");
            series1.Points.DataBindXY(xs1, ys1);
            series1.ChartType = SeriesChartType.Line;
            series1.MarkerStyle = MarkerStyle.Circle;

            Series series2 = new Series("Group B");
            series2.Points.DataBindXY(xs2, ys2);
            series2.ChartType = SeriesChartType.Line;
            series2.MarkerStyle = MarkerStyle.Circle;

            // add each series to the chart
            parent.chart1.Series.Clear();
            parent.chart1.Series.Add(series1);
            parent.chart1.Series.Add(series2);

            // additional styling
            parent.chart1.ResetAutoValues();
            parent.chart1.Titles.Clear();
            parent.chart1.Titles.Add($"Scatter Plot ({pointCount:N0} points per series)");
            parent.chart1.ChartAreas[0].AxisX.Title = "Horizontal Axis Label";
            parent.chart1.ChartAreas[0].AxisY.Title = "Vertical Axis Label";
            parent.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            parent.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;

            parent.chart1.ChartAreas[0].AxisX.Maximum =  1000;
            parent.chart1.ChartAreas[0].AxisX.Minimum = -1000;
            parent.chart1.ChartAreas[0].AxisY.Maximum = 1000;
            parent.chart1.ChartAreas[0].AxisY.Minimum = -1000;

            parent.chart1.ChartAreas[0].AxisX.Crossing = 0;
            parent.chart1.ChartAreas[0].AxisY.Crossing = 0;
        }

        private void testbutton_Click(object sender, EventArgs e)
        {
            scattertest();
        }


        private void fourfieldbutton_Click(object sender, EventArgs e)
        {
            int plusprodpost = 1;
            int minusprodpost = 2;
            int moneypost = 23;

            select.make_fourfield(plusprodpost, minusprodpost, moneypost);

        }
    }
}
