using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using System.Drawing.Printing;
using Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls;
using Big3.Hitbase.SoundEngine;
using System.IO;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormEditArtists : Form
    {
        PersonGroupTableAdapter personGroupAdapter;
        PersonGroupDataSet personGroup = new PersonGroupDataSet();
        DataBase dataBase = null;
        private PrintDocument printDocument;
        private int currentPrintRecord;
        private int currentPage;
        private int lastSortColumn = 0;
        private SortOrder lastSortOrder = SortOrder.Ascending;

        public FormEditArtists(DataBase db)
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);

            dataBase = db;

            personGroupAdapter = new PersonGroupTableAdapter(db);

            Tools.FormatXPTable(personGroupTable);

            personGroupTable.TableModel = new XPTable.Models.TableModel();
            personGroupTable.ColumnModel = new XPTable.Models.ColumnModel();

            personGroupTable.TableModel.RowHeight += 4;
            personGroupTable.EditStartAction = XPTable.Editors.EditStartAction.SingleClickWhenSelected;

            personGroupTable.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(dataBase.GetNameOfField(Field.ArtistCDName, true), 100));
            personGroupTable.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(dataBase.GetNameOfField(Field.ArtistCDSaveAs, true), 100));

            XPTable.Models.ComboBoxColumn artistTypeColumn = new XPTable.Models.ComboBoxColumn(dataBase.GetNameOfField(Field.ArtistCDType, true), 100);
            artistTypeColumn.ShowDropDownButton = false;
            personGroupTable.ColumnModel.Columns.Add(artistTypeColumn);

            XPTable.Models.ComboBoxColumn artistSexColumn = new XPTable.Models.ComboBoxColumn(dataBase.GetNameOfField(Field.ArtistCDSex, true), 100);
            artistSexColumn.ShowDropDownButton = false;
            personGroupTable.ColumnModel.Columns.Add(artistSexColumn);

            // Herkunftsland
            XPTable.Models.ComboBoxColumn countryColumn = new XPTable.Models.ComboBoxColumn(dataBase.GetNameOfField(Field.ArtistCDCountry, true), 100);
            countryColumn.ShowDropDownButton = false;
            personGroupTable.ColumnModel.Columns.Add(countryColumn);

            // Geboren am
            XPTable.Models.TextColumn dateOfBirthColumn = new XPTable.Models.TextColumn(dataBase.GetNameOfField(Field.ArtistCDDateOfBirth, true), 100);
            dateOfBirthColumn.Comparer = typeof(XPTable.Sorting.NumberComparer);
            personGroupTable.ColumnModel.Columns.Add(dateOfBirthColumn);
            // Gestorben am
            XPTable.Models.TextColumn dateOfDeathColumn = new XPTable.Models.TextColumn(dataBase.GetNameOfField(Field.ArtistCDDateOfDeath, true), 100);
            dateOfDeathColumn.Comparer = typeof(XPTable.Sorting.NumberComparer);
            personGroupTable.ColumnModel.Columns.Add(dateOfDeathColumn);

            // Homepage
            personGroupTable.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(dataBase.GetNameOfField(Field.ArtistCDHomepage, true), 100));

            // Bild
            personGroupTable.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(dataBase.GetNameOfField(Field.ArtistCDImageFilename, true), 100));

            // Kommentar
            personGroupTable.ColumnModel.Columns.Add(new XPTable.Models.TextColumn(dataBase.GetNameOfField(Field.ArtistCDComment, true), 100));

            // Verwendet
            XPTable.Models.TextColumn usedColumn = new XPTable.Models.TextColumn(StringTable.Used, 60);
            usedColumn.Editable = false;
            personGroupTable.ColumnModel.Columns.Add(usedColumn);

            UpdateVisibleState();
        }

        private void UpdateVisibleState()
        {
            if (Big3.Hitbase.Configuration.Settings.Current.PersonGroupListType == 0)
            {
                toolStripButtonDetails.Checked = true;
                personGroupTable.Visible = false;
                personGroupListBox.Visible = true;
            }

            if (Big3.Hitbase.Configuration.Settings.Current.PersonGroupListType == 1)
            {
                toolStripButtonTable.Checked = true;
                personGroupTable.Visible = true;
                personGroupListBox.Visible = false;
                XPTableHelper.XPTableConfiguration tableConfig = XPTableHelper.LoadTableConfiguration(personGroupTable, "ArtistTableConfig");
                if (tableConfig != null)
                {
                    lastSortColumn = tableConfig.SortColumn;
                    lastSortOrder = (SortOrder)tableConfig.SortOrder;
                }
            }
        }

        private void FormEditArtists_Load(object sender, EventArgs e)
        {
            FillArtistList();

            if (Big3.Hitbase.Configuration.Settings.Current.PersonGroupListType == 1)
                personGroupTable.Sort(lastSortColumn, lastSortOrder);

            Configuration.Settings.RestoreWindowPlacement(this, "FormEditArtist");

            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

            UpdateWindowState();
        }

        private void FillArtistList()
        {
            Cursor.Current = Cursors.WaitCursor;

            personGroup.PersonGroup.Clear();
            personGroupAdapter.Fill(personGroup.PersonGroup);

            personGroupListBox.Items.Clear();
            personGroupTable.TableModel.Rows.Clear();

            foreach (PersonGroupDataSet.PersonGroupRow row in personGroup.PersonGroup)
            {
                // Leeren Interpreten nicht anzeigen. Kann schon mal angelegt werden.
                if (string.IsNullOrEmpty(row.Name))
                    continue;

                if (personGroupListBox.Visible)
                {
                    try
                    {
                        personGroupListBox.Items.Add(row);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    XPTable.Models.Row newRow = new XPTable.Models.Row();
                    newRow.Cells.Add(new XPTable.Models.Cell(row.Name));
                    newRow.Cells.Add(new XPTable.Models.Cell(row.SaveAs));
                    newRow.Cells.Add(new XPTable.Models.Cell(DataBase.GetNameOfPersonGroupType((PersonGroupType)(row.IsTypeNull() ? 0 : row.Type))));
                    newRow.Cells.Add(new XPTable.Models.Cell(DataBase.GetNameOfPersonGroupSex((SexType)(row.IsSexNull() ? 0 : row.Sex))));
                    newRow.Cells.Add(new XPTable.Models.Cell(row.Country));

                    string date = row.IsBirthDayNull() ? "" : Misc.FormatDate(row.BirthDay);
                    XPTable.Models.Cell cellBirthDay = new XPTable.Models.Cell(date);
                    if (row.IsBirthDayNull())
                        cellBirthDay.Data = null;
                    else
                        cellBirthDay.Data = Misc.Atoi(row.BirthDay);
                    newRow.Cells.Add(cellBirthDay);

                    date = row.IsDayOfDeathNull() ? "" : Misc.FormatDate(row.DayOfDeath);
                    XPTable.Models.Cell cellDayOfDeath = new XPTable.Models.Cell(date);
                    if (row.IsDayOfDeathNull())
                        cellDayOfDeath.Data = null;
                    else
                        cellDayOfDeath.Data = Misc.Atoi(row.DayOfDeath);
                    newRow.Cells.Add(cellDayOfDeath);

                    newRow.Cells.Add(new XPTable.Models.Cell(row.URL));

                    XPTable.Models.Cell cellImageFilename = new XPTable.Models.Cell(row.IsImageFilenameNull() ? "" : row.ImageFilename);
                    if (row.IsImageFilenameNull())
                        cellImageFilename.Data = null;
                    else
                        cellImageFilename.Data = row.ImageFilename;
                    newRow.Cells.Add(cellImageFilename);
                    
                    newRow.Cells.Add(new XPTable.Models.Cell(row.Comment));
                    newRow.Cells.Add(new XPTable.Models.Cell(""));
                    newRow.Tag = row;
                    personGroupTable.TableModel.Rows.Add(newRow);
                }
            }

            toolStripLabelCount.Text = String.Format(StringTable.NumberOfPersonsGroups, personGroup.PersonGroup.Count);

            Cursor.Current = Cursors.Default;

            if (personGroupListBox.Visible)
            {
                //personGroupListBox.Sorted = false;
                //personGroupListBox.Sorted = true;
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            Edit();

            UpdateWindowState();
        }

        private void Edit()
        {
            PersonGroupDataSet.PersonGroupRow personGroupRow = GetSelectedRow();

            if (personGroupRow == null)
                return;

            PersonGroup personGroup = dataBase.GetPersonGroupById(personGroupRow.PersonGroupID);
            PersonGroupWindow personGroupWindow = new PersonGroupWindow(dataBase, PersonType.Unknown, personGroup);

            personGroupWindow.ChangeAllSoundFiles = true;

            int oldPersonGroupId = personGroup.ID;
            string oldPersonGroup = personGroup.Name;

            if (personGroupWindow.ShowDialog() == true)
            {
                personGroupListBox.ClearImageCache();
                personGroupListBox.Invalidate(false);

            }
        }



        private void toolStripButtonDetails_Click(object sender, EventArgs e)
        {
            personGroupTable.Visible = false;
            personGroupListBox.Visible = true;
            toolStripButtonDetails.Checked = true;
            toolStripButtonTable.Checked = false;

            FillArtistList();

            UpdateWindowState();
        }

        private void toolStripButtonTable_Click(object sender, EventArgs e)
        {
            personGroupTable.Visible = true;
            personGroupListBox.Visible = false;
            toolStripButtonDetails.Checked = false;
            toolStripButtonTable.Checked = true;

            FillArtistList();

            UpdateWindowState();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            Delete();

            UpdateWindowState();
        }

        private void Delete()
        {
            PersonGroupDataSet.PersonGroupRow selPersonGroup = GetSelectedRow();
            int selectedIndex = GetSelectedIndex();

            if (selPersonGroup == null)
                return;

            if (MessageBox.Show(String.Format(StringTable.DeleteArtist, selPersonGroup.Name), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            // Prüfen, ob es noch Tracks oder CDs mit diesem Interpreten gibt.
            bool artistFound = IsPersonGroupUsed(selPersonGroup.PersonGroupID);

            if (artistFound)
            {
                FormDeleteExistingArtist formExisting = new FormDeleteExistingArtist(dataBase, selPersonGroup);

                if (formExisting.ShowDialog() == DialogResult.OK)
                {
                    string personGroupName = (string)formExisting.comboBoxArtists.SelectedItem;

                    PersonGroupDataSet.PersonGroupRow newPersonGroup = dataBase.GetPersonGroupRowByName(personGroupName, false);

                    PersonGroupHelper.UpdateSoundfiles(this.dataBase, selPersonGroup.PersonGroupID, selPersonGroup.Name, personGroupName, delegate
                    {
                        dataBase.ExecuteNonQuery(string.Format("UPDATE CD SET ArtistID={0} WHERE ArtistID={1}", newPersonGroup.PersonGroupID, selPersonGroup.PersonGroupID));
                        dataBase.ExecuteNonQuery(string.Format("UPDATE Track SET ArtistID={0} WHERE ArtistID={1}", newPersonGroup.PersonGroupID, selPersonGroup.PersonGroupID));
                        dataBase.ExecuteNonQuery(string.Format("UPDATE CD SET ComposerID={0} WHERE ComposerID={1}", newPersonGroup.PersonGroupID, selPersonGroup.PersonGroupID));
                        dataBase.ExecuteNonQuery(string.Format("UPDATE Track SET ComposerID={0} WHERE ComposerID={1}", newPersonGroup.PersonGroupID, selPersonGroup.PersonGroupID));

                        dataBase.ExecuteNonQuery(string.Format("UPDATE Participant SET PersonGroupID={0} WHERE PersonGroupID={1}", newPersonGroup.PersonGroupID, selPersonGroup.PersonGroupID));

                        dataBase.ExecuteNonQuery(string.Format("DELETE FROM PersonGroup WHERE PersonGroupID={0}", selPersonGroup.PersonGroupID));

                        personGroupListBox.Items.Remove(selPersonGroup);
                        personGroupTable.TableModel.Rows.RemoveAt(selectedIndex);
                    });
                }
            }
            else
            {
                dataBase.ExecuteNonQuery(string.Format("DELETE FROM PersonGroup WHERE PersonGroupID={0}", selPersonGroup.PersonGroupID));

                personGroupListBox.Items.Remove(selPersonGroup);
                personGroupTable.TableModel.Rows.RemoveAt(selectedIndex);
            }
        }

        private PersonGroupDataSet.PersonGroupRow GetSelectedRow()
        {
            if (personGroupTable.Visible)
            {
                if (personGroupTable.SelectedItems.Length < 1)
                    return null;

                return (PersonGroupDataSet.PersonGroupRow)personGroupTable.SelectedItems[0].Tag;
            }
            else
            {
                if (personGroupListBox.SelectedIndex < 0)
                    return null;

                return (PersonGroupDataSet.PersonGroupRow)personGroupListBox.SelectedItem;
            }
        }

        private int GetSelectedIndex()
        {
            if (personGroupTable.Visible)
            {
                if (personGroupTable.SelectedItems.Length < 1)
                    return -1;

                return personGroupTable.SelectedIndicies[0];
            }
            else
            {
                if (personGroupListBox.SelectedIndex < 0)
                    return -1;

                return personGroupListBox.SelectedIndex;
            }
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            PersonGroup personGroup = new PersonGroup();

            PersonGroupWindow personGroupWindow = new PersonGroupWindow(dataBase, PersonType.Unknown, personGroup);

            if (personGroupWindow.ShowDialog() == true)
            {
                UpdateWindowState();
                FillArtistList();
            }

        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            if (GetSelectedIndex() >= 0)
                CheckArtist(GetSelectedIndex());

            UpdateWindowState();
        }

        private void buttonCheckAll_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            for (int i = 0; i < personGroupTable.TableModel.Rows.Count; i++)
                CheckArtist(i);

            Cursor.Current = Cursors.Default;

            UpdateWindowState();
        }

        private void CheckArtist(int row)
        {
            PersonGroupDataSet.PersonGroupRow personGroupRow = (PersonGroupDataSet.PersonGroupRow)personGroupTable.TableModel.Rows[row].Tag;

            Color textColor;
            if (IsPersonGroupUsed(personGroupRow.PersonGroupID))
            {
                personGroupTable.TableModel.Rows[row].Cells[10].Text = StringTable.Yes;
                textColor = Color.Black;
            }
            else
            {
                personGroupTable.TableModel.Rows[row].Cells[10].Text = StringTable.No;
                textColor = Color.LightGray;
            }

            for (int i = 0; i < personGroupTable.TableModel.Rows[row].Cells.Count; i++)
                personGroupTable.TableModel.Rows[row].Cells[i].ForeColor = textColor;
        }

        /// <summary>
        /// Liefert true zurück, wenn es CDs oder Tracks angegebenen Interpreten gibt. Ansonsten false.
        /// </summary>
        /// <param name="artistID"></param>
        /// <returns></returns>
        private bool IsPersonGroupUsed(int personGroupID)
        {
            int personGroupCount = 0;

            personGroupCount += Convert.ToInt32(dataBase.ExecuteScalar(string.Format("SELECT count(*) FROM Track where ArtistID = {0}", personGroupID)));
            if (personGroupCount > 0)
                return true;
            personGroupCount += Convert.ToInt32(dataBase.ExecuteScalar(string.Format("SELECT count(*) FROM CD where ArtistID = {0}", personGroupID)));
            if (personGroupCount > 0)
                return true;
            personGroupCount += Convert.ToInt32(dataBase.ExecuteScalar(string.Format("SELECT count(*) FROM CD where ComposerID = {0}", personGroupID)));
            if (personGroupCount > 0)
                return true;
            personGroupCount += Convert.ToInt32(dataBase.ExecuteScalar(string.Format("SELECT count(*) FROM Track where ComposerID = {0}", personGroupID)));
            if (personGroupCount > 0)
                return true;

            personGroupCount += Convert.ToInt32(dataBase.ExecuteScalar(string.Format("SELECT count(*) FROM Participant where PersonGroupID = {0}", personGroupID)));
            if (personGroupCount > 0)
                return true;

            return false;
        }

        private void artistTable_DoubleClick(object sender, EventArgs e)
        {
            Edit();
        }

        private void personGroupListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = personGroupListBox.IndexFromPoint(e.Location);

            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                Edit();
            }
        }

        private void artistTable_EditingCancelled(object sender, XPTable.Events.CellEditEventArgs e)
        {
            PersonGroupDataSet.PersonGroupRow personGroupRow = (PersonGroupDataSet.PersonGroupRow)personGroupTable.TableModel.Rows[e.Row].Tag;
            personGroupRow.CancelEdit();
        }

        private void artistTable_EditingStopped(object sender, XPTable.Events.CellEditEventArgs e)
        {
            PersonGroupDataSet.PersonGroupRow personGroupRow = (PersonGroupDataSet.PersonGroupRow)personGroupTable.TableModel.Rows[e.Row].Tag;

            switch (e.Column)
            {
                case 2:
                    personGroupRow.Type = ((XPTable.Editors.ComboBoxCellEditor)e.Editor).SelectedIndex;
                    personGroupRow.EndEdit();
                    personGroupAdapter.Update(personGroupRow);
                    break;
                case 3:
                    personGroupRow.Sex = ((XPTable.Editors.ComboBoxCellEditor)e.Editor).SelectedIndex;
                    personGroupRow.EndEdit();
                    personGroupAdapter.Update(personGroupRow);
                    break;
                case 5:
                    {
                        string newValue = ((XPTable.Editors.TextCellEditor)e.Editor).TextBox.Text;
                        try
                        {
                            if (newValue.Length > 0)
                            {
                                personGroupRow.BirthDay = Misc.ParseDate(newValue);
                                // Das richtig formatierte Datum anzeigen
                                ((XPTable.Editors.TextCellEditor)e.Editor).TextBox.Text = Misc.FormatDate(personGroupRow.BirthDay);
                                e.Cell.Data = personGroupRow.BirthDay;
                            }
                            else
                            {
                                personGroupRow.SetBirthDayNull();
                                e.Cell.Data = null;
                            }
                        }
                        catch
                        {
                            MessageBox.Show(StringTable.InvalidDateValue, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            e.Cancel = true;
                        }
                        break;
                    }

                case 6:
                    {
                        string newValue = ((XPTable.Editors.TextCellEditor)e.Editor).TextBox.Text;
                        try
                        {
                            if (newValue.Length > 0)
                            {
                                personGroupRow.DayOfDeath = Misc.ParseDate(newValue);
                                // Das richtig formatierte Datum anzeigen
                                ((XPTable.Editors.TextCellEditor)e.Editor).TextBox.Text = Misc.FormatDate(personGroupRow.DayOfDeath);
                                e.Cell.Data = personGroupRow.DayOfDeath;
                            }
                            else
                            {
                                personGroupRow.SetDayOfDeathNull();
                                e.Cell.Data = null;
                            }
                        }
                        catch
                        {
                            MessageBox.Show(StringTable.InvalidDateValue, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            e.Cancel = true;
                        }
                        break;
                    }
            }
        }

        private void artistTable_BeginEditing(object sender, XPTable.Events.CellEditEventArgs e)
        {
            switch (e.Column)
            {
                case 2:
                    ((XPTable.Editors.ComboBoxCellEditor)e.Editor).Items.Clear();
                    ((XPTable.Editors.ComboBoxCellEditor)e.Editor).Items.AddRange(DataBase.GetAvailablePersonGroupTypes());
                    break;
                case 3:
                    ((XPTable.Editors.ComboBoxCellEditor)e.Editor).Items.Clear();
                    ((XPTable.Editors.ComboBoxCellEditor)e.Editor).Items.AddRange(DataBase.GetAvailablePersonGroupSex());
                    break;
                case 4:
                    ((XPTable.Editors.ComboBoxCellEditor)e.Editor).Items.Clear();
                    ((XPTable.Editors.ComboBoxCellEditor)e.Editor).DropDownStyle = XPTable.Editors.DropDownStyle.DropDown;
                    List<String> countries = DataBase.GetAvailableCountries(personGroup.PersonGroup);
                    ((XPTable.Editors.ComboBoxCellEditor)e.Editor).Items.AddRange(countries.ToArray());
                    break;
                default:
                    break;
            }

            PersonGroupDataSet.PersonGroupRow personGroupRow = (PersonGroupDataSet.PersonGroupRow)personGroupTable.TableModel.Rows[e.Row].Tag;
            personGroupRow.BeginEdit();
        }

        private void artistTable_CellPropertyChanged(object sender, XPTable.Events.CellEventArgs e)
        {
            try
            {
                PersonGroupDataSet.PersonGroupRow personGroupRow = (PersonGroupDataSet.PersonGroupRow)personGroupTable.TableModel.Rows[e.Row].Tag;

                switch (e.Column)
                {
                    case 0:
                        personGroupRow.Name = e.Cell.Text;
                        break;
                    case 1:
                        personGroupRow.SaveAs = e.Cell.Text;
                        break;
                    case 4:
                        personGroupRow.Country = e.Cell.Text;
                        break;
                    case 7:
                        personGroupRow.URL = e.Cell.Text;
                        break;
                    case 8:
                        personGroupRow.ImageFilename = e.Cell.Text;
                        break;
                    case 9:
                        personGroupRow.Comment = e.Cell.Text;
                        break;
                }

                personGroupRow.EndEdit();
                personGroupAdapter.Update(personGroupRow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            FillArtistList();

            UpdateWindowState();
        }

        private void artistTable_SelectionChanged(object sender, XPTable.Events.SelectionEventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            if (personGroupTable.Visible)
            {
                buttonEdit.Enabled = (personGroupTable.SelectedItems.Length > 0);
                buttonDelete.Enabled = (personGroupTable.SelectedItems.Length > 0);
                buttonCheck.Enabled = (personGroupTable.SelectedItems.Length > 0);
                buttonCheckAll.Enabled = true;
            }
            else
            {
                buttonEdit.Enabled = (personGroupListBox.SelectedItems.Count > 0);
                buttonDelete.Enabled = (personGroupListBox.SelectedItems.Count > 0);
                buttonCheck.Enabled = false;
                buttonCheckAll.Enabled = false;
            }
        }

        private void toolStripButtonPrint_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            printDialog.PrinterSettings = Big3.Hitbase.Configuration.Settings.Current.GlobalPrinterSettings;

            printDialog.Document = printDocument;
            printDialog.AllowSomePages = true;
            currentPrintRecord = 0;
            currentPage = 1;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Big3.Hitbase.Configuration.Settings.Current.GlobalPrinterSettings = printDialog.PrinterSettings;
                    printDocument.PrinterSettings = printDialog.PrinterSettings;
                    printDocument.Print();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void toolStripButtonPrintPreview_Click(object sender, EventArgs e)
        {
            PrintPreviewDialog previewDialog = new PrintPreviewDialog();

            previewDialog.Document = printDocument;

            currentPrintRecord = 0;
            currentPage = 1;

            previewDialog.ShowDialog();
        }

        private Font printFont = new Font("Arial", 8, FontStyle.Regular);
        private Font printFontBold = new Font("Arial", 8, FontStyle.Bold);
        private Font printFontHeader = new Font("Arial", 12, FontStyle.Bold);

        void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Zuerst die Gesamtbreite aller Spalten ermitteln
            int totalWidth = 0;
            foreach (XPTable.Models.Column column in personGroupTable.ColumnModel.Columns)
            {
                if (column.Visible)
                    totalWidth += column.Width;
            }

            // Jetzt den Prozentanteil jeder einzelnen Spalte errechnen und auf die Seite hochrechnen
            int[] columnWidth = new int[personGroupTable.ColumnModel.Columns.Count];
            int nr = 0;
            foreach (XPTable.Models.Column column in personGroupTable.ColumnModel.Columns)
            {
                if (column.Visible)
                {
                    columnWidth[nr] = (int)((double)e.MarginBounds.Width / (double)totalWidth * (double)column.Width);
                    nr++;
                }
            }

            bool printThisPage = (e.PageSettings.PrinterSettings.PrintRange == PrintRange.AllPages ||
                currentPage >= e.PageSettings.PrinterSettings.FromPage &&
                currentPage <= e.PageSettings.PrinterSettings.ToPage);

            int yPosition = e.MarginBounds.Top;
            // Spaltenüberschrift drucken
            if (printThisPage)
                PrintHeader(e.Graphics, e.MarginBounds, columnWidth, personGroupTable, ref yPosition);

            int startYPosition = yPosition;

            if (personGroupTable.Visible)
            {
                for (int i = currentPrintRecord; i < personGroupTable.TableModel.Rows.Count; i++)
                {
                    if (yPosition + printFont.Height > e.MarginBounds.Bottom)
                    {
                        currentPrintRecord = i;
                        yPosition = e.MarginBounds.Top;

                        if (e.PageSettings.PrinterSettings.PrintRange == PrintRange.SomePages &&
                            currentPage >= e.PageSettings.PrinterSettings.ToPage)
                        {
                            e.HasMorePages = false;
                            break;
                        }

                        if (printThisPage)
                        {
                            currentPage++;
                            e.HasMorePages = true;
                            break;
                        }
                        
                        currentPage++;

                        printThisPage = (e.PageSettings.PrinterSettings.PrintRange == PrintRange.AllPages ||
                            currentPage >= e.PageSettings.PrinterSettings.FromPage &&
                            currentPage <= e.PageSettings.PrinterSettings.ToPage);

                        if (printThisPage)
                            PrintHeader(e.Graphics, e.MarginBounds, columnWidth, personGroupTable, ref yPosition);
                    }

                    if (printThisPage)
                    {
                        int colNr = 0;
                        int xPosition = e.MarginBounds.Left;
                        foreach (XPTable.Models.Cell cell in personGroupTable.TableModel.Rows[i].Cells)
                        {
                            if (personGroupTable.ColumnModel.Columns[cell.Index].Visible)
                            {
                                //e.Graphics.DrawString(cell.Text, printFont, Brushes.Black, new PointF(xPosition, yPosition));
                                StringFormat sf = new StringFormat();
                                sf.Trimming = StringTrimming.EllipsisCharacter;

                                e.Graphics.DrawString(cell.Text, printFont, Brushes.Black, new RectangleF(xPosition, yPosition, columnWidth[colNr], printFont.Height), sf);
                                xPosition += columnWidth[colNr];
                                colNr++;
                            }
                        }
                    }

                    yPosition += printFont.Height;
                }
            }
            else
            {
                int column = 0;

                for (int i = currentPrintRecord; i < personGroup.PersonGroup.Count; i++)
                {
                    int xPosition;

                    PersonGroupDataSet.PersonGroupRow personGroupRow = personGroup.PersonGroup[i];

                    if (yPosition + printFont.Height * 7 > e.MarginBounds.Bottom)
                    {
                        if (column < 1)
                        {
                            column++;
                            yPosition = startYPosition;
                        }
                        else
                        {
                            currentPrintRecord = i;
                            yPosition = e.MarginBounds.Top;

                            if (e.PageSettings.PrinterSettings.PrintRange == PrintRange.SomePages &&
                                currentPage >= e.PageSettings.PrinterSettings.ToPage)
                            {
                                e.HasMorePages = false;
                                break;
                            }

                            if (printThisPage)
                            {
                                currentPage++;
                                e.HasMorePages = true;
                                break;
                            }

                            currentPage++;

                            printThisPage = (e.PageSettings.PrinterSettings.PrintRange == PrintRange.AllPages ||
                                currentPage >= e.PageSettings.PrinterSettings.FromPage &&
                                currentPage <= e.PageSettings.PrinterSettings.ToPage);

                            if (printThisPage)
                                PrintHeader(e.Graphics, e.MarginBounds, columnWidth, personGroupTable, ref yPosition);
                            column = 0;
                        }
                    }

                    if (column == 0)
                        xPosition = e.MarginBounds.Left;
                    else
                        xPosition = e.MarginBounds.Left + e.MarginBounds.Width / 2;

                    int imageWidth = printFont.Height * 7;

                    if (printThisPage)
                    {
                        if (!personGroupRow.IsImageFilenameNull() && personGroupRow.ImageFilename.Length > 0)
                        {
                            try
                            {
                                Image img = Image.FromFile(Misc.FindCover(personGroupRow.ImageFilename));

                                SizeF imageBoundingBox = new SizeF(imageWidth, imageWidth);
                                SizeF bestSize = GetBestFitSize(img, imageBoundingBox);
                                RectangleF imageRect = new RectangleF(
                                    new PointF(
                                        xPosition + (imageBoundingBox.Width - bestSize.Width) / 2,
                                        yPosition + (imageBoundingBox.Height - bestSize.Height) / 2),
                                    bestSize);
                                e.Graphics.DrawImage(img, imageRect);
                            }
                            catch       // Fehler ignorieren
                            {
                            }
                        }
                    }

                    xPosition += imageWidth + 20;

                    StringFormat sf = new StringFormat();
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    int artistWidth = e.MarginBounds.Width / 2 - imageWidth - 30;
                    Rectangle artistRect = new Rectangle(xPosition, yPosition, artistWidth, printFont.Height);
                    if (printThisPage)
                        e.Graphics.DrawString(personGroupRow.Name, printFontBold, Brushes.Black, artistRect, sf);

                    string artistType = DataBase.GetNameOfPersonGroupType((PersonGroupType)(personGroupRow.IsTypeNull() ? 0 : personGroupRow.Type));
                    string artistSex = DataBase.GetNameOfPersonGroupSex((SexType)(personGroupRow.IsSexNull() ? 0 : personGroupRow.Sex));

                    int yOffset = (int)((double)printFontBold.Height * 1.5);

                    if (printThisPage)
                        e.Graphics.DrawString(artistType + ", " + artistSex, printFont, Brushes.Black, new PointF(xPosition, yPosition + yOffset));

                    yOffset += printFont.Height;

                    if (!personGroupRow.IsCountryNull() && personGroupRow.Country.Length > 0)
                    {
                        if (printThisPage)
                            e.Graphics.DrawString(StringTable.Country + ": " + personGroupRow.Country, printFont, Brushes.Black, new PointF(xPosition, yPosition + yOffset));
                        yOffset += printFont.Height;
                    }

                    if (!personGroupRow.IsBirthDayNull())
                    {
                        if (printThisPage)
                        {
                            if ((PersonGroupType)personGroupRow.Type == PersonGroupType.Single)
                                e.Graphics.DrawString(StringTable.Born + ": " + personGroupRow.BirthDay, printFont, Brushes.Black, new PointF(xPosition, yPosition + yOffset));
                            else
                                e.Graphics.DrawString(StringTable.Founded + ": " + personGroupRow.BirthDay, printFont, Brushes.Black, new PointF(xPosition, yPosition + yOffset));
                        }
                        yOffset += printFont.Height;
                    }

                    if (!personGroupRow.IsDayOfDeathNull())
                    {
                        if (printThisPage)
                        {
                            if ((PersonGroupType)personGroupRow.Type == PersonGroupType.Single)
                                e.Graphics.DrawString(StringTable.Died + ": " + personGroupRow.DayOfDeath, printFont, Brushes.Black, new PointF(xPosition, yPosition + yOffset));
                            else
                                e.Graphics.DrawString(StringTable.BreakAway + ": " + personGroupRow.DayOfDeath, printFont, Brushes.Black, new PointF(xPosition, yPosition + yOffset));
                        }
                        yOffset += printFont.Height;
                    }

                    if (!personGroupRow.IsURLNull())
                    {
                        if (printThisPage)
                            e.Graphics.DrawString(personGroupRow.URL, printFont, Brushes.Black, new PointF(xPosition, yPosition + yOffset));
                        yOffset += printFont.Height;
                    }

                    yPosition += printFont.Height * 8;
                }
            }
        }

        void PrintHeader(Graphics g, Rectangle bounds, int[] columnWidth, XPTable.Models.Table table, ref int yPosition)
        {
            int nr = 0;
            int xPosition = bounds.Left;

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            g.DrawString(StringTable.PersonsGroups, printFontHeader, Brushes.Black, new RectangleF(xPosition, yPosition, bounds.Width, printFontHeader.Height), sf);

            sf.Alignment = StringAlignment.Far;         // Seitennummer, rechtsbündig
            sf.LineAlignment = StringAlignment.Far;         
            g.DrawString(string.Format("{0} {1}", StringTable.Page, currentPage), printFont, Brushes.Black, new RectangleF(xPosition, yPosition, bounds.Width, printFontHeader.Height), sf);

            sf.Alignment = StringAlignment.Near;         // Datum, linkgsbündig
            g.DrawString(DateTime.Now.ToShortDateString(), printFont, Brushes.Black, new RectangleF(xPosition, yPosition, bounds.Width, printFontHeader.Height), sf);

            yPosition += printFontHeader.Height + printFont.Height;

            if (personGroupTable.Visible)
            {
                foreach (XPTable.Models.Column column in personGroupTable.ColumnModel.Columns)
                {
                    if (column.Visible)
                    {
                        g.DrawString(column.Text, printFont, Brushes.Black, new PointF(xPosition, yPosition));
                        xPosition += columnWidth[nr];
                        nr++;
                    }
                }

                yPosition += printFont.Height;

                g.DrawLine(Pens.Black, bounds.Left, yPosition, bounds.Right, yPosition);
            }
        }

        private void toolStripButtonPageSetup_Click(object sender, EventArgs e)
        {
            PageSetupDialog pageSetup = new PageSetupDialog();
            pageSetup.Document = printDocument;
            PageSettings pageSettings = new PageSettings(printDocument.PrinterSettings);
            pageSetup.PrinterSettings = printDocument.PrinterSettings;
            pageSetup.PageSettings = pageSettings;
            pageSetup.ShowDialog();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                FillArtistList();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    
        /// <summary>
        /// Liefert die optimale Größe zurück, wenn der Aspect-Ratio beibehalten werden soll.
        /// </summary>
        /// <param name="origSize"></param>
        /// <returns></returns>
        private SizeF GetBestFitSize(Image img, SizeF boundingSize)
        {
            SizeF bestFitSize = new SizeF();
            double ratio = (double)img.Width / (double)img.Height;

            if (img.Width > img.Height)
            {
                bestFitSize.Width = boundingSize.Width;
                bestFitSize.Height = (int)((double)boundingSize.Width / ratio);
            }
            else
            {
                bestFitSize.Height = boundingSize.Height;
                bestFitSize.Width = (int)((double)boundingSize.Height * ratio);
            }

            return bestFitSize;
        }

        private void FormEditArtists_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void artistListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormEditArtists_FormClosed(object sender, FormClosedEventArgs e)
        {
            Big3.Hitbase.Configuration.Settings.Current.PersonGroupListType = (toolStripButtonDetails.Checked ? 0 : 1);
            XPTableHelper.SaveTableConfiguration(personGroupTable, "ArtistTableConfig");

            Configuration.Settings.SaveWindowPlacement(this, "FormEditArtist");
        }
    }

}
