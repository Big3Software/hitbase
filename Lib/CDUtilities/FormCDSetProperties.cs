using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormCDSetProperties : Form
    {
        private SetDataSet.SetRow cdSetRow;

        public FormCDSetProperties(SetDataSet.SetRow cdSetRow)
        {
            InitializeComponent();

            if (!cdSetRow.IsNameNull())
                textBoxCDSetName.Text = cdSetRow.Name;

            this.cdSetRow = cdSetRow;

            UpdateWindowState();

            FormThemeManager.SetTheme(this);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            cdSetRow.Name = textBoxCDSetName.Text;
        }

        private void textBoxCDSetName_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonOK.Enabled = textBoxCDSetName.Text.Length > 0;
        }
    }
}