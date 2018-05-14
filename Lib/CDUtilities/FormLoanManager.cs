using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.DataBaseEngine.LoanedCDDataSetTableAdapters;
using Big3.Hitbase.DataBaseEngine.CDDataSetTableAdapters;
using Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormLoanManager : Form
    {
        private DataBase dataBase;

        public FormLoanManager(DataBase db)
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);

            dataBase = db;

            DefineTableLayout();

            Tools.FormatXPTable(tableLoanedCDs);

            FillList();
        }

        private void DefineTableLayout()
        {
            tableLoanedCDs.TableModel = new XPTable.Models.TableModel();
            tableLoanedCDs.ColumnModel = new XPTable.Models.ColumnModel();

            tableLoanedCDs.TableModel.RowHeight += 4;

            tableLoanedCDs.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(StringTable.Artist, 100));
            tableLoanedCDs.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(StringTable.Title, 100));
            tableLoanedCDs.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(StringTable.LoanedTo, 100));
            tableLoanedCDs.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(StringTable.LoanedDate, 100));
            tableLoanedCDs.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(StringTable.ReturnDate, 100));
            tableLoanedCDs.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(StringTable.Comment, 100));

            for (int i = 0; i < tableLoanedCDs.ColumnModel.Columns.Count; i++)
                tableLoanedCDs.ColumnModel.Columns[i].Editable = false;

            tableLoanedCDs.SelectionChanged += new XPTable.Events.SelectionEventHandler(tableLoanedCDs_SelectionChanged);
        }

        void tableLoanedCDs_SelectionChanged(object sender, XPTable.Events.SelectionEventArgs e)
        {
            UpdateWindowState();
        }

        private void FillList()
        {
            LoanedCDTableAdapter ta = new LoanedCDTableAdapter(dataBase);
            LoanedCDDataSet.LoanedCDDataTable dt = ta.GetData();

            CDTableAdapter cdta = new CDTableAdapter(dataBase);
            PersonGroupTableAdapter personGroupta = new PersonGroupTableAdapter(dataBase);

            tableLoanedCDs.TableModel.Rows.Clear();
            
            foreach (LoanedCDDataSet.LoanedCDRow loanedCDRow in dt)
            {
                string artist;
                string title;

                CDDataSet.CDDataTable cd = cdta.GetDataById(loanedCDRow.CDID);
                if (cd.Count == 0)      // CD wurde wohl gelöscht
                {
                    artist = "<" + StringTable.Deleted + ">";
                    title = "<" + StringTable.Deleted + ">";
                }
                else
                {
                    PersonGroupDataSet.PersonGroupDataTable personGroup = personGroupta.GetDataById(cd[0].ArtistID);

                    artist = personGroup[0].Name;
                    title = cd[0].Title;
                }

                XPTable.Models.Row newRow = new XPTable.Models.Row();
                newRow.Cells.Add(new XPTable.Models.Cell(artist));
                newRow.Cells.Add(new XPTable.Models.Cell(title));
                newRow.Cells.Add(new XPTable.Models.Cell(loanedCDRow.LoanedTo));
                newRow.Cells.Add(new XPTable.Models.Cell(loanedCDRow.LoanedDate.ToShortDateString()));
                if (!loanedCDRow.IsReturnDateNull())
                {
                    if (loanedCDRow.ReturnDate <= DateTime.Now)
                    {
                        newRow.ForeColor = Color.Red;
                    }

                    newRow.Cells.Add(new XPTable.Models.Cell(loanedCDRow.ReturnDate.ToShortDateString()));
                }
                else
                {
                    newRow.Cells.Add(new XPTable.Models.Cell(""));
                }
                newRow.Cells.Add(new XPTable.Models.Cell(loanedCDRow.Comment));

                newRow.Tag = loanedCDRow;

                tableLoanedCDs.TableModel.Rows.Add(newRow);
            }

            UpdateWindowState();
        }

        private void buttonProperties_Click(object sender, EventArgs e)
        {
            if (tableLoanedCDs.SelectedItems.Length > 0)
            {
                LoanedCDDataSet.LoanedCDRow loanedCDRow = (LoanedCDDataSet.LoanedCDRow)tableLoanedCDs.SelectedItems[0].Tag;

                FormLoanProperties formLoanProperties = new FormLoanProperties(dataBase, loanedCDRow);

                if (formLoanProperties.ShowDialog(this) == DialogResult.OK)
                {
                    LoanedCDTableAdapter ta = new LoanedCDTableAdapter(dataBase);

                    ta.Update(loanedCDRow);
                    FillList();
                }
            }
        }

        private void buttonReturn_Click(object sender, EventArgs e)
        {
            if (tableLoanedCDs.SelectedItems.Length > 0)
            {
                LoanedCDDataSet.LoanedCDRow loanedCDRow = (LoanedCDDataSet.LoanedCDRow)tableLoanedCDs.SelectedItems[0].Tag;

                LoanedCDTableAdapter ta = new LoanedCDTableAdapter(dataBase);

                CDTableAdapter cdta = new CDTableAdapter(dataBase);
                PersonGroupTableAdapter personGroupta = new PersonGroupTableAdapter(dataBase);

                string artist = "";
                string title = "";

                try
                {
                    CD cd = dataBase.GetCDById(loanedCDRow.CDID);

                    artist = cd.Artist;
                    title = cd.Title;
                }
                catch
                {
                    artist = "<" + StringTable.Deleted + ">";
                    title = "<" + StringTable.Deleted + ">";
                }
                string msg = String.Format(StringTable.ReturnLoanedCD, artist, title);
                if (MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ta.Delete(loanedCDRow.CDID);
                    FillList();
                }
            }
        }

        private void UpdateWindowState()
        {
            buttonProperties.Enabled = (tableLoanedCDs.SelectedItems.Length > 0);
            buttonReturn.Enabled = (tableLoanedCDs.SelectedItems.Length > 0);
        }

        /// <summary>
        /// Prüft, ob eine CD zurückgegeben werden müsste und zeigt dann den
        /// Dialog an.
        /// </summary>
        /// <param name="db"></param>
        static public void CheckLoanedCDs(DataBase db)
        {
            try
            {
                string sql = "SELECT count(*) from LoanedCD WHERE ReturnDate <= GetDate()";

                object result = db.ExecuteScalar(sql);
                if (result != null)
                {
                    int count = (int)result;
                    if (count > 0)
                    {
                        FormLoanManager formLoanManager = new FormLoanManager(db);

                        formLoanManager.ShowDialog();
                    }
                }
            }
            catch (Exception e)
            {
                Big3.Hitbase.Miscellaneous.FormUnhandledException formEx = new Big3.Hitbase.Miscellaneous.FormUnhandledException(e);
                formEx.ShowDialog();
            }
        }
    }
}
