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
    public partial class OggVorbisSettings : Form
    {
        public OggVorbisSettings()
        {
            InitializeComponent();

            textOggParameter.Text = Settings.Current.RecordOggExeParameter;
            int currentPos;
            int nPos = textOggParameter.Text.IndexOf("-q ");

            string nPara;

            nPara = textOggParameter.Text.Substring(nPos + 3, 2);
            nPara = nPara.TrimEnd(' ');
            currentPos = Convert.ToInt32(nPara);
            trackBar1.Value = currentPos;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Settings.Current.RecordOggExeParameter = textOggParameter.Text;
        }

        private void buttonResetDefaultOGG_Click(object sender, EventArgs e)
        {
            // "-q -1" bis "-q 10"
            textOggParameter.Text = "-q 6 %1 %2";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int newQuality;
            newQuality = trackBar1.Value;
            int nPos = textOggParameter.Text.IndexOf("-q ");

            if (textOggParameter.Text.Length > nPos + 3)
            {
                string old;

                old = textOggParameter.Text.Substring(nPos, 5);
                old = old.TrimEnd(' ');
                textOggParameter.Text = textOggParameter.Text.Replace(old, "-q " + newQuality.ToString());
            }

        }
    }
}
