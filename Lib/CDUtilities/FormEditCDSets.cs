using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.DataBaseEngine.SetDataSetTableAdapters;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormEditCDSets : Form
    {
        SetTableAdapter cdSetAdapter;
        SetDataSet cdSetDataset = new SetDataSet();
        DataBase dataBase = null;

        public FormEditCDSets(DataBase db)
        {
            InitializeComponent();
            
            FormThemeManager.SetTheme(this);

            dataBase = db;
            cdSetAdapter = new SetTableAdapter(db);

            XPTable.Renderers.GradientHeaderRenderer gradHeader = new XPTable.Renderers.GradientHeaderRenderer();
            cdSetTable.HeaderRenderer = gradHeader;
            cdSetTable.FullRowSelect = true;
            cdSetTable.SelectionStyle = XPTable.Models.SelectionStyle.ListView;
            cdSetTable.AlternatingRowColor = Color.WhiteSmoke;

            cdSetTable.GridLines = XPTable.Models.GridLines.Both;
            cdSetTable.GridLineStyle = XPTable.Models.GridLineStyle.Solid;

            cdSetTable.TableModel = new XPTable.Models.TableModel();
            cdSetTable.ColumnModel = new XPTable.Models.ColumnModel();

            cdSetTable.TableModel.RowHeight += 4;

            cdSetTable.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(StringTable.CDSet, cdSetTable.Width-30));
            cdSetTable.ColumnModel.Columns[0].Editable = false;
        }

        private void FormEditCDSets_Load(object sender, EventArgs e)
        {
            FillCDSetList();
        }

        private void FillCDSetList()
        {
            Cursor.Current = Cursors.WaitCursor;

            cdSetDataset.Clear();
            cdSetAdapter.Fill(cdSetDataset.Set);

            cdSetTable.TableModel.Rows.Clear();

            foreach (SetDataSet.SetRow row in cdSetDataset.Set)
            {
                XPTable.Models.Row newRow = new XPTable.Models.Row();
                newRow.Cells.Add(new XPTable.Models.Cell(row.Name));

                newRow.Tag = row;

                cdSetTable.TableModel.Rows.Add(newRow);
            }

            Cursor.Current = Cursors.Default;
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            New();
        }

        private void New()
        {
            SetDataSet.SetRow cdSetRow = cdSetDataset.Set.NewSetRow();

            if (cdSetRow == null)
                return;

            FormCDSetProperties formCDSet = new FormCDSetProperties(cdSetRow);

            if (formCDSet.ShowDialog(this) == DialogResult.OK)
            {
                cdSetDataset.Set.AddSetRow(cdSetRow);
                cdSetAdapter.Update(cdSetDataset.Set);

                FillCDSetList();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            Edit();
        }

        private void Edit()
        {
            if (cdSetTable.SelectedItems.Length < 1)
                return;

            SetDataSet.SetRow cdSetRow = (SetDataSet.SetRow)cdSetTable.TableModel.Rows[cdSetTable.SelectedIndicies[0]].Tag;

            if (cdSetRow == null)
                return;

            FormCDSetProperties formCDSet = new FormCDSetProperties(cdSetRow);

            if (formCDSet.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    cdSetAdapter.Update(cdSetRow);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                FillCDSetList();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void Delete()
        {
            if (cdSetTable.SelectedItems.Length < 1)
                return;

            SetDataSet.SetRow selCDSet = (SetDataSet.SetRow)cdSetTable.TableModel.Rows[cdSetTable.SelectedIndicies[0]].Tag;

            if (selCDSet == null)
                return;

            int selectedIndex = cdSetTable.SelectedIndicies[0];

            if (MessageBox.Show(String.Format(StringTable.DeleteCDSet, selCDSet.Name), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            // Prüfen, ob es noch CDs mit dem CD-Set gibt.
            int cdSetCount = Convert.ToInt32(dataBase.ExecuteScalar(string.Format("SELECT count(*) FROM CD where SetID = {0}", selCDSet.SetID)));

            if (cdSetCount > 0)
            {
                MessageBox.Show(string.Format(StringTable.DeleteCDSetNotAllowed, selCDSet.Name), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                dataBase.ExecuteNonQuery(string.Format("DELETE FROM [Set] WHERE SetID={0}", selCDSet.SetID));

                cdSetTable.TableModel.Rows.RemoveAt(selectedIndex);
                FillCDSetList();
            }
        }

        private void cdSetTable_DoubleClick(object sender, EventArgs e)
        {
            Edit();
        }
    }
}