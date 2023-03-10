using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NarvarldDB
{
    public partial class FormSelectData : Form
    {
        private DbTGSAnalysTest db = null;
        private Chart chart1;
        public int ihda;
        public int focusuniversity;
        public static string hda = "Högskolan Dalarna";
        public string focusname;
        public int focusthickness;
        public int linethickness;
        private FormDisplay parent = null;

        public Dictionary<string, int> unidict = new Dictionary<string, int>();
        public Dictionary<int, int> unialiasdict = new Dictionary<int, int>();
        private Dictionary<string, string> unishortdict = new Dictionary<string, string>();
        private Dictionary<int, Dictionary<string, float>> pricedict = new Dictionary<int, Dictionary<string, float>>();
        private Dictionary<string, char> examgroupdict = 
            new Dictionary<string, char>() { 
            { "--Alla examina efter nivå",'E'},
            { "--Alla lärarutbildningar", 'L' }, 
            { "--Alla vårdutbildningar", 'V' }, 
            { "--Alla specialistsjuksköterskor", 'S' }, 
            { "--Alla teknikutbildningar", 'T' }, 
            { "--Alla civilingenjörsutbildningar", 'C' }, 
            { "--Alla forskarutbildningar", 'F' }};
        private Dictionary<string, string> pubtypedict = new Dictionary<string, string>() {
            {"art","Artikel"},
            {"ovr","Övrigt"},
            {"rec","Recension"},
            {"pro","Proceedings"},
            {"for","Forskningsöversikt"},
            {"dok","Doktorsavhandling"},
            {"sam","Samlingsverk"},
            {"kon","Konferensbidrag"},
            {"pat","Patent"},
            {"rap","Rapport"},
            {"lic","Licentiatavhandling"},
            {"kfu","Konstnärligt"},
            {"bok","Bok"},
            {"kap","Bokkapitel"}
        };

        public static List<string> pubtypelistUKA = new List<string>(){
            {"Akademiska avhandlingar"},
            {"Artikel i vetenskaplig tidskrift"},
            {"Böcker på svenska"},
            {"Böcker på Övriga språk"},
            {"Uppgift saknas"},
            {"Övriga publikationer"}
            };

        private Dictionary<string, List<string>> pubsweUKAdict = new Dictionary<string, List<string>>()//from UKÄ pubtype to list of Swepub pubtypes
            {
            {"Akademiska avhandlingar",new List<string>(){"publication/doctoral-thesis","publication/licentiate-thesis"}},
            {"Artikel i vetenskaplig tidskrift",new List<string>(){"publication/journal-article","publication/journal-article:ref","publication/journal-article:nonref"}},
            {"Böcker på svenska",new List<string>(){ "publication/book:swe"}},
            {"Böcker på Övriga språk",new List<string>(){"publication/book:other","publication/edited-book"}},
            {"Uppgift saknas",new List<string>(){"publication"}},
            {"Övriga publikationer",new List<string>(){"conference",
            "conference/other",
            "conference/paper",
            "conference/poster",
            "conference/proceeding",
            "publication/book-chapter",
            "publication/book-review",
            "publication/critical-edition",
            "publication/editorial-letter",
            "publication/encyclopedia-entry",
            "publication/foreword-afterword",
            "publication/journal-issue",
            "publication/magazine-article",
            "publication/newspaper-article",
            "publication/other",
            "publication/preprint",
            "publication/report",
            "publication/report-chapter",
            "publication/review-article",
            "publication/working-paper"}}
            //{"Akademiska avhandlingar",new List<string>(){"dok","lic"}},
            //{"Artikel i vetenskaplig tidskrift",new List<string>(){"art"}},
            //{"Böcker på svenska",new List<string>(){"bok","pro","sam"}},
            //{"Böcker på Övriga språk",new List<string>()},
            //{"Uppgift saknas",new List<string>(){"ovr"}},
            //{"Övriga publikationer",new List<string>(){"rec","for","kon","pat","rap","kfu","kap"}}
            };

        

        public static Dictionary<string, string> sourcedict = new Dictionary<string, string>();
        public static Dictionary<string, string> sourceurldict = new Dictionary<string, string>();
        public static Dictionary<string, string> subjectsynonyms = new Dictionary<string, string>() { 
        {"Humanistiskt","Humaniora"},
        {"Samhällsvetenskapligt","Samhällsvetenskap"},
        {"Naturvetenskapligt","Naturvetenskap"},
        {"Tekniskt","Teknik"},
        {"Medicinskt","Medicin"},
        {"Verksamhetsförlagd utbildning","Verksamhetsförlagd utb."},
        {"Juridiskt","Juridik"},
        {"Farmaceutiskt","Farmaci"},
        {"Odontologiskt","Odontologi"},
        {"Teologiskt","Teologi"}
        };

        public FormSelectData(DbTGSAnalysTest dbpar, Chart chartpar, FormDisplay parentpar)
        {
            InitializeComponent();

            //Position in lower right corner:
            Screen myScreen = Screen.FromControl(this);
            this.StartPosition = FormStartPosition.Manual;
            int xpos = myScreen.WorkingArea.Width - this.Width;
            int ypos = myScreen.WorkingArea.Height - this.Height;
            if (xpos < 0)
                xpos = 0;
            if (ypos < 0)
                ypos = 0;
            this.Location = new Point(xpos, ypos);
            

            db = dbpar;
            parent = parentpar;
            chart1 = chartpar;
            ihda = (from c in db.OV_University where c.Name == hda select c.Id).FirstOrDefault();
            focusuniversity = ihda;
            focusname = hda;
            focuslabel.Text = focusname;
            focusthickness = 5;
            linethickness = 2;

            var q = (from c in db.OV_University select c);
            foreach (OV_University ou in q)
            {
                if ( ou.Mergedwith == null)
                {
                    LB_uni.Items.Add(ou.Name);
                    unidict.Add(ou.Name, ou.Id);
                    if (ou.Swepubcode != null)
                        unishortdict.Add(ou.Name, ou.Swepubcode);
                    else
                        unishortdict.Add(ou.Name, ou.Name);
                }
                else
                {
                    unialiasdict.Add(ou.Id, (int)ou.Mergedwith);
                }
            }
            var qtype = from c in db.OV_Incometype select c;
            foreach (OV_Incometype oi in qtype)
            {
                LB_incometype.Items.Add(oi.Name);
            }
            var qsource = from c in db.OV_Incomesource select c;
            foreach (OV_Incomesource oi in qsource)
            {
                LB_incomesource.Items.Add(oi.Name);
            }

            var qarea = from c in db.OV_subjectarea select c;
            foreach (OV_subjectarea oa in qarea)
            {
                if (!subjectsynonyms.ContainsKey(oa.Name))
                    LB_subjectarea.Items.Add(oa.Name);
            }

            LB_exam.Items.Add("    Totalt grund&avancerad");
            var qexam = from c in db.OV_examtype select c;
            string[] pad = new string[] { "  ", " ", "" };
            foreach (OV_examtype oe in qexam)
            {
                if (oe.Kolumn >= 0)
                    LB_exam.Items.Add(pad[oe.Kolumn] + oe.Name);
                else
                    LB_exam.Items.Add("(" + oe.Name + ")");
            }
            foreach (string s in examgroupdict.Keys)
                LB_exam.Items.Add(s);


            LB_staff.Items.Add("Andel forskarutbildade");
            LB_staff.Items.Add("Andel stödpersonal");
            //LB_staff.Items.Add("");
            var qstaff = from c in db.OV_stafftype select c;
            foreach (OV_stafftype os in qstaff)
            {
                LB_staff.Items.Add(os.Name);
            }

            var qsector = from c in db.OV_mysector select c;
            foreach (OV_mysector os in qsector)
            {
                LB_lsjsubject.Items.Add(os.Name);
                foreach (OV_mysubject oss in os.OV_mysubject)
                    LB_lsjsubject.Items.Add(" - " + oss.Name);
            }

            CB_startyear.Items.Add(" no year");
            for (int i = 1973; i < 2024; i++)
                CB_startyear.Items.Add(i);
            CB_endyear.Items.Add(" no year");
            for (int i = 1973; i < 2024; i++)
                CB_endyear.Items.Add(i);
        }

        private void Trendline()
        {
            chart1.Series.Add("TrendLine");
            chart1.Series["TrendLine"].ChartType = SeriesChartType.Line;
            chart1.Series["TrendLine"].BorderWidth = 3;
            chart1.Series["TrendLine"].Color = Color.Red;
            // Line of best fit is linear
            string typeRegression = "Linear";//"Exponential";//
            // The number of days for Forecasting
            string forecasting = "1";
            // Show Error as a range chart.
            string error = "false";
            // Show Forecasting Error as a range chart.
            string forecastingError = "false";
            // Formula parameters
            string parameters = typeRegression + ',' + forecasting + ',' + error + ',' + forecastingError;
            chart1.Series[0].Sort(PointSortOrder.Ascending, "X");
            // Create Forecasting Series.
            chart1.DataManipulator.FinancialFormula(FinancialFormula.Forecasting, parameters, chart1.Series[0], chart1.Series["TrendLine"]);
        }

        private void SaveValuesToFile(StringBuilder sb, string title)
        {
            string fn = util.uniquefilename(title + ".txt");
            using (StreamWriter sw = new StreamWriter(fn))
            {
                sw.WriteLine(sb.ToString());
            }
        }

        private void SaveValuesToFile(Chart chart1)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Series ss in chart1.Series)
            {
                foreach (DataPoint dp in ss.Points)
                    sb.Append(ss.Name + "\t"+ dp.XValue + "\t"+ dp.YValues[0]+"\n");
            }

            SaveValuesToFile(sb, chart1.Titles["Title1"].Text);
        }

        public string getuni(int iuni)
        {
            if (iuni == 0)
                return "";
            else if (unialiasdict.ContainsKey(iuni))
                return getuni(unialiasdict[iuni]);
            var q = from c in unidict.Keys where unidict[c] == iuni select c;
            return q.FirstOrDefault();
        }

        public string getunishort(int iuni)
        {
            string uni = getuni(iuni);
            if (unishortdict.ContainsKey(uni))
                return unishortdict[uni].ToUpper();
            else
                return "";
        }

        private static void fill_sourcedict()
        {
            string s = "";
            s = "OV_antagningspoang"; sourcedict.Add(s, "UHRs antagningsstatistik"); sourceurldict.Add(s, "http://statistik.uhr.se/");
            s = "OV_applicants"; sourcedict.Add(s, "UHRs antagningsstatistik"); sourceurldict.Add(s, "http://statistik.uhr.se/");
            s = "OV_course"; sourcedict.Add(s, "UHRs antagningsstatistik"); sourceurldict.Add(s, "http://statistik.uhr.se/");
            s = "OV_demography"; sourcedict.Add(s, "SCBs statistikdatabas"); sourceurldict.Add(s, "http://www.statistikdatabasen.scb.se/pxweb/sv/ssd/");
            s = "OV_establishment"; sourcedict.Add(s, "Bak- och framgrund (Ladok/SCB)"); sourceurldict.Add(s, "");
            s = "OV_exam"; sourcedict.Add(s, "UKÄs statistikdatabas"); sourceurldict.Add(s, "https://www.uka.se/statistik--analys/statistikdatabas-hogskolan-i-siffror.html");
            s = "OV_hsthpr"; sourcedict.Add(s, "UKÄs statistikdatabas"); sourceurldict.Add(s, "https://www.uka.se/statistik--analys/statistikdatabas-hogskolan-i-siffror.html");
            s = "OV_income"; sourcedict.Add(s, "Bak- och framgrund (Ladok/SCB)"); sourceurldict.Add(s, "");
            s = "OV_price"; sourcedict.Add(s, "SCBs statistikdatabas"); sourceurldict.Add(s, "http://www.statistikdatabasen.scb.se/pxweb/sv/ssd/");
            s = "OV_publication"; sourcedict.Add(s, "UKÄ, SwePub"); sourceurldict.Add(s, "https://www.kb.se/samverkan-och-utveckling/swepub/data-access.html");
            s = "OV_sjuk"; sourcedict.Add(s, "Statskontoret"); sourceurldict.Add(s, "https://www.statskontoret.se/siteassets/dokument/exceldokument/stkt-sjfv-oppen-data-2020-f02-2.xlsx");
            s = "OV_staff"; sourcedict.Add(s, "UKÄs statistikdatabas"); sourceurldict.Add(s, "https://www.uka.se/statistik--analys/statistikdatabas-hogskolan-i-siffror.html");
            s = "OV_studentbackground"; sourcedict.Add(s, "Bak- och framgrund (Ladok/SCB)"); sourceurldict.Add(s, "");
            s = "OV_studentcohort"; sourcedict.Add(s, "Bak- och framgrund (Ladok/SCB)"); sourceurldict.Add(s, "");
            s = "OV_studentflow"; sourcedict.Add(s, "Bak- och framgrund (Ladok/SCB)"); sourceurldict.Add(s, "");
            s = "OV_University_Income"; sourcedict.Add(s, "UKÄs statistikdatabas"); sourceurldict.Add(s, "https://www.uka.se/statistik--analys/statistikdatabas-hogskolan-i-siffror.html");
            s = "OV_VRbibliometry"; sourcedict.Add(s, "Vetenskapsrådet"); sourceurldict.Add(s, "https://www.vr.se/analys/vi-analyserar-och-utvarderar/bibliometri.html");
            s = "OV_registered"; sourcedict.Add(s, "UKÄs statistikdatabas"); sourceurldict.Add(s, "https://www.uka.se/statistik--analys/statistikdatabas-hogskolan-i-siffror.html");
            s = "OV_finance"; sourcedict.Add(s, "UKÄs statistikdatabas"); sourceurldict.Add(s, "https://www.uka.se/statistik--analys/statistikdatabas-hogskolan-i-siffror.html");
            //s = "OV_"; sourcedict.Add(s, ""); sourceurldict.Add(s, "");
        }

        public static string getsource(string[] dblist,bool withurl)
        {
            if (sourcedict.Count == 0)
                fill_sourcedict();

            string s = "Källa: ";
            foreach (string ss in dblist)
            {
                if (sourcedict.ContainsKey(ss))
                {
                    if (!s.Contains(sourcedict[ss]))
                    {
                        s += sourcedict[ss] + " ";
                        if (withurl)
                            s += sourceurldict[ss] + " ";
                    }
                }
                else
                    s += "okänd källa " + ss + " ";
            }
            return s;
        }

        private void Quitbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public double getappscore(int year, int iuni, int urvalsgrupp, bool median)
        {
            IQueryable<float?> q;
            if (iuni > 0)
            {
                q = from c in db.OV_antagningspoang
                    where c.Urvalsgrupp == urvalsgrupp
                    where c.OV_course.Uni == iuni
                    where c.OV_course.Year == year
                    orderby c.Number
                    select c.Number;
            }
            else
            {
                q = from c in db.OV_antagningspoang
                    where c.Urvalsgrupp == urvalsgrupp
                    where c.OV_course.Year == year
                    orderby c.Number
                    select c.Number;
            }
            if (median)
            {
                int mid = q.Count() / 2;
                double? score = 0;
                if (q.Count() % 2 == 0)
                {
                    score = 0.5 * (q.Skip(mid).FirstOrDefault() + q.Skip(mid + 1).FirstOrDefault());
                }
                else
                {
                    score = q.Skip(mid).FirstOrDefault();
                }
                if (score != null)
                    return (double)score;
                else
                    return 0;
            }
            else
            {
                var q1 = from c in q where c > 0 select c;
                double ratio = q1.Count();
                parent.memo("q1,q: " + q1.Count() + ", " + q.Count());
                if (q.Count() > 0)
                    return ratio / q.Count();
                else
                    return 0;
            }
        }

        public void totalappscore(int urvalsgrupp, int isector, int isubject, bool median)
        {
            this.Cursor = Cursors.WaitCursor;

            parent.memo(urvalsgrupp + ", " + isector + ", " + isubject);
            //int minyear = (from c in db.OV_course select c.Year).Min();
            int minyear = 2013;
            int maxyear = (from c in db.OV_course select c.Year).Max();
            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            double ssmax = 0;

            string ugcode = (from c in db.OV_urvalsgrupp where c.Id == urvalsgrupp select c.Code).FirstOrDefault();

            if ( median)
                chart1.ChartAreas[0].AxisY.Title = "Antagningspoäng";
            else
                chart1.ChartAreas[0].AxisY.Title = "Andel utbildningar med konkurrens";


            // Set the text of the title
            if (median)
            {
                if (LB_uni.CheckedItems.Count > 1)
                    chart1.Titles["Title1"].Text = "Antagningspoäng (median " + ugcode + ") till utvalda lärosäten";
                else
                    chart1.Titles["Title1"].Text = focusname + "Antagningspoäng (median " + ugcode + ")";
            }
            else
            {
                if (LB_uni.CheckedItems.Count > 1)
                    chart1.Titles["Title1"].Text = "Andel utbildningar med konkurrens vid utvalda lärosäten";
                else
                    chart1.Titles["Title1"].Text = focusname + "Andel utbildningar med konkurrens";
            }
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande till riket";
                chart1.ChartAreas[0].AxisY.Title = "I förhållande till riket";
            }
            if (CB_refyear.Checked)
            {
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";
                chart1.ChartAreas[0].AxisY.Title += " (" + minyear.ToString() + "=100)";
            }

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_antagningspoang" }, true);

            //if (isector > 0)
            //{
            //    chart1.Titles["Title1"].Text += ", " + (from c in db.OV_mysector where c.Id == isector select c.Name).FirstOrDefault();
            //}
            //if (isubject > 0)
            //{
            //    chart1.Titles["Title1"].Text += ", " + (from c in db.OV_mysubject where c.Id == isubject select c.Name).FirstOrDefault();
            //}


            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);


            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                for (int year = minyear; year <= maxyear; year++)
                {
                    refdict.Add(year, getappscore(year,0,urvalsgrupp,median));
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;

            StringBuilder sb = new StringBuilder();

            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    yearreference = 0.01 * getappscore(minyear,unidict[uniname],urvalsgrupp,median);
                    if (CB_fraction.Checked)
                        yearreference = yearreference / refdict[minyear];
                }


                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = getappscore(year,unidict[uniname],urvalsgrupp,median);
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                    if (CB_values_to_file.Checked)
                        sb.Append(ss.Name+"\t"+year + "\t" + amount+"\n");
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            if (CB_values_to_file.Checked)
            {
                SaveValuesToFile(sb, chart1.Titles["Title1"].Text);
            }

            //            chart1.ChartAreas[0].AxisY.Maximum = ssmax * 1.1;
            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;

            this.Cursor = Cursors.Default;

        }

        IQueryable<OV_applicants> dospecialstring(IQueryable<OV_applicants> qa, string specialstring)
        {
            if (specialstring == "högsking")
            {
                return from c in qa
                       where c.OV_course.Name.Contains("ingenjör")
                       where !c.OV_course.Name.Contains("ivil")
                       where !c.OV_course.Name.Contains("enare del")
                       where !c.OV_course.Name.Contains("astermin")
                       where !c.OV_course.Name.Contains("asår")
                       where !c.OV_course.Name.Contains("asutbildning")
                       select c;

            }
            else if ( specialstring == "lärare")
            {
                return from c in qa
                       where (c.OV_course.Name.Contains("lärar") || c.OV_course.Name.Contains("ompletterande ped"))
                       where !c.OV_course.Name.Contains("högskol")
                       where !c.OV_course.Name.Contains("master")
                       where !c.OV_course.Name.Contains("enare del")
                       where !c.OV_course.Name.Contains("astermin")
                       where !c.OV_course.Name.Contains("asår")
                       select c;
            }
            else if (specialstring == "ssk")
            {
                return from c in qa
                       where c.OV_course.Name.Contains("juksköte")
                       where !c.OV_course.Name.Contains("pecialis")
                       where !c.OV_course.Name.Contains("master")
                       where !c.OV_course.Name.Contains("öntgen")
                       where !c.OV_course.Name.Contains("enare del")
                       where !c.OV_course.Name.Contains("astermin")
                       where !c.OV_course.Name.Contains("asår")
                       select c;
            }
            else if (specialstring == "specssk")
            {
                return from c in qa
                       where c.OV_course.Name.ToLower().Contains("pecialists")
                       where !c.OV_course.Name.Contains("master")
                       where !c.OV_course.Name.Contains("öntgen")
                       where !c.OV_course.Name.Contains("enare del")
                       where !c.OV_course.Name.Contains("astermin")
                       where !c.OV_course.Name.Contains("asår")
                       select c;
            }
            else
                return qa;
        }

        IQueryable<OV_course> dospecialstring(IQueryable<OV_course> q, string specialstring)
        {
            if (specialstring == "högsking")
            {
                return from c in q
                       where c.Name.Contains("ingenjör")
                       where !c.Name.Contains("ivil")
                       where !c.Name.Contains("enare del")
                       where !c.Name.Contains("astermin")
                       where !c.Name.Contains("asår")
                       where !c.Name.Contains("asutbildning")
                       select c;

            }
            else if (specialstring == "lärare")
            {
                return from c in q
                       where (c.Name.Contains("lärar") || c.Name.Contains("ompletterande ped"))
                       where !c.Name.Contains("högskol")
                       where !c.Name.Contains("master")
                       where !c.Name.Contains("enare del")
                       where !c.Name.Contains("astermin")
                       where !c.Name.Contains("asår")
                       select c;
            }
            else if (specialstring == "ssk")
            {
                return from c in q
                       where c.Name.Contains("juksköte")
                       where !c.Name.Contains("pecialis")
                       where !c.Name.Contains("master")
                       where !c.Name.Contains("öntgen")
                       where !c.Name.Contains("enare del")
                       where !c.Name.Contains("astermin")
                       where !c.Name.Contains("asår")
                       select c;
            }
            else if (specialstring == "specssk")
            {
                return from c in q
                       where c.Name.ToLower().Contains("pecialists")
                       where !c.Name.Contains("master")
                       where !c.Name.Contains("öntgen")
                       where !c.Name.Contains("enare del")
                       where !c.Name.Contains("astermin")
                       where !c.Name.Contains("asår")
                       select c;
            }
            else
                return q;
        }


        public int getapplicants(int year, int iuni, int itype, int isector, int isubject, int gender, int age, bool? fkprog,bool? ht, string specialstring)
        {
            //parent.memo("get_applicants " + year);
            int sum = 0;
            if (gender + age > 0)
            {
                IQueryable<OV_applicants> qa;
                if (iuni == 0)
                {
                    if (isector == 0)
                    {
                        if (isubject == 0)
                        {
                            qa = from c in db.OV_applicants
                                where c.Year == year
                                select c;
                            if ( !string.IsNullOrEmpty(specialstring))
                            {
                                qa = dospecialstring(qa, specialstring);
                                //qa = from c in qa
                                //     where c.OV_course.Name.Contains("ingenjör")
                                //     where !c.OV_course.Name.Contains("ivil")
                                //     where !c.OV_course.Name.Contains("enare del")
                                //     where !c.OV_course.Name.Contains("astermin")
                                //     where !c.OV_course.Name.Contains("asår")
                                //     select c;
                            }
                        }
                        else
                        {
                            qa = from c in db.OV_applicants
                                where c.OV_course.Subject == isubject
                                where c.Year == year
                                select c;
                        }
                    }
                    else
                    {
                        qa = from c in db.OV_applicants
                            where c.OV_course.Sector == isector
                            where c.Year == year
                            select c;
                    }
                }
                else
                {
                    if (isector == 0)
                    {
                        if (isubject == 0)
                        {
                            qa = from c in db.OV_applicants
                                where c.OV_course.Uni == iuni
                                where c.Year == year
                                select c;
                            if (!string.IsNullOrEmpty(specialstring))
                            {
                                qa = dospecialstring(qa, specialstring);
                            }
                        }
                        else
                        {
                            qa = from c in db.OV_applicants
                                where c.OV_course.Uni == iuni
                                where c.OV_course.Subject == isubject
                                where c.Year == year
                                select c;
                        }
                    }
                    else
                    {
                        qa = from c in db.OV_applicants
                            where c.OV_course.Uni == iuni
                            where c.OV_course.Sector == isector
                            where c.Year == year
                            select c;
                    }
                }

                if (fkprog != null)
                {
                    if ((bool)fkprog)
                        qa = from c in qa where c.OV_course.Program select c;
                    else
                        qa = from c in qa where !c.OV_course.Program select c;
                }
                if ( ht != null)
                {
                    if ((bool)ht)
                        qa = from c in qa where c.OV_course.HT select c;
                    else
                        qa = from c in qa where !c.OV_course.HT select c;
                }
                //parent.memo("qa.Count = " + qa.Count());

                if (gender * age > 0)
                {
                    qa = from c in qa
                         where c.Gender == gender
                         where c.Age == age
                         select c;
                }
                else if (gender != 0)
                {
                    qa = from c in qa
                         where c.Gender == gender
                         select c;

                }
                else if (age != 0)
                {
                    qa = from c in qa
                         where c.Age == age
                         select c;
                }
                if (qa.Count() > 0)
                    switch (itype)
                    {
                        case 1:
                            sum = (from c in qa select c.Appl1h).Sum();
                            break;
                        case 2:
                            sum = (from c in qa select c.Appltotal).Sum();
                            break;
                        case 3:
                            sum = (from c in qa select c.Accepted).Sum();
                            break;
                        case 4:
                            sum = (from c in qa select c.Reserves).Sum();
                            break;
                        default:
                            sum = (from c in qa select c.Appl1h).Sum();
                            break;
                    }

                //List<int> courselist = (from c in q select c.Id).ToList();
                //foreach (OV_course cc in q)
                //{
                //    var qa = from c in db.OV_applicants
                //             where c.Course == cc.Id
                //             select c;

                //    //parent.memo("qq.Count = " + qq.Count());
                //    //qa = qa.Concat(qq);
                //    //parent.memo("qa.Count = " + qa.Count());

                //}
            }
            else
            {
                IQueryable<OV_course> q;
                if (iuni == 0)
                {
                    if (isector == 0)
                    {
                        if (isubject == 0)
                        {
                            q = from c in db.OV_course
                                where c.Year == year
                                select c;
                            if (!string.IsNullOrEmpty(specialstring))
                            {
                                q = dospecialstring(q, specialstring);
                                //q = from c in q
                                //    where c.Name.Contains("ingenjör")
                                //    where !c.Name.Contains("ivil")
                                //    where !c.Name.Contains("enare del")
                                //     where !c.Name.Contains("astermin")
                                //     where !c.Name.Contains("asår")
                                //     select c;
                            }
                        }
                        else
                        {
                            q = from c in db.OV_course
                                where c.Subject == isubject
                                where c.Year == year
                                select c;
                        }
                    }
                    else
                    {
                        q = from c in db.OV_course
                            where c.Sector == isector
                            where c.Year == year
                            select c;
                    }
                }
                else
                {
                    if (isector == 0)
                    {
                        if (isubject == 0)
                        {
                            q = from c in db.OV_course
                                where c.Uni == iuni
                                where c.Year == year
                                select c;
                            if (!string.IsNullOrEmpty(specialstring))
                            {
                                q = dospecialstring(q, specialstring);
                            }
                        }
                        else
                        {
                            q = from c in db.OV_course
                                where c.Uni == iuni
                                where c.Subject == isubject
                                where c.Year == year
                                select c;
                        }
                    }
                    else
                    {
                        q = from c in db.OV_course
                            where c.Uni == iuni
                            where c.Sector == isector
                            where c.Year == year
                            select c;
                    }
                }

                if (fkprog != null)
                {
                    if ((bool)fkprog)
                        q = from c in q where c.Program select c;
                    else
                        q = from c in q where !c.Program select c;
                }
                if (ht != null)
                {
                    if ((bool)ht)
                        q = from c in q where c.HT select c;
                    else
                        q = from c in q where !c.HT select c;
                }

                //parent.memo("q.Count = " + q.Count());

                if (q.Count() > 0)
                {
                    //if (year == 2020)
                    //{
                    //    foreach (OV_course c in q)
                    //    {
                    //        parent.memo(c.OV_University.Name + ": " + c.Name);
                    //    }
                    //}
                    switch (itype)
                    {
                        case 1:
                            sum = (from c in q select c.Appl1h).Sum();
                            break;
                        case 2:
                            sum = (from c in q select c.Appltotal).Sum();
                            break;
                        case 3:
                            sum = (from c in q select c.Accepted).Sum();
                            break;
                        case 4:
                            sum = (from c in q select c.Reserves).Sum();
                            break;
                        default:
                            sum = (from c in q select c.Appl1h).Sum();
                            break;
                    }

                }
            }
            return sum;
        }

        public void totalapplicants(int itype, int isector, int isubject, int gender, int age)
        {
            List<int> sectorlist = new List<int>();
            if (isector > 0)
                sectorlist.Add(isector);
            List<int> subjectlist = new List<int>();
            if (isubject > 0)
                subjectlist.Add(isubject);
            totalapplicants(itype, sectorlist, subjectlist, gender, age,"");
        }

        public void totalapplicants(int itype, List<int> sectorlist, List<int> subjectlist, int gender, int age, string specialstring)
        {
            this.Cursor = Cursors.WaitCursor;

            //parent.memo(itype + ", " + isector + ", " + isubject);
            int minyear = (from c in db.OV_course select c.Year).Min() + 1; //skip incomplete 2008
            int maxyear = (from c in db.OV_course select c.Year).Max();
            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            Dictionary<string, int> incomedict = new Dictionary<string, int>();

            bool? fkprog = null;
            if (RB_prog.Checked)
                fkprog = true;
            else if (RB_fk.Checked)
                fkprog = false;

            bool? ht = null;
            if (RB_HT.Checked)
                ht = true;
            else if (RB_VT.Checked)
                ht = false;

            bool genderfraction = RB_examgender.Checked;
            bool agefraction = RB_studentage.Checked;

            chart1.Series.Clear();
            double ssmax = 0;

            chart1.ChartAreas[0].AxisY.Title = "Antal studenter";


            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Utvalda lärosäten ";
            else
                chart1.Titles["Title1"].Text = focusname + " ";
            if (genderfraction)
                chart1.Titles["Title1"].Text += "andel kvinnor ";
            else if (agefraction)
                chart1.Titles["Title1"].Text += "andel 35+ ";

            switch (itype)
            {
                case 1: 
                    chart1.Titles["Title1"].Text += "förstahandssökande";
                    chart1.ChartAreas[0].AxisY.Title = "1:a-handssökande";
                    break;
                case 2:
                    chart1.Titles["Title1"].Text += "sökande totalt";
                    chart1.ChartAreas[0].AxisY.Title = "totalt antal sökande";
                    break;
                case 3:
                    chart1.Titles["Title1"].Text += "antagna";
                    chart1.ChartAreas[0].AxisY.Title = "antagna";
                    break;
                case 4:
                    chart1.Titles["Title1"].Text += "reserver";
                    chart1.ChartAreas[0].AxisY.Title = "reserver";
                    break;
            }
            if (CBperantagen.Checked)
            {
                chart1.Titles["Title1"].Text += " per antagen";
                chart1.ChartAreas[0].AxisY.Title += " per antagen";

            }
            if (genderfraction)
                chart1.ChartAreas[0].AxisY.Title += " andel kvinnor";
            else if (agefraction)
                chart1.ChartAreas[0].AxisY.Title += " andel 35+";

            double zeroreplacement = 1;
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", som andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
                zeroreplacement = 0.001;
            }
            if (CB_refyear.Checked)
            {
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";
                chart1.ChartAreas[0].AxisY.Title += " (" + minyear.ToString() + "=100)";
            }
            if ( fkprog != null)
            {
                if ( (bool)fkprog)
                    chart1.Titles["Title1"].Text += ", program";
                else
                    chart1.Titles["Title1"].Text += ", fristående kurs";
            }

            if ( sectorlist.Count == 1)
            {
                chart1.Titles["Title1"].Text += ", "+(from c in db.OV_mysector where c.Id == sectorlist.First() select c.Name).FirstOrDefault();
            }
            if (subjectlist.Count == 1)
            {
                chart1.Titles["Title1"].Text += ", " + (from c in db.OV_mysubject where c.Id == subjectlist.First() select c.Name).FirstOrDefault();
            }
            if (!String.IsNullOrEmpty(specialstring))
            {
                chart1.Titles["Title1"].Text += ", "+specialstring;
            }


            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_applicants" }, true);

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            List<Tuple<int, int>> sslist = new List<Tuple<int, int>>();
            if ((sectorlist.Count == 0) && (subjectlist.Count == 0))
            {
                Tuple<int, int> tt = new Tuple<int, int>(0, 0);
                sslist.Add(tt);
            }
            else
            {
                foreach (int isector in sectorlist)
                {
                    Tuple<int, int> tt = new Tuple<int, int>(isector, 0);
                    sslist.Add(tt);
                }
                foreach (int isubject in subjectlist)
                {
                    Tuple<int, int> tt = new Tuple<int, int>(0, isubject);
                    sslist.Add(tt);
                }
            }

            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();

            foreach (Tuple<int, int> tt in sslist)
            {
                ////Specialare för HDa, ta FK från språk och program för allt annat
                //if (tt.Item1 == 4)
                //    fkprog = false;
                //else
                //    fkprog = true;

                Dictionary<int, double> refdict = new Dictionary<int, double>();
                if (CB_fraction.Checked)
                {
                    for (int year = minyear; year <= maxyear; year++)
                    {
                        refdict.Add(year, getapplicants(year, 0, itype, tt.Item1, tt.Item2, gender, age, fkprog,ht, specialstring));
                    }

                    //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
                }

                

                //if ( CB_refyear.Checked)
                //    chart1.ChartAreas[0].AxisY.Maximum = 150;




                foreach (string uniname in unilist)
                {
                    string ssname = uniname;
                    if (unilist.Count == 1 && ((sectorlist.Count > 0) || (subjectlist.Count > 0)))
                        ssname = "";
                    if (tt.Item1 > 0)
                        ssname += " "+(from c in db.OV_mysector where c.Id == tt.Item1 select c.Name).First();
                    if (tt.Item2 > 0)
                        ssname += " " + (from c in db.OV_mysubject where c.Id == tt.Item2 select c.Name).First();
                    ssname = ssname.Trim();
                    Series ss = new Series(ssname);
                    if (uniname == focusname && !CB_sumuni.Checked)
                        ss.BorderWidth = focusthickness;
                    else
                        ss.BorderWidth = linethickness;


                    double yearreference = 1;
                    if (CB_refyear.Checked)
                    {
                        yearreference = 0.01 * getapplicants(minyear, unidict[uniname], itype, tt.Item1, tt.Item2, gender, age, fkprog,ht,specialstring);
                        if (CB_fraction.Checked)
                            yearreference = yearreference / refdict[minyear];
                    }


                    //parent.memo("q.Count = " + q.Count());
                    for (int year = minyear; year <= maxyear; year++)
                    {
                        double amount = 0;
                        if (genderfraction)
                        {
                            double amen = getapplicants(year, unidict[uniname], itype, tt.Item1, tt.Item2, 2, age, fkprog, ht, specialstring);
                            double awomen = getapplicants(year, unidict[uniname], itype, tt.Item1, tt.Item2, 1, age, fkprog, ht, specialstring);
                            if (amen + awomen > 0)
                                amount = awomen / (amen + awomen);

                        }
                        else if (agefraction)
                        {
                            double aold = getapplicants(year, unidict[uniname], itype, tt.Item1, tt.Item2, gender, 3, fkprog, ht, specialstring);
                            double a1 = getapplicants(year, unidict[uniname], itype, tt.Item1, tt.Item2, gender, 1, fkprog, ht, specialstring);
                            double a2 = getapplicants(year, unidict[uniname], itype, tt.Item1, tt.Item2, gender, 2, fkprog, ht, specialstring);
                            if (a1 + a2 + aold > 0)
                                amount = aold / (a1 + a2 + aold);
                            parent.memo("aold,aall = " + aold + ", " + (a1 + a2 + aold));
                        }
                        else
                        {
                            amount = getapplicants(year, unidict[uniname], itype, tt.Item1, tt.Item2, gender, age, fkprog, ht, specialstring);
                            if (CBperantagen.Checked)
                            {
                                double antagna = getapplicants(year, unidict[uniname], 3, tt.Item1, tt.Item2, gender, age, fkprog, ht, specialstring);
                                if (antagna > 0)
                                    amount = amount / antagna;
                                else
                                    continue;
                            }
                        }
                        amount = amount / yearreference;
                        if (CB_fraction.Checked)
                            amount = amount / refdict[year];
                        if (CB_logarithm.Checked && amount == 0)
                            amount = zeroreplacement;
                        ss.Points.AddXY(year, amount);
                        if (CB_sumuni.Checked)
                        {
                            if (!sumdict.ContainsKey(year))
                                sumdict.Add(year, amount);
                            else
                                sumdict[year] += amount;
                        }

                        if (amount > ssmax)
                            ssmax = amount;
                        if (CB_memo.Checked)
                            parent.memo(year + "\t" + amount);
                    }
                    ss.ChartType = SeriesChartType.Line;
                    chart1.Series.Add(ss);
                }
            }


            chart1.ChartAreas[0].AxisY.IsLogarithmic = CB_logarithm.Checked;
            if ( CB_logarithm.Checked)
            {
                chart1.ChartAreas[0].AxisY.Minimum = zeroreplacement;
            }
            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;

            this.Cursor = Cursors.Default;

        }

        public List<int> getuni_in_lan(int ilan)
        {
            List<int> unilist = new List<int>();
            var q = from c in db.OV_University_Kommun where c.OV_Kommun.Lan == ilan select c.Uni;
            foreach (int iuni in q)
                if (!unilist.Contains(iuni))
                    unilist.Add(iuni);
            return unilist;
        }

        public List<int> getlan_uni(int iuni)
        {
            List<int> lanlist = new List<int>();
            var q = from c in db.OV_University_Kommun where c.Uni == iuni select c.OV_Kommun.Lan;
            foreach (int ilan in q)
                if (!lanlist.Contains(ilan))
                    lanlist.Add(ilan);
            return lanlist;
        }

        public double get_homesettling_fraction(int iunipar, int year)
        {
            //parent.memo("get_homesettling");
            double amount = 0;
            double total = 0;
            double home = 0;

            List<int> unilist = new List<int>();

            if (iunipar == 0)
            {
                unilist = (from c in db.OV_University where c.Mergedwith == null select c.Id).ToList();
            }
            else
                unilist.Add(iunipar);

            //parent.memo("ghf uni loop:");

            foreach (int iuni in unilist)
            {
                //parent.memo("iuni = " + iuni);
                List<int> lanlist = getlan_uni(iuni);
                var q = from c in db.OV_studentflow
                        where c.Year == year
                        where c.Uni == iuni
                        select c;

                foreach (OV_studentflow osf in q)
                {
                    total += osf.Number;
                    if (lanlist.Contains(osf.Tolan))
                        home += osf.Number;
                }
            }

            if (total > 0)
                amount = home / total;

            return amount;
        }

        public double get_homerecruiting_fraction(int iunipar, int year)
        {
            double amount = 0;
            double total = 0;
            double home = 0;

            List<int> unilist = new List<int>();

            if (iunipar == 0)
            {
                unilist = (from c in db.OV_University where c.Mergedwith == null select c.Id).ToList();
            }
            else
                unilist.Add(iunipar);

            foreach (int iuni in unilist)
            {
                List<int> lanlist = getlan_uni(iuni);
                var q = from c in db.OV_studentflow
                        where c.Year == year
                        where c.Uni == iuni
                        select c;

                foreach (OV_studentflow osf in q)
                {
                    total += osf.Number;
                    if (lanlist.Contains(osf.Fromlan))
                        home += osf.Number;
                }
            }

            if (total > 0)
                amount = home / total;

            parent.memo("homerecruit uni,year,amount " + iunipar + ", " + year + ", " + amount);
            return amount;
        }

        public void settling_by_uni()
        {
            parent.memo("settling by uni");

            int minyear = (from c in db.OV_studentflow where c.Year > 0 select c.Year).Min();
            int maxyear = (from c in db.OV_studentflow select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_studentflow" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            double ssmax = 0;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Andel alumner som jobbar i lärosätets län";
            else
                chart1.Titles["Title1"].Text = focusname + " andel alumner som jobbar i lärosätets län";
            if (CB_fraction.Checked)
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);


            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                for (int year = minyear; year <= maxyear; year++)
                {
                    refdict.Add(year, get_homesettling_fraction(0,year));
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            parent.memo("uni loop:");

            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                List<int> lanlist = getlan_uni(unidict[uniname]);

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    yearreference = 0.01*get_homesettling_fraction(unidict[uniname], minyear);
                    if (CB_fraction.Checked)
                        yearreference = yearreference / refdict[minyear];
                }


                parent.memo("year loop");
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    //parent.memo("Year = " + year);
                    double amount = get_homesettling_fraction(unidict[uniname],year);
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }

                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            parent.memo("end of settling by uni");
        }

        public Series getsumseries(List<string> unilist)
        {
            if (CB_sumuni.Checked)
            {
                string sumname = "";
                string separator = "+";
                if (CB_meanuni.Checked)
                    separator = "/";
                foreach (string uniname in unilist)
                {
                    sumname += unishortdict[uniname] + separator;
                }
                if (sumname == "du+hig+mdh+slu+uu+oru+".Replace("+", separator))
                {
                    if (CB_meanuni.Checked)
                        sumname = "medel Östsvenska";
                    else
                        sumname = "totalt Östsvenska";
                }
                Series sumseries = new Series(sumname);
                sumseries.BorderWidth = 5;
                sumseries.ChartType = SeriesChartType.Line;
                return sumseries;
            }
            else
                return null;

        }

        public void settling_by_lan()
        {
            int minyear = (from c in db.OV_studentflow where c.Year > 0 select c.Year).Min();
            int maxyear = (from c in db.OV_studentflow select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_studentflow" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            chart1.Series.Clear();
            double ssmax = 0;

            // Set the text of the title
            chart1.Titles["Title1"].Text = "Andel studenter som jobbar i länet som har tagit examen vid lärosäte i länet";
            if (CB_fraction.Checked)
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            foreach (OV_Lan ol in (from c in db.OV_Lan select c))
            {
                Series ss = new Series(ol.Name);
                if (ol.Name == "Dalarnas län")
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                for (int year = minyear; year <= maxyear; year++)
                {
                    double total = 0;
                    double home = 0;

                    List<int> unilist = getuni_in_lan(ol.Id);
                    foreach (OV_studentflow osf in (from c in db.OV_studentflow where c.Tolan == ol.Id where c.Year == year select c))
                    {
                        total += osf.Number;
                        if (unilist.Contains(osf.Uni))
                            home += osf.Number;
                    }

                    double amount = 0;
                    if (total > 0)
                        amount = home / total;
                    ss.Points.AddXY(year, amount);
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);

                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;

        }

        public void recruit_by_uni()
        {
            parent.memo("recruit by uni");
            int minyear = (from c in db.OV_studentflow where c.Year > 0 select c.Year).Min();
            int maxyear = (from c in db.OV_studentflow select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_studentflow" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            double ssmax = 0;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Andel alumner som kommer från lärosätets län";
            else
                chart1.Titles["Title1"].Text = focusname + " andel alumner som kommer från lärosätets län";
            if (CB_fraction.Checked)
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            chart1.ChartAreas[0].AxisY.Title = "Andel alumner från hemmalänet";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);


            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                for (int year = minyear; year <= maxyear; year++)
                {
                    refdict.Add(year, get_homerecruiting_fraction(0, year));
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();

            SortedDictionary<double, Series> seriesdict = new SortedDictionary<double, Series>();
            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                List<int> lanlist = getlan_uni(unidict[uniname]);

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    yearreference = 0.01 * get_homerecruiting_fraction(unidict[uniname], minyear);
                    if (CB_fraction.Checked)
                        yearreference = yearreference / refdict[minyear];
                }


                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = get_homerecruiting_fraction(unidict[uniname], year);
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(uniname+"\t"+year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                //ss.Label = ss.Name;
                if (CB_serieslabel.Checked)
                    ss.Points.Last().Label = ss.Name;
                //chart1.Series.Add(ss);
                double key = -ss.Points.Last().YValues[0];
                while (seriesdict.ContainsKey(key))
                    key += 0.001;
                seriesdict.Add(key, ss);
            }
            foreach (double dd in seriesdict.Keys)
                chart1.Series.Add(seriesdict[dd]);

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }

            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;


        }

        public void recruit_by_lan()
        {
            int minyear = (from c in db.OV_studentflow where c.Year > 0 select c.Year).Min();
            int maxyear = (from c in db.OV_studentflow select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_studentflow" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            chart1.Series.Clear();
            double ssmax = 0;

            // Set the text of the title
            chart1.Titles["Title1"].Text = "Andel studenter från länet som tagit examen vid lärosäte i länet";
            if (CB_fraction.Checked)
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";
            chart1.ChartAreas[0].AxisY.Title = "Andel alumner som tagit examen in hemmalänet";

            SortedDictionary<double, Series> seriesdict = new SortedDictionary<double, Series>();

            foreach (OV_Lan ol in (from c in db.OV_Lan select c))
            {
                if (ol.Name.Contains("Riket"))
                    continue;
                Series ss = new Series(ol.Name);
                //if (ol.Name == "Dalarnas län")
                //    ss.BorderWidth = focusthickness;
                //else
                    ss.BorderWidth = linethickness;

                if (CB_memo.Checked)
                    parent.memo(ol.Name);

                for (int year = minyear; year <= maxyear; year++)
                {
                    double total = 0;
                    double home = 0;

                    List<int> unilist = getuni_in_lan(ol.Id);
                    foreach (OV_studentflow osf in (from c in db.OV_studentflow where c.Fromlan == ol.Id where c.Year == year select c))
                    {
                        total += osf.Number;
                        if (unilist.Contains(osf.Uni))
                            home += osf.Number;
                    }
                    double amount = 0;
                    if (total > 0)
                        amount = home / total;
                    ss.Points.AddXY(year, amount);
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);

                }
                ss.ChartType = SeriesChartType.Line;
                 //chart1.Series.Add(ss);
                seriesdict.Add(-ss.Points.Last().YValues[0], ss);
            }
            foreach (double dd in seriesdict.Keys)
                chart1.Series.Add(seriesdict[dd]);

            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;

        }

        public double diversityratio(int uni, int year, bool gender, bool foreign, bool eduparent)
        {
            double x = 0;

            var q = from c in db.OV_studentbackground where c.Year == year select c;
            if (uni != 0)
                q = from c in q where c.Uni == uni select c;

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_studentbackground" }, true);

            IQueryable<OV_demography> qdemo = null;
            if ( CB_demography.Checked)
            {
                qdemo = from c in db.OV_demography
                        where c.Year == year
                        select c;
                if (uni != 0)
                {
                    List<int> lanlist = getlan_uni(uni);
                    qdemo = from c in qdemo where lanlist.Contains(c.Lan) select c;
                }
            }

            if (gender)
            {
                var q1 = from c in q where c.Gender == 2 select c.Number;
                var q2 = from c in q where c.Gender == 1 select c.Number;
                double x1 = 0;
                double x2 = 1;
                if (q1.Count() > 0)
                    x1 = q1.Sum();
                if (q2.Count() > 0)
                    x2 = q2.Sum();
                x = x1 / (x1 + x2);

                if (CB_demography.Checked)
                {
                    var qd1 = from c in qdemo where c.Gender == 2 select c.Number;
                    var qd2 = from c in qdemo where c.Gender == 1 select c.Number;

                    double xd1 = 0;
                    double xd2 = 1;
                    if (qd1.Count() > 0)
                        xd1 = qd1.Sum();
                    if (qd2.Count() > 0)
                        xd2 = qd2.Sum();
                    double xd = xd1 / (xd1 + xd2);
                    if ( xd > 0)
                        x = x / xd;
                }
            }

            else if (foreign)
            {
                var q1 = from c in q where c.Foreignbackground <= 2 select c.Number;
                var q2 = from c in q where c.Foreignbackground > 2 select c.Number;
                double x1 = 0;
                double x2 = 1;
                if (q1.Count() > 0)
                    x1 = q1.Sum();
                if (q2.Count() > 0)
                    x2 = q2.Sum();
                x = x1 / (x1 + x2);

                if (CB_demography.Checked)
                {
                    var qd1 = from c in qdemo where c.Foreignbackground == 6 select c.Number;
                    var qd2 = from c in qdemo where c.Foreignbackground == 7 select c.Number;

                    double xd1 = 0;
                    double xd2 = 1;
                    if (qd1.Count() > 0)
                        xd1 = qd1.Sum();
                    if (qd2.Count() > 0)
                        xd2 = qd2.Sum();
                    double xd = xd1 / (xd1 + xd2);
                    if (xd > 0)
                        x = x / xd;
                }
            }
            else if (eduparent)
            {
                var q1 = from c in q where !(bool)c.Educatedparent select c.Number;
                var q2 = from c in q where (bool)c.Educatedparent select c.Number;
                double x1 = 0;
                double x2 = 1;
                if (q1.Count() > 0)
                    x1 = q1.Sum();
                if (q2.Count() > 0)
                    x2 = q2.Sum();
                x = x1 / (x1 + x2);

                if (CB_demography.Checked)
                {
                    var qd1 = from c in qdemo where !(bool)c.Educated select c.Number;
                    var qd2 = from c in qdemo where (bool)c.Educated select c.Number;

                    double xd1 = 0;
                    double xd2 = 1;
                    if (qd1.Count() > 0)
                        xd1 = qd1.Sum();
                    if (qd2.Count() > 0)
                        xd2 = qd2.Sum();
                    double xd = xd1 / (xd1 + xd2);
                    if (xd > 0)
                        x = x / xd;
                }
            }


            return x;
        }

        public void studentage()
        {
            parent.memo("studentage dummy");
        }

        public void totaldiversity()
        {
            if ( RB_studentage.Checked)
            {
                studentage();
                return;
            }

            int minyear = (from c in db.OV_studentbackground select c.Year).Min();
            int maxyear = (from c in db.OV_studentbackground select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_studentbackground" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Mångfald";

            double ssmax = 0;
            double ssmin = 0;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Mångfald utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " mångfald";
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
                chart1.ChartAreas[0].AxisY.Title += " i förhållande till rikssnittet";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            if (RB_examgender.Checked)
            {
                chart1.Titles["Title1"].Text += " könsfördelning";
                chart1.ChartAreas[0].AxisY.Title = "andel män";
            }
            else if (RB_examforeign.Checked)
            {
                chart1.Titles["Title1"].Text += " svensk/utländsk bakgrund";
                chart1.ChartAreas[0].AxisY.Title = "andel utländsk bakgrund";
            }
            else if (RB_exameduparent.Checked)
            {
                chart1.Titles["Title1"].Text += " hög/lågutbildade föräldrar";
                chart1.ChartAreas[0].AxisY.Title = "andel lågutbildade";
            }

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                for (int year = minyear; year <= maxyear; year++)
                {
                    refdict.Add(year, diversityratio(0, year, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked));
                }
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    if (CB_fraction.Checked)
                        yearreference = 0.01 * (diversityratio(unidict[uniname], minyear, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked) / refdict[minyear]);
                    else
                        yearreference = 0.01 * diversityratio(unidict[uniname], minyear, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked);
                }


                //var q = (from c in db.OV_studentbackground where c.Uni == unidict[uniname] where c.Area == iarea orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = diversityratio(unidict[uniname], year, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked);
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (amount < ssmin)
                        ssmin = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }

            double ssdef = 0.2;
            ssmax = Math.Max(ssmax, Math.Abs(ssmin));
            if (ssmax < ssdef)
                ssmax = ssdef;
            if (ssmin < 0)
                ssmin = -Math.Max(ssmax, Math.Abs(ssmin));

            if (ssmin < 0)
            {
                double diff = ssmax - ssmin;
                double newdiff = roundaxis(diff);
                if (ssmax > Math.Abs(ssmin))
                {
                    ssmax = 0.6 * newdiff;
                    ssmin = -0.4 * newdiff;
                }
                else
                {
                    ssmax = 0.4 * newdiff;
                    ssmin = -0.6 * newdiff;
                }
            }
            else
                ssmax = roundaxis(ssmax);

            chart1.ChartAreas[0].AxisY.Maximum = ssmax;
            chart1.ChartAreas[0].AxisY.Minimum = ssmin;




        }

        public double roundaxis(double len)
        {
            double x = 0.24*len; //0.2 because chart type divides axis in 5 sections
            int decimals = 0;

            if (x < 0.3)
                decimals = 2;
            else if (x < 3)
                decimals = 1;
            else if (x > 100)
            {
                int ndig = (int)Math.Truncate(Math.Log10(x));
                double p10ndig = Math.Pow(10, ndig);
                return p10ndig * roundaxis(len / p10ndig);
            }
            double xr = Math.Round(x, decimals);
            parent.memo("x,xr,dec = " + x + " " + xr + " " + decimals);
            return 5*xr;
        }

        public bool isforeign(OV_studentbackground os)
        {
            if (os.OV_foreigntype.Name.Contains("svensk"))
                return false;
            if (os.OV_foreigntype.Name.Contains("två utr"))
                return true;
            if (os.OV_foreigntype.Name.Contains("nrikes"))
                return false;
            return true;
        }

        public bool hascredit(OV_studentbackground os,int mincredit)
        {
            if (os.OV_creditgroup.Lower >= mincredit)
                return true;
            else
                return false;
        }
        public double creditfreqratio(int uni, int year, bool total, bool gender, bool foreign, bool eduparent, int mincredit)
        {
            double x = 0;

            var q = (from c in db.OV_studentbackground where c.Creditgroup != null where c.Year == year select c).ToList();
            parent.memo("q.Count " + q.Count());
            if (uni != 0)
                q = (from c in q where c.Uni == uni select c).ToList();
            //parent.memo("q-uni.Count " + q.Count());

            if (total)
            {
                var q1 = from c in q where hascredit(c,mincredit) select c.Number;
                var q2 = from c in q where !hascredit(c,mincredit) select c.Number;
                double x1 = 0;
                double x2 = 1;
                if (q1.Count() > 0)
                    x1 = q1.Sum();
                if (q2.Count() > 0)
                    x2 = q2.Sum();
                x = x1 / (x1 + x2);
            }
            else if (gender)
            {
                var q11 = from c in q where hascredit(c, mincredit) where c.Gender == 1 select c.Number;
                var q21 = from c in q where !hascredit(c, mincredit) where c.Gender == 1 select c.Number;
                var q12 = from c in q where hascredit(c, mincredit) where c.Gender == 2 select c.Number;
                var q22 = from c in q where !hascredit(c, mincredit) where c.Gender == 2 select c.Number;
                double x11 = 0;
                double x21 = 1;
                double x12 = 0;
                double x22 = 1;
                if (q11.Count() > 0)
                    x11 = q11.Sum();
                if (q21.Count() > 0)
                    x21 = q21.Sum();
                if (q12.Count() > 0)
                    x12 = q12.Sum();
                if (q22.Count() > 0)
                    x22 = q22.Sum();
                x = x11 / (x11 + x21) - x12 / (x12 + x22);
            }
            else if (foreign)
            {
                double x11 = 0;
                double x21 = 1;
                double x12 = 0;
                double x22 = 1;
                var q11 = from c in q where hascredit(c, mincredit) where isforeign(c) select c.Number;
                var q21 = from c in q where !hascredit(c, mincredit) where isforeign(c) select c.Number;
                var q12 = from c in q where hascredit(c, mincredit) where !isforeign(c) select c.Number;
                var q22 = from c in q where !hascredit(c, mincredit) where !isforeign(c) select c.Number;
                if (q11.Count() > 0)
                    x11 = q11.Sum();
                if (q21.Count() > 0)
                    x21 = q21.Sum();
                if (q12.Count() > 0)
                    x12 = q12.Sum();
                if (q22.Count() > 0)
                    x22 = q22.Sum();
                x = x11 / (x11 + x21) - x12 / (x12 + x22);
            }
            else if (eduparent)
            {
                var q11 = from c in q where hascredit(c, mincredit) where !(bool)c.Educatedparent select c.Number;
                var q21 = from c in q where !hascredit(c, mincredit) where !(bool)c.Educatedparent select c.Number;
                var q12 = from c in q where hascredit(c, mincredit) where (bool)c.Educatedparent select c.Number;
                var q22 = from c in q where !hascredit(c, mincredit) where (bool)c.Educatedparent select c.Number;
                double x11 = 0;
                double x21 = 1;
                double x12 = 0;
                double x22 = 1;
                if (q11.Count() > 0)
                    x11 = q11.Sum();
                if (q21.Count() > 0)
                    x21 = q21.Sum();
                if (q12.Count() > 0)
                    x12 = q12.Sum();
                if (q22.Count() > 0)
                    x22 = q22.Sum();
                x = x11 / (x11 + x21) - x12 / (x12 + x22);
            }

            return x;
        }


        public double examfreqratio_OLD(int uni, int year, bool total, bool gender, bool foreign, bool eduparent, bool? progfk)
        {
            double x = 0;

            var q = from c in db.OV_studentbackground where c.Exam != null where c.Year == year select c;
            if (progfk != null)
                q = from c in q where c.Progfk == progfk select c;
            else
                q = from c in q where c.Progfk == null select c;
            parent.memo("q.Count " + q.Count());
            if (uni != 0)
                q = from c in q where c.Uni == uni select c;
            parent.memo("q-uni.Count " + q.Count());

            if ( total )
            {
                var q1 = from c in q where (bool)c.Exam select c.Number;
                var q2 = from c in q where !(bool)c.Exam select c.Number;
                double x1 = 0;
                double x2 = 1;
                if (q1.Count() > 0)
                    x1 = q1.Sum();
                if (q2.Count() > 0)
                    x2 = q2.Sum();
                x = x1 / (x1 + x2);
            }
            else if ( gender)
            {
                var q11 = from c in q where (bool)c.Exam where c.Gender == 1 select c.Number;
                var q21 = from c in q where !(bool)c.Exam where c.Gender == 1 select c.Number;
                var q12 = from c in q where (bool)c.Exam where c.Gender == 2 select c.Number;
                var q22 = from c in q where !(bool)c.Exam where c.Gender == 2 select c.Number;
                double x11 = 0;
                double x21 = 1;
                double x12 = 0;
                double x22 = 1;
                if (q11.Count() > 0)
                    x11 = q11.Sum();
                if (q21.Count() > 0)
                    x21 = q21.Sum();
                if (q12.Count() > 0)
                    x12 = q12.Sum();
                if (q22.Count() > 0)
                    x22 = q22.Sum();
                x = x11 / (x11 + x21) - x12 / (x12 + x22);
            }
            else if (foreign)
            {
                double x11 = 0;
                double x21 = 1;
                double x12 = 0;
                double x22 = 1;
                if (progfk == null) //olika kodat utländsk bakgrund
                {
                    var q11 = from c in q where (bool)c.Exam where c.Foreignbackground <= 2 select c.Number;
                    var q21 = from c in q where !(bool)c.Exam where c.Foreignbackground <= 2 select c.Number;
                    var q12 = from c in q where (bool)c.Exam where c.Foreignbackground > 2 select c.Number;
                    var q22 = from c in q where !(bool)c.Exam where c.Foreignbackground > 2 select c.Number;
                    if (q11.Count() > 0)
                        x11 = q11.Sum();
                    if (q21.Count() > 0)
                        x21 = q21.Sum();
                    if (q12.Count() > 0)
                        x12 = q12.Sum();
                    if (q22.Count() > 0)
                        x22 = q22.Sum();
                }
                else
                {
                    var q11 = from c in q where (bool)c.Exam where c.Foreignbackground ==6 select c.Number;
                    var q21 = from c in q where !(bool)c.Exam where c.Foreignbackground ==6 select c.Number;
                    var q12 = from c in q where (bool)c.Exam where c.Foreignbackground ==7 select c.Number;
                    var q22 = from c in q where !(bool)c.Exam where c.Foreignbackground ==7 select c.Number;
                    if (q11.Count() > 0)
                        x11 = q11.Sum();
                    if (q21.Count() > 0)
                        x21 = q21.Sum();
                    if (q12.Count() > 0)
                        x12 = q12.Sum();
                    if (q22.Count() > 0)
                        x22 = q22.Sum();
                }
                x = x11 / (x11 + x21) - x12 / (x12 + x22);
            }
            else if (eduparent)
            {
                var q11 = from c in q where (bool)c.Exam where !(bool)c.Educatedparent select c.Number;
                var q21 = from c in q where !(bool)c.Exam where !(bool)c.Educatedparent select c.Number;
                var q12 = from c in q where (bool)c.Exam where (bool)c.Educatedparent select c.Number;
                var q22 = from c in q where !(bool)c.Exam where (bool)c.Educatedparent select c.Number;
                double x11 = 0;
                double x21 = 1;
                double x12 = 0;
                double x22 = 1;
                if (q11.Count() > 0)
                    x11 = q11.Sum();
                if (q21.Count() > 0)
                    x21 = q21.Sum();
                if (q12.Count() > 0)
                    x12 = q12.Sum();
                if (q22.Count() > 0)
                    x22 = q22.Sum();
                x = x11 / (x11 + x21) - x12 / (x12 + x22);
            }

            return x;
        }

        public double[] examfreqratio(int uni, int yeardiff, int examyearmax, bool total, bool gender, bool foreign, bool eduparent, bool? progfk,bool cumulative, bool asdiff)
        {
            double[] x = {0,0};

            int yearmax = examyearmax - yeardiff;

            var q = from c in db.OV_studentcohort where c.Examyear != null where c.Examyear-c.Year <= yeardiff where c.Year <= yearmax select c;
            var qtot = from c in db.OV_studentcohort where c.Examyear == null where c.Creditgroup == null where c.Year <= yearmax select c;
            if (!cumulative)
                q = from c in q where c.Examyear - c.Year == yeardiff select c;
            //if (progfk != null)
            //    q = from c in q where c.Progfk == progfk select c;
            //else
            //    q = from c in q where c.Progfk == null select c;
            parent.memo("q.Count " + q.Count());
            if (uni != 0)
            {
                q = from c in q where c.Uni == uni select c;
                qtot = from c in qtot where c.Uni == uni select c;
            }
            parent.memo("q-uni.Count " + q.Count());

            if (total)
            {
                var q1 = from c in q select c.Number;
                var q2 = from c in qtot select c.Number;
                double x1 = 0;
                double x2 = 1;
                if (q1.Count() > 0)
                    x1 = q1.Sum();
                if (q2.Count() > 0)
                    x2 = q2.Sum();
                x[0] = x1 / x2;
            }
            else if (gender)
            {
                var q11 = from c in q    where c.Gender == 1 select c.Number; // 1 = kvinna
                var q21 = from c in qtot where c.Gender == 1 select c.Number;
                var q12 = from c in q    where c.Gender == 2 select c.Number; // 2 = man
                var q22 = from c in qtot where c.Gender == 2 select c.Number;
                double x11 = 0;
                double x21 = 1;
                double x12 = 0;
                double x22 = 1;
                if (q11.Count() > 0)
                    x11 = q11.Sum();
                if (q21.Count() > 0)
                    x21 = q21.Sum();
                if (q12.Count() > 0)
                    x12 = q12.Sum();
                if (q22.Count() > 0)
                    x22 = q22.Sum();
                if (asdiff)
                    x[0] = x11 / x21 - x12 / x22;
                else
                {
                    x[0] = x11 / x21;
                    x[1] = x12 / x22;
                }
            }
            else if (foreign)
            {
                double x11 = 0;
                double x21 = 1;
                double x12 = 0;
                double x22 = 1;
                var q11 = from c in q    where c.Foreignbackground == 7 select c.Number; // 7 = svensk
                var q21 = from c in qtot where c.Foreignbackground == 7 select c.Number;
                var q12 = from c in q    where c.Foreignbackground == 6 select c.Number; // 6 = utländsk
                var q22 = from c in qtot where c.Foreignbackground == 6 select c.Number;
                if (q11.Count() > 0)
                    x11 = q11.Sum();
                if (q21.Count() > 0)
                    x21 = q21.Sum();
                if (q12.Count() > 0)
                    x12 = q12.Sum();
                if (q22.Count() > 0)
                    x22 = q22.Sum();
                if (asdiff)
                    x[0] = x11 / x21 - x12 / x22;
                else
                {
                    x[0] = x11 / x21;
                    x[1] = x12 / x22;
                }
            }
            else if (eduparent)
            {
                var q11 = from c in q    where (bool)c.Educatedparent select c.Number;
                var q21 = from c in qtot where (bool)c.Educatedparent select c.Number;
                var q12 = from c in q    where !(bool)c.Educatedparent select c.Number;
                var q22 = from c in qtot where !(bool)c.Educatedparent select c.Number;
                double x11 = 0;
                double x21 = 1;
                double x12 = 0;
                double x22 = 1;
                if (q11.Count() > 0)
                    x11 = q11.Sum();
                if (q21.Count() > 0)
                    x21 = q21.Sum();
                if (q12.Count() > 0)
                    x12 = q12.Sum();
                if (q22.Count() > 0)
                    x22 = q22.Sum();
                if (asdiff)
                    x[0] = x11 / x21 - x12 / x22;
                else
                {
                    x[0] = x11 / x21;
                    x[1] = x12 / x22;
                }
            }

            return x;
        }

        public void examfreq()
        {
            //int minyear = (from c in db.OV_studentcohort where c.Examyear != null select c.Year).Min();
            //int maxyear = (from c in db.OV_studentcohort where c.Examyear != null select c.Year).Max();
            int minyeardiff = 0;
            int maxyeardiff = 10;
            int examyearmax = (int)(from c in db.OV_studentcohort where c.Examyear != null select c.Examyear).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_studentcohort" }, true);

            //if (CB_startyear.SelectedItem != null)
            //{
            //    int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
            //    if (minset > minyear)
            //        minyear = minset;
            //}
            //if (CB_endyear.SelectedItem != null)
            //{
            //    int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
            //    if (maxset < maxyear)
            //        maxyear = maxset;
            //}

            chart1.ChartAreas[0].AxisX.Maximum = maxyeardiff + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyeardiff - 1;


            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Examensfrekvens";

            bool? progfk = null;
            if (RB_prog.Checked)
                progfk = true;
            else if (RB_fk.Checked)
                progfk = false;
            bool cumulative = CB_cumulative.Checked;
            bool diversitycheck = (RB_examgender.Checked || RB_examforeign.Checked || RB_exameduparent.Checked);
            bool asdiff = CB_diversitydiff.Checked;

            parent.memo("progfk " + progfk);

            double ssmax = 0;
            double ssmin = 0;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Examensfrekvens utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " examensfrekvens";
            chart1.ChartAreas[0].AxisX.Title = "År från programstart";
            chart1.ChartAreas[0].AxisY.Title = "Andel som tagit examen (%)";
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
                chart1.ChartAreas[0].AxisY.Title += " i förhållande till rikssnittet";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + maxyeardiff.ToString() + "=100)";

            string label1 = "";
            string label2 = "";
            if ( RB_examgender.Checked)
            {
                label1 = " kvinnor";
                label2 = " män";
            }
            if (RB_examforeign.Checked)
            {
                label1 = " svensk";
                label2 = " utländsk";
            }
            if (RB_exameduparent.Checked)
            {
                label1 = " högutb föräldr";
                label2 = " lågutb föräldr";
            }

            if ( RB_examgender.Checked && asdiff)
            {
                chart1.Titles["Title1"].Text += " diff kvinnor/män";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel män   |   fördel kvinnor -->";
            }
            else if (RB_examforeign.Checked && asdiff)
            {
                chart1.Titles["Title1"].Text += " diff svensk/utländsk bakgrund";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel utländsk   |   fördel svensk -->";
            }
            else if (RB_exameduparent.Checked && asdiff)
            {
                chart1.Titles["Title1"].Text += " diff hög/lågutbildade föräldrar";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel låg   |   fördel hög -->";
            }

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                for (int yeardiff = minyeardiff; yeardiff <= maxyeardiff; yeardiff++)
                {
                    refdict.Add(yeardiff, examfreqratio(0, yeardiff,examyearmax, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked,progfk,cumulative,asdiff)[0]);
                }
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                Series ss2 = null;
                if (!asdiff)
                {
                    ss.Name += label1;
                    ss2 = new Series(uniname+label2);
                }
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    if (CB_fraction.Checked)
                        yearreference = 0.01 * (examfreqratio(unidict[uniname], maxyeardiff, examyearmax, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, progfk, cumulative,asdiff)[0] / refdict[maxyeardiff]);
                    else
                        yearreference = 0.01 * examfreqratio(unidict[uniname], maxyeardiff, examyearmax, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, progfk, cumulative,asdiff)[0];
                }


                //var q = (from c in db.OV_studentbackground where c.Uni == unidict[uniname] where c.Area == iarea orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int yeardiff = minyeardiff; yeardiff <= maxyeardiff; yeardiff++)
                {
                    double[] amount = examfreqratio(unidict[uniname], yeardiff, examyearmax, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, progfk, cumulative,asdiff);
                    int xmax = 0;
                    if (!asdiff && diversitycheck)
                        xmax = 1;
                    for (int x = 0; x <= xmax; x++)
                    {
                        amount[x] = 100*amount[x] / yearreference;
                        if (CB_fraction.Checked)
                            amount[x] = amount[x] / refdict[yeardiff];
                        if (x == 0)
                            ss.Points.AddXY(yeardiff, amount[x]);
                        else
                            ss2.Points.AddXY(yeardiff, amount[x]);
                        if (CB_sumuni.Checked)
                        {
                            if (!sumdict.ContainsKey(yeardiff))
                                sumdict.Add(yeardiff, amount[x]);
                            else
                                sumdict[yeardiff] += amount[x];
                        }
                        if (amount[x] > ssmax)
                            ssmax = amount[x];
                        if (amount[x] < ssmin)
                            ssmin = amount[x];
                        if (CB_memo.Checked)
                            parent.memo(yeardiff + "\t" + amount[x]);
                    }
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
                if (!asdiff && diversitycheck)
                {
                    ss2.ChartType = SeriesChartType.Line;
                    chart1.Series.Add(ss2);
                }
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            double ssdef = 0.2;
            ssmax = Math.Max(ssmax,Math.Abs(ssmin));
            if (ssmax < ssdef)
                ssmax = ssdef;
            if (ssmin < 0 || asdiff)
                //ssmin = -Math.Max(ssmax, Math.Abs(ssmin));
            {
                double axislength = roundaxis(ssmax - ssmin);
                chart1.ChartAreas[0].AxisY.Maximum = 0.6*axislength;
                chart1.ChartAreas[0].AxisY.Minimum = -0.4*axislength;
            }
            else if (CB_refyear.Checked)
            {
                double axislength = roundaxis(ssmax);
                chart1.ChartAreas[0].AxisY.Maximum = axislength;
                chart1.ChartAreas[0].AxisY.Minimum = 0;

            }
            else 
            {
                double axislength = 100;
                chart1.ChartAreas[0].AxisY.Maximum = axislength;
                chart1.ChartAreas[0].AxisY.Minimum = 0;

            }


            //chart1.ChartAreas[0].AxisY.Maximum = ssmax * 1.8;
            //chart1.ChartAreas[0].AxisY.Minimum = ssmin * 1.2;

        }

        public void creditrate()
        {
            int minyear = (from c in db.OV_studentcohort where c.Creditgroup != null select c.Year).Min();
            int maxyear = (from c in db.OV_studentcohort where c.Creditgroup != null select c.Year).Max(); 
            int baseyear = 2018;
            if (baseyear - minyear > 10)
                minyear = baseyear - 10;
            int mincredit = 300;
            parent.memo("minyear, maxyear = " + minyear + ", " + maxyear);

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_studentcohort" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Genomsnittligt antal HP";
            chart1.ChartAreas[0].AxisX.Title = "År efter studiestart";

            bool? progfk = null;
            if (RB_prog.Checked)
                progfk = true;
            else if (RB_fk.Checked)
                progfk = false;

            parent.memo("progfk " + progfk);

            double ssmax = 0;
            double ssmin = 0;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Medelantal poäng, utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + ", medelantal poäng";
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
                chart1.ChartAreas[0].AxisY.Title += " i förhållande till rikssnittet";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            if (RB_examgender.Checked)
            {
                chart1.Titles["Title1"].Text += " diff kvinnor/män";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel män   |   fördel kvinnor -->";
            }
            else if (RB_examforeign.Checked)
            {
                chart1.Titles["Title1"].Text += " diff svensk/utländsk bakgrund";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel utländsk   |   fördel svensk -->";
            }
            else if (RB_exameduparent.Checked)
            {
                chart1.Titles["Title1"].Text += " diff hög/lågutbildade föräldrar";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel låg   |   fördel hög -->";
            }

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                for (int year = minyear; year <= maxyear; year++)
                {
                    refdict.Add(year, creditfreqratio(0, year, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, mincredit));
                }
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    if (CB_fraction.Checked)
                        yearreference = 0.01 * (creditfreqratio(unidict[uniname], minyear, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, mincredit) / refdict[minyear]);
                    else
                        yearreference = 0.01 * creditfreqratio(unidict[uniname], minyear, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, mincredit);
                }


                //var q = (from c in db.OV_studentbackground where c.Uni == unidict[uniname] where c.Area == iarea orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = creditfreqratio(unidict[uniname], year, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, mincredit);
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(baseyear-year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (amount < ssmin)
                        ssmin = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(baseyear-year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(baseyear-year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            double ssdef = 0.2;
            ssmax = Math.Max(ssmax, Math.Abs(ssmin));
            if (ssmax < ssdef)
                ssmax = ssdef;
            if (ssmin < 0)
            //ssmin = -Math.Max(ssmax, Math.Abs(ssmin));
            {
                double axislength = roundaxis(ssmax - ssmin);
                chart1.ChartAreas[0].AxisY.Maximum = 0.6 * axislength;
                chart1.ChartAreas[0].AxisY.Minimum = -0.4 * axislength;
            }
            else
            {
                double axislength = roundaxis(ssmax);
                chart1.ChartAreas[0].AxisY.Maximum = axislength;
                chart1.ChartAreas[0].AxisY.Minimum = 0;

            }


            //chart1.ChartAreas[0].AxisY.Maximum = ssmax * 1.8;
            //chart1.ChartAreas[0].AxisY.Minimum = ssmin * 1.2;

        }

        public void examfreq_OLD()
        {
            int minyear = (from c in db.OV_studentbackground where c.Exam != null select c.Year).Min();
            int maxyear = (from c in db.OV_studentbackground where c.Exam != null select c.Year).Max() - 3; //Skip last 3 years, no meaningful exam frequency

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_studentbackground" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Examensfrekvens";

            bool? progfk = null;
            if (RB_prog.Checked)
                progfk = true;
            else if (RB_fk.Checked)
                progfk = false;

            parent.memo("progfk " + progfk);

            double ssmax = 0;
            double ssmin = 0;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Examensfrekvens utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " examensfrekvens";
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
                chart1.ChartAreas[0].AxisY.Title += " i förhållande till rikssnittet";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            if (RB_examgender.Checked)
            {
                chart1.Titles["Title1"].Text += " diff kvinnor/män";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel män   |   fördel kvinnor -->";
            }
            else if (RB_examforeign.Checked)
            {
                chart1.Titles["Title1"].Text += " diff svensk/utländsk bakgrund";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel utländsk   |   fördel svensk -->";
            }
            else if (RB_exameduparent.Checked)
            {
                chart1.Titles["Title1"].Text += " diff hög/lågutbildade föräldrar";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel låg   |   fördel hög -->";
            }

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                for (int year = minyear; year <= maxyear; year++)
                {
                    refdict.Add(year, examfreqratio_OLD(0, year, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, progfk));
                }
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    if (CB_fraction.Checked)
                        yearreference = 0.01 * (examfreqratio_OLD(unidict[uniname], minyear, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, progfk) / refdict[minyear]);
                    else
                        yearreference = 0.01 * examfreqratio_OLD(unidict[uniname], minyear, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, progfk);
                }


                //var q = (from c in db.OV_studentbackground where c.Uni == unidict[uniname] where c.Area == iarea orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = examfreqratio_OLD(unidict[uniname], year, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, progfk);
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (amount < ssmin)
                        ssmin = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            double ssdef = 0.2;
            ssmax = Math.Max(ssmax, Math.Abs(ssmin));
            if (ssmax < ssdef)
                ssmax = ssdef;
            if (ssmin < 0)
            //ssmin = -Math.Max(ssmax, Math.Abs(ssmin));
            {
                double axislength = roundaxis(ssmax - ssmin);
                chart1.ChartAreas[0].AxisY.Maximum = 0.6 * axislength;
                chart1.ChartAreas[0].AxisY.Minimum = -0.4 * axislength;
            }
            else
            {
                double axislength = roundaxis(ssmax);
                chart1.ChartAreas[0].AxisY.Maximum = axislength;
                chart1.ChartAreas[0].AxisY.Minimum = 0;

            }


            //chart1.ChartAreas[0].AxisY.Maximum = ssmax * 1.8;
            //chart1.ChartAreas[0].AxisY.Minimum = ssmin * 1.2;

        }

        public void creditrate_OLD()
        {
            int minyear = (from c in db.OV_studentbackground where c.Creditgroup != null select c.Year).Min();
            int maxyear = (from c in db.OV_studentbackground where c.Creditgroup != null select c.Year).Max(); //Skip last 3 years, no meaningful exam frequency
            int baseyear = 2018;
            if (baseyear - minyear > 10)
                minyear = baseyear - 10;
            int mincredit = 300;
            parent.memo("minyear, maxyear = " + minyear + ", " + maxyear);

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_studentbackground" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Andel som tagit minst X poäng";
            chart1.ChartAreas[0].AxisX.Title = "År efter studiestart";

            bool? progfk = null;
            if (RB_prog.Checked)
                progfk = true;
            else if (RB_fk.Checked)
                progfk = false;

            parent.memo("progfk " + progfk);

            double ssmax = 0;
            double ssmin = 0;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Andel som tagit X poäng, utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + ", andel som tagit minst X poäng";
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
                chart1.ChartAreas[0].AxisY.Title += " i förhållande till rikssnittet";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            if (RB_examgender.Checked)
            {
                chart1.Titles["Title1"].Text += " diff kvinnor/män";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel män   |   fördel kvinnor -->";
            }
            else if (RB_examforeign.Checked)
            {
                chart1.Titles["Title1"].Text += " diff svensk/utländsk bakgrund";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel utländsk   |   fördel svensk -->";
            }
            else if (RB_exameduparent.Checked)
            {
                chart1.Titles["Title1"].Text += " diff hög/lågutbildade föräldrar";
                chart1.ChartAreas[0].AxisY.Title = "<-- fördel låg   |   fördel hög -->";
            }

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                for (int year = minyear; year <= maxyear; year++)
                {
                    refdict.Add(year, creditfreqratio(0, year, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, mincredit));
                }
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    if (CB_fraction.Checked)
                        yearreference = 0.01 * (creditfreqratio(unidict[uniname], minyear, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, mincredit) / refdict[minyear]);
                    else
                        yearreference = 0.01 * creditfreqratio(unidict[uniname], minyear, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, mincredit);
                }


                //var q = (from c in db.OV_studentbackground where c.Uni == unidict[uniname] where c.Area == iarea orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = creditfreqratio(unidict[uniname], year, RB_allexamfreq.Checked, RB_examgender.Checked, RB_examforeign.Checked, RB_exameduparent.Checked, mincredit);
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(baseyear - year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (amount < ssmin)
                        ssmin = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(baseyear - year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(baseyear - year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            double ssdef = 0.2;
            ssmax = Math.Max(ssmax, Math.Abs(ssmin));
            if (ssmax < ssdef)
                ssmax = ssdef;
            if (ssmin < 0)
            //ssmin = -Math.Max(ssmax, Math.Abs(ssmin));
            {
                double axislength = roundaxis(ssmax - ssmin);
                chart1.ChartAreas[0].AxisY.Maximum = 0.6 * axislength;
                chart1.ChartAreas[0].AxisY.Minimum = -0.4 * axislength;
            }
            else
            {
                double axislength = roundaxis(ssmax);
                chart1.ChartAreas[0].AxisY.Maximum = axislength;
                chart1.ChartAreas[0].AxisY.Minimum = 0;

            }


            //chart1.ChartAreas[0].AxisY.Maximum = ssmax * 1.8;
            //chart1.ChartAreas[0].AxisY.Minimum = ssmin * 1.2;

        }

        public void prestationsgrad()
        {
            prestationsgrad(0);
        }

        public void prestationsgrad(int iarea)
        {
            int minyear = (from c in db.OV_hsthpr select c.Year).Min();
            int maxyear = (from c in db.OV_hsthpr select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_hsthpr" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1;
            chart1.ChartAreas[0].AxisX.Interval = 1;


            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Prestationsgrad";

            double ssmax = 0;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Prestationsgrad utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " prestationsgrad";
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
                chart1.ChartAreas[0].AxisY.Title += " i förhållande till rikssnittet";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_hsthpr where c.Uni == 0 where c.Area == iarea orderby c.Year select c);
                foreach (OV_hsthpr oi in qt)
                {
                    refdict.Add(oi.Year, oi.HPR/oi.HST);
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    OV_hsthpr oi = (from c in db.OV_hsthpr where c.Uni == unidict[uniname] where c.Area == iarea where c.Year == minyear select c).FirstOrDefault();
                    if (oi != null)
                    {
                        if (CB_fraction.Checked)
                            yearreference = 0.01 * ((oi.HPR/oi.HST) / refdict[minyear]);
                        else
                            yearreference = 0.01 * oi.HPR/oi.HST;
                    }
                }


                var q = (from c in db.OV_hsthpr where c.Uni == unidict[uniname] where c.Area == iarea orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    double hst = 0;
                    double hpr = 0;
                    foreach (OV_hsthpr oi in (from c in q where c.Year == year select c))
                    {
                        if (oi != null)
                        {
                            hst += oi.HST;
                            hpr += oi.HPR;
                        }
                    }
                    if (hst > 0)
                        amount = hpr / hst;
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
        }

        public void examgroup_stackedarea(string selitem)
        {
            this.Cursor = Cursors.WaitCursor;

            int minyear = (from c in db.OV_exam select c.Year).Min();
            int maxyear = (from c in db.OV_exam select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_exam" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }
            parent.memo("minyear, maxyear = " + minyear + ", " + maxyear);
            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            double ssmax = 0;

            Dictionary<string, int> incomedict = fill_incomedict();

            Dictionary<int, double> pengdict = fill_pengdict(0.8);

            Dictionary<string, int> examdict = new Dictionary<string, int>();

            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Antal examina per år";

            // Set the text of the title
            chart1.Titles["Title1"].Text = focusname + " examina "+selitem;
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            var qtype = from c in db.OV_examtype where c.Grp == examgroupdict[selitem].ToString() select c;
            List<Series> ls = new List<Series>();
            //double prestation = 0.8;
            foreach (OV_examtype oi in qtype)
            {
                if (oi.Name.Contains("Total"))
                    continue;

                ls.Add(new Series(oi.Name));
                examdict.Add(oi.Name, oi.Id);
            }

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_exam where c.Uni == 0 where c.Examtype0 == 0 orderby c.Year select c);
                foreach (OV_exam oi in qt)
                {
                    refdict.Add(oi.Year, oi.Number);
                }
            }

            double yearreference = 1;
            if (CB_refyear.Checked)
            {
                OV_exam oi = (from c in db.OV_exam 
                                where c.Uni == focusuniversity 
                                where c.Examtype1 == 0
                              where c.Gender == 0
                              where c.Age == 0
                              where c.Year == minyear 
                                select c).FirstOrDefault();
                if (oi != null)
                {
                    if (CB_fraction.Checked)
                        yearreference = 0.01 * (oi.Number / refdict[minyear]);
                    else
                        yearreference = 0.01 * oi.Number;
                }
            }

            Dictionary<int, double> ssmaxdict = new Dictionary<int, double>();
            for (int year = minyear; year <= maxyear; year++)
                ssmaxdict.Add(year, 0);

            foreach (Series ss in ls)
            {
                ss.ChartType = SeriesChartType.StackedArea;
                var q = (from c in db.OV_exam 
                         where c.Uni == focusuniversity
                         where ((c.Examtype0 == examdict[ss.Name] && c.Examtype1 == 0) || (c.Examtype1 == examdict[ss.Name] && c.Examtype2 == 0) || (c.Examtype2 == examdict[ss.Name]))
                         where c.Gender == 0
                         where c.Age == 0
                         orderby c.Year select c);
                //var qtot = null;
                //if (CB_fraction.Checked)
                //    qtot = (from c in db.OV_hsthpr where c.Uni == 0 where c.Incometype == 0 where c.Incomesource == incomedict[ss.Name] orderby c.Year select c);
                for (int year = minyear; year <= maxyear; year++)
                //foreach (OV_hsthpr oi in q)
                {
                    //OV_hsthpr oi = (from c in q where c.Year == year select c).FirstOrDefault();
                    //double amount = 0;
                    //if (oi != null)
                    //    amount = oi.Amount;
                    var qoi = (from c in q where c.Year == year select c);
                    double amount = 0;
                    foreach (OV_exam oi in qoi)
                        amount += oi.Number;

                    amount = amount / yearreference;
                    if (CB_HSTpeng.Checked)
                        amount = HST_to_money(amount, incomedict[ss.Name], pengdict);
                    if (CB_fraction.Checked)
                    {
                        amount = amount / refdict[year];
                        if (!CB_refyear.Checked)
                            amount *= 100;
                    }

                    ss.Points.AddXY(year, amount);

                    ssmaxdict[year] += amount;

                    //if (amount > ssmax)
                    //    ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(ss.Name + "\t" + year + "\t" + amount);
                }

                chart1.Series.Add(ss);
            }

            for (int year = minyear; year <= maxyear; year++)
                if (ssmaxdict[year] > ssmax)
                    ssmax = ssmaxdict[year];

            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;

        }


        
        public void totalexam()
        {
            totalexam(new int[] { 0, 0, 0 },0,0);
        }

        public void totalexam(int[] itypes, int gender, int age)
        {
            parent.memo("totalexam");
            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_exam select c.Year).Min();
            int maxyear = (from c in db.OV_exam select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_exam" }, true);

            if (RB_permoney.Checked)
            {
                int minyearmoney = (from c in db.OV_University_Income select c.Year).Min();
                int maxyearmoney = (from c in db.OV_University_Income select c.Year).Max();
                minyear = Math.Max(minyear, minyearmoney);
                maxyear = Math.Min(maxyear, maxyearmoney);
            }
            else if (RB_perscientist.Checked)
            {
                int minyearsci = (from c in db.OV_staff select c.Year).Min();
                int maxyearsci = (from c in db.OV_staff select c.Year).Max();
                minyear = Math.Max(minyear, minyearsci);
                maxyear = Math.Min(maxyear, maxyearsci);
            }
            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }



            if (CB_startyear.SelectedItem != null)
            {
                minyear = util.tryconvert(CB_startyear.SelectedItem.ToString());
            }
            if (CB_endyear.SelectedItem != null)
            {
                maxyear = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxyear < 0)
                    maxyear = 9999;
            }
            parent.memo("minyear, maxyear = " + minyear + ", " + maxyear);

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Antal examina";

            if ( RB_permoney.Checked)
            {
                if ( !CB_reverse.Checked)
                {
                    chart1.ChartAreas[0].AxisY.Title = "Antal examina per Mkr";
                }
                else
                {
                    chart1.ChartAreas[0].AxisY.Title = "Mkr per examen";
                }
            }
            else if (RB_perscientist.Checked)
            {
                if (!CB_reverse.Checked)
                {
                    chart1.ChartAreas[0].AxisY.Title = "Antal examina per lärare";
                }
                else
                {
                    chart1.ChartAreas[0].AxisY.Title = "Lärare per examen";
                }
            }

            string priceindex = get_priceindex();

            double ssmax = 0;

            int itype = 0;
            int icol = 0;
            for (int i=0;i<itypes.Length;i++)
                if (itypes[i] > 0)
                {
                    itype = itypes[i];
                    icol = i;
                }
            parent.memo("itype, icol = " + itype + ", " + icol);

            string examname = "totalt";
            if (itype > 0)
            {
                examname = (from c in db.OV_examtype where c.Id == itype select c.Name).FirstOrDefault();
            }
            string resourcetype = "teaching";
            if (examname.Contains("PhD"))
                resourcetype = "research";

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Examina " + examname + " utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " examina " + examname;
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";
            if (RB_permoney.Checked)
            {

                if (resourcetype == "research")
                {
                    if (CB_reverse.Checked)
                        chart1.Titles["Title1"].Text = "Mkr forskningsmedel " + priceindex + " per " + chart1.Titles["Title1"].Text;
                    else
                        chart1.Titles["Title1"].Text += " per Mkr forskningsmedel " + priceindex;
                }
                else
                {
                    if (CB_reverse.Checked)
                        chart1.Titles["Title1"].Text = "Mkr takbelopp " + priceindex + " per " + chart1.Titles["Title1"].Text;
                    else
                        chart1.Titles["Title1"].Text += " per Mkr takbelopp " + priceindex;
                }
            }
            else if (RB_perscientist.Checked)
                chart1.Titles["Title1"].Text += " per lärare";


            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
                parent.memo(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            IEnumerable<dynamic> qtype = null;
            int all = 0;
            //if (itype > 162) //For PhD, "all" is 163
            //    all = 163;
            if (icol == 0)
                qtype = (from c in db.OV_exam where c.Examtype0 == itype where c.Examtype1 == all where c.Examtype2 == all select c);
            else if (icol == 1)
                qtype = (from c in db.OV_exam where c.Examtype1 == itype where c.Examtype2 == all select c);
            else if (icol == 2)
                qtype = (from c in db.OV_exam where c.Examtype2 == itype select c);

            int moneyunit = 1000;
            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                IEnumerable<dynamic> qt = (from c in qtype where c.Uni == 0 where c.Gender == gender where c.Age == age orderby c.Year select c);

                foreach (OV_exam oi in qt)
                {
                    refdict.Add(oi.Year, oi.Number);
                }

                if (RB_permoney.Checked)
                {
                    //var qmoney = from c in db.OV_University_Income
                    //             where c.Uni == 0
                    //             where c.Incometype == 6
                    //             select c;
                    for (int i = minyear; i <= maxyear; i++)
                    {
                        double amount = get_income(0,i,resourcetype) * adjustprice(i, priceindex);
                        refdict[i] = moneyunit * refdict[i] / amount;
                    }

                }
                else if (RB_perscientist.Checked)
                {
                    //var qsci = from c in db.OV_staff
                    //           where c.Uni == 0
                    //           where c.OV_stafftype.Teacher == true
                    //           select c;
                    for (int i = minyear; i <= maxyear; i++)
                    {
                        double amount = get_staff(0,i,resourcetype);
                        refdict[i] = refdict[i] / amount;
                    }

                }


                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();

            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    OV_exam oi = (from c in qtype where c.Uni == unidict[uniname] where c.Gender == gender where c.Age == age where c.Year == minyear select c).FirstOrDefault();
                    if (oi != null)
                    {
                        if (CB_fraction.Checked)
                            yearreference = 0.01 * (oi.Number / refdict[minyear]);
                        else
                            yearreference = 0.01 * oi.Number;
                    }
                }


                var q = (from c in qtype where c.Uni == unidict[uniname] where c.Gender == gender where c.Age == age orderby c.Year select c);
                parent.memo("q.Count = " + q.Count());
                double totalexam = 0;
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    foreach (OV_exam oi in (from c in q where c.Year == year  where c.Gender == gender where c.Age == age select c))
                    {
                        if (oi != null)
                        {
                            amount += oi.Number;
                        }
                    }

                    totalexam += amount;

                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];

                    if (RB_permoney.Checked)
                    {
                        double money = get_income(unidict[uniname],year,resourcetype);
                        //var qmoney = from c in db.OV_University_Income
                        //             where c.Uni == unidict[uniname]
                        //             where c.Year == year
                        //             where c.Incometype == 6
                        //             where c.Incomesource == 0
                        //             select c.Amount;
                        //if (qmoney.Count() > 0)
                        money = money * adjustprice(year, priceindex);
                        parent.memo("year, amount, money = " +year + ", "+ amount + ", " + money);
                        if (money > 0)
                            amount = moneyunit * amount / money;
                        if (CB_reverse.Checked && amount > 0)
                            amount = 1 / amount;
                    }
                    else if (RB_perscientist.Checked)
                    {
                        double sci = get_staff(unidict[uniname],year,resourcetype);
                        //var qsci = from c in db.OV_staff
                        //           where c.Uni == unidict[uniname]
                        //           where c.Year == year
                        //           where c.OV_stafftype.Teacher == true
                        //           select c.Number;
                        //if (qsci.Count() > 0)
                        //    sci = qsci.Sum();
                        parent.memo("amount, sci = " + amount + ", " + sci);
                        if (sci > 0)
                            amount = amount / sci;
                        if (CB_reverse.Checked && amount > 0)
                            amount = 1 / amount;
                    }


                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }

                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);

                parent.memo(uniname + "\t" + totalexam);
            }

            if ( CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if ( CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year]/unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                sumseries.ChartType = SeriesChartType.Line;
                chart1.Series.Add(sumseries);
                
            }

            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;
        }

        public bool goodstaff(OV_staff os,int itype, bool phd, bool support)
        {
            
            if (phd)
                return (bool)os.OV_stafftype.PhD;
            else if ( support)
                return (os.Stafftype > 0) && !((bool)os.OV_stafftype.Researcher || (bool)os.OV_stafftype.Teacher);
            else
                return (os.Stafftype == itype);
        }

        public void agestaff(int itype, bool phd, bool support)
        {
            Dictionary<int, double> agegroupdict = new Dictionary<int, double>()
            {
                {11,25 },
                {12,40 },
                {13,47 },
                {14,52 },
                {15,57 },
                {16,64 }
            };


            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_staff select c.Year).Min();
            int maxyear = (from c in db.OV_staff select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_staff" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }
            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Medelålder (år) ";
            if (itype > 0)
                chart1.ChartAreas[0].AxisY.Title += (string)LB_staff.SelectedItem;
            double ssmax = 0;

            string areaname = (string)LB_staff.SelectedItem;
            if (itype > 0)
            {
                areaname = (from c in db.OV_stafftype where c.Id == itype select c.Name).FirstOrDefault();
            }

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Medelålder anställda " + areaname + " utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " medelålder anställda " + areaname;
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
                chart1.ChartAreas[0].AxisY.Title = "I förhållande till rikssnittet";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";
            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            //Dictionary<int, double> refdict = new Dictionary<int, double>();
            //if (CB_fraction.Checked)
            //{
            //    var qt = (from c in db.OV_staff 
            //              where c.Uni == 0 
            //              where c.Gender == 0
            //              where c.Age == 0
            //              //where c.Stafftype != 0
            //              orderby c.Year
            //              select c);
                
            //    for (int year = minyear; year <= maxyear; year++)
            //    {
            //        double amount = 0;
            //        double total = 0;
            //        foreach (OV_staff oi in (from c in qt
            //                                 where c.Year == year
            //                                 select c))
            //        {
            //            if (oi != null)
            //            {
            //                if (goodstaff(oi, itype, phd, support))
            //                    amount += oi.Number;
            //                if ( oi.Stafftype == 0)
            //                    total += oi.Number;
            //            }
            //        }
            //        if ( !CB_staffabsolute.Checked && itype != 0)
            //            amount = amount / total;
            //        refdict.Add(year, amount);
            //    }
                    

            //    //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            //}

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                //if (CB_refyear.Checked)
                //{
                //    var qref = (from c in db.OV_staff
                //                where c.Uni == unidict[uniname]
                //                where goodstaff(c, itype, phd, support)
                //                where c.Gender == 0
                //                where c.Age > 0
                //                //where c.Stafftype != 0
                //                where c.Year == minyear
                //                select c);
                //    if (qref.Count() > 0)
                //    {
                //        double amount = 0;
                //        double total = 0;
                //        foreach (OV_staff oi in (from c in qref
                //                                 select c))
                //        {
                //            if (oi != null)
                //            {
                //                if (goodstaff(oi, itype, phd, support))
                //                    amount += oi.Number;
                //                if (oi.Stafftype == 0)
                //                    total += oi.Number;
                //            }
                //        }
                //        if (!CB_staffabsolute.Checked)
                //            amount = amount / total;
                //        if (CB_fraction.Checked)
                //            yearreference = 0.01 * ( amount/ refdict[minyear]);
                //        else
                //            yearreference = 0.01 * amount;
                //    }
                //}


                var q = (from c in db.OV_staff 
                         where c.Uni == unidict[uniname]
                         where c.Gender == 0
                         where c.Age > 0
                         orderby c.Year
                         //where c.Stafftype != 0
                         select c);
                //var qwomen = q;
                //if ( RB_examgender.Checked)
                //{
                    
                //    qwomen = (from c in db.OV_staff
                //             where c.Uni == unidict[uniname]
                //             where c.Gender == 1
                //             where c.Age == 0
                //             orderby c.Year
                //             //where c.Stafftype != 0
                //             select c);
                //}
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    double total = 0;
                    double agesum = 0;
                    //double amountwomen = 0;
                    foreach (OV_staff oi in (from c in q 
                                             where c.Year == year
                                             select c))
                    {
                        if (oi != null)
                        {
                            if (goodstaff(oi, itype, phd, support))
                            {
                                amount += oi.Number;
                                agesum += oi.Number * agegroupdict[oi.Age];
                            }
                            if (oi.Stafftype == 0)
                                total += oi.Number;
                        }
                    }

                    if (amount > 0)
                        amount = agesum / amount;

                    //if (RB_examgender.Checked)
                    //{

                    //    foreach (OV_staff oi in (from c in qwomen
                    //                             where c.Year == year
                    //                             select c))
                    //    {
                    //        if (oi != null)
                    //        {
                    //            if (goodstaff(oi, itype, phd, support))
                    //                amountwomen += oi.Number;
                    //        }
                    //    }
                    //    if ( amount > 0)
                    //        amount = amountwomen / amount;
                    //}
                    //else
                    {
                        //if (!CB_staffabsolute.Checked)
                        //    amount = amount / total;
                        amount = amount / yearreference;
                        //if (CB_fraction.Checked)
                        //    amount = amount / refdict[year];
                    }
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            chart1.ChartAreas[0].AxisY.Maximum = roundaxis(ssmax);
            this.Cursor = Cursors.Default;
        }

        public void totalstaff(int itype, bool phd, bool support)
        {
            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_staff select c.Year).Min();
            int maxyear = (from c in db.OV_staff select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_staff" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }
            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            //Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = (string)LB_staff.SelectedItem;
            double ssmax = 0;

            string areaname = (string)LB_staff.SelectedItem;
            if (itype > 0)
            {
                areaname = (from c in db.OV_stafftype where c.Id == itype select c.Name).FirstOrDefault();
            }

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Anställda " + areaname + " utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " anställda " + areaname;
            if (RB_examgender.Checked)
            {
                chart1.Titles["Title1"].Text += " andel kvinnor";
                chart1.ChartAreas[0].AxisY.Title = "Andel kvinnor bland " + areaname;
            }
            else
            {
                if (CB_fraction.Checked)
                {
                    chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
                    chart1.ChartAreas[0].AxisY.Title = "I förhållande till rikssnittet";
                }
                if (CB_refyear.Checked)
                    chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";
            }
            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_staff
                          where c.Uni == 0
                          where c.Gender == 0
                          where c.Age == 0
                          //where c.Stafftype != 0
                          orderby c.Year
                          select c);

                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    double total = 0;
                    foreach (OV_staff oi in (from c in qt
                                             where c.Year == year
                                             select c))
                    {
                        if (oi != null)
                        {
                            if (goodstaff(oi, itype, phd, support))
                                amount += oi.Number;
                            if (oi.Stafftype == 0)
                                total += oi.Number;
                        }
                    }
                    if (!CB_staffabsolute.Checked && itype != 0)
                        amount = amount / total;
                    refdict.Add(year, amount);
                }


                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    var qref = (from c in db.OV_staff
                                where c.Uni == unidict[uniname]
                                where goodstaff(c, itype, phd, support)
                                where c.Gender == 0
                                where c.Age == 0
                                //where c.Stafftype != 0
                                where c.Year == minyear
                                select c);
                    if (qref.Count() > 0)
                    {
                        double amount = 0;
                        double total = 0;
                        foreach (OV_staff oi in (from c in qref
                                                 select c))
                        {
                            if (oi != null)
                            {
                                if (goodstaff(oi, itype, phd, support))
                                    amount += oi.Number;
                                if (oi.Stafftype == 0)
                                    total += oi.Number;
                            }
                        }
                        if (!CB_staffabsolute.Checked)
                            amount = amount / total;
                        if (CB_fraction.Checked)
                            yearreference = 0.01 * (amount / refdict[minyear]);
                        else
                            yearreference = 0.01 * amount;
                    }
                }


                var q = (from c in db.OV_staff
                         where c.Uni == unidict[uniname]
                         where c.Gender == 0
                         where c.Age == 0
                         orderby c.Year
                         //where c.Stafftype != 0
                         select c);
                var qwomen = q;
                if (RB_examgender.Checked)
                {

                    qwomen = (from c in db.OV_staff
                              where c.Uni == unidict[uniname]
                              where c.Gender == 1
                              where c.Age == 0
                              orderby c.Year
                              //where c.Stafftype != 0
                              select c);
                }
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    double total = 0;
                    double amountwomen = 0;
                    foreach (OV_staff oi in (from c in q
                                             where c.Year == year
                                             select c))
                    {
                        if (oi != null)
                        {
                            if (goodstaff(oi, itype, phd, support))
                                amount += oi.Number;
                            if (oi.Stafftype == 0)
                                total += oi.Number;
                        }
                    }
                    if (RB_examgender.Checked)
                    {

                        foreach (OV_staff oi in (from c in qwomen
                                                 where c.Year == year
                                                 select c))
                        {
                            if (oi != null)
                            {
                                if (goodstaff(oi, itype, phd, support))
                                    amountwomen += oi.Number;
                            }
                        }
                        if (amount > 0)
                            amount = amountwomen / amount;
                    }
                    else
                    {
                        if (!CB_staffabsolute.Checked)
                            amount = amount / total;
                        amount = amount / yearreference;
                        if (CB_fraction.Checked)
                            amount = amount / refdict[year];
                    }
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            chart1.ChartAreas[0].AxisY.Maximum = roundaxis(ssmax);
            this.Cursor = Cursors.Default;
        }


        public void totalhst()
        {
            totalhst(0, 0);
        }

        public void averagepeng()
        {
            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_hsthpr select c.Year).Min();
            int maxyear = (from c in db.OV_hsthpr select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_hsthpr" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = fill_incomedict();

            Dictionary<int, double> pengdict = fill_pengdict(0.8);

            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "1000-kr per student";
            double ssmax = 0;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Genomsnittlig studentpeng utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = "Genomsnittlig studentpeng "+focusname;
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande riket";
                chart1.ChartAreas[0].AxisY.Title = "I förhållande till riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            Dictionary<int, double> hstdict = new Dictionary<int, double>();
            Dictionary<int, double> moneydict = new Dictionary<int, double>();
            for (int i = minyear; i <= maxyear; i++)
            {
                hstdict.Add(i, 0);
                moneydict.Add(i, 0);
            }

            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_hsthpr 
                          where c.Uni == 0 
                          where c.Area > 0
                          where c.Year >= minyear where c.Year <= maxyear 
                          orderby c.Year select c);
                foreach (OV_hsthpr oi in qt)
                {
                    hstdict[oi.Year] += oi.HST;
                    moneydict[oi.Year] += HST_to_money(oi.HST, oi.Area, pengdict);
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }
            for (int i = minyear; i <= maxyear; i++)
            {
                refdict.Add(i, moneydict[i] / hstdict[i]);
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                //if (CB_refyear.Checked)
                //{
                //    OV_hsthpr oi = (from c in db.OV_hsthpr 
                //                    where c.Uni == unidict[uniname] 
                //                    where c.Area > 0 
                //                    where c.Year == minyear 
                //                    select c).FirstOrDefault();
                //    if (oi != null)
                //    {
                //        if (CB_fraction.Checked)
                //            yearreference = 0.01 * (oi.HST / refdict[minyear]);
                //        else
                //            yearreference = 0.01 * oi.HST;
                //    }
                //}


                var q = (from c in db.OV_hsthpr 
                         where c.Uni == unidict[uniname] 
                         where c.Area > 0 
                         orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double hst = 0;
                    double money = 0;
                    foreach (OV_hsthpr oi in (from c in q 
                                              where c.Year == year select c))
                    {
                        if (oi != null)
                        {
                            hst += oi.HST;
                            money += HST_to_money(oi.HST, oi.Area, pengdict);
                        }
                    }
                    double amount = money / hst;
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;

        }

        public Dictionary<int,double> fill_moneyfracdict(int uni, int minyear, int maxyear, int incometype, int incomesource)
        {
            //what fraction of uni income comes from specified type and source?
            Dictionary<int, double> moneyfracdict = new Dictionary<int, double>();
            for (int i = minyear; i <= maxyear; i++)
            {
                moneyfracdict.Add(i,0);
            }

            var qiu = (from c in db.OV_University_Income
                       where c.Uni == uni
                       where c.Incometype == incometype //utbildning=6
                       where c.Incomesource == incomesource
                       where c.Year >= minyear
                       where c.Year <= maxyear
                       orderby c.Year
                       select c);
            foreach (OV_University_Income oi in qiu)
            {
                moneyfracdict[oi.Year] = oi.Amount;
            }
            var qit = (from c in db.OV_University_Income
                       where c.Uni == uni
                       where c.Incometype == 0 //total
                       where c.Incomesource == 0 //total
                       where c.Year >= minyear
                       where c.Year <= maxyear
                       orderby c.Year
                       select c);
            foreach (OV_University_Income oi in qit)
            {
                moneyfracdict[oi.Year] = moneyfracdict[oi.Year] / oi.Amount;
            }

            return moneyfracdict;
        }

        public Dictionary<int,double> fill_staffdict(int uni, int minyear, int maxyear, int incometype, int incomesource )
        {
            //how many staff are financed by the specified income type and source?
            Dictionary<int, double> staffdict = new Dictionary<int, double>();
            Dictionary<int, double> moneyfracdict = new Dictionary<int, double>();

            for (int i = minyear; i <= maxyear; i++)
            {
                staffdict.Add(i,0);
            }
            var qs = (from c in db.OV_staff
                      where c.Uni == uni
                      where c.Stafftype == 0
                      where c.Gender == 0
                      where c.Age == 0
                      where c.Year >= minyear
                      where c.Year <= maxyear
                      orderby c.Year
                      select c);
            foreach (OV_staff oi in qs)
            {
                staffdict[oi.Year] = oi.Number;
            }

            if (incometype > 0 || incomesource > 0)
            {
                moneyfracdict = fill_moneyfracdict(uni, minyear, maxyear, 6, 0);


                for (int i = minyear; i <= maxyear; i++)
                {
                    staffdict[i] = staffdict[i] * moneyfracdict[i];
                }
            }

            return staffdict;
        }

        public Dictionary<int,double> student_teacher_ratio_dict(int uni, int minyear, int maxyear)
        {
            Dictionary<int, double> refdict = new Dictionary<int, double>();
            Dictionary<int, int> regdict = new Dictionary<int, int>();
            Dictionary<int, double> staffdict = new Dictionary<int, double>();
            Dictionary<int, double> moneyfracdict = new Dictionary<int, double>();
            for (int i = minyear; i <= maxyear; i++)
            {
                regdict.Add(i, 0);
                staffdict.Add(i, 0);
                moneyfracdict.Add(i, 0);
            }

            var qt = (from c in db.OV_registered
                        where c.Uni == uni
                        where c.Gender == 0
                        where c.Age == 0
                        where c.Year >= minyear
                        where c.Year <= maxyear
                        orderby c.Year
                        select c);
            foreach (OV_registered oi in qt)
            {
                regdict[oi.Year] += oi.Number;
            }

            staffdict = fill_staffdict(uni, minyear, maxyear, 6, 0);

            for (int i = minyear; i <= maxyear; i++)
            {
                refdict.Add(i, regdict[i] / staffdict[i]);
            }

            return refdict;
        }

        public Dictionary<int,double> fill_hstdict(int uni, int minyear, int maxyear)
        {
            Dictionary<int, double> hstdict = new Dictionary<int, double>();
            for (int i = minyear; i <= maxyear; i++)
            {
                hstdict.Add(i, 0);
            }

            if (CB_HSTpeng.Checked)
            {
                Dictionary<int, OV_subjectarea> pengdict = fill_pengdict2();
                var qt = (from c in db.OV_hsthpr where c.Uni == uni where c.Area != 0 orderby c.Year select c);
                foreach (OV_hsthpr oi in qt)
                {
                    if (hstdict.ContainsKey(oi.Year))
                        hstdict[oi.Year] += oi.HST * (double)pengdict[oi.Area].HSTpeng2018 + oi.HPR * (double)pengdict[oi.Area].HPRpeng2018;
                }

            }
            else
            {
                var qt = (from c in db.OV_hsthpr where c.Uni == uni where c.Area == 0 orderby c.Year select c);

                foreach (OV_hsthpr oi in qt)
                {
                    if (hstdict.ContainsKey(oi.Year))
                        hstdict[oi.Year] += oi.HST;
                }
            }

            return hstdict;
        }

        public Dictionary<int, double> studHST_ratio_dict(int uni, int minyear, int maxyear)
        {
            Dictionary<int, double> refdict = new Dictionary<int, double>();
            Dictionary<int, double> hstdict = new Dictionary<int, double>();
            Dictionary<int, int> regdict = new Dictionary<int, int>();


            for (int i = minyear; i <= maxyear; i++)
            {
                regdict.Add(i, 0);
            }

            var qr = (from c in db.OV_registered
                      where c.Uni == uni
                      where c.Gender == 0
                      where c.Age == 0
                      where c.Year >= minyear
                      where c.Year <= maxyear
                      orderby c.Year
                      select c);
            foreach (OV_registered oi in qr)
            {
                regdict[oi.Year] += oi.Number;
            }

            hstdict = fill_hstdict(uni, minyear, maxyear);

            for (int i = minyear; i <= maxyear; i++)
            {
                refdict.Add(i, regdict[i]/hstdict[i]);
            }

            return refdict;
        }

        public Dictionary<int, double> HST_teacher_ratio_dict(int uni, int minyear, int maxyear)
        {
            Dictionary<int, double> refdict = new Dictionary<int, double>();
            Dictionary<int, double> hstdict = new Dictionary<int, double>();
            Dictionary<int, double> staffdict = new Dictionary<int, double>();
            Dictionary<int, double> moneyfracdict = new Dictionary<int, double>();
            for (int i = minyear; i <= maxyear; i++)
            {
                hstdict.Add(i, 0);
                staffdict.Add(i, 0);
                moneyfracdict.Add(i, 0);
            }

            hstdict = fill_hstdict(uni, minyear, maxyear);
            //var qt = (from c in db.OV_hsthpr where c.Uni == uni where c.Area == 0 orderby c.Year select c);

            //foreach (OV_hsthpr oi in qt)
            //{
            //    if (hstdict.ContainsKey(oi.Year))
            //        hstdict[oi.Year] += oi.HST;
            //}

            staffdict = fill_staffdict(uni, minyear, maxyear, 6, 0);

            for (int i = minyear; i <= maxyear; i++)
            {
                if (staffdict[i] > 0)
                    refdict.Add(i, hstdict[i] / staffdict[i]);
                else
                    refdict.Add(i, 0);
            }

            return refdict;
        }

        public void HSTteacher_ratio()
        {
            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_staff select c.Year).Min();
            int maxyear = (from c in db.OV_staff select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_hsthpr" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = fill_incomedict();

            //Dictionary<int, double> pengdict = fill_pengdict(0.8);

            chart1.Series.Clear();
            string perstring = "HST per ";
            if (CB_HSTpeng.Checked)
                perstring = "GU-intäkt (kkr) per ";
            chart1.ChartAreas[0].AxisY.Title = perstring +"anställd";
            double ssmax = 0;
            double ssmin = 1e20;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = perstring + "utbildningsanställd";
            else
                chart1.Titles["Title1"].Text = perstring + "utbildningsanställd " + focusname;
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande riket";
                chart1.ChartAreas[0].AxisY.Title = "I förhållande till riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = HST_teacher_ratio_dict(0, minyear, maxyear);
            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;

            StringBuilder sb = new StringBuilder(chart1.Titles["Title1"].Text+"\n");
            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                Dictionary<int, double> ratiodict = HST_teacher_ratio_dict(unidict[uniname], minyear, maxyear);
                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    yearreference = ratiodict[minyear];
                }


                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = ratiodict[year];
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    if (Double.IsInfinity(amount))
                    {
                        parent.memo("Bad value for " + uniname + " " + year);
                    }
                    ss.Points.AddXY(year, amount);
                    if (amount > ssmax)
                        ssmax = amount;
                    if (amount < ssmin)
                        ssmin = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                    if (CB_values_to_file.Checked)
                        sb.Append(ss.Name+"\t"+year + "\t" + amount+"\n");
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }


            if (CB_values_to_file.Checked)
            {
                SaveValuesToFile(sb, chart1.Titles["Title1"].Text);
            }


            chart1.ChartAreas[0].AxisY.Maximum = roundaxis(1.1*ssmax);
            if (CB_axiszero.Checked || ssmin >= ssmax)
                chart1.ChartAreas[0].AxisY.Minimum = 0;
            else
                chart1.ChartAreas[0].AxisY.Minimum = roundaxis(ssmin * 0.8);
            this.Cursor = Cursors.Default;

        }


        public void studHST_ratio()
        {
            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_hsthpr select c.Year).Min();
            int maxyear = (from c in db.OV_hsthpr select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_hsthpr" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            //Dictionary<string, int> incomedict = fill_incomedict();

            //Dictionary<int, double> pengdict = fill_pengdict(0.8);

            chart1.Series.Clear();
            string perstring = "per HST";
            if (CB_HSTpeng.Checked)
                perstring = "per GU-intäkt (Mkr)";
            chart1.ChartAreas[0].AxisY.Title = "Individer "+perstring;
            double ssmax = 0;
            double ssmin = 1e20;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Individer "+perstring;
            else
                chart1.Titles["Title1"].Text = "Individer " +perstring + " " + focusname;
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande riket";
                chart1.ChartAreas[0].AxisY.Title = "I förhållande till riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = studHST_ratio_dict(0, minyear, maxyear);
            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                Dictionary<int, double> ratiodict = studHST_ratio_dict(unidict[uniname], minyear, maxyear);
                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    yearreference = ratiodict[minyear];
                }


                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = ratiodict[year];
                    if (CB_HSTpeng.Checked)
                        amount = amount * 1000; //per Mkr istf per kkr
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (amount > ssmax)
                        ssmax = amount;
                    if (amount < ssmin)
                        ssmin = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            chart1.ChartAreas[0].AxisY.Maximum = roundaxis(1.1 * ssmax);
            if (CB_axiszero.Checked || ssmin >= ssmax)
                chart1.ChartAreas[0].AxisY.Minimum = 0;
            else
                chart1.ChartAreas[0].AxisY.Minimum = roundaxis(ssmin * 0.8);
            this.Cursor = Cursors.Default;

        }



        public void student_teacher_ratio()
        {
            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_staff select c.Year).Min();
            int maxyear = (from c in db.OV_staff select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_registered" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = fill_incomedict();

            //Dictionary<int, double> pengdict = fill_pengdict(0.8);

            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Studenter per anställd";
            double ssmax = 0;
            double ssmin = 1e20;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Studenter per utbildningsanställd";
            else
                chart1.Titles["Title1"].Text = "Studenter per utbildningsanställd " + focusname;
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande riket";
                chart1.ChartAreas[0].AxisY.Title = "I förhållande till riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = student_teacher_ratio_dict(0, minyear, maxyear);
            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                Dictionary<int, double> ratiodict = student_teacher_ratio_dict(unidict[uniname], minyear, maxyear);
                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    yearreference = ratiodict[minyear];
                }


                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = ratiodict[year];
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (amount > ssmax)
                        ssmax = amount;
                    if (amount < ssmin)
                        ssmin = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            chart1.ChartAreas[0].AxisY.Maximum = roundaxis(1.1 * ssmax);
            if (CB_axiszero.Checked || ssmin >= ssmax)
                chart1.ChartAreas[0].AxisY.Minimum = 0;
            else
                chart1.ChartAreas[0].AxisY.Minimum = roundaxis(ssmin * 0.8);
            this.Cursor = Cursors.Default;

        }

        public void totalhst(int itype, int isource)
        {
            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_hsthpr select c.Year).Min();
            int maxyear = (from c in db.OV_hsthpr select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_hsthpr" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Antal HST";
            double ssmax = 0;

            string areaname = "totalt";
            if ( itype > 0)
            {
                areaname = (from c in db.OV_subjectarea where c.Id == itype select c.Name).FirstOrDefault();
            }

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "HST "+areaname+" utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " HST "+areaname;
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_hsthpr where c.Uni == 0 where c.Area == itype orderby c.Year select c);
                foreach (OV_hsthpr oi in qt)
                {
                    refdict.Add(oi.Year, oi.HST);
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    OV_hsthpr oi = (from c in db.OV_hsthpr where c.Uni == unidict[uniname] where c.Area == itype where c.Year == minyear select c).FirstOrDefault();
                    if (oi != null)
                    {
                        if (CB_fraction.Checked)
                            yearreference = 0.01 * (oi.HST / refdict[minyear]);
                        else
                            yearreference = 0.01 * oi.HST;
                    }
                }


                var q = (from c in db.OV_hsthpr where c.Uni == unidict[uniname] where c.Area == itype orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    foreach (OV_hsthpr oi in (from c in q where c.Year == year select c))
                    {
                        if (oi != null)
                        {
                            amount += oi.HST;
                        }
                    }
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;
        }

        public double get_income(int uni, int year, string purpose)
        {
            if (purpose == "research")
            {
                return get_income(uni, year, 2) + get_income(uni, year, 4);
            }
            else if (purpose == "teaching")
            {
                return get_income(uni, year, 6) + get_income(uni, year, 5);
            }
            else
                return get_income(uni, year, 0);
        }

        public double get_income(int uni, int year, int incometype)
        {
            var qmoney = from c in db.OV_University_Income
                         where c.Uni == uni
                         where c.Year == year
                         where c.Incometype == incometype
                         where c.Incomesource == 0
                         select c.Amount;
            if (qmoney.Count() > 0)
                return qmoney.Sum();
            else
                return 0;
        }

        public double get_staff(int uni, int year, string purpose)
        {
            var qsci = from c in db.OV_staff
                       where c.Uni == uni
                       where c.Year == year
                       
                       select c;
            //var q2;
            if (purpose == "research")
                qsci = from c in qsci
                     where c.OV_stafftype.Researcher == true
                     select c;
            else if ( purpose == "teaching")
                qsci = from c in qsci
                       where c.OV_stafftype.Teacher == true
                       select c;
            var q2 = from c in qsci select c.Number;
            if (q2.Count() > 0)
                return q2.Sum();
            else
                return 0;
        }

        public void setstartyear(int year)
        {
            if (year > 0)
            {
                parent.memo("Setting start year to " + year);
                CB_startyear.Text = year.ToString();
                parent.memo("Starting year set to: " + CB_startyear.SelectedItem);
            }
            else
            {
                parent.memo("Unsetting start year");
                CB_startyear.SelectedItem = null;

            }
        }

        public int getstartyear()
        {
            if (CB_startyear.SelectedItem != null)
            {
                return util.tryconvert(CB_startyear.SelectedItem.ToString());
            }
            else
                return -1;

        }

        public int getendyear()
        {
            if (CB_endyear.SelectedItem != null)
            {
                return util.tryconvert(CB_endyear.SelectedItem.ToString());
            }
            else
                return -1;

        }
        public void setendyear(int year)
        {
            if (year > 0)
            {
                parent.memo("Setting end year to " + year);
                CB_endyear.Text = year.ToString();
                parent.memo("Ending year set to: " + CB_startyear.SelectedItem);
            }
            else
            {
                parent.memo("Unsetting end year");
                CB_endyear.SelectedItem = null;

            }
        }

        //Dictionary<string, string> pubtypedict = new Dictionary<string, string>() 
        //    { 
        //        {"kap","Kapitel i bok"},
        //        {"bok","Bok"},
        //        {"kfu","Konstnärlig"},
        //        {"lic","Lic-avhandling"},
        //        {"rap","Rapport"},
        //        {"art","Artikel"},
        //        {"pat","Patent"},
        //        {"kon","Konferensbidrag"},
        //        {"sam","Samlingsverk (red.)"},
        //        {"dok","Doktorsavhandling"},
        //        {"for","Forskningsöversikt"},
        //        {"pro","Proceedings (red.)"},
        //        {"rec","Recension"},
        //        {"ovr","Övrigt"}
         //   };

        int UKAyear = 2012;
        
        private int get_pubnumber(OV_publication op)
        {
            if (op.NumberUKA != null)
                return (int)op.NumberUKA;
            else if (op.Year < UKAyear)
                return op.NumberSwepub;
            else
                return 0;
        }
            
        private void publications_stackedarea()
        {
            this.Cursor = Cursors.WaitCursor;

            int minyear = (from c in db.OV_publication select c.Year).Min();
            int maxyear = (from c in db.OV_publication select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_publication" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }
            parent.memo("minyear, maxyear = " + minyear + ", " + maxyear);
            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            double ssmax = 0;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();

            Dictionary<int, double> pengdict = new Dictionary<int, double>();

            Dictionary<string, int> pubdict = new Dictionary<string, int>();

            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Antal publikationer per år";

            // Set the text of the title
            chart1.Titles["Title1"].Text = focusname + " publikationer";
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            //var qtype = from c in db.OV_examtype where c.Grp == examgroupdict[selitem].ToString() select c;
            //List<string> pubtype = (from c in db.OV_publication select c.Pubtype).Distinct().ToList();
            List<Series> ls = new List<Series>();
            //double prestation = 0.8;
            //foreach (string s in pubtypedict.Keys)
            foreach (string s in pubtypelistUKA)
            {
                Series sss = new Series(s);
                //sss.Label = s;
                ls.Add(sss);
                //examdict.Add(oi.Name, oi.Id);
            }

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_publication where c.Uni == 0 where c.Year >= minyear where c.Year <= maxyear select c);
                for (int i=minyear; i <= maxyear; i++)
                {
                    refdict.Add(i, 0);
                }
                foreach (OV_publication op in qt)
                    refdict[op.Year] += get_pubnumber(op);
            }

            double yearreference = 1;
            //if (CB_refyear.Checked)
            //{
            //    OV_exam oi = (from c in db.OV_exam
            //                  where c.Uni == focusuniversity
            //                  where c.Examtype1 == 0
            //                  where c.Gender == 0
            //                  where c.Age == 0
            //                  where c.Year == minyear
            //                  select c).FirstOrDefault();
            //    if (oi != null)
            //    {
            //        if (CB_fraction.Checked)
            //            yearreference = 0.01 * (oi.Number / refdict[minyear]);
            //        else
            //            yearreference = 0.01 * oi.Number;
            //    }
            //}

            Dictionary<int, double> ssmaxdict = new Dictionary<int, double>();
            for (int year = minyear; year <= maxyear; year++)
                ssmaxdict.Add(year, 0);

            var qall = (from c in db.OV_publication
                        where c.Uni == focusuniversity
                        where c.Subject == 0
                        //where c.Pubtype == ss.Name
                        orderby c.Year
                        select c);

            foreach (Series ss in ls)
            {
                ss.ChartType = SeriesChartType.StackedArea;
                IEnumerable<OV_publication> q;
                
                //q = (from c in qall
                //         where c.Pubtype == ss.Name
                //         select c);
                //ss.Label = pubtypedict[ss.Name]; //fulhack att ändra namnet efter q-villkoret
                //ss.Name = pubtypedict[ss.Name]; //fulhack att ändra namnet efter q-villkoret
                //var qtot = null;
                //if (CB_fraction.Checked)
                //    qtot = (from c in db.OV_hsthpr where c.Uni == 0 where c.Incometype == 0 where c.Incomesource == incomedict[ss.Name] orderby c.Year select c);
                for (int year = minyear; year <= maxyear; year++)
                //foreach (OV_hsthpr oi in q)
                {
                    //OV_hsthpr oi = (from c in q where c.Year == year select c).FirstOrDefault();
                    //double amount = 0;
                    //if (oi != null)
                    //    amount = oi.Amount;
                    var qoi = (from c in qall where c.Year == year select c);
                    if (year < UKAyear)
                    {
                        qoi = from c in qoi where pubsweUKAdict[ss.Name].Contains(c.Pubtype) select c;
                    }
                    else
                    {
                        qoi = from c in qoi where c.Pubtype== ss.Name select c;
                    }
                    double amount = 0;
                    foreach (OV_publication oi in qoi)
                        amount += get_pubnumber(oi);

                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                    {
                        amount = amount / refdict[year];
                        if (!CB_refyear.Checked)
                            amount *= 100;
                    }

                    ss.Points.AddXY(year, amount);

                    ssmaxdict[year] += amount;

                    //if (amount > ssmax)
                    //    ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(ss.Name + "\t" + year + "\t" + amount);
                }

                //ss.Name = pubtypedict[ss.Name]; //fulhack att ändra namnet efter q-villkoret
                chart1.Series.Add(ss);
            }

            for (int year = minyear; year <= maxyear; year++)
                if (ssmaxdict[year] > ssmax)
                    ssmax = ssmaxdict[year];

            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;

        }



        public void totalpub()
        {
            totalpub("");
        }



        public void totalpub(string pubtype)
        {
            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_publication select c.Year).Min();
            int maxyear = (from c in db.OV_publication select c.Year).Max();
            
            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_publication" }, true);


            if ( RB_permoney.Checked)
            {
                int minyearmoney = (from c in db.OV_University_Income select c.Year).Min();
                int maxyearmoney = (from c in db.OV_University_Income select c.Year).Max();
                minyear = Math.Max(minyear, minyearmoney);
                maxyear = Math.Min(maxyear, maxyearmoney);
            }
            else if (RB_perscientist.Checked)
            {
                int minyearsci = (from c in db.OV_staff select c.Year).Min();
                int maxyearsci = (from c in db.OV_staff select c.Year).Max();
                minyear = Math.Max(minyear, minyearsci);
                maxyear = Math.Min(maxyear, maxyearsci);
            }
            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Antal publikationer";

            if (RB_permoney.Checked)
            {
                if (!CB_reverse.Checked)
                {
                    chart1.ChartAreas[0].AxisY.Title = "Antal publikationer per Mkr";
                }
                else
                {
                    chart1.ChartAreas[0].AxisY.Title = "Mkr per publikation";
                }
            }
            else if (RB_perscientist.Checked)
            {
                if (!CB_reverse.Checked)
                {
                    chart1.ChartAreas[0].AxisY.Title = "Antal publikationer per forskare";
                }
                else
                {
                    chart1.ChartAreas[0].AxisY.Title = "Forskare per publikation";
                }
            }

            double ssmax = 0;

            string areaname = "totalt";
            if (!String.IsNullOrEmpty(pubtype))
            {
                areaname = pubtype;
            }

            string priceindex = get_priceindex();

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Publikationer " + areaname + " utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " publikationer " + areaname;
            if (RB_permoney.Checked)
            {
                if (CB_reverse.Checked)
                    chart1.Titles["Title1"].Text = "Mkr forskningsmedel " + priceindex + " per " + chart1.Titles["Title1"].Text;
                else
                    chart1.Titles["Title1"].Text += " per Mkr forskningsmedel " + priceindex;
            }
            else if (RB_perscientist.Checked)
            {
                if (CB_reverse.Checked)
                    chart1.Titles["Title1"].Text = "Forskare per " + chart1.Titles["Title1"].Text;
                else
                    chart1.Titles["Title1"].Text += " per forskare " + priceindex;
            }
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";


            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            double moneyunit = 1000;

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                for (int i = minyear; i <= maxyear; i++)
                    refdict.Add(i, 0);
                var qt = from c in db.OV_publication 
                         where c.Year >= minyear
                         where c.Year <= maxyear
                         where c.Subject == 0
                         select c;
                if (!String.IsNullOrEmpty(pubtype))
                    qt = from c in qt where c.Pubtype == pubtype select c;
                else
                    qt = from c in qt where (c.Pubtype == "Total") select c;
                foreach (OV_publication oi in qt)
                {
                    refdict[oi.Year] += get_pubnumber(oi);
                }

                if ( RB_permoney.Checked)
                {
                    var qmoney = from c in db.OV_University_Income 
                                 where c.Uni == 0 where ((c.Incometype == 2) || (c.Incometype == 4)) 
                                 select c;
                    for (int i = minyear; i <= maxyear; i++)
                    {
                        double amount = (from c in qmoney where c.Year == i select c.Amount).Sum() * adjustprice(i, priceindex);
                        refdict[i] = moneyunit*refdict[i] / amount;
                    }

                }
                else if (RB_perscientist.Checked)
                {
                    var qsci = from c in db.OV_staff
                                 where c.Uni == 0
                                 where c.OV_stafftype.Researcher == true
                                 select c;
                    for (int i = minyear; i <= maxyear; i++)
                    {
                        double amount = (from c in qsci where c.Year == i select c.Number).Sum();
                        refdict[i] = refdict[i] / amount;
                    }

                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;
                double yearreference = 1;
                //if (CB_refyear.Checked)
                //{
                //    var qref = from c in db.OV_publication where c.Uni == unidict[uniname] where c.Year == minyear select c;
                //    if (!String.IsNullOrEmpty(pubtype))
                //        qref = from c in qref where c.Pubtype == pubtype select c;

                //    if ( qref.Count() > 0)
                //    {
                //        int nsum = (from c in qref select c.Number).Sum();
                //        if (CB_fraction.Checked)
                //            yearreference = 0.01 * (nsum / refdict[minyear]);
                //        else
                //            yearreference = 0.01 * nsum;
                //    }
                //}


                var q = (from c in db.OV_publication where c.Uni == unidict[uniname] where c.Subject == 0 select c);
                if (!String.IsNullOrEmpty(pubtype))
                    q = from c in q where c.Pubtype == pubtype select c;
                else
                    q = from c in q where ((c.Pubtype == "Total")) select c;

                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    foreach (OV_publication oi in (from c in q where c.Year == year select c))
                    {
                        if (oi != null)
                        {
                            amount += get_pubnumber(oi);
                        }
                    }
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];

                    if (RB_permoney.Checked)
                    {
                        double money = get_income(unidict[uniname],year,"research") * adjustprice(year, priceindex); 
                        parent.memo("amount, money = " +amount+", "+ money);
                        if ( money > 0)
                            amount = moneyunit*amount / money;
                        if (CB_reverse.Checked && amount > 0)
                            amount = 1 / amount;
                    }
                    else if (RB_perscientist.Checked)
                    {
                        double sci = get_staff(unidict[uniname],year,"research");
                        parent.memo("amount, sci = " + amount + ", " + sci);
                        if (sci > 0)
                            amount = amount / sci;
                        if (CB_reverse.Checked && amount > 0)
                            amount = 1 / amount;
                    }

                    ss.Points.AddXY(year, amount);

                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;
        }
        
        public void sickleave()
        {
            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_sjuk where c.Number > 0 select c.Year).Min();
            int maxyear = (from c in db.OV_sjuk where c.Number > 0 select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_sjuk" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Sjukfrånvaro (%)";

            double ssmax = 0;
            int totalid = 14;

            string areaname = "totalt";

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Sjukfrånvaro " + areaname + " utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " sjukfrånvaro " + areaname;
            if (CB_fraction.Checked)
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            //Dictionary<int, double> totaldict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                chart1.ChartAreas[0].AxisY.Title = "Sjukfrånvaro i förhållande till rikssnittet";
                for (int i = minyear; i <= maxyear; i++)
                {
                    refdict.Add(i, (from c in db.OV_sjuk where c.Year == i select (double)c.Number).Average());
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;
                double yearreference = 1;
                if (CB_refyear.Checked)
                    yearreference = (double)(from c in db.OV_sjuk where c.Uni == unidict[uniname] where c.Year == minyear select c.Number).FirstOrDefault();


                var q = from c in db.OV_sjuk
                        where c.Uni == unidict[uniname]
                        select c;
                if (unidict[uniname] == 0)
                {
                    q = from c in db.OV_sjuk
                        select c;

                }

                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    double total = 0;
                    int n = 0;
                    foreach (OV_sjuk oi in (from c in q
                                                     where c.Year == year
                                                     select c))
                    {
                        if (oi != null)
                        {
                            amount += (float)oi.Number;
                            n++;
                        }
                    }
                    if (n > 0)
                        amount = amount / n;
                    if (total > 0)
                        amount = (amount / total) / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }
            parent.memo("ssmax = " + ssmax);

            if (ssmax == 0)
                ssmax = 1;
            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;

        }

        public double medianfromincomeclasses(int[] numbers)
        { 
            //Assume valid incomes in numbers[2]..[12], with [2] = 0..100 etc. and total in numbers[13]

            double binwidth = 100;
            double half = 0.5*(numbers[13]-numbers[1]);
            int bin = 1;
            int sum = 0;
            while ( sum <= half)
            {
                bin++;
                sum += numbers[bin];
                if (bin >= 13) //not valid data in numbers
                    return 0;
            }

            double x = binwidth * (bin - 1); //upper edge of final bin
            x -= (sum - half)/numbers[bin] * binwidth;
            return x;
        }

        public void medianincome()
        {

            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_income where c.Number > 0 select c.Year).Min();
            int maxyear = (from c in db.OV_income where c.Number > 0 select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_income" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Medianinkomst (tusental kr/år)";

            double ssmax = 0;
            int totalid = 14;

            string areaname = "totalt";

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Medianinkomst alumner år 3 utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " medianinkomst";
            if (CB_fraction.Checked)
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            Dictionary<int, double> totaldict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                for (int i = minyear; i <= maxyear; i++)
                {
                    int[] numbers = new int[14];
                    for (int j = 2; j < 14;j++ )
                    {
                        var qq = from c in db.OV_income
                                      where c.Year == i
                                      where c.Income3y == j
                                      select c.Number;
                        if (qq.Count() > 0)
                            numbers[j] = qq.Sum();
                        else
                            numbers[j] = 99999;
                    }
                    refdict.Add(i, medianfromincomeclasses(numbers));
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;
                double yearreference = 1;
                //if (CB_refyear.Checked)
                //{
                //    int refyear = minyear;
                //    int nsum = 0;
                //    do
                //    {
                //        var qref = from c in db.OV_income
                //                   where c.Uni == unidict[uniname]
                //                   where c.Year == refyear
                //                   select c;

                //        if (qref.Count() > 0)
                //        {
                //            nsum = 0;
                //            var qnsum = from c in qref where etypes.Contains(c.OV_incometype.Id) select c.Number;
                //            if (qnsum.Count() > 0)
                //                nsum = qnsum.Sum();
                //            double ntotal = (from c in qref where c.Etype3y == totalid select c.Number).Sum();
                //            if ((ntotal > 0) && (nsum > 0))
                //            {
                //                if (CB_fraction.Checked)
                //                    yearreference = 0.01 * ((nsum / ntotal) / refdict[minyear]);
                //                else
                //                    yearreference = 0.01 * nsum / ntotal;
                //            }
                //        }
                //        //parent.memo("yearreference = " + yearreference);
                //        refyear++;
                //    }
                //    while (nsum <= 0);
                //}


                var q = from c in db.OV_income
                        where c.Uni == unidict[uniname]
                        select c;

                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    
                    int[] numbers = new int[14]{0,0,0,0,0,0,0,0,0,0,0,0,0,0};

                    foreach (OV_income oi in (from c in q
                                                     where c.Year == year
                                                     select c))
                    {
                        if (oi != null)
                        {
                            numbers[oi.Income3y] = oi.Number;
                        }
                    }
                    amount = medianfromincomeclasses(numbers);
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }
            parent.memo("ssmax = " + ssmax);

            if (ssmax == 0)
                ssmax = 1;
            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;

        }

        public void totalestablished()
        {
            List<int> etypes = (from c in db.OV_establishmenttype where c.Level == 3 select c.Id).ToList();
            totalestablished(etypes, "Andel etablerade ");
        }

        public void totalestablished(List<int> etypes, string ename)
        {
            string sss = "etypes: ";
            foreach (int i in etypes)
                sss += " " + i;
            parent.memo(sss);
            
            this.Cursor = Cursors.WaitCursor;
            int minyear = (from c in db.OV_establishment where c.Number>0 select c.Year).Min();
            int maxyear = (from c in db.OV_establishment where c.Number>0 select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_establishment" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Andel av alumni";

            double ssmax = 0;
            int totalid = 14; 

            string areaname = "totalt";

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = ename + areaname + " utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + ename + areaname;
            if (CB_fraction.Checked)
                chart1.Titles["Title1"].Text += ", i förhållande till rikssnittet";
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            Dictionary<int, double> totaldict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                for (int i = minyear; i <= maxyear; i++)
                {
                    refdict.Add(i, 0);
                    totaldict.Add(i, 0);
                }
                var qt = (from c in db.OV_establishment select c);
                foreach (OV_establishment oi in qt)
                {
                    if ( oi.Etype3y == totalid)
                        totaldict[oi.Year] += oi.Number;
                    else if ( etypes.Contains(oi.OV_establishmenttype.Id))
                        refdict[oi.Year] += oi.Number;
                }
                for (int year = minyear; year <= maxyear; year++)
                {
                    if ( totaldict[year] > 0)
                        refdict[year] = refdict[year] / totaldict[year];
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();


            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;
                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    int refyear = minyear;
                    int nsum = 0;
                    do
                    {
                        var qref = from c in db.OV_establishment
                                   where c.Uni == unidict[uniname]
                                   where c.Year == refyear
                                   select c;

                        if (qref.Count() > 0)
                        {
                            nsum = 0;
                            var qnsum = from c in qref where etypes.Contains(c.OV_establishmenttype.Id) select c.Number;
                            if (qnsum.Count() > 0)
                                nsum = qnsum.Sum();
                            double ntotal = (from c in qref where c.Etype3y == totalid select c.Number).Sum();
                            if ((ntotal > 0) && (nsum > 0))
                            {
                                if (CB_fraction.Checked)
                                    yearreference = 0.01 * ((nsum / ntotal) / refdict[minyear]);
                                else
                                    yearreference = 0.01 * nsum / ntotal;
                            }
                        }
                        //parent.memo("yearreference = " + yearreference);
                        refyear++;
                    }
                    while (nsum <= 0);
                }


                var q = from c in db.OV_establishment 
                        where c.Uni == unidict[uniname] select c;

                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    double total = 0;
                    foreach (OV_establishment oi in (from c in q 
                                                     where c.Year == year select c))
                    {
                        if (oi != null)
                        {
                            if (oi.Etype3y == totalid)
                                total += oi.Number;
                            else if (etypes.Contains(oi.OV_establishmenttype.Id))
                                amount += oi.Number;
                        }
                    }
                    if (total > 0)
                        amount = (amount/total) / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                }
                ss.ChartType = SeriesChartType.Line;
                chart1.Series.Add(ss);
            }
            parent.memo("ssmax = " + ssmax);

            if (ssmax == 0)
                ssmax = 1;
            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);
            }
            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;
        }

        public void fill_pricedict()
        {
            if (pricedict.Count > 0)
                return;

            foreach (OV_price op in (from c in db.OV_price select c))
            {
                Dictionary<string, float> dd = new Dictionary<string, float>();
                dd.Add("Löpande priser", 1);
                dd.Add("AKI", (float)op.AKI);
                dd.Add("PLO", (float)op.PLO);
                dd.Add("Prodavdrag", (float)op.Prodavdrag);
                dd.Add("KPI", (float)op.KPI);
                pricedict.Add(op.Year, dd);
            }
        }

        public string get_priceindex()
        {
            fill_pricedict();
            string priceindex = "Löpande priser";
            if (RB_salaryindex.Checked)
                priceindex = "AKI";
            if (RB_PLO.Checked)
                priceindex = "PLO";
            if (RB_KPI.Checked)
                priceindex = "KPI";
            return priceindex;
        }

        public double adjustprice(int year, string priceindex)
        {
            if (!pricedict.ContainsKey(year))
                return 1;
            if (!pricedict[year].ContainsKey(priceindex))
                return 1;

            return pricedict[year][priceindex];
        }

        public void totalincome()
        {
            totalincome(0, 0);
        }
        
        public void totalincome(int itype, int isource)
        {
            this.Cursor = Cursors.WaitCursor;

            int minyear = (from c in db.OV_University_Income select c.Year).Min();
            int maxyear = (from c in db.OV_University_Income select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_University_Income" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            string priceindex = get_priceindex();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "1000-tal kronor, "+priceindex;
            double ssmax = 0;


            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Totala intäkter utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " totala intäkter";

            if (itype != 0)
                chart1.Titles["Title1"].Text += (from c in db.OV_Incometype where c.Id == itype select c.Name).FirstOrDefault();
            if (isource != 0)
                chart1.Titles["Title1"].Text += (from c in db.OV_Incomesource where c.Id == isource select c.Name).FirstOrDefault();
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            else if (CB_demography.Checked)
            {
                chart1.Titles["Title1"].Text += ", per capita";
                chart1.ChartAreas[0].AxisY.Title += " per capita";

            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_University_Income where c.Uni == 0 where c.Incometype == itype where c.Incomesource == isource orderby c.Year select c);
                foreach (OV_University_Income oi in qt)
                {
                    refdict.Add(oi.Year, oi.Amount*adjustprice(oi.Year,priceindex));
                }
                
                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }
            else if (CB_demography.Checked)
            {
                var qt = from c in db.OV_demography
                         where c.Lan == 0
                         orderby c.Year
                         select c;
                foreach (OV_demography od in qt)
                {
                    refdict.Add(od.Year, od.Number);
                }

            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();
            StringBuilder sb = new StringBuilder();

            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if ( uniname == focusname && !CB_sumuni.Checked)
                     ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    OV_University_Income oi = (from c in db.OV_University_Income where c.Uni == unidict[uniname] where c.Incometype == itype where c.Incomesource == isource where c.Year == minyear select c).FirstOrDefault();
                    if (oi != null)
                    {
                        if (CB_fraction.Checked)
                            yearreference = 0.01 * (oi.Amount * adjustprice(oi.Year, priceindex) / refdict[minyear]);
                        else
                            yearreference = 0.01 * oi.Amount * adjustprice(oi.Year, priceindex);
                        
                    }
                }


                var q = (from c in db.OV_University_Income where c.Uni == unidict[uniname] where c.Incometype == itype where c.Incomesource == isource orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    foreach (OV_University_Income oi in (from c in q where c.Year == year select c))
                    {
                        if (oi != null)
                        {
                            amount += oi.Amount;
                        }
                    }
                    amount = amount * adjustprice(year, priceindex) / yearreference;
                    if (CB_fraction.Checked || CB_demography.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                    if (CB_values_to_file.Checked)
                        sb.Append(ss.Name + "\t" + year + "\t" + amount + "\n");

                    
                }
                ss.ChartType = SeriesChartType.Line;
                
                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);

            }

            //if (CB_values_to_file.Checked)
            //{
            //    SaveValuesToFile(sb, chart1.Titles["Title1"].Text);
            //}


            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;
        }


        public void totalfinance(int ipost, int iact)
        {
            totalfinance(new List<int>() { ipost }, new List<int>() { iact });
        }

        public void totalfinance(List<int> postlist, List<int> actlist)
        { 
            this.Cursor = Cursors.WaitCursor;

            int minyear = (from c in db.OV_finance select c.Year).Min();
            int maxyear = (from c in db.OV_finance select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_finance" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }
            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            string priceindex = get_priceindex();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "1000-tal kronor, " + priceindex;
            double ssmax = 0;
            double ssmin = 0;

            string postname = (from c in db.OV_financepost where c.Id == postlist[0] select c.Name).First();
            string actname = (from c in db.OV_financeverksamhet where c.Id == actlist[0] select c.Name).First();

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = postname+", "+actname +" utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " " + postname + ", "+actname;

            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            else if (CB_demography.Checked)
            {
                chart1.Titles["Title1"].Text += ", per capita";
                chart1.ChartAreas[0].AxisY.Title += " per capita";

            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                foreach (int ipost in postlist)
                    foreach (int iact in actlist)
                    {
                        var qt = (from c in db.OV_finance where c.Uni == 0 where c.Post == ipost where c.Verksamhet == iact orderby c.Year select c);
                        foreach (OV_finance oi in qt)
                        {
                            if (!refdict.ContainsKey(oi.Year))
                                refdict.Add(oi.Year, oi.Amount * adjustprice(oi.Year, priceindex));
                            else
                                refdict[oi.Year] += oi.Amount * adjustprice(oi.Year, priceindex);
                        }
                    }
                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }
            else if (CB_demography.Checked)
            {
                var qt = from c in db.OV_demography
                         where c.Lan == 0
                         orderby c.Year
                         select c;
                foreach (OV_demography od in qt)
                {
                    refdict.Add(od.Year, od.Number);
                }

            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();
            StringBuilder sb = new StringBuilder();

            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    double oisum = 0;

                    foreach (int ipost in postlist)
                        foreach (int iact in actlist)
                        {
                            OV_finance oi = (from c in db.OV_finance
                                             where c.Uni == unidict[uniname]
                                             where c.Post == ipost
                                             where c.Verksamhet == iact
                                             where c.Year == minyear
                                             select c).FirstOrDefault();
                            if (oi != null)
                                oisum += oi.Amount;
                        }
                    //if (oisum > 0)
                    {
                        if (CB_fraction.Checked)
                            yearreference = 0.01 * (oisum * adjustprice(minyear, priceindex) / refdict[minyear]);
                        else
                            yearreference = 0.01 * oisum * adjustprice(minyear, priceindex);

                    }
                }


                var q = (from c in db.OV_finance 
                         where c.Uni == unidict[uniname] 
                         //where c.Post == ipost 
                         //where c.Verksamhet == iact 
                         //orderby c.Year 
                         select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    foreach (int ipost in postlist)
                        foreach (int iact in actlist)
                        {
                            OV_finance oi = (from c in q
                                             where c.Post == ipost
                                             where c.Verksamhet == iact
                                             where c.Year == year
                                             select c).FirstOrDefault();
                            if (oi != null)
                            {
                                amount += oi.Amount;
                            }
                        }
                    amount = amount * adjustprice(year, priceindex) / yearreference;
                    if (CB_fraction.Checked || CB_demography.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (amount < ssmin)
                        ssmin = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                    if (CB_values_to_file.Checked)
                        sb.Append(ss.Name + "\t" + year + "\t" + amount + "\n");


                }
                ss.ChartType = SeriesChartType.Line;

                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);

            }

            //if (CB_values_to_file.Checked)
            //{
            //    SaveValuesToFile(sb, chart1.Titles["Title1"].Text);
            //}

            if (ssmax == ssmin)
                return;

            if (ssmin < 0)
            {
                chart1.ChartAreas[0].AxisY.Maximum = roundaxis(ssmax);
                chart1.ChartAreas[0].AxisY.Minimum = -roundaxis(Math.Abs(ssmin));
            }
            else
            {
                chart1.ChartAreas[0].AxisY.Maximum = roundaxis(ssmax);
                chart1.ChartAreas[0].AxisY.Minimum = 0;
            }


            this.Cursor = Cursors.Default;
        }

        public double get_externalfraction(int year, int uni, int itype)
        {
            var qtot = from c in db.OV_University_Income 
                    where c.Uni == uni 
                    where c.Incometype == itype 
                    where c.Incomesource == 0 
                    where c.Year == year
                    select c.Amount;
            var qanslag = from c in db.OV_University_Income
                          where c.Uni == uni
                          where c.Incometype == itype
                          where c.Incomesource == 1
                          where c.Year == year
                          select c.Amount;
            double frac = 0;
            double tot = qtot.Sum();
            if ( tot > 0)
            {
                double ext = tot - qanslag.Sum();
                frac = ext / tot;
            }
            return frac;
        }

        public void externalincome()
        {
            this.Cursor = Cursors.WaitCursor;

            int minyear = (from c in db.OV_University_Income select c.Year).Min();
            int maxyear = (from c in db.OV_University_Income select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_University_Income" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            string priceindex = get_priceindex();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Andel externa medel";
            double ssmax = 0;



            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Andel externa intäkter utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " andel externa intäkter";

            int? itype = (from c in db.OV_Incometype where c.Name == (string)LB_incometype.SelectedItem select c.Id).FirstOrDefault();
            if (itype == null)
                itype = 0;

            if (itype != 0)
                chart1.Titles["Title1"].Text += ", "+(from c in db.OV_Incometype where c.Id == itype select c.Name).FirstOrDefault();
            //if (isource != 0)
            //    chart1.Titles["Title1"].Text += (from c in db.OV_Incomesource where c.Id == isource select c.Name).FirstOrDefault();
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", i förhållande till riket";
                chart1.ChartAreas[0].AxisY.Title = "I förhållande till riket";
            }
            else if (CB_demography.Checked)
            {
                chart1.Titles["Title1"].Text += ", per capita";
                chart1.ChartAreas[0].AxisY.Title += " per capita";

            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                //var qt = (from c in db.OV_University_Income where c.Uni == 0 where c.Incometype == itype where c.Incomesource == isource orderby c.Year select c);
                for (int year=minyear;year <= maxyear;year++)
                {
                    refdict.Add(year, get_externalfraction(year,0,(int)itype));
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            //Series sumseries = getsumseries(unilist);
            //Dictionary<int, double> sumdict = new Dictionary<int, double>();

            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    double refamount = get_externalfraction(minyear, unidict[uniname], (int)itype);
                    //OV_University_Income oi = (from c in db.OV_University_Income where c.Uni == unidict[uniname] where c.Incometype == itype where c.Incomesource == isource where c.Year == minyear select c).FirstOrDefault();
                    if (refamount > 0)
                    {
                        if (CB_fraction.Checked)
                            yearreference = 0.01 * refamount / refdict[minyear];
                        else
                            yearreference = 0.01 * refamount;

                    }
                }


                //var q = (from c in db.OV_University_Income where c.Uni == unidict[uniname] where c.Incometype == itype where c.Incomesource == isource orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = get_externalfraction(year,unidict[uniname],(int)itype);
                    //foreach (OV_University_Income oi in (from c in q where c.Year == year select c))
                    //{
                    //    if (oi != null)
                    //    {
                    //        amount += oi.Amount;
                    //    }
                    //}
                    amount = amount  / yearreference;
                    if (CB_fraction.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);


                }
                ss.ChartType = SeriesChartType.Line;

                chart1.Series.Add(ss);
            }

            chart1.ChartAreas[0].AxisY.Maximum = roundaxis(ssmax);
            this.Cursor = Cursors.Default;
        }

        private double HST_to_money(double amount, int subjectarea, Dictionary<int, double> pengdict)
        {
            if (pengdict.ContainsKey(subjectarea))
                return amount * pengdict[subjectarea];
            else
                return amount * pengdict[2]; //HJS-peng
        }

        private Dictionary<int,double> fill_pengdict(double prestation)
        {
            Dictionary<int, double> pengdict = new Dictionary<int, double>();
            var qtype = from c in db.OV_subjectarea select c;
            foreach (OV_subjectarea oi in qtype)
            {
                if (oi.Name.Contains("Total"))
                    continue;
                

                pengdict.Add(oi.Id, (double)(oi.HSTpeng2018 + prestation * oi.HPRpeng2018));
            }


            return pengdict;
        }

        private Dictionary<int, OV_subjectarea> fill_pengdict2()
        {
            Dictionary<int, OV_subjectarea> pengdict = new Dictionary<int, OV_subjectarea>();
            var qtype = from c in db.OV_subjectarea select c;
            foreach (OV_subjectarea oi in qtype)
            {
                if (oi.Name.Contains("Total"))
                    continue;


                pengdict.Add(oi.Id, oi);
            }


            return pengdict;
        }

        private Dictionary<string, int> fill_incomedict()
        {
            Dictionary<string,int> incomedict = new Dictionary<string,int>();
            var qtype = from c in db.OV_subjectarea select c;
            foreach (OV_subjectarea oi in qtype)
            {
                if (oi.Name.Contains("Total"))
                    continue;

                incomedict.Add(oi.Name, oi.Id);
            }

            return incomedict;
        }

        private void hst_stackedarea()
        {
            int minyear = (from c in db.OV_hsthpr select c.Year).Min();
            int maxyear = (from c in db.OV_hsthpr select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_hsthpr" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }
            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            double ssmax = 0;

            Dictionary<string, int> incomedict = fill_incomedict();
            double prestation = 0.8;
            Dictionary<int, double> pengdict = fill_pengdict(prestation);

            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Antal HST";

            // Set the text of the title
            chart1.Titles["Title1"].Text = focusname + " HST per utbildningsområde";
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";
            if (CB_HSTpeng.Checked)
            {
                chart1.ChartAreas[0].AxisY.Title = "HST-HPR-ersättning (tkr)";
                chart1.Titles["Title1"].Text = "Utbildningsproduktion i 2018 års priser";
            }


            List<Series> ls = new List<Series>();
            var qtype = from c in db.OV_subjectarea select c;
            foreach (OV_subjectarea oi in qtype)
            {
                if (oi.Name.Contains("Total"))
                    continue;
                if (subjectsynonyms.ContainsKey(oi.Name))
                    continue;

                ls.Add(new Series(oi.Name));
            }

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_hsthpr where c.Uni == 0 where c.Area == 0 orderby c.Year select c);
                foreach (OV_hsthpr oi in qt)
                {
                    refdict.Add(oi.Year, oi.HST);
                }
            }

            double yearreference = 1;
            if (CB_refyear.Checked)
            {
                OV_hsthpr oi = (from c in db.OV_hsthpr where c.Uni == focusuniversity where c.Area == 0 where c.Year == minyear select c).FirstOrDefault();
                if (oi != null)
                {
                    if (CB_fraction.Checked)
                        yearreference = 0.01 * (oi.HST / refdict[minyear]);
                    else
                        yearreference = 0.01 * oi.HST;
                }
            }

            Dictionary<int, double> ssmaxdict = new Dictionary<int, double>();
            for (int year = minyear; year <= maxyear; year++)
                ssmaxdict.Add(year, 0);

            foreach (Series ss in ls)
            {
                double sssum = 0;
                //ss.ChartType = SeriesChartType.Line;
                ss.ChartType = SeriesChartType.StackedArea;
                var q = (from c in db.OV_hsthpr where c.Uni == focusuniversity where c.Area == incomedict[ss.Name] orderby c.Year select c);
                //var qtot = null;
                //if (CB_fraction.Checked)
                //    qtot = (from c in db.OV_hsthpr where c.Uni == 0 where c.Incometype == 0 where c.Incomesource == incomedict[ss.Name] orderby c.Year select c);
                for (int year = minyear; year <= maxyear; year++)
                //foreach (OV_hsthpr oi in q)
                {
                    //OV_hsthpr oi = (from c in q where c.Year == year select c).FirstOrDefault();
                    //double amount = 0;
                    //if (oi != null)
                    //    amount = oi.Amount;
                    var qoi = (from c in q where c.Year == year select c);
                    double amount = 0;
                    foreach (OV_hsthpr oi in qoi)
                        amount += oi.HST;

                    amount = amount / yearreference;
                    if (CB_HSTpeng.Checked)
                        amount = HST_to_money(amount, incomedict[ss.Name], pengdict);
                    if (CB_fraction.Checked)
                    {
                        amount = amount / refdict[year];
                        if (!CB_refyear.Checked)
                            amount *= 100;
                    }

                    ss.Points.AddXY(year, amount);

                    ssmaxdict[year] += amount;
                    sssum += amount;

                    //if (amount > ssmax)
                    //    ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(ss.Name + "\t" + year + "\t" + amount);
                }

                if (sssum > 0)
                    chart1.Series.Add(ss);
            }

            for (int year = minyear; year <= maxyear; year++)
                if (ssmaxdict[year] > ssmax)
                    ssmax = ssmaxdict[year];

            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;

        }


        private void incometype_stackedarea()
        {
            int minyear = (from c in db.OV_University_Income select c.Year).Min();
            int maxyear = (from c in db.OV_University_Income select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_University_Income" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            double ssmax = 0;

            string priceindex = get_priceindex();

            int? isource = (from c in db.OV_Incomesource where c.Name == (string)LB_incomesource.SelectedItem select c.Id).FirstOrDefault();
            if (isource == null)
                isource = 0;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "1000-tal kronor " + priceindex;

            // Set the text of the title
            chart1.Titles["Title1"].Text = focusname + " intäkter per intäktsslag";
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            if (isource > 0)
                chart1.Titles["Title1"].Text += ", " + (string)LB_incomesource.SelectedItem;

            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            var qtype = from c in db.OV_Incometype select c;
            List<Series> ls = new List<Series>();
            foreach (OV_Incometype oi in qtype)
            {
                if (oi.Name == "Total")
                    continue;

                ls.Add(new Series(oi.Name));
                incomedict.Add(oi.Name, oi.Id);
            }

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_University_Income where c.Uni == 0 where c.Incometype == 0 where c.Incomesource == isource orderby c.Year select c);
                foreach (OV_University_Income oi in qt)
                {
                    refdict.Add(oi.Year, oi.Amount*adjustprice(oi.Year,priceindex));
                }
            }

            double yearreference = 1;
            if (CB_refyear.Checked)
            {
                OV_University_Income oi = (from c in db.OV_University_Income where c.Uni == focusuniversity where c.Incometype == 0 where c.Incomesource == isource where c.Year == minyear select c).FirstOrDefault();
                if (oi != null)
                {
                    if (CB_fraction.Checked)
                        yearreference = 0.01 * (oi.Amount * adjustprice(oi.Year, priceindex) / refdict[minyear]);
                    else
                        yearreference = 0.01 * oi.Amount * adjustprice(oi.Year, priceindex);
                }
            }

            Dictionary<int, double> ssmaxdict = new Dictionary<int, double>();
            for (int year = minyear; year <= maxyear; year++)
                ssmaxdict.Add(year, 0);

            double maxamountsum = 0;
            foreach (Series ss in ls)
            {
                //ss.ChartType = SeriesChartType.Line;
                ss.ChartType = SeriesChartType.StackedArea;
                var q = (from c in db.OV_University_Income where c.Uni == focusuniversity where c.Incometype == incomedict[ss.Name] where c.Incomesource == isource orderby c.Year select c);
                //var qtot = null;
                //if (CB_fraction.Checked)
                //    qtot = (from c in db.OV_University_Income where c.Uni == 0 where c.Incometype == 0 where c.Incomesource == incomedict[ss.Name] orderby c.Year select c);
                double amountsum = 0;
                for (int year = minyear; year <= maxyear; year++)
                //foreach (OV_University_Income oi in q)
                {
                    //OV_University_Income oi = (from c in q where c.Year == year select c).FirstOrDefault();
                    //double amount = 0;
                    //if (oi != null)
                    //    amount = oi.Amount;
                    var qoi = (from c in q where c.Year == year select c);
                    double amount = 0;
                    foreach (OV_University_Income oi in qoi)
                        amount += oi.Amount;

                    amount = amount * adjustprice(year, priceindex);

                    if (CB_fraction.Checked)
                    {
                        amount = amount / refdict[year];
                        if (!CB_refyear.Checked)
                            amount *= 100;
                    }

                    ss.Points.AddXY(year, amount);

                    ssmaxdict[year] += amount;
                    amountsum += amount;

                    if (CB_memo.Checked)
                        parent.memo(ss.Name + "\t" + year + "\t" + amount);
                }

                if (amountsum > maxamountsum)
                    maxamountsum = amountsum;
                if (amountsum > 0.001 * maxamountsum)
                    chart1.Series.Add(ss);

            }

            for (int year = minyear; year <= maxyear; year++)
                if (ssmaxdict[year] > ssmax)
                    ssmax = ssmaxdict[year];

            chart1.ChartAreas[0].AxisY.Maximum = roundaxis(ssmax);

        }

        private void finance_stackedarea(int verksamhet)
        {
            int minyear = (from c in db.OV_finance select c.Year).Min();
            int maxyear = (from c in db.OV_finance select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_finance" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            double ssmax = 0;

            string priceindex = get_priceindex();

            int? isource = (from c in db.OV_Incomesource where c.Name == (string)LB_incomesource.SelectedItem select c.Id).FirstOrDefault();
            if (isource == null)
                isource = 0;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "1000-tal kronor " + priceindex;

            // Set the text of the title
            chart1.Titles["Title1"].Text = focusname + " intäkter per intäktsslag";
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            if (isource > 0)
                chart1.Titles["Title1"].Text += ", " + (string)LB_incomesource.SelectedItem;

            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            var qtype = from c in db.OV_Incometype select c;
            List<Series> ls = new List<Series>();
            foreach (OV_Incometype oi in qtype)
            {
                if (oi.Name == "Total")
                    continue;

                ls.Add(new Series(oi.Name));
                incomedict.Add(oi.Name, oi.Id);
            }

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_University_Income where c.Uni == 0 where c.Incometype == 0 where c.Incomesource == isource orderby c.Year select c);
                foreach (OV_University_Income oi in qt)
                {
                    refdict.Add(oi.Year, oi.Amount * adjustprice(oi.Year, priceindex));
                }
            }

            double yearreference = 1;
            if (CB_refyear.Checked)
            {
                OV_University_Income oi = (from c in db.OV_University_Income where c.Uni == focusuniversity where c.Incometype == 0 where c.Incomesource == isource where c.Year == minyear select c).FirstOrDefault();
                if (oi != null)
                {
                    if (CB_fraction.Checked)
                        yearreference = 0.01 * (oi.Amount * adjustprice(oi.Year, priceindex) / refdict[minyear]);
                    else
                        yearreference = 0.01 * oi.Amount * adjustprice(oi.Year, priceindex);
                }
            }

            Dictionary<int, double> ssmaxdict = new Dictionary<int, double>();
            for (int year = minyear; year <= maxyear; year++)
                ssmaxdict.Add(year, 0);

            double maxamountsum = 0;
            foreach (Series ss in ls)
            {
                //ss.ChartType = SeriesChartType.Line;
                ss.ChartType = SeriesChartType.StackedArea;
                var q = (from c in db.OV_University_Income where c.Uni == focusuniversity where c.Incometype == incomedict[ss.Name] where c.Incomesource == isource orderby c.Year select c);
                //var qtot = null;
                //if (CB_fraction.Checked)
                //    qtot = (from c in db.OV_University_Income where c.Uni == 0 where c.Incometype == 0 where c.Incomesource == incomedict[ss.Name] orderby c.Year select c);
                double amountsum = 0;
                for (int year = minyear; year <= maxyear; year++)
                //foreach (OV_University_Income oi in q)
                {
                    //OV_University_Income oi = (from c in q where c.Year == year select c).FirstOrDefault();
                    //double amount = 0;
                    //if (oi != null)
                    //    amount = oi.Amount;
                    var qoi = (from c in q where c.Year == year select c);
                    double amount = 0;
                    foreach (OV_University_Income oi in qoi)
                        amount += oi.Amount;

                    amount = amount * adjustprice(year, priceindex);

                    if (CB_fraction.Checked)
                    {
                        amount = amount / refdict[year];
                        if (!CB_refyear.Checked)
                            amount *= 100;
                    }

                    ss.Points.AddXY(year, amount);

                    ssmaxdict[year] += amount;
                    amountsum += amount;

                    if (CB_memo.Checked)
                        parent.memo(ss.Name + "\t" + year + "\t" + amount);
                }

                if (amountsum > maxamountsum)
                    maxamountsum = amountsum;
                if (amountsum > 0.001 * maxamountsum)
                    chart1.Series.Add(ss);

            }

            for (int year = minyear; year <= maxyear; year++)
                if (ssmaxdict[year] > ssmax)
                    ssmax = ssmaxdict[year];

            chart1.ChartAreas[0].AxisY.Maximum = roundaxis(ssmax);

        }

        private void incomesource_stackedarea()
        {
            int minyear = (from c in db.OV_University_Income select c.Year).Min();
            int maxyear = (from c in db.OV_University_Income select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_University_Income" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            int incometype = 0;
            string itypestring = "";
            int? itype = (from c in db.OV_Incometype where c.Name == (string)LB_incometype.SelectedItem select c.Id).FirstOrDefault();
            if (itype != null)
            {
                incometype = (int)itype;
                itypestring = "; " + (string)LB_incometype.SelectedItem;
            }


            double ssmax = 0;
            string priceindex = get_priceindex();

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "1000-tal kronor, "+priceindex;

            // Set the text of the title
            chart1.Titles["Title1"].Text = focusname + " intäkter per intäktskälla" + itypestring;
            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            var qtype = from c in db.OV_Incomesource select c;
            List<Series> ls = new List<Series>();
            foreach (OV_Incomesource oi in qtype)
            {
                if (oi.Name == "Total")
                    continue;

                ls.Add(new Series(oi.Name));
                incomedict.Add(oi.Name, oi.Id);
            }

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_University_Income where c.Uni == 0 where c.Incometype == incometype where c.Incomesource == 0 orderby c.Year select c);
                foreach (OV_University_Income oi in qt)
                {
                    refdict.Add(oi.Year, oi.Amount * adjustprice(oi.Year, priceindex));
                }
            }

            double yearreference = 1;
            if (CB_refyear.Checked)
            {
                OV_University_Income oi = (from c in db.OV_University_Income where c.Uni == focusuniversity where c.Incometype == incometype where c.Incomesource == 0 where c.Year == minyear select c).FirstOrDefault();
                if (oi != null)
                {
                    if (CB_fraction.Checked)
                        yearreference = 0.01 * (oi.Amount * adjustprice(oi.Year, priceindex) / refdict[minyear]);
                    else
                        yearreference = 0.01 * oi.Amount * adjustprice(oi.Year, priceindex);
                }
            }

            Dictionary<int, double> ssmaxdict = new Dictionary<int, double>();
            for (int year = minyear; year <= maxyear; year++)
                ssmaxdict.Add(year, 0);

            double maxamountsum = 0;
            foreach (Series ss in ls)
            {
                //ss.ChartType = SeriesChartType.Line;
                ss.ChartType = SeriesChartType.StackedArea;
                var q = (from c in db.OV_University_Income where c.Uni == focusuniversity where c.Incomesource == incomedict[ss.Name] where c.Incometype == incometype orderby c.Year select c);
                //var qtot = null;
                //if (CB_fraction.Checked)
                //    qtot = (from c in db.OV_University_Income where c.Uni == 0 where c.Incometype == 0 where c.Incomesource == incomedict[ss.Name] orderby c.Year select c);
                double amountsum = 0;
                for (int year = minyear; year <= maxyear; year++)
                //foreach (OV_University_Income oi in q)
                {
                    //OV_University_Income oi = (from c in q where c.Year == year select c).FirstOrDefault();
                    var qoi = (from c in q where c.Year == year select c);
                    double amount = 0;
                    foreach (OV_University_Income oi in qoi)
                        amount += oi.Amount;

                    amount = amount * adjustprice(year, priceindex);
                    amount = amount / yearreference;
                    if (CB_fraction.Checked)
                    {
                        amount = amount / refdict[year];
                        if (!CB_refyear.Checked)
                            amount *= 100;
                    }

                    ss.Points.AddXY(year, amount);
                    ssmaxdict[year] += amount;
                    amountsum += amount;
                    if (CB_memo.Checked)
                        parent.memo(ss.Name + "\t" + year + "\t" + amount);
                }

                if (amountsum > maxamountsum)
                    maxamountsum = amountsum;
                if (amountsum > 0.001 * maxamountsum)
                    chart1.Series.Add(ss);
            }

            for (int year = minyear; year <= maxyear; year++)
                if (ssmaxdict[year] > ssmax)
                    ssmax = ssmaxdict[year];

            chart1.ChartAreas[0].AxisY.Maximum = roundaxis(ssmax);

        }

        private void RB_IncomeType_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_IncomeType.Checked)
                updatechart(sender, e);

        }

        private void LB_uni_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (string s in LB_uni.SelectedItems)
            {
                if ( s != focusname)
                {
                    focusuniversity = unidict[s];
                    focusname = s;
                    focuslabel.Text = focusname;
                    updatechart(sender,e);
                }
            }
        }

        public void checkuni(string uniname)
        {
            int i = LB_uni.Items.IndexOf(uniname);
            LB_uni.SetItemCheckState(i, CheckState.Checked);
        }

        public bool setfocusuni(string uniname)
        {
            if (unidict.ContainsKey(uniname))
            {
                focusuniversity = unidict[uniname];
                focusname = uniname;
                focuslabel.Text = focusname;
                checkuni(uniname);
                return true;
            }
            else
                return false;
        }

        private void RB_TotalIncome_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_TotalIncome.Checked)
                updatechart(sender, e);

        }

        private void select_typesource()
        {
            if (LB_incometype.SelectedItems.Count == 1)
                LB_incometype_SelectedIndexChanged(null, null);
            else if (LB_incomesource.SelectedItems.Count == 1)
                LB_incomesource_SelectedIndexChanged(null, null);
        }

        private void RB_source_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_IncomeSource.Checked)
                updatechart(sender, e);

        }

        private void LB_incometype_SelectedIndexChanged(object sender, EventArgs e)
        {
            int? itype = (from c in db.OV_Incometype where c.Name == (string)LB_incometype.SelectedItem select c.Id).FirstOrDefault();
            int? isource = (from c in db.OV_Incomesource where c.Name == (string)LB_incomesource.SelectedItem select c.Id).FirstOrDefault();
            if (isource == null)
                isource = 0;
            if (itype > 0)
                totalincome((int)itype, (int)isource);
            RB_typesource.Checked = true;
        }

        private void LB_incomesource_SelectedIndexChanged(object sender, EventArgs e)
        {
            int? isource = (from c in db.OV_Incomesource where c.Name == (string)LB_incomesource.SelectedItem select c.Id).FirstOrDefault();
            int? itype = (from c in db.OV_Incometype where c.Name == (string)LB_incometype.SelectedItem select c.Id).FirstOrDefault();
            if (itype == null)
                itype = 0;
            if (isource > 0)
                totalincome((int)itype,(int)isource);
            RB_typesource.Checked = true;
        }

        private void staff_stackedarea()
        {
            int minyear = (from c in db.OV_staff select c.Year).Min();
            int maxyear = (from c in db.OV_staff select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_staff" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            double ssmax = 0;

            Dictionary<string, int> staffdict = new Dictionary<string, int>();
            Dictionary<string, string> staffaliasdict = new Dictionary<string, string>();
            staffaliasdict.Add("Meriteringsanställningar", "Meriteringsanställning");

            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Antal anställda (personår)";

            // Set the text of the title
            chart1.Titles["Title1"].Text = focusname + " antal anställda";

            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            var qtype = from c in db.OV_stafftype select c;
            List<Series> ls = new List<Series>();
            foreach (OV_stafftype oi in qtype)
            {
                if (oi.Name == "Total")
                    continue;

                ls.Add(new Series(oi.Name));
                staffdict.Add(oi.Name, oi.Id);
            }

            double yearreference = 1;
            if (CB_refyear.Checked)
            {
                OV_staff oi = (from c in db.OV_staff 
                               where c.Uni == focusuniversity 
                               where c.Stafftype == 0 
                               where c.Gender == 0
                               where c.Age == 0
                               where c.Year == minyear 
                               select c).FirstOrDefault();
                if (oi != null)
                {
                    yearreference = 0.01 * oi.Number;
                }
            }

            Dictionary<int, double> ssmaxdict = new Dictionary<int, double>();
            for (int year = minyear; year <= maxyear; year++)
                ssmaxdict.Add(year, 0);

            double maxamountsum = 0;
            foreach (Series ss in ls)
            {
                //ss.ChartType = SeriesChartType.Line;
                if (staffaliasdict.ContainsKey(ss.Name))
                    continue;
                ss.ChartType = SeriesChartType.StackedArea;
                var q = (from c in db.OV_staff 
                         where c.Uni == focusuniversity 
                         where c.Stafftype == staffdict[ss.Name] 
                         where c.Gender == 0
                         where c.Age == 0
                         orderby c.Year select c);
                //var qtot = null;
                //if (CB_fraction.Checked)
                //    qtot = (from c in db.OV_University_Income where c.Uni == 0 where c.Incometype == 0 where c.Incomesource == incomedict[ss.Name] orderby c.Year select c);
                double amountsum = 0;
                for (int year = minyear; year <= maxyear; year++)
                //foreach (OV_University_Income oi in q)
                {
                    //OV_University_Income oi = (from c in q where c.Year == year select c).FirstOrDefault();
                    //double amount = 0;
                    //if (oi != null)
                    //    amount = oi.Amount;
                    var qoi = (from c in q where c.Year == year select c);
                    double amount = 0;
                    foreach (OV_staff oi in qoi)
                        amount += oi.Number;
                    foreach (string salias in staffaliasdict.Keys)
                    {
                        if (staffaliasdict[salias] == ss.Name)
                        {
                            var qalias = (from c in db.OV_staff
                                          where c.Uni == focusuniversity
                                          where c.Stafftype == staffdict[salias]
                                          where c.Gender == 0
                                          where c.Age == 0
                                          where c.Year == year
                                          select c.Number);
                            if (qalias.Count() > 0)
                                amount += (double)qalias.Sum();
                        }
                    }

                    ss.Points.AddXY(year, amount);

                    ssmaxdict[year] += amount;
                    amountsum += amount;

                    if (CB_memo.Checked)
                        parent.memo(ss.Name + "\t" + year + "\t" + amount);
                }

                if (amountsum > maxamountsum)
                    maxamountsum = amountsum;
                if (amountsum > 0 && amountsum > 0.001 * maxamountsum)
                    chart1.Series.Add(ss);

            }

            for (int year = minyear; year <= maxyear; year++)
                if (ssmaxdict[year] > ssmax)
                    ssmax = ssmaxdict[year];

            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;

        }

        private void RB_subjectarea_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_subjectarea.Checked)
                updatechart(sender, e);

        }

        private void RB_prestationsgrad_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_prestationsgrad.Checked)
                updatechart(sender, e);
        }

        private void LB_subjectarea_SelectedIndexChanged(object sender, EventArgs e)
        {
            int? itype = (from c in db.OV_subjectarea where c.Name == (string)LB_subjectarea.SelectedItem select c.Id).FirstOrDefault();
            if (itype > 0)
                totalhst((int)itype, 0);
            RB_hstsubject.Checked = true;
        }

        public void updatechart(object sender, EventArgs e)
        {
            if ( sender == null)
            {
                parent.memo("Macro-generated chart");
            }
            else if (!CB_updatechart.Checked && !sender.Equals(Refreshbutton))
            {
                parent.memo("Skipping updatechart. Click 'Uppdatera diagram' manually!");
                return;
            }

            chart1.ChartAreas[0].AxisX.Title = "";
            chart1.ChartAreas[0].AxisY.Minimum = 0;


            // Set cursor as hourglass
            //Application.UseWaitCursor = true;
            this.Cursor = Cursors.WaitCursor;

            if (RB_TotalIncome.Checked)
                totalincome();
            else if (RB_IncomeType.Checked)
                incometype_stackedarea();
            else if (RB_IncomeSource.Checked)
                incomesource_stackedarea();
            else if (RB_typesource.Checked)
                select_typesource();
            else if (RB_externalincome.Checked)
                externalincome();
            else if (RB_hsttotal.Checked)
                totalhst();
            else if (RB_hstsubject.Checked)
                LB_subjectarea_SelectedIndexChanged(sender, e);
            else if (RB_subjectarea.Checked)
                hst_stackedarea();
            else if (RB_averagepeng.Checked)
                averagepeng();
            else if (RB_prestationsgrad.Checked)
                prestationsgrad();
            else if (RB_totalexam.Checked)
                totalexam();
            else if (RB_examsubject.Checked)
                totalexam();
            else if (RB_examage.Checked)
                examage_list();
            else if (RB_publication.Checked)
            {
                if (CB_peerreview.Checked)
                    totalpub("art");
                else
                    totalpub();
            }
            else if (RB_pubtype.Checked)
                publications_stackedarea();
            else if (RB_establishment.Checked)
                totalestablished();
            else if (RB_salary.Checked)
                medianincome();
            else if (RB_diversity.Checked)
                totaldiversity();
            else if (RB_entrepreneur.Checked)
            {
                List<int> etypes = new List<int>() { 3, 12 };
                totalestablished(etypes, "Andel företagare ");
            }
            else if (RB_localrecruit.Checked)
            {
                if (CB_geography_lan.Checked)
                    recruit_by_lan();
                else
                    recruit_by_uni();

            }
            else if (RB_settling.Checked)
            {
                if (CB_geography_lan.Checked)
                    settling_by_lan();
                else
                    settling_by_uni();
            }
            else if (RB_examfrequency.Checked)
                examfreq();
            else if (RB_creditrate.Checked)
                creditrate();
            else if (RB_sickleave.Checked)
                sickleave();
            else if (RB_stafftype.Checked)
                LB_staff_SelectedIndexChanged(sender, e);
            else if (RB_staffstacked.Checked)
                staff_stackedarea();
            else if (RB_agestaff.Checked)
                agestaff(0,false,false);
            else if (RB_applicants.Checked)
                LB_lsjsubject_SelectedIndexChanged(sender, e);
            else if (RB_appl_eng.Checked)
            {
                int itype = 0;
                if (RB_appl1h.Checked)
                    itype = 1;
                if (RB_appltotal.Checked)
                    itype = 2;
                if (RB_accepted.Checked)
                    itype = 3;
                if (RB_reserves.Checked)
                    itype = 4;

                FormSelectSpecial fss = new FormSelectSpecial();
                fss.ShowDialog();
                parent.memo("Selected in fss: " + fss.selection);
                totalapplicants(itype, new List<int>(), new List<int>(), 0, 0, fss.selection);
            }
            else if (RB_HDa_sectors.Checked)
            {
                int itype = 0;
                if (RB_appl1h.Checked)
                    itype = 1;
                if (RB_appltotal.Checked)
                    itype = 2;
                if (RB_accepted.Checked)
                    itype = 3;
                if (RB_reserves.Checked)
                    itype = 4;
                List<int> sectorlist;
                List<int> subjectlist;
                if (CB_HDaSpecial.Checked)
                {
                    sectorlist = new List<int>() { 4, 5 };
                    subjectlist = new List<int>() { 20, 22, 26, 28, 47, 74, 76, 86, 105, 145 };
                }
                else
                {
                    sectorlist = new List<int>() { 1,2,3,4, 5,6 };
                    subjectlist = new List<int>();
                }

                totalapplicants(itype, sectorlist, subjectlist, 0, 0, "");

            }
            else if (RB_appscore.Checked)
                totalappscore(6, 0, 0, CB_appscore.Checked);
            else if (RB_civing.Checked)
                civing_table();
            else if (RB_bibliometry.Checked)
                VR_bibliometry();
            else if (RB_bibliomoney.Checked)
                VR_bibliomoney();
            else if (RB_student_teacher_ratio.Checked)
                student_teacher_ratio();
            else if (RB_HSTteacher.Checked)
                HSTteacher_ratio();
            else if (RB_studHST.Checked)
                studHST_ratio();
            // Set cursor as default arrow
            //Application.UseWaitCursor = false;

            if ( CB_trendline.Checked)
                Trendline();

            if (CB_serieslabel.Checked)
                foreach (Series ss in chart1.Series)
                {
                    if (unishortdict.ContainsKey(ss.Name))
                        ss.Points.Last().Label = unishortdict[ss.Name];
                    else
                        ss.Points.Last().Label = ss.Name;
                }


            if (CB_values_to_file.Checked)
            {
                SaveValuesToFile(chart1);
            }


            this.Cursor = Cursors.Default;
        }

        private void VR_bibliometry()
        {
            this.Cursor = Cursors.WaitCursor;

            int minyear = (from c in db.OV_VRbibliometry select c.Year).Min();
            int maxyear = (from c in db.OV_VRbibliometry select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_VRbibliometry" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            //string priceindex = get_priceindex();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Bibliometriskt index (VR)";
            double ssmax = 0;


            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Bibliometriskt index, utvalda lärosäten";
            else
                chart1.Titles["Title1"].Text = focusname + " bibliometriskt index";

            if (CB_fraction.Checked)
            {
                chart1.Titles["Title1"].Text += ", andel av riket";
                chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            }
            if (CB_refyear.Checked)
                chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_VRbibliometry where c.Uni == 0 where c.Subject == 0 orderby c.Year select c);
                foreach (OV_VRbibliometry oi in qt)
                {
                    refdict.Add(oi.Year, oi.Bibindex);
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();
            StringBuilder sb = new StringBuilder();

            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    OV_VRbibliometry oi = (from c in db.OV_VRbibliometry where c.Uni == unidict[uniname] where c.Subject == 0 where c.Year == minyear select c).FirstOrDefault();
                    if (oi != null)
                    {
                        if (CB_fraction.Checked)
                            yearreference = 0.01 * (oi.Bibindex ) / refdict[minyear];
                        else
                            yearreference = 0.01 * oi.Bibindex;

                    }
                }


                var q = (from c in db.OV_VRbibliometry where c.Uni == unidict[uniname] where c.Subject == 0 orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    foreach (OV_VRbibliometry oi in (from c in q where c.Year == year select c))
                    {
                        if (oi != null)
                        {
                            amount += oi.Bibindex;
                        }
                    }
                    amount = amount / yearreference;
                    if (CB_fraction.Checked || CB_demography.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                    if (CB_values_to_file.Checked)
                        sb.Append(ss.Name + "\t" + year + "\t" + amount + "\n");


                }
                ss.ChartType = SeriesChartType.Line;

                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);

            }

            //if (CB_values_to_file.Checked)
            //{
            //    SaveValuesToFile(sb, chart1.Titles["Title1"].Text);
            //}


            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;
        }

        private double zeroregression(List<Tuple<int,double>> pairs)
        {
            //regression through origin. Returns slope
            double sumxy = 0;
            double sumxx = 0;
            foreach (Tuple<int,double> tt in pairs)
            {
                sumxy += tt.Item1 * tt.Item2;
                sumxx += (double)tt.Item1 * tt.Item1;
            }
            return sumxy / sumxx;
        }

        private double regressionresidual(List<Tuple<int, double>> pairs, double slope, double intercept)
        {
            return regressionresidual(pairs, slope, intercept, false);
        }

        private double regressionresidual(List<Tuple<int, double>> pairs, double slope, double intercept, bool scramble)
        {
            //residuals from a regression line
            double res2 = 0;
            Random rnd = new Random();

            foreach (Tuple<int, double> tt in pairs)
            {
                double yline = slope * tt.Item1 + intercept;
                double y = tt.Item2;
                if (scramble)
                    y = pairs[rnd.Next(pairs.Count)].Item2;
                res2 += (yline - y) * (yline - y);
            }
            return Math.Sqrt(res2);
        }

        private void print_signed_residuals(List<Tuple<int, double>> pairs, double slope, double intercept)
        {
            foreach (Tuple<int, double> tt in pairs)
            {
                double yline = slope * tt.Item1 + intercept;
                double y = tt.Item2;
                parent.memo(tt.Item1 + "\t" + (y - yline));
            }

        }

        private void VR_bibliomoney()
        {
            Dictionary<int, Dictionary<int, int>> moneydict = new Dictionary<int, Dictionary<int, int>>();

            int incometype = 2; //Forskning & forskarutbildning
            int incomesource = 0; //allting


            this.Cursor = Cursors.WaitCursor;

            int minyear = (from c in db.OV_VRbibliometry select c.Year).Min();
            int maxyear = (from c in db.OV_VRbibliometry select c.Year).Max();
            int minmoneyyear = (from c in db.OV_University_Income select c.Year).Min();
            int maxmoneyyear = (from c in db.OV_University_Income select c.Year).Max();

            chart1.Titles["Title2"].Text = getsource(new string[] { "OV_VRbibliometry", "OV_University_Income" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            // Set the text of the title
            chart1.Titles["Title1"].Text = "Bibliometriskt index vs. forskningsresurser (varje punkt = ett lärosäte ett år)";
            //if (LB_uni.CheckedItems.Count > 1)
            //    chart1.Titles["Title1"].Text = "Bibliometriskt index, utvalda lärosäten";
            //else
            //    chart1.Titles["Title1"].Text = focusname + " bibliometriskt index";

            //if (CB_fraction.Checked)
            //{
            //    chart1.Titles["Title1"].Text += ", andel av riket";
            //    chart1.ChartAreas[0].AxisY.Title = "Andel av riket";
            //}
            //if (CB_refyear.Checked)
            //    chart1.Titles["Title1"].Text += " (" + minyear.ToString() + "=100)";

            int bestoffset = 5;
            chart1.Titles["Title1"].Text += ", offset=" + bestoffset + " år";

            //Dictionary<string, int> incomedict = new Dictionary<string, int>();
            string priceindex = get_priceindex();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Bibliometriskt index (VR) år N";
            chart1.ChartAreas[0].AxisX.Title = "Forskningsresurser (Tkr) år N-" + bestoffset+", "+priceindex;
            double ssmax = 0;
            double sxmax = 0;



            var qinc = from c in db.OV_University_Income where c.Incometype == incometype where c.Incomesource == incomesource select c;

            List<string> unilist = (from c in db.OV_VRbibliometry select c.OV_University.Name).Distinct().ToList();
            List<int> gooduni = new List<int>();
            foreach (string uniname in unilist)
            {
                int uni = unidict[uniname];
                if (uni <= 0)
                    continue;
                gooduni.Add(uni);
                moneydict.Add(uni, new Dictionary<int, int>());
                for (int year = minmoneyyear;year<=maxmoneyyear;year++)
                {
                    moneydict[uni].Add(year, 0);
                }
            }

            foreach (OV_University_Income ou in qinc)
            {
                if (!gooduni.Contains(ou.Uni))
                    continue;
                moneydict[ou.Uni][ou.Year] += ou.Amount;
            }

            parent.memo("Offset\tResidual\t#pairs\tSlope\tksum/n\tcorr");
            var qq = from c in db.OV_VRbibliometry where c.Subject == 0 select c;
            double ksum = 0;
            List<Tuple<int, double>> bestpairs = new List<Tuple<int, double>>();
            Series sp = new Series(" ");

            int minoffset = bestoffset;
            int maxoffset = bestoffset;
            for (int offset = minoffset; offset <= maxoffset;offset++ )
            {
                List<Tuple<int,double>> pairs = new List<Tuple<int,double>>();
                //foreach (OV_VRbibliometry ob in qq)
                foreach (int uni in gooduni)
                {
                    for (int year = minyear; year <= maxyear; year++)
                    {
                        //foreach (OV_VRbibliometry ob in (from )
                        //{
                            //if (!gooduni.Contains(ob.Uni))
                            //    continue;
                            //if (ob.Year < minyear)
                            //    continue;
                            //if (ob.Year > maxyear)
                            //    continue;

                            int myear = year - offset;
                            if (myear < minmoneyyear)
                                continue;
                            if (myear > maxmoneyyear)
                                continue;
                            double bibindex = (from c in qq where c.Uni == uni where c.Year == year select c.Bibindex).Sum();
                            if (bibindex == 0)
                                continue;
                            pairs.Add(new Tuple<int, double>((int)(moneydict[uni][myear] * adjustprice(myear, priceindex)), bibindex));
                            if (offset == bestoffset)
                            {
                                Tuple<int, double> ttt = new Tuple<int, double>((int)(moneydict[uni][myear] * adjustprice(myear, priceindex)), bibindex);
                                bestpairs.Add(ttt);
                                sp.Points.AddXY((double)ttt.Item1, ttt.Item2);
                                if (CB_serieslabel.Checked)
                                    sp.Points.Last().Label = unishortdict[getuni(uni)].ToUpper() + (year % 100);
                                if (ttt.Item2 > ssmax)
                                    ssmax = ttt.Item2;
                                if (ttt.Item1 > sxmax)
                                    sxmax = ttt.Item1;

                            }
                            ksum += bibindex / (moneydict[uni][myear] * adjustprice(myear, priceindex));
                        //}
                    }
                }
                double slope = zeroregression(pairs);
                double residual = regressionresidual(pairs, slope, 0)/pairs.Count;
                double scrambleresidual = regressionresidual(pairs, slope, 0,true) / pairs.Count;
                //print_signed_residuals(pairs, slope, 0);
                double corr = 1 - residual / scrambleresidual;
                parent.memo(offset + "\t" + residual + "\t" + pairs.Count + "\t" + slope + "\t" + ksum / pairs.Count+"\t"+corr);
            }

            sp.ChartType = SeriesChartType.Point;

            chart1.Series.Add(sp);
            chart1.ChartAreas[0].AxisY.Maximum = roundaxis(2*ssmax);
            chart1.ChartAreas[0].AxisX.Maximum = roundaxis(2*sxmax);
            chart1.ChartAreas[0].AxisY.Minimum = 10;
            chart1.ChartAreas[0].AxisX.Minimum = 1000;

            chart1.ChartAreas[0].AxisX.IsLogarithmic = true;
            chart1.ChartAreas[0].AxisY.IsLogarithmic = true;
            //chart1.ChartAreas[0].AxisX.Minimum = roundaxis(sxmax);
            this.Cursor = Cursors.Default;


            return;

            //List<string> unilist = new List<string>();
            //foreach (string s in LB_uni.CheckedItems)
            //{
            //    unilist.Add(s);
            //}
            //if (unilist.Count == 0)
            //    unilist.Add(focusname);

            Dictionary<int, double> refdict = new Dictionary<int, double>();
            if (CB_fraction.Checked)
            {
                var qt = (from c in db.OV_VRbibliometry where c.Uni == 0 where c.Subject == 0 orderby c.Year select c);
                foreach (OV_VRbibliometry oi in qt)
                {
                    refdict.Add(oi.Year, oi.Bibindex);
                }

                //chart1.ChartAreas[0].AxisY.Maximum = 0.2;
            }

            //if ( CB_refyear.Checked)
            //    chart1.ChartAreas[0].AxisY.Maximum = 150;
            Series sumseries = getsumseries(unilist);
            Dictionary<int, double> sumdict = new Dictionary<int, double>();
            StringBuilder sb = new StringBuilder();

            foreach (string uniname in unilist)
            {
                Series ss = new Series(uniname);
                if (uniname == focusname && !CB_sumuni.Checked)
                    ss.BorderWidth = focusthickness;
                else
                    ss.BorderWidth = linethickness;

                double yearreference = 1;
                if (CB_refyear.Checked)
                {
                    OV_VRbibliometry oi = (from c in db.OV_VRbibliometry where c.Uni == unidict[uniname] where c.Subject == 0 where c.Year == minyear select c).FirstOrDefault();
                    if (oi != null)
                    {
                        if (CB_fraction.Checked)
                            yearreference = 0.01 * (oi.Bibindex) / refdict[minyear];
                        else
                            yearreference = 0.01 * oi.Bibindex;

                    }
                }


                var q = (from c in db.OV_VRbibliometry where c.Uni == unidict[uniname] where c.Subject == 0 orderby c.Year select c);
                //parent.memo("q.Count = " + q.Count());
                for (int year = minyear; year <= maxyear; year++)
                {
                    double amount = 0;
                    foreach (OV_VRbibliometry oi in (from c in q where c.Year == year select c))
                    {
                        if (oi != null)
                        {
                            amount += oi.Bibindex;
                        }
                    }
                    amount = amount  / yearreference;
                    if (CB_fraction.Checked || CB_demography.Checked)
                        amount = amount / refdict[year];
                    ss.Points.AddXY(year, amount);
                    if (CB_sumuni.Checked)
                    {
                        if (!sumdict.ContainsKey(year))
                            sumdict.Add(year, amount);
                        else
                            sumdict[year] += amount;
                    }
                    if (amount > ssmax)
                        ssmax = amount;
                    if (CB_memo.Checked)
                        parent.memo(year + "\t" + amount);
                    if (CB_values_to_file.Checked)
                        sb.Append(ss.Name + "\t" + year + "\t" + amount + "\n");


                }
                ss.ChartType = SeriesChartType.Point;

                chart1.Series.Add(ss);
            }

            if (CB_sumuni.Checked)
            {
                foreach (int year in sumdict.Keys)
                {
                    if (CB_meanuni.Checked)
                        sumseries.Points.AddXY(year, sumdict[year] / unilist.Count);
                    else
                        sumseries.Points.AddXY(year, sumdict[year]);
                }
                ssmax = sumdict.Values.Max();
                chart1.Series.Add(sumseries);

            }

            //if (CB_values_to_file.Checked)
            //{
            //    SaveValuesToFile(sb, chart1.Titles["Title1"].Text);
            //}


            double axislength = roundaxis(ssmax);
            chart1.ChartAreas[0].AxisY.Maximum = axislength;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.Cursor = Cursors.Default;
        }

        private void examage_list()
        {

            var qtype = from c in db.OV_examtype
                        where c.Kolumn == 1
                        where c.Level != null
                        orderby c.Level
                        select c;

            int minyear = (from c in db.OV_exam select c.Year).Min();
            int maxyear = (from c in db.OV_exam select c.Year).Max();

            //chart1.Titles["Title2"].Text = getsource(new string[] { "OV_exam" }, true);

            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }
            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            parent.memo("Examina per åldersgrupp " + minyear + "-" + maxyear);
            parent.memo("Examen\t-24\t25-34\t35+");
            foreach (OV_examtype otype in qtype)
            {
                StringBuilder sb = new StringBuilder(otype.Name);
                for (int i=1;i<=3;i++)
                {
                    var q = from c in db.OV_exam
                            where c.Examtype1 == otype.Id
                            where c.Examtype2 == 0
                            where c.Age == i
                            where c.Gender == 0
                            where c.Uni == 0
                            where c.Year >= minyear
                            where c.Year <= maxyear
                            select c.Number;
                    if (q != null && q.Count() > 0)
                        sb.Append("\t" + q.Sum());
                    else
                        sb.Append("\t0");

                }
                parent.memo(sb.ToString());
            }
        }

        private void civing_table()
        {
            Dictionary<string, Dictionary<int, int>> citable = new Dictionary<string, Dictionary<int, int>>();
            foreach (OV_examtype ctype in (from c in db.OV_examtype where c.Grp=="C" select c))
            {
                Dictionary<int, int> dum = new Dictionary<int, int>();
                citable.Add(ctype.Name, dum);
                StringBuilder sb = new StringBuilder(ctype.Name);
                for (int year=1992;year<2019;year++)
                {
                    citable[ctype.Name].Add(year, 0);
                    var q = from c in db.OV_exam
                            where c.Examtype2 == ctype.Id
                            where c.Year == year
                            where c.Uni > 0
                            where c.Gender == 0
                            where c.Age == 0
                            select c;
                    if (q != null)
                        citable[ctype.Name][year] = q.Count();
                    sb.Append("\t"+citable[ctype.Name][year].ToString());
                }
                parent.memo(sb.ToString());
            }
            parent.memo(getsource(new string[] { "OV_exam" }, true));
        }

        public void clearuniselection()
        {
            LB_uni.ClearSelected();
            foreach (int i in LB_uni.CheckedIndices)
                LB_uni.SetItemCheckState(i, CheckState.Unchecked);


            focusuniversity = ihda;
            focusname = hda;
            focuslabel.Text = focusname;

        }

        private void ClearUniButton_Click(object sender, EventArgs e)
        {
            clearuniselection();
            updatechart(sender, e);
        }

        public void CompetitorButton_Click(object sender, EventArgs e)
        {
            clearuniselection();
            List<string> lcomp = new List<string>();
            lcomp.Add("Högskolan Dalarna");
            lcomp.Add("Högskolan i Gävle");
            lcomp.Add("Mälardalens högskola");
            lcomp.Add("Högskolan Väst");
            lcomp.Add("Högskolan i Skövde");
            lcomp.Add("Högskolan i Halmstad");
            lcomp.Add("Högskolan i Jönköping");
            lcomp.Add("Mittuniversitetet");
            lcomp.Add("Högskolan i Borås");
            lcomp.Add("Karlstads universitet");
            lcomp.Add("Örebro universitet");
            lcomp.Add("Högskolan Kristianstad");
            lcomp.Add("Blekinge tekniska högskola");
            lcomp.Add("Södertörns Högskola");
            lcomp.Add("Malmö Universitet");
            lcomp.Add("Linnéuniversitetet");

            for (int i =0; i < LB_uni.Items.Count;i++)
            {
                if ( lcomp.Contains(LB_uni.Items[i]))
                {
                    LB_uni.SetItemCheckState(i, CheckState.Checked);
                }
            }
            updatechart(sender, e);
        }

        private void ostsvenska_button_Click(object sender, EventArgs e)
        {
            clearuniselection();
            List<string> lcomp = new List<string>();
            lcomp.Add("Högskolan Dalarna");
            lcomp.Add("Högskolan i Gävle");
            lcomp.Add("Mälardalens högskola");
            lcomp.Add("Uppsala universitet");
            lcomp.Add("Sveriges lantbruksuniversitet");
            lcomp.Add("Örebro universitet");

            for (int i = 0; i < LB_uni.Items.Count; i++)
            {
                if (lcomp.Contains(LB_uni.Items[i]))
                {
                    LB_uni.SetItemCheckState(i, CheckState.Checked);
                }
            }
            updatechart(sender, e);

        }



        private void RB_totalexam_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_totalexam.Checked)
                updatechart(sender, e);
            
        }

        private void LB_exam_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selitem = (string)LB_exam.SelectedItem;
            if (selitem.Contains("--"))
            {
                examgroup_stackedarea(selitem);
            }
            else
            {
                OV_examtype oe = (from c in db.OV_examtype where c.Name == (selitem).Trim() select c).FirstOrDefault();
                int[] itypes = new int[] { 0, 0, 0 };
                if (oe != null)
                {
                    itypes[oe.Kolumn] = oe.Id;
                }
                totalexam(itypes, 0, 0);
                RB_examsubject.Checked = true;
            }
        }

        private void RB_examsubject_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_examsubject.Checked)
                updatechart(sender, e);
            
        }

        private void RB_publication_CheckedChanged(object sender, EventArgs e)
        {
            parent.memo("In RB_publication_CheckedChanged");
            parent.memo(e.ToString());
            if ( RB_publication.Checked)
                updatechart(sender, e);
        }

        private void RB_establishment_CheckedChanged(object sender, EventArgs e)
        {
            parent.memo("In RB_establishment_CheckedChanged");
            if (RB_establishment.Checked)
                updatechart(sender, e);
        }

        private void RB_localrecruit_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_localrecruit.Checked)
                updatechart(sender, e);
        }

        private void RB_settling_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_settling.Checked)
                updatechart(sender, e);
        }


        private void RB_hstsubject_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_hstsubject.Checked)
                updatechart(sender, e);

        }

        private void RB_hsttotal_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_hsttotal.Checked)
                updatechart(sender, e);
        }

        private void CB_geography_lan_CheckedChanged(object sender, EventArgs e)
        {
            if (!RB_localrecruit.Checked)
                RB_settling.Checked = true;
            updatechart(sender, e);
        }

        private void CB_fraction_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void RB_bibabsolute_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_totalexam.Checked)
            {
                RB_totalexam.Checked = false;
                if (RB_bibabsolute.Checked)
                    RB_totalexam.Checked = true;

            }
            else
            {
                RB_publication.Checked = false;
                if (RB_bibabsolute.Checked)
                    RB_publication.Checked = true;
            }
            //updatechart(sender, e);
        }

        private void RB_permoney_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_totalexam.Checked)
            {
                RB_totalexam.Checked = false;
                if (RB_permoney.Checked)
                    RB_totalexam.Checked = true;

            }
            else
            {
                RB_publication.Checked = false;
                if (RB_permoney.Checked)
                    RB_publication.Checked = true;
            }
            //updatechart(sender, e);

        }

        private void RB_perscientist_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_totalexam.Checked)
            {
                RB_totalexam.Checked = false;
                if (RB_perscientist.Checked)
                    RB_totalexam.Checked = true;

            }
            else
            {
                RB_publication.Checked = false;
                if (RB_perscientist.Checked)
                    RB_publication.Checked = true;
            }
            //updatechart(sender, e);

        }

        private void StudentflowButton_Click(object sender, EventArgs e)
        {
            FormSankey fs = new FormSankey(db, parent, this,chart1);
            fs.Show();
        }

        private void RB_sickleave_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_sickleave.Checked)
                updatechart(sender, e);
        }

        private void RB_entrepreneur_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_entrepreneur.Checked)
                updatechart(sender, e);
        }

        private void RB_salary_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_salary.Checked)
                updatechart(sender, e);
        }

        private void RB_examfrequency_CheckedChanged(object sender, EventArgs e)
        {
            if ( RB_examfrequency.Checked)
                updatechart(sender, e);
        }

        private void RB_allexamfreq_CheckedChanged(object sender, EventArgs e)
        {
            RB_examfrequency.Checked = false;
            if (RB_allexamfreq.Checked)
                RB_examfrequency.Checked = true;
        }

        private void RB_examgender_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbb = RB_diversity;
            if (RB_examfrequency.Checked)
                rbb = RB_examfrequency;
            parent.memo("rbb = " + rbb.Text);
            rbb.Checked = false;
            if (RB_examgender.Checked)
                rbb.Checked = true;

        }

        private void RB_examforeign_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbb = RB_diversity;
            if (RB_examfrequency.Checked)
                rbb = RB_examfrequency;
            rbb.Checked = false;
            if (RB_examforeign.Checked)
                rbb.Checked = true;

        }

        private void RB_exameduparent_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbb = RB_diversity;
            if (RB_examfrequency.Checked)
                rbb = RB_examfrequency;
            rbb.Checked = false;
            if (RB_exameduparent.Checked)
                rbb.Checked = true;

        }

        private void RB_studentage_CheckedChanged(object sender, EventArgs e)
        {
            //RadioButton rbb = RB_diversity;
            //if (RB_examfrequency.Checked)
            //    rbb = RB_examfrequency;
            //rbb.Checked = false;
            //if (RB_studentage.Checked)
            //    rbb.Checked = true;
            RB_applicants.Checked = true;
        }



        private void RB_diversity_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_diversity.Checked)
                updatechart(sender, e);
        }

        private void RB_stafftype_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_stafftype.Checked)
                updatechart(sender, e);

        }

        private void LB_staff_SelectedIndexChanged(object sender, EventArgs e)
        {
            int? itype = (from c in db.OV_stafftype where c.Name == (string)LB_staff.SelectedItem select c.Id).FirstOrDefault();
            if ( itype == null)
            {
                parent.memo("No staff type selected.");
                return;
            }

            parent.memo((string)LB_staff.SelectedItem+" itype = " + itype);
            if (itype > 0)
                totalstaff((int)itype, false, false);
            else if (((string)LB_staff.SelectedItem).Contains("orskarutbildade"))
                totalstaff(0, true, false);
            else if (((string)LB_staff.SelectedItem).Contains("tödperson"))
                totalstaff(0, false, true);
            else
                totalstaff(0, false, false);

            RB_stafftype.Checked = true;
        }

        private void RB_running_CheckedChanged(object sender, EventArgs e)
        {
            updatechart(sender, e);
        }

        private void RB_PLO_CheckedChanged(object sender, EventArgs e)
        {
            updatechart(sender, e);
        }

        private void RB_KPI_CheckedChanged(object sender, EventArgs e)
        {
            updatechart(sender, e);
        }

        private void RB_salaryindex_CheckedChanged(object sender, EventArgs e)
        {
            updatechart(sender, e);
        }

        private void RB_applicants_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_applicants.Checked)
                updatechart(sender, e);
        }

        private void LB_lsjsubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sel = (string)LB_lsjsubject.SelectedItem;
            int isector = 0;
            int isubject = 0;
            if (sel != null)
            {
                if (sel.Contains(" - "))
                {
                    //mysubject
                    sel = sel.Replace(" - ", "");
                    isubject = (from c in db.OV_mysubject where c.Name == sel select c.Id).FirstOrDefault();

                }
                else
                {
                    //mysector
                    isector = (from c in db.OV_mysector where c.Name == sel select c.Id).FirstOrDefault();
                }
            }
            int itype = 0;
            if (RB_appl1h.Checked)
                itype = 1;
            if (RB_appltotal.Checked)
                itype = 2;
            if (RB_accepted.Checked)
                itype = 3;
            if (RB_reserves.Checked)
                itype = 4;
            totalapplicants(itype, isector, isubject, 0, 0);

            RB_applicants.Checked = true;
        }

        private void RB_appl1h_CheckedChanged(object sender, EventArgs e)
        {
            RB_applicants.Checked = false;
            if (RB_appl1h.Checked)
                RB_applicants.Checked = true;
        }

        private void RB_appltotal_CheckedChanged(object sender, EventArgs e)
        {
            RB_applicants.Checked = false;
            if (RB_appltotal.Checked)
                RB_applicants.Checked = true;

        }

        private void RB_accepted_CheckedChanged(object sender, EventArgs e)
        {
            RB_applicants.Checked = false;
            if (RB_accepted.Checked)
                RB_applicants.Checked = true;

        }

        private void RB_reserves_CheckedChanged(object sender, EventArgs e)
        {
            RB_applicants.Checked = false;
            if (RB_reserves.Checked)
                RB_applicants.Checked = true;

        }

        private void RB_appscore_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_appscore.Checked)
                updatechart(sender, e);

        }

        private void UrvalsgruppButton_Click(object sender, EventArgs e)
        {
            foreach (OV_urvalsgrupp ou in (from c in db.OV_urvalsgrupp select c))
            {
                var q = from c in db.OV_antagningspoang where c.Urvalsgrupp == ou.Id select c;
                if (q.Count() > 1000)
                {
                    int minyear = (from c in q select c.OV_course.Year).Min();
                    int maxyear = (from c in q select c.OV_course.Year).Max();
                    parent.memo(ou.Code + ": " + q.Count() + ", " + minyear + "-" + maxyear);
                }
            }
        }

        private void Refreshbutton_Click(object sender, EventArgs e)
        {
            updatechart(sender, e);
        }

        private void RB_HDa_sectors_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_HDa_sectors.Checked)
                updatechart(sender, e);

        }

        private void CB_logarithm_CheckedChanged(object sender, EventArgs e)
        {
            updatechart(sender, e);
        }

        private void RB_pubtype_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_pubtype.Checked)
                updatechart(sender, e);
        }

        private void RB_civing_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_civing.Checked)
                updatechart(sender, e);
        }

        private void CB_trendline_CheckedChanged(object sender, EventArgs e)
        {
            updatechart(sender, e);
        }

        private void RB_averagepeng_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_averagepeng.Checked)
                updatechart(sender, e);

        }

        private void Regionbutton_Click(object sender, EventArgs e)
        {
            FormRegional fs = new FormRegional(db, parent, this, chart1,unidict);
            fs.Show();
        }

        private void testbutton_Click(object sender, EventArgs e)
        {
            //int minyear = (from c in db.OV_course select c.Year).Min() + 1; //skip incomplete 2008
            //int maxyear = (from c in db.OV_course select c.Year).Max();

            //for (int year = minyear; year <= maxyear; year++)
            //{
            //    var q = from c in db.OV_course where year == c.Year select c;
            //    var qa = from c in db.OV_applicants where year == c.Year select c;
            //    parent.memo(year + ": " + qa.Count() + " / " + q.Count() + " = " + qa.Count() / q.Count());
            //}

            var q = from c in db.OV_course
                    where c.Name.Contains("mneslärar")
                    where !c.Name.Contains("ompletterande pedag")
                    where !c.Name.Contains("KPU")
                    where !c.Name.Contains("asår")
                    where c.Year == 2012
                    select c;
            foreach (var oc in q)
            {
                StringBuilder sb = new StringBuilder(oc.Name);
                var dict = util.parse_amneslarare(oc.Name);
                foreach (int k in dict.Keys)
                {
                    sb.Append("\t" + dict[k]);
                }
                parent.memo(sb.ToString());
            }
        }

        private void CB_updatechart_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_updatechart.Checked)
                updatechart(sender, e);
        }

        private void CB_meanuni_CheckedChanged(object sender, EventArgs e)
        {
            if ( CB_meanuni.Checked)
            {
                CB_sumuni.Checked = true;
            }
        }

        private void CB_sumuni_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_sumuni.Checked)
                updatechart(sender, e);
        }

        private void RB_appl_eng_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_appl_eng.Checked)
                updatechart(sender, e);

        }

        private void RB_creditrate_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_creditrate.Checked)
                updatechart(sender, e);
        }

        private void Macrobutton_Click(object sender, EventArgs e)
        {
            FormMacro fm = new FormMacro(db, parent, this, chart1);
            fm.Show();
        }

        private void RB_externalincome_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_externalincome.Checked)
                updatechart(sender, e);
        }

        private void RB_staffstacked_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_staffstacked.Checked)
                updatechart(sender, e);
        }

        private void RB_examage_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_examage.Checked)
                updatechart(sender, e);

        }

        private void RB_stackedexams_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void RB_bibliometry_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_bibliometry.Checked)
                updatechart(sender, e);

        }

        private void RB_bibliomoney_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_bibliomoney.Checked)
                updatechart(sender, e);

        }

        private void RB_student_teacher_ratio_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_student_teacher_ratio.Checked)
                updatechart(sender, e);

        }

        private void RB_HSTteacher_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_HSTteacher.Checked)
                updatechart(sender, e);
        }

        private void RB_studHST_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_studHST.Checked)
                updatechart(sender, e);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormCompetitors fc = new FormCompetitors(db, parent, this, focusuniversity, 2021);
            fc.Show();
        }

        private void acceptstatbutton_Click(object sender, EventArgs e)
        {
            int minyear = 2013;
            int maxyear = (from c in db.OV_course select c.Year).Max();
            if (CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.ChartAreas[0].AxisX.Maximum = maxyear + 1;
            chart1.ChartAreas[0].AxisX.Minimum = minyear - 1; chart1.ChartAreas[0].AxisX.Interval = 1;


            for (int year=minyear;year<=maxyear;year++)
            {
                var q = from c in db.OV_course
                        where c.Year == year
                        where c.Accepted > 0
                        select c;
                if (RB_prog.Checked)
                    q = from c in q
                        where c.Program
                        select c;
                else if (RB_fk.Checked)
                    q = from c in q
                        where !c.Program
                        select c;

                Dictionary<int, hbookclass> unihist = new Dictionary<int, hbookclass>();
                unihist.Add(0,new hbookclass("Hela riket antagna per kurs "+year));
                int nbins = 5;
                double min = 0;
                double max = 50;
                unihist[0].SetBins(min, max, nbins);

                foreach (OV_course oc in q)
                {
                    unihist[0].Add((double)oc.Accepted);
                    if (!unihist.ContainsKey(oc.Uni))
                    {
                        unihist.Add(oc.Uni, new hbookclass(oc.OV_University.Name+" antagna per kurs "+year));
                        unihist[oc.Uni].SetBins(min, max, nbins);
                    }
                    unihist[oc.Uni].Add((double)oc.Accepted);
                }

                foreach (int uni in unihist.Keys)
                    parent.memo(unihist[uni].GetDHist());
            }


        }

        private void financebutton_Click(object sender, EventArgs e)
        {
            FormFinance ff = new FormFinance(db, this, parent);
            ff.Show();
        }

        public void make_fourfield(int plusprodpost, int minusprodpost, int moneypost)
        {
            int endyear = getendyear();
            int startyear = getstartyear();
            if (endyear < 0)
                endyear = (from c in db.OV_finance select c.Year).Max();
            if (startyear < 0)
                startyear = endyear - 10;

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            //string priceindex = get_priceindex();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisX.Title = "Över/underproduktion (tkr)";
            chart1.ChartAreas[0].AxisY.Title = "Ekonomiskt resultat GU (tkr)";
            double ssxmax = 0;
            double ssxmin = 0;
            double ssymax = 0;
            double ssymin = 0;

            var q = from c in db.OV_finance
                    where c.Verksamhet == 1
                    where c.Year >= startyear
                    where c.Year <= endyear
                    select c;

            // Set the text of the title
            if (LB_uni.CheckedItems.Count > 1)
                chart1.Titles["Title1"].Text = "Över/under-prod vs. ekonomiskt resultat GU";
            else
                chart1.Titles["Title1"].Text = focusname + " Över/under-prod vs.ekonomiskt resultat GU";

            List<string> unilist = new List<string>();
            foreach (string s in LB_uni.CheckedItems)
            {
                unilist.Add(s);
            }
            if (unilist.Count == 0)
                unilist.Add(focusname);

            foreach (string uni in unilist)
            {
                var qplus = from c in q
                            where c.Uni == unidict[uni]
                            where c.Post == plusprodpost
                            select c;
                var qminus = from c in q
                            where c.Uni == unidict[uni]
                            where c.Post == minusprodpost
                            select c;
                var qmoney = from c in q
                            where c.Uni == unidict[uni]
                            where c.Post == moneypost
                            select c;
                double[] xp = new double[endyear - startyear + 1];
                double[] yp = new double[endyear - startyear + 1];

                foreach (OV_finance ff in qplus)
                {
                    int i = ff.Year - startyear;
                    xp[i] = ff.Amount;
                }
                foreach (OV_finance ff in qminus)
                {
                    int i = ff.Year - startyear;
                    xp[i] -= ff.Amount;
                }
                foreach (OV_finance ff in qmoney)
                {
                    int i = ff.Year - startyear;
                    yp[i] = ff.Amount;
                }
                if (xp.Max() > ssxmax)
                    ssxmax = xp.Max();
                if (xp.Min() < ssxmin)
                    ssxmin = xp.Min();
                if (yp.Max() > ssymax)
                    ssymax = yp.Max();
                if (yp.Min() < ssymin)
                    ssymin = yp.Min();

                Series series1 = new Series(uni);
                series1.Points.DataBindXY(xp, yp);
                for (int i = 0; i < xp.Length; i++)
                    series1.Points[i].Label = (startyear + i).ToString();
                series1.Points[endyear-startyear].Label = unishortdict[uni];
                series1.ChartType = SeriesChartType.Line;
                series1.MarkerStyle = MarkerStyle.Circle;
                chart1.Series.Add(series1);

            }

            chart1.ChartAreas[0].AxisX.Maximum =  roundaxis(Math.Max(ssxmax,Math.Abs(ssxmin)));
            chart1.ChartAreas[0].AxisX.Minimum = -roundaxis(Math.Max(ssxmax, Math.Abs(ssxmin)));
            chart1.ChartAreas[0].AxisY.Maximum =  roundaxis(Math.Max(ssymax, Math.Abs(ssymin)));
            chart1.ChartAreas[0].AxisY.Minimum = -roundaxis(Math.Max(ssymax, Math.Abs(ssymin)));
            //chart1.ChartAreas[0].AxisX.Crossing = 0;
            //chart1.ChartAreas[0].AxisY.Crossing = 0;
            chart1.ChartAreas[0].AxisX.Interval = 0.25 * roundaxis(Math.Max(ssxmax, Math.Abs(ssxmin)));
            chart1.ChartAreas[0].AxisY.Interval = 0.25 * roundaxis(Math.Max(ssymax, Math.Abs(ssymin))); ;
        }

        private void RB_agestaff_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_agestaff.Checked)
                updatechart(sender, e);
        }

        private void SpecListButton_Click(object sender, EventArgs e)
        {
            int endyear = getendyear();
            int startyear = getstartyear();
            if (endyear < 0)
                endyear = (from c in db.OV_finance select c.Year).Max();
            if (startyear < 0)
                startyear = endyear - 10;

            FormSelectSpecial fss = new FormSelectSpecial();
            fss.ShowDialog();
            parent.memo("Selected in fss: " + fss.selection);
            string specialstring = fss.selection;
            if (String.IsNullOrEmpty(specialstring))
                return;

            Dictionary<int, Dictionary<string, Dictionary<string, int>>> a1hdict = new Dictionary<int, Dictionary<string, Dictionary<string, int>>>();
            Dictionary<int, Dictionary<string, Dictionary<string, int>>> atotdict = new Dictionary<int, Dictionary<string, Dictionary<string, int>>>();
            Dictionary<string, List<string>> semprog = new Dictionary<string, List<string>>();
            semprog.Add("VT", new List<string>());
            semprog.Add("HT", new List<string>());
            List<string> semlist = new List<string>() { "VT", "HT" };

            for (int year = startyear; year <= endyear; year++)
            {
                a1hdict.Add(year, new Dictionary<string, Dictionary<string, int>>());
                a1hdict[year].Add("VT", new Dictionary<string, int>());
                a1hdict[year].Add("HT", new Dictionary<string, int>());
                atotdict.Add(year, new Dictionary<string, Dictionary<string, int>>());
                atotdict[year].Add("VT", new Dictionary<string, int>());
                atotdict[year].Add("HT", new Dictionary<string, int>());
                var q = from c in db.OV_course
                        where c.Year == year
                        select c;
                if (RB_prog.Checked)
                    q = from c in q where c.Program select c;
                else if (RB_fk.Checked)
                    q = from c in q where !c.Program select c;

                if (!string.IsNullOrEmpty(specialstring))
                {
                    q = dospecialstring(q, specialstring);
                }
                foreach (OV_course oc in q)
                {
                    string ht = oc.HT ? "HT" : "VT";
                    parent.memo(oc.Name + "\t" + oc.OV_University.Name + "\t" + oc.Year + "\t" + ht + "\t" + oc.OV_mysubject.Code+"\t"+oc.Subject+"\t"+oc.Appl1h+"\t"+oc.Appltotal);
                    if (!a1hdict[year][ht].ContainsKey(oc.OV_mysubject.Name))
                    {
                        a1hdict[year][ht].Add(oc.OV_mysubject.Name, oc.Appl1h);
                        atotdict[year][ht].Add(oc.OV_mysubject.Name, oc.Appltotal);
                    }
                    else
                    {
                        a1hdict[year][ht][oc.OV_mysubject.Name] += oc.Appl1h;
                        atotdict[year][ht][oc.OV_mysubject.Name] += oc.Appltotal;
                    }
                    if (!semprog[ht].Contains(oc.OV_mysubject.Name))
                        semprog[ht].Add(oc.OV_mysubject.Name);
                }

            }
            foreach (string sem in semlist)
            {
                StringBuilder sbhead = new StringBuilder(sem);
                foreach (string prog in semprog[sem])
                    sbhead.Append("\t" + prog);
                parent.memo(sbhead.ToString());
                for (int year = startyear; year <= endyear; year++)
                {
                    StringBuilder sb = new StringBuilder(year.ToString());
                    foreach (string prog in semprog[sem])
                    {
                        if (a1hdict[year][sem].ContainsKey(prog))
                            sb.Append("\t" + a1hdict[year][sem][prog]);
                        else
                            sb.Append("\t0");
                    }
                    parent.memo(sb.ToString());
                }
            }
        }
    }
}
