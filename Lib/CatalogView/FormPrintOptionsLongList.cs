using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.CatalogView
{
    public partial class FormPrintOptionsLongList : Form
    {
        public FormPrintOptionsLongList()
        {
            InitializeComponent();

            checkBoxPrintCDCover.Checked = Settings.Current.PrintCDCover;
            checkBoxTextBelowCDCover.Checked = Settings.Current.PrintTextUnderCDCover;
            numericUpDownCDCoverSize.Value = Settings.Current.PrintCDCoverSize;
            if (Settings.Current.PrintCDCoverAlign == 0)
                radioButtonCDCoverPositionLeft.Checked = true;
            if (Settings.Current.PrintCDCoverAlign == 1)
                radioButtonCDCoverPositionCenter.Checked = true;
            if (Settings.Current.PrintCDCoverAlign == 2)
                radioButtonCDCoverPositionRight.Checked = true;
        }

        private void FormPrintOptionsLongList_Load(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            groupBoxPosition.Enabled = checkBoxPrintCDCover.Checked && checkBoxTextBelowCDCover.Checked;
            labelCDCoverSize.Enabled = checkBoxPrintCDCover.Checked;
            numericUpDownCDCoverSize.Enabled = checkBoxPrintCDCover.Checked;
            checkBoxTextBelowCDCover.Enabled = checkBoxPrintCDCover.Checked;
        }

        private void checkBoxPrintCDCover_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Settings.Current.PrintCDCover= checkBoxPrintCDCover.Checked;
            Settings.Current.PrintTextUnderCDCover = checkBoxTextBelowCDCover.Checked;
            Settings.Current.PrintCDCoverSize = (int)numericUpDownCDCoverSize.Value;
            if (radioButtonCDCoverPositionLeft.Checked)
                Settings.Current.PrintCDCoverAlign = 0;
            if (radioButtonCDCoverPositionCenter.Checked)
                Settings.Current.PrintCDCoverAlign = 1;
            if (radioButtonCDCoverPositionRight.Checked)
                Settings.Current.PrintCDCoverAlign = 2;
        }

        private void checkBoxTextBelowCDCover_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxTextBelowCDCover.Checked)
            {
                radioButtonCDCoverPositionLeft.Checked = false;
                radioButtonCDCoverPositionCenter.Checked = false;
                radioButtonCDCoverPositionRight.Checked = true;
            }
            UpdateWindowState();
        }
    }
}
