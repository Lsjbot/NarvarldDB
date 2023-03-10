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
    public partial class FormRegional : Form
    {
        private DbTGSAnalysTest db = null;
        private FormDisplay parent = null;
        private FormSelectData selpar = null;
        private Chart chart1 = null;
        private Dictionary<string, int> unidict = new Dictionary<string, int>();

        public FormRegional(DbTGSAnalysTest dbpar, FormDisplay parentpar, FormSelectData selectpar, Chart chartpar, Dictionary<string,int> unidictpar)
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
            unidict = unidictpar;

            LB_lan.Items.Add(" --Riket--");
            foreach (OV_Lan ol in (from c in db.OV_Lan select c))
                LB_lan.Items.Add(ol.Name);

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

        private void Edubutton_Click(object sender, EventArgs e)
        {
            int minyear = (from c in db.OV_demography where c.Year > 0 select c.Year).Min();
            int maxyear = (from c in db.OV_demography select c.Year).Max();
            if (selpar.CB_startyear.SelectedItem != null)
            {
                int minset = util.tryconvert(selpar.CB_startyear.SelectedItem.ToString());
                if (minset > minyear)
                    minyear = minset;
            }
            if (selpar.CB_endyear.SelectedItem != null)
            {
                int maxset = util.tryconvert(selpar.CB_endyear.SelectedItem.ToString());
                if (maxset < maxyear)
                    maxyear = maxset;
            }

            chart1.Series.Clear();
            double ssmax = 0;

            // Set the text of the title
            chart1.Titles["Title1"].Text = "Andel högutbildade per län";
            chart1.ChartAreas[0].AxisY.Title = "Andel högutbildade i befolkningen (%)";
            chart1.Titles["Title2"].Text = FormSelectData.getsource(new string[] { "OV_demography" }, true);

            foreach (OV_Lan ol in (from c in db.OV_Lan select c))
            {
                Series ss = new Series(ol.Name);
                if (ol.Id == 0) //Hela riket
                    ss.BorderWidth = 4;
                //if (ol.Name == "Dalarnas län")
                //    ss.BorderWidth = 5;
                //else
                //    ss.BorderWidth = 2;

                for (int year = minyear; year <= maxyear; year++)
                {

                    var qu = from c in db.OV_demography
                             where c.Year == year
                             where c.Educated == false
                             where c.Lan == ol.Id
                             select c.Number;
                    var qe = from c in db.OV_demography
                             where c.Year == year
                             where c.Educated == true
                             where c.Lan == ol.Id
                             select c.Number;

                    if (qu.Count() == 0)
                        continue;
                    if (qe.Count() == 0)
                        continue;

                    double uneducated = qu.Sum();
                    double educated = qe.Sum();
                    double total = uneducated + educated;

                    double amount = 0;
                    if (total > 0)
                        amount = 100*educated / total;
                    ss.Points.AddXY(year, amount);
                    if (amount > ssmax)
                        ssmax = amount;
                    //if (selpar.CB_memo.Checked)
                    //    parent.memo(year + "\t" + amount);

                }
                if (ss.Points.Count() > 0)
                {
                    ss.ChartType = SeriesChartType.Line;
                    chart1.Series.Add(ss);
                }
            }

            chart1.ChartAreas[0].AxisY.Maximum = ssmax * 1.1;


        }

        private Dictionary<int,List<int>> ageconvert(bool foreign)
        {
            //returns dictionary from UKÄ age classes to SCB.
            //different for education statistics and foreign statistics

            Dictionary<int, List<int>> dict = new Dictionary<int,List<int>>();
            if (foreign)
            {
                dict.Add(1, new List<int>() { 33 });
                dict.Add(2, new List<int>() { 34,35 });
                dict.Add(3, new List<int>() { 36,37 });
            }
            else
            {
                dict.Add(1, new List<int>() { 23 });
                dict.Add(2, new List<int>() { 24 });
                dict.Add(3, new List<int>() { 25 });
            }
            return dict;
        }



        private Dictionary<int,string> fill_kommundict()
        {
            Dictionary<int, string> kommundict = new Dictionary<int, string>();
            var q = from c in db.OV_Kommun select c;
            foreach (OV_Kommun ok in q)
                kommundict.Add(ok.Id, ok.Name);
            return kommundict;
        }


        private Dictionary<int, string> fill_landict()
        {
            Dictionary<int, string> landict = new Dictionary<int, string>();
            var q = from c in db.OV_Lan select c;
            foreach (OV_Lan ok in q)
                landict.Add(ok.Id, ok.Name);
            return landict;
        }

        private void recruitbutton_Click(object sender, EventArgs e)
        {
            Dictionary<int, string> kommundict = fill_kommundict();
            Dictionary<string, SortedDictionary<string,int>> absoluterecruitdict = new Dictionary<string, SortedDictionary<string,int>>();
            Dictionary<string,int> absolutenationaldict = new Dictionary<string,int>();
            
            foreach (string uni in unidict.Keys)
            {
                absoluterecruitdict.Add(uni, new SortedDictionary<string,int>());
                var q = from c in db.OV_recruitkommun where c.Uni == unidict[uni] where c.Age == 0 where c.Gender == 0 where c.Year == 2020 select c;
                if (q.Count() == 0)
                    continue;

                foreach (int kk in kommundict.Keys)
                {
                    var qk = from c in q where c.Kommun == kk select c.Number;
                    int number = qk.Count() > 0 ? qk.Sum() : 0;
                    if (kk == 0) //dubbelräkning av riks- och länssiffror när kommun=0
                    {
                        number = number / 2;
                        absolutenationaldict.Add(uni, number);
                    }
                    else
                        absoluterecruitdict[uni].Add(kommundict[kk], number);
                }
                var sortedDict = from entry in absoluterecruitdict[uni] orderby entry.Value descending select entry;
                int sumothers = 0;
                int n = 0;
                StringBuilder sb = new StringBuilder(uni);
                foreach (KeyValuePair<string,int> c in sortedDict)
                {
                    if (n<5)
                    {
                        sb.Append("\t" + c.Key + "\t" + c.Value);
                    }
                    else
                    {
                        sumothers += c.Value;
                    }
                    n++;
                }
                sb.Append("\tÖvriga\t" + sumothers);
                memo(sb.ToString());
            }
        }

        private void foreignbutton_Click(object sender, EventArgs e)
        {
            Dictionary<int, List<int>> ageconvertdict = ageconvert(true);
            Dictionary<int, string> kommundict = fill_kommundict();
            Dictionary<string, Dictionary<int, double>> yearunidict = new Dictionary<string, Dictionary<int, double>>();

            int startyear = 2017;
            int endyear = 2020;
            foreach (string uni in unidict.Keys)
            {
                yearunidict.Add(uni, new Dictionary<int, double>());
                for (int year = startyear; year <= endyear; year++)
                {
                    yearunidict[uni].Add(year, 0);
                }
            }
            for (int year = startyear; year <=endyear; year++)
            {
                memo("Year = " + year);
                Dictionary<int, Dictionary<int, Dictionary<int, double>>> foreignkommundict = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>(); //kommun,age,gender,fraction
                Dictionary<int, Dictionary<int, Dictionary<int, double>>> nativekommundict = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>(); //kommun,age,gender,fraction
                Dictionary<int, Dictionary<int, Dictionary<int, double>>> foreignfrackommundict = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>(); //kommun,age,gender,fraction

                memo("Filling foreignfrackommundict");
                foreach (int ik in kommundict.Keys)
                {
                    Console.WriteLine(kommundict[ik]);
                    if (ik == 0) //total
                        continue;
                    if (ik == 999) //okänd
                        continue;
                    foreignkommundict.Add(ik, new Dictionary<int, Dictionary<int, double>>());
                    nativekommundict.Add(ik, new Dictionary<int, Dictionary<int, double>>());
                    foreignfrackommundict.Add(ik, new Dictionary<int, Dictionary<int, double>>());
                    foreach (int ia in ageconvertdict.Keys) //age groups UKÄ
                    {
                        foreignkommundict[ik].Add(ia, new Dictionary<int, double>());
                        foreignkommundict[ik][ia].Add(1, 0);
                        foreignkommundict[ik][ia].Add(2, 0);
                        nativekommundict[ik].Add(ia, new Dictionary<int, double>());
                        nativekommundict[ik][ia].Add(1, 0);
                        nativekommundict[ik][ia].Add(2, 0);
                        foreignfrackommundict[ik].Add(ia, new Dictionary<int, double>());
                        foreach (int ia2 in ageconvertdict[ia]) //age groups SCB
                        {
                            for (int ig = 1; ig <= 2; ig++) //genders
                            {
                                var qq = from c in db.OV_demographykommun where c.Kommun == ik where c.Age == ia2 where c.Gender == ig where c.Year == year where c.Foreignbackground != null select c;
                                double foreigners = (from c in qq where c.Foreignbackground == 6 select c.Number).Sum();
                                double natives = (from c in qq where c.Foreignbackground == 7 select c.Number).Sum();
                                foreignkommundict[ik][ia][ig] += foreigners;
                                nativekommundict[ik][ia][ig] += natives;
                            }
                        }
                        foreignfrackommundict[ik][ia].Add(1, foreignkommundict[ik][ia][1] / (foreignkommundict[ik][ia][1] + nativekommundict[ik][ia][1]));
                        foreignfrackommundict[ik][ia].Add(2, foreignkommundict[ik][ia][2] / (foreignkommundict[ik][ia][2] + nativekommundict[ik][ia][2]));
                    }

                }
                memo("Done filling foreignfrackommundict");

                foreach (string uni in unidict.Keys)
                {
                    Console.WriteLine(uni);
                    double wsum = 0;
                    double sum = 0;
                    var q = from c in db.OV_recruitkommun where c.Uni == unidict[uni] where c.Year == year where c.Gender > 0 where c.Age > 0 select c;
                    foreach (OV_recruitkommun ok in q)
                    {
                        if (ok.Kommun == 0) //total
                            continue;
                        if (ok.Kommun == 999) //okänd
                            continue;
                        double w = foreignfrackommundict[ok.Kommun][ok.Age][ok.Gender];
                        wsum += w * ok.Number;
                        sum += ok.Number;
                    }
                    if (sum > 0)
                    {
                        double foreignfrac = wsum / sum;
                        memo(uni + "\t" + foreignfrac);
                        yearunidict[uni][year] = foreignfrac;
                    }
                }
            }

            StringBuilder ysb = new StringBuilder();
            for (int year = startyear; year <= endyear; year++)
                ysb.Append("\t" + year);
            memo(ysb.ToString());
            foreach (string uni in unidict.Keys)
            {
                StringBuilder sb = new StringBuilder(uni);
                for (int year = startyear; year <= endyear; year++)
                    sb.Append("\t" + yearunidict[uni][year]);
                memo(sb.ToString());
            }
        }

        private void educationbutton_Click(object sender, EventArgs e)
        {
            Dictionary<int, List<int>> ageconvertdict = ageconvert(false);
            Dictionary<int, string> kommundict = fill_kommundict();
            Dictionary<string, Dictionary<int, double>> yearunidict = new Dictionary<string, Dictionary<int, double>>();

            int startyear = 2018;
            int endyear = 2020;
            foreach (string uni in unidict.Keys)
            {
                yearunidict.Add(uni, new Dictionary<int, double>());
                for (int year = startyear; year <= endyear; year++)
                {
                    yearunidict[uni].Add(year, 0);
                }
            }
            for (int year = startyear; year <= endyear; year++)
            {
                memo("Year = " + year);
                Dictionary<int, Dictionary<int, Dictionary<int, double>>> lowkommundict = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>(); //kommun,age,gender,fraction
                Dictionary<int, Dictionary<int, Dictionary<int, double>>> highkommundict = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>(); //kommun,age,gender,fraction
                Dictionary<int, Dictionary<int, Dictionary<int, double>>> lowfrackommundict = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>(); //kommun,age,gender,fraction

                memo("Filling lowedufrackommundict");
                foreach (int ik in kommundict.Keys)
                {
                    Console.WriteLine(kommundict[ik]);
                    if (ik == 0) //total
                        continue;
                    if (ik == 999) //okänd
                        continue;
                    lowkommundict.Add(ik, new Dictionary<int, Dictionary<int, double>>());
                    highkommundict.Add(ik, new Dictionary<int, Dictionary<int, double>>());
                    lowfrackommundict.Add(ik, new Dictionary<int, Dictionary<int, double>>());
                    foreach (int ia in ageconvertdict.Keys) //age groups UKÄ
                    {
                        lowkommundict[ik].Add(ia, new Dictionary<int, double>());
                        lowkommundict[ik][ia].Add(1, 0);
                        lowkommundict[ik][ia].Add(2, 0);
                        highkommundict[ik].Add(ia, new Dictionary<int, double>());
                        highkommundict[ik][ia].Add(1, 0);
                        highkommundict[ik][ia].Add(2, 0);
                        lowfrackommundict[ik].Add(ia, new Dictionary<int, double>());
                        foreach (int ia2 in ageconvertdict[ia]) //age groups SCB
                        {
                            for (int ig = 1; ig <= 2; ig++) //genders
                            {
                                var qq = from c in db.OV_demographykommun where c.Kommun == ik where c.Age == ia2 where c.Gender == ig where c.Year == year where c.Educated != null select c;
                                double lowedu = (from c in qq where !(bool)c.Educated select c.Number).Sum();
                                double highedu = (from c in qq where (bool)c.Educated select c.Number).Sum();
                                lowkommundict[ik][ia][ig] += lowedu;
                                highkommundict[ik][ia][ig] += highedu;
                            }
                        }
                        lowfrackommundict[ik][ia].Add(1, lowkommundict[ik][ia][1] / (lowkommundict[ik][ia][1] + highkommundict[ik][ia][1]));
                        lowfrackommundict[ik][ia].Add(2, lowkommundict[ik][ia][2] / (lowkommundict[ik][ia][2] + highkommundict[ik][ia][2]));
                    }

                }
                memo("Done filling lowfrackommundict");

                foreach (string uni in unidict.Keys)
                {
                    Console.WriteLine(uni);
                    double wsum = 0;
                    double sum = 0;
                    var q = from c in db.OV_recruitkommun where c.Uni == unidict[uni] where c.Year == year where c.Gender > 0 where c.Age > 0 select c;
                    foreach (OV_recruitkommun ok in q)
                    {
                        if (ok.Kommun == 0) //total
                            continue;
                        if (ok.Kommun == 999) //okänd
                            continue;
                        double w = lowfrackommundict[ok.Kommun][ok.Age][ok.Gender];
                        wsum += w * ok.Number;
                        sum += ok.Number;
                    }
                    if (sum > 0)
                    {
                        double lowfrac = wsum / sum;
                        memo(uni + "\t" + lowfrac);
                        yearunidict[uni][year] = lowfrac;
                    }
                }
            }

            StringBuilder ysb = new StringBuilder();
            for (int year = startyear; year <= endyear; year++)
                ysb.Append("\t" + year);
            memo(ysb.ToString());
            foreach (string uni in unidict.Keys)
            {
                StringBuilder sb = new StringBuilder(uni);
                for (int year = startyear; year <= endyear; year++)
                    sb.Append("\t" + yearunidict[uni][year]);
                memo(sb.ToString());
            }

        }

        private void Transitionbutton_Click(object sender, EventArgs e)
        {
            Dictionary<int, string> kommundict = fill_kommundict();
            Dictionary<string, Dictionary<int,Dictionary<int, double>>> yearunifracdict = new Dictionary<string, Dictionary<int,Dictionary<int, double>>>();
            Dictionary<string, Dictionary<int, Dictionary<int, double>>> yearunistuddict = new Dictionary<string, Dictionary<int, Dictionary<int, double>>>();

            int startyear = 2014;
            int endyear = 2017;
            foreach (string uni in unidict.Keys)
            {
                yearunifracdict.Add(uni, new Dictionary<int,Dictionary<int, double>>());
                yearunistuddict.Add(uni, new Dictionary<int,Dictionary<int, double>>());
                for (int year = startyear; year <= endyear; year++)
                {
                    yearunifracdict[uni].Add(year, new Dictionary<int,double>());
                    yearunistuddict[uni].Add(year, new Dictionary<int,double>());
                    yearunifracdict[uni][year].Add(0, 0);
                    yearunifracdict[uni][year].Add(1, 0);
                    yearunifracdict[uni][year].Add(2, 0);
                    yearunistuddict[uni][year].Add(0, 0);
                    yearunistuddict[uni][year].Add(1, 0);
                    yearunistuddict[uni][year].Add(2, 0);
                }
            }

            var otall = (from c in db.OV_transition
                                select c).ToList();

            for (int year = startyear; year <= endyear; year++)
            {
                memo("Year " + year);
                foreach (string uni in unidict.Keys)
                {
                    memo(uni);
                    Console.WriteLine(year + " " + uni);
                    var qr = from c in db.OV_recruitkommun
                             where c.Year == year+3 //recruit 3 years after highschool graduation
                             where c.Uni == unidict[uni]
                             where c.Age == 0
                             select c;
                    foreach (OV_recruitkommun ork in qr)
                    {
                        if (ork.Kommun == 0 || ork.Kommun == 999)
                            continue;
                        yearunistuddict[uni][year][ork.Gender] += ork.Number;
                        OV_transition ot = (from c in otall
                                            where c.Kommun == ork.Kommun
                                            where c.Gender == ork.Gender
                                            where c.Year == year
                                            select c).First();
                        yearunifracdict[uni][year][ork.Gender] += ot.Fraction*ork.Number;

                    }
                }
            }

            StringBuilder ysb = new StringBuilder();
            for (int year = startyear; year <= endyear; year++)
                ysb.Append("\t" + year);
            memo(ysb.ToString());
            foreach (string uni in unidict.Keys)
            {
                StringBuilder sb = new StringBuilder(uni);
                for (int year = startyear; year <= endyear; year++)
                {
                    double meanfrac = (yearunifracdict[uni][year][1] + yearunifracdict[uni][year][2]) / (yearunistuddict[uni][year][1] + yearunistuddict[uni][year][2]);
                    sb.Append("\t" + meanfrac);
                }
                memo(sb.ToString());
            }


        }

        private Dictionary<string,string> fill_unishort()
        {
            Dictionary<string, string> unishortdict = new Dictionary<string, string>();
            unishortdict.Add("--Hela riket--", "Riket");
            unishortdict.Add("Beckmans designhögskola", "Beckm");
            unishortdict.Add("Blekinge tekniska högskola", "BTH");
            unishortdict.Add("Chalmers tekniska högskola", "CTH");
            unishortdict.Add("Ericastiftelsen", "Erica");
            unishortdict.Add("Försvarshögskolan", "FH");
            unishortdict.Add("Gammelkroppa skogsskola", "GSS");
            unishortdict.Add("Gymnastik- och idrottshögskolan", "GIH");
            unishortdict.Add("Göteborgs universitet", "GU");
            unishortdict.Add("Handelshögskolan i Stockholm", "HHS");
            unishortdict.Add("Högskolan Dalarna", "HDa");
            unishortdict.Add("Högskolan Kristianstad", "HKr");
            unishortdict.Add("Högskolan Väst", "HV");
            unishortdict.Add("Högskolan i Borås", "HB");
            unishortdict.Add("Högskolan i Gävle", "HiG");
            unishortdict.Add("Högskolan i Halmstad", "HHS");
            unishortdict.Add("Högskolan i Jönköping", "HJ");
            unishortdict.Add("Högskolan i Skövde", "HiS");
            unishortdict.Add("Johannelunds teologiska högskola", "JTH");
            unishortdict.Add("Karlstads universitet", "KaU");
            unishortdict.Add("Karolinska institutet", "KI");
            unishortdict.Add("Konstfack", "Kf");
            unishortdict.Add("Kungl. Konsthögskolan", "KKH");
            unishortdict.Add("Kungl. Musikhögskolan i Stockholm", "KMS");
            unishortdict.Add("Kungl. Tekniska högskolan", "KTH");
            unishortdict.Add("Linköpings universitet", "LiU");
            unishortdict.Add("Linnéuniversitetet", "Lnu");
            unishortdict.Add("Luleå tekniska universitet", "LTU");
            unishortdict.Add("Lunds universitet", "LU");
            unishortdict.Add("Mittuniversitetet", "MiUn");
            unishortdict.Add("Mälardalens högskola", "MdH");
            unishortdict.Add("Newmaninstitutet", "Newm");
            unishortdict.Add("Röda Korsets högskola", "RK");
            unishortdict.Add("Sophiahemmet högskola", "Sophia");
            unishortdict.Add("Stockholms Musikpedagogiska Institut", "SMI");
            unishortdict.Add("Stockholms konstnärliga högskola", "SKH");
            unishortdict.Add("Stockholms universitet", "SU");
            unishortdict.Add("Sveriges lantbruksuniversitet", "SLU");
            unishortdict.Add("Södertörns högskola", "SH");
            unishortdict.Add("Teologiska Högskolan Stockholm", "THS");
            unishortdict.Add("Umeå universitet", "UmU");
            unishortdict.Add("Uppsala universitet", "UU");
            unishortdict.Add("Örebro teologiska högskola", "ÖTH");
            unishortdict.Add("Örebro universitet", "ÖrU");
            unishortdict.Add("Övr. enskilda anordn. psykoterapeututb.", "Övr");
            unishortdict.Add("Malmö universitet", "MaU");
            unishortdict.Add("Ersta Sköndal Bräcke högskola", "ESBH");
            return unishortdict;
        }

        private void latlongbutton_Click(object sender, EventArgs e)
        {
            Dictionary<int, double> kommunlat = new Dictionary<int, double>();
            Dictionary<int, double> kommunlon = new Dictionary<int, double>();
            Dictionary<string, string> unishortdict = fill_unishort();
            foreach (OV_Kommun ok in (from c in db.OV_Kommun select c))
                if (ok.Lat != null)
                {
                    kommunlat.Add(ok.Id, (double)ok.Lat);
                    kommunlon.Add(ok.Id, (double)ok.Lon);
                }


            foreach (string uni in unidict.Keys)
            {
                double stud = 0;
                double sumlat = 0;
                double sumlat2 = 0;
                double sumlon = 0;
                double sumlon2 = 0;
                double sumlatlon = 0;

                var qr = from c in db.OV_recruitkommun
                         where c.Year == 2020
                         where c.Uni == unidict[uni]
                         where c.Age == 0
                         where c.Gender == 0
                         select c;

                foreach (OV_recruitkommun ork in qr)
                {
                    if (ork.Kommun == 0 || ork.Kommun == 999)
                        continue;
                    stud += ork.Number;
                    sumlat += ork.Number * kommunlat[ork.Kommun];
                    sumlat2 += ork.Number * kommunlat[ork.Kommun] * kommunlat[ork.Kommun];
                    sumlon += ork.Number * kommunlon[ork.Kommun];
                    sumlon2 += ork.Number * kommunlon[ork.Kommun] * kommunlon[ork.Kommun];
                    sumlatlon += ork.Number * kommunlat[ork.Kommun] * kommunlon[ork.Kommun];


                }

                double kmdeg = 10000 / 90;
                double lat = sumlat / stud;
                double lon = sumlon / stud;
                double siglat = Math.Sqrt(sumlat2 / stud - lat * lat);
                double siglatkm = siglat * kmdeg;
                double siglon = Math.Sqrt(sumlon2 / stud - lon * lon);
                double siglonkm = siglon * kmdeg * Math.Cos(lat * 3.14 / 180);
                double sigsig = Math.Sqrt(siglatkm * siglatkm + siglonkm * siglonkm);

                //{{Location map~|India|label=santali|label_size=177|mark=X sheer black 17.gif|marksize=8|position=right|background=white|lat=23.7291|long=86.6919}}

                string mappoint = sigsig > 0 ? "{{Location map~|Sweden|label="+unishortdict[uni]+"|label_size=177|mark=Light-blue pog.svg|marksize="+Math.Round(0.25*sigsig)+"|position=right|background=white|lat="+lat+"|long="+lon+"}}" : "";

                memo(uni + "\t" + lat + "\t" + lon + "\t" + siglatkm + "\t" + siglonkm + "\t" + sigsig+"\t"+mappoint); 
            }
        }

        private void agebutton_Click(object sender, EventArgs e)
        {
            int startyear = 2008;
            int endyear = 2020;
            Dictionary<string, Dictionary<int, Dictionary<int, double>>> ageunidict = new Dictionary<string, Dictionary<int, Dictionary<int, double>>>();
            foreach (string uni in unidict.Keys)
            {
                ageunidict.Add(uni, new Dictionary<int, Dictionary<int, double>>());
                for (int year = startyear; year <= endyear; year++)
                {
                    ageunidict[uni].Add(year, new Dictionary<int, double>());
                    ageunidict[uni][year].Add(0, 0);
                    ageunidict[uni][year].Add(1, 0);
                    ageunidict[uni][year].Add(2, 0);
                    ageunidict[uni][year].Add(3, 0);
                }
            }

            for (int year = startyear; year <= endyear; year++)
            {
                memo("Year " + year);
                foreach (string uni in unidict.Keys)
                {
                    memo(uni);
                    Console.WriteLine(year + " " + uni);
                    var qr = from c in db.OV_recruitkommun
                             where c.Year == year
                             where c.Uni == unidict[uni]
                             where c.Gender == 0
                             where c.Lan == 0
                             select c;
                    foreach (OV_recruitkommun ork in qr)
                    {
                        if (ork.Kommun == 999)
                            continue;
                        ageunidict[uni][year][ork.Age] += ork.Number;

                    }
                }
            }

            StringBuilder sbhead = new StringBuilder("");
            for (int year = startyear; year <= endyear; year++)
            {
                sbhead.Append("\t" + year);
            }
            memo(sbhead.ToString());
            foreach (string uni in unidict.Keys)
            {
                StringBuilder sb = new StringBuilder(uni);
                for (int year = startyear; year <= endyear; year++)
                {
                    double index = (20*ageunidict[uni][year][1] + 30*ageunidict[uni][year][2] + 40* ageunidict[uni][year][3]) / ageunidict[uni][year][0];
                    sb.Append("\t" + index);
                }
                memo(sb.ToString());
            }

            memo(sbhead.ToString());
            foreach (string uni in unidict.Keys)
            {
                StringBuilder sb = new StringBuilder(uni);
                for (int year = startyear; year <= endyear; year++)
                {
                    double index = 0;
                    for (int i = 1; i < 4; i++)
                        index += (ageunidict[uni][year][i] / ageunidict[uni][year][0] - 0.3333) * (ageunidict[uni][year][i] / ageunidict[uni][year][0] - 0.3333);
                    sb.Append("\t" + index);
                }
                memo(sb.ToString());
            }



        }

        private void button1_Click(object sender, EventArgs e)
            //marknadsandel per län och kommun
        {
            int uni = selpar.focusuniversity;

            int year = (from c in db.OV_recruitkommun select c.Year).Max();

            foreach (OV_Lan ol in db.OV_Lan)
            {
                var q = from c in db.OV_recruitkommun
                        where c.Year == year
                        where c.Gender == 0
                        where c.Age == 0
                        where c.Kommun == 0
                        where c.Lan == ol.Id
                        select c;
                double nfocus = (from c in q where c.Uni == uni select c.Number).First();
                double ntot = (from c in q where c.Uni == 0 select c.Number).First();
                double share = nfocus / ntot;
                parent.memo(ol.Name + "\t" + 100*share);

                foreach (OV_Kommun ok in ol.OV_Kommun)
                {
                    var qq = from c in db.OV_recruitkommun
                            where c.Year == year
                            where c.Gender == 0
                            where c.Age == 0
                            where c.Kommun == ok.Id
                            //where c.Lan == ol.Id
                            select c;
                    var qqq = from c in qq where c.Uni == uni select c.Number;

                    double nfocus2 = qqq.Count() > 0 ? qqq.First() : 0;
                    var qq0 = from c in qq where c.Uni == 0 select c.Number;
                    double ntot2 = qq0.Count()>0?qq0.First():0;
                    double share2 = nfocus2 / ntot2;
                    parent.memo(ok.Name + "\t" + 100 * share2);

                }
            }
        }
    }
}
