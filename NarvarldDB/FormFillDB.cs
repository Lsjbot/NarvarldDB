using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;       //Microsoft Excel 14 object in references-> COM tab


namespace NarvarldDB
{
    public partial class FormFillDB : Form
    {
        DbTGSAnalysTest db = null;


        //static string nvfolder = @"\\dustaff\home\sja\Documents\Närvärld\";
        static string nvfolder = @"C:\Users\sja\OneDrive - Högskolan Dalarna\Dokument\Närvärld\";
        //static string ukafolder = nvfolder + @"UKÄ-statistik -2017\";
        static string ukafolder = nvfolder + @"UKÄ-statistik\";
        static string swepubfolder = nvfolder + @"Bibliometri Swepub\";
        static string bakframgrundfolder = @"\\dustaff\home\sja\Documents\bakframgrund\";

        Dictionary<string, universityclass> unidict = new Dictionary<string, universityclass>();
        Dictionary<int, string> uniindex = new Dictionary<int, string>();

        class universityclass
        {
            public int number = -1;
            public List<string> kommun = new List<string>(); //Kommun(er) där lärosätet ligger
            public List<string> lan = new List<string>();    //Län där lärosätet ligger
            public string merged_with = ""; //Namn på lärosäte som detta har uppgått i eller bytt namn till
            public double lat = 0.0; //latitud
            public double lon = 0.0; //longitud
        }

        public FormFillDB(DbTGSAnalysTest dbpar)
        {
            InitializeComponent();
            db = dbpar;
        }

        public int register_fileentry(string fn)
        {
            int id = (from c in db.Datafiles select c.Id).Max() + 1;
            Datafiles df = new Datafiles();
            df.Id = id;
            df.Filename = fn;
            df.Entrydate = DateTime.Now;
            df.Filemodified = File.GetLastWriteTime(fn);
            db.Datafiles.InsertOnSubmit(df);
            db.SubmitChanges();
            memo(fn + " registered as done.");
            return id;
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

        private void Kommunbutton_Click(object sender, EventArgs e)
        {
            List<int> landone = (from c in db.OV_Lan select c.Id).ToList();

            Dictionary<int, string> landict = new Dictionary<int, string>();
            landict.Add(01, "Stockholms län");
            landict.Add(03, "Uppsala län");
            landict.Add(04, "Södermanlands län");
            landict.Add(05, "Östergötlands län");
            landict.Add(06, "Jönköpings län");
            landict.Add(07, "Kronobergs län");
            landict.Add(08, "Kalmar län");
            landict.Add(09, "Gotlands län");
            landict.Add(10, "Blekinge län");
            landict.Add(12, "Skåne län");
            landict.Add(13, "Hallands län");
            landict.Add(14, "Västra Götalands län");
            landict.Add(17, "Värmlands län");
            landict.Add(18, "Örebro län");
            landict.Add(19, "Västmanlands län");
            landict.Add(20, "Dalarnas län");
            landict.Add(21, "Gävleborgs län");
            landict.Add(22, "Västernorrlands län");
            landict.Add(23, "Jämtlands län");
            landict.Add(24, "Västerbottens län");
            landict.Add(25, "Norrbottens län");
            foreach (int id in landict.Keys)
            {
                if (!landone.Contains(id))
                {
                    OV_Lan ol = new OV_Lan();
                    ol.Id = id;
                    ol.Name = landict[id];
                    db.OV_Lan.InsertOnSubmit(ol);
                }
            }
            db.SubmitChanges();

            memo("Län done");

            List<int> kommundone = (from c in db.OV_Kommun select c.Id).ToList();
            

            int n = 0;
            using (StreamReader sr = new StreamReader(nvfolder + "kommuner-scb - Copy.txt"))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();


                    //if (n > 250)
                    //    Console.WriteLine(line);

                    string[] words = line.Split('\t');

                    OV_Kommun kc = new OV_Kommun();

                    int id = util.tryconvert(words[2]);
                    if (id <= 0)
                        continue;
                    if (kommundone.Contains(id))
                        continue;
                    kc.Id = id;
                    kc.Lan = util.tryconvert(words[3]);
                    kc.Name = words[4];
                    kc.Lat = (float)util.tryconvertdouble(words[7]);
                    kc.Lon = (float)util.tryconvertdouble(words[8]);
                    kc.Population = util.tryconvert(words[1]);

                    db.OV_Kommun.InsertOnSubmit(kc);

                    n++;
                    if ((n % 10) == 0)
                    {
                        Console.WriteLine("n (adm1)   = " + n.ToString());

                    }

                }
                db.SubmitChanges();

                Console.WriteLine("n    (adm1)= " + n.ToString());
                register_fileentry(nvfolder + "kommuner-scb - Copy.txt");
            }

            memo("Kommuner done");

            foreach (OV_Lan ol in (from c in db.OV_Lan select c))
            {
                ol.Population = (from c in db.OV_Kommun where c.Lan == ol.Id select c.Population).Sum();
            }
            db.SubmitChanges();
            memo("Län population updated");

        }

        public int university_alias(string alias,string truename, int nuni)
        {
            universityclass uu0 = new universityclass(); uu0.number = nuni; unidict.Add(alias, uu0); uniindex.Add(nuni, alias); nuni++;
            unidict[alias].merged_with = truename;
            return nuni + 1;
        }

        public void university_data()
        {


            int nuni = 0;
            universityclass uu0 = new universityclass(); uu0.number = nuni; unidict.Add("Total", uu0); uniindex.Add(nuni, "Total"); nuni++;
            universityclass uu1 = new universityclass(); uu1.number = nuni; unidict.Add("Blekinge internationella hälsohögskola", uu1); uniindex.Add(nuni, "Blekinge internationella hälsohögskola"); nuni++;
            universityclass uu2 = new universityclass(); uu2.number = nuni; unidict.Add("Ersta högskola", uu2); uniindex.Add(nuni, "Ersta högskola"); nuni++;
            universityclass uu3 = new universityclass(); uu3.number = nuni; unidict.Add("Stiftelsen Stora Sköndal", uu3); uniindex.Add(nuni, "Stiftelsen Stora Sköndal"); nuni++;
            universityclass uu4 = new universityclass(); uu4.number = nuni; unidict.Add("Vårdhögskolan i Göteborg", uu4); uniindex.Add(nuni, "Vårdhögskolan i Göteborg"); nuni++;
            universityclass uu5 = new universityclass(); uu5.number = nuni; unidict.Add("Vårdhögskolan Falun", uu5); uniindex.Add(nuni, "Vårdhögskolan Falun"); nuni++;
            universityclass uu6 = new universityclass(); uu6.number = nuni; unidict.Add("Vårdhögskolan i Borås", uu6); uniindex.Add(nuni, "Vårdhögskolan i Borås"); nuni++;
            universityclass uu7 = new universityclass(); uu7.number = nuni; unidict.Add("Vårdhögskolan Gävle", uu7); uniindex.Add(nuni, "Vårdhögskolan Gävle"); nuni++;
            universityclass uu8 = new universityclass(); uu8.number = nuni; unidict.Add("Vårdhögskolan i Halland", uu8); uniindex.Add(nuni, "Vårdhögskolan i Halland"); nuni++;
            universityclass uu9 = new universityclass(); uu9.number = nuni; unidict.Add("Hälsohögskolan i Jönköping", uu9); uniindex.Add(nuni, "Hälsohögskolan i Jönköping"); nuni++;
            universityclass uu10 = new universityclass(); uu10.number = nuni; unidict.Add("Hälsohögskolan Väst, Skövde", uu10); uniindex.Add(nuni, "Hälsohögskolan Väst, Skövde"); nuni++;
            universityclass uu11 = new universityclass(); uu11.number = nuni; unidict.Add("Vårdhögskolan Kristianstad", uu11); uniindex.Add(nuni, "Vårdhögskolan Kristianstad"); nuni++;
            universityclass uu12 = new universityclass(); uu12.number = nuni; unidict.Add("Bohusläns vårdhögskola", uu12); uniindex.Add(nuni, "Bohusläns vårdhögskola"); nuni++;
            universityclass uu13 = new universityclass(); uu13.number = nuni; unidict.Add("Hälsohögskolan Väst i Vänersborg", uu13); uniindex.Add(nuni, "Hälsohögskolan Väst i Vänersborg"); nuni++;
            universityclass uu14 = new universityclass(); uu14.number = nuni; unidict.Add("Vårdhögskolan i Vänersborg", uu14); uniindex.Add(nuni, "Vårdhögskolan i Vänersborg"); nuni++;
            universityclass uu15 = new universityclass(); uu15.number = nuni; unidict.Add("Hälsohögskolan i Värmland", uu15); uniindex.Add(nuni, "Hälsohögskolan i Värmland"); nuni++;
            universityclass uu16 = new universityclass(); uu16.number = nuni; unidict.Add("Ingesunds Musikhögskola", uu16); uniindex.Add(nuni, "Ingesunds Musikhögskola"); nuni++;
            universityclass uu17 = new universityclass(); uu17.number = nuni; unidict.Add("Hälsohögskolan i Stockholm", uu17); uniindex.Add(nuni, "Hälsohögskolan i Stockholm"); nuni++;
            universityclass uu18 = new universityclass(); uu18.number = nuni; unidict.Add("Hälsouniversitetet i Linköping", uu18); uniindex.Add(nuni, "Hälsouniversitetet i Linköping"); nuni++;
            universityclass uu19 = new universityclass(); uu19.number = nuni; unidict.Add("Högskolan i Kalmar", uu19); uniindex.Add(nuni, "Högskolan i Kalmar"); nuni++;
            universityclass uu20 = new universityclass(); uu20.number = nuni; unidict.Add("Kalmar läns vårdhögskola", uu20); uniindex.Add(nuni, "Kalmar läns vårdhögskola"); nuni++;
            universityclass uu21 = new universityclass(); uu21.number = nuni; unidict.Add("Växjö universitet", uu21); uniindex.Add(nuni, "Växjö universitet"); nuni++;
            universityclass uu22 = new universityclass(); uu22.number = nuni; unidict.Add("Vårdhögskolan i Växjö", uu22); uniindex.Add(nuni, "Vårdhögskolan i Växjö"); nuni++;
            universityclass uu23 = new universityclass(); uu23.number = nuni; unidict.Add("Vårdhögskolan Boden", uu23); uniindex.Add(nuni, "Vårdhögskolan Boden"); nuni++;
            universityclass uu24 = new universityclass(); uu24.number = nuni; unidict.Add("Vårdhögskolan Lund/Helsingborg", uu24); uniindex.Add(nuni, "Vårdhögskolan Lund/Helsingborg"); nuni++;
            universityclass uu25 = new universityclass(); uu25.number = nuni; unidict.Add("Vårdhögskolan i Malmö", uu25); uniindex.Add(nuni, "Vårdhögskolan i Malmö"); nuni++;
            universityclass uu26 = new universityclass(); uu26.number = nuni; unidict.Add("Vårdhögskolan i Sundsvall/Ö-vik", uu26); uniindex.Add(nuni, "Vårdhögskolan i Sundsvall/Ö-vik"); nuni++;
            universityclass uu27 = new universityclass(); uu27.number = nuni; unidict.Add("Vårdhögskolan i Östersund", uu27); uniindex.Add(nuni, "Vårdhögskolan i Östersund"); nuni++;
            universityclass uu28 = new universityclass(); uu28.number = nuni; unidict.Add("Vårdhögskolan i Eskilstuna", uu28); uniindex.Add(nuni, "Vårdhögskolan i Eskilstuna"); nuni++;
            universityclass uu29 = new universityclass(); uu29.number = nuni; unidict.Add("Vårdhögskolan i Västerås", uu29); uniindex.Add(nuni, "Vårdhögskolan i Västerås"); nuni++;
            universityclass uu30 = new universityclass(); uu30.number = nuni; unidict.Add("Dans- och cirkushögskolan", uu30); uniindex.Add(nuni, "Dans- och cirkushögskolan"); nuni++;
            universityclass uu31 = new universityclass(); uu31.number = nuni; unidict.Add("Dramatiska institutet", uu31); uniindex.Add(nuni, "Dramatiska institutet"); nuni++;
            universityclass uu32 = new universityclass(); uu32.number = nuni; unidict.Add("Lärarhögskolan i Stockholm", uu32); uniindex.Add(nuni, "Lärarhögskolan i Stockholm"); nuni++;
            universityclass uu33 = new universityclass(); uu33.number = nuni; unidict.Add("Operahögskolan i Stockholm", uu33); uniindex.Add(nuni, "Operahögskolan i Stockholm"); nuni++;
            universityclass uu34 = new universityclass(); uu34.number = nuni; unidict.Add("Stockholms dramatiska högskola", uu34); uniindex.Add(nuni, "Stockholms dramatiska högskola"); nuni++;
            universityclass uu35 = new universityclass(); uu35.number = nuni; unidict.Add("Teaterhögskolan i Stockholm", uu35); uniindex.Add(nuni, "Teaterhögskolan i Stockholm"); nuni++;
            universityclass uu36 = new universityclass(); uu36.number = nuni; unidict.Add("Grafiska institutet/IHR", uu36); uniindex.Add(nuni, "Grafiska institutet/IHR"); nuni++;
            universityclass uu37 = new universityclass(); uu37.number = nuni; unidict.Add("Hälsohögskolan i Umeå", uu37); uniindex.Add(nuni, "Hälsohögskolan i Umeå"); nuni++;
            universityclass uu38 = new universityclass(); uu38.number = nuni; unidict.Add("Högskolan på Gotland", uu38); uniindex.Add(nuni, "Högskolan på Gotland"); nuni++;
            universityclass uu39 = new universityclass(); uu39.number = nuni; unidict.Add("Vårdhögskolan i Uppsala", uu39); uniindex.Add(nuni, "Vårdhögskolan i Uppsala"); nuni++;
            universityclass uu40 = new universityclass(); uu40.number = nuni; unidict.Add("Vårdhögskolan i Örebro", uu40); uniindex.Add(nuni, "Vårdhögskolan i Örebro"); nuni++;
            universityclass uu41 = new universityclass(); uu41.number = nuni; unidict.Add("Beckmans designhögskola", uu41); uniindex.Add(nuni, "Beckmans designhögskola"); nuni++;
            universityclass uu42 = new universityclass(); uu42.number = nuni; unidict.Add("Blekinge tekniska högskola", uu42); uniindex.Add(nuni, "Blekinge tekniska högskola"); nuni++;
            universityclass uu43 = new universityclass(); uu43.number = nuni; unidict.Add("Chalmers tekniska högskola", uu43); uniindex.Add(nuni, "Chalmers tekniska högskola"); nuni++;
            universityclass uu44 = new universityclass(); uu44.number = nuni; unidict.Add("Ericastiftelsen", uu44); uniindex.Add(nuni, "Ericastiftelsen"); nuni++;
            universityclass uu45 = new universityclass(); uu45.number = nuni; unidict.Add("Ersta Sköndal högskola", uu45); uniindex.Add(nuni, "Ersta Sköndal högskola"); nuni++;
            universityclass uu46 = new universityclass(); uu46.number = nuni; unidict.Add("Försvarshögskolan", uu46); uniindex.Add(nuni, "Försvarshögskolan"); nuni++;
            universityclass uu46b = new universityclass(); uu46b.number = nuni; unidict.Add("Försvarshögskolan Stockholm", uu46b); uniindex.Add(nuni, "Försvarshögskolan Stockholm"); nuni++;
            universityclass uu47 = new universityclass(); uu47.number = nuni; unidict.Add("Gammelkroppa skogsskola", uu47); uniindex.Add(nuni, "Gammelkroppa skogsskola"); nuni++;
            universityclass uu48 = new universityclass(); uu48.number = nuni; unidict.Add("Gymnastik- och idrottshögskolan", uu48); uniindex.Add(nuni, "Gymnastik- och idrottshögskolan"); nuni++;
            universityclass uu49 = new universityclass(); uu49.number = nuni; unidict.Add("Göteborgs universitet", uu49); uniindex.Add(nuni, "Göteborgs universitet"); nuni++;
            universityclass uu50 = new universityclass(); uu50.number = nuni; unidict.Add("Handelshögskolan i Stockholm", uu50); uniindex.Add(nuni, "Handelshögskolan i Stockholm"); nuni++;
            universityclass uu51 = new universityclass(); uu51.number = nuni; unidict.Add("Högskolan Dalarna", uu51); uniindex.Add(nuni, "Högskolan Dalarna"); nuni++;
            universityclass uu52 = new universityclass(); uu52.number = nuni; unidict.Add("Högskolan Kristianstad", uu52); uniindex.Add(nuni, "Högskolan Kristianstad"); nuni++;
            universityclass uu53 = new universityclass(); uu53.number = nuni; unidict.Add("Högskolan Väst", uu53); uniindex.Add(nuni, "Högskolan Väst"); nuni++;
            universityclass uu54 = new universityclass(); uu54.number = nuni; unidict.Add("Högskolan i Borås", uu54); uniindex.Add(nuni, "Högskolan i Borås"); nuni++;
            universityclass uu55 = new universityclass(); uu55.number = nuni; unidict.Add("Högskolan i Gävle", uu55); uniindex.Add(nuni, "Högskolan i Gävle"); nuni++;
            universityclass uu56 = new universityclass(); uu56.number = nuni; unidict.Add("Högskolan i Halmstad", uu56); uniindex.Add(nuni, "Högskolan i Halmstad"); nuni++;
            universityclass uu57 = new universityclass(); uu57.number = nuni; unidict.Add("Högskolan i Jönköping", uu57); uniindex.Add(nuni, "Högskolan i Jönköping"); nuni++;
            universityclass uu57b = new universityclass(); uu57b.number = nuni; unidict.Add("Jönköping University", uu57b); uniindex.Add(nuni, "Jönköping University"); nuni++;
            universityclass uu58 = new universityclass(); uu58.number = nuni; unidict.Add("Högskolan i Skövde", uu58); uniindex.Add(nuni, "Högskolan i Skövde"); nuni++;
            universityclass uu59 = new universityclass(); uu59.number = nuni; unidict.Add("Johannelunds teologiska högskola", uu59); uniindex.Add(nuni, "Johannelunds teologiska högskola"); nuni++;
            universityclass uu60 = new universityclass(); uu60.number = nuni; unidict.Add("Karlstads universitet", uu60); uniindex.Add(nuni, "Karlstads universitet"); nuni++;
            universityclass uu61 = new universityclass(); uu61.number = nuni; unidict.Add("Karolinska institutet", uu61); uniindex.Add(nuni, "Karolinska institutet"); nuni++;
            universityclass uu62 = new universityclass(); uu62.number = nuni; unidict.Add("Konstfack", uu62); uniindex.Add(nuni, "Konstfack"); nuni++;
            universityclass uu63 = new universityclass(); uu63.number = nuni; unidict.Add("Kungl. Konsthögskolan", uu63); uniindex.Add(nuni, "Kungl. Konsthögskolan"); nuni++;
            universityclass uu64 = new universityclass(); uu64.number = nuni; unidict.Add("Kungl. Musikhögskolan i Stockholm", uu64); uniindex.Add(nuni, "Kungl. Musikhögskolan i Stockholm"); nuni++;
            universityclass uu64b = new universityclass(); uu64b.number = nuni; unidict.Add("Kungliga Musikhögskolan i Stockholm", uu64b); uniindex.Add(nuni, "Kungliga Musikhögskolan i Stockholm"); nuni++;
            universityclass uu65 = new universityclass(); uu65.number = nuni; unidict.Add("Kungl. Tekniska högskolan", uu65); uniindex.Add(nuni, "Kungl. Tekniska högskolan"); nuni++;
            universityclass uu66 = new universityclass(); uu66.number = nuni; unidict.Add("Linköpings universitet", uu66); uniindex.Add(nuni, "Linköpings universitet"); nuni++;
            universityclass uu67 = new universityclass(); uu67.number = nuni; unidict.Add("Linnéuniversitetet", uu67); uniindex.Add(nuni, "Linnéuniversitetet"); nuni++;
            universityclass uu69 = new universityclass(); uu69.number = nuni; unidict.Add("Luleå tekniska universitet", uu69); uniindex.Add(nuni, "Luleå tekniska universitet"); nuni++;
            universityclass uu70 = new universityclass(); uu70.number = nuni; unidict.Add("Lunds universitet", uu70); uniindex.Add(nuni, "Lunds universitet"); nuni++;
            universityclass uu71 = new universityclass(); uu71.number = nuni; unidict.Add("Malmö högskola", uu71); uniindex.Add(nuni, "Malmö högskola"); nuni++;
            universityclass uu72 = new universityclass(); uu72.number = nuni; unidict.Add("Mittuniversitetet", uu72); uniindex.Add(nuni, "Mittuniversitetet"); nuni++;
            universityclass uu73 = new universityclass(); uu73.number = nuni; unidict.Add("Mälardalens högskola", uu73); uniindex.Add(nuni, "Mälardalens högskola"); nuni++;
            universityclass uu76 = new universityclass(); uu76.number = nuni; unidict.Add("Newmaninstitutet", uu76); uniindex.Add(nuni, "Newmaninstitutet"); nuni++;
            universityclass uu77 = new universityclass(); uu77.number = nuni; unidict.Add("Röda Korsets högskola", uu77); uniindex.Add(nuni, "Röda Korsets högskola"); nuni++;
            universityclass uu77b = new universityclass(); uu77b.number = nuni; unidict.Add("Röda korsets högskola", uu77b); uniindex.Add(nuni, "Röda korsets högskola"); nuni++;
            universityclass uu78 = new universityclass(); uu78.number = nuni; unidict.Add("Sophiahemmet högskola", uu78); uniindex.Add(nuni, "Sophiahemmet högskola"); nuni++;
            universityclass uu78b = new universityclass(); uu78b.number = nuni; unidict.Add("Sophiahemmet Högskola", uu78b); uniindex.Add(nuni, "Sophiahemmet Högskola"); nuni++;
            universityclass uu79 = new universityclass(); uu79.number = nuni; unidict.Add("Stockholms Musikpedagogiska Institut", uu79); uniindex.Add(nuni, "Stockholms Musikpedagogiska Institut"); nuni++;
            universityclass uu80 = new universityclass(); uu80.number = nuni; unidict.Add("Stockholms konstnärliga högskola", uu80); uniindex.Add(nuni, "Stockholms konstnärliga högskola"); nuni++;
            universityclass uu81 = new universityclass(); uu81.number = nuni; unidict.Add("Stockholms universitet", uu81); uniindex.Add(nuni, "Stockholms universitet"); nuni++;
            universityclass uu82 = new universityclass(); uu82.number = nuni; unidict.Add("Sveriges lantbruksuniversitet", uu82); uniindex.Add(nuni, "Sveriges lantbruksuniversitet"); nuni++;
            universityclass uu82b = new universityclass(); uu82b.number = nuni; unidict.Add("Sveriges Lantbruksuniversitet", uu82b); uniindex.Add(nuni, "Sveriges Lantbruksuniversitet"); nuni++;
            universityclass uu83 = new universityclass(); uu83.number = nuni; unidict.Add("Södertörns högskola", uu83); uniindex.Add(nuni, "Södertörns högskola"); nuni++;
            universityclass uu84 = new universityclass(); uu84.number = nuni; unidict.Add("Teologiska Högskolan Stockholm", uu84); uniindex.Add(nuni, "Teologiska Högskolan Stockholm"); nuni++;
            universityclass uu84b = new universityclass(); uu84b.number = nuni; unidict.Add("Teologiska högskolan Stockholm", uu84b); uniindex.Add(nuni, "Teologiska högskolan Stockholm"); nuni++;
            universityclass uu85 = new universityclass(); uu85.number = nuni; unidict.Add("Umeå universitet", uu85); uniindex.Add(nuni, "Umeå universitet"); nuni++;
            universityclass uu86 = new universityclass(); uu86.number = nuni; unidict.Add("Uppsala universitet", uu86); uniindex.Add(nuni, "Uppsala universitet"); nuni++;
            universityclass uu87 = new universityclass(); uu87.number = nuni; unidict.Add("Örebro teologiska högskola", uu87); uniindex.Add(nuni, "Örebro teologiska högskola"); nuni++;
            universityclass uu88 = new universityclass(); uu88.number = nuni; unidict.Add("Örebro universitet", uu88); uniindex.Add(nuni, "Örebro universitet"); nuni++;
            universityclass uu89 = new universityclass(); uu89.number = nuni; unidict.Add("Övr. enskilda anordn. psykoterapeututb.", uu89); uniindex.Add(nuni, "Övr. enskilda anordn. psykoterapeututb."); nuni++;
            universityclass uu90 = new universityclass(); uu90.number = nuni; unidict.Add("Malmö universitet", uu90); uniindex.Add(nuni, "Malmö universitet"); nuni++;
            universityclass uu91 = new universityclass(); uu91.number = nuni; unidict.Add("Ersta Sköndal Bräcke högskola", uu91); uniindex.Add(nuni, "Ersta Sköndal Bräcke högskola"); nuni++;
            //Stiftelsen Högskolan i Jönköping
            universityclass uu57c = new universityclass(); uu57c.number = nuni; unidict.Add("Stiftelsen Högskolan i Jönköping", uu57c); uniindex.Add(nuni, "Stiftelsen Högskolan i Jönköping"); nuni++;
            universityclass uu57d = new universityclass(); uu57d.number = nuni; unidict.Add("Stiftelsen Stiftelsen Högskolan i Jönköping", uu57d); uniindex.Add(nuni, "Stiftelsen Stiftelsen Högskolan i Jönköping"); nuni++;
            universityclass uu64c = new universityclass(); uu64c.number = nuni; unidict.Add("Kungl. Musikhögskolan i Sthlm", uu64c); uniindex.Add(nuni, "Kungl. Musikhögskolan i Sthlm"); nuni++;
            universityclass uu33b = new universityclass(); uu33b.number = nuni; unidict.Add("Operahögskolan", uu33b); uniindex.Add(nuni, "Operahögskolan"); nuni++;
            universityclass uu35b = new universityclass(); uu35b.number = nuni; unidict.Add("Teaterhögskolan", uu35b); uniindex.Add(nuni, "Teaterhögskolan"); nuni++;
            universityclass uu41b = new universityclass(); uu41b.number = nuni; unidict.Add("Beckmans Designhögskola", uu41b); uniindex.Add(nuni, "Beckmans Designhögskola"); nuni++;
            universityclass uu77c = new universityclass(); uu77c.number = nuni; unidict.Add("Röda Korsets Högskola", uu77c); uniindex.Add(nuni, "Röda Korsets Högskola"); nuni++;

            //nuni = university_alias("Gymnastik- och idrottshögskolan", "Gymnastik- och idrottshögskolan",nuni);
            nuni = university_alias("Högskolan i Sundsvall/Härnösand", "Mittuniversitetet", nuni);
            nuni = university_alias("Högskolan i Östersund", "Mittuniversitetet", nuni);
            nuni = university_alias("Högskolan i Dalarna", "Högskolan Dalarna", nuni);
            nuni = university_alias("Karlstad universitet", "Karlstads universitet", nuni);
            nuni = university_alias("Lunds Universitet", "Lunds universitet", nuni);
            nuni = university_alias("Uppsala Universitet", "Uppsala universitet", nuni);
            nuni = university_alias("Totalt", "Total", nuni);
            nuni = university_alias("Riket", "Total", nuni);

            nuni = university_alias("Kungliga tekniska högskolan", "Kungl. Tekniska högskolan", nuni);
            nuni = university_alias("Kungl. tekniska högskolan", "Kungl. Tekniska högskolan", nuni);
            nuni = university_alias("KTH", "Kungl. Tekniska högskolan", nuni);
            nuni = university_alias("Chalmers Tekniska Högskola", "Chalmers tekniska högskola", nuni);
            nuni = university_alias("Chalmers", "Chalmers tekniska högskola", nuni);
            nuni = university_alias("Karolinska Institutet", "Karolinska institutet", nuni);
            nuni = university_alias("Blekinge Tekniska Högskola", "Blekinge tekniska högskola", nuni);
            //nuni = university_alias("", "", nuni);
            //nuni = university_alias("", "", nuni);
            //nuni = university_alias("", "", nuni);
            //nuni = university_alias("", "", nuni);
            //nuni = university_alias("", "", nuni);



            nuni = university_alias("Örebro Teologiska Högskola", "Örebro teologiska högskola", nuni);
            nuni = university_alias("Teologiska Högskolan, Stockholm", "Teologiska Högskolan Stockholm", nuni);
            
            unidict["Blekinge internationella hälsohögskola"].merged_with = "Blekinge tekniska högskola";
            unidict["Ersta högskola"].merged_with = "Ersta Sköndal Bräcke högskola";
            unidict["Ersta Sköndal högskola"].merged_with = "Ersta Sköndal Bräcke högskola";
            unidict["Stiftelsen Stora Sköndal"].merged_with = "Ersta Sköndal Bräcke högskola";
            unidict["Vårdhögskolan i Göteborg"].merged_with = "Göteborgs universitet";
            unidict["Vårdhögskolan Falun"].merged_with = "Högskolan Dalarna";
            unidict["Vårdhögskolan i Borås"].merged_with = "Högskolan i Borås";
            unidict["Vårdhögskolan Gävle"].merged_with = "Högskolan i Gävle";
            unidict["Vårdhögskolan i Halland"].merged_with = "Högskolan i Halmstad";
            unidict["Hälsohögskolan i Jönköping"].merged_with = "Högskolan i Jönköping";
            unidict["Hälsohögskolan Väst, Skövde"].merged_with = "Högskolan i Skövde";
            unidict["Vårdhögskolan Kristianstad"].merged_with = "Högskolan Kristianstad";
            unidict["Bohusläns vårdhögskola"].merged_with = "Högskolan Väst";
            unidict["Hälsohögskolan Väst i Vänersborg"].merged_with = "Högskolan Väst";
            unidict["Vårdhögskolan i Vänersborg"].merged_with = "Högskolan Väst";
            unidict["Hälsohögskolan i Värmland"].merged_with = "Karlstads universitet";
            unidict["Ingesunds Musikhögskola"].merged_with = "Karlstads universitet";
            unidict["Hälsohögskolan i Stockholm"].merged_with = "Karolinska institutet";
            unidict["Hälsouniversitetet i Linköping"].merged_with = "Linköpings universitet";
            unidict["Högskolan i Kalmar"].merged_with = "Linnéuniversitetet";
            unidict["Kalmar läns vårdhögskola"].merged_with = "Linnéuniversitetet";
            unidict["Växjö universitet"].merged_with = "Linnéuniversitetet";
            unidict["Vårdhögskolan i Växjö"].merged_with = "Linnéuniversitetet";
            unidict["Vårdhögskolan Boden"].merged_with = "Luleå tekniska universitet";
            unidict["Vårdhögskolan Lund/Helsingborg"].merged_with = "Lunds universitet";
            unidict["Vårdhögskolan i Malmö"].merged_with = "Malmö högskola";
            unidict["Vårdhögskolan i Sundsvall/Ö-vik"].merged_with = "Mittuniversitetet";
            unidict["Vårdhögskolan i Östersund"].merged_with = "Mittuniversitetet";
            unidict["Vårdhögskolan i Eskilstuna"].merged_with = "Mälardalens högskola";
            unidict["Vårdhögskolan i Västerås"].merged_with = "Mälardalens högskola";
            unidict["Dans- och cirkushögskolan"].merged_with = "Stockholms konstnärliga högskola";
            unidict["Dramatiska institutet"].merged_with = "Stockholms konstnärliga högskola";
            unidict["Lärarhögskolan i Stockholm"].merged_with = "Stockholms konstnärliga högskola";
            unidict["Operahögskolan i Stockholm"].merged_with = "Stockholms konstnärliga högskola";
            unidict["Operahögskolan"].merged_with = "Stockholms konstnärliga högskola";
            unidict["Stockholms dramatiska högskola"].merged_with = "Stockholms konstnärliga högskola";
            unidict["Teaterhögskolan i Stockholm"].merged_with = "Stockholms konstnärliga högskola";
            unidict["Teaterhögskolan"].merged_with = "Stockholms konstnärliga högskola";
            unidict["Grafiska institutet/IHR"].merged_with = "Stockholms universitet";
            unidict["Hälsohögskolan i Umeå"].merged_with = "Umeå universitet";
            unidict["Högskolan på Gotland"].merged_with = "Uppsala universitet";
            unidict["Vårdhögskolan i Uppsala"].merged_with = "Uppsala universitet";
            unidict["Vårdhögskolan i Örebro"].merged_with = "Örebro universitet";
            unidict["Försvarshögskolan Stockholm"].merged_with = "Försvarshögskolan";
            unidict["Jönköping University"].merged_with = "Högskolan i Jönköping";
            unidict["Stiftelsen Högskolan i Jönköping"].merged_with = "Högskolan i Jönköping";
            unidict["Stiftelsen Stiftelsen Högskolan i Jönköping"].merged_with = "Högskolan i Jönköping";
            unidict["Sophiahemmet Högskola"].merged_with = "Sophiahemmet högskola";
            unidict["Teologiska högskolan Stockholm"].merged_with = "Teologiska Högskolan Stockholm";
            unidict["Sveriges Lantbruksuniversitet"].merged_with = "Sveriges lantbruksuniversitet";
            unidict["Röda korsets högskola"].merged_with = "Röda Korsets högskola";
            unidict["Röda Korsets Högskola"].merged_with = "Röda Korsets högskola";
            unidict["Kungliga Musikhögskolan i Stockholm"].merged_with = "Kungl. Musikhögskolan i Stockholm";
            unidict["Kungl. Musikhögskolan i Sthlm"].merged_with = "Kungl. Musikhögskolan i Stockholm";
            unidict["Malmö högskola"].merged_with = "Malmö universitet";
            unidict["Beckmans Designhögskola"].merged_with = "Beckmans designhögskola";


            unidict["Beckmans designhögskola"].lan.Add("Stockholms län");
            unidict["Blekinge tekniska högskola"].lan.Add("Blekinge län");
            unidict["Chalmers tekniska högskola"].lan.Add("Västra Götalands län");
            unidict["Ericastiftelsen"].lan.Add("Stockholms län");
            unidict["Ersta Sköndal Bräcke högskola"].lan.Add("Stockholms län");
            unidict["Ersta Sköndal Bräcke högskola"].lan.Add("Västra Götalands län");
            unidict["Försvarshögskolan"].lan.Add("Stockholms län");
            unidict["Gammelkroppa skogsskola"].lan.Add("Värmlands län");
            unidict["Gymnastik- och idrottshögskolan"].lan.Add("Stockholms län");
            unidict["Göteborgs universitet"].lan.Add("Västra Götalands län");
            unidict["Handelshögskolan i Stockholm"].lan.Add("Stockholms län");
            unidict["Högskolan Dalarna"].lan.Add("Dalarnas län");
            unidict["Högskolan Kristianstad"].lan.Add("Skåne län");
            unidict["Högskolan Väst"].lan.Add("Västra Götalands län");
            unidict["Högskolan i Borås"].lan.Add("Västra Götalands län");
            unidict["Högskolan i Gävle"].lan.Add("Gävleborgs län");
            unidict["Högskolan i Halmstad"].lan.Add("Hallands län");
            unidict["Högskolan i Jönköping"].lan.Add("Jönköpings län");
            unidict["Högskolan i Skövde"].lan.Add("Västra Götalands län");
            unidict["Johannelunds teologiska högskola"].lan.Add("Stockholms län");
            unidict["Karlstads universitet"].lan.Add("Värmlands län");
            unidict["Karolinska institutet"].lan.Add("Stockholms län");
            unidict["Konstfack"].lan.Add("Stockholms län");
            unidict["Kungl. Konsthögskolan"].lan.Add("Stockholms län");
            unidict["Kungl. Musikhögskolan i Stockholm"].lan.Add("Stockholms län");
            unidict["Kungl. Tekniska högskolan"].lan.Add("Stockholms län");
            unidict["Linköpings universitet"].lan.Add("Östergötlands län");
            unidict["Linnéuniversitetet"].lan.Add("Kronobergs län");
            unidict["Linnéuniversitetet"].lan.Add("Kalmar län");
            unidict["Luleå tekniska universitet"].lan.Add("Norrbottens län");
            unidict["Lunds universitet"].lan.Add("Skåne län");
            unidict["Malmö högskola"].lan.Add("Skåne län");
            unidict["Malmö universitet"].lan.Add("Skåne län");
            unidict["Mittuniversitetet"].lan.Add("Västernorrlands län");
            unidict["Mälardalens högskola"].lan.Add("Södermanlands län");
            unidict["Mittuniversitetet"].lan.Add("Jämtlands län");
            unidict["Mälardalens högskola"].lan.Add("Västmanlands län");
            unidict["Newmaninstitutet"].lan.Add("Uppsala län");
            unidict["Röda Korsets högskola"].lan.Add("Stockholms län");
            unidict["Sophiahemmet högskola"].lan.Add("Stockholms län");
            unidict["Stockholms Musikpedagogiska Institut"].lan.Add("Stockholms län");
            unidict["Stockholms konstnärliga högskola"].lan.Add("Stockholms län");
            unidict["Stockholms universitet"].lan.Add("Stockholms län");
            unidict["Sveriges lantbruksuniversitet"].lan.Add("Uppsala län");
            unidict["Södertörns högskola"].lan.Add("Stockholms län");
            unidict["Teologiska Högskolan Stockholm"].lan.Add("Stockholms län");
            unidict["Umeå universitet"].lan.Add("Västerbottens län");
            unidict["Uppsala universitet"].lan.Add("Uppsala län");
            unidict["Örebro teologiska högskola"].lan.Add("Örebro län");
            unidict["Örebro universitet"].lan.Add("Örebro län");
            unidict["Övr. enskilda anordn. psykoterapeututb."].lan.Add("Stockholms län");

            unidict["Beckmans designhögskola"].kommun.Add("Stockholm");
            unidict["Blekinge tekniska högskola"].kommun.Add("Karlskrona");
            unidict["Blekinge tekniska högskola"].kommun.Add("Karlshamn");
            unidict["Chalmers tekniska högskola"].kommun.Add("Göteborg");
            unidict["Ericastiftelsen"].kommun.Add("Stockholm");
            unidict["Ersta Sköndal Bräcke högskola"].kommun.Add("Stockholm");
            unidict["Ersta Sköndal Bräcke högskola"].kommun.Add("Göteborg");
            unidict["Försvarshögskolan"].kommun.Add("Stockholm");
            unidict["Gammelkroppa skogsskola"].kommun.Add("Filipstad");
            unidict["Gymnastik- och idrottshögskolan"].kommun.Add("Stockholm");
            unidict["Göteborgs universitet"].kommun.Add("Göteborg");
            unidict["Handelshögskolan i Stockholm"].kommun.Add("Stockholm");
            unidict["Högskolan Dalarna"].kommun.Add("Falun");
            unidict["Högskolan Dalarna"].kommun.Add("Borlänge");
            unidict["Högskolan Kristianstad"].kommun.Add("Kristianstad");
            unidict["Högskolan Väst"].kommun.Add("Trollhättan");
            unidict["Högskolan i Borås"].kommun.Add("Borås");
            unidict["Högskolan i Gävle"].kommun.Add("Gävle");
            unidict["Högskolan i Halmstad"].kommun.Add("Halmstad");
            unidict["Högskolan i Jönköping"].kommun.Add("Jönköping");
            unidict["Högskolan i Skövde"].kommun.Add("Skövde");
            unidict["Johannelunds teologiska högskola"].kommun.Add("Stockholm");
            unidict["Karlstads universitet"].kommun.Add("Karlstad");
            unidict["Karolinska institutet"].kommun.Add("Stockholm");
            unidict["Karolinska institutet"].kommun.Add("Huddinge");
            unidict["Konstfack"].kommun.Add("Stockholm");
            unidict["Kungl. Konsthögskolan"].kommun.Add("Stockholm");
            unidict["Kungl. Musikhögskolan i Stockholm"].kommun.Add("Stockholm");
            unidict["Kungl. Tekniska högskolan"].kommun.Add("Stockholm");
            unidict["Linköpings universitet"].kommun.Add("Linköping");
            unidict["Linköpings universitet"].kommun.Add("Norrköping");
            unidict["Linnéuniversitetet"].kommun.Add("Kalmar");
            unidict["Linnéuniversitetet"].kommun.Add("Växjö");
            unidict["Luleå tekniska universitet"].kommun.Add("Luleå");
            unidict["Luleå tekniska universitet"].kommun.Add("Piteå");
            unidict["Lunds universitet"].kommun.Add("Lund");
            unidict["Lunds universitet"].kommun.Add("Helsingborg");
            unidict["Malmö universitet"].kommun.Add("Malmö");
            unidict["Mittuniversitetet"].kommun.Add("Sundsvall");
            unidict["Mälardalens högskola"].kommun.Add("Västerås");
            unidict["Mittuniversitetet"].kommun.Add("Östersund");
            unidict["Mittuniversitetet"].kommun.Add("Härnösand");
            unidict["Mälardalens högskola"].kommun.Add("Eskilstuna");
            unidict["Newmaninstitutet"].kommun.Add("Uppsala");
            unidict["Röda Korsets högskola"].kommun.Add("Stockholm");
            unidict["Sophiahemmet högskola"].kommun.Add("Stockholm");
            unidict["Stockholms Musikpedagogiska Institut"].kommun.Add("Stockholm");
            unidict["Stockholms konstnärliga högskola"].kommun.Add("Stockholm");
            unidict["Stockholms universitet"].kommun.Add("Stockholm");
            unidict["Sveriges lantbruksuniversitet"].kommun.Add("Uppsala");
            unidict["Södertörns högskola"].kommun.Add("Huddinge");
            unidict["Teologiska Högskolan Stockholm"].kommun.Add("Stockholm");
            unidict["Umeå universitet"].kommun.Add("Umeå");
            unidict["Uppsala universitet"].kommun.Add("Uppsala");
            unidict["Uppsala universitet"].kommun.Add("Gotland");
            unidict["Örebro teologiska högskola"].kommun.Add("Örebro");
            unidict["Örebro universitet"].kommun.Add("Örebro");
            unidict["Övr. enskilda anordn. psykoterapeututb."].kommun.Add("Stockholm");


        }

        private void Unibutton_Click(object sender, EventArgs e)
        {
            university_data();

            List<string> unidone = (from c in db.OV_University select c.Name).ToList();

            foreach (string uni in unidict.Keys)
            {
                if (unidone.Contains(uni))
                    continue;
                OV_University ou = new OV_University();
                ou.Name = uni;
                ou.Id = unidict[uni].number;
                db.OV_University.InsertOnSubmit(ou);
            }
            db.SubmitChanges();
            memo("Uni pass 1");

            int nok = 1;
            foreach (string uni in unidict.Keys)
            {
                if (unidone.Contains(uni))
                    continue;

                if (!String.IsNullOrEmpty(unidict[uni].merged_with))
                {
                    OV_University ou = (from c in db.OV_University where c.Id == unidict[uni].number select c).FirstOrDefault();
                    ou.Mergedwith = unidict[unidict[uni].merged_with].number;
                }
                foreach (string k in unidict[uni].kommun)
                {
                    int? kid = (from c in db.OV_Kommun where c.Name == k select c.Id).FirstOrDefault();
                    if (kid == null)
                        memo("Invalid kommun " + k);
                    else
                    {
                        OV_University_Kommun ok = new OV_University_Kommun();
                        ok.Id = nok;
                        nok++;
                        ok.Kommun = (int)kid;
                        ok.Uni = unidict[uni].number;
                        db.OV_University_Kommun.InsertOnSubmit(ok);
                    }
                }
            }
            db.SubmitChanges();
            memo("Uni pass 2");

        }

        private void Incomebutton_Click(object sender, EventArgs e)
        {

            Dictionary<string, int> incomedict = new Dictionary<string, int>();
            Dictionary<string, int> sourcedict = new Dictionary<string, int>();

            int iou = 1;
            var qou = from c in db.OV_University_Income select c.Id;
            if (qou.Count() > 0)
                iou = qou.Max()+1;

            List<string> files = util.get_filelist(ukafolder);
            foreach (string fn in files)
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                if (!fn.Contains("totala-intakter"))
                    continue;
                string rex = @"_(\d{4})_";
                int year = -1;
                foreach (Match match in Regex.Matches(fn, rex))
                {
                    year = util.tryconvert(match.Groups[1].Value);
                }
                memo("year = "+year);

                var q = from c in db.OV_University_Income where c.Year == year select c;
                if (q.Count() > 0)
                {
                    memo("Skipping " + year);
                    continue;
                }
                else
                    memo("Doing " + year);

                int incomecol;
                if (year < 2006) //change in file format 2006
                    incomecol = 4;
                else
                    incomecol = 6;

                int nline = 0;
                using (StreamReader sr = new StreamReader(fn))
                {
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        nline++;
                        if (nline % 100 == 0)
                        {
                            memo(nline + " lines");
                            //break; //#########################################
                        }
                        string[] words = line.Split('\t');
                        int iuni = getuni(words[1],db);
                        if (iuni < 0)
                        {
                            memo("iuni = " + iuni+" "+words[1]);
                            continue;
                        }

                        int iincome = -1;
                        if (String.IsNullOrEmpty(words[2]))
                            words[2] = "Total";

                        //memo("words2 = " + words[2]);
                        if (incomedict.ContainsKey(words[2]))
                        {
                            iincome = incomedict[words[2]];
                            //memo("1");
                        }
                        else
                        {
                            OV_Incometype ii = (from c in db.OV_Incometype where c.Name == words[2] select c).FirstOrDefault();
                            if (ii != null)
                            {
                                iincome = ii.Id;
                                //memo("2");
                            }
                            else
                            {
                                //memo("3");
                                iincome = (from c in db.OV_Incometype select c.Id).Max() + 1;
                                OV_Incometype oi = new OV_Incometype();
                                oi.Id = iincome;
                                oi.Name = words[2];
                                db.OV_Incometype.InsertOnSubmit(oi);
                                db.SubmitChanges();
                            }
                            incomedict.Add(words[2], iincome);
                        }
                        //memo("iincome = " + iincome);

                        int isource = -1;
                        if (String.IsNullOrEmpty(words[3]))
                            words[3] = "Total";
                        if (sourcedict.ContainsKey(words[3]))
                        {
                            isource = sourcedict[words[3]];
                        }
                        else
                        {
                            OV_Incomesource ii = (from c in db.OV_Incomesource where c.Name == words[3] select c).FirstOrDefault();
                            if (ii != null)
                            {
                                isource = ii.Id;
                            }
                            else
                            {
                                isource = (from c in db.OV_Incomesource select c.Id).Max() + 1;
                                OV_Incomesource oi = new OV_Incomesource();
                                oi.Id = isource;
                                oi.Name = words[3];
                                db.OV_Incomesource.InsertOnSubmit(oi);
                                db.SubmitChanges();
                            }
                            sourcedict.Add(words[3], isource);
                        }

                        int income = util.tryconvert(words[incomecol]);
                        if (income == -1) //conversion failed
                            continue;

                        OV_University_Income ou = new OV_University_Income();
                        ou.Id = iou;
                        iou++;
                        ou.Uni = iuni;
                        ou.Incometype = iincome;
                        ou.Incomesource = isource;
                        ou.Year = year;
                        ou.Amount = income;
                        db.OV_University_Income.InsertOnSubmit(ou);
                    }
                    db.SubmitChanges();
                    register_fileentry(fn);
                }
            }
        }

        public int getuni(string namepar, DbTGSAnalysTest db)
        {
            if (unidict.Count == 0)
                university_data();

            int iuni = 0;

            string name = namepar.Trim().Replace("  ", " ");

            if (String.IsNullOrEmpty(name))
            {
                iuni = 0;
            }
            else if (unidict.ContainsKey(name))
            {
                string mname = name;
                while (!String.IsNullOrEmpty(unidict[mname].merged_with))
                    mname = unidict[mname].merged_with;
                iuni = unidict[mname].number;
                //if (mname != name)
                //    memo(name + " -> " + mname);
            }
            else
            {
                memo("Invalid uni " + name);
                iuni = -1;
            }

            return iuni;
        }

        public string getuni(int iuni)
        {
            if (iuni == 0)
                return "";
            var q = from c in unidict.Keys where unidict[c].number == iuni select c;
            return q.FirstOrDefault();
        }

        private void HSTbutton_Click(object sender, EventArgs e)
        {
            int ihh = 1;

            int minyear = 9999;
            int maxyear = -1;

            var qhh = from c in db.OV_hsthpr select c.Id;
            if (qhh.Count() > 0)
            {
                ihh = qhh.Max() + 1;
                qhh = from c in db.OV_hsthpr select c.Year;
                minyear = qhh.Max() + 1;
            }


            int nline = 0;
            Dictionary<string, int> areadict = new Dictionary<string, int>();
            var qa = from c in db.OV_subjectarea select c;
            int iarea = qa.Count() + 1;
            foreach (OV_subjectarea os in qa)
                areadict.Add(os.Name, os.Id);

            int ndouble = 0;

            List<string> files = util.get_filelist(ukafolder);
            foreach (string fn in files)
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                if (!fn.Contains("hst-hpr"))
                    continue;
                string rex = @"_(\d{4})_";
                int fileyear = 9999;
                foreach (Match match in Regex.Matches(fn, rex))
                {
                    fileyear = util.tryconvert(match.Groups[1].Value);
                }
                memo("year = " + fileyear);

                if (fileyear < minyear)
                    continue;

                //string fn = ukafolder + @"\hst-hpr.txt";

                using (StreamReader sr = new StreamReader(fn))
                {
                    string hline = sr.ReadLine();
                    string[] hw = hline.Split('\t');
                    int iyear = 1;
                    int iunicol = 2;
                    int ihst = 4;
                    int ihpr = 5;
                    int icarea = 3;
                    for (int ih = 0;ih<hw.Length;ih++)
                    {
                        if (hw[ih] == "År")
                            iyear = ih;
                        else if (hw[ih].Contains("ärosäte"))
                            iunicol = ih;
                        else if (hw[ih].Contains("student"))
                            ihst = ih;
                        else if (hw[ih].Contains("prestat"))
                            ihpr = ih;
                        else if (hw[ih].Contains("bildnings"))
                            icarea = ih;

                    }

                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        nline++;
                        if (nline % 100 == 0)
                        {
                            memo(nline + " lines");
                            //break; //#########################################
                        }
                        string[] words = line.Split('\t');
                        //if (words.Length < 5)
                        //    continue;
                        //if (words.Length > 5)
                        //{
                        //    ihst++;
                        //    ihpr++;
                        //}
                        if ((words[ihst] == "0") && (words[ihpr] == "0"))
                            continue;
                        int iuni = getuni(words[iunicol].Trim().Replace("  ", " "), db);
                        if (iuni <= 0)
                            continue;
                        int year = util.tryconvert(words[iyear]);
                        if (year < minyear)
                            continue;
                        if (year > maxyear)
                            maxyear = year;
                        int area = -1;
                        if (!areadict.ContainsKey(words[icarea]))
                        {
                            OV_subjectarea os = new OV_subjectarea();
                            os.Id = iarea;
                            iarea++;
                            os.Name = words[icarea];
                            areadict.Add(os.Name, os.Id);

                            db.OV_subjectarea.InsertOnSubmit(os);
                            db.SubmitChanges();
                        }
                        area = areadict[words[icarea]];
                        if (area == 23)
                            area = 0;

                        int hst = util.tryconvert(words[ihst]);
                        int hpr = util.tryconvert(words[ihpr]);
                        var qq = from c in db.OV_hsthpr where c.Uni == iuni where c.Year == year where c.Area == area select c;
                        if (qq.Count() > 0)
                        {
                            if (ndouble < 10)
                            {
                                memo("Double entry " + line);
                            }
                            ndouble++;
                            continue;
                        }

                        OV_hsthpr oh = new OV_hsthpr();
                        oh.Id = ihh;
                        ihh++;
                        oh.Uni = iuni;
                        oh.Year = year;
                        oh.Area = area;
                        oh.HST = hst;
                        oh.HPR = hpr;

                        db.OV_hsthpr.InsertOnSubmit(oh);
                        db.SubmitChanges();
                    }
                    register_fileentry(fn);

                }
            }
            HST_total(minyear,maxyear);
        }

        private void HST_total()
        {
            int minyear = (from c in db.OV_hsthpr select c.Year).Min();
            int maxyear = (from c in db.OV_hsthpr select c.Year).Max();
            HST_total(minyear, maxyear);
        }

        private void HST_total(int minyear,int maxyear)
        {
            int ihh = 1;
            var qhh = from c in db.OV_hsthpr select c.Id;
            if (qhh.Count() > 0)
                ihh = qhh.Max() + 1;
            int ndouble = 0;

            for (int year=minyear;year<=maxyear;year++)
            {
                memo(year.ToString());
                foreach (int area in (from c in db.OV_subjectarea select c.Id))
                {
                    float hstsum = 0;
                    float hprsum = 0;
                    var q = from c in db.OV_hsthpr where c.Uni > 0 where year == c.Year where area == c.Area select c;
                    if (q.Count() == 0)
                    {
                        if ( area != 0)
                            continue;
                        else
                        {
                            var qall = from c in db.OV_hsthpr where c.Uni > 0 where year == c.Year where c.Area > 0 select c;
                            memo("qall");
                            if (qall.Count() == 0)
                                continue;
                            else
                            {
                                hstsum = (from c in qall select c.HST).Sum();
                                hprsum = (from c in qall select c.HPR).Sum();
                                memo("sumsum");
                            }
                        }
                    }
                    else
                    {
                        hstsum = (from c in q select c.HST).Sum();
                        hprsum = (from c in q select c.HPR).Sum();
                    }
                    var qq = from c in db.OV_hsthpr where c.Uni == 0 where c.Year == year where c.Area == area select c;
                    if (qq.Count() > 0)
                    {
                        if (ndouble < 10)
                        {
                            memo("Double entry HST_total");
                        }
                        ndouble++;
                        continue;
                    }
                    OV_hsthpr oh = new OV_hsthpr();
                    oh.Id = ihh;
                    ihh++;
                    oh.Uni = 0;
                    oh.Year = year;
                    oh.Area = area;
                    oh.HST = hstsum;
                    oh.HPR = hprsum;

                    db.OV_hsthpr.InsertOnSubmit(oh);
                    db.SubmitChanges();
                }
            }
        }

        private void HSTtotal_button_Click(object sender, EventArgs e)
        {
            HST_total();
        }

        private int getexamtype(string examname, int col)
        {
            return getexamtype(examname, col, -1);
        }

        private int getexamtype(string examname, int col, int level)
        {
            int iet = 0;
            if (!String.IsNullOrEmpty(examname))
            {
                var q = (from c in db.OV_examtype where c.Name == examname.Trim() select c);
                if (q.Count() > 0)
                {
                    iet = q.First().Id;
                }
                else
                {
                    if (examname.Contains("examen") || examname.Contains("motsv"))
                    {
                        iet = getexamtype(examname.Replace("examen", "examina").Replace("/motsv", ""), col);
                    }
                    if (iet <= 0)
                    {
                        memo("Creating "+examname);
                        OV_examtype oe = new OV_examtype();
                        int ioe = 1;
                        var qall = from c in db.OV_examtype select c.Id;
                        if (qall.Count() > 0)
                            ioe = qall.Max() + 1;
                        oe.Id = ioe;
                        oe.Name = examname.Trim();
                        oe.Kolumn = col;
                        if (level > 0)
                            oe.Level = level;
                        else
                            oe.Level = null;
                        db.OV_examtype.InsertOnSubmit(oe);
                        db.SubmitChanges();
                        iet = oe.Id;
                        //return -1;
                    }
                }
            }
            return iet;
        }

        private Dictionary<string, int> fill_genderdict()
        {
            return fill_genderdict(false);
        }

        private Dictionary<string, int> fill_genderdict(bool lowercase)
        {
            Dictionary<string, int> genderdict = new Dictionary<string, int>();
            var q = from c in db.OV_gender select c;
            foreach (OV_gender og in q)
                if (lowercase)
                    genderdict.Add(og.Name.ToLower(), og.Id);
                else
                    genderdict.Add(og.Name, og.Id);
            genderdict.Add("totalt", 0);
            return genderdict;
        }

        private Dictionary<string, int> fill_agedict()
        {
            Dictionary<string, int> agedict = new Dictionary<string, int>();
            var q = from c in db.OV_age select c;
            foreach (OV_age og in q)
                agedict.Add(og.Name, og.Id);
            return agedict;
        }

        private void PhDbutton_Click(object sender, EventArgs e)
        {

            Dictionary<string, int> genderdict = fill_genderdict();
            Dictionary<string, int> agedict = fill_agedict();
            Dictionary<string, int> examtypedict = new Dictionary<string, int>();


            int iou = 1;
            var qou = from c in db.OV_exam select c.Id;
            if (qou.Count() > 0)
                iou = qou.Max() + 1;

            string phd = "PhD ";
            int iphd = getexamtype(phd.Trim(), 0, 3);
            examtypedict.Add(phd.Trim(), iphd);
            //List<int> doneyears = new List<int>();
            var q = from c in db.OV_exam where c.Examtype0 == iphd select c.Year;
            List<int> doneyears = q.Distinct().ToList();

            List<string> files = util.get_filelist(ukafolder);
            foreach (string fn in files)
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                if (!fn.Contains("doktorsexamina")) //specialare för att lägga till lärosätesdata
                    continue;

                int nline = 0;
                using (StreamReader sr = new StreamReader(fn))
                {
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        nline++;
                        if (nline % 100 == 0)
                        {
                            memo(nline + " lines");
                            //break; //#########################################
                        }
                        string[] words = line.Split('\t');

                        int year = util.tryconvert(words[0]);
                        if ( year < 0)
                        {
                            memo("Bad year " + words[0]);
                            continue;
                        }

                        if (doneyears.Contains(year))
                            continue;

                        int iuni = getuni(words[1].Trim().Replace("  ", " "), db);
                        //memo("iuni = " + iuni);
                        if (iuni < 0)
                        {
                            memo("Unknown uni " + words[1]);
                            continue;
                        }

                        //if (iuni == 0)
                        //{
                        //    memo("Skipping national total"); //specialare för att lägga till lärosätesdata
                        //    continue;
                        //}

                        int iet0 = -1;
                        if (String.IsNullOrEmpty(words[2]))
                        {
                            if (examtypedict.ContainsKey(phd.Trim()))
                                iet0 = examtypedict[phd.Trim()];
                            else
                            {
                                iet0 = getexamtype(phd.Trim(), 0, 3);
                                examtypedict.Add(phd.Trim(), iet0);
                            }
                        }
                        else if (examtypedict.ContainsKey(phd + words[2]))
                            iet0 = examtypedict[phd + words[2]];
                        else
                        {
                            iet0 = getexamtype(phd + words[2], 0, 3);
                            examtypedict.Add(phd + words[2], iet0);
                        }
                        int iet1 = -1;
                        if (String.IsNullOrEmpty(words[3]))
                            iet1 = 0;
                        else if (examtypedict.ContainsKey(phd + words[3]))
                            iet1 = examtypedict[phd + words[3]];
                        else
                        {
                            iet1 = getexamtype(phd + words[3], 1, 3);
                            examtypedict.Add(phd + words[3], iet1);
                        }
                        int iet2 = -1;
                        if (String.IsNullOrEmpty(words[4]))
                            iet2 = 0;
                        else if (examtypedict.ContainsKey(phd + words[4]))
                            iet2 = examtypedict[phd + words[4]];
                        else
                        {
                            iet2 = getexamtype(phd + words[4], 2, 3);
                            examtypedict.Add(phd + words[4], iet2);
                        }

                        int n = util.tryconvert(words[7]);
                        if (n == -1) //conversion failed
                            continue;

                        OV_exam ou = new OV_exam();
                        ou.Id = iou;
                        iou++;
                        ou.Uni = iuni;
                        ou.Year = year;
                        ou.Examtype0 = iet0;
                        ou.Examtype1 = iet1;
                        ou.Examtype2 = iet2;
                        ou.Gender = genderdict[words[5]];
                        ou.Age = agedict[words[6]];
                        ou.Number = n;
                        db.OV_exam.InsertOnSubmit(ou);
                        db.SubmitChanges();
                    }
                    register_fileentry(fn);

                }
            }


        }


        private void ExamButton_Click(object sender, EventArgs e)
        {

            Dictionary<string, int> genderdict = fill_genderdict();
            Dictionary<string, int> agedict = fill_agedict();
            Dictionary<string, int> examtypedict = new Dictionary<string, int>();


            int iou = 1;
            var qou = from c in db.OV_exam select c.Id;
            if (qou.Count() > 0)
                iou = qou.Max() + 1;

            List<string> files = util.get_filelist(ukafolder);
            foreach (string fn in files)
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                if (!fn.Contains("examina-ar"))
                    continue;
                string rex = @"_(\d{4})_";
                int year = -1;
                foreach (Match match in Regex.Matches(fn, rex))
                {
                    year = util.tryconvert(match.Groups[1].Value);
                }
                memo("year = " + year);

                var q = from c in db.OV_exam where c.Year == year where c.Examtype0 == 0 select c;
                if (q.Count() > 0)
                {
                    memo("Skipping " + year);
                    continue;
                }
                else
                    memo("Doing " + year);

                int nline = 0;
                using (StreamReader sr = new StreamReader(fn))
                {
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        nline++;
                        if (nline % 100 == 0)
                        {
                            memo(nline + " lines");
                            //break; //#########################################
                        }
                        string[] words = line.Split('\t');
                        int iuni = getuni(words[1].Trim().Replace("  ", " "), db);
                        //memo("iuni = " + iuni);
                        if ( iuni < 0)
                        {
                            memo("Unknown uni " + words[1]);
                            continue;
                        }

                        int iet0 = -1;
                        if (examtypedict.ContainsKey(words[2]))
                            iet0 = examtypedict[words[2]];
                        else
                        {
                            iet0 = getexamtype(words[2], 0);
                            examtypedict.Add(words[2], iet0);
                        }
                        int iet1 = -1;
                        if (examtypedict.ContainsKey(words[3]))
                            iet1 = examtypedict[words[3]];
                        else
                        {
                            iet1 = getexamtype(words[3], 1);
                            examtypedict.Add(words[3], iet1);
                        }
                        int iet2 = -1;
                        if (examtypedict.ContainsKey(words[4]))
                            iet2 = examtypedict[words[4]];
                        else
                        {
                            iet2 = getexamtype(words[4], 2);
                            examtypedict.Add(words[4], iet2);
                        }

                        int n = util.tryconvert(words[7]);
                        if (n == -1) //conversion failed
                            continue;

                        OV_exam ou = new OV_exam();
                        ou.Id = iou;
                        iou++;
                        ou.Uni = iuni;
                        ou.Year = year;
                        ou.Examtype0 = iet0;
                        ou.Examtype1 = iet1;
                        ou.Examtype2 = iet2;
                        ou.Gender = genderdict[words[5]];
                        ou.Age = agedict[words[6]];
                        ou.Number = n;
                        db.OV_exam.InsertOnSubmit(ou);
                        db.SubmitChanges();
                    }
                    register_fileentry(fn);

                }
            }

        }

        private Dictionary<int, Dictionary<string, Dictionary<string, int>>> read_swepub(string fn)
        {
            // Data från http://bibliometri.swepub.kb.se/bibliometri

            //"_recordID","_orgCode","_pubYear","_publicatType","_creatorCount","_numLocalCreator"

            Dictionary<string, int> artcount = new Dictionary<string, int>();
            Dictionary<int, Dictionary<string, Dictionary<string, int>>> artdict = new Dictionary<int, Dictionary<string, Dictionary<string, int>>>();
            List<string> unicodes = new List<string>();
            int n = 0;
            using (StreamReader sr = new StreamReader(fn))
            {
                string headline = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();


                    //if (n > 250)
                    //    Console.WriteLine(line);
                    if (n % 1000 == 0)
                        memo(n.ToString());

                    string[] words = line.Split(',');

                    string id = words[0].Trim('"');
                    if (!artcount.ContainsKey(id))
                        artcount.Add(id, 0);
                    artcount[id]++;

                    string unicode = words[1].Trim('"');
                    if (!unicodes.Contains(unicode))
                        unicodes.Add(unicode);
                    int year = util.tryconvert(words[2].Trim('"'));
                    string pubtype = words[3].Trim('"');

                    if (!artdict.ContainsKey(year))
                    {
                        Dictionary<string, Dictionary<string, int>> temp = new Dictionary<string, Dictionary<string, int>>();
                        artdict.Add(year, temp);
                    }
                    if ( !artdict[year].ContainsKey(unicode))
                    {
                        Dictionary<string, int> temp2 = new Dictionary<string, int>();
                        artdict[year].Add(unicode, temp2);
                    }
                    if(!artdict[year][unicode].ContainsKey(pubtype))
                    {
                        artdict[year][unicode].Add(pubtype, 0);
                    }
                    artdict[year][unicode][pubtype]++;

                    n++;


                }
                register_fileentry(fn);

            }
            foreach (string uc in unicodes)
                memo(uc);

            memo("artcount " + artcount.Count);
            memo("n " + n);
            return artdict;
        }

        private void SWEPUB_PublicationButton_Click(object sender, EventArgs e)
        {
            Dictionary<int, Dictionary<string, Dictionary<string, int>>> artdict = read_swepub(swepubfolder + "swepub_query_result_180809.csv");

            int iop = 1;
            var qold = from c in db.OV_publication select c.Id;
            if (qold.Count() > 0)
                iop = qold.Max() + 1;

            foreach (int year in artdict.Keys)
            {
                foreach (string unicode in artdict[year].Keys)
                {
                    int? xuni = (from c in db.OV_University where c.Swepubcode == unicode select c.Id).FirstOrDefault();
                    if (xuni != null)
                    {
                        int uni = (int)xuni;
                        if (uni == 0)
                        {
                            memo("unicode = /" + unicode + "/");
                        }
                        else
                        {
                            foreach (string pubtype in artdict[year][unicode].Keys)
                            {
                                var q = from c in db.OV_publication
                                        where c.Year == year
                                        where c.Uni == uni
                                        where c.Pubtype == pubtype
                                        select c;
                                if (q.Count() == 0)
                                {
                                    OV_publication op = new OV_publication();
                                    op.Id = iop;
                                    iop++;
                                    op.Uni = uni;
                                    op.Year = year;
                                    op.Pubtype = pubtype;
                                    op.NumberSwepub = artdict[year][unicode][pubtype];
                                    db.OV_publication.InsertOnSubmit(op);
                                }
                            }
                            db.SubmitChanges();
                        }
                    }
                }
            }

            publication_totals();

        }

        private void read_UKA_publication_files(string folder, bool fromswepub)
        {
            Dictionary<string, int> subjectdict = new Dictionary<string, int>();
            Dictionary<string, int> pubtypedict = new Dictionary<string, int>();

            int iou = 1;
            var qou = from c in db.OV_publication select c.Id;
            if (qou.Count() > 0)
                iou = qou.Max() + 1;

            List<string> baduni = new List<string>();

            List<string> files = util.get_filelist(folder);
            foreach (string fn in files)
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                if (!fromswepub && !fn.Contains("hogskolans-publikationerbrutto"))
                    continue;
                if (fromswepub && !fn.Contains("UKA-format"))
                    continue;
                string rex = @"_(\d{4})_";
                int year = -1;
                foreach (Match match in Regex.Matches(fn, rex))
                {
                    year = util.tryconvert(match.Groups[1].Value);
                }
                if (year < 0)
                {
                    rex = @"(\d{4})";
                    foreach (Match match in Regex.Matches(fn, rex))
                    {
                        year = util.tryconvert(match.Groups[1].Value);
                    }

                }
                memo("year = " + year);

                if (year < 0)
                    continue;

                //var q = from c in db.OV_University_Income where c.Year == year select c;
                //if (q.Count() > 0)
                //{
                //    memo("Skipping " + year);
                //    continue;
                //}
                //else
                memo("Doing " + year);


                int nline = 0;
                using (StreamReader sr = new StreamReader(fn))
                {
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        nline++;
                        if (nline % 100 == 0)
                        {
                            memo(nline + " lines");
                            //break; //#########################################
                        }
                        string[] words = line.Split('\t');
                        int iuni = getuni(words[1], db);
                        if (iuni < 0)
                        {
                            memo("iuni = " + iuni + " " + words[1]);
                            if (!baduni.Contains(words[1]))
                                baduni.Add(words[1]);
                            continue;
                        }

                        int isubject = -1;
                        if (String.IsNullOrEmpty(words[3]))
                            words[3] = "Total";

                        //memo("words2 = " + words[2]);
                        if (subjectdict.ContainsKey(words[3]))
                        {
                            isubject = subjectdict[words[3]];
                            //memo("1");
                        }
                        else
                        {
                            OV_researchsubject ii = (from c in db.OV_researchsubject where c.Name == words[3] select c).FirstOrDefault();
                            if (ii != null)
                            {
                                isubject = ii.Id;
                                //subjectdict.Add(words[3], isubject);
                                //memo("2");
                            }
                            else
                            {
                                //memo("3");
                                isubject = (from c in db.OV_researchsubject select c.Id).Max() + 1;
                                OV_researchsubject oi = new OV_researchsubject();
                                oi.Id = isubject;
                                oi.Name = words[3];
                                db.OV_researchsubject.InsertOnSubmit(oi);
                                db.SubmitChanges();
                            }
                            subjectdict.Add(words[3], isubject);
                        }
                        //memo("iincome = " + iincome);

                        int ipubtype = -1;
                        if (String.IsNullOrEmpty(words[2]))
                            words[2] = "Total";

                        int number = util.tryconvert(words[6]);
                        if (number == -1) //conversion failed
                            continue;

                        var qq = from c in db.OV_publication
                                 where c.Uni == iuni
                                 where c.Year == year
                                 where c.Subject == isubject
                                 where c.Pubtype == words[2]
                                 select c;
                        OV_publication ovp = qq.FirstOrDefault();
                        if (ovp != null)
                        {
                            if (fromswepub)
                            {
                                ovp.NumberSwepub = number;
                            }
                            else
                            {
                                ovp.NumberUKA = number;
                            }
                        }
                        else
                        {
                            ovp = new OV_publication();
                            ovp.Id = iou;
                            iou++;
                            ovp.Uni = iuni;
                            ovp.Year = year;
                            ovp.Subject = isubject;
                            ovp.Pubtype = words[2];
                            if (fromswepub)
                            {
                                ovp.NumberUKA = null;
                                ovp.NumberSwepub = number;
                            }
                            else
                            {
                                ovp.NumberUKA = number;
                                ovp.NumberSwepub = 0;
                            }
                            db.OV_publication.InsertOnSubmit(ovp);
                        }
                    }
                    db.SubmitChanges();
                    register_fileentry(fn);
                }
            }
            foreach (string ss in baduni)
                memo(ss);

        }

        private void PublicationButton_Click(object sender, EventArgs e)
        {
            read_UKA_publication_files(ukafolder, false);
        }

        private void publication_totals()
        {
            int iop = 1;
            var qold = from c in db.OV_publication select c.Id;
            if (qold.Count() > 0)
                iop = qold.Max() + 1;

            Dictionary<string,int[]> artsumdict = new Dictionary<string,int[]>();

            var q = from c in db.OV_publication
                    select c;

            int minyear = (from c in q select c.Year).Min();
            int maxyear = (from c in q select c.Year).Max();

            var q0 = from c in q
                     where c.Uni == 0
                     select c;

            memo("Summing");
            foreach (OV_publication op in q)
            {
                if (op.Uni == 0)
                    continue;

                if ( !artsumdict.ContainsKey(op.Pubtype))
                {
                    int[] ii = new int[maxyear - minyear + 1];
                    for (int i=0; i<ii.Length;i++)
                        ii[i] = 0;
                    artsumdict.Add(op.Pubtype, ii);
                }
                artsumdict[op.Pubtype][op.Year - minyear] += op.NumberSwepub;
            }

            memo("Filling");
            foreach (string pt in artsumdict.Keys)
            {
                for (int i=0;i<artsumdict[pt].Length;i++)
                {
                    OV_publication op0 = (from c in q0
                              where c.Pubtype == pt
                              where c.Year == minyear + i
                              select c).FirstOrDefault();
                    if (op0 == null)
                    {
                        op0 = new OV_publication();
                        op0.Id = iop;
                        iop++;
                        op0.Uni = 0;
                        op0.Year = minyear + i;
                        op0.Pubtype = pt;
                        op0.NumberSwepub = artsumdict[pt][i];
                        db.OV_publication.InsertOnSubmit(op0);
                    }
                    else
                        op0.NumberSwepub = artsumdict[pt][i];
                }
            }
            db.SubmitChanges();
            memo("Done");
        }

        private void Publicationsumbutton_Click(object sender, EventArgs e)
        {
            publication_totals();
        }


        private void Establishbutton_Click(object sender, EventArgs e)
        {
            int n = 0;
            int ioe = 1;
            var qi = from c in db.OV_establishment select c.Id;
            if (qi.Count() > 0)
                ioe = qi.Max() + 1;
            Dictionary<string, int> etypedict = new Dictionary<string, int>();
            foreach (OV_establishmenttype oe in (from c in db.OV_establishmenttype select c))
            {
                etypedict.Add(oe.Name, oe.Id);
            }

            string fn = bakframgrundfolder + "etablering3-vs-uni.txt";
            using (StreamReader sr = new StreamReader(fn))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string yearline = sr.ReadLine();
                string[] yearwords = yearline.Split('\t');
                int[] years = new int[yearwords.Length];
                for (int i = 0; i < yearwords.Length;i++ )
                {
                    years[i] = util.tryconvert(yearwords[i]);
                }
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 250)
                    //    Console.WriteLine(line);
                    if (n % 1000 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    if (!etypedict.ContainsKey(words[0]))
                    {
                        OV_establishmenttype oet = new OV_establishmenttype();
                        if (etypedict.Count > 0)
                            oet.Id = etypedict.Values.Max() + 1;
                        else
                            oet.Id = 1;
                        oet.Name = words[0];
                        etypedict.Add(oet.Name, oet.Id);
                        db.OV_establishmenttype.InsertOnSubmit(oet);
                        db.SubmitChanges();
                    }

                    int etype = etypedict[words[0]];
                    int iuni = getuni(words[1], db);
                    if (iuni < 0)
                        continue;

                    for (int i = 2; i < yearwords.Length;i++ )
                    {
                        if (years[i] > 0)
                        {
                            int k = util.tryconvertbf(words[i]);
                            if ( k > 0)
                            {
                                OV_establishment oe = new OV_establishment();
                                oe.Id = ioe;
                                ioe++;
                                oe.Uni = iuni;
                                oe.Etype3y = etype;
                                oe.Year = years[i];
                                oe.Number = k;
                                db.OV_establishment.InsertOnSubmit(oe);
                                db.SubmitChanges();
                            }
                        }
                    }

                    n++;


                }
                register_fileentry(fn);

            }
            memo("n " + n);

        }

        private void Studenflowbutton_Click(object sender, EventArgs e)
        {
            int n = 0;
            int ioe = 1;
            var qi = from c in db.OV_studentflow select c.Id;
            if (qi.Count() > 0)
                ioe = qi.Max() + 1;
            Dictionary<string, int> landict = new Dictionary<string, int>();
            foreach (OV_Lan oe in (from c in db.OV_Lan select c))
            {
                landict.Add(oe.Name.ToLower(), oe.Id);
            }

            string fn = bakframgrundfolder + "from-worklan3-exam-allyear.v2.txt";
                            

            using (StreamReader sr = new StreamReader(fn))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string tolanline = sr.ReadLine();

                string[] tolanwords = tolanline.Split('\t');
                int[] tolan = new int[tolanwords.Length];
                for (int i = 0; i < tolanwords.Length; i++)
                {
                    if (landict.ContainsKey(tolanwords[i].Trim().ToLower()))
                        tolan[i] = landict[tolanwords[i].Trim().ToLower()];
                    else
                        tolan[i] = -1;
                }

                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    if (!landict.ContainsKey(words[2].Trim().ToLower()))
                    {
                        memo(words[2] + " unknown län 0");
                        continue;
                    }
                    int fromlan = landict[words[2].Trim().ToLower()];

                    int iuni = getuni(words[1], db);
                    if (iuni < 0)
                        continue;

                    int iet = getexamtype(words[0], -1);
                    if (iet < 0)
                        continue;

                    for (int i = 2; i < tolan.Length; i++)
                    {
                        if (tolan[i] > 0)
                        {
                            int k = util.tryconvertbf(words[i]);
                            //memo("k=" + k);
                            if (k > 0)
                            {
                                OV_studentflow oe = new OV_studentflow();
                                oe.Id = ioe;
                                ioe++;
                                oe.Uni = iuni;
                                oe.Fromlan = fromlan;
                                oe.Tolan = tolan[i];
                                oe.Year = 0;
                                oe.Number = k;
                                oe.Examtype = iet;
                                db.OV_studentflow.InsertOnSubmit(oe);
                            }
                        }
                    }
                    db.SubmitChanges();

                    n++;


                }
                register_fileentry(fn);
            }
            memo("n " + n);

        }

        //private void Studenflowbutton_Click_NOEXAM(object sender, EventArgs e)
        //{
        //    int n = 0;
        //    int ioe = 1;
        //    var qi = from c in db.OV_studentflow select c.Id;
        //    if (qi.Count() > 0)
        //        ioe = qi.Max() + 1;
        //    Dictionary<string, int> landict = new Dictionary<string, int>();
        //    foreach (OV_Lan oe in (from c in db.OV_Lan select c))
        //    {
        //        landict.Add(oe.Name.ToLower(), oe.Id);
        //    }
        //    using (StreamReader sr = new StreamReader(bakframgrundfolder + "fromlan-worklan3-uni.txt"))
        //    {
        //        sr.ReadLine();
        //        sr.ReadLine();
        //        sr.ReadLine();
        //        sr.ReadLine();
        //        sr.ReadLine();
        //        string yearline = sr.ReadLine();

        //        string[] yearwords = yearline.Split('\t');
        //        int[] years = new int[yearwords.Length];
        //        for (int i = 0; i < yearwords.Length; i++)
        //        {
        //            years[i] = util.tryconvert(yearwords[i]);
        //        }
        //        while (!sr.EndOfStream)
        //        {
        //            String line = sr.ReadLine();

        //            //if (n > 10)
        //            //    break;
        //            if (n % 100 == 0)
        //                memo(n.ToString());

        //            string[] words = line.Split('\t');

        //            if (!landict.ContainsKey(words[0].Trim().ToLower()))
        //            {
        //                memo(words[0] + " unknown län 0");
        //                continue;
        //            }
        //            int fromlan = landict[words[0].Trim().ToLower()];
        //            if (!landict.ContainsKey(words[1].Trim().ToLower()))
        //            {
        //                memo(words[1] + " unknown län 1");
        //                continue;
        //            }
        //            int tolan = landict[words[1].Trim().ToLower()];

        //            int iuni = getuni(words[2], db);
        //            if (iuni < 0)
        //                continue;

        //            for (int i = 2; i < yearwords.Length; i++)
        //            {
        //                if (years[i] > 0)
        //                {
        //                    int k = util.tryconvertbf(words[i]);
        //                    //memo("k=" + k);
        //                    if (k > 0)
        //                    {
        //                        OV_studentflow oe = new OV_studentflow();
        //                        oe.Id = ioe;
        //                        ioe++;
        //                        oe.Uni = iuni;
        //                        oe.Fromlan = fromlan;
        //                        oe.Tolan = tolan;
        //                        oe.Year = years[i];
        //                        oe.Number = k;
        //                        oe.Examtype = 0;
        //                        db.OV_studentflow.InsertOnSubmit(oe);
        //                    }
        //                }
        //            }
        //            db.SubmitChanges();

        //            n++;


        //        }
        //    }
        //    memo("n " + n);

        //}


        private void Staffbutton_Click(object sender, EventArgs e)
        {
            int ihh = 1;
            var qhh = from c in db.OV_staff select c.Id;
            if (qhh.Count() > 0)
                ihh = qhh.Max() + 1;

            Dictionary<string, int> genderdict = fill_genderdict();
            Dictionary<string, int> agedict = fill_agedict();

            Dictionary<string, int> typedict = new Dictionary<string, int>();
            var qa = from c in db.OV_stafftype select c;
            //int itype = qa.Count() + 1;
            foreach (OV_stafftype os in qa)
                typedict.Add(os.Name, os.Id);

            List<string> files = util.get_filelist(ukafolder);
            foreach (string fn in files)
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                if (!fn.Contains("personal-helar"))
                    continue;
                string rex = @"_(\d{4})_";
                int year = -1;
                foreach (Match match in Regex.Matches(fn, rex))
                {
                    year = util.tryconvert(match.Groups[1].Value);
                }
                memo("year = " + year);

                var q = from c in db.OV_staff where c.Year == year select c;
                if (q.Count() > 0)
                {
                    memo("Skipping " + year);
                    continue;
                }
                else
                    memo("Doing " + year);

                int nline = 0;
                using (StreamReader sr = new StreamReader(fn))
                {
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        nline++;
                        if (nline % 100 == 0)
                        {
                            memo(nline + " lines");
                            //break; //#########################################
                        }
                        string[] words = line.Split('\t');
                        int iuni = getuni(words[1], db);
                        if (iuni < 0)
                            continue;
                        //memo("iuni = " + iuni);

                        int itype = -1;
                        if (String.IsNullOrEmpty(words[2]))
                            words[2] = "Total";

                        //memo("words2 = " + words[2]);
                        if (typedict.ContainsKey(words[2]))
                        {
                            itype = typedict[words[2]];
                            //memo("1");
                        }
                        else
                        {
                            OV_stafftype ii = (from c in db.OV_stafftype where c.Name == words[2] select c).FirstOrDefault();
                            if (ii != null)
                            {
                                itype = ii.Id;
                                //memo("2");
                            }
                            else
                            {
                                //memo("3");
                                itype = (from c in db.OV_stafftype select c.Id).Max() + 1;
                                OV_stafftype oi = new OV_stafftype();
                                oi.Id = itype;
                                oi.Name = words[2];
                                db.OV_stafftype.InsertOnSubmit(oi);
                                db.SubmitChanges();
                            }
                            typedict.Add(words[2], itype);
                        }
                        //memo("iincome = " + iincome);


                        float staffnumber = (float)util.tryconvertdouble(words[5]);
                        if (staffnumber <= 0) //conversion failed
                            continue;

                        OV_staff ou = new OV_staff();
                        ou.Id = ihh;
                        ihh++;
                        ou.Uni = iuni;
                        ou.Stafftype = itype;
                        ou.Gender = genderdict[words[3]];
                        ou.Age = agedict[words[4].Replace('–','-')];
                        ou.Year = year;
                        ou.Number = staffnumber;
                        db.OV_staff.InsertOnSubmit(ou);
                    }
                    db.SubmitChanges();
                    register_fileentry(fn);
                }
            }

        }

        private void Sickbutton_Click(object sender, EventArgs e)
        {
            int n = 0;
            int ioe = 1;
            var qi = from c in db.OV_sjuk select c.Id;
            if (qi.Count() > 0)
                ioe = qi.Max() + 1;

            //string fn = nvfolder + "Sjukfrånvaro per lärosäte from 2016100-2.txt"; 
            string fn = nvfolder + "stkt-sjfv-oppen-data-2020-f02-2.txt";
            using (StreamReader sr = new StreamReader(fn))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                //sr.ReadLine();
                //sr.ReadLine();
                //sr.ReadLine();
                string yearline = sr.ReadLine();

                string[] yearwords = yearline.Split('\t');
                int[] years = new int[yearwords.Length];
                for (int i = 0; i < yearwords.Length; i++)
                {
                    years[i] = util.tryconvert(yearwords[i]);
                }
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    int iuni = getuni(words[0], db);
                    if (iuni < 0)
                        continue;

                    for (int i = 1; i < yearwords.Length; i++)
                    {
                        if (years[i] > 0)
                        {
                            float k = (float)util.tryconvertdouble(words[i]);
                            //memo("k=" + k);
                            if (k > 0)
                            {
                                OV_sjuk oe = new OV_sjuk();
                                oe.Id = ioe;
                                ioe++;
                                oe.Uni = iuni;
                                oe.Year = years[i];
                                oe.Number = k;
                                db.OV_sjuk.InsertOnSubmit(oe);
                            }
                        }
                    }
                    db.SubmitChanges();

                    n++;


                }
                register_fileentry(fn);
            }
            memo("n " + n);


        }

        private void Salarybutton_Click(object sender, EventArgs e)
        {
            int n = 0;
            int ioe = 1;
            var qi = from c in db.OV_income select c.Id;
            if (qi.Count() > 0)
                ioe = qi.Max() + 1;
            Dictionary<string, int> etypedict = new Dictionary<string, int>();
            foreach (OV_incomeclass oe in (from c in db.OV_incomeclass select c))
            {
                etypedict.Add(oe.Name, oe.Id);
            }

            string fn = bakframgrundfolder + "inkomst-år3.txt";
            using (StreamReader sr = new StreamReader(fn))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string yearline = sr.ReadLine();
                string[] yearwords = yearline.Split('\t');
                int[] years = new int[yearwords.Length];
                for (int i = 0; i < yearwords.Length; i++)
                {
                    years[i] = util.tryconvert(yearwords[i]);
                }
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 250)
                    //    Console.WriteLine(line);
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    if (!etypedict.ContainsKey(words[0]))
                    {
                        OV_incomeclass oet = new OV_incomeclass();
                        if (etypedict.Count > 0)
                            oet.Id = etypedict.Values.Max() + 1;
                        else
                            oet.Id = 1;
                        oet.Name = words[0];
                        etypedict.Add(oet.Name, oet.Id);
                        db.OV_incomeclass.InsertOnSubmit(oet);
                        db.SubmitChanges();
                    }

                    int etype = etypedict[words[0]];
                    int iuni = getuni(words[1], db);
                    if (iuni < 0)
                        continue;

                    for (int i = 2; i < yearwords.Length; i++)
                    {
                        if (years[i] > 0)
                        {
                            int k = util.tryconvertbf(words[i]);
                            if (k > 0)
                            {
                                OV_income oe = new OV_income();
                                oe.Id = ioe;
                                ioe++;
                                oe.Uni = iuni;
                                oe.Income3y = etype;
                                oe.Year = years[i];
                                oe.Number = k;
                                db.OV_income.InsertOnSubmit(oe);
                            }
                        }
                    }
                    db.SubmitChanges();

                    n++;


                }
                register_fileentry(fn);
            
            }
            memo("n " + n);


        }

        private void Backgroundbutton_Click(object sender, EventArgs e)
        {
            //There are four different versions of this function, to read different versions of the input data. Check the filename.
            int n = 0;
            int ioe = 1;
            var qi = from c in db.OV_studentcohort select c.Id;
            if (qi.Count() > 0)
                ioe = qi.Max() + 1;
            Dictionary<string, int> foreigndict = new Dictionary<string, int>();
            foreach (OV_foreigntype oe in (from c in db.OV_foreigntype select c))
            {
                foreigndict.Add(oe.Name, oe.Id);
            }
            Dictionary<string, int> cgdict = new Dictionary<string, int>();
            int cgmax = 0;
            foreach (OV_creditgroup oe in (from c in db.OV_creditgroup select c))
            {
                cgdict.Add(oe.Name, oe.Id);
                if (oe.Id > cgmax)
                    cgmax = oe.Id;
            }
            Dictionary<string, int> genderdict = fill_genderdict();

            //First by credit group:

            string fn = bakframgrundfolder + "cohort-hp-background2005-2018.txt";
            int regid = register_fileentry(fn);

                
            using (StreamReader sr = new StreamReader(fn))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string genderline = sr.ReadLine();
                string[] genderwords = genderline.Split('\t');
                int[] genders = new int[genderline.Length];
                for (int i=0;i<genderwords.Length;i++)
                {
                    if (genderwords[i].Contains("Kvinnor"))
                        genders[i] = 1;
                    else if (genderwords[i].Contains("Män"))
                        genders[i] = 2;
                    else
                        genders[i] = -1;
                }

                //string yearline = sr.ReadLine();

                //string[] yearwords = yearline.Split('\t');
                //int[] years = new int[yearwords.Length];
                //for (int i = 0; i < yearwords.Length; i++)
                //{
                //    if (yearwords[i].Length > 4)
                //        years[i] = util.tryconvert(yearwords[i].Substring(0, 4));
                //    else
                //        years[i] = -1;
                //}
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    int iuni = getuni(words[3], db);
                    if (iuni < 0)
                        continue;

                    //bool exam = (words[1].Trim() == "Ja");
                    //bool eduparent = words[2].Contains("ftergymnasial");

                    //int foreign = 0;
                    //if (foreigndict.ContainsKey(words[2]))
                    //    foreign = foreigndict[words[2]];
                    bool educated = words[2].Contains("ftergymn");

                    string cg = words[0];
                    if (String.IsNullOrEmpty(cg))
                        continue;
                    int cgid = -1;
                    if (cgdict.ContainsKey(cg))
                        cgid = cgdict[cg];
                    else
                    {
                        cgmax++;
                        cgid = cgmax;
                        OV_creditgroup og = new OV_creditgroup();
                        og.Id = cgmax;
                        og.Name = cg;
                        db.OV_creditgroup.InsertOnSubmit(og);
                        db.SubmitChanges();
                        cgdict.Add(og.Name,og.Id);
                    }

                    int year = util.tryconvert(words[1]);
                    if (year <= 0)
                        continue;

                    for (int i = 4; i <= 7; i++)
                    {
                        int k = util.tryconvertbf(words[i]);
                        //memo("k=" + k);
                        if (k > 0)
                        {
                            OV_studentcohort oe = new OV_studentcohort();
                            oe.Id = ioe;
                            ioe++;
                            oe.Uni = iuni;
                            if (i >= 6)
                                oe.Foreignbackground = 7;
                            else
                                oe.Foreignbackground = 6;
                            oe.Gender = genders[i];
                            oe.Educatedparent = educated;
                            oe.Progfk = true;
                            oe.Year = year;
                            oe.Examyear = null;
                            oe.Number = k;
                            oe.Exam = null;
                            oe.Creditgroup = cgid;
                            oe.Source = regid;
                            db.OV_studentcohort.InsertOnSubmit(oe);

                            //sum all credit groups
                            OV_studentcohort oe2 = (from c in db.OV_studentcohort
                                      where c.Creditgroup == null
                                      where c.Uni == oe.Uni
                                      where c.Foreignbackground == oe.Foreignbackground
                                      where c.Gender == oe.Gender
                                      where c.Educatedparent == oe.Educatedparent
                                      where c.Year == oe.Year
                                      where c.Examyear == null select c).FirstOrDefault();
                            if (oe2 == null)
                            {
                                oe2 = new OV_studentcohort();
                                oe2.Id = ioe;
                                ioe++;
                                oe2.Uni = oe.Uni;
                                oe2.Foreignbackground = oe.Foreignbackground;
                                oe2.Gender = oe.Gender;
                                oe2.Educatedparent = oe.Educatedparent;
                                oe2.Year = oe.Year;
                                oe2.Examyear = null;
                                oe2.Number = k;
                                oe2.Creditgroup = null;
                                oe2.Progfk = true;
                                oe2.Source = oe.Source;
                                db.OV_studentcohort.InsertOnSubmit(oe2);
                            }
                            else
                                oe2.Number += k;
                                      
                        }
                    }
                    db.SubmitChanges();

                    n++;


                }
            }
            memo("n " + n);
            n = 0;


            //... then by examyear:

            string fn2 = bakframgrundfolder + "cohort-examyear-background2005-2014.txt";
            int regid2 = register_fileentry(fn2);


            using (StreamReader sr = new StreamReader(fn2))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string genderline = sr.ReadLine();
                string[] genderwords = genderline.Split('\t');
                int[] genders = new int[genderline.Length];
                for (int i = 0; i < genderwords.Length; i++)
                {
                    if (genderwords[i].Contains("Kvinnor"))
                        genders[i] = 1;
                    else if (genderwords[i].Contains("Män"))
                        genders[i] = 2;
                    else
                        genders[i] = -1;
                }

                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    int iuni = getuni(words[3], db);
                    if (iuni < 0)
                        continue;

                    bool educated = words[2].Contains("ftergymn");

                    int examyear = util.tryconvert(words[0]);
                    if (examyear <= 0)
                        continue;

                    int year = util.tryconvert(words[1]);
                    if (year <= 0)
                        continue;

                    for (int i = 4; i <= 7; i++)
                    {
                        int k = util.tryconvertbf(words[i]);
                        //memo("k=" + k);
                        if (k > 0)
                        {
                            OV_studentcohort oe = new OV_studentcohort();
                            oe.Id = ioe;
                            ioe++;
                            oe.Uni = iuni;
                            if (i >= 6)
                                oe.Foreignbackground = 7;
                            else
                                oe.Foreignbackground = 6;
                            oe.Gender = genders[i];
                            oe.Educatedparent = educated;
                            oe.Progfk = true;
                            oe.Year = year;
                            oe.Examyear = examyear;
                            oe.Number = k;
                            oe.Exam = null;
                            oe.Creditgroup = null;
                            oe.Source = regid2;
                            db.OV_studentcohort.InsertOnSubmit(oe);

                        }
                    }
                    db.SubmitChanges();

                    n++;


                }
            }
            memo("n " + n);

        }

        private void Backgroundbutton_Click3(object sender, EventArgs e)
        {
            //There are four different versions of this function, to read different versions of the input data. Check the filename.
            int n = 0;
            int ioe = 1;
            var qi = from c in db.OV_studentbackground select c.Id;
            if (qi.Count() > 0)
                ioe = qi.Max() + 1;
            Dictionary<string, int> foreigndict = new Dictionary<string, int>();
            foreach (OV_foreigntype oe in (from c in db.OV_foreigntype select c))
            {
                foreigndict.Add(oe.Name, oe.Id);
            }
            Dictionary<string, int> cgdict = new Dictionary<string, int>();
            int cgmax = 0;
            foreach (OV_creditgroup oe in (from c in db.OV_creditgroup select c))
            {
                cgdict.Add(oe.Name, oe.Id);
                if (oe.Id > cgmax)
                    cgmax = oe.Id;
            }
            Dictionary<string, int> genderdict = fill_genderdict();

            string fn = bakframgrundfolder + "studbakgrund-poänggrupp.txt";

            using (StreamReader sr = new StreamReader(fn))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string genderline = sr.ReadLine();
                string[] genderwords = genderline.Split('\t');
                int[] genders = new int[genderline.Length];
                for (int i = 0; i < genderwords.Length; i++)
                {
                    if (genderwords[i].Contains("Kvinnor"))
                        genders[i] = 1;
                    else if (genderwords[i].Contains("Män"))
                        genders[i] = 2;
                    else
                        genders[i] = -1;
                }

                //string yearline = sr.ReadLine();

                //string[] yearwords = yearline.Split('\t');
                //int[] years = new int[yearwords.Length];
                //for (int i = 0; i < yearwords.Length; i++)
                //{
                //    if (yearwords[i].Length > 4)
                //        years[i] = util.tryconvert(yearwords[i].Substring(0, 4));
                //    else
                //        years[i] = -1;
                //}
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    int iuni = getuni(words[3], db);
                    if (iuni < 0)
                        continue;

                    //bool exam = (words[1].Trim() == "Ja");
                    //bool eduparent = words[2].Contains("ftergymnasial");

                    int foreign = 0;
                    if (foreigndict.ContainsKey(words[2]))
                        foreign = foreigndict[words[2]];

                    string cg = words[0];
                    if (String.IsNullOrEmpty(cg))
                        continue;
                    int cgid = -1;
                    if (cgdict.ContainsKey(cg))
                        cgid = cgdict[cg];
                    else
                    {
                        cgmax++;
                        cgid = cgmax;
                        OV_creditgroup og = new OV_creditgroup();
                        og.Id = cgmax;
                        og.Name = cg;
                        db.OV_creditgroup.InsertOnSubmit(og);
                        db.SubmitChanges();
                        cgdict.Add(og.Name, og.Id);
                    }

                    int year = util.tryconvert(words[1]);
                    if (year <= 0)
                        continue;

                    for (int i = 4; i <= 7; i++)
                    {
                        int k = util.tryconvertbf(words[i]);
                        //memo("k=" + k);
                        if (k > 0)
                        {
                            OV_studentbackground oe = new OV_studentbackground();
                            oe.Id = ioe;
                            ioe++;
                            oe.Uni = iuni;
                            oe.Foreignbackground = foreign;
                            oe.Gender = genders[i];
                            oe.Educatedparent = (i >= 6);
                            oe.Year = year;
                            oe.Number = k;
                            oe.Exam = null;
                            oe.Progfk = null;
                            oe.Creditgroup = cgid;
                            db.OV_studentbackground.InsertOnSubmit(oe);
                        }
                    }
                    db.SubmitChanges();

                    n++;


                }
                register_fileentry(fn);

            }
            memo("n " + n);

        }

        private void Backgroundbutton_Click_OLD(object sender, EventArgs e)
        {
            int n = 0;
            int ioe = 1;
            var qi = from c in db.OV_studentbackground select c.Id;
            if (qi.Count() > 0)
                ioe = qi.Max() + 1;
            Dictionary<string, int> foreigndict = new Dictionary<string, int>();
            foreach (OV_foreigntype oe in (from c in db.OV_foreigntype select c))
            {
                foreigndict.Add(oe.Name, oe.Id);
            }
            Dictionary<string, int> genderdict = fill_genderdict();

            string fn = bakframgrundfolder + "studentbakgrund-progfk.txt";

            using (StreamReader sr = new StreamReader(fn))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string genderline = sr.ReadLine();
                string[] genderwords = genderline.Split('\t');
                int[] genders = new int[genderline.Length];
                for (int i = 0; i < genderwords.Length; i++)
                {
                    if (genderwords[i].Contains("Kvinnor"))
                        genders[i] = 1;
                    else if (genderwords[i].Contains("Män"))
                        genders[i] = 2;
                    else
                        genders[i] = -1;
                }

                string yearline = sr.ReadLine();

                string[] yearwords = yearline.Split('\t');
                int[] years = new int[yearwords.Length];
                for (int i = 0; i < yearwords.Length; i++)
                {
                    if (yearwords[i].Length > 4)
                        years[i] = util.tryconvert(yearwords[i].Substring(0, 4));
                    else
                        years[i] = -1;
                }
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    int iuni = getuni(words[0], db);
                    if (iuni < 0)
                        continue;

                    bool exam = (words[1].Trim() == "Ja");
                    bool eduparent = words[2].Contains("ftergymnasial");

                    int foreign = 0;
                    if (words[3].Contains("två utrikes"))
                        foreign = 6;
                    else if (words[3].Contains("två inrikes"))
                        foreign = 7;

                    bool progfk = words[4].Contains("Program");

                    for (int i = 2; i < yearwords.Length; i++)
                    {
                        if (years[i] > 0)
                        {
                            int k = util.tryconvertbf(words[i]);
                            //memo("k=" + k);
                            if (k > 0)
                            {
                                OV_studentbackground oe = new OV_studentbackground();
                                oe.Id = ioe;
                                ioe++;
                                oe.Uni = iuni;
                                oe.Foreignbackground = foreign;
                                oe.Gender = genders[i];
                                oe.Educatedparent = eduparent;
                                oe.Year = years[i];
                                oe.Number = k;
                                oe.Exam = exam;
                                oe.Progfk = progfk;
                                db.OV_studentbackground.InsertOnSubmit(oe);
                            }
                        }
                    }
                    db.SubmitChanges();

                    n++;


                }
                register_fileentry(fn);

            }
            memo("n " + n);

        }

        private void Backgroundbutton_Click_OLDER(object sender, EventArgs e)
        {
            int n = 0;
            int ioe = 1;
            var qi = from c in db.OV_studentbackground select c.Id;
            if (qi.Count() > 0)
                ioe = qi.Max() + 1;
            Dictionary<string, int> foreigndict = new Dictionary<string, int>();
            foreach (OV_foreigntype oe in (from c in db.OV_foreigntype select c))
            {
                foreigndict.Add(oe.Name, oe.Id);
            }
            Dictionary<string, int> genderdict = fill_genderdict();

            string fn = bakframgrundfolder + "examensfrekvens-bakgrund-alltalla.txt";
            using (StreamReader sr = new StreamReader(fn))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string yearline = sr.ReadLine();

                string[] yearwords = yearline.Split('\t');
                int[] years = new int[yearwords.Length];
                for (int i = 0; i < yearwords.Length; i++)
                {
                    if (yearwords[i].Length > 4)
                        years[i] = util.tryconvert(yearwords[i].Substring(0, 4));
                    else
                        years[i] = -1;
                }
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    int iuni = getuni(words[4], db);
                    if (iuni < 0)
                        continue;

                    bool exam = (words[0].Trim() == "Ja");
                    bool eduparent = words[1].Contains("ftergymnasial");

                    if (!foreigndict.ContainsKey(words[2].Trim()))
                    {
                        OV_foreigntype of = new OV_foreigntype();
                        of.Id = 1;
                        if (foreigndict.Count > 0)
                            of.Id = foreigndict.Values.Max() + 1;
                        of.Name = words[2].Trim();
                        db.OV_foreigntype.InsertOnSubmit(of);
                        db.SubmitChanges();
                        foreigndict.Add(words[2].Trim(), of.Id);
                    }

                    for (int i = 2; i < yearwords.Length; i++)
                    {
                        if (years[i] > 0)
                        {
                            int k = util.tryconvertbf(words[i]);
                            //memo("k=" + k);
                            if (k > 0)
                            {
                                OV_studentbackground oe = new OV_studentbackground();
                                oe.Id = ioe;
                                ioe++;
                                oe.Uni = iuni;
                                oe.Foreignbackground = foreigndict[words[2].Trim()];
                                oe.Gender = genderdict[words[3].Trim()];
                                oe.Educatedparent = eduparent;
                                oe.Year = years[i];
                                oe.Number = k;
                                oe.Exam = exam;
                                db.OV_studentbackground.InsertOnSubmit(oe);
                            }
                        }
                    }
                    db.SubmitChanges();

                    n++;


                }
                register_fileentry(fn);
            
            }
            memo("n " + n);

        }

        private void Pricebutton_Click(object sender, EventArgs e)
        {
            int n = 0;

            string fn = nvfolder + "sulf-plo.v2.txt";
            using (StreamReader sr = new StreamReader(fn))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    int year = -1;
                    if (words[0].Length > 4)
                        year = 1996;
                    else if ( words[0].Length == 4)
                        year = util.tryconvert(words[0]);

                    if (year < 0)
                        continue;

                    OV_price op = (from c in db.OV_price where c.Year == year select c).FirstOrDefault();
                    bool newyear = (op == null);
                    if ( newyear)
                    {
                        op = new OV_price();
                        op.Year = year;
                    }
                    op.AKI = util.tryconvertdouble(words[4]);
                    op.Prodavdrag = util.tryconvertdouble(words[5]);
                    op.PLO = util.tryconvertdouble(words[6]);
                    op.KPI = util.tryconvertdouble(words[7]);
                    if (newyear)
                        db.OV_price.InsertOnSubmit(op);
                    db.SubmitChanges();

                    n++;
                }
            }
            memo("n " + n);


        }

        private void Demographybutton_Click(object sender, EventArgs e)
        {
            int n = 0;
            int ioe = 1;
            var qi = from c in db.OV_demography select c.Id;
            if (qi.Count() > 0)
                ioe = qi.Max() + 1;

            string fn = nvfolder + "scb-pop-total.txt";

            memo(fn);
            using (StreamReader sr = new StreamReader(fn))
            {
                string hline = "";
                do
                    hline = sr.ReadLine();
                while (!hline.StartsWith("#####"));

                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');
                    if (words.Length < 4)
                        continue;

                    int year = util.tryconvert(words[0]);

                    OV_demography od = new OV_demography();
                    od.Id = ioe;
                    ioe++;
                    od.Lan = 0;
                    od.Year = year;
                    od.Age = null;
                    od.Foreignbackground = null;
                    od.Educated = null;
                    od.Gender = null;
                    od.Number = util.tryconvert(words[3]);
                    db.OV_demography.InsertOnSubmit(od);


                    db.SubmitChanges();

                    n++;


                }
                register_fileentry(fn);

            }
            memo("n  " + n);

            ///// OBS! disabled the rest
            ///// OBS! disabled the rest
            return;
            ///// OBS! disabled the rest
            ///// OBS! disabled the rest
            
            Dictionary<string, int> foreigndict = new Dictionary<string, int>();
            foreach (OV_foreigntype oe in (from c in db.OV_foreigntype select c))
            {
                foreigndict.Add(oe.Name, oe.Id);
            }
            Dictionary<string, int> genderdict = fill_genderdict(true);
            Dictionary<string, int> agedict = fill_agedict();

            fn = nvfolder + "scb-pop-utländsk-svensk.txt";
            memo(fn);
            using (StreamReader sr = new StreamReader(fn))
            {
                string yearline = sr.ReadLine();

                string[] yearwords = yearline.Split('\t');
                int[] years = new int[yearwords.Length];
                for (int i = 0; i < yearwords.Length; i++)
                {
                    if (yearwords[i].Length > 4)
                        years[i] = util.tryconvert(yearwords[i].Trim('"'));
                    else
                        years[i] = -1;
                }
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    int ilan = util.tryconvert(words[0].Substring(1, 2));

                    //bool eduparent = words[1].Contains("ftergymnasial");

                    if (!foreigndict.ContainsKey(words[1].Trim('"')))
                    {
                        OV_foreigntype of = new OV_foreigntype();
                        of.Id = 1;
                        if (foreigndict.Count > 0)
                            of.Id = foreigndict.Values.Max() + 1;
                        of.Name = words[1].Trim('"');
                        db.OV_foreigntype.InsertOnSubmit(of);
                        db.SubmitChanges();
                        foreigndict.Add(words[1].Trim('"'), of.Id);
                    }
                    if (!agedict.ContainsKey(words[2].Trim('"')))
                    {
                        OV_age of = new OV_age();
                        of.Id = 1;
                        if (agedict.Count > 0)
                            of.Id = agedict.Values.Max() + 1;
                        of.Name = words[2].Trim('"');
                        db.OV_age.InsertOnSubmit(of);
                        db.SubmitChanges();
                        agedict.Add(words[2].Trim('"'), of.Id);
                    }

                    for (int i = 2; i < yearwords.Length; i++)
                    {
                        if (years[i] > 0)
                        {
                            int k = util.tryconvertbf(words[i]);
                            //memo("k=" + k);
                            if (k > 0)
                            {
                                OV_demography oe = new OV_demography();
                                oe.Id = ioe;
                                ioe++;
                                oe.Lan = ilan;
                                oe.Foreignbackground = foreigndict[words[1].Trim('"')];
                                oe.Age = agedict[words[2].Trim('"')];
                                oe.Gender = genderdict[words[3].Trim('"')];
                                //oe.Educatedparent = eduparent;
                                oe.Year = years[i];
                                oe.Number = k;
                                //oe.Exam = exam;
                                db.OV_demography.InsertOnSubmit(oe);
                            }
                        }
                    }
                    db.SubmitChanges();

                    n++;


                }
                register_fileentry(fn);

            }
            memo("n  " + n);

            string fn2 = nvfolder + "scb-pop-utbildning.txt";
            memo(fn2);
            using (StreamReader sr = new StreamReader(fn2))
            {
                string yearline = sr.ReadLine();

                string[] yearwords = yearline.Split('\t');
                int[] years = new int[yearwords.Length];
                for (int i = 0; i < yearwords.Length; i++)
                {
                    if (yearwords[i].Length > 4)
                        years[i] = util.tryconvert(yearwords[i].Trim('"'));
                    else
                        years[i] = -1;
                }
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    int ilan = util.tryconvert(words[0].Substring(1, 2));

                    //bool eduparent = words[1].Contains("ftergymnasial");

                    //if (!foreigndict.ContainsKey(words[1].Trim('"')))
                    //{
                    //    OV_foreigntype of = new OV_foreigntype();
                    //    of.Id = 1;
                    //    if (foreigndict.Count > 0)
                    //        of.Id = foreigndict.Values.Max() + 1;
                    //    of.Name = words[1].Trim('"');
                    //    db.OV_foreigntype.InsertOnSubmit(of);
                    //    db.SubmitChanges();
                    //    foreigndict.Add(words[1].Trim('"'), of.Id);
                    //}
                    if (!agedict.ContainsKey(words[1].Trim('"')))
                    {
                        OV_age of = new OV_age();
                        of.Id = 1;
                        if (agedict.Count > 0)
                            of.Id = agedict.Values.Max() + 1;
                        of.Name = words[1].Trim('"');
                        db.OV_age.InsertOnSubmit(of);
                        db.SubmitChanges();
                        agedict.Add(words[1].Trim('"'), of.Id);
                    }

                    bool educated = false;
                    if (words[2].Contains("ftergymn"))
                        educated = true;
                    else if (words[2].Contains("orskarutb"))
                        educated = true;


                    for (int i = 2; i < yearwords.Length; i++)
                    {
                        if (years[i] > 0)
                        {
                            int k = util.tryconvertbf(words[i]);
                            //memo("k=" + k);
                            if (k > 0)
                            {
                                OV_demography oe = new OV_demography();
                                oe.Id = ioe;
                                ioe++;
                                oe.Lan = ilan;
                                //oe.Foreignbackground = foreigndict[words[1].Trim('"')];
                                oe.Age = agedict[words[1].Trim('"')];
                                oe.Gender = genderdict[words[3].Trim('"')];
                                oe.Educated = educated;
                                oe.Year = years[i];
                                oe.Number = k;
                                //oe.Exam = exam;
                                db.OV_demography.InsertOnSubmit(oe);
                            }
                        }
                    }
                    db.SubmitChanges();

                    n++;


                }
                register_fileentry(fn2);

            }
            memo("n  " + n);


        }

        private void fixsubjectcoding(string keystring, int oldsubject, int newsubject, int newsector)
        {
            var q =
                from c in db.OV_course
                where c.Subject == oldsubject
                where c.Name.Contains(keystring)
                select c;
            foreach (OV_course c in q)
            {
                c.Subject = newsubject;
                c.Sector = newsector;
            }
            db.SubmitChanges();
            memo("Done " + keystring);
        }

        private void fixsubjectcoding(List<string> keystring, int oldsubject, int newsubject, int newsector)
        {
            var q =
                from c in db.OV_course
                select c;
            if (oldsubject > 0)
                q = from c in q
                    where c.Subject == oldsubject
                    select c;
            foreach (string key in keystring)
                q = from c in q
                    where c.Name.ToLower().Contains(key)
                    select c;
            foreach (OV_course c in q)
            {
                if (c.Subject != newsubject)
                {
                    c.Subject = newsubject;
                    c.Sector = newsector;
                }
            }
            db.SubmitChanges();
            memo("Done " + keystring);
        }

        private void Fixbutton_Click(object sender, EventArgs e)
        {
            //fixsubjectcoding("askinteknik", 149, 74, 2);
            //fixsubjectcoding("askiningenjör", 149, 74, 2);
            //fixsubjectcoding("aterialteknik", 149, 76, 2);
            //fixsubjectcoding("aterialdesign", 149, 76, 2);
            //fixsubjectcoding("aterialvetenskap", 149, 76, 2);

            //int n = 0;
            //for (int year = 2016; year < 2020; year++)
            //{
            //    int dup = 0;
            //    var q = from c in db.OV_course where c.Year == year select c;
            //    foreach (OV_course cc in q)
            //    {
            //        n++;
            //        if (n % 100 == 0)
            //            memo("n=" + n);
            //        //memo(cc.Name);
            //        if (cc.OV_applicants.Count != 5)
            //            dup++;
            //    }
            //    //var duplicates = (from r in q
            //    //                  group r by new { r.Course, r.Gender, r.Age } into results
            //    //                  select results.Skip(1)
            //    //     ).SelectMany(a => a);
            //    //var duplicates = (from r in q
            //    //                  group r by new { r.Course} into results
            //    //                  select results.Skip(5)
            //    //     ).SelectMany(a => a);

            //    memo(year + " duplicates = " + dup);
            //}


            //var qsa = from c in db.OV_subjectarea select c;
            //Dictionary<int, int> syndict = new Dictionary<int, int>();
            //foreach (string s in FormSelectData.subjectsynonyms.Keys)
            //{
            //    int i0 = (from c in qsa where c.Name == s select c.Id).First();
            //    int i1 = (from c in qsa where c.Name == FormSelectData.subjectsynonyms[s] select c.Id).First();
            //    syndict.Add(i0, i1);
            //}

            //int n = 0;
            //foreach (OV_hsthpr ohh in from c in db.OV_hsthpr select c)
            //{
            //    if (syndict.ContainsKey(ohh.Area))
            //        ohh.Area = syndict[ohh.Area];
            //    n++;
            //    if (n % 100 == 0)
            //    {
            //        memo(n.ToString());
            //        db.SubmitChanges();
            //    }
            //}
                


            //dbContext.tbl_mytable.DeleteAllOnSubmit(duplicates);
            //dbContext.SubmitChanges();

            
        }

        private void Studentflow_Engineersbutton_Click(object sender, EventArgs e)
        {
            int n = 0;
            int ioe = 1;
            var qi = from c in db.OV_studentflow select c.Id;
            if (qi.Count() > 0)
                ioe = qi.Max() + 1;
            Dictionary<string, int> landict = new Dictionary<string, int>();
            foreach (OV_Lan oe in (from c in db.OV_Lan select c))
            {
                landict.Add(oe.Name.ToLower(), oe.Id);
            }

            string fn = bakframgrundfolder + "martin-ing.txt";
            using (StreamReader sr = new StreamReader(fn))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string tolanline = sr.ReadLine();

                string[] tolanwords = tolanline.Split('\t');
                int[] tolan = new int[tolanwords.Length];
                for(int i=0;i< tolanwords.Length;i++)
                {
                    if (landict.ContainsKey(tolanwords[i].Trim().ToLower()))
                        tolan[i] = landict[tolanwords[i].Trim().ToLower()];
                    else
                        tolan[i] = -1;
                }

                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    //if (n > 10)
                    //    break;
                    if (n % 100 == 0)
                        memo(n.ToString());

                    string[] words = line.Split('\t');

                    if (!landict.ContainsKey(words[0].Trim().ToLower()))
                    {
                        memo(words[0] + " unknown län 0");
                        continue;
                    }
                    int fromlan = landict[words[0].Trim().ToLower()];

                    for (int i = 1; i < tolanwords.Length; i++)
                    {
                        if (tolan[i] >= 0)
                        {
                            int k = util.tryconvertbf(words[i]);
                            //memo("k=" + k);
                            if (k > 0)
                            {
                                memo("Från "+ words[0].Trim() + " [" + k + "] " + "Till "+tolanwords[i].Trim());
                            }
                        }
                    }

                    n++;


                }
                register_fileentry(fn);

            }
            memo("n " + n);

        }

        private string makeyearline(string name,int minyear,int maxyear)
        {
            string s = name.PadRight(20)+"\t"+minyear+" - "+maxyear;
            return s;
        }

        private void Yearbutton_Click(object sender, EventArgs e)
        {
            int minyear = (int)(from c in db.Course select c.Year).Min();
            int maxyear = (int)(from c in db.Course select c.Year).Max();
            memo(makeyearline("Course", minyear, maxyear));

            minyear = (int)(from c in db.CourseTGS select c.CourseCourse.Year).Min();
            maxyear = (int)(from c in db.CourseTGS select c.CourseCourse.Year).Max();
            memo(makeyearline("CourseTGS", minyear, maxyear));

            minyear = (int)(from c in db.Egetamne select c.SubjectbudgetSubjectbudget.Year).Min();
            maxyear = (int)(from c in db.Egetamne select c.SubjectbudgetSubjectbudget.Year).Max();
            memo(makeyearline("Egetamne", minyear, maxyear));
            
            minyear = (int)(from c in db.OV_antagningspoang select c.OV_course.Year).Min();
            maxyear = (int)(from c in db.OV_antagningspoang select c.OV_course.Year).Max();
            memo(makeyearline("OV_antagningspoang", minyear, maxyear));
            
            var q = from c in db.OV_applicants select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_applicants", minyear, maxyear));

            q = from c in db.OV_course select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            var qvt = from c in db.OV_course where c.Year == maxyear where !c.HT select c;
            var qht = from c in db.OV_course where c.Year == maxyear where c.HT select c;
            string ss = " ";
            if (qvt.Count() > 0)
                ss += "VT";
            if (qht.Count() > 0)
                ss += "+HT";
            memo(makeyearline("OV_course", minyear, maxyear)+ss);

            q = from c in db.OV_demography select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_demography", minyear, maxyear));

            q = from c in db.OV_establishment select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_establishment", minyear, maxyear));

            q = from c in db.OV_exam select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_exam", minyear, maxyear));

            q = from c in db.OV_hsthpr select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_hsthpr", minyear, maxyear));

            q = from c in db.OV_income select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_income", minyear, maxyear));

            q = from c in db.OV_price select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_price", minyear, maxyear));

            q = from c in db.OV_publication select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_publication", minyear, maxyear));

            q = from c in db.OV_sjuk select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_sjuk", minyear, maxyear));

            q = from c in db.OV_staff select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_staff", minyear, maxyear));

            q = from c in db.OV_studentbackground select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_studentbackground", minyear, maxyear));

            q = from c in db.OV_studentcohort select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_studentcohort", minyear, maxyear));

            q = from c in db.OV_studentflow select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_studentflow", minyear, maxyear));

            q = from c in db.OV_University_Income select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_University_Income", minyear, maxyear));

            q = from c in db.OV_VRbibliometry select c.Year;
            minyear = q.Min();
            maxyear = q.Max();
            memo(makeyearline("OV_VRbibliometry", minyear, maxyear));

            FormSelectData.getsource(new string[] { "dummy" }, false);
            foreach (string s in FormSelectData.sourcedict.Keys)
                memo(s+" "+FormSelectData.getsource(new string[] { s },true));
        }

        private void FormFillDB_Load(object sender, EventArgs e)
        {

        }

        

        private void button1_Click(object sender, EventArgs e) //SWEPUB
        {
            if (pubclass.afflist.Count == 0)
                pubclass.readaffs(swepubfolder + "afflist.txt");
            if (pubclass.unidict.Count == 0)
                pubclass.readuni(swepubfolder+"lärosäten.txt");

            List<int> dbuni = (from c in db.OV_University
                               where c.Mergedwith == null
                               select c.Id).ToList();

            Dictionary<string, int> subjectdictUKA = new Dictionary<string, int>();
            Dictionary<int,string> subjectdictUKA2 = new Dictionary<int,string>();
            var qsubj = from c in db.OV_researchsubject select c;
            foreach (OV_researchsubject or in qsubj)
            {
                subjectdictUKA.Add(or.Name, or.Id);
                subjectdictUKA2.Add(or.Id,or.Name);
            }
            subjectdictUKA2[0] = "";

            //         year           uni            pubtype              subj number
            Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<int, int>>>> d = new Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<int, int>>>>();

            for (int year = pubclass.startyear;year<= pubclass.endyear;year++)
            {
                d.Add(year, new Dictionary<int, Dictionary<string, Dictionary<int, int>>>());
                foreach (int iuni in dbuni)
                {
                    d[year].Add(iuni, new Dictionary<string, Dictionary<int, int>>());
                    foreach (string pt in pubclass.pubtypelist)
                    {
                        d[year][iuni].Add(pt, new Dictionary<int, int>());
                        for (int isubj =0;isubj<=7;isubj++)
                        {
                            d[year][iuni][pt].Add(isubj, 0);
                        }
                    }
                }
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int n = 0;
                using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        n++;
                        if (n % 1000 == 0)
                            memo("n = " + n);
                        //if (n > 10000)
                        //    break;

                        pubclass pub = pubclass.parseJSON(line);

                        if (pub == null)
                            continue;

                        //grand total:
                        d[pub.year][0][""][0]++;

                        foreach (int isweu in pub.sweuni)
                        {
                            if (!pubclass.unidict.ContainsKey(isweu))
                            {
                                memo("Missing uni "+isweu);
                                continue;

                            }
                            int iuni = getuni(pubclass.unidict[isweu], db);

                            if (iuni < 0)
                            {
                                memo("Missing uni " + pubclass.unidict[isweu]);
                                continue;
                            }
                            d[pub.year][iuni][""][0]++;
                            string genre = pub.get_genre();
                            d[pub.year][iuni][genre][0]++;

                            foreach (string subj in pub.subjectlist)
                            {
                                int isubj = subjectdictUKA[subj];
                                d[pub.year][iuni][genre][isubj]++;
                                d[pub.year][iuni][""][isubj]++;

                            }
                        }

                        string genre2 = pub.get_genre();
                        d[pub.year][0][genre2][0]++;

                        foreach (string subj in pub.subjectlist)
                        {
                            int isubj = subjectdictUKA[subj];
                            d[pub.year][0][genre2][isubj]++;
                            d[pub.year][0][""][isubj]++;

                        }

                    }
                }

                memo("Total n = " + n);

                string header = "År	Lärosäte	Publikationstyp	Forskningsämnesområde	Kön	Åldersgrupp	Antal";
                string tab = "\t";

                for (int year = pubclass.startyear; year <= pubclass.endyear; year++)
                {
                    string fnout = swepubfolder + "UKA-format-" + year.ToString() + ".txt";
                    using (StreamWriter sw = new StreamWriter(fnout))
                    {
                        sw.WriteLine(header);
                        foreach (int iuni in dbuni)
                        {
                            foreach (string pt in pubclass.pubtypelist)
                            {
                                for (int isubj = 0; isubj <= 7; isubj++)
                                {
                                    if (d[year][iuni][pt][isubj] > 0)
                                        sw.WriteLine(year.ToString() + tab + getuni(iuni) + tab + pt + tab + subjectdictUKA2[isubj] + tab + "Total" + tab + "Total" + tab + d[year][iuni][pt][isubj].ToString());
                                }
                            }
                        }
                    }
                }

            }
        }

        private void SwepubDBbutton_Click(object sender, EventArgs e)
        {
            read_UKA_publication_files(swepubfolder, true);
        }

        public Dictionary<int, string> vsdict = new Dictionary<int, string>();

        private void fill_OV_VRsubject()
        {
            vsdict.Add(2, "Agriculture");
            vsdict.Add(3, "Biology");
            vsdict.Add(4, "Biomolecular");
            vsdict.Add(5, "Blood");
            vsdict.Add(6, "Chemistry");
            vsdict.Add(7, "Computer Science");
            vsdict.Add(8, "Dentistry");
            vsdict.Add(9, "Ecology");
            vsdict.Add(10, "Economics");
            vsdict.Add(11, "Education");
            vsdict.Add(12, "Engineering");
            vsdict.Add(13, "Engineering Mathematics");
            vsdict.Add(14, "Environmental Health");
            vsdict.Add(15, "Environmental Studies");
            vsdict.Add(16, "Ergonomics");
            vsdict.Add(17, "Geoscience");
            vsdict.Add(18, "Health");
            vsdict.Add(19, "Health Studies");
            vsdict.Add(20, "Humanities");
            vsdict.Add(21, "Immunology");
            vsdict.Add(22, "Information Science");
            vsdict.Add(23, "Materials Science");
            vsdict.Add(24, "Mathematics");
            vsdict.Add(25, "Mechanics");
            vsdict.Add(26, "Medicine, External");
            vsdict.Add(27, "Medicine, Internal");
            vsdict.Add(28, "Neuroscience");
            vsdict.Add(29, "Oncology");
            vsdict.Add(30, "Pharmacology");
            vsdict.Add(31, "Physics");
            vsdict.Add(32, "Psychology");
            vsdict.Add(33, "Social Science");
            vsdict.Add(34, "Statistics");
            vsdict.Add(35, "Surgery");
            vsdict.Add(0, "Totalt");

            var q = from c in db.OV_VRsubject select c;
            if (q.Count() == 0)
            {
                foreach (int k in vsdict.Keys)
                {
                    OV_VRsubject os = new OV_VRsubject();
                    os.Id = k;
                    os.Name = vsdict[k];
                    db.OV_VRsubject.InsertOnSubmit(os);
                }
                db.SubmitChanges();
            }
        }

        private void read_VR_fordelningsunderlag(string fn)
        {
            memo("Reading " + fn);

            string rex = @"(\d{4})";
            int year = -1;
            foreach (Match match in Regex.Matches(fn, rex))
            {
                year = util.tryconvert(match.Groups[1].Value);
            }
            memo("year = " + year);

            var qyear = from c in db.OV_VRbibliometry where c.Year == year select c;
            if (qyear.Count() > 0)
            {
                memo("Year already in db");
                return;
            }

            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook;
            //Excel.Worksheet xlvtl;
            //Excel.Worksheet xlhtl;



            xlWorkBook = xlApp.Workbooks.Open(fn, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            //xlvtl = xlWorkBook.Sheets[1];
            //xlhtl = xlWorkBook.Sheets[6];

            List<Excel.Worksheet> sheetlist = new List<Excel.Worksheet>();

            foreach (Excel.Worksheet xll in xlWorkBook.Sheets)
            {
                sheetlist.Add(xll);
            }

            Excel.Worksheet volume = xlWorkBook.Sheets["Volym"];
            Excel.Worksheet fieldnorm;
            if (year < 2017) 
            fieldnorm = xlWorkBook.Sheets["Summa Fältnorm cit"];
            else
                fieldnorm = xlWorkBook.Sheets["Summa Fältnormerade citeringar"];
            Excel.Worksheet bibindex = xlWorkBook.Sheets["Bibliometriskt index"];

            int ovrid = 1;
            var qid = from c in db.OV_VRbibliometry select c.Id;
            if (qid.Count() > 0)
                ovrid = qid.Max()+1;

            int row0 = 3;
            if (year < 2015) //Format changes...
                row0 = 2;
            else if (year > 2020) //...and again
                row0 = 4;

            for (int row = row0+1;row<= 34;row++)
            {
                memo(util.getstring(volume.Cells[row, 1]));
                int uni = getuni(util.getstring(volume.Cells[row, 1]), db);
                if (String.IsNullOrEmpty(util.getstring(volume.Cells[row, 1]))||(uni < 0))
                {
                    memo("================= " + util.getstring(volume.Cells[row, 1]) + " invalid uni");
                    continue; //skip this row
                }
                for (int col = 2; col <= 37; col++)
                {
                    OV_VRbibliometry ovr = new OV_VRbibliometry();
                    ovr.Id = ovrid;
                    ovrid++;
                    ovr.Year = year;
                    if (String.IsNullOrEmpty(util.getstring(volume.Cells[row0, col])))
                        continue;
                    else if ((util.getstring(volume.Cells[row0, col]) == "Totalt") || (util.getstring(volume.Cells[row0, col]) == "Lärosäte totalt"))
                        ovr.Subject = 0;
                    else
                    {
                        if (year > 2020) //Format change 2021
                            ovr.Subject = col - 1;
                        else
                            ovr.Subject = col;
                    }
                    ovr.Uni = uni;
                    ovr.Volume = util.getdouble(volume.Cells[row, col]);
                    ovr.Fieldnormcit = util.getdouble(fieldnorm.Cells[row, col]);
                    ovr.Bibindex = util.getdouble(bibindex.Cells[row, col]);
                    db.OV_VRbibliometry.InsertOnSubmit(ovr);
                    db.SubmitChanges();
                }
            }


            //Cleanup
            xlWorkBook.Close(false, null, null);
            xlApp.Quit();

            foreach (Excel.Worksheet xll in sheetlist)
                Marshal.ReleaseComObject(xll);
            //Marshal.ReleaseComObject(xlhtl);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            register_fileentry(fn);

        }

        private void VRbutton_Click(object sender, EventArgs e)
        {
            fill_OV_VRsubject();
            string folder = nvfolder + @"Bibliometri VR\";
            foreach (string fn in Directory.GetFiles(folder))
            {
                memo(fn);
                if (fn.Contains("ördelningsunderlag"))
                    read_VR_fordelningsunderlag(fn);
                else
                    memo("Skipping");
            }
            memo("\nDone");
        }

        private void Recruitkommun_button_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> landict = new Dictionary<string, int>();
            Dictionary<string, int> kommundict = new Dictionary<string, int>();
            Dictionary<string, int> genderdict = fill_genderdict();
            Dictionary<string, int> agedict = fill_agedict();

            //var qlan = from c in db.OV_Lan select c;
            //foreach (OV_Lan o in qlan)
            //    landict.Add(o.Name, o.Id);
            landict.Add("", 0);
            landict.Add("Stockholm",1);
            landict.Add("Uppsala",3);
            landict.Add("Södermanland",4);
            landict.Add("Östergötland",5);
            landict.Add("Jönköping",6);
            landict.Add("Kronoberg",7);
            landict.Add("Kalmar",8);
            landict.Add("Gotland",9);
            landict.Add("Blekinge",10);
            landict.Add("Skåne",12);
            landict.Add("Halland",13);
            landict.Add("Västra Götaland",14);
            landict.Add("Värmland",17);
            landict.Add("Örebro",18);
            landict.Add("Västmanland",19);
            landict.Add("Dalarna",20);
            landict.Add("Gävleborg",21);
            landict.Add("Västernorrland",22);
            landict.Add("Jämtland",23);
            landict.Add("Västerbotten",24);
            landict.Add("Norrbotten",25);
            landict.Add("Okänt", 99);

            var qkommun = from c in db.OV_Kommun select c;
            foreach (OV_Kommun o in qkommun)
                kommundict.Add(o.Name, o.Id);
            kommundict.Add("", 0);
            kommundict.Add("Upplands-Väsby", 114);

            int iou = 1;
            var qou = from c in db.OV_recruitkommun select c.Id;
            if (qou.Count() > 0)
                iou = qou.Max()+1;


            List<string> files = util.get_filelist(ukafolder);
            foreach (string fn in files)
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                if (!fn.Contains("recruitkommun"))
                    continue;
                string rex = @"_(\d{4})_";

                List<int> skipyear = (from c in db.OV_recruitkommun select c.Year).Distinct().ToList();

                int nline = 0;
                using (StreamReader sr = new StreamReader(fn))
                {
                    sr.ReadLine(); //skip header
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        nline++;
                        if (nline % 100 == 0)
                        {
                            memo(nline + " lines");
                            //break; //#########################################
                        }
                        string[] words = line.Split('\t');

                        int year = util.tryconvert(words[0].Substring(2, 4));
                        if (skipyear.Contains(year))
                            continue;
                        bool ht = words[0].StartsWith("HT");
                        int iuni = getuni(words[1],db);
                        if (iuni < 0)
                        {
                            memo("iuni = " + iuni+" "+words[1]);
                            continue;
                        }

                        int ilan = landict[words[2]];
                        int ikommun = kommundict[words[3]];
                        int igender = genderdict[words[4]];
                        int iage = agedict[words[5]];


                        int number = util.tryconvert(words[6]);
                        if (number == -1) //conversion failed
                            continue;

                        OV_recruitkommun ou = new OV_recruitkommun();
                        ou.Id = iou;
                        iou++;
                        ou.Uni = iuni;
                        ou.Lan = ilan;
                        ou.Kommun = ikommun;
                        ou.Gender = igender;
                        ou.Age = iage;
                        ou.HT = ht;
                        ou.Year = year;
                        ou.Number = number;
                        db.OV_recruitkommun.InsertOnSubmit(ou);
                        if (nline < 100 || nline%1000 == 0)
                            db.SubmitChanges();
                    }
                    db.SubmitChanges();
                    register_fileentry(fn);
                }
            }
        }

        private void kommundemografi_utbildning()
        {
            //Dictionary<string, int> landict = new Dictionary<string, int>();
            Dictionary<int, int> kommunlandict = new Dictionary<int, int>();
            Dictionary<string, int> genderdict = fill_genderdict(true);
            Dictionary<string, int> agedict = fill_agedict();

            var qkommun = from c in db.OV_Kommun select c;
            foreach (OV_Kommun o in qkommun)
                if (o.Lan != null)
                    kommunlandict.Add(o.Id, (int)o.Lan);

            int iou = 1;
            var qou = from c in db.OV_demographykommun select c.Id;
            if (qou.Count() > 0)
                iou = qou.Max() + 1;

            List<string> files = util.get_filelist(nvfolder);
            foreach (string fn in files)
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                if (!fn.Contains("Befolkning kommun efter utbildningsnivå och ålder"))
                    continue;

                List<int> skipyear = (from c in db.OV_demographykommun where c.Educated != null select c.Year).Distinct().ToList();

                Dictionary<int, int> yearcolumndict = new Dictionary<int, int>();

                int nline = 0;
                using (StreamReader sr = new StreamReader(fn))
                {
                    sr.ReadLine(); //skip header
                    sr.ReadLine(); //skip header
                    string yearline = sr.ReadLine(); //line with year column headers
                    string[] yearwords = yearline.Split('\t');
                    bool skipall = true;
                    for (int iy = 4; iy < yearwords.Length; iy++)
                    {
                        int yy = util.tryconvert(yearwords[iy]);
                        yearcolumndict.Add(iy, yy);
                        if (!skipyear.Contains(yy))
                            skipall = false;
                    }
                    if (skipall) //no year that needs doing in this file
                    {
                        memo("skipall");
                        continue;
                    }
                    int ikommun = -1;
                    int iage = -1;
                    bool educated = false;
                    int igender = -1;
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        if (line.StartsWith("\""))
                            break;
                        nline++;
                        if (nline % 100 == 0)
                        {
                            memo(nline + " lines");
                            //break; //#########################################
                        }
                        string[] words = line.Split('\t');
                        if (words.Length < yearwords.Length)
                            continue;

                        if (words[0].Length >= 4)
                            ikommun = util.tryconvert(words[0].Substring(0, 4));
                        if (!String.IsNullOrEmpty(words[1]))
                            iage = agedict[words[1]];
                        if (words[2].Contains("förgymnasial"))
                            educated = false;
                        else if (words[2].Contains("eftergymnasial"))
                            educated = true;
                        else if (words[2].Contains("saknas"))
                            educated = false;
                        if (String.IsNullOrEmpty(words[3]))
                            continue;
                        igender = genderdict[words[3]];

                        for (int iy = 4; iy < words.Length; iy++)
                        {
                            int year = yearcolumndict[iy];
                            if (skipyear.Contains(year))
                                continue;
                            int number = util.tryconvert(words[iy]);
                            if (number == -1) //conversion failed
                                continue;

                            OV_demographykommun ou = new OV_demographykommun();
                            ou.Id = iou;
                            iou++;
                            ou.Lan = kommunlandict[ikommun];
                            ou.Kommun = ikommun;
                            ou.Gender = igender;
                            ou.Age = iage;
                            ou.Educated = educated;
                            ou.Year = year;
                            ou.Number = number;
                            db.OV_demographykommun.InsertOnSubmit(ou);
                        }
                        if (nline < 100 || nline % 1000 == 0)
                            db.SubmitChanges();
                    }
                    db.SubmitChanges();
                    register_fileentry(fn);
                }
                memo("Done with " + fn);
            }
            memo("========== DONE =======");

        }

        private void kommundemografi_foreign()
        {
            //Dictionary<string, int> landict = new Dictionary<string, int>();
            Dictionary<int, int> kommunlandict = new Dictionary<int, int>();
            Dictionary<string, int> genderdict = fill_genderdict(true);
            Dictionary<string, int> agedict = fill_agedict();

            var qkommun = from c in db.OV_Kommun select c;
            foreach (OV_Kommun o in qkommun)
                if (o.Lan != null)
                    kommunlandict.Add(o.Id, (int)o.Lan);

            int iou = 1;
            var qou = from c in db.OV_demographykommun select c.Id;
            if (qou.Count() > 0)
                iou = qou.Max() + 1;

            List<string> files = util.get_filelist(nvfolder);
            foreach (string fn in files)
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                if (!fn.Contains("kommundemografi-ålder-kön-härkomst"))
                    continue;

                List<int> skipyear = (from c in db.OV_demographykommun where c.Foreignbackground != null select c.Year).Distinct().ToList();

                Dictionary<int, int> yearcolumndict = new Dictionary<int, int>();

                int nline = 0;
                using (StreamReader sr = new StreamReader(fn))
                {
                    sr.ReadLine(); //skip header
                    sr.ReadLine(); //skip header
                    string yearline = sr.ReadLine(); //line with year column headers
                    string[] yearwords = yearline.Split('\t');
                    bool skipall = true;
                    for (int iy = 4; iy < yearwords.Length; iy++)
                    {
                        int yy = util.tryconvert(yearwords[iy]);
                        yearcolumndict.Add(iy, yy);
                        if (!skipyear.Contains(yy))
                            skipall = false;
                    }
                    if (skipall) //no year that needs doing in this file
                    {
                        memo("skipall");
                        continue;
                    }
                    int ikommun = -1;
                    int iage = -1;
                    int iforeign = -1;
                    int igender = -1;
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        if (line.StartsWith("\""))
                            break;
                        nline++;
                        if (nline % 100 == 0)
                        {
                            memo(nline + " lines");
                            //break; //#########################################
                        }
                        string[] words = line.Split('\t');
                        if (words.Length < yearwords.Length)
                            continue;
                        if (words[0].Contains("definieras"))
                            break;

                        if (words[0].Length >= 4)
                            ikommun = util.tryconvert(words[0].Substring(0, 4));
                        if (!String.IsNullOrEmpty(words[2]))
                            iage = agedict[words[2]];
                        if (words[1].Contains("utländsk"))
                            iforeign = 6;
                        else if (words[1].Contains("svensk"))
                            iforeign = 7;
                        if (String.IsNullOrEmpty(words[3]))
                            continue;
                        igender = genderdict[words[3]];

                        for (int iy = 4; iy < words.Length; iy++)
                        {
                            int year = yearcolumndict[iy];
                            if (skipyear.Contains(year))
                                continue;
                            int number = util.tryconvert(words[iy]);
                            if (number == -1) //conversion failed
                                continue;

                            OV_demographykommun ou = new OV_demographykommun();
                            ou.Id = iou;
                            iou++;
                            ou.Lan = kommunlandict[ikommun];
                            ou.Kommun = ikommun;
                            ou.Gender = igender;
                            ou.Age = iage;
                            ou.Foreignbackground = iforeign;
                            ou.Year = year;
                            ou.Number = number;
                            db.OV_demographykommun.InsertOnSubmit(ou);
                        }
                        if (nline < 100 || nline % 1000 == 0)
                            db.SubmitChanges();
                    }
                    db.SubmitChanges();
                    register_fileentry(fn);
                }
                memo("Done with " + fn);
            }
            memo("========== DONE =======");

        }

        private void kommundemografi_button_Click(object sender, EventArgs e)
        {
            kommundemografi_foreign();
            //kommundemografi_utbildning();
        }

        private void Transitionbutton_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> landict = new Dictionary<string, int>();
            Dictionary<string, int> kommundict = new Dictionary<string, int>();
            Dictionary<string, int> genderdict = fill_genderdict(true);
            Dictionary<int, int> kommunlandict = new Dictionary<int, int>();

            var qkommun = from c in db.OV_Kommun select c;
            foreach (OV_Kommun o in qkommun)
            {
                kommundict.Add(o.Name, o.Id);
                if (o.Lan != null)
                    kommunlandict.Add(o.Id, (int)o.Lan);
            }
            kommundict.Add("", 0);
            kommundict.Add("Upplands-Väsby", 114);

            //var qlan = from c in db.OV_Lan select c;
            //foreach (OV_Lan o in qlan)
            //    landict.Add(o.Name, o.Id);
            landict.Add("", 0);
            landict.Add("Stockholm", 1);
            landict.Add("Uppsala", 3);
            landict.Add("Södermanland", 4);
            landict.Add("Östergötland", 5);
            landict.Add("Jönköping", 6);
            landict.Add("Kronoberg", 7);
            landict.Add("Kalmar", 8);
            landict.Add("Gotland", 9);
            landict.Add("Blekinge", 10);
            landict.Add("Skåne", 12);
            landict.Add("Halland", 13);
            landict.Add("Västra Götaland", 14);
            landict.Add("Värmland", 17);
            landict.Add("Örebro", 18);
            landict.Add("Västmanland", 19);
            landict.Add("Dalarna", 20);
            landict.Add("Gävleborg", 21);
            landict.Add("Västernorrland", 22);
            landict.Add("Jämtland", 23);
            landict.Add("Västerbotten", 24);
            landict.Add("Norrbotten", 25);
            landict.Add("Okänt", 99);


            int iou = 1;
            var qou = from c in db.OV_transition select c.Id;
            if (qou.Count() > 0)
                iou = qou.Max() + 1;


            List<string> files = util.get_filelist(nvfolder);
            foreach (string fn in files)
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                if (!fn.Contains("övergångstal"))
                    continue;
                string rex = @"\/(\d{4})";

                List<int> skipyear = (from c in db.OV_transition select c.Year).Distinct().ToList();

                int nline = 0;
                using (StreamReader sr = new StreamReader(fn))
                {
                    sr.ReadLine(); //skip header
                    sr.ReadLine(); //skip header
                    string yearline = sr.ReadLine();
                    string[] yearwords = yearline.Split();
                    Dictionary<int, int> yeardict = new Dictionary<int, int>();
                    for (int i = 0; i < yearwords.Length;i++ )
                        foreach (Match m in Regex.Matches(yearwords[i], rex))
                        {
                            int year = util.tryconvert(m.Groups[1].Value);
                            yeardict.Add(i, year);
                        }

                    int ikommun = -1;
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        nline++;
                        if (nline % 100 == 0)
                        {
                            memo(nline + " lines");
                            //break; //#########################################
                        }
                        string[] words = line.Split('\t');

                        if (!String.IsNullOrEmpty(words[0]))
                        {
                            if (words[0] == "Uppgift saknas")
                                break;
                            ikommun = kommundict[words[0].Trim()];
                        }
                        int ilan = kommunlandict[ikommun];
                        int igender = genderdict[words[2]];

                        for (int k = 3; k < words.Length; k++)
                        {
                            float number = (float)util.tryconvertdouble(words[k]);
                            if (number == -1) //conversion failed
                                continue;

                            OV_transition ou = new OV_transition();
                            ou.Id = iou;
                            iou++;
                            ou.Lan = ilan;
                            ou.Kommun = ikommun;
                            ou.Gender = igender;
                            ou.Year = yeardict[k];
                            ou.Fraction = number;
                            db.OV_transition.InsertOnSubmit(ou);
                        }
                        if (nline < 20 || nline % 1000 == 0)
                            db.SubmitChanges();
                    }
                    db.SubmitChanges();
                    register_fileentry(fn);
                }
            }

        }

        private void Registeredbutton_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> genderdict = fill_genderdict();
            Dictionary<string, int> agedict = fill_agedict();

            int iou = 1;
            var qou = from c in db.OV_registered select c.Id;
            if (qou.Count() > 0)
                iou = qou.Max() + 1;


            List<string> files = util.get_filelist(ukafolder);
            foreach (string fn in files)
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                if (!fn.Contains("registeredstudents"))
                    continue;

                List<int> skipyear = (from c in db.OV_registered select c.Year).Distinct().ToList();

                int nline = 0;
                using (StreamReader sr = new StreamReader(fn))
                {
                    sr.ReadLine(); //skip header
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        nline++;
                        if (nline % 100 == 0)
                        {
                            memo(nline + " lines");
                            //break; //#########################################
                        }
                        string[] words = line.Split('\t');

                        int year = util.tryconvert(words[0].Substring(2, 4));
                        if (skipyear.Contains(year))
                            continue;
                        bool ht = words[0].StartsWith("HT");
                        int iuni = getuni(words[1], db);
                        if (iuni < 0)
                        {
                            memo("iuni = " + iuni + " " + words[1]);
                            continue;
                        }

                        int igender = genderdict[words[5]];
                        int iage = agedict[words[6]];


                        int number = util.tryconvert(words[7]);
                        if (number == -1) //conversion failed
                            continue;

                        OV_registered ou = new OV_registered();
                        ou.Id = iou;
                        iou++;
                        ou.Uni = iuni;
                        ou.Gender = igender;
                        ou.Age = iage;
                        ou.HT = ht;
                        ou.Year = year;
                        ou.Number = number;
                        db.OV_registered.InsertOnSubmit(ou);
                        if (nline < 100 || nline % 1000 == 0)
                            db.SubmitChanges();
                    }
                    db.SubmitChanges();
                    register_fileentry(fn);
                }
            }

        }

        private void Financebutton_Click(object sender, EventArgs e)
        {
            foreach (string fn in Directory.GetFiles(ukafolder))
            {
                memo(fn);
                if (!fn.Contains(".txt"))
                    continue;
                
                if (Path.GetFileName(fn).StartsWith("takbeloppsupp"))
                {
                    read_takbeloppsuppfoljning(fn);
                }
                else if (Path.GetFileName(fn).StartsWith("balansrakn"))
                {
                    read_balansrakning(fn);
                }
                else if (Path.GetFileName(fn).StartsWith("resultatrakn"))
                {
                    read_resultatrakning(fn);
                }
            }
            memo("===== DONE =====");
        }

        private Dictionary<string,int> get_fpostdict()
        {
            Dictionary<string, int> fpostdict = new Dictionary<string, int>();
            foreach (OV_financepost fp in db.OV_financepost)
            {
                fpostdict.Add(fp.Name, fp.Id);
            }
            return fpostdict;
        }

        private Dictionary<string, int> get_fverkdict()
        {
            Dictionary<string, int> fverkdict = new Dictionary<string, int>();
            foreach (OV_financeverksamhet fp in db.OV_financeverksamhet)
            {
                fverkdict.Add(fp.Name, fp.Id);
            }
            return fverkdict;
        }

        private void read_resultatrakning(string fn)
        {
            memo("Resultaträkning " + fn);
            Dictionary<string, int> fpostdict = get_fpostdict();
            Dictionary<string, int> fverkdict = get_fverkdict();

            int fid = 1;
            if (db.OV_finance.Count() > 0)
                fid = (from c in db.OV_finance select c.Id).Max() + 1;

            var q = from c in db.OV_finance
                    where c.Post == fpostdict["Verksamhetens intäkter"]
                    select c.Year;
            int lastyear = -9999;
            if (q.Count() > 0)
                lastyear = q.Max();

            using (StreamReader sr = new StreamReader(fn))
            {
                string hline = sr.ReadLine();
                string[] hwords = hline.Split('\t');

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] words = line.Split('\t');
                    //if (String.IsNullOrEmpty(words[5]))
                    //    continue;
                    //float amount = util.tryconvert(words[5]);
                    //if (amount <= 0)
                    //    continue;
                    int year = util.tryconvert(words[1]);
                    if (year <= lastyear)
                        continue;
                    int iuni = getuni(words[2], db);
                    if (iuni < 0)
                        continue;
                    int post = fpostdict[words[3]];
                    for (int i = 5; i < 11; i++)
                    {
                        int verk = fverkdict[hwords[i]];
                        int? amount = util.tryconvertnull(words[i]);
                        if (amount == null)
                            continue;

                        OV_finance fp = new OV_finance();
                        fp.Id = fid;
                        fid++;
                        fp.Uni = iuni;
                        fp.Post = post;
                        fp.Verksamhet = verk;
                        fp.Year = year;
                        fp.Amount = (float)amount;
                        db.OV_finance.InsertOnSubmit(fp);
                        if (fid % 100 == 0)
                        {
                            memo("fid = " + fid);
                            db.SubmitChanges();
                        }
                    }
                }
                db.SubmitChanges();
                register_fileentry(fn);

            }

        }

        private void read_balansrakning(string fn)
        {
            memo("Balansräkning " + fn);
            Dictionary<string, int> fpostdict = get_fpostdict();
            Dictionary<string, int> fverkdict = get_fverkdict();

            int fid = 1;
            if (db.OV_finance.Count() > 0)
                fid = (from c in db.OV_finance select c.Id).Max() + 1;

            var q = from c in db.OV_finance
                    where c.Post == fpostdict["Balanserad kapitalförändring"]
                    select c.Year;
            int lastyear = -9999;
            if (q.Count() > 0)
                lastyear = q.Max();

            using (StreamReader sr = new StreamReader(fn))
            {
                string hline = sr.ReadLine();
                string[] hwords = hline.Split('\t');

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] words = line.Split('\t');
                    //if (String.IsNullOrEmpty(words[5]))
                    //    continue;
                    //float amount = util.tryconvert(words[5]);
                    //if (amount <= 0)
                    //    continue;
                    int year = util.tryconvert(words[1]);
                    if (year <= lastyear)
                        continue;
                    int iuni = getuni(words[2], db);
                    if (iuni < 0)
                        continue;
                    int post = fpostdict[words[3]];
                    for (int i = 5; i < 9; i++)
                    {
                        int verk = fverkdict[hwords[i]];
                        int? amount = util.tryconvertnull(words[i]);
                        if (amount == null)
                            continue;

                        OV_finance fp = new OV_finance();
                        fp.Id = fid;
                        fid++;
                        fp.Uni = iuni;
                        fp.Post = post;
                        fp.Verksamhet = verk;
                        fp.Year = year;
                        fp.Amount = (float)amount;
                        db.OV_finance.InsertOnSubmit(fp);
                        if (fid % 100 == 0)
                        {
                            memo("fid = " + fid);
                            db.SubmitChanges();
                        }
                    }
                }
                db.SubmitChanges();
                register_fileentry(fn);

            }
        }

        private void read_takbeloppsuppfoljning(string fn)
        {
            memo("Takbelopp " + fn);
            Dictionary<string, int> fpostdict = get_fpostdict();
            Dictionary<string, int> fverkdict = get_fverkdict();

            int fid = 1;
            if (db.OV_finance.Count() > 0)
                fid = (from c in db.OV_finance select c.Id).Max()+1;

            var q = from c in db.OV_finance
                    where c.Post == fpostdict["Takbelopp"]
                    select c.Year;
            int lastyear = -9999;
            if (q.Count() > 0)
                lastyear = q.Max();

            using (StreamReader sr = new StreamReader(fn))
            {
                string hline = sr.ReadLine(); //skip header
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] words = line.Split('\t');
                    if (String.IsNullOrEmpty(words[5]))
                        continue;
                    int? amount = util.tryconvertnull(words[5]);
                    if (amount == null)
                        continue;
                    
                    int year = util.tryconvert(words[1]);
                    if (year <= lastyear)
                        continue;
                    int iuni = getuni(words[2], db);
                    int post = fpostdict[words[3]];
                    int verk = 1; //vanlig grundutbildning

                    OV_finance fp = new OV_finance();
                    fp.Id = fid;
                    fid++;
                    fp.Uni = iuni;
                    fp.Post = post;
                    fp.Verksamhet = verk;
                    fp.Year = year;
                    fp.Amount = (float)amount;
                    db.OV_finance.InsertOnSubmit(fp);
                    if (fid % 100 == 0)
                    {
                        memo("fid = " + fid);
                        db.SubmitChanges();
                    }
                }
                db.SubmitChanges();
                register_fileentry(fn);

            }

        }
    }
}
