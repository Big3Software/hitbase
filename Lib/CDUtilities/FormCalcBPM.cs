using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormCalcBPM : Form
    {
        int lastTimePressed = 0;
        int firstTimePressed = 0;
        int currentBpm = 0;
        int numberOfTimesPressed = 0;

        public FormCalcBPM()
        {
            InitializeComponent();
        }

        public int BPM
        {
            get
            {
                return currentBpm;
            }
        }

        private void buttonBeat_Click(object sender, EventArgs e)
        {
            int curTick = System.Environment.TickCount;
        	int lastBPM;

	        if (this.firstTimePressed == 0)
		        this.firstTimePressed = curTick;

            lastBPM = (int)(60.0/(double)((curTick-lastTimePressed)/1000.0)+0.5);
	        if ((lastBPM > currentBpm*3/2 ||
		         lastBPM < currentBpm/2) &&
		         numberOfTimesPressed > 5)
            {                                    // Da hat aber einer geschlafen!
		        labelValue.ForeColor = Color.Red;
	        }
	
	        lastTimePressed = curTick;

	        if (numberOfTimesPressed > 0)
	        {
		        currentBpm = (int)((double)numberOfTimesPressed*60.0/(double)((lastTimePressed-firstTimePressed)/1000.0)+0.5);
		        labelValue.Text = currentBpm.ToString();
	        }

	        numberOfTimesPressed++;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            lastTimePressed = 0;
            firstTimePressed = 0;
            currentBpm = 0;
            numberOfTimesPressed = 0;

            labelValue.ForeColor = Label.DefaultForeColor;
            labelValue.Text = "";
        }
    }
}
