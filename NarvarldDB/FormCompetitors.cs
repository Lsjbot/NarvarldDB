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

namespace NarvarldDB
{
    public partial class FormCompetitors : Form
    {
        private DbTGSAnalysTest db = null;
        private FormDisplay parent = null;
        private FormSelectData select = null;
        int focusuni = 52;//"Högskolan Dalarna";
        int year = 2021;
        Dictionary<string, List<int>> instsubjects = new Dictionary<string, List<int>>();
        Dictionary<string, Dictionary<string, List<int>>> instsubjectgroups = new Dictionary<string, Dictionary<string,List<int>>>();
        Dictionary<string, List<int>> instsector = new Dictionary<string, List<int>>();
        Dictionary<int, string> subjname = new Dictionary<int, string>();
        Dictionary<int, string> sectorname = new Dictionary<int, string>();
        List<int> amneslarare = new List<int>();

        public FormCompetitors(DbTGSAnalysTest dbpar, FormDisplay parentpar, FormSelectData selectpar, int focusunipar, int yearpar)
        {
            InitializeComponent();
            db = dbpar;
            parent = parentpar;
            select = selectpar;
            focusuni = focusunipar;
            year = yearpar;

            var q = from c in db.OV_course
                    where c.Year == year
                    where c.Uni == focusuni
                    where c.Program
                    select c;

            foreach (OV_course oc in q)
            {
                if (islater(oc.Name))
                    continue;
                LBprog.Items.Add(oc.Name + " |" + oc.Subject);
            }

            var qs = from c in db.OV_mysubject
                     select c;

            foreach (OV_mysubject os in qs)
            {
                LBsubj.Items.Add(os.Name + " | " + os.Id);
                subjname.Add(os.Id, os.Name);
            }

            foreach (OV_mysector ms in (from c in db.OV_mysector select c))
            {
                sectorname.Add(ms.Id, ms.Name);
            }

            amneslarare = (from c in db.OV_mysubject where c.Name.StartsWith("ä") select c.Id).ToList();

            read_inst();
        }

        private void read_inst()
        {
            string fn = FormDisplay.nvfolder + "mysubject per institution.txt";
            using (StreamReader sr = new StreamReader(fn))
            {
                string header = sr.ReadLine();
                string[] hwords = header.Split('\t');
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] words = line.Split('\t');
                    int subj = util.tryconvert(words[0]);
                    int sector = util.tryconvert(words[3]);
                    if (words.Length > 4)
                    {
                        for (int i=4;i<words.Length;i++)
                        {
                            if (!string.IsNullOrEmpty(words[i]))
                            {
                                if (!instsubjects.ContainsKey(hwords[i]))
                                    instsubjects.Add(hwords[i], new List<int>() { subj });
                                else
                                    instsubjects[hwords[i]].Add(subj);
                                if (!instsector.ContainsKey(hwords[i]))
                                    instsector.Add(hwords[i], new List<int>() { sector });
                                else if (!instsector[hwords[i]].Contains(sector))
                                    instsector[hwords[i]].Add(sector);
                            }
                        }
                    }
                }
            }
            string fn2 = FormDisplay.nvfolder + "mysubjectlist per institution.txt";
            using (StreamReader sr = new StreamReader(fn2))
            {
                string header = sr.ReadLine();
                string[] hwords = header.Split('\t');
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] words = line.Split('\t');
                    if (words.Length < 3)
                        continue;
                    string inst = words[0];
                    if (!instsubjectgroups.ContainsKey(inst))
                        instsubjectgroups.Add(inst, new Dictionary<string, List<int>>());
                    string groupname = words[1];
                    instsubjectgroups[inst].Add(groupname, new List<int>());
                    string[] subs = words[2].Split(',');
                    foreach (string sub in subs)
                    {
                        int? isub = (from c in db.OV_mysubject
                                    where c.Code == sub
                                     select c.Id).FirstOrDefault();
                        if (isub > 0)
                            instsubjectgroups[inst][groupname].Add((int)isub);
                    }
                }
            }
        }


        private void Closebutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ismagister(string prog)
        {
            return prog.ToLower().Contains("magister");
        }

        private bool ismaster(string prog)
        {
            return prog.ToLower().Contains("master");
        }

        private bool islater(string prog)
        {
            if (prog.ToLower().Contains("senare del"))
                return true;
            if (prog.ToLower().Contains("termin "))
                return true;
            return false;
        }

        private bool isregular(string prog)
        {
            if (ismaster(prog))
                return false;
            if (ismagister(prog))
                return false;
            if (islater(prog))
                return false;
            return true;
        }

        private void LBprog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LBprog.SelectedItem == null)
                return;

            string prog = LBprog.SelectedItem.ToString().Split('|')[0].Trim();
            int subj = util.tryconvert(LBprog.SelectedItem.ToString().Split('|')[1].Trim());
            bool magisterprog = ismagister(prog);
            bool masterprog = ismaster(prog);

            var q = from c in db.OV_course
                    where c.Year == year
                    where c.Subject == subj
                    where c.Program
                    select c;

            parent.memo("====== " + prog + " ======");
            parent.memo("Namn\tUni\t1:handssök\tTotalsök\tAntagna\tReserver");
            foreach (OV_course oc in q)
            {
                if (islater(oc.Name))
                    continue;
                if (ismagister(oc.Name) != magisterprog)
                    continue;
                if (ismaster(oc.Name) != masterprog)
                    continue;
                parent.memo(oc.Name + "\t" + oc.Code.Split('-')[1] + "\t" + oc.Appl1h + "\t" + oc.Appltotal + "\t" + oc.Accepted + "\t" + oc.Reserves);
            }
        }

        private void LBsubj_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LBsubj.SelectedItem == null)
                return;

            string prog = LBsubj.SelectedItem.ToString().Split('|')[0].Trim();
            int subj = util.tryconvert(LBsubj.SelectedItem.ToString().Split('|')[1].Trim());

            var q = from c in db.OV_course
                    where c.Year == year
                    where c.Subject == subj
                    where c.Program
                    select c;

            parent.memo("====== " + prog + " ======");
            parent.memo("Namn\tUni\t1:handssök\tTotalsök\tAntagna\tReserver");
            foreach (OV_course oc in q)
            {
                if (islater(oc.Name))
                    continue;
                bool isadvanced = ismaster(oc.Name) || ismagister(oc.Name);
                if (isadvanced != CBadvanced.Checked)
                    continue;
                parent.memo(oc.Name + "\t" + oc.Code.Split('-')[1] + "\t" + oc.Appl1h + "\t" + oc.Appltotal+"\t" + oc.Accepted+"\t" + oc.Reserves);
            }

        }

        private string subjectline(IEnumerable<OV_course> q, string sname)
        {
            var qfocus = from c in q where c.Uni == focusuni select c;
            int nuni = (from c in q select c.Uni).Distinct().Count();
            int ntot = -1;
            int nfocus = 0;
            if (select.RB_appl1h.Checked)
            {
                ntot = (from c in q select c.Appl1h).Sum();
                if (qfocus.Count() > 0)
                    nfocus = (from c in qfocus select c.Appl1h).Sum();
            }
            else if (select.RB_appltotal.Checked)
            {
                ntot = (from c in q select c.Appltotal).Sum();
                if (qfocus.Count() > 0)
                    nfocus = (from c in qfocus select c.Appltotal).Sum();
            }
            double nacc = (from c in q select c.Accepted).Sum();
            double naccfocus = (from c in qfocus select c.Accepted).Sum();
            int nres = (from c in q select c.Reserves).Sum();
            int nresfocus = (from c in qfocus select c.Reserves).Sum();

            if (ntot > 0)
            {
                string ss = sname + "\t" + nuni + "\t" + ntot + "\t" + nfocus + "\t" + (((float)100 * nfocus) / ntot).ToString("N2") + "%\t" + (ntot / q.Count()).ToString("N2");
                if (nacc > 0 )
                {
                    ss += "\t" + (nres / nacc).ToString("N1");
                    if (naccfocus > 0)
                        ss += "\t" + (nresfocus / naccfocus).ToString("N1");
                }
                return ss;
            }
            else
                return "";

        }

        private void Allbutton_Click(object sender, EventArgs e)
        {
            int year = select.getendyear();
            if (year < 0)
                year = 2021;
            parent.memo("########### " + year + " ##################");
            if (select.RB_appl1h.Checked)
            {
                parent.memo("-- Förstahandssökande --");
            }
            else if (select.RB_appltotal.Checked)
            {
                parent.memo("-- Sökande totalt --");
            }

            foreach (string inst in instsubjects.Keys)
            {
                parent.memo("====== " + inst + " ======");
                parent.memo("Område\tLärosäten med området\tSökande riket\tSökande "+select.getunishort(focusuni)+"\tAndel "+select.getunishort(focusuni)+"\tSökande per utbildning\tReserver per antagen riket\treserver per antagen "+ select.getunishort(focusuni));

                for (int iprog= 0; iprog <= 1;iprog++)
                {
                    string progstring = " FK";
                    if (iprog == 1)
                        progstring = " program";
                    foreach (int subj in instsubjects[inst])
                    {
                        var q = from c in db.OV_course
                                where c.Year == year
                                where c.Subject == subj select c;
                        if (iprog==0)
                        {
                            q = from c in q where !c.Program select c;
                        }
                        else if (iprog==1)
                        {
                            q = from c in q where c.Program select c;
                        }
                        if (q.Count() == 0)
                            continue;
                        parent.memo(subjectline(q, subjname[subj]+progstring));
                    }

                    //if (inst == "ILU")
                    //{
                    //    var q = from c in db.OV_course
                    //            where c.Year == year
                    //            where amneslarare.Contains((int)c.Subject)
                    //            select c;
                    //    if (select.RB_fk.Checked)
                    //    {
                    //        q = from c in q where !c.Program select c;
                    //    }
                    //    else if (select.RB_prog.Checked)
                    //    {
                    //        q = from c in q where c.Program select c;
                    //    }
                    //    if (q.Count() == 0)
                    //        continue;
                    //    parent.memo(subjectline(q, "Ämneslärare totalt"));

                    //}

                    foreach (string groupname in instsubjectgroups[inst].Keys)
                    {
                        IEnumerable<OV_course> qq = null;
                        foreach (int isub in instsubjectgroups[inst][groupname])
                        {
                            var q = from c in db.OV_course
                                    where c.Year == year
                                    where c.Subject == isub
                                    select c;
                            if (iprog == 0)
                            {
                                q = from c in q where !c.Program select c;
                            }
                            else if (iprog == 1)
                            {
                                q = from c in q where c.Program select c;
                            }
                            if (q.Count() == 0)
                                continue;
                            if (qq == null)
                                qq = q;
                            else
                                qq = qq.Concat(q);
                        }
                        parent.memo(subjectline(qq, groupname+progstring));
                    }
                }
                //Sector:
                foreach (int sector in instsector[inst])
                {
                    var q = from c in db.OV_course
                            where c.Year == year
                            where c.Sector == sector
                            select c;
                    if (select.RB_fk.Checked)
                    {
                        q = from c in q where !c.Program select c;
                    }
                    else if (select.RB_prog.Checked)
                    {
                        q = from c in q where c.Program select c;
                    }
                    if (q.Count() == 0)
                        continue;

                    parent.memo(subjectline(q, "Sektor " + sectorname[sector]));

                    //var qfocus = from c in q where c.Uni == focusuni select c;
                    //int nuni = (from c in q select c.Uni).Distinct().Count();
                    //int ntot = -1;
                    //int nfocus = 0;
                    //if (select.RB_appl1h.Checked)
                    //{
                    //    ntot = (from c in q select c.Appl1h).Sum();
                    //    if (qfocus.Count() > 0)
                    //        nfocus = (from c in qfocus select c.Appl1h).Sum();
                    //}
                    //else if (select.RB_appltotal.Checked)
                    //{
                    //    ntot = (from c in q select c.Appltotal).Sum();
                    //    if (qfocus.Count() > 0)
                    //        nfocus = (from c in qfocus select c.Appltotal).Sum();
                    //}
                    //if (ntot > 0)
                    //{
                    //    parent.memo("Sektor " + sectorname[sector] + "\t" + nuni + "\t" + ntot + "\t" + nfocus + "\t" + (((float)100 * nfocus) / ntot).ToString("N2") + "%\t" + (ntot / q.Count()).ToString("N2"));
                    //}
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int startyear = (from c in db.OV_course select c.Year).Max() - 2;
            parent.memo("==== Medelvärde " + startyear + "-" + (startyear + 2));
            parent.memo("Område\tUtbildningar\tLärosäten\tSökande/utb totalt\tSökande/utb 1hand");
            foreach (OV_mysubject oms in db.OV_mysubject)
            {
                if (oms.Sector == 0)
                    continue;
                var q = from c in db.OV_course
                        where c.Subject == oms.Id
                        where c.Year >= startyear
                        select c;
                List<OV_course> cl;
                if (oms.Sector != 4) //include FK for language sector only
                {
                    q = from c in q
                        where c.Program
                        select c;
                    cl = new List<OV_course>();
                    foreach (OV_course c in q)
                        if (isregular(c.Name))
                            cl.Add(c);
                }
                else
                    cl = q.ToList();
                if (cl.Count() == 0)
                    continue;
                var cluni = (from c in cl select c.Uni).Distinct();
                double apptot = (from c in cl select c.Appltotal).Sum();
                double app1h = (from c in cl select c.Appl1h).Sum();
                string line = oms.Name + "\t" + 
                    (int)(cl.Count() / 3) + "\t" + 
                    cluni.Count() + "\t" + 
                    (apptot / cl.Count()).ToString("N1").Replace(" ", "") + "\t" + 
                    (app1h / cl.Count()).ToString("N1").Replace(" ", "");
                if (cluni.Count() < 6)
                {
                    line += "\t";
                    foreach (int iuni in cluni)
                    {
                        line += select.getunishort(iuni)+" ";
                    }
                }
                var clprog = (from c in cl select c.Name).Distinct();
                if (clprog.Count()<4)
                {
                    line += "\t";
                    foreach (string pr in clprog)
                    {
                        line += pr + "; ";
                    }
                }
                parent.memo(line);
            }
        }
    }
}
