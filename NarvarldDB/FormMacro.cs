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
    public partial class FormMacro : Form
    {
        private DbTGSAnalysTest db = null;
        private FormDisplay parent = null;
        private FormSelectData selpar = null;
        private Chart chart1 = null;
        static string nvfolder = @"\\dustaff\home\sja\Documents\Närvärld\";


        public FormMacro(DbTGSAnalysTest dbpar, FormDisplay parentpar, FormSelectData selectpar, Chart chartpar)
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

        }

        public void memo(string s)
        {
            richTextBox1.AppendText(s + "\n");
            richTextBox1.ScrollToCaret();
        }

        private void Quitbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void selectuni(string uni)
        {
            if (selpar.setfocusuni(uni))
            {
                memo("Setting uni to " + uni);
                selpar.LB_uni.ClearSelected();
                foreach (int i in selpar.LB_uni.CheckedIndices)
                    selpar.LB_uni.SetItemCheckState(i, CheckState.Unchecked);
            }
            else
                memo("Invalid uni " + uni);
        }

        private void dochart(string charttype, bool loopuni)
        {
            if ( loopuni)
            {
                foreach (string uniname in selpar.unidict.Keys)
                {
                    memo(uniname);
                    selectuni(uniname);
                    dochart(charttype, false);
                }
            }
            else //this is where we really make the chart
            {
                switch (charttype)
                {
                    case "income":
                        selpar.RB_IncomeType.Checked = true;
                        selpar.RB_salaryindex.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "incomesource":
                        selpar.RB_IncomeSource.Checked = true;
                        selpar.RB_salaryindex.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "externalincome":
                        selpar.RB_externalincome.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "hst":
                        selpar.RB_hsttotal.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "hstämne":
                        selpar.RB_subjectarea.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "prestation":
                        selpar.RB_prestationsgrad.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "examfreq":
                        selpar.RB_allexamfreq.Checked = true;
                        selpar.RB_examfrequency.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "examfreqgender":
                        selpar.RB_examgender.Checked = true;
                        selpar.RB_examfrequency.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "examfreqforeign":
                        selpar.RB_examforeign.Checked = true;
                        selpar.RB_examfrequency.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "examfreqparentedu":
                        selpar.RB_exameduparent.Checked = true;
                        selpar.RB_examfrequency.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "examtype":
                        int oldyear = selpar.getstartyear();
                        if ( oldyear < 1999)
                            selpar.setstartyear(1999);
                        //selpar.RB_stackedexams.Checked = true;
                        //selpar.updatechart(null, null);
                        selpar.examgroup_stackedarea("--Alla examina efter nivå");
                        selpar.setstartyear(oldyear);
                        break;
                    case "examtypevård":
                        oldyear = selpar.getstartyear();
                        if (oldyear < 1999)
                            selpar.setstartyear(1999);
                        selpar.examgroup_stackedarea("--Alla vårdutbildningar");
                        selpar.setstartyear(oldyear);
                        break;
                    case "examtypelärare":
                        oldyear = selpar.getstartyear();
                        if (oldyear < 1999)
                            selpar.setstartyear(1999);
                        selpar.examgroup_stackedarea("--Alla lärarutbildningar");
                        selpar.setstartyear(oldyear);
                        break;
                    case "examtypeteknik":
                        oldyear = selpar.getstartyear();
                        if (oldyear < 1999)
                            selpar.setstartyear(1999);
                        selpar.examgroup_stackedarea("--Alla teknikutbildningar");
                        selpar.setstartyear(oldyear);
                        break;
                    case "examtypephd":
                        oldyear = selpar.getstartyear();
                        if (oldyear < 1999)
                            selpar.setstartyear(1999);
                        selpar.examgroup_stackedarea("--Alla forskarutbildningar");
                        selpar.setstartyear(oldyear);
                        break;
                    case "publications":
                        oldyear = selpar.getendyear();
                        selpar.setendyear(2019);
                        selpar.RB_pubtype.Checked = true;
                        selpar.updatechart(null, null);
                        selpar.setendyear(oldyear);
                        break;
                    case "staff":
                        selpar.LB_staff.Text = "Total";
                        selpar.RB_stafftype.Checked = true;
                        selpar.CB_staffabsolute.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "supportstaff":
                        selpar.LB_staff.Text = "Andel stödpersonal";
                        selpar.RB_stafftype.Checked = true;
                        selpar.CB_staffabsolute.Checked = false;
                        selpar.updatechart(null, null);
                        break;
                    case "examprice":
                        selpar.RB_permoney.Checked = true;
                        selpar.CB_reverse.Checked = true;
                        selpar.RB_salaryindex.Checked = true;
                        selpar.RB_totalexam.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "phdprice":
                        selpar.RB_permoney.Checked = true;
                        selpar.CB_reverse.Checked = true;
                        selpar.RB_salaryindex.Checked = true;
                        selpar.RB_totalexam.Checked = true;
                        selpar.totalexam(new int[1]{163},0,0);
                        break;
                    case "pubprice":
                        selpar.RB_permoney.Checked = true;
                        selpar.CB_reverse.Checked = true;
                        selpar.RB_salaryindex.Checked = true;
                        selpar.setendyear(2019);
                        selpar.RB_publication.Checked = true;
                        selpar.updatechart(null,null);
                        selpar.setendyear(-1);
                        break;
                    case "examprod":
                        selpar.RB_permoney.Checked = true;
                        selpar.CB_reverse.Checked = false;
                        selpar.RB_salaryindex.Checked = true;
                        selpar.RB_totalexam.Checked = true;
                        selpar.updatechart(null, null);
                        break;
                    case "phdprod":
                        selpar.RB_permoney.Checked = true;
                        selpar.CB_reverse.Checked = false;
                        selpar.RB_salaryindex.Checked = true;
                        selpar.RB_totalexam.Checked = true;
                        selpar.totalexam(new int[1] { 163 }, 0, 0);
                        break;
                    case "pubprod":
                        selpar.RB_permoney.Checked = true;
                        selpar.CB_reverse.Checked = false;
                        selpar.RB_salaryindex.Checked = true;
                        oldyear = selpar.getendyear();
                        selpar.setendyear(2019);
                        selpar.RB_publication.Checked = true;
                        selpar.updatechart(null, null);
                        selpar.setendyear(oldyear);
                        break;
                    default:
                        memo("Invalid chart type " + charttype);
                        break;
                }
                parent.Savebutton_Click(null, null);
            }
        }

        private void Runbutton_Click(object sender, EventArgs e)
        {
            //Run macro from textfile
            //
            // By default, each generated chart is saved to a file in nvfolder, with filename == chart title
            // Whatever options are set in FormSelectData will be used
            // Lines starting with / are comments
            //
            // Valid commands in macro:
            //
            // uni XXX -- set XXX as focus uni. XXX must be name in the form it has in unilist
            // uni -XXX -- remove XXX from uni list.
            // loop alluni -- loop over all universities for each following command, until "end alluni" or end of file. Overrides previous uni settings.
            // end alluni -- terminate university loop
            // trendline on
            // trendline off
            //
            // chart XXX -- make chart, with XXX any of the following:
            // income -- total income
            // incomesource -- income per source, stacked area
            // hst -- total HST
            // hstämne -- HST per ämne, stacked area
            // examfreq -- exam frequency
            // examfreqgender
            // examfreqforeign
            // examfreqparentedu
            // examtype -- exam types, stacked area
            // examtypevård -- exam types, stacked area
            // examtypelärare -- exam types, stacked area
            // examtypeteknik -- exam types, stacked area
            // examtypephd -- exam types, stacked area
            // publications -- publication type, stacked area
            // staff -- total staff
            // examprice -- cost per exam
            // phdprice -- cost per PhD
            // pubprice -- cost per publication

            selpar.CB_updatechart.Checked = false; //disable automatic chart drawing

            openFileDialog1.InitialDirectory = nvfolder;
            bool loopuni = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fn = openFileDialog1.FileName;
                using (StreamReader sr = new StreamReader(fn))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (String.IsNullOrEmpty(line))
                            continue;
                        if (line[0] == '/')
                            continue;
                        memo(line);
                        string[] words = line.Split();
                        switch (words[0])
                        {
                            case "chart": 
                                dochart(words[1],loopuni);
                                break;
                            case "uni":
                                if (!loopuni)
                                    selectuni(line.Replace(words[0],"").Trim());
                                else
                                    memo("invalid command inside loop");
                                break;
                            case "comparison":
                                if (!loopuni)
                                    selectcompetitors(words[1],line.Replace(words[0], "").Replace(words[1],"").Trim());
                                else
                                    memo("invalid command inside loop");
                                break;
                            case "trendline":
                                if (words[1] == "on")
                                    selpar.CB_trendline.Checked = true;
                                else
                                    selpar.CB_trendline.Checked = false;
                                break;
                            case "serieslabel":
                                if (words[1] == "on")
                                    selpar.CB_serieslabel.Checked = true;
                                else
                                    selpar.CB_serieslabel.Checked = false;
                                break;
                            case "startyear":
                                selpar.setstartyear(util.tryconvert(words[1]));
                                break;
                            case "endyear":
                                selpar.setendyear(util.tryconvert(words[1]));
                                break;
                            case "loop":
                                if (words[1] == "alluni")
                                {
                                    if (!loopuni)
                                    {
                                        loopuni = true;
                                    }
                                    else
                                        memo("invalid command inside loop");
                                }
                                else
                                    memo("invalid loop " + words[1]);
                                break;
                            case "end":
                                if (words[1] == "alluni")
                                {
                                    if (loopuni)
                                    {
                                        loopuni = false;
                                    }
                                    else
                                        memo("invalid command outside loop");
                                }
                                else
                                    memo("invalid loop " + words[1]);
                                break;
                            default:
                                memo("Invalid command " + line);
                                break;

                        }
                    }
                    memo("Done!");
                }
            }
            


        }

        private void selectcompetitors(string compgroup, string uni)
        {
            memo("Comparison " + compgroup + "; " + uni);
            switch (compgroup)
            {
                case "region":
                    selpar.CompetitorButton_Click(null, null);
                    break;
                case "nation":
                    selpar.setfocusuni("--Hela riket--");
                    break;
                default:
                    selpar.setfocusuni("--Hela riket--");
                    break;
            }
            selpar.setfocusuni(uni);
        }
    }
}
