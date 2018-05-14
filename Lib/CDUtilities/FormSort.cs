using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormSort : Form
    {
        public class ComboBoxItem
        {
            public Field Field;
            private DataBase dataBase;

            public ComboBoxItem(DataBase dataBase, Field field)
            {
                this.dataBase = dataBase;
                this.Field = field;
            }

            public override string ToString()
            {
                if (Field == Field.None)
                    return "<" + StringTable.None + ">";
                else
                    return dataBase.GetNameOfField(Field);
            }
        }

        public FormSort(DataBase dataBase, FieldType fieldType, SortFieldCollection sortFields)
        {
            InitializeComponent();

            FillList(dataBase, comboBoxSortField1, fieldType, sortFields.Count > 0 ? sortFields[0].Field : Field.None);
            if (sortFields.Count > 0 && sortFields[0].SortDirection == SortDirection.Descending)
                radioButtonSortDirection1Descending.Checked = true;
            else
                radioButtonSortDirection1Ascending.Checked = true;

            FillList(dataBase, comboBoxSortField2, fieldType, sortFields.Count > 1 ? sortFields[1].Field : Field.None);
            if (sortFields.Count > 1 && sortFields[1].SortDirection == SortDirection.Descending)
                radioButtonSortDirection2Descending.Checked = true;
            else
                radioButtonSortDirection2Ascending.Checked = true;

            FillList(dataBase, comboBoxSortField3, fieldType, sortFields.Count > 2 ? sortFields[2].Field : Field.None);
            if (sortFields.Count > 2 && sortFields[2].SortDirection == SortDirection.Descending)
                radioButtonSortDirection3Descending.Checked = true;
            else
                radioButtonSortDirection3Ascending.Checked = true;
        }

        private void FillList(DataBase dataBase, ComboBox comboBox, FieldType fieldType, Field selectedField)
        {
            FieldCollection fields = null;

            switch (fieldType)
            {
                case FieldType.CD:
                    fields = FieldHelper.GetAllCDFields(false);
                    break;
                case FieldType.Track:
                    fields = FieldHelper.GetAllTrackFields(false);
                    break;
                case FieldType.All:
                    fields = FieldHelper.GetAllFields();
                    break;
            }

            if (fields == null)
                throw new Exception("unknown field type!");

            comboBox.Items.Add(new ComboBoxItem(dataBase, Field.None));

            foreach (Field field in fields)
            {
                int index = comboBox.Items.Add(new ComboBoxItem(dataBase, field));
                if (field == selectedField)
                    comboBox.SelectedIndex = index;
            }

            if (comboBox.SelectedIndex < 0)
                comboBox.SelectedIndex = 0;
        }

        public SortFieldCollection SortFields
        {
            get
            {
                SortFieldCollection sortFields = new SortFieldCollection();
                if (comboBoxSortField1.SelectedItem != null && ((ComboBoxItem)comboBoxSortField1.SelectedItem).Field != Field.None)
                {
                    SortDirection sortDirection = radioButtonSortDirection1Ascending.Checked ? SortDirection.Ascending: SortDirection.Descending;
                    sortFields.Add(new SortField(((ComboBoxItem)comboBoxSortField1.SelectedItem).Field, sortDirection));
                }
                if (comboBoxSortField2.SelectedItem != null && ((ComboBoxItem)comboBoxSortField2.SelectedItem).Field != Field.None)
                {
                    SortDirection sortDirection = radioButtonSortDirection2Ascending.Checked ? SortDirection.Ascending: SortDirection.Descending;
                    sortFields.Add(new SortField(((ComboBoxItem)comboBoxSortField2.SelectedItem).Field, sortDirection));
                }
                if (comboBoxSortField3.SelectedItem != null && ((ComboBoxItem)comboBoxSortField3.SelectedItem).Field != Field.None)
                {
                    SortDirection sortDirection = radioButtonSortDirection3Ascending.Checked ? SortDirection.Ascending: SortDirection.Descending;
                    sortFields.Add(new SortField(((ComboBoxItem)comboBoxSortField3.SelectedItem).Field, sortDirection));
                }

                return sortFields;
            }
        }
    }
}
