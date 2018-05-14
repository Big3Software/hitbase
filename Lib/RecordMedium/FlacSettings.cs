using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.RecordMedium
{
    public partial class FlacSettings : Form
    {
        public FlacSettings()
        {
            InitializeComponent();
            textFlacParameter.Text = Settings.Current.RecordFlacExeParameter;

            if (textFlacParameter.Text.IndexOf("-0 ") >= 0)
                trackBar1.Value = 0;
            if (textFlacParameter.Text.IndexOf("-1 ") >= 0)
                trackBar1.Value = 1;
            if (textFlacParameter.Text.IndexOf("-2 ") >= 0)
                trackBar1.Value = 2;
            if (textFlacParameter.Text.IndexOf("-3 ") >= 0)
                trackBar1.Value = 3;
            if (textFlacParameter.Text.IndexOf("-4 ") >= 0)
                trackBar1.Value = 4;
            if (textFlacParameter.Text.IndexOf("-5 ") >= 0)
                trackBar1.Value = 5;
            if (textFlacParameter.Text.IndexOf("-6 ") >= 0)
                trackBar1.Value = 6;
            if (textFlacParameter.Text.IndexOf("-7 ") >= 0)
                trackBar1.Value = 7;
            if (textFlacParameter.Text.IndexOf("-8 ") >= 0)
                trackBar1.Value = 8;
        }

        private void buttonResetDefaultMP3_Click(object sender, EventArgs e)
        {
            // flac "-0" bis "-8" - "-5" empfohlen
            textFlacParameter.Text = "-5 %1 %2";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Settings.Current.RecordFlacExeParameter = textFlacParameter.Text;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (textFlacParameter.Text.IndexOf("-0 ") >= 0)
                textFlacParameter.Text = textFlacParameter.Text.Replace("-0 ", "-" + trackBar1.Value.ToString() + " ");
            if (textFlacParameter.Text.IndexOf("-1 ") >= 0)
                textFlacParameter.Text = textFlacParameter.Text.Replace("-1 ", "-" + trackBar1.Value.ToString() + " ");
            if (textFlacParameter.Text.IndexOf("-2 ") >= 0)
                textFlacParameter.Text = textFlacParameter.Text.Replace("-2 ", "-" + trackBar1.Value.ToString() + " ");
            if (textFlacParameter.Text.IndexOf("-3 ") >= 0)
                textFlacParameter.Text = textFlacParameter.Text.Replace("-3 ", "-" + trackBar1.Value.ToString() + " ");
            if (textFlacParameter.Text.IndexOf("-4 ") >= 0)
                textFlacParameter.Text = textFlacParameter.Text.Replace("-4 ", "-" + trackBar1.Value.ToString() + " ");
            if (textFlacParameter.Text.IndexOf("-5 ") >= 0)
                textFlacParameter.Text = textFlacParameter.Text.Replace("-5 ", "-" + trackBar1.Value.ToString() + " ");
            if (textFlacParameter.Text.IndexOf("-6 ") >= 0)
                textFlacParameter.Text = textFlacParameter.Text.Replace("-6 ", "-" + trackBar1.Value.ToString() + " ");
            if (textFlacParameter.Text.IndexOf("-7 ") >= 0)
                textFlacParameter.Text = textFlacParameter.Text.Replace("-7 ", "-" + trackBar1.Value.ToString() + " ");
            if (textFlacParameter.Text.IndexOf("-8 ") >= 0)
                textFlacParameter.Text = textFlacParameter.Text.Replace("-8 ", "-" + trackBar1.Value.ToString() + " ");
        }
    }
}
