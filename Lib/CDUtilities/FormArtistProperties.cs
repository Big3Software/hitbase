using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using System.Collections;
using System.Diagnostics;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormArtistProperties : Form
    {
        public string imageFilename;
        private PersonGroupDataSet.PersonGroupRow thePersonGroupRow = null;
        private DataBase dataBase;
        private PersonType personType;
        private string originalArtistName;

        public FormArtistProperties()
        {
            InitializeComponent();
        }

        public FormArtistProperties(DataBase db, PersonType persontype, PersonGroupDataSet.PersonGroupRow personGroupRow)
        {
            InitializeComponent();

            switch (persontype)
            {
                case PersonType.Composer:
                    Text = StringTable.Composer;
                    groupBox.Text = StringTable.Composer;
                    break;
                case PersonType.Artist:
                    Text = StringTable.Artist;
                    groupBox.Text = StringTable.Artist;
                    break;
                case PersonType.Unknown:
                default:
                    Text = StringTable.PersonOrGroup;
                    groupBox.Text = StringTable.PersonOrGroup;
                    break;
            }

            dataBase = db;

            originalArtistName = personGroupRow.Name;

            personType = persontype;
            thePersonGroupRow = personGroupRow;
            thePersonGroupRow.BeginEdit();

            comboBoxArtistType.DataSource = DataBase.GetAvailablePersonGroupTypes();
            comboBoxSex.DataSource = DataBase.GetAvailablePersonGroupSex();

            textBoxName.DataBindings.Add("Text", personGroupRow, "Name");
            textBoxSaveAs.DataBindings.Add("Text", personGroupRow, "SaveAs");
            textBoxComment.DataBindings.Add("Text", personGroupRow, "Comment");
            textBoxHomepage.DataBindings.Add("Text", personGroupRow, "URL");
            comboBoxArtistType.DataBindings.Add("SelectedIndex", personGroupRow, "Type", true);
            comboBoxSex.DataBindings.Add("SelectedIndex", personGroupRow, "Sex", true);
            comboBoxCountry.DataBindings.Add("Text", personGroupRow, "Country");

            // Vorhandene Länder ermitteln
            String[] distinctValues = dataBase.GetAvailableCountries();

            comboBoxCountry.DataSource = distinctValues;

            if (!personGroupRow.IsImageFilenameNull() && personGroupRow.ImageFilename.Length > 0)
            {
                choosePictureButton.ImageFilename = personGroupRow.ImageFilename;
            }

            if (!personGroupRow.IsBirthDayNull())
                textBoxBirthday.Text = Misc.FormatDate(personGroupRow.BirthDay);

            if (!personGroupRow.IsDayOfDeathNull())
                textBoxDayOfDeath.Text = Misc.FormatDate(personGroupRow.DayOfDeath);

            UpdateWindowState();

            choosePictureButton.CoverType = CoverType.PersonGroup;
            choosePictureButton.PersonGroup = personGroupRow;
        }

        private void FormArtistProperties_Load(object sender, EventArgs e)
        {
        }

        private void comboBoxCountry_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonGoToHomepage_Click(object sender, EventArgs e)
        {
            if (textBoxHomepage.Text != null && textBoxHomepage.Text.Length > 0)
            {
                try
                {
                    Process.Start(textBoxHomepage.Text);
                }
                catch
                {       // Exception wird ignoriert
                }
            }
        }

        private void UpdateWindowState()
        {
            buttonOK.Enabled = (textBoxName.Text.Length > 0);

            PersonGroupType groupType = (PersonGroupType)comboBoxArtistType.SelectedIndex;
            if (groupType == PersonGroupType.Single)
            {
                labelBirthday.Text = StringTable.Born + ":";
                labelDeath.Text = StringTable.Died + ":";
            }
            else
            {
                labelBirthday.Text = StringTable.Founded + ":";
                labelDeath.Text = StringTable.BreakAway + ":";
            }
        }

        private void textBoxBirthday_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (textBoxBirthday.Text.Length > 0)
                {
                    thePersonGroupRow.BirthDay = Misc.ParseDate(textBoxBirthday.Text);
                    // Das richtig formatierte Datum anzeigen
                    textBoxBirthday.Text = Misc.FormatDate(thePersonGroupRow.BirthDay);
                }
                else
                {
                    thePersonGroupRow.SetBirthDayNull();
                }
            }
            catch
            {
                MessageBox.Show(StringTable.InvalidDateValue, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }
        }

        private void textBoxDayOfDeath_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (textBoxDayOfDeath.Text.Length > 0)
                {
                    thePersonGroupRow.DayOfDeath = Misc.ParseDate(textBoxDayOfDeath.Text);
                    textBoxDayOfDeath.Text = Misc.FormatDate(thePersonGroupRow.DayOfDeath);
                }
                else
                {
                    thePersonGroupRow.SetDayOfDeathNull();
                }
            }
            catch
            {
                MessageBox.Show(StringTable.InvalidDateValue, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }
        }

        private void textBoxName_Leave(object sender, EventArgs e)
        {
            // Wenn man nur zwei Wörter eingegeben hat, setzen wir bei "Speichern unter" jetzt erstmal 
            // standardmäßig die getauschten Wörter (z.B. "Bryan Adams" -> "Adams, Bryan"

            string[] words = textBoxName.Text.Split(' ');
            if (textBoxSaveAs.Text.Length < 1)
            {
                if (words.Length == 2)
                {
                    textBoxSaveAs.Text = words[1] + ", " + words[0];
                    thePersonGroupRow.SaveAs = textBoxSaveAs.Text;
                }
                else
                {
                    textBoxSaveAs.Text = textBoxName.Text;
                    thePersonGroupRow.SaveAs = textBoxSaveAs.Text;
                }
            }

            UpdateWindowState();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            thePersonGroupRow.CancelEdit();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (String.Compare(originalArtistName, textBoxName.Text, true) != 0)
            {
                // JUS 16.10.2004: Prüfen, ob Interpret schon vorhanden 
                PersonGroupDataSet.PersonGroupRow row = dataBase.GetPersonGroupRowByName(textBoxName.Text, false);

                if (row != null)
                {
                    String str;
                    
                    if (personType == PersonType.Artist)
                        str = String.Format(StringTable.ArtistAlreadyExists, textBoxName.Text);
                    else
                        str = String.Format(StringTable.ComposerAlreadyExists, textBoxName.Text);

                    MessageBox.Show(str, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            DialogResult = DialogResult.OK;

            thePersonGroupRow.EndEdit();

            if (!string.IsNullOrEmpty(choosePictureButton.ImageFilename))
            {
                thePersonGroupRow.ImageFilename = choosePictureButton.ImageFilename;
            }
            else
            {
                thePersonGroupRow.SetImageFilenameNull();
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void comboBoxArtistType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }
    }
}