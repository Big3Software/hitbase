using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.DataBaseEngine.CDQueryDataSetTableAdapters;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormSearchAndReplace : Form
    {
        public class ComboBoxItem
        {
            public Field Field;
            private DataBase dataBase;

            public ComboBoxItem(DataBase db, Field f)
            {
                dataBase = db;
                Field = f;
            }

            public override string ToString()
            {
                return dataBase.GetNameOfField(Field);
            }
        }

        private DataBase dataBase;
        private bool canceled;

        public FormSearchAndReplace(DataBase db)
        {
            dataBase = db;

            InitializeComponent();

            FormThemeManager.SetTheme(this);
        }

        private void radioButtonCD_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCD.Checked)
                FillFieldList();
        }

        private void radioButtonTrack_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTrack.Checked)
                FillFieldList();
        }

        private void FillFieldList()
        {
            comboBoxField.Items.Clear();

            FieldCollection fields;
            if (radioButtonCD.Checked)
            {
                fields = FieldHelper.GetAllCDFields(true);
            }
            else
            {
                fields = FieldHelper.GetAllTrackFields(true);
            }

            CDQueryDataSet ds = new CDQueryDataSet();

            foreach (Field field in fields)
            {
                DataColumn dataColumn;

                dataColumn = DataBase.GetDataColumnByField(field);

                if (dataColumn != null && dataColumn.DataType == typeof(string))
                    comboBoxField.Items.Add(new ComboBoxItem(dataBase, field));
            }
        }

        private void comboBoxField_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonSearchAndReplace.Enabled = (comboBoxField.SelectedIndex >= 0 && textBoxSearchFor.Text.Length > 0);
        }

        private void buttonSearchAndReplace_Click(object sender, EventArgs e)
        {
            canceled = false;

            if (radioButtonCD.Checked)
            {
                SearchAndReplaceCD();
            }
            else
            {
                SearchAndReplaceTrack();
            }
        }

        private void SearchAndReplaceTrack()
        {
            TrackTableAdapter ta = new TrackTableAdapter(dataBase);
            CDQueryDataSet ds = new CDQueryDataSet();
            //ds.EnforceConstraints = false;
            ta.Fill(ds.Track);

            Field selectedField = ((ComboBoxItem)comboBoxField.SelectedItem).Field;

            progressBar.Visible = true;
            labelStatus.Visible = true;
            labelStatus.Text = "";
            progressBar.Minimum = 0;
            progressBar.Maximum = ds.Track.Rows.Count;

            int found = 0;

            DataColumn fieldColumn = ds.Track.GetDataColumnByField(selectedField);

            foreach (Big3.Hitbase.DataBaseEngine.CDQueryDataSet.TrackRow row in ds.Track.Rows)
            {
                if (canceled)
                    break;

                object value = row[fieldColumn];

                if (value is string)
                {
                    string str = value as string;

                    if (FindAndReplaceString(ref str))
                    {
                        row[fieldColumn] = str;

                        ta.Update(row);

                        found++;
                        labelStatus.Text = string.Format(StringTable.RecordsReplaced, found);
                        labelStatus.Update();
                    }
                }

                progressBar.Value++;
                progressBar.Update();

                Application.DoEvents();
            }

            DialogResult = DialogResult.OK;

            string message;

            if (found == 0)
                message = StringTable.FinishReplaceNothing;
            else
                if (found == 1)
                    message = StringTable.FinishReplaceSingle;
                else
                    message = string.Format(StringTable.FinishReplaceMulti, found);

            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SearchAndReplaceCD()
        {
            CDTableAdapter ta = new CDTableAdapter(dataBase);
            CDQueryDataSet ds = new CDQueryDataSet();
            ta.Fill(ds.CD);

            Field selectedField = ((ComboBoxItem)comboBoxField.SelectedItem).Field;

            progressBar.Visible = true;
            labelStatus.Visible = true;
            labelStatus.Text = "";
            progressBar.Minimum = 0;
            progressBar.Maximum = ds.CD.Rows.Count;

            int found = 0;

            DataColumn fieldColumn = ds.CD.GetDataColumnByField(selectedField);

            foreach (CDQueryDataSet.CDRow row in ds.CD.Rows)
            {
                if (canceled)
                    break;

                object value = row.GetValueByField(selectedField);

                if (value is string)
                {
                    string str = value as string;

                    if (FindAndReplaceString(ref str))
                    {
                        row[fieldColumn] = str;

                        ta.Update(row);

                        found++;
                        labelStatus.Text = string.Format(StringTable.RecordsReplaced, found);
                        labelStatus.Update();
                    }
                }

                progressBar.Value++;
                progressBar.Update();

                Application.DoEvents();
            }

            DialogResult = DialogResult.OK;

            string message;

            if (found == 0)
                message = StringTable.FinishReplaceNothing;
            else
                if (found == 1)
                    message = StringTable.FinishReplaceSingle;
                else
                    message = string.Format(StringTable.FinishReplaceMulti, found);

            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool FindAndReplaceString(ref string str)
        {
            int startPos = 0;
            bool somethingChanged = false;

            while (true)
            {
                int posFound;

                if (checkBoxCaseSensitive.Checked == true)
                    posFound = str.IndexOf(textBoxSearchFor.Text, startPos);
                else
                    posFound = str.IndexOf(textBoxSearchFor.Text, startPos, StringComparison.CurrentCultureIgnoreCase);

                if (posFound >= 0)
                {
                    bool wholeword = true;
                    if (checkBoxWholeWords.Checked == true)
                    {
                        if (posFound > 0 && !Char.IsSeparator(str[posFound - 1]) && !Char.IsPunctuation(str[posFound - 1]))
                            wholeword = false;

                        if (posFound + textBoxSearchFor.Text.Length < str.Length && !Char.IsSeparator(str[posFound + textBoxSearchFor.Text.Length]) && !Char.IsPunctuation(str[posFound + textBoxSearchFor.Text.Length]))
                            wholeword = false;
                    }

                    if (wholeword)
                    {
                        // Gefunden, jetzt ersetzen und updaten
                        str = str.Remove(posFound, textBoxSearchFor.Text.Length);
                        str = str.Insert(posFound, textBoxReplace.Text);
                        somethingChanged = true;
                    }

                    startPos = posFound + textBoxReplace.Text.Length;
                }
                else
                {
                    break;
                }
            }

            return somethingChanged;
        }

        private void textBoxSearchFor_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void textBoxReplace_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void FormSearchAndReplace_Load(object sender, EventArgs e)
        {
            // Warnmeldung ausgeben
            bool answer = false;
            MessageBoxEx.Show(this, StringTable.SearchAndReplaceWarning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, ref answer, "SearchAndReplaceWarning");

            UpdateWindowState();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            canceled = true;
        }
    }
}