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
    public partial class FormDeleteMedium : Form
    {
        private string mediumToDelete;
        private DataBase dataBase;

        public FormDeleteMedium(DataBase db, string mediumToDelete)
        {
            InitializeComponent();

            this.mediumToDelete = mediumToDelete;
            dataBase = db;
            UpdateWindowState();

            FormThemeManager.SetTheme(this);
        }

        private void radioButtonDeleteCategory_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void radioButtonChangeCategory_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            comboBoxMediums.Enabled = radioButtonChangeCategory.Checked;
        }
    
        private void FillMediums()
        {
            foreach (Medium medium in dataBase.AllMediums)
            {
                if (medium.Name != mediumToDelete)
                    comboBoxMediums.Items.Add(medium.Name);
            }
        }

        private void FormDeleteMedium_Load(object sender, EventArgs e)
        {
            FillMediums();
        }

        public string SelectedMedium
        {
            get
            {
                return comboBoxMediums.SelectedItem as string;
            }
        }

    }
}
