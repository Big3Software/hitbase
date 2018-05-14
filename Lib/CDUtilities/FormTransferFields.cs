using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormTransferFields : Form
    {
        private class ComboBoxItem
        {
            public Field Field { get; set; }
            
            private DataBase dataBase;

            public ComboBoxItem(DataBase db, Field field)
            {
                Field = field;
                dataBase = db;
            }

            public override string ToString()
            {
                return dataBase.GetNameOfFieldFull(Field);
            }
        }

        private DataBase dataBase;
        private Condition condition = new Condition();

        public FormTransferFields(DataBase db)
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);

            dataBase = db;
        }

        private void buttonFilter_Click(object sender, EventArgs e)
        {
            FormSearch formSearch = new FormSearch(dataBase, condition, false, false);
            if (formSearch.ShowDialog(this) == DialogResult.OK)
            {
                condition = formSearch.GetCondition();    
            }
        }

        private void FormTransferFields_Load(object sender, EventArgs e)
        {
        	FieldCollection fl = FieldHelper.GetAllFields();

            foreach (Field field in fl)
            {
                comboBoxSourceField.Items.Add(new ComboBoxItem(dataBase, field));
            }

            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonOK.Enabled = (comboBoxSourceField.SelectedItem != null && comboBoxTargetField.SelectedItem != null);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Field sourceField = ((ComboBoxItem)comboBoxSourceField.SelectedItem).Field;
            Field targetField = ((ComboBoxItem)comboBoxTargetField.SelectedItem).Field;
            if (DataBase.GetTypeByField(sourceField) != DataBase.GetTypeByField(targetField))
            {
                if (MessageBox.Show(StringTable.FieldTypeMismatch, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            TransferFields();
        }

        private void TransferFields()
        {
            FormProgress formProgress = new FormProgress();
            Field sourceField = ((ComboBoxItem)comboBoxSourceField.SelectedItem).Field;

            try
            {
                formProgress.Text = StringTable.TransferFieldsRunning;
                formProgress.Show();

                TransferFields(formProgress);
            }
            finally
            {
                formProgress.Close();
            }
        }

        private void TransferFields(FormProgress formProgress)
        {
            Field sourceField = ((ComboBoxItem)comboBoxSourceField.SelectedItem).Field;
            Field targetField = ((ComboBoxItem)comboBoxTargetField.SelectedItem).Field;

            CDQueryDataSet cdQuery = dataBase.ExecuteCDQuery();

            CDDataView CDView = new CDDataView(dataBase, cdQuery, condition, new SortFieldCollection(), new FieldCollection());

            int errorCount = 0;

            formProgress.ProgressBar.Maximum = CDView.Rows.Count;
            formProgress.ProgressBar.Value = 0;

            for (int i = 0; i < CDView.Rows.Count; i++)
            {
                CD cd = dataBase.GetCDById(CDView.GetCDID(i));

                try
                {
                    if (FieldHelper.IsCDField(sourceField))
                    {
                        object o = cd.GetValueByField(sourceField);
                        cd.SetValueToField(targetField, o);

                        if (checkBoxMoveField.Checked)
                        {
                            Type fieldType = DataBase.GetTypeByField(sourceField);
                            if (fieldType == typeof(string))
                            {
                                cd.SetValueToField(sourceField, "");
                            }
                            else if (fieldType == typeof(int))
                            {
                                cd.SetValueToField(sourceField, 0);
                            }
                            else
                            {
                                cd.SetValueToField(sourceField, null);
                            }
                        }
                    }
                    else
                    {
                        for (int track = 0; track < cd.Tracks.Count; track++)
                        {
                            object o = cd.GetTrackValueByField(track, sourceField);

                            cd.SetTrackValueToField(track, targetField, o);

                            if (checkBoxMoveField.Checked)
                            {
                                Type fieldType = DataBase.GetTypeByField(sourceField);
                                if (fieldType == typeof(string))
                                {
                                    cd.SetTrackValueToField(track, sourceField, "");
                                }
                                else if (fieldType == typeof(int))
                                {
                                    cd.SetTrackValueToField(track, sourceField, 0);
                                }
                                else
                                {
                                    cd.SetTrackValueToField(track, sourceField, null);
                                }
                            }
                        }
                    }

                    cd.Save(dataBase);
                }
                catch (Exception e)
                {
                    errorCount++;
                }

                if (formProgress.Canceled)
                    break;

                formProgress.ProgressBar.Value++;
                Application.DoEvents();
            }

            if (errorCount > 0)
            {
                MessageBox.Show(string.Format(StringTable.TransferFieldsTotal, CDView.Rows.Count - errorCount, CDView.Rows.Count), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void comboBoxSourceField_SelectedIndexChanged(object sender, EventArgs e)
        {
            Field sourceField = ((ComboBoxItem)comboBoxSourceField.SelectedItem).Field;
            FieldCollection fieldCol;
            
            if (FieldHelper.IsCDField(sourceField))
                fieldCol = FieldHelper.GetAllCDFields(false);
            else
                fieldCol = FieldHelper.GetAllTrackFields(false);

            foreach (Field field in fieldCol)
            {
                comboBoxTargetField.Items.Add(new ComboBoxItem(dataBase, field));
            }

            UpdateWindowState();
        }

        private void comboBoxTargetField_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }
    }
}
