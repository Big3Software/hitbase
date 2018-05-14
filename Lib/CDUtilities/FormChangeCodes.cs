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
    public partial class FormChangeCodes : Form
    {
        private DataBase dataBase;

        public FormChangeCodes(DataBase db)
        {
            InitializeComponent();
            
            FormThemeManager.SetTheme(this);

            dataBase = db;

            FillList(comboBoxSearchFor);
            FillList(comboBoxReplace);
        }

        public string SearchFor { get; set; }
        public string Replace { get; set; }

        private void FillList(ComboBox comboBox)
        {
            comboBox.BeginUpdate();
            for (int i = 0; i < 26; i++)
            {
                string str = string.Format("{0}: {1}", (char)(i + 65), dataBase.Codes[i]);

                comboBox.Items.Add(str);
            }
            comboBox.EndUpdate();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (comboBoxSearchFor.SelectedIndex >= 0)
                SearchFor = string.Format("{0}", (char)('A' + comboBoxSearchFor.SelectedIndex));

            if (comboBoxReplace.SelectedIndex >= 0)
                Replace = string.Format("{0}", (char)('A' + comboBoxReplace.SelectedIndex));
        }
    }
}
