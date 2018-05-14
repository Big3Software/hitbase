using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.DataBaseEngine.MediumDataSetTableAdapters;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormMediums : Form
    {
        private class ListBoxItem
        {
            public MediumDataSet.MediumRow Medium;

            public ListBoxItem(MediumDataSet.MediumRow medium)
            {
                Medium = medium;
            }

            public override string ToString()
            {
                return Medium.Name;
            }
        }

        private DataBase dataBase;

        public FormMediums(DataBase db)
        {
            dataBase = db;

            InitializeComponent();

            FormThemeManager.SetTheme(this);
        }

        private void FormMediums_Load(object sender, EventArgs e)
        {
            checkBoxAutoSort.Checked = Settings.Current.AutoSortMediums;

            FillList();

            UpdateWindowState();
        }

        private void FillList()
        {
            listBoxMedium.Items.Clear();

            MediumTableAdapter mta = new MediumTableAdapter(dataBase);
            MediumDataSet.MediumDataTable mdt = mta.GetData();
            if (Settings.Current.AutoSortMediums)
            {
                foreach (MediumDataSet.MediumRow medium in mdt.OrderBy(x => x.Name))
                {
                    listBoxMedium.Items.Add(new ListBoxItem(medium));
                }
            }
            else
            {
                foreach (MediumDataSet.MediumRow medium in mdt.OrderBy(x => x.Order))
                {
                    listBoxMedium.Items.Add(new ListBoxItem(medium));
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormName formName = new FormName();
            formName.Text = StringTable.AddNewMedium;
            formName.NameValue = "";
            formName.AllowEmpty = false;
            formName.ValidateName += delegate(object sender1, ValidateNameEventArgs e1)
            {
                if (dataBase.AllMediums.Names.Contains(e1.Name) && e1.Name != e1.OriginalName)
                {
                    MessageBox.Show(string.Format(StringTable.MediumAlreadyExists, e1.Name), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e1.Cancel = true;
                }
            };
            if (formName.ShowDialog(this) == DialogResult.OK)
            {
                MediumTableAdapter cta = new MediumTableAdapter(dataBase);
                MediumDataSet.MediumDataTable cdt = cta.GetData();
                cdt.AddMediumRow(formName.NameValue, listBoxMedium.Items.Count + 1);
                cta.Update(cdt);

                FillList();
            }

            UpdateWindowState();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (listBoxMedium.SelectedItem == null)
                return;

            ListBoxItem item = (ListBoxItem)listBoxMedium.SelectedItem;

            FormName formName = new FormName();
            formName.Text = StringTable.EditMedium;
            formName.NameValue = item.Medium.Name;
            if (formName.ShowDialog(this) == DialogResult.OK)
            {
                MediumTableAdapter mta = new MediumTableAdapter(dataBase);
                MediumDataSet.MediumDataTable mdt = mta.GetDataById(item.Medium.MediumID);
                mdt[0].Name = formName.NameValue;
                mta.Update(mdt);

                listBoxMedium.Items[listBoxMedium.SelectedIndex] = new ListBoxItem(mdt[0]);
            }

            UpdateWindowState();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxMedium.SelectedItem == null)
                return;

            ListBoxItem item = (ListBoxItem)listBoxMedium.SelectedItem;
            // Prüfen, ob es CDs bzw. Tracks mit dem zu löschenden Medium gibt.
            if (dataBase.IsMediumUsed(item.Medium.MediumID))
            {
                FormDeleteMedium formDeleteMedium = new FormDeleteMedium(dataBase, item.Medium.Name);
                if (formDeleteMedium.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    dataBase.DeleteMedium(item.Medium.Name, formDeleteMedium.SelectedMedium);
                    listBoxMedium.Items.Remove(item);
                    SetOrderByList();
                }
            }
            else
            {
                if (MessageBox.Show(string.Format(StringTable.DeleteMedium, item.Medium.Name), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dataBase.DeleteMedium(item.Medium.Name, null);
                    listBoxMedium.Items.Remove(item);
                    SetOrderByList();
                }
            }

            UpdateWindowState();
        }

        /// <summary>
        /// Aktualisiert die Order aus der Liste in die Datenbank.
        /// </summary>
        private void SetOrderByList()
        {
            int index = 1;
            
            foreach (ListBoxItem item in listBoxMedium.Items)
            {
                MediumTableAdapter mta = new MediumTableAdapter(dataBase);
                MediumDataSet.MediumDataTable dt = mta.GetDataByName(item.Medium.Name);
                dt[0].Order = index;

                mta.Update(dt);

                index++;
            }
        }

        private void listBoxMediums_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonEdit.Enabled = (listBoxMedium.SelectedIndex >= 0);
            buttonDelete.Enabled = (listBoxMedium.SelectedIndex >= 0);
            buttonUp.Enabled = (listBoxMedium.SelectedIndex > 0);
            buttonDown.Enabled = (listBoxMedium.SelectedIndex >= 0 && listBoxMedium.SelectedIndex < listBoxMedium.Items.Count - 1);
            buttonUp.Visible = !checkBoxAutoSort.Checked;
            buttonDown.Visible = !checkBoxAutoSort.Checked;
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (listBoxMedium.SelectedItem == null)
                return;

            ListBoxItem item = (ListBoxItem)listBoxMedium.SelectedItem;
            int index = listBoxMedium.SelectedIndex;
            listBoxMedium.Items.RemoveAt(index);
            listBoxMedium.Items.Insert(index - 1, item);
            listBoxMedium.SelectedIndex = index - 1;

            SetOrderByList();

            UpdateWindowState();
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (listBoxMedium.SelectedItem == null)
                return;

            ListBoxItem item = (ListBoxItem)listBoxMedium.SelectedItem;
            int index = listBoxMedium.SelectedIndex;
            listBoxMedium.Items.RemoveAt(index);
            listBoxMedium.Items.Insert(index + 1, item);
            listBoxMedium.SelectedIndex = index + 1;

            SetOrderByList();

            UpdateWindowState();
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = StringTable.FilterHitbase;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                DataBase dbImport = new DataBase();
                dbImport.Open(openFileDialog.FileName);

                MediumTableAdapter cta = new MediumTableAdapter(dataBase);
                MediumDataSet.MediumDataTable cdt = cta.GetData();

                int count = listBoxMedium.Items.Count + 1;
                foreach (Medium Medium in dbImport.AllMediums)
                {
                    if (dataBase.GetIdByMedium(Medium.Name) < 0)
                    {
                        cdt.AddMediumRow(Medium.Name, count++);
                    }
                }

                cta.Update(cdt);

                dbImport.Close();

                FillList();
                UpdateWindowState();
            }
        }

        private void FormMediums_FormClosed(object sender, FormClosedEventArgs e)
        {
            dataBase.UpdateMediums();
        }

        private void checkBoxAutoSort_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Current.AutoSortMediums = checkBoxAutoSort.Checked;

            UpdateWindowState();

            FillList();
        }
    }
}
