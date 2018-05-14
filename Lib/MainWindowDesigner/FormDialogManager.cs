using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using System.IO;
using Big3.Hitbase.DataBaseEngine.DialogDataSetTableAdapters;
using Big3.Hitbase.DataBaseEngine.CategoryDataSetTableAdapters;

namespace Big3.Hitbase.MainWindowDesigner
{
    public partial class FormDialogManager : Form
    {
        private DataBase dataBase;
        private DialogDataSet dialogsDataset;
        private DialogTableAdapter dialogTableAdapter;

        public FormDialogManager(DataBase db)
        {
            InitializeComponent();

            dataBase = db;
            dialogTableAdapter = new DialogTableAdapter(dataBase);
        }

        private void FormSaveDialog_Load(object sender, EventArgs e)
        {
            LoadDialogs();

            tableDialogs.GridLines = XPTable.Models.GridLines.Both;
            tableDialogs.GridLineStyle = XPTable.Models.GridLineStyle.Solid;

            XPTable.Renderers.GradientHeaderRenderer gradHeader = new XPTable.Renderers.GradientHeaderRenderer();
            tableDialogs.HeaderRenderer = gradHeader;
            tableDialogs.FullRowSelect = true;
            tableDialogs.SelectionStyle = XPTable.Models.SelectionStyle.ListView;
            tableDialogs.AlternatingRowColor = Color.WhiteSmoke;

            XPTable.Models.Column newColumn = new XPTable.Models.TextColumn();

            tableDialogs.TableModel = new XPTable.Models.TableModel();
            tableDialogs.TableModel.RowHeight = 20;
            XPTable.Models.ColumnModel model = new XPTable.Models.ColumnModel();

            model.Columns.Add(new XPTable.Models.TextColumn(StringTable.Category, 150));
            model.Columns.Add(new XPTable.Models.TextColumn(StringTable.DialogDefined, 200));

            model.Columns[0].Editable = false;
            model.Columns[1].Editable = false;

            tableDialogs.ColumnModel = model;

            FillList();
        }

        private void LoadDialogs()
        {
            dialogsDataset = new DialogDataSet();

            dialogTableAdapter.Fill(dialogsDataset.Dialog);
        }

        private void FillList()
        {
            tableDialogs.TableModel.Rows.Clear();

            CategoryTableAdapter kategorieTableAdapter = new CategoryTableAdapter(dataBase);
            CategoryDataSet ds = new CategoryDataSet();
            kategorieTableAdapter.Fill(ds.Category);

            // Zuerst der Standard-Dialog
            string[] newRow = new string[2];
            newRow[0] = "<" + StringTable.Default + ">";
            newRow[1] = HasDialog(0) ? StringTable.Yes : StringTable.No;
            XPTable.Models.Row newTableRow = new XPTable.Models.Row(newRow);
            newTableRow.Tag = null;
            tableDialogs.TableModel.Rows.Add(newTableRow);

            foreach (CategoryDataSet.CategoryRow row in ds.Category.Rows)
            {
                newRow[0] = row.Name;
                newRow[1] = HasDialog(row.CategoryID) ? StringTable.Yes : StringTable.No;
                newTableRow = new XPTable.Models.Row(newRow);
                newTableRow.Tag = row;
                tableDialogs.TableModel.Rows.Add(newTableRow);
            }
        }

        private bool HasDialog(int categoryId)
        {
            if (GetDialogRowFromCategory(categoryId) != null)
                return true;
            else
                return false;
        }

        private DialogDataSet.DialogRow GetDialogRowFromCategory(int categoryId)
        {
            foreach (DialogDataSet.DialogRow dlgRow in dialogsDataset.Dialog)
            {
                if (dlgRow.CategoryID == categoryId)
                    return dlgRow;
            }

            return null;
        }

        private void UpdateWindowState()
        {
            buttonImport.Enabled = (tableDialogs.SelectedItems != null && tableDialogs.SelectedItems.Length > 0);

            bool hasDialog = false;
            if (tableDialogs.SelectedItems != null && tableDialogs.SelectedItems.Length > 0)
            {
                CategoryDataSet.CategoryRow row = ((CategoryDataSet.CategoryRow)tableDialogs.SelectedItems[0].Tag);

                DialogDataSet.DialogRow dialogRow;
                if (row == null)
                    dialogRow = GetDialogRowFromCategory(0);
                else
                    dialogRow = GetDialogRowFromCategory(row.CategoryID);

                if (dialogRow != null)
                    hasDialog = true;
            }

            buttonExport.Enabled = hasDialog;
            buttonEdit.Enabled = hasDialog;
            buttonDelete.Enabled = hasDialog;
        }

        private void tableDialogs_SelectionChanged(object sender, XPTable.Events.SelectionEventArgs e)
        {
            UpdateWindowState();
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (tableDialogs.SelectedItems == null || tableDialogs.SelectedItems.Length == 0)
                return;

            CategoryDataSet.CategoryRow row = ((CategoryDataSet.CategoryRow)tableDialogs.SelectedItems[0].Tag);

            DialogDataSet.DialogRow dialogRow;
            if (row == null)
                dialogRow = GetDialogRowFromCategory(0);
            else
                dialogRow = GetDialogRowFromCategory(row.CategoryID);

            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = StringTable.XmlFileDialogFilter;

            if (fileDlg.ShowDialog(this) == DialogResult.OK)
            {
                // Prüfen, ob es sich um einen Dialog handelt
                string xmlDialog = File.ReadAllText(fileDlg.FileName);
                string errorMessage = "";

                if (!MainCDUserControl.IsHitbaseDialog(xmlDialog, ref errorMessage))
                {
                    MessageBox.Show(StringTable.NoHitbaseDialog, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DialogTableAdapter ta = new DialogTableAdapter(dataBase);

                if (dialogRow == null)      // Neu
                {
                    DialogDataSet ds = new DialogDataSet();
                    dialogRow = ds.Dialog.AddDialogRow(row.CategoryID, xmlDialog);
                }
                else
                {
                    dialogRow.DialogXML = xmlDialog;
                }

                ta.Update(dialogRow);

                LoadDialogs();
                FillList();
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            if (tableDialogs.SelectedItems == null || tableDialogs.SelectedItems.Length == 0)
                return;

            CategoryDataSet.CategoryRow row = ((CategoryDataSet.CategoryRow)tableDialogs.SelectedItems[0].Tag);

            DialogDataSet.DialogRow dialogRow;
            if (row == null)
                dialogRow = GetDialogRowFromCategory(0);
            else
                dialogRow = GetDialogRowFromCategory(row.CategoryID);

            if (dialogRow == null)
                return;

            SaveFileDialog fileDlg = new SaveFileDialog();
            fileDlg.Filter = StringTable.XmlFileDialogFilter;

            if (fileDlg.ShowDialog(this) == DialogResult.OK)
            {
                File.WriteAllText(fileDlg.FileName, dialogRow.DialogXML);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (tableDialogs.SelectedItems == null || tableDialogs.SelectedItems.Length == 0)
                return;

            CategoryDataSet.CategoryRow row = ((CategoryDataSet.CategoryRow)tableDialogs.SelectedItems[0].Tag);

            if (row == null)
            {
                // Der Standarddialog kann nicht gelöscht werden
                MessageBox.Show(StringTable.CanNotDeleteStandardDialog, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string question = string.Format(StringTable.QueryDeleteDialog, row.Name);
            if (MessageBox.Show(question, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DialogDataSet.DialogRow dialogRow;
                dialogRow = GetDialogRowFromCategory(row.CategoryID);

                if (dialogRow == null)
                {
                    MessageBox.Show(StringTable.ErrorDeleteDialog, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DialogTableAdapter ta = new DialogTableAdapter(dataBase);
                ta.Delete(dialogRow.DialogID);

                LoadDialogs();
                FillList();
            }
        }
    }
}