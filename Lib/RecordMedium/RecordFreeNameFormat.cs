using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.RecordMedium
{
    public partial class RecordFreeNameFormat : Form
    {
        public class ListBoxItem
        {
            public Field Field;
            private DataBase dataBase;
            
            public ListBoxItem(DataBase dataBase, Field field)
            {
                this.dataBase = dataBase;
                this.Field = field;
            }
            public override string ToString()
            {
                return dataBase.GetNameOfField(Field);
            }
        }

        private CD CD;
        private DataBase DataBase;
        private int currentselectedSampleTrack;
        RecordMedium Recordmedium;
   
        public RecordFreeNameFormat(CD cd, DataBase database, RecordMedium recmedium)
        {
            InitializeComponent();
            CD = cd;
            DataBase = database;
            FieldCollection fields = FieldHelper.GetAllCDFields(false);
            FieldCollection track_fields = FieldHelper.GetAllTrackFields(false);
            Recordmedium = recmedium;

            comboBoxFelder.Items.Add("Häufig benötigte Felder");
            comboBoxFelder.Items.Add("Alle Felder");

            for (int nPos = 0; nPos < CD.NumberOfTracks; nPos++)
            {
                domainUpDown1.Items.Add((nPos+1).ToString());
                domainUpDown1.SelectedIndex = 0;
            }

            foreach (Field field in fields)
                listAvailableFields.Items.Add(new ListBoxItem(database, field));

            foreach (Field tfield in track_fields)
                listAvailableTrackFields.Items.Add(new ListBoxItem(database, tfield));
            
            listTemplates.Items.AddRange(Settings.Current.RecordFilenameFormat);

            comboBoxFelder.SelectedIndex = 0;

            updateDialog();
        }

        public string SelectedNameFormat { get; set; }

        private void listTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            textFormatFile.Text = listTemplates.Text;
            buttonSaveTemplate.Enabled = true;
            buttonDeleteTemplate.Enabled = true;
            BuildFileName(currentselectedSampleTrack);
            updateDialog();
        }



        private string BuildFileName(int track)
        {
            string form = textFormatFile.Text;
            
            string fieldcontent;
            string fieldcontentNew;
            string fieldName;
            FieldCollection fields = FieldHelper.GetAllCDFields(false);
            FieldCollection track_fields = FieldHelper.GetAllTrackFields(false);

            // Dann liegt wohl keine CD im Laufwerk
            if (CD.ID == 0)
                return "";

            CDQueryDataSet CDQuery = DataBase.GetCDQueryById(CD.ID);

            foreach (Field field in fields)
            {
                try
                {
                    fieldName = DataBase.GetNameOfField(field);
                    if (form.IndexOf("[" + fieldName + "]") >= 0)
                    {
                        fieldcontent = CDQuery.CD[0].GetStringByField(DataBase, field);

                        // Hier unbedingt Backslashes aus Feldinhalten entfernen bzw. ersetzen
                        fieldcontent = fieldcontent.Replace("\\", Settings.Current.RecordFileNameCharBackslash);

                        form = form.Replace("[" + fieldName + "]", fieldcontent);
                    }
                }
                catch
                {
                }
            }

            foreach (Field field in track_fields)
            {
                try
                {
                    fieldName = DataBase.GetNameOfField(field);

                    if (form.IndexOf("[Track - " + fieldName + "]") >= 0)
                    {
                        fieldcontent = CD.GetTrackStringByField(track, field);

                        // Hier unbedingt Backslashes aus Feldinhalten entfernen bzw. ersetzen
                        fieldcontent = fieldcontent.Replace("\\", Settings.Current.RecordFileNameCharBackslash);

                        // Sonderbehandlung für Nr. Feld
                        if (fieldName == "Nr.")
                        {
                            //if (CD.NumberOfTracks > 9)
                            // Immer 2 Stellen auch wenn unter 10 Tracks
                            fieldcontentNew = string.Format("{0:00}", Convert.ToInt32(fieldcontent));

                            if (CD.NumberOfTracks > 99)
                                fieldcontentNew = string.Format("{0:000}", Convert.ToInt32(fieldcontent));

                            if (CD.NumberOfTracks > 999)
                                fieldcontentNew = string.Format("{0:0000}", Convert.ToInt32(fieldcontent));

                            form = form.Replace("[Track - " + fieldName + "]", fieldcontentNew);
                        }
                        else
                        {
                            form = form.Replace("[Track - " + fieldName + "]", fieldcontent);
                        }
                    }
                }
                catch
                {
                }
            }

            // Jetzt müssen noch eventuell nicht erlaubte Zeichen entfernt werden
            // Der Backslash hier nicht, nur im Feldnamen, siehe oben
            form = form.Replace("\"", Settings.Current.RecordFileNameCharAnfuehrung);
            //form = form.Replace("\\", Settings.Current.RecordFileNameCharBackslash);
            form = form.Replace(":", Settings.Current.RecordFileNameCharDoppelpunkt);
            form = form.Replace("?", Settings.Current.RecordFileNameCharFragezeichen);
            form = form.Replace(">", Settings.Current.RecordFileNameCharGroesser);
            form = form.Replace("<", Settings.Current.RecordFileNameCharKleiner);
            form = form.Replace("|", Settings.Current.RecordFileNameCharPipe);
            form = form.Replace("/", Settings.Current.RecordFileNameCharSlash);
            form = form.Replace("*", Settings.Current.RecordFileNameCharStern);
            if (Settings.Current.RecordFileNameCharUserOrg1.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg1, Settings.Current.RecordFileNameCharUserNew1);
            if (Settings.Current.RecordFileNameCharUserOrg2.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg2, Settings.Current.RecordFileNameCharUserNew2);
            if (Settings.Current.RecordFileNameCharUserOrg3.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg3, Settings.Current.RecordFileNameCharUserNew3);
            if (Settings.Current.RecordFileNameCharUserOrg4.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg4, Settings.Current.RecordFileNameCharUserNew4);
            if (Settings.Current.RecordFileNameCharUserOrg5.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg5, Settings.Current.RecordFileNameCharUserNew5);
            if (Settings.Current.RecordFileNameCharUserOrg6.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg6, Settings.Current.RecordFileNameCharUserNew6);

            textExample.Text = Recordmedium.textFileDir.Text + "\\" + form;
            
            //textFormatFile.Text = listTemplates.Text;
            SelectedNameFormat = textFormatFile.Text;
            
            return form;
        }



        private void buildFileName()
        {
            string form = textFormatFile.Text;
            string fieldcontent;
            string fieldName;
            FieldCollection fields = FieldHelper.GetAllCDFields(false);
            FieldCollection track_fields = FieldHelper.GetAllTrackFields(false);

            CDQueryDataSet CDQuery = DataBase.GetCDQueryById(CD.ID);

            foreach (Field field in fields)
            {
                try
                {
                    fieldName = DataBase.GetNameOfField(field);
                    if (form.IndexOf("[" + fieldName + "]") >= 0)
                    {
                        fieldcontent = CDQuery.CD[0].GetStringByField(DataBase, field);
                        form = form.Replace("[" + fieldName + "]", fieldcontent);
                    }
                }
                catch
                { 
                }
            }

            foreach (Field field in track_fields)
            {
                try
                {
                    fieldName = DataBase.GetNameOfField(field);
                    
                    if (form.IndexOf("[Track - " + fieldName + "]") >= 0)
                    {
                        fieldcontent = CD.GetTrackStringByField(currentselectedSampleTrack, field);

                        if (fieldName == "Nr.")
                        {
                            if (CD.NumberOfTracks > 9)
                                fieldcontent = string.Format("{0:00}", Convert.ToInt32(fieldcontent));

                            if (CD.NumberOfTracks > 99)
                                fieldcontent = string.Format("{0:000}", Convert.ToInt32(fieldcontent));

                            form = form.Replace("[Track - " + fieldName + "]", fieldcontent);
                        }
                        else
                        {
                            form = form.Replace("[Track - " + fieldName + "]", fieldcontent);
                        }
                    }
                }
                catch
                { 
                }
            }

            //form = form.Replace("[Titel]", CD.Title);
            //form = form.Replace("[Interpret]", CD.Artist);
            //form = form.Replace("[Track-Titel]", CD.Tracks[currentselectedSampleTrack].Title);
            //form = form.Replace("[Track-Interpret]", CD.Tracks[currentselectedSampleTrack].Artist);
            //form = form.Replace("[Tracknummer]", CD.Tracks[currentselectedSampleTrack].TrackNumber.ToString());

        }
        private void comboBoxFelder_SelectedIndexChanged(object sender, EventArgs e)
        {
            listAvailableFields.Items.Clear();
            listAvailableTrackFields.Items.Clear();

            if (comboBoxFelder.SelectedIndex == 0)
            {
                listAvailableFields.Items.Add(new ListBoxItem(this.DataBase, Field.Title));
                listAvailableFields.Items.Add(new ListBoxItem(this.DataBase, Field.ArtistCDName));
                listAvailableFields.Items.Add(new ListBoxItem(this.DataBase, Field.NumberOfTracks));
                listAvailableFields.Items.Add(new ListBoxItem(this.DataBase, Field.CDSet));
                
                listAvailableTrackFields.Items.Add(new ListBoxItem(this.DataBase, Field.TrackTitle));
                listAvailableTrackFields.Items.Add(new ListBoxItem(this.DataBase, Field.ArtistTrackName));
                listAvailableTrackFields.Items.Add(new ListBoxItem(this.DataBase, Field.TrackNumber));
            }
            if (comboBoxFelder.SelectedIndex == 1)
            {
                FieldCollection fields = FieldHelper.GetAllCDFields(false);
                FieldCollection track_fields = FieldHelper.GetAllTrackFields(false);

                foreach (Field field in fields)
                    listAvailableFields.Items.Add(new ListBoxItem(this.DataBase, field));

                foreach (Field tfield in track_fields)
                    listAvailableTrackFields.Items.Add(new ListBoxItem(this.DataBase, tfield));
            }
            updateDialog();
        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {
            currentselectedSampleTrack = domainUpDown1.SelectedIndex;

            BuildFileName(currentselectedSampleTrack);
        }

        private void listAvailableFields_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textFormatFile.Text = textFormatFile.Text.Insert(textFormatFile.SelectionStart, "[" + listAvailableFields.SelectedItem.ToString() + "]");
        }

        private void listAvailableTrackFields_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textFormatFile.Text = textFormatFile.Text.Insert(textFormatFile.SelectionStart, "[Track - " + listAvailableTrackFields.SelectedItem.ToString() + "]");
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            int nPos;
            Settings.Current.RecordFilenameFormat = new string[listTemplates.Items.Count];

            for (nPos = 0; nPos < listTemplates.Items.Count; nPos++)
            {
                Settings.Current.RecordFilenameFormat[nPos] = listTemplates.Items[nPos].ToString();
            }
        }

        private void buttonAddTemplate_Click(object sender, EventArgs e)
        {
            listTemplates.Items.Add("<Frei>");
            updateDialog();
        }

        private void buttonDeleteTemplate_Click(object sender, EventArgs e)
        {
            if (listTemplates.SelectedIndex >= 0)
            {
                listTemplates.Items.RemoveAt(listTemplates.SelectedIndex);
            }
            updateDialog();
        }

        private void buttonSaveTemplate_Click(object sender, EventArgs e)
        {
            int SelPos = listTemplates.SelectedIndex;
            listTemplates.Items.Insert(SelPos, textFormatFile.Text); 
            listTemplates.Items.RemoveAt(SelPos+1);
            updateDialog();
        }

        private void buttonResetDefault_Click(object sender, EventArgs e)
        {
            Settings.Current.RecordFilenameFormat = new String[4];

            Settings.Current.RecordFilenameFormat[0] = "[Interpret, Name]\\[Album]\\[Track - Nr.] - [Track - Titel]";
            Settings.Current.RecordFilenameFormat[1] = "[Interpret, Name]\\[Album]\\[Track - Nr.] - [Track - Interpret, Name] - [Track - Titel]";
            Settings.Current.RecordFilenameFormat[2] = "[Genre]\\[Interpret, Name]\\[Album]\\[Track - Nr.] - [Track - Titel]";
            Settings.Current.RecordFilenameFormat[3] = "[Genre]\\[Interpret, Name]\\[Album]\\[Track - Nr.] - [Track - Interpret, Name] - [Track - Titel]";

            listTemplates.Items.Clear();
            listTemplates.Items.AddRange(Settings.Current.RecordFilenameFormat);
            updateDialog();
        }
        private void updateDialog()
        {
            if (listTemplates.Items.Count == 0 || listTemplates.SelectedIndex < 0)
            {
                buttonDownTemplate.Enabled = false;
                buttonUpTemplate.Enabled = false;
                buttonDeleteTemplate.Enabled = false;
                buttonSaveTemplate.Enabled = false;
            }

            if (listTemplates.SelectedIndex >= 0)
            {
                if (listTemplates.SelectedIndex == listTemplates.Items.Count - 1)
                    buttonDownTemplate.Enabled = false;
                else
                    buttonDownTemplate.Enabled = true;
                if (listTemplates.SelectedIndex == 0)
                    buttonUpTemplate.Enabled = false;
                else
                    buttonUpTemplate.Enabled = true;

                buttonDeleteTemplate.Enabled = true;
                buttonSaveTemplate.Enabled = true;
            }
        }

        private void buttonUpTemplate_Click(object sender, EventArgs e)
        {
            int selIndex = listTemplates.SelectedIndex;
            listTemplates.Items.Insert(selIndex - 1, listTemplates.Text);
            listTemplates.Items.RemoveAt(selIndex + 1);
            listTemplates.SelectedIndex = selIndex - 1;
        }

        private void buttonDownTemplate_Click(object sender, EventArgs e)
        {
            int selIndex = listTemplates.SelectedIndex;
            listTemplates.Items.Insert(selIndex + 2, listTemplates.Text);
            listTemplates.Items.RemoveAt(selIndex);
            listTemplates.SelectedIndex = selIndex + 1;
        }
    }
}
