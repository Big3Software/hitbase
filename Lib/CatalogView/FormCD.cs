using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.CDUtilities;
using System.Diagnostics;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.CatalogView
{
    public partial class FormCD : Form
    {
        public CD CD
        {
            get;
            set;
        }

        public int CurrentTrack
        {
            get;
            set;
        }

        public bool ShowTrack { get; set; }

        public DataBase DataBase
        {
            get;
            set;
        }

        private bool readOnly;
        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                readOnly = value;
                UpdateWindowState();
            }
        }

        private bool embedded;
        public bool Embedded
        { 
            get 
            { 
                return embedded; 
            }
            set
            {
                embedded = value;
                buttonOK.Visible = false;
                buttonCancel.Visible = false;
            }
        }

        public FormCD()
        {
            InitializeComponent();
        }

        public FormCD(CD cd, DataBase db)
        {
            CD = cd;
            DataBase = db;

            InitializeComponent();

            pictureButtonBackCover.CoverType = CoverType.Back;
            pictureButtonFrontCover.CoverType = CoverType.Front;
            pictureButtonCDLabel.CoverType = CoverType.Label;

            pictureButtonBackCover.CD = CD;
            pictureButtonFrontCover.CD = CD;
            pictureButtonCDLabel.CD = CD;

            userFieldsCD.SetFields(DataBase, DataBase.Master.UserCDFields);
            userFieldsTrack.SetFields(DataBase, DataBase.Master.UserTrackFields);

            textBoxTitleCD.DataBase = DataBase;
            textBoxTitleCD.AutoCompleteTextBoxType = AutoCompleteTextBoxType.Title;
            textBoxArtistCD.DataBase = DataBase;
            textBoxArtistCD.AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;
            textBoxComposerCD.DataBase = DataBase;
            textBoxComposerCD.AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;

            textBoxArtistTrack.DataBase = DataBase;
            textBoxArtistTrack.AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;
            textBoxTitleTrack.DataBase = DataBase;
            textBoxTitleTrack.AutoCompleteTextBoxType = AutoCompleteTextBoxType.TrackTitle;
            textBoxComposerTrack.DataBase = DataBase;
            textBoxComposerTrack.AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;

            participantsList.DataBase = DataBase;
            participantsList.ShowTitle = false;

            Fill();
        }

        private void Fill()
        {
            FillCategories();

            foreach (Medium medium in DataBase.AllMediums)
            {
                comboBoxMediumCD.Items.Add(medium.Name);
            }

            foreach (string language in DataBase.AllLanguages)
            {
                comboBoxLanguageCD.Items.Add(language);
                comboBoxLanguageTrack.Items.Add(language);
            }

            foreach (string label in DataBase.AllLabels)
            {
                comboBoxLabelCD.Items.Add(label);
            }

            foreach (string set in DataBase.GetAvailableSets())
            {
                comboBoxSetCD.Items.Add(set);
            }

            FillCD();
            FillTrack();
            FillParticipant();

            SetSamplerText();

            UpdateWindowState();
        }

        private void FillCD()
        {
            if (CD == null)
                return;

            checkBoxOriginalCD.Checked = CD.Original;
            checkBoxSampler.Checked = CD.Sampler;
            textBoxTitleCD.Text = CD.Title;
            textBoxArtistCD.Text = CD.Artist;
            textBoxComposerCD.Text = CD.Composer;
            comboBoxCategoryCD.SelectedItem = CD.Category;
            comboBoxMediumCD.SelectedItem = CD.Medium;
            numericUpDownNumberOfTracks.Value = CD.NumberOfTracks;
            textBoxDateCD.Text = this.DataBase.FormatDate(CD.Date);
            textBoxCodesCD.Text = CD.Codes;
            textBoxArchiveNumberCD.Text = CD.ArchiveNumber;
            textBoxYearRecordedCD.Text = CD.YearRecorded > 0 ? CD.YearRecorded.ToString() : "";
            ratingControlCD.Value = CD.Rating;
            textBoxCopyrightCD.Text = CD.Copyright;
            comboBoxLabelCD.Text = CD.Label;
            comboBoxLanguageCD.Text = CD.Language;
            textBoxUPCCD.Text = CD.UPC;
            textBoxLocationCD.Text = CD.Location;
            textBoxHomepageCD.Text = CD.URL;
            textBoxCommentCD.Text = CD.Comment;
            labelDateCD.Text = DataBase.Master.DateName + ":";
            labelIdentityCD.Text = CD.Identity;
            textBoxLengthCD.Text = Misc.GetLongTimeString(CD.TotalLength);
            textBoxLengthCD.ReadOnly = !string.IsNullOrEmpty(CD.Identity);
            textBoxPriceCD.Value = CD.Price;

            numericUpDownNumberInCDSet.Value = CD.CDSetNumber;
            comboBoxSetCD.Text = CD.CDSetName;
            if (!string.IsNullOrEmpty(CD.CDSetName))
                radioButtonMultiCD.Checked = true;
            else
                radioButtonSingleCD.Checked = true;

            userFieldsCD.SetValue(0, CD.UserField1);
            userFieldsCD.SetValue(1, CD.UserField2);
            userFieldsCD.SetValue(2, CD.UserField3);
            userFieldsCD.SetValue(3, CD.UserField4);
            userFieldsCD.SetValue(4, CD.UserField5);

            pictureButtonFrontCover.ImageFilename = CD.CDCoverFrontFilename;
            pictureButtonBackCover.ImageFilename = CD.CDCoverBackFilename;
            pictureButtonCDLabel.ImageFilename = CD.CDCoverLabelFilename;
        }

        private void FillParticipant()
        {
            if (CD == null)
                return;

            participantsList.Fill(CD.Participants);
        }

        private void GetCD()
        {
            if (CD == null)
                return;

            CD.Artist = textBoxArtistCD.Text;
            CD.Composer = textBoxComposerCD.Text;
            CD.Title = textBoxTitleCD.Text;
            CD.Original = checkBoxOriginalCD.Checked;
            CD.Sampler = checkBoxSampler.Checked;
            CD.Codes = textBoxCodesCD.Text;
            CD.NumberOfTracks = (int)numericUpDownNumberOfTracks.Value;

            if (comboBoxCategoryCD.SelectedItem != null)
                CD.Category = comboBoxCategoryCD.SelectedItem.ToString();
            else
                CD.Category = "";

            if (comboBoxMediumCD.SelectedItem != null)
                CD.Medium = comboBoxMediumCD.SelectedItem.ToString();
            else
                CD.Medium = "";

            CD.Date = this.DataBase.ParseDate(textBoxDateCD.Text);
            CD.ArchiveNumber = textBoxArchiveNumberCD.Text;

            int yearRecorded = 0;
            Int32.TryParse(textBoxYearRecordedCD.Text, out yearRecorded);
            CD.YearRecorded = yearRecorded;
            CD.Rating = ratingControlCD.Value;

            CD.Copyright = textBoxCopyrightCD.Text;
            CD.Label = comboBoxLabelCD.Text;
            CD.Language = comboBoxLanguageCD.Text;
            CD.UPC = textBoxUPCCD.Text;
            CD.Location = textBoxLocationCD.Text;
            CD.URL = textBoxHomepageCD.Text;
            CD.Comment = textBoxCommentCD.Text;
            CD.Price = textBoxPriceCD.Value;

            if (String.IsNullOrEmpty(CD.Identity))
                CD.TotalLength = Misc.ParseTimeString(textBoxLengthCD.Text);

            if (radioButtonSingleCD.Checked)
            {
                CD.CDSetName = "";
                CD.CDSetNumber = 0;
            }
            else
            {
                CD.CDSetNumber = (int)numericUpDownNumberInCDSet.Value;
                CD.CDSetName = comboBoxSetCD.Text;
            }

            CD.UserField1 = userFieldsCD.GetValue(0);
            CD.UserField2 = userFieldsCD.GetValue(1);
            CD.UserField3 = userFieldsCD.GetValue(2);
            CD.UserField4 = userFieldsCD.GetValue(3);
            CD.UserField5 = userFieldsCD.GetValue(4);

            CD.CDCoverFrontFilename = pictureButtonFrontCover.ImageFilename;
            CD.CDCoverBackFilename = pictureButtonBackCover.ImageFilename;
            CD.CDCoverLabelFilename = pictureButtonCDLabel.ImageFilename;

            GetTrack();

            CD.Participants = participantsList.GetData();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (tabControl.SelectedIndex == 1)
            {
                if (keyData == Keys.PageDown)
                {
                    NextTrack();
                    return true;
                }

                if (keyData == Keys.PageUp)
                {
                    PreviousTrack();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void GetTrack()
        {
            if (CD == null || ReadOnly)
                return;
            Track track = CD.Tracks[CurrentTrack];

            track.Artist = textBoxArtistTrack.Text;
            track.Title = textBoxTitleTrack.Text;
            track.Composer = textBoxComposerTrack.Text;
            track.Codes = textBoxCodesTrack.Text;
            if (comboBoxCategoryTrack.SelectedItem != null)
                track.Category = comboBoxCategoryTrack.SelectedItem.ToString();
            else
                track.Category = "";

            track.Length = Misc.ParseTimeString(textBoxLengthTrack.Text);
            track.Language = comboBoxLanguageTrack.Text;
            track.Bpm = (int)numericUpDownBpmTrack.Value;

            track.YearRecorded = Misc.Atoi(textBoxYearRecordedTrack.Text);

            track.Comment = textBoxCommentTrack.Text;
            track.Soundfile = textBoxSoundfileTrack.Text;
            track.Rating = ratingControlTrack.Value;
            track.Lyrics = textBoxLyricsTrack.Text;

            track.UserField1 = userFieldsTrack.GetValue(0);
            track.UserField2 = userFieldsTrack.GetValue(1);
            track.UserField3 = userFieldsTrack.GetValue(2);
            track.UserField4 = userFieldsTrack.GetValue(3);
            track.UserField5 = userFieldsTrack.GetValue(4);
        }

        private void UpdateWindowState()
        {
            // Alle Controls auf Read-Only setzen
            if (ReadOnly)
            {
                SetReadOnly();
                return;
            }

            if (CD == null)
                return;

            buttonPreviousTrack.Enabled = (CurrentTrack > 0);
            buttonNextTrack.Enabled = (CurrentTrack < CD.NumberOfTracks - 1);

            numericUpDownNumberInCDSet.Enabled = radioButtonMultiCD.Checked;
            comboBoxSetCD.Enabled = radioButtonMultiCD.Checked;

            textBoxArtistCD.ReadOnly = Settings.Current.SamplerUseFixedArtist && checkBoxSampler.Checked;

            buttonArtistCD.Enabled = textBoxArtistCD.Text.Length > 0;
            buttonComposerCD.Enabled = textBoxComposerCD.Text.Length > 0;
            buttonArtistTrack.Enabled = textBoxArtistTrack.Text.Length > 0;
            buttonComposerTrack.Enabled = textBoxComposerTrack.Text.Length > 0;
        }

        private void SetReadOnly()
        {
            SetReadOnly(this);

            if (CD != null)
            {
                buttonPreviousTrack.Enabled = (CurrentTrack > 0);
                buttonNextTrack.Enabled = (CurrentTrack < CD.NumberOfTracks - 1);
            }
        }

        private void SetReadOnly(Control parentCtl)
        {
            foreach (Control ctl in parentCtl.Controls)
            {
                if (ctl is TextBox)
                    ((TextBox)ctl).ReadOnly = ReadOnly;
                if (ctl is ComboBox || 
                    ctl is CheckBox || 
                    ctl is NumericUpDown || 
                    ctl is Button || 
                    ctl is Big3.Hitbase.Controls.RatingControl || 
                    ctl is RadioButton)
                    ctl.Enabled = false;

                SetReadOnly(ctl);
            }
        }

        void FillTrack()
        {
            if (CD == null)
                return;

            Track track = CD.Tracks[CurrentTrack];

            labelTrackNumber.Text = string.Format(StringTable.TrackNumber, CurrentTrack + 1);

            if (checkBoxSampler.Checked)
                textBoxArtistTrack.Text = track.Artist;
            else
                textBoxArtistTrack.Text = textBoxArtistCD.Text;

            textBoxTitleTrack.Text = track.Title;
            textBoxComposerTrack.Text = track.Composer;
            textBoxLengthTrack.Text = Misc.GetShortTimeString(track.Length);
            textBoxLengthTrack.ReadOnly = !string.IsNullOrEmpty(CD.Identity);
            textBoxCodesTrack.Text = track.Codes;
            comboBoxCategoryTrack.SelectedItem = track.Category;
            comboBoxLanguageTrack.Text = track.Language;
            if (track.YearRecorded > 0)
                textBoxYearRecordedTrack.Text = track.YearRecorded.ToString();
            else
                textBoxYearRecordedTrack.Text = "";

            textBoxCommentTrack.Text = track.Comment;
            textBoxSoundfileTrack.Text = track.Soundfile;
            ratingControlTrack.Value = track.Rating;
            textBoxLyricsTrack.Text = track.Lyrics;
            labelMD5Track.Text = track.CheckSum;
            numericUpDownBpmTrack.Value = track.Bpm;

            userFieldsTrack.SetValue(0, track.UserField1);
            userFieldsTrack.SetValue(1, track.UserField2);
            userFieldsTrack.SetValue(2, track.UserField3);
            userFieldsTrack.SetValue(3, track.UserField4);
            userFieldsTrack.SetValue(4, track.UserField5);

            textBoxArtistTrack.ReadOnly = !checkBoxSampler.Checked;

            labelSearchLyrics.Text = "";
        }

        private void buttonPreviousTrack_Click(object sender, EventArgs e)
        {
            PreviousTrack();
        }

        private void PreviousTrack()
        {
            GetTrack();

            if (CurrentTrack > 0)
                CurrentTrack--;

            FillTrack();

            UpdateWindowState();
        }

        private void buttonNextTrack_Click(object sender, EventArgs e)
        {
            NextTrack();
        }

        private void NextTrack()
        {
            GetTrack();

            if (CurrentTrack < CD.NumberOfTracks - 1)
                CurrentTrack++;

            FillTrack();

            UpdateWindowState();
        }

        private void radioButtonSingleCD_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void radioButtonMultiCD_CheckedChanged(object sender, EventArgs e)
        {
            if (this.numericUpDownNumberInCDSet.Value == 0)
            {
                // Wenn vorher noch kein Name für das CD-Set definiert wurde, nehmen wir
                // jetzt als Standard den Titel der CD.
                if (string.IsNullOrEmpty(this.comboBoxSetCD.SelectedText))
                    comboBoxSetCD.SelectedText = textBoxTitleCD.Text;

                numericUpDownNumberInCDSet.Value = 1;
            }

            UpdateWindowState();
        }

        private void buttonArtistCD_Click(object sender, EventArgs e)
        {
		    PersonGroupDataSet.PersonGroupRow row = DataBase.GetPersonGroupByName(textBoxArtistCD.Text, true);
            FormArtistProperties formArtistProperties = new FormArtistProperties(DataBase, PersonType.Artist, row);
            if (formArtistProperties.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter personGroupAdapter = new Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter(this.DataBase);

                    personGroupAdapter.Update(row);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void buttonComposerCD_Click(object sender, EventArgs e)
        {
            PersonGroupDataSet.PersonGroupRow row = DataBase.GetPersonGroupByName(textBoxComposerCD.Text, true);
            FormArtistProperties formComposerProperties = new FormArtistProperties(DataBase, PersonType.Composer, row);
            if (formComposerProperties.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter personGroupAdapter = new Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter(this.DataBase);

                    personGroupAdapter.Update(row);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void buttonNextFreeArchiveNumber_Click(object sender, EventArgs e)
        {
            textBoxArchiveNumberCD.Text = DataBase.GetNextFreeArchiveNumber();
        }

        private void buttonHomepage_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxHomepageCD.Text))
            {
                try
                {
                    Process.Start(textBoxHomepageCD.Text);
                }
                catch
                {       // Exception wird ignoriert
                }
            }
        }

        private void tabPageTracks_Enter(object sender, EventArgs e)
        {
            if (CD == null)
                return;

            if (!this.ReadOnly)
            {
                CD.NumberOfTracks = (int)numericUpDownNumberOfTracks.Value;
                FillTrack();
                UpdateWindowState();
            }
        }

        private void tabPageTracks_Leave(object sender, EventArgs e)
        {
            GetTrack();
        }

        private void buttonArtistTrack_Click(object sender, EventArgs e)
        {
            PersonGroupDataSet.PersonGroupRow row = DataBase.GetPersonGroupByName(textBoxArtistTrack.Text, true);
            FormArtistProperties formArtistProperties = new FormArtistProperties(DataBase, PersonType.Artist, row);
            if (formArtistProperties.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter personGroupAdapter = new Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter(this.DataBase);

                    personGroupAdapter.Update(row);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void buttonComposerTrack_Click(object sender, EventArgs e)
        {
            PersonGroupDataSet.PersonGroupRow row = DataBase.GetPersonGroupByName(textBoxComposerTrack.Text, true);
            FormArtistProperties formComposerProperties = new FormArtistProperties(DataBase, PersonType.Composer, row);
            if (formComposerProperties.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter personGroupAdapter = new Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter(this.DataBase);

                    personGroupAdapter.Update(row);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void buttonCategories_Click(object sender, EventArgs e)
        {
            FormCategories formCategories = new FormCategories(DataBase);

            formCategories.ShowDialog(this);

            UpdateCategories();
        }

        private void UpdateCategories()
        {
            GetCD();

            comboBoxCategoryCD.Items.Clear();
            comboBoxCategoryTrack.Items.Clear();

            FillCategories();

            comboBoxCategoryCD.SelectedItem = CD.Category;
            comboBoxCategoryTrack.SelectedItem = CD.Tracks[CurrentTrack].Category;
        }

        private void FillCategories()
        {
            // Liste der Kategorien neu füllen, da sie sich geändert haben kann
            foreach (Category category in DataBase.AllCategories)
            {
                comboBoxCategoryCD.Items.Add(category.Name);
                comboBoxCategoryTrack.Items.Add(category.Name);
            }
        }

        private void buttonMedium_Click(object sender, EventArgs e)
        {
            FormMediums formMediums = new FormMediums(DataBase);

            formMediums.ShowDialog(this);

            UpdateMediums();
        }

        private void UpdateMediums()
        {
            GetCD();

            comboBoxMediumCD.Items.Clear();

            FillMediums();

            comboBoxMediumCD.SelectedItem = CD.Medium;
        }

        private void FillMediums()
        {
            foreach (Medium medium in DataBase.AllMediums)
            {
                comboBoxMediumCD.Items.Add(medium.Name);
            }
        }

        private void buttonCodes_Click(object sender, EventArgs e)
        {
            GetCD();

            FormChooseCodes formChooseCodes = new FormChooseCodes(DataBase, CD.Codes);
            if (formChooseCodes.ShowDialog(this) == DialogResult.OK)
            {
                textBoxCodesCD.Text = formChooseCodes.Codes;
            }
        }

        private void buttonCategoryTrack_Click(object sender, EventArgs e)
        {
            FormCategories formCategories = new FormCategories(DataBase);

            formCategories.ShowDialog(this);

            UpdateCategories();
        }

        private void buttonCodesTrack_Click(object sender, EventArgs e)
        {
            GetCD();

            FormChooseCodes formChooseCodes = new FormChooseCodes(DataBase, CD.Tracks[CurrentTrack].Codes);
            if (formChooseCodes.ShowDialog(this) == DialogResult.OK)
            {
                textBoxCodesTrack.Text = formChooseCodes.Codes;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            GetCD();
        }

        private void buttonBpmTrack_Click(object sender, EventArgs e)
        {
            FormCalcBPM formCalcBPM = new FormCalcBPM();

            if (formCalcBPM.ShowDialog(this) == DialogResult.OK)
            {
                numericUpDownBpmTrack.Value = formCalcBPM.BPM;
            }
        }

        private void checkBoxSampler_CheckedChanged(object sender, EventArgs e)
        {
            SetSamplerText();

            if (checkBoxSampler.Checked)
            {
                if (Settings.Current.SamplerUseFixedArtist)
                {
                    textBoxArtistCD.Text = Settings.Current.SamplerFixedArtistText;
                }
            }

            UpdateWindowState();
        }

        private void SetSamplerText()
        {
            if (checkBoxSampler.Checked)
            {
                labelArtistCD.Text = StringTable.Title + " 1:";
                labelTitleCD.Text = StringTable.Title + " 2:";
            }
            else
            {
                labelArtistCD.Text = StringTable.Artist + ":";
                labelTitleCD.Text = StringTable.Title + ":";
            }
        }

        private void FormCD_Load(object sender, EventArgs e)
        {
            if (ShowTrack)
            {
                tabControl.SelectedIndex = 1;
            }

            participantsList.tableParticipants.Height -= 10;
        }

        private void pictureButtonFrontCover_LoadFromWeb()
        {
            CDCoverAmazon.GetCDCover(this.CD);

            FillCD();
        }

        private void buttonSoundfileTrack_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = StringTable.ChooseSoundfile;
            ofd.Filter = StringTable.FilterSoundfile;
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                textBoxSoundfileTrack.Text = ofd.FileName;
            }
        }

        private void buttonSearchLyrics_Click(object sender, EventArgs e)
        {
            labelSearchLyrics.Text = StringTable.SearchLyrics;
            labelSearchLyrics.Update();

            string lyrics = LyricsSearch.Search(textBoxArtistTrack.Text, textBoxTitleTrack.Text);

            if (!string.IsNullOrEmpty(lyrics))
            {
                textBoxLyricsTrack.Text = lyrics;
                labelSearchLyrics.Text = "";
            }
            else
            {
                labelSearchLyrics.Text = StringTable.LyricsNotFound;
            }
        }

        internal void SetCD(CD cd)
        {
            this.CD = cd;

            CurrentTrack = 0;

            Fill();

            //??? Das hier funktioniert nicht richtig... ich nehme an, liegt am Interop zwischen
            // WPF und Winforms. Können wir einbauen, wenn das FormCD auch WPF ist.
            //tabControl.SelectedIndex = 0;
        }

        internal void SetCD(CD cd, int trackIndex)
        {
            this.CD = cd;

            CurrentTrack = trackIndex;

            Fill();

            //??? Das hier funktioniert nicht richtig... ich nehme an, liegt am Interop zwischen
            // WPF und Winforms. Können wir einbauen, wenn das FormCD auch WPF ist.
            //tabControl.SelectedIndex = 1;
        }

        private void textBoxArtistCD_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void textBoxComposerCD_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void textBoxArtistTrack_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void textBoxComposerTrack_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void textBoxArchiveNumberCD_Leave(object sender, EventArgs e)
        {
            if (Configuration.Settings.Current.NoDuplicateArchiveNumbers)
            {
                int recordFound = this.DataBase.CheckArchiveNumber(textBoxArchiveNumberCD.Text);

                if (recordFound > 0 && recordFound != this.CD.ID)           // Doppelte Archiv-Nummer gefunden!!
                {
                    MessageBox.Show(StringTable.ArchiveNumberAlreadyExists, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBoxArchiveNumberCD.Focus();
                }
            }
        }
    }
}
