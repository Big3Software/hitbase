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
    public partial class ChooseColumnFieldControl : UserControl
    {
        public delegate void SelectionChangedDelegate();
        public event SelectionChangedDelegate SelectionChanged;

        private DataBase dataBase;
        private FieldType fieldType;

        private ColumnFieldCollection currentFields;
        private ColumnFieldCollection defaultFields;

        public FieldType FieldType
        {
            get { return fieldType; }
            set { fieldType = value; }
        }

        public ChooseColumnFieldControl()
        {
            InitializeComponent();
        }

        public void Init(DataBase db, FieldType ft, ColumnFieldCollection fields, ColumnFieldCollection defaultFields = null)
        {
            dataBase = db;
            fieldType = ft;
            currentFields = fields;
            this.defaultFields = defaultFields;

            buttonDefault.Visible = defaultFields != null;

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
            listViewFields.Items.Clear();

            if (currentFields != null)
            {
                foreach (ColumnField field in currentFields)
                {
                    ListViewItem newItem;

                    if (fieldType == DataBaseEngine.FieldType.TrackAndCD)
                        newItem = listViewFields.Items.Add(dataBase.GetNameOfFieldFull(field.Field));
                    else
                        newItem = listViewFields.Items.Add(dataBase.GetNameOfField(field.Field));

                    newItem.SubItems.Add(field.Width.ToString());
                    newItem.Tag = field;
                    newItem.Checked = true;
                }
            }

            foreach (Field field in FieldHelper.GetAllFields())
            {
                if (field != Field.None && currentFields.GetColumnField(field) == null && !FieldHelper.IsInternalField(field))
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
                    {
                        ColumnField cf = new ColumnField(field);
                        newItem.Tag = cf;
                        newItem.SubItems.Add(cf.Width.ToString());
                    }
                }
            }
        }

        public ColumnFieldCollection SelectedFields
        {
            get
            {
                ColumnFieldCollection fields = new ColumnFieldCollection();
                foreach (ListViewItem lvField in listViewFields.Items)
                {
                    if (lvField.Checked)
                    {
                        fields.Add((ColumnField)lvField.Tag);
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
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            MoveSelectionDown();
        }

        private void MoveSelectionDown()
        {
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
        }

        private void listViewFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();

            if (listViewFields.SelectedItems.Count > 0)
            {
                ColumnField columnField = (ColumnField)listViewFields.SelectedItems[0].Tag;
                textBoxWidth.Text = columnField.Width.ToString();
            }
        }

        private void textBoxWidth_TextChanged(object sender, EventArgs e)
        {
            if (listViewFields.SelectedItems.Count > 0)
            {
                ColumnField columnField = (ColumnField)listViewFields.SelectedItems[0].Tag;

                int width = 0;
                if (Int32.TryParse(textBoxWidth.Text, out width))
                    columnField.Width = width;

                listViewFields.SelectedItems[0].SubItems[1].Text = width.ToString();
            }
        }

        private void buttonDefault_Click(object sender, EventArgs e)
        {
            currentFields.Clear();

            currentFields.AddRange(defaultFields);

            FillList();
            UpdateWindowState();
        }
    }
}
