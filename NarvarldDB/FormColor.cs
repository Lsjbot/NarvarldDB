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
    public partial class FormColor : Form
    {
        FormDisplay parent = null;
        Dictionary<string,Series> seriesdict = new Dictionary<string,Series>();

        public FormColor(FormDisplay parentpar)
        {
            InitializeComponent();
            parent = parentpar;

            foreach (Series ss in parent.chart1.Series)
            {
                LBseries.Items.Add(ss.Name);
                seriesdict.Add(ss.Name, ss);
                ss.Palette = ChartColorPalette.None;
            }
            parent.chart1.Palette = ChartColorPalette.Pastel;

            foreach (string p in Enum.GetNames(typeof(ChartColorPalette)))
            {
                LBpalette.Items.Add(p);
            }
            
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LBseries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LBseries.SelectedIndex != -1)
            {
                Series ss = seriesdict[LBseries.SelectedItem.ToString()];
                colorDialog1.Color = ss.Color;
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                    ss.Color = colorDialog1.Color;
            }
        }

        private void LBpalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            parent.chart1.Palette = (ChartColorPalette)Enum.Parse(typeof(ChartColorPalette), LBpalette.SelectedItem.ToString());
        }
    }
}
