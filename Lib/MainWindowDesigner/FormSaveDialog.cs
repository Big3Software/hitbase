using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.DataBaseEngine.CategoryDataSetTableAdapters;

namespace Big3.Hitbase.MainWindowDesigner
{
    public partial class FormSaveDialog : Form
    {
        public class ListBoxItem
        {
            public string Category;
            public int ID;

            public ListBoxItem(int id, string category)
            {
                ID = id;
                Category = category;
            }

            public override string ToString()
            {
                return Category;
            }
        }

        private DataBase dataBase;

        public FormSaveDialog(DataBase db)
        {
            InitializeComponent();

            dataBase = db;
        }

        private void FormSaveDialog_Load(object sender, EventArgs e)
        {
            CategoryTableAdapter kategorieTableAdapter = new CategoryTableAdapter(dataBase);
            CategoryDataSet ds = new CategoryDataSet();
            kategorieTableAdapter.Fill(ds.Category);
            foreach (CategoryDataSet.CategoryRow row in ds.Category.Rows)
            {
                listBoxCategories.Items.Add(new ListBoxItem(row.CategoryID, row.Name));
            }
        }

        private void radioButtonCategory_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWindowState();

            if (radioButtonCategory.Checked && listBoxCategories.SelectedItem == null)
                listBoxCategories.SelectedIndex = 0;
        }

        private void radioButtonStandard_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            listBoxCategories.Enabled = radioButtonCategory.Checked;
            buttonOK.Enabled = (!radioButtonCategory.Checked || listBoxCategories.SelectedItem != null);
        }

        public int SelectedCategoryId
        {
            get
            {
                if (radioButtonCategory.Checked)
                {
                    if (listBoxCategories.SelectedItem != null)
                    {
                        return ((ListBoxItem)listBoxCategories.SelectedItem).ID;
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false);
                        return 0;
                    }
                }
                else
                {
                    return 0;       // Standard
                }
            }
        }

        private void listBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }
    }
}