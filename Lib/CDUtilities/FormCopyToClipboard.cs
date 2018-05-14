using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Microsoft.Win32;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormCopyToClipboard : Form
    {
        public class ComboBoxSeperator
        {
            string display;
            public string Value;

            public ComboBoxSeperator(string displaystring, string valuestring)
            {
                display = displaystring;
                Value = valuestring;
            }

            public override string ToString()
            {
                return display;
            }
        }

        private int[] records;
        private DataBase dataBase;
        private bool copyTracks;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="recs"></param>
        /// <param name="tracks">true, wenn es sich um Track-IDs handelt.</param>
        public FormCopyToClipboard(DataBase db, int[] recs, bool tracks)
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);

            records = recs;
            dataBase = db;
            copyTracks = tracks;

            comboBoxFieldSeperator.Items.Add(new ComboBoxSeperator("TAB", "\t"));
            comboBoxFieldSeperator.Items.Add(new ComboBoxSeperator(";", ";"));
            comboBoxFieldSeperator.Items.Add(new ComboBoxSeperator(",", ","));

            comboBoxRecordSeperator.Items.Add(new ComboBoxSeperator("CR/LF", "\r\n"));

            FieldCollection selectedFields;
            string[] defaultFields;

            if (copyTracks)
            {
                defaultFields = new string[2];
                defaultFields[0] = ((int)Field.ArtistTrackName).ToString();
                defaultFields[1] = ((int)Field.TrackTitle).ToString();
            }
            else
            {
                defaultFields = new string[2];
                defaultFields[0] = ((int)Field.ArtistCDName).ToString();
                defaultFields[1] = ((int)Field.Title).ToString();
            }

            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Global.HitbaseRegistryKey))
            {
                regKey.OpenSubKey(Global.HitbaseRegistryKey);
                string[] stringFields;
                
                if (copyTracks)
                    stringFields = (string[])regKey.GetValue("CopyToClipboardTrackFields", defaultFields);
                else
                    stringFields = (string[])regKey.GetValue("CopyToClipboardCDFields", defaultFields);

                int iCount = 0;
                selectedFields = new FieldCollection();
                foreach (string s in stringFields)
                    selectedFields.Add((Field)Convert.ToInt32(s));

                string fieldSep = (string)regKey.GetValue("CopyToClipboardFieldSeperator");
                if (fieldSep != null)
                {
                    foreach (ComboBoxSeperator sep in comboBoxFieldSeperator.Items)
                    {
                        if (sep.Value == fieldSep)
                        {
                            comboBoxFieldSeperator.SelectedItem = sep;
                            break;
                        }
                    }
                }
                else
                {
                    comboBoxFieldSeperator.SelectedIndex = 0;
                }

                string recordSep = (string)regKey.GetValue("CopyToClipboardRecordSeperator");
                if (recordSep != null)
                {
                    foreach (ComboBoxSeperator sep in comboBoxRecordSeperator.Items)
                    {
                        if (sep.Value == recordSep)
                        {
                            comboBoxRecordSeperator.SelectedItem = sep;
                            break;
                        }
                    }
                }
                else
                {
                    comboBoxRecordSeperator.SelectedIndex = 0;
                }

                checkBoxFieldsInFirstLine.Checked = Convert.ToBoolean(regKey.GetValue("CopyToClipboardFieldsInFirstLine", true));
                checkBoxQuoteTextFields.Checked = Convert.ToBoolean(regKey.GetValue("CopyToClipboardQuoteTextFields", false));
            }

            chooseFieldControl.Init(dataBase, copyTracks ? FieldType.TrackAndCD : FieldType.CD, selectedFields);
            chooseFieldControl.SelectionChanged += new ChooseFieldControl.SelectionChangedDelegate(chooseFieldControl_SelectionChanged);
        }

        void chooseFieldControl_SelectionChanged()
        {
            buttonOK.Enabled = chooseFieldControl.SelectedFields.Count > 0;
        }

        private void FormCopyToClipboard_Load(object sender, EventArgs e)
        {
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Felder in Registry speichern
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Global.HitbaseRegistryKey, true))
            {
                // Field-Array in String-Array konvertieren
                String[] stringFields = new string[chooseFieldControl.SelectedFields.Count];
                int iCount = 0;
                foreach (Field field in chooseFieldControl.SelectedFields)
                    stringFields[iCount++] = ((int)field).ToString();

                if (copyTracks)
                    regKey.SetValue("CopyToClipboardTrackFields", stringFields);
                else
                    regKey.SetValue("CopyToClipboardCDFields", stringFields);

                regKey.SetValue("CopyToClipboardFieldSeperator", ((ComboBoxSeperator)comboBoxFieldSeperator.SelectedItem).Value);
                regKey.SetValue("CopyToClipboardRecordSeperator", ((ComboBoxSeperator)comboBoxRecordSeperator.SelectedItem).Value);

                regKey.SetValue("CopyToClipboardFieldsInFirstLine", checkBoxFieldsInFirstLine.Checked);
                regKey.SetValue("CopyToClipboardQuoteTextFields", checkBoxQuoteTextFields.Checked);
            }

            CopyDataToClipboard(chooseFieldControl.SelectedFields);
        }

        private void CopyDataToClipboard(FieldCollection selectedFields)
        {
            StringBuilder clipboardText = new StringBuilder();

            string fieldSeperator = ((ComboBoxSeperator)comboBoxFieldSeperator.SelectedItem).Value;
            string recordSeperator = ((ComboBoxSeperator)comboBoxRecordSeperator.SelectedItem).Value;

            // Header-Zeile schreiben?
            if (checkBoxFieldsInFirstLine.Checked)
            {
                int iCount = 0;
                foreach (Field field in selectedFields)
                {
                    clipboardText.Append(dataBase.GetNameOfFieldFull(field));
                    
                    if (iCount < selectedFields.Count-1)
                        clipboardText.Append(fieldSeperator);
                    
                    iCount++;
                }

                clipboardText.Append(recordSeperator);
            }

            progressBar.Maximum = records.Length;
            progressBar.Value = 0;
            CDQueryDataSet cdQuery = null;
            
            if (copyTracks)
                cdQuery = dataBase.ExecuteTrackQuery();
            else
                cdQuery = dataBase.ExecuteCDQuery();

            foreach (int id in records)
            {
                if (copyTracks)
                {
                    string filter = string.Format("TrackID={0}", id);
                    CDQueryDataSet.TrackRow liedRow = (CDQueryDataSet.TrackRow)cdQuery.Track.Select(filter)[0];
                    int iCount = 0;
                    foreach (Field field in selectedFields)
                    {
                        object value = liedRow.GetValueByField(dataBase, field);
                        if (checkBoxQuoteTextFields.Checked && value is string)
                            clipboardText.Append("\"" + value + "\"");
                        else
                            clipboardText.Append(value);

                        if (iCount < selectedFields.Count - 1)
                            clipboardText.Append(fieldSeperator);

                        iCount++;
                    }
                }
                else
                {
                    string filter = string.Format("CDID={0}", id);
                    CDQueryDataSet.CDRow cdRow = (CDQueryDataSet.CDRow)cdQuery.CD.Select(filter)[0];
                    int iCount = 0;
                    foreach (Field field in selectedFields)
                    {
                        object value = cdRow.GetValueByField(field);
                        if (checkBoxQuoteTextFields.Checked && value is string)
                            clipboardText.Append("\"" + value + "\"");
                        else
                            clipboardText.Append(value);

                        if (iCount < selectedFields.Count - 1)
                            clipboardText.Append(fieldSeperator);

                        iCount++;
                    }
                }
                
                clipboardText.Append(recordSeperator);

                progressBar.Value++;
                progressBar.Update();
                Application.DoEvents();
            }

            Clipboard.SetText(clipboardText.ToString(), TextDataFormat.Text);
        }
    }
}
