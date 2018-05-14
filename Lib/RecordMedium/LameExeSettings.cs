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
    public partial class LameExeSettings : Form
    {
        public LameExeSettings()
        {
            InitializeComponent();

            textLameParameter.Text = Settings.Current.RecordLameExeParameter;
            int currentPos;
            int nPos = textLameParameter.Text.IndexOf("-V ");

            currentPos = Convert.ToInt32(textLameParameter.Text.Substring(nPos + 3, 1));
            trackBar1.Value = 9 - currentPos;
        }

        private void buttonResetDefaultMP3_Click(object sender, EventArgs e)
        {
            textLameParameter.Text = "-V 2 --vbr-new %1 %2";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int newQuality;
            newQuality = 9 - trackBar1.Value;
            int nPos = textLameParameter.Text.IndexOf("-V ");
            
            if (textLameParameter.Text.Length > nPos + 3)
            {
                string old;

                old = textLameParameter.Text.Substring(nPos, 4);
                textLameParameter.Text = textLameParameter.Text.Replace(old, "-V " + newQuality.ToString());
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Settings.Current.RecordLameExeParameter = textLameParameter.Text;
        }
    }
}
