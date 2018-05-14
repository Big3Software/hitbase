using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.DataBaseEngine.CategoryDataSetTableAdapters;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls;
using System.Linq;
using Big3.Hitbase.SoundEngine;
using System.Windows.Threading;
using System.IO;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormCategories : Form
    {
        private class ListBoxItem
        {
            public CategoryDataSet.CategoryRow Category;

            public ListBoxItem(CategoryDataSet.CategoryRow category)
            {
                Category = category;
            }

            public override string ToString()
            {
                return Category.Name;
            }
        }

        private DataBase dataBase;

        public FormCategories(DataBase db)
        {
            dataBase = db;

            InitializeComponent();

            FormThemeManager.SetTheme(this);
        }

        private void FormCategories_Load(object sender, EventArgs e)
        {
            checkBoxAutoSort.Checked = Settings.Current.AutoSortGenres;

            FillList();

            UpdateWindowState();
        }

        private void FillList()
        {
            listBoxCategories.Items.Clear();

            CategoryTableAdapter cta = new CategoryTableAdapter(dataBase);
            CategoryDataSet.CategoryDataTable cdt = cta.GetData();

            if (checkBoxAutoSort.Checked)
            {
                foreach (CategoryDataSet.CategoryRow category in cdt.OrderBy(x => x.Name))
                {
                    listBoxCategories.Items.Add(new ListBoxItem(category));
                }
            }
            else
            {
                foreach (CategoryDataSet.CategoryRow category in cdt.OrderBy(x => x.Order))
                {
                    listBoxCategories.Items.Add(new ListBoxItem(category));
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormName formName = new FormName();
            formName.Text = StringTable.AddNewCategory;
            formName.NameValue = "";
            formName.AllowEmpty = false;
            formName.ValidateName += delegate(object sender1, ValidateNameEventArgs e1)
            {
                if (dataBase.AllCategories.Names.Contains(e1.Name) && e1.Name != e1.OriginalName)
                {
                    MessageBox.Show(string.Format(StringTable.CategoryAlreadyExists, e1.Name), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e1.Cancel = true;
                }
            };
            if (formName.ShowDialog(this) == DialogResult.OK)
            {
                CategoryTableAdapter cta = new CategoryTableAdapter(dataBase);
                CategoryDataSet.CategoryDataTable cdt = cta.GetData();
                cdt.AddCategoryRow(formName.NameValue, listBoxCategories.Items.Count + 1);
                cta.Update(cdt);

                FillList();
            }

            UpdateWindowState();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (listBoxCategories.SelectedItem == null)
                return;

            ListBoxItem item = (ListBoxItem)listBoxCategories.SelectedItem;

            FormName formName = new FormName();
            formName.Text = StringTable.EditCategory;
            formName.NameValue = item.Category.Name;
            if (formName.ShowDialog(this) == DialogResult.OK)
            {
                int oldId = dataBase.GetIdByCategory(item.Category.Name);

                CategoryTableAdapter cta = new CategoryTableAdapter(dataBase);
                CategoryDataSet.CategoryDataTable cdt = cta.GetDataById(item.Category.CategoryID);
                cdt[0].Name = formName.NameValue;
                cta.Update(cdt);

                listBoxCategories.Items[listBoxCategories.SelectedIndex] = new ListBoxItem(cdt[0]);

                UpdateSoundfiles(oldId, item.Category.Name, formName.NameValue, false);
            }

            UpdateWindowState();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxCategories.SelectedItem == null)
                return;

            ListBoxItem item = (ListBoxItem)listBoxCategories.SelectedItem;
            // Prüfen, ob es CDs bzw. Tracks mit der zu löschenden Kategorie gibt.
            int countCD = 0;
            int countTrack = 0;
            if (dataBase.IsCategoryUsedCount(item.Category.CategoryID, out countCD, out countTrack))
            {
                FormDeleteCategory formDeleteCategory = new FormDeleteCategory(dataBase, item.Category.Name, countCD, countTrack);
                if (formDeleteCategory.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    // Genres in MP3s löschen bzw. umbenennen
                    int oldId = dataBase.GetIdByCategory(item.Category.Name);
                    UpdateSoundfiles(oldId, item.Category.Name, formDeleteCategory.SelectedCategory, true);
                }
            }
            else
            {
                if (MessageBox.Show(string.Format(StringTable.DeleteCategory, item.Category.Name), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dataBase.DeleteCategory(item.Category.Name, null);
                    listBoxCategories.Items.Remove(item);
                    SetOrderByList();
                }
            }

            UpdateWindowState();
        }

        private void UpdateSoundfiles(int oldId, string oldGenre, string newGenre, bool delete)
        {
            WaitProgressWindow waitProgress = new WaitProgressWindow();
            waitProgress.Show();
            waitProgress.progressControl.textBlockStatus.Text = StringTable.UpdatingGenres;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                DataTable dt = dataBase.ExecuteFreeSql("SELECT Soundfile from Track WHERE CategoryID = " + oldId);

                waitProgress.Dispatcher.Invoke((Action)(() =>
                    {
                        waitProgress.progressControl.progressBar.Maximum = dt.Rows.Count;
                    }));

                foreach (DataRow row in dt.Rows)
                {
                    if (waitProgress.Canceled)
                        break;

                    string filename = row[0] as string;

                    if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
                    {
                        SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(filename);
                        if (string.Compare(sfi.Genre, oldGenre, true) == 0)
                        {
                            sfi.Genre = newGenre;
                            SoundFileInformation.WriteMP3Tags(sfi, Field.TrackCategory);
                        }
                    }

                    waitProgress.Dispatcher.Invoke((Action)(() =>
                    {
                        waitProgress.progressControl.progressBar.Value++;
                    }));
                }
            };
            bw.RunWorkerCompleted += delegate
            {
                waitProgress.Close();
                if (!waitProgress.Canceled)
                {
                    if (delete)
                    {
                        dataBase.DeleteCategory(oldGenre, newGenre);
                    }

                    FillList();

                    SetOrderByList();
                }
            };
            bw.RunWorkerAsync();
        }

        /// <summary>
        /// Aktualisiert die Order aus der Liste in die Datenbank.
        /// </summary>
        private void SetOrderByList()
        {
            int index = 1;
            
            foreach (ListBoxItem item in listBoxCategories.Items)
            {
                CategoryTableAdapter cta = new CategoryTableAdapter(dataBase);
                CategoryDataSet.CategoryDataTable dt = cta.GetDataById(item.Category.CategoryID);
                dt[0].Order = index;

                cta.Update(dt);

                index++;
            }
        }

        private void listBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonEdit.Enabled = (listBoxCategories.SelectedIndex >= 0);
            buttonDelete.Enabled = (listBoxCategories.SelectedIndex >= 0);
            buttonUp.Enabled = (listBoxCategories.SelectedIndex > 0 && !checkBoxAutoSort.Checked);
            buttonDown.Enabled = (listBoxCategories.SelectedIndex >= 0 && listBoxCategories.SelectedIndex < listBoxCategories.Items.Count - 1 && !checkBoxAutoSort.Checked);
            buttonUp.Visible = !checkBoxAutoSort.Checked;
            buttonDown.Visible = !checkBoxAutoSort.Checked;
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (listBoxCategories.SelectedItem == null)
                return;

            ListBoxItem item = (ListBoxItem)listBoxCategories.SelectedItem;
            int index = listBoxCategories.SelectedIndex;
            listBoxCategories.Items.RemoveAt(index);
            listBoxCategories.Items.Insert(index - 1, item);
            listBoxCategories.SelectedIndex = index - 1;

            SetOrderByList();

            UpdateWindowState();
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (listBoxCategories.SelectedItem == null)
                return;

            ListBoxItem item = (ListBoxItem)listBoxCategories.SelectedItem;
            int index = listBoxCategories.SelectedIndex;
            listBoxCategories.Items.RemoveAt(index);
            listBoxCategories.Items.Insert(index + 1, item);
            listBoxCategories.SelectedIndex = index + 1;

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

                CategoryTableAdapter cta = new CategoryTableAdapter(dataBase);
                CategoryDataSet.CategoryDataTable cdt = cta.GetData();

                int count = listBoxCategories.Items.Count + 1;
                foreach (Category category in dbImport.AllCategories)
                {
                    if (dataBase.GetIdByCategory(category.Name) < 0)
                    {
                        cdt.AddCategoryRow(category.Name, count++);
                    }
                }

                cta.Update(cdt);

                dbImport.Close();

                FillList();
                UpdateWindowState();
            }
        }

        private void FormCategories_FormClosed(object sender, FormClosedEventArgs e)
        {
            dataBase.UpdateCategories();
        }

        private void checkBoxAutoSort_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Current.AutoSortGenres = checkBoxAutoSort.Checked;

            UpdateWindowState();

            FillList();
        }
    }
}
