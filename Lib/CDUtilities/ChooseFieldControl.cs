using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.CDUtilities
{
    public partial class ChooseFieldControl : UserControl
    {
        public delegate void SelectionChangedDelegate();
        public event SelectionChangedDelegate SelectionChanged;

        private DataBase dataBase;
        private FieldType fieldType;

        private FieldCollection currentFields;

        public FieldType FieldType
        {
            get { return fieldType; }
            set { fieldType = value; }
        }

        public ChooseFieldControl()
        {
            InitializeComponent();
        }

        public void Init(DataBase db, FieldType ft, FieldCollection fields)
        {
            dataBase = db;
            fieldType = ft;
            currentFields = fields;

            if (fields != null)
            {
                foreach (Field field in fields)
                {
                    ListViewItem newItem = listViewFields.Items.Add(dataBase.GetNameOfField(field));
                    newItem.Tag = field;
                    newItem.Checked = true;
                }
            }

            FillList();

            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonMoveDown.Enabled = listViewFields.SelectedItems.Count > 0;
            buttonMoveUp.Enabled = listViewFields.SelectedItems.Count > 0;
        }

        private void FillList()
        {
            FieldCollection fc = FieldHelper.GetAllFields();
            fc.SortFields(dataBase);

            foreach (Field field in fc)
            {
                if (field != Field.None && currentFields.IndexOf(field) < 0 && !FieldHelper.IsInternalField(field))
                {
                    ListViewItem newItem = null;
                    switch (fieldType)
                    {
                        case FieldType.CD:
                            if (FieldHelper.IsCDField(field))
                                newItem = listViewFields.Items.Add(dataBase.GetNameOfField(field));
                            break;
                        case FieldType.Track:
                            if (FieldHelper.IsTrackField(field))
                                newItem = listViewFields.Items.Add(dataBase.GetNameOfField(field));
                            break;
                        case FieldType.TrackMain:
                            if (FieldHelper.IsTrackField(field, true))
                                newItem = listViewFields.Items.Add(dataBase.GetNameOfField(field));
                            break;
                        default:
                            newItem = listViewFields.Items.Add(dataBase.GetNameOfFieldFull(field));
                            break;
                    }

                    if (newItem != null)
                        newItem.Tag = field;
                }
            }
        }

        public FieldCollection SelectedFields
        {
            get
            {
                FieldCollection fields = new FieldCollection();
                foreach (ListViewItem lvField in listViewFields.Items)
                {
                    if (lvField.Checked)
                    {
                        fields.Add((Field)lvField.Tag);
                    }
                }

                return fields;
            }
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            MoveSelectionUp();
        }

        private void MoveSelectionUp()
        {
            listViewFields.BeginUpdate();
            ListView.SelectedIndexCollection items = listViewFields.SelectedIndices;
            int[] selIndices = new int[items.Count];
            items.CopyTo(selIndices, 0);
            for (int i = 0; i < selIndices.Length; i++)
            {
                if (selIndices[i] > 0)
                {
                    int index = selIndices[i];
                    ListViewItem lvField = listViewFields.Items[index];
                    listViewFields.Items.Remove(lvField);
                    listViewFields.Items.Insert(index - 1, lvField);
                    listViewFields.Items[index-1].Selected = true;
                }
            }

            if (SelectionChanged != null)
                SelectionChanged();
            listViewFields.EndUpdate();
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            MoveSelectionDown();
        }

        private void MoveSelectionDown()
        {
            listViewFields.BeginUpdate();
            ListView.SelectedIndexCollection items = listViewFields.SelectedIndices;
            int[] selIndices = new int[items.Count];
            items.CopyTo(selIndices, 0);
            for (int i = 0; i < selIndices.Length; i++)
            {
                if (selIndices[i] < listViewFields.Items.Count - 1)
                {
                    int index = selIndices[i];
                    ListViewItem lvField = (ListViewItem)listViewFields.Items[index];
                    listViewFields.Items.Remove(lvField);
                    listViewFields.Items.Insert(index + 1, lvField);
                    listViewFields.Items[index + 1].Selected = true;
                }
            }

            if (SelectionChanged != null)
                SelectionChanged();

            listViewFields.EndUpdate();
        }

        private void listViewFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }
    }
}
