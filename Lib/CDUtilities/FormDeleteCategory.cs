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
    public partial class FormDeleteCategory : Form
    {
        private string categoryToDelete;
        private DataBase dataBase;

        public FormDeleteCategory(DataBase db, string categoryToDelete, int countCD, int countTrack)
        {
            InitializeComponent();
            labelInfo.Text = string.Format(labelInfo.Text, countCD, countTrack, categoryToDelete);

            this.categoryToDelete = categoryToDelete;
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
            comboBoxCategories.Enabled = radioButtonChangeCategory.Checked;
        }

        private void FormDeleteCategory_Load(object sender, EventArgs e)
        {
            FillCategories();
        }

        private void FillCategories()
        {
            foreach (Category category in dataBase.AllCategories)
            {
                if (category.Name != categoryToDelete)
                    comboBoxCategories.Items.Add(category.Name);
            }
        }

        public string SelectedCategory
        {
            get
            {
                return comboBoxCategories.SelectedItem as string;
            }
        }
    }
}
