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
    public partial class FormSelectSpecial : Form
    {
        public string selection = "";
        public FormSelectSpecial()
        {
            InitializeComponent();
        }

        private void Quitbutton_Click(object sender, EventArgs e)
        {
            selection = "";
            this.Close();
        }

        private void Engbutton_Click(object sender, EventArgs e)
        {
            selection = "högsking";
            this.Close();
        }

        private void Teacherbutton_Click(object sender, EventArgs e)
        {
            selection = "lärare";
            this.Close();
        }

        private void Nursebutton_Click(object sender, EventArgs e)
        {
            selection = "ssk";
            this.Close();
        }

        private void Specsskbutton_Click(object sender, EventArgs e)
        {
            selection = "specssk";
            this.Close();
        }
    }
}
