using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.CDUtilities;
using Big3.Hitbase.DataBaseEngine;
using XPTable.Models;

namespace Big3.Hitbase.MainWindowDesigner.Controls
{
    public partial class ParticipantsListUserControl : UserControl
    {
        private DataBase dataBase;

        public DataBase DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }

        public ParticipantsListUserControl()
        {
            InitializeComponent();

            UpdateWindowState();

            Tools.FormatXPTable(tableParticipants);

            XPTable.Models.Column newColumn = new XPTable.Models.TextColumn();

            tableParticipants.TableModel = new XPTable.Models.TableModel();
            tableParticipants.TableModel.RowHeight = 20;
            XPTable.Models.ColumnModel model = new XPTable.Models.ColumnModel();

            model.Columns.Add(new XPTable.Models.TextColumn(StringTable.Role, 100));
            model.Columns.Add(new XPTable.Models.TextColumn(StringTable.Name, 150));
            model.Columns.Add(new XPTable.Models.NumberColumn(StringTable.TrackNumber, 50));
            model.Columns.Add(new XPTable.Models.TextColumn(StringTable.Comment, 150));

            model.Columns[0].Editable = false;
            model.Columns[1].Editable = false;
            model.Columns[2].Editable = false;
            model.Columns[3].Editable = false;

            tableParticipants.ColumnModel = model;
        }

        public ParticipantsListUserControl(DataBase db) : this()
        {
            dataBase = db;
        }

        public bool ShowTitle
        {
            get
            {
                return labelTitle.Visible;
            }
            set
            {
                labelTitle.Visible = value;
                if (value == false)
                {
                    tableParticipants.Top = 2;
                    tableParticipants.Height += 12;
                }
            }
        }


        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Participant participant = new Participant();
            FormEditParticipant formNewParticipant = new FormEditParticipant(dataBase, participant, false);

            if (formNewParticipant.ShowDialog(this) == DialogResult.OK)
            {
                Cell[] items = new Cell[4];
                items[0] = new Cell(participant.Role);
                items[1] = new Cell(participant.Name);
                if (participant.TrackNumber > 0)
                    items[2] = new Cell(participant.TrackNumber);
                else
                    items[2] = new Cell(null);
                items[3] = new Cell(participant.Comment);

                Row row = new Row(items);
                tableParticipants.TableModel.Rows.Add(row);
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            Edit();
        }

        private void Edit()
        {
            Row selectedRow = tableParticipants.TableModel.Rows[tableParticipants.SelectedIndicies[0]];
            Participant participant = new Participant();
            participant.Role = selectedRow.Cells[0].Text;
            participant.Name = selectedRow.Cells[1].Text;

            try
            {
                if (selectedRow.Cells[2].Data != null)
                    participant.TrackNumber = (int)selectedRow.Cells[2].Data;
            }
            catch
            {
            }

            participant.Comment = selectedRow.Cells[3].Text;
            FormEditParticipant formNewParticipant = new FormEditParticipant(dataBase, participant, true);

            if (formNewParticipant.ShowDialog(this) == DialogResult.OK)
            {
                selectedRow.Cells[0].Text = participant.Role;
                selectedRow.Cells[1].Text = participant.Name;
                if (participant.TrackNumber > 0)
                    selectedRow.Cells[2].Data = participant.TrackNumber;
                else
                    selectedRow.Cells[2].Data = null;
                selectedRow.Cells[3].Text = participant.Comment;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (tableParticipants.SelectedItems.Length < 1)
                return;

            tableParticipants.TableModel.Rows.RemoveAt(tableParticipants.SelectedIndicies[0]);
        }

        private void tableParticipants_SelectionChanged(object sender, XPTable.Events.SelectionEventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonEdit.Enabled = tableParticipants.SelectedItems.Length > 0;
            buttonDelete.Enabled = tableParticipants.SelectedItems.Length > 0;
        }

        private void tableParticipants_CellDoubleClick(object sender, XPTable.Events.CellMouseEventArgs e)
        {
            Edit();
        }

        public void Fill(ParticipantList participants)
        {
            if (participants == null)
                return;

            tableParticipants.TableModel.Rows.Clear();

            foreach (Participant p in participants)
            {
                Cell[] cells = new Cell[4];
                //string[] cells = new string[4];
                cells[0] = new Cell(p.Role);
                cells[1] = new Cell(p.Name);
                if (p.TrackNumber > 0)
                    cells[2] = new Cell(p.TrackNumber);
                else
                    cells[2] = new Cell("");
                cells[3] = new Cell(p.Comment);
                XPTable.Models.Row row = new XPTable.Models.Row(cells);
                tableParticipants.TableModel.Rows.Add(row);
            }
        }

        public ParticipantList GetData()
        {
            ParticipantList participants = new ParticipantList();
            int i = 0;
            foreach (XPTable.Models.Row row in tableParticipants.TableModel.Rows)
            {
                Participant newParticipant = new Participant();
                newParticipant.Role = row.Cells[0].Text;
                newParticipant.Name = row.Cells[1].Text;
                if (row.Cells[2].Data != null)
                {
                    try { newParticipant.TrackNumber = (int)row.Cells[2].Data; }
                    catch { }
                }
                newParticipant.Comment = row.Cells[3].Text;
                participants.Add(newParticipant);
                i++;
            }

            return participants;
        }
    }
}
