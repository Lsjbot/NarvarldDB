using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NarvarldDB
{
    public partial class Form1 : Form
    {
        static string connectionstring = "Data Source=db-tgsanalys-test.du.se;Initial Catalog=dbTGSAnalysTest;Integrated Security=True;Pooling=False";
        static DbTGSAnalysTest db = null;

        public Form1()
        {
            InitializeComponent();
            db = new DbTGSAnalysTest(connectionstring);

        }

        private void Quitbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DBbutton_Click(object sender, EventArgs e)
        {
            FormFillDB fdb = new FormFillDB(db);
            fdb.Show();
        }

        private void Readbutton_Click(object sender, EventArgs e)
        {
            FormDisplay fd = new FormDisplay(db);
            fd.Show();
        }
    }
}
