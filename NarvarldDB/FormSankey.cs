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
    public partial class FormSankey : Form
    {
        private DbTGSAnalysTest db = null;
        private FormDisplay parent = null;
        private FormSelectData selpar = null;
        private Chart chart1 = null;

        public FormSankey(DbTGSAnalysTest dbpar, FormDisplay parentpar, FormSelectData selectpar, Chart chartpar)
        {
            InitializeComponent();

            //Position in lower right corner:
            Screen myScreen = Screen.FromControl(this);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(myScreen.WorkingArea.Width - this.Width, myScreen.WorkingArea.Height - this.Height);

            db = dbpar;
            parent = parentpar;
            selpar = selectpar;
            chart1 = chartpar;

            LB_lan.Items.Add(" --Riket--");
            LB_lan.Items.Add(" --Region Mellansverige--");
            foreach (OV_Lan ol in (from c in db.OV_Lan select c))
                LB_lan.Items.Add(ol.Name);

        }

        private void kompetensforsorjning()
        {
            int startyear = -1;
            int endyear = 9999;

            if (selpar.CB_startyear.SelectedItem != null)
            {
                startyear = util.tryconvert(selpar.CB_startyear.SelectedItem.ToString());
            }
            if (selpar.CB_endyear.SelectedItem != null)
            {
                endyear = util.tryconvert(selpar.CB_endyear.SelectedItem.ToString());
                if (endyear < 0)
                    endyear = 9999;
            }

            parent.memo("Startyear = " + startyear.ToString());
            parent.memo("Endyear = " + endyear.ToString());

            OV_examtype oe = (from c in db.OV_examtype where c.Name == ((string)selpar.LB_exam.SelectedItem).Trim(new char[] { ' ', '(', ')' }) select c).FirstOrDefault();
            parent.memo("oe = " + oe.Id + ", " + oe.Name);

            List<int> mellansverige = new List<int> { 3, 4, 17, 18, 19, 20, 21 };

            foreach (int lan in mellansverige)
            {
                string tolan = (from c in db.OV_Lan where c.Id == lan select c.Name).First();
                Dictionary<string, Dictionary<string, int>> lanunidict = new Dictionary<string, Dictionary<string, int>>();
                Dictionary<string, int> unilandict = new Dictionary<string, int>();
                var q3 = from c in db.OV_studentflow
                         where c.Examtype == oe.Id
                         where c.Tolan == lan
                         where c.Year >= startyear
                         where c.Year <= endyear
                         select c;
                parent.memo("q3.Count = " + q3.Count());
                foreach (OV_studentflow os in q3)
                {
                    if (!lanunidict.ContainsKey(os.OV_Lan.Name))
                        lanunidict.Add(os.OV_Lan.Name, new Dictionary<string, int>());
                    if (!lanunidict[os.OV_Lan.Name].ContainsKey(os.OV_University.Name))
                        lanunidict[os.OV_Lan.Name].Add(os.OV_University.Name, 0);
                    lanunidict[os.OV_Lan.Name][os.OV_University.Name] += os.Number;

                    if (!unilandict.ContainsKey(os.OV_University.Name))
                        unilandict.Add(os.OV_University.Name, os.Number);
                    else
                        unilandict[os.OV_University.Name] += os.Number;
                }
                
                foreach (string fromlan in lanunidict.Keys)
                {
                    foreach (string uni in lanunidict[fromlan].Keys)
                        memo("Från " + fromlan + " [" + lanunidict[fromlan][uni] + "] " + uni);
                }
                memo("===========================================================");
                foreach (string uni in unilandict.Keys)
                {
                    memo(uni + " [" + unilandict[uni] + "] Till " + tolan);
                }
                memo("===========================================================");
                memo("===========================================================");
                //foreach (string fromuni in ndict.Keys)
                //{
                //    parent.memo("Från " + fromuni + " [" + ndict[fromuni] + "] " + "Till Dalarnas län");
                //}
                //parent.memo("oe = " + oe.Id + ", " + oe.Name);
                //ndict.Clear();

            }

            Dictionary<string, int> frommellan = new Dictionary<string, int>();
            Dictionary<string, int> tomellan = new Dictionary<string, int>();
            Dictionary<string, int> fromother = new Dictionary<string, int>();
            Dictionary<string, int> toother = new Dictionary<string, int>();

            var q4 = from c in db.OV_studentflow
                     where c.Examtype == oe.Id
                     where c.Year >= startyear
                     where c.Year <= endyear
                     select c;

            foreach (OV_studentflow os in q4)
            {
                string uniname = os.OV_University.Name;
                if (mellansverige.Contains(os.Fromlan))
                {
                    if (!frommellan.ContainsKey(uniname))
                        frommellan.Add(uniname, 0);
                    frommellan[uniname] += os.Number;
                }
                else
                {
                    if (!fromother.ContainsKey(uniname))
                        fromother.Add(uniname, 0);
                    fromother[uniname] += os.Number;
                }
                if (mellansverige.Contains(os.Tolan))
                {
                    if (!tomellan.ContainsKey(uniname))
                        tomellan.Add(uniname, 0);
                    tomellan[uniname] += os.Number;
                }
                else
                {
                    if (!toother.ContainsKey(uniname))
                        toother.Add(uniname, 0);
                    toother[uniname] += os.Number;
                }
            }

            foreach (string uniname in frommellan.Keys)
            {
                memo("Från Mellansverige [" + frommellan[uniname] + "] " + uniname);
                memo("Från övriga Sverige [" + fromother[uniname] + "] " + uniname);
                if (tomellan.ContainsKey(uniname))
                    memo(uniname + " [" + tomellan[uniname] + "] Till Mellansverige");
                else
                    tomellan.Add(uniname, 0);
                memo(uniname + " [" + toother[uniname] + "] Till övriga Sverige");
            }
            memo("===========================================================");
            memo("======================"+oe.Name+"==================================");

            memo(oe.Name + "\tFrån Mellansverige\tFrån övriga\tTill Mellansverige\tTill övriga");
            foreach (string uniname in frommellan.Keys)
            {
                memo(uniname + "\t" + frommellan[uniname] + "\t" + fromother[uniname] + "\t" + tomellan[uniname] + "\t" + toother[uniname]);
            }

        }

        private void oldsankey()
        {
            int startyear = -1;
            int endyear = 9999;

            if (selpar.CB_startyear.SelectedItem != null)
            {
                startyear = util.tryconvert(selpar.CB_startyear.SelectedItem.ToString());
            }
            if (selpar.CB_endyear.SelectedItem != null)
            {
                endyear = util.tryconvert(selpar.CB_endyear.SelectedItem.ToString());
                if (endyear < 0)
                    endyear = 9999;
            }

            parent.memo("Startyear = " + startyear.ToString());
            parent.memo("Endyear = " + endyear.ToString());

            if (selpar.LB_exam.SelectedItem == null)//All students at focus university
            {
                //int year = 2013;
                var qq = from c in db.OV_studentflow where c.Year >= startyear where c.Year <= endyear select c;
                //var qq = from c in db.OV_studentflow where c.Uni == focusuniversity where c.Year >= startyear where c.Year <= endyear select c;
                int nlan = 30;
                int[,] lantable = new int[nlan, nlan];
                Dictionary<int, string> landict = new Dictionary<int, string>();
                foreach (OV_Lan ol in (from c in db.OV_Lan select c))
                    landict.Add(ol.Id, ol.Name);
                for (int i = 0; i < nlan; i++)
                    for (int j = 0; j < nlan; j++)
                        lantable[i, j] = 0;
                foreach (OV_studentflow os in qq)
                {
                    lantable[os.Fromlan, os.Tolan] += os.Number;
                }

                int sum = 0;
                for (int i = 0; i < nlan; i++)
                    for (int j = 0; j < nlan; j++)
                        if (lantable[i, j] > 0)
                        {
                            parent.memo("Från " + landict[i] + " [" + lantable[i, j] + "] " + "Till " + landict[j]);
                            sum += lantable[i, j];
                        }
                parent.memo("Sum = " + sum);
            }
            else //Do 3 diagrams: 
            // all with selected exam at focus uni, 
            // all from selected län with exam, which uni
            // all working in selected län with exam, which uni
            {

                Dictionary<string, int> ndict = new Dictionary<string, int>();
                OV_examtype oe = (from c in db.OV_examtype where c.Name == ((string)selpar.LB_exam.SelectedItem).Trim(new char[]{' ','(',')'}) select c).FirstOrDefault();
                parent.memo("oe = " + oe.Id + ", " + oe.Name);
                var q1 = from c in db.OV_studentflow
                         where c.Examtype == oe.Id
                         where c.Uni == selpar.focusuniversity
                         //where c.Year >= startyear
                         //where c.Year <= endyear 
                         select c;
                parent.memo("q1.Count = " + q1.Count());
                foreach (OV_studentflow os in q1)
                {
                    parent.memo("Från " + os.OV_Lan.Name + " [" + os.Number + "] " + "Till " + os.TolanOV_Lan.Name);
                }
                foreach (OV_studentflow os in q1)
                {
                    parent.memo("Year = " + os.Year);
                }
                parent.memo("==================================================");
                var q2 = from c in db.OV_studentflow
                         where c.Examtype == oe.Id
                         where c.Fromlan == 20
                         where c.Year >= startyear
                         where c.Year <= endyear
                         select c;
                parent.memo("q2.Count = " + q2.Count());
                foreach (OV_studentflow os in q2)
                {
                    if (!ndict.ContainsKey(os.OV_University.Name))
                        ndict.Add(os.OV_University.Name, os.Number);
                    else
                        ndict[os.OV_University.Name] += os.Number;

                }
                foreach (string touni in ndict.Keys)
                {
                    parent.memo("Från Dalarnas län [" + ndict[touni] + "] " + "Till " + touni);
                }
                ndict.Clear();
                parent.memo("==================================================");
                var q3 = from c in db.OV_studentflow
                         where c.Examtype == oe.Id
                         where c.Tolan == 20
                         where c.Year >= startyear
                         where c.Year <= endyear
                         select c;
                parent.memo("q3.Count = " + q3.Count());
                foreach (OV_studentflow os in q3)
                {
                    if (!ndict.ContainsKey(os.OV_University.Name))
                        ndict.Add(os.OV_University.Name, os.Number);
                    else
                        ndict[os.OV_University.Name] += os.Number;

                }
                foreach (string fromuni in ndict.Keys)
                {
                    parent.memo("Från " + fromuni + " [" + ndict[fromuni] + "] " + "Till Dalarnas län");
                }
                parent.memo("oe = " + oe.Id + ", " + oe.Name);
                ndict.Clear();
                parent.memo("==================================================");
                var q4 = from c in db.OV_studentflow
                         where c.Examtype == oe.Id
                         where c.Year >= startyear
                         where c.Year <= endyear
                         where ((c.Fromlan == 20) || (c.Tolan == 20))
                         select c;
                parent.memo("q4.Count = " + q4.Count());
                Dictionary<string, int> fromdalarnadict = new Dictionary<string, int>();
                Dictionary<string, int> fromotherdict = new Dictionary<string, int>();
                Dictionary<string, int> todalarnadict = new Dictionary<string, int>();
                Dictionary<string, int> tootherdict = new Dictionary<string, int>();
                Dictionary<string, string> netdict = new Dictionary<string, string>();
                foreach (OV_studentflow os in q4)
                {
                    if (os.Fromlan == 20)
                    {
                        if (!fromdalarnadict.ContainsKey(os.OV_University.Name))
                            fromdalarnadict.Add(os.OV_University.Name, os.Number);
                        else
                            fromdalarnadict[os.OV_University.Name] += os.Number;
                    }
                    else
                    {
                        if (!fromotherdict.ContainsKey(os.OV_University.Name))
                            fromotherdict.Add(os.OV_University.Name, os.Number);
                        else
                            fromotherdict[os.OV_University.Name] += os.Number;
                    }
                    if (os.Tolan == 20)
                    {
                        if (!todalarnadict.ContainsKey(os.OV_University.Name))
                            todalarnadict.Add(os.OV_University.Name, os.Number);
                        else
                            todalarnadict[os.OV_University.Name] += os.Number;
                    }
                    else
                    {
                        if (!tootherdict.ContainsKey(os.OV_University.Name))
                            tootherdict.Add(os.OV_University.Name, os.Number);
                        else
                            tootherdict[os.OV_University.Name] += os.Number;
                    }
                }

                foreach (string uni in fromdalarnadict.Keys)
                {
                    if (todalarnadict.ContainsKey(uni))
                    {
                        int net = todalarnadict[uni] - fromdalarnadict[uni];
                        string netstring = net.ToString();
                        if (net > 0)
                            netstring = "+" + netstring;
                        netdict.Add(uni, uni + " (" + netstring + ")");
                    }
                    else
                        netdict.Add(uni, "(-" + fromdalarnadict[uni].ToString() + ")");
                }
                foreach (string uni in todalarnadict.Keys)
                {
                    if (!netdict.ContainsKey(uni))
                        netdict.Add(uni, "(+" + todalarnadict[uni].ToString() + ")");
                }

                foreach (string uni in fromotherdict.Keys)
                {
                    parent.memo("Från övriga län [" + fromotherdict[uni] + "] " + netdict[uni]);
                }
                foreach (string uni in fromdalarnadict.Keys)
                {
                    parent.memo("Från Dalarna [" + fromdalarnadict[uni] + "] " + netdict[uni]);
                }
                foreach (string uni in todalarnadict.Keys)
                {
                    parent.memo(netdict[uni] + " [" + todalarnadict[uni] + "] Till Dalarna");
                }
                foreach (string uni in tootherdict.Keys)
                {
                    parent.memo(netdict[uni] + " [" + tootherdict[uni] + "] Till övriga län");
                }

            }

        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void memo(string s)
        {
            richTextBox1.AppendText(s + "\n");
            richTextBox1.ScrollToCaret();
        }

        private void Importbutton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            chart1.Series.Clear();
            chart1.Titles["Title1"].Text = "Nettoflöde in/ut av högskoleutbildade i förhållande till alla från länet som tar examen";
            chart1.ChartAreas[0].AxisY.Title = "% nettoflöde per län/region";

            int startyear = (from c in db.OV_studentflow where c.Year > 0 select c.Year).Min();
            int endyear = (from c in db.OV_studentflow select c.Year).Max();
            
            if (selpar.CB_startyear.SelectedItem != null)
            {
                startyear = util.tryconvert(selpar.CB_startyear.SelectedItem.ToString());
            }
            if (selpar.CB_endyear.SelectedItem != null)
            {
                endyear = util.tryconvert(selpar.CB_endyear.SelectedItem.ToString());
                if (endyear < 0)
                    endyear = DateTime.Now.Year;
            }



            parent.memo("Startyear = " + startyear.ToString());
            parent.memo("Endyear = " + endyear.ToString());
            Dictionary<int,Series> ssdict = new Dictionary<int,Series>();
            foreach (OV_Lan lan in (from c in db.OV_Lan select c))
            {
                ssdict.Add(lan.Id,new Series(lan.Name));
            }

            ssdict.Add(101, new Series("Stockholm"));
            ssdict.Add(102, new Series("Övriga storstadslän"));
            ssdict.Add(103, new Series("Norrland"));
            ssdict.Add(104, new Series("Övriga"));
            ssdict.Add(105, new Series("Uppsala, Östergötland"));

            Dictionary<int, int> lanregion = new Dictionary<int, int>();
            lanregion.Add(1, 101);
            lanregion.Add(2, 101);
            lanregion.Add(3, 105);
            lanregion.Add(4, 104);
            lanregion.Add(5, 105);
            lanregion.Add(6, 104);
            lanregion.Add(7, 104);
            lanregion.Add(8, 104);
            lanregion.Add(9, 104);
            lanregion.Add(10, 104);
            lanregion.Add(11, 101);
            lanregion.Add(12, 102);
            lanregion.Add(13, 104);
            lanregion.Add(14, 102);
            lanregion.Add(15, 101);
            lanregion.Add(16, 101);
            lanregion.Add(17, 104);
            lanregion.Add(18, 104);
            lanregion.Add(19, 104);
            lanregion.Add(20, 104);
            lanregion.Add(21, 103);
            lanregion.Add(22, 103);
            lanregion.Add(23, 103);
            lanregion.Add(24, 103);
            lanregion.Add(25, 103);
            
            double ssmax = 0;
            double ssmin = 0;

            parent.memo("year loop");
            //parent.memo("q.Count = " + q.Count());
            for (int year = startyear; year <= endyear; year++)
            {
                Dictionary<int, int> homestay = new Dictionary<int, int>();
                Dictionary<int, int> inflow = new Dictionary<int, int>();
                Dictionary<int, int> outflow = new Dictionary<int, int>();
                foreach (int ilan in (from c in db.OV_Lan select c.Id))
                {
                    homestay.Add(ilan, 0);
                    inflow.Add(ilan, 0);
                    outflow.Add(ilan, 0);
                }
                for (int i=101;i<110;i++)
                {
                    homestay.Add(i, 0);
                    inflow.Add(i, 0);
                    outflow.Add(i, 0);
                }


                var q = from c in db.OV_studentflow where c.Year == year select c;
                foreach (OV_studentflow os in q)
                {
                    if (os.Tolan == os.Fromlan)
                    {
                        homestay[os.Tolan] += os.Number;
                        homestay[lanregion[os.Tolan]] += os.Number;
                    }
                    else
                    {
                        outflow[os.Fromlan] += os.Number;
                        inflow[os.Tolan] += os.Number;
                        outflow[lanregion[os.Fromlan]] += os.Number;
                        inflow[lanregion[os.Tolan]] += os.Number;
                    }
                }
            

                foreach (int id in ssdict.Keys)
                {
                    //Series ss = new Series(lan.Name);

                    if (id < 100)
                        continue;
                    //parent.memo("Year = " + year);
                    double amount = 100*(inflow[id]-outflow[id])/((double)homestay[id]+(double)outflow[id]);
                    //amount = amount / yearreference;
                    //if (CB_fraction.Checked)
                    //    amount = amount / refdict[year];
                    ssdict[id].Points.AddXY(year, amount);
                    if (amount > ssmax)
                        ssmax = amount;
                    if (amount < ssmin)
                        ssmin = amount;
                    //if (CB_memo.Checked)
                    //    parent.memo(year + "\t" + amount);
                }
            }

            memo("ssmin, ssmax = " + ssmin + ", " + ssmax);

            foreach (int id in ssdict.Keys)
            {
                if (id < 100)
                    continue;
                ssdict[id].ChartType = SeriesChartType.Line;
                chart1.Series.Add(ssdict[id]);
            }

            //double ssdef = 0.2;
            //ssmax = Math.Max(ssmax, Math.Abs(ssmin));
            //if (ssmax < ssdef)
            //    ssmax = ssdef;
            //if (ssmin < 0)
            //    ssmin = -Math.Max(ssmax, Math.Abs(ssmin));

            //if (ssmin < 0)
            //{
            //    double diff = ssmax - ssmin;
            //    double newdiff = 2*selpar.roundaxis(diff);
            //    if (ssmax > Math.Abs(ssmin))
            //    {
            //        ssmax = 0.6 * newdiff;
            //        ssmin = -0.4 * newdiff;
            //    }
            //    else
            //    {
            //        ssmax = 0.4 * newdiff;
            //        ssmin = -0.6 * newdiff;
            //    }
            //}
            //else
            //    ssmax = selpar.roundaxis(ssmax);
            ssmax = 90;
            ssmin = -60;
            memo("ssmin, ssmax = " + ssmin + ", " + ssmax);

            chart1.ChartAreas[0].AxisY.Maximum = ssmax;
            chart1.ChartAreas[0].AxisY.Minimum = ssmin;
            parent.memo("end of Importbutton");
            this.Cursor = Cursors.Default;

        }

        private void oldsankeybutton_Click(object sender, EventArgs e)
        {
            oldsankey();
        }

        private void mellansverigebutton_Click(object sender, EventArgs e)
        {
            kompetensforsorjning();
        }
    }
}
