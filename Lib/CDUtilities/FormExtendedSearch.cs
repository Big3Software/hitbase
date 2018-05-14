using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormExtendedSearch : Form
    {
        public class FieldItem
        {
            public Field Field;
            private DataBase dataBase;

            public FieldItem(DataBase db, Field f)
            {
                dataBase = db;
                Field = f;
            }

            public override string ToString()
            {
                return dataBase.GetNameOfField(Field);
            }
        }

        public class OperatorItem
        {
            public Operator Operator;

            public OperatorItem(Operator o)
            {
                Operator = o;
            }

            public override string ToString()
            {
                return Condition.GetNameOfOperator(Operator);
            }
        }

        private DataBase dataBase;

        private Condition condition = new Condition();
        public Condition Condition
        {
            get
            {
                return condition;
            }
            set
            {
                condition = value;
            }
        }

        public FormExtendedSearch(DataBase db, Condition condition, bool trackFields)
        {
            InitializeComponent();

            if (condition == null)
                this.condition = new Condition();
            else
                this.condition = (Condition)Misc.CopyObject(condition);
            dataBase = db;

            FieldCollection fields;
            
            if (trackFields)
                fields = FieldHelper.GetAllFields();
            else
                fields = FieldHelper.GetAllCDFields(false);

            foreach (Field field in fields)
            {
                comboBoxField.Items.Add(new FieldItem(dataBase, field));
            }

            Operator[] operators = Condition.GetAllOperators();
            foreach (Operator op in operators)
            {
                comboBoxOperator.Items.Add(new OperatorItem(op));
            }

            FillList();
        }

        private void UpdateWindowState()
        {
            buttonAdd.Enabled = (comboBoxField.SelectedItem != null && comboBoxOperator.SelectedItem != null);
            buttonDelete.Enabled = (listViewCondition.SelectedItems.Count > 0);
        }

        private void FillList()
        {
            listViewCondition.Items.Clear();

            foreach (SingleCondition cond in Condition)
            {
                string fieldName = dataBase.GetNameOfField(cond.Field);
                string opName = Condition.GetNameOfOperator(cond.Operator);
                ListViewItem newItem = listViewCondition.Items.Add(fieldName);
                newItem.SubItems.Add(opName);
                newItem.SubItems.Add(cond.Value.ToString());
                newItem.Tag = cond;
            }

            UpdateWindowState();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listViewCondition.SelectedItems.Count < 1)
                return;

            int selIndex = listViewCondition.SelectedIndices[0];
            SingleCondition cond = (SingleCondition)listViewCondition.SelectedItems[0].Tag;

            Condition.Remove(cond);

            listViewCondition.Items[selIndex].Remove();

            if (selIndex < listViewCondition.Items.Count)
                listViewCondition.Items[selIndex].Selected = true;

            UpdateWindowState();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (comboBoxField.SelectedItem == null || comboBoxOperator.SelectedItem == null)
                return;

            FieldItem selectedField = (FieldItem)comboBoxField.SelectedItem;
            OperatorItem selectedOperator = (OperatorItem)comboBoxOperator.SelectedItem;
            string value = textBoxValue.Text;

            Condition.Add(selectedField.Field, selectedOperator.Operator, value);

            FillList();
        }

        private void listViewCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCondition.SelectedItems.Count < 1)
                return;

            SingleCondition cond = (SingleCondition)listViewCondition.SelectedItems[0].Tag;

            foreach (FieldItem item in comboBoxField.Items)
            {
                if (item.Field == cond.Field)
                {
                    comboBoxField.SelectedItem = item;
                    break;
                }
            }

            foreach (OperatorItem item in comboBoxOperator.Items)
            {
                if (item.Operator == cond.Operator)
                {
                    comboBoxOperator.SelectedItem = item;
                    break;
                }
            }

            textBoxValue.Text = cond.Value.ToString();


            UpdateWindowState();
        }

        private void comboBoxField_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void comboBoxOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }
    }
}
