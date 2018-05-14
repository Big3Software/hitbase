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
using Big3.Hitbase.Controls;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormSearch : Form
    {
        private DataBase dataBase;
        private Condition condition;
        private Condition codesCondition = new Condition();
        private Condition trackCodesCondition = new Condition();

        public FormSearch(DataBase db, Condition condition, bool showTrack, bool showCDTabOnly)
        {
            dataBase = db;
            this.condition = condition;

            InitializeComponent();

            textBoxArtistCD.DataBase = db;
            textBoxArtistCD.AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;
            textBoxTitleCD.DataBase = db;
            textBoxTitleCD.AutoCompleteTextBoxType = AutoCompleteTextBoxType.Title;
            textBoxComposerCD.DataBase = db;
            textBoxComposerCD.AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;

            textBoxArtistTrack.DataBase = db;
            textBoxArtistTrack.AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;
            textBoxTrackname.DataBase = db;
            textBoxTrackname.AutoCompleteTextBoxType = AutoCompleteTextBoxType.TrackTitle;
            textBoxComposerTrack.DataBase = db;
            textBoxComposerTrack.AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;

            if (showTrack)
                tabControl.SelectTab(1);

            if (showCDTabOnly)
                tabControl.TabPages.Remove(tabPageTrack);

            userFieldsControlCD.UsedAsFilter = true;
            userFieldsControlCD.SetFields(db, db.Master.UserCDFields);

            userFieldsControlTrack.UsedAsFilter = true;
            userFieldsControlTrack.SetFields(db, db.Master.UserTrackFields);

            comboBoxSamplerCD.Items.Add("<" + StringTable.All + ">");
            comboBoxSamplerCD.Items.Add(StringTable.NoSampler);
            comboBoxSamplerCD.Items.Add(StringTable.OnlySampler);
            comboBoxSamplerCD.SelectedIndex = 0;

            comboBoxOriginalCD.Items.Add("<" + StringTable.All + ">");
            comboBoxOriginalCD.Items.Add(StringTable.NoOriginalCDs);
            comboBoxOriginalCD.Items.Add(StringTable.OnlyOriginalCDs);
            comboBoxOriginalCD.SelectedIndex = 0;

            comboBoxCategoryCD.Items.Add("<" + StringTable.All + ">");
            db.AddCategoriesToComboBox(comboBoxCategoryCD);
            comboBoxCategoryCD.SelectedIndex = 0;

            comboBoxMediumCD.Items.Add("<" + StringTable.All + ">");
            db.AddMediaToComboBox(comboBoxMediumCD);
            comboBoxMediumCD.SelectedIndex = 0;

            db.AddLabelsToComboBox(comboBoxLabelCD);

            comboBoxCategoryTrack.Items.Add("<" + StringTable.All + ">");
            db.AddCategoriesToComboBox(comboBoxCategoryTrack);
            comboBoxCategoryTrack.SelectedIndex = 0;

            SetCondition();
        }

        private void SetCondition()
        {
            if (condition == null)
                return;

            SetCDCondition();
            SetTrackCondition();
        }

        private void SetCDCondition()
        {
            SetStringField(Field.ArtistCDName, textBoxArtistCD);
            SetStringField(Field.Title, textBoxTitleCD);
            SetStringField(Field.ComposerCDName, textBoxComposerCD);
            SetStringField(Field.TotalLength, Operator.GreaterEqual, textBoxTotalLengthFromCD);
            SetStringField(Field.TotalLength, Operator.LessEqual, textBoxTotalLengthToCD);
            SetStringField(Field.NumberOfTracks, Operator.GreaterEqual, textBoxNumberOfTracksFromCD);
            SetStringField(Field.NumberOfTracks, Operator.LessEqual, textBoxNumberOfTracksToCD);
            SetStringField(Field.Date, Operator.GreaterEqual, textBoxDateFromCD);
            SetStringField(Field.Date, Operator.LessEqual, textBoxDateToCD);
            SetStringField(Field.YearRecorded, Operator.GreaterEqual, textBoxYearRecordedFromCD);
            SetStringField(Field.YearRecorded, Operator.LessEqual, textBoxYearRecordedToCD);
            SetRatingField(Field.Rating, Operator.GreaterEqual, ratingControlFromCD);
            SetRatingField(Field.Rating, Operator.LessEqual, ratingControlToCD);
            SetStringField(Field.ArchiveNumber, Operator.GreaterEqual, textBoxArchiveNumberFromCD);
            SetStringField(Field.ArchiveNumber, Operator.LessEqual, textBoxArchiveNumberToCD);
            SetStringField(Field.Price, Operator.GreaterEqual, textBoxPriceFromCD);
            SetStringField(Field.Price, Operator.LessEqual, textBoxPriceToCD);

            SetUserField(Field.User1, userFieldsControlCD, 0);
            SetUserField(Field.User2, userFieldsControlCD, 1);
            SetUserField(Field.User3, userFieldsControlCD, 2);
            SetUserField(Field.User4, userFieldsControlCD, 3);
            SetUserField(Field.User5, userFieldsControlCD, 4);

            SetListField(Field.Sampler, comboBoxSamplerCD);
            SetListField(Field.OriginalCD, comboBoxOriginalCD);
            SetListField(Field.Category, comboBoxCategoryCD);
            SetListField(Field.Medium, comboBoxMediumCD);
            SetListField(Field.Label, comboBoxLabelCD);
            SetStringField(Field.UPC, textBoxUPCCD);
            SetStringField(Field.Homepage, textBoxURLCD);
            SetStringField(Field.Copyright, textBoxCopyrightCD);
            SetStringField(Field.Comment, textBoxCommentCD);
            SetStringField(Field.CDCoverFront, textBoxFrontCoverCD);
            SetStringField(Field.CDCoverBack, textBoxBackCoverCD);
            SetStringField(Field.CDCoverLabel, textBoxCDLabelCD);
            SetStringField(Field.Language, textBoxLanguageCD);
            SetStringField(Field.Location, textBoxLocationCD);

            foreach (SingleCondition sc in condition)
            {
                if (sc.Field == Field.Codes)
                    codesCondition.Add(new SingleCondition(sc.Field, sc.Operator, sc.Value, sc.Logical));
                if (sc.Field == Field.TrackCodes)
                    trackCodesCondition.Add(new SingleCondition(sc.Field, sc.Operator, sc.Value, sc.Logical));
            }
        }

        private void SetTrackCondition()
        {
            SetStringField(Field.ArtistTrackName, textBoxArtistTrack);
            SetStringField(Field.TrackTitle, textBoxTrackname);
            SetStringField(Field.ComposerTrackName, textBoxComposerTrack);

            SetStringField(Field.TrackLength, Operator.GreaterEqual, textBoxLengthFromTrack);
            SetStringField(Field.TrackLength, Operator.LessEqual, textBoxLengthToTrack);
            SetStringField(Field.TrackBpm, Operator.GreaterEqual, textBoxBPMFromTrack);
            SetStringField(Field.TrackBpm, Operator.LessEqual, textBoxBPMToTrack);

            SetStringField(Field.TrackComment, textBoxCommentTrack);
            SetStringField(Field.TrackLyrics, textBoxLyricsTrack);
            SetStringField(Field.TrackSoundFile, textBoxSoundfileTrack);

            SetStringField(Field.TrackYearRecorded, Operator.GreaterEqual, textBoxYearRecordedFromTrack);
            SetStringField(Field.TrackYearRecorded, Operator.LessEqual, textBoxYearRecordedToTrack);
            SetRatingField(Field.TrackRating, Operator.GreaterEqual, ratingControlFromTrack);
            SetRatingField(Field.TrackRating, Operator.LessEqual, ratingControlToTrack);

            SetUserField(Field.TrackUser1, userFieldsControlTrack, 0);
            SetUserField(Field.TrackUser2, userFieldsControlTrack, 1);
            SetUserField(Field.TrackUser3, userFieldsControlTrack, 2);
            SetUserField(Field.TrackUser4, userFieldsControlTrack, 3);
            SetUserField(Field.TrackUser5, userFieldsControlTrack, 4);

            SetListField(Field.TrackCategory, comboBoxCategoryTrack);
            SetStringField(Field.TrackLanguage, textBoxLanguageTrack);
        }

        private void SetUserField(Field field, UserFieldsControl userFieldControl, int fieldIndex)
        {
            SingleCondition cond = condition.FindByField(field);
            if (cond != null)
            {
                userFieldControl.SetValue(fieldIndex, (string)condition.FindByField(field).Value);
            }
        }

        private void SetRatingField(Field field, Operator op, RatingControl ratingControl)
        {
            SingleCondition cond = condition.FindByField(field, op);
            if (cond != null)
                ratingControl.Value = (int)cond.Value;
        }

        private void SetListField(Field field, ComboBox comboBox)
        {
            SingleCondition cond = condition.FindByField(field);
            if (cond != null)
            {
                switch (field)
                {
                    case Field.Sampler:
                    case Field.OriginalCD:
                        comboBox.SelectedIndex = (int)cond.Value + 1;
                        break;
                    default:
                        comboBox.Text = (string)cond.Value;
                        break;
                }
            }
        }

        private void SetStringField(Field field, Operator op, TextBox textBox)
        {
            SingleCondition cond = condition.FindByField(field, op);
            if (cond != null)
            {
                switch (field)
                {
                    case Field.TotalLength:
                    case Field.TrackLength:
                        textBox.Text = Misc.GetLongTimeString((int)cond.Value);
                        break;
                    case Field.Date:
                        ((TextBoxDate)textBox).Value = (string)cond.Value;
                        break;
                    case Field.Price:
                        ((TextBoxCurrency)textBox).Value = (int)cond.Value;
                        break;
                    default:
                        textBox.Text = cond.Value.ToString();
                        break;
                }
            }
        }

        private void SetStringField(Field field, TextBox textBox)
        {
            if (condition.FindByField(field) != null)
            {
                switch (field)
                {
                    default:
                        textBox.Text = (string)condition.FindByField(field).Value;
                        break;
                }
            }
        }

        public Condition GetCondition()
        {
            Operator stringOperator;

            if (checkBoxExactMatch.Checked)
                stringOperator = Operator.StartsWith;
            else
                stringOperator = Operator.Contains;

            Condition cond = new Condition();

            GetCDCondition(cond, stringOperator);
            GetTrackCondition(cond, stringOperator);

            return cond;
        }

        private void GetCDCondition(Condition cond, Operator stringOperator)
        {
            AddToCondition(cond, Field.ArtistCDName, stringOperator, textBoxArtistCD);
            AddToCondition(cond, Field.Title, stringOperator, textBoxTitleCD);
            AddToCondition(cond, Field.ComposerCDName, stringOperator, textBoxComposerCD);
            AddToCondition(cond, Field.TotalLength, Operator.GreaterEqual, textBoxTotalLengthFromCD);
            AddToCondition(cond, Field.TotalLength, Operator.LessEqual, textBoxTotalLengthToCD);
            AddToCondition(cond, Field.NumberOfTracks, Operator.GreaterEqual, textBoxNumberOfTracksFromCD);
            AddToCondition(cond, Field.NumberOfTracks, Operator.LessEqual, textBoxNumberOfTracksToCD);
            AddToCondition(cond, Field.Date, Operator.GreaterEqual, textBoxDateFromCD);
            AddToCondition(cond, Field.Date, Operator.LessEqual, textBoxDateToCD);
            AddToCondition(cond, Field.YearRecorded, Operator.GreaterEqual, textBoxYearRecordedFromCD);
            AddToCondition(cond, Field.YearRecorded, Operator.LessEqual, textBoxYearRecordedToCD);
            AddToCondition(cond, Field.Rating, Operator.GreaterEqual, ratingControlFromCD);
            AddToCondition(cond, Field.Rating, Operator.LessEqual, ratingControlToCD);
            AddToCondition(cond, Field.ArchiveNumber, Operator.GreaterEqual, textBoxArchiveNumberFromCD);
            AddToCondition(cond, Field.ArchiveNumber, Operator.LessEqual, textBoxArchiveNumberToCD);
            AddToCondition(cond, Field.Price, Operator.GreaterEqual, textBoxPriceFromCD);
            AddToCondition(cond, Field.Price, Operator.LessEqual, textBoxPriceToCD);

            AddToCondition(cond, Field.User1, stringOperator, userFieldsControlCD, 0);
            AddToCondition(cond, Field.User2, stringOperator, userFieldsControlCD, 1);
            AddToCondition(cond, Field.User3, stringOperator, userFieldsControlCD, 2);
            AddToCondition(cond, Field.User4, stringOperator, userFieldsControlCD, 3);
            AddToCondition(cond, Field.User5, stringOperator, userFieldsControlCD, 4);

            AddToCondition(cond, Field.Sampler, Operator.Equal, comboBoxSamplerCD);
            AddToCondition(cond, Field.OriginalCD, Operator.Equal, comboBoxOriginalCD);
            AddToCondition(cond, Field.Category, Operator.Equal, comboBoxCategoryCD);
            AddToCondition(cond, Field.Medium, Operator.Equal, comboBoxMediumCD);
            AddToCondition(cond, Field.Label, stringOperator, comboBoxLabelCD);
            AddToCondition(cond, Field.UPC, stringOperator, textBoxUPCCD);
            AddToCondition(cond, Field.Homepage, stringOperator, textBoxURLCD);
            AddToCondition(cond, Field.Copyright, stringOperator, textBoxCopyrightCD);
            AddToCondition(cond, Field.Comment, stringOperator, textBoxCommentCD);
            AddToCondition(cond, Field.CDCoverFront, stringOperator, textBoxFrontCoverCD);
            AddToCondition(cond, Field.CDCoverBack, stringOperator, textBoxBackCoverCD);
            AddToCondition(cond, Field.CDCoverLabel, stringOperator, textBoxCDLabelCD);
            AddToCondition(cond, Field.Language, stringOperator, textBoxLanguageCD);
            AddToCondition(cond, Field.Location, stringOperator, textBoxLocationCD);

            foreach (SingleCondition sc in codesCondition)
                cond.Add(new SingleCondition(sc.Field, sc.Operator, sc.Value, sc.Logical));
        }

        private void GetTrackCondition(Condition cond, Operator stringOperator)
        {
            AddToCondition(cond, Field.ArtistTrackName, stringOperator, textBoxArtistTrack);
            AddToCondition(cond, Field.TrackTitle, stringOperator, textBoxTrackname);
            AddToCondition(cond, Field.ComposerTrackName, stringOperator, textBoxComposerTrack);

            AddToCondition(cond, Field.TrackLength, Operator.GreaterEqual, textBoxLengthFromTrack);
            AddToCondition(cond, Field.TrackLength, Operator.LessEqual, textBoxLengthToTrack);
            AddToCondition(cond, Field.TrackBpm, Operator.GreaterEqual, textBoxBPMFromTrack);
            AddToCondition(cond, Field.TrackBpm, Operator.LessEqual, textBoxBPMToTrack);

            AddToCondition(cond, Field.TrackComment, stringOperator, textBoxCommentTrack);
            AddToCondition(cond, Field.TrackLyrics, stringOperator, textBoxLyricsTrack);
            AddToCondition(cond, Field.TrackSoundFile, stringOperator, textBoxSoundfileTrack);

            AddToCondition(cond, Field.TrackYearRecorded, Operator.GreaterEqual, textBoxYearRecordedFromTrack);
            AddToCondition(cond, Field.TrackYearRecorded, Operator.LessEqual, textBoxYearRecordedToTrack);
            AddToCondition(cond, Field.TrackRating, Operator.GreaterEqual, ratingControlFromTrack);
            AddToCondition(cond, Field.TrackRating, Operator.LessEqual, ratingControlToTrack);

            AddToCondition(cond, Field.TrackUser1, stringOperator, userFieldsControlTrack, 0);
            AddToCondition(cond, Field.TrackUser2, stringOperator, userFieldsControlTrack, 1);
            AddToCondition(cond, Field.TrackUser3, stringOperator, userFieldsControlTrack, 2);
            AddToCondition(cond, Field.TrackUser4, stringOperator, userFieldsControlTrack, 3);
            AddToCondition(cond, Field.TrackUser5, stringOperator, userFieldsControlTrack, 4);

            AddToCondition(cond, Field.TrackCategory, Operator.Equal, comboBoxCategoryTrack);
            AddToCondition(cond, Field.TrackLanguage, stringOperator, textBoxLanguageTrack);

            foreach (SingleCondition sc in trackCodesCondition)
                cond.Add(new SingleCondition(sc.Field, sc.Operator, sc.Value, sc.Logical));
        }

        private void AddToCondition(Condition cond, Field field, Operator stringOperator, UserFieldsControl userFieldControl, int fieldIndex)
        {
            if (!string.IsNullOrEmpty(userFieldControl.GetValue(fieldIndex)))
            {
                if (dataBase.Master.UserCDFields[fieldIndex].Type == UserFieldType.Text)
                    cond.Add(new SingleCondition(field, stringOperator, userFieldControl.GetValue(fieldIndex)));
                else
                    cond.Add(new SingleCondition(field, Operator.Equal, userFieldControl.GetValue(fieldIndex)));
            }
        }

        private void AddToCondition(Condition cond, Field field, Operator op, TextBox textBox)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                switch (field)
                {
                    case Field.TotalLength:
                    case Field.TrackLength:
                        cond.Add(new SingleCondition(field, op, Misc.ParseTimeString(textBox.Text)));
                        break;
                    case Field.Date:
                        cond.Add(new SingleCondition(field, op, ((TextBoxDate)textBox).Value));
                        break;
                    case Field.Price:
                        cond.Add(new SingleCondition(field, op, ((TextBoxCurrency)textBox).Value));
                        break;
                    case Field.ArchiveNumber:
                        {
                            if (Settings.Current.SortArchiveNumberNumeric)
                            {
                                int number = 0;
                                Int32.TryParse(textBox.Text, out number);
                                cond.Add(new SingleCondition(field, op, number));
                            }
                            else
                            {
                                cond.Add(new SingleCondition(field, op, textBox.Text));
                            }

                            break;
                        }
                    case Field.NumberOfTracks:
                        {
                            int number = 0;
                            Int32.TryParse(textBox.Text, out number);
                            cond.Add(new SingleCondition(field, op, number));
                            break;
                        }
                    case Field.TrackBpm:
                        {
                            int bpm = 0;
                            Int32.TryParse(textBox.Text, out bpm);
                            cond.Add(new SingleCondition(field, op, bpm));
                            break;
                        }
                    case Field.YearRecorded:
                        {
                            int year = 0;
                            Int32.TryParse(textBox.Text, out year);
                            cond.Add(new SingleCondition(field, op, year));
                            break;
                        }
                    default:
                        cond.Add(new SingleCondition(field, op, textBox.Text));
                        break;
                }
            }
        }

        private void AddToCondition(Condition cond, Field field, Operator op, ComboBox comboBox)
        {
            if (field == Field.Label)
            {
                if (!string.IsNullOrEmpty(comboBox.Text))
                    cond.Add(new SingleCondition(field, op, comboBox.Text));
                return;
            }

            if (comboBox.SelectedIndex > 0)
            {
                switch (field)
                {
                    case Field.Sampler:
                    case Field.OriginalCD:
                        cond.Add(new SingleCondition(field, op, comboBox.SelectedIndex-1));
                        break;
                    default:
                        cond.Add(new SingleCondition(field, op, comboBox.Text));
                        break;
                }
            }
        }

        private void AddToCondition(Condition cond, Field field, Operator op, RatingControl ratingControl)
        {
            if (ratingControl.Value > 0)
                cond.Add(new SingleCondition(field, op, ratingControl.Value));
        }

        private void buttonCodesCD_Click(object sender, EventArgs e)
        {
            EditCodesFilter(false);
        }

        private void buttonCodesTrack_Click(object sender, EventArgs e)
        {
            EditCodesFilter(true);
        }

        private void EditCodesFilter(bool trackCodes)
        {
            FormSearchCodes formSearchCodes = new FormSearchCodes(this.dataBase, trackCodes ? this.trackCodesCondition : this.codesCondition, trackCodes);

            if (formSearchCodes.ShowDialog(this) == DialogResult.OK)
            {
                if (trackCodes)
                    trackCodesCondition.Clear();
                else
                    codesCondition.Clear();

                for (int i = 0; i < formSearchCodes.CodeSearchItems.Count; i++)
                {
                    FormSearchCodes.CodeSearchItem item = formSearchCodes.CodeSearchItems[i];

                    Operator fieldOperator;

                    if (item.ComboNot.SelectedIndex > 0)
                        fieldOperator = Operator.NotContains;
                    else
                        fieldOperator = Operator.Contains;

                    if (item.ComboCode.SelectedItem != null && item.ComboCode.SelectedItem.ToString() != "")
                    {
                        string code = item.ComboCode.SelectedItem.ToString().Left(1);

                        SingleCondition newCond = new SingleCondition(trackCodes ? Field.TrackCodes : Field.Codes, fieldOperator, code);

                        if (i > 0)
                        {
                            if (formSearchCodes.CodeSearchItems[i - 1].ComboAndOr.SelectedIndex == 1)
                                newCond.Logical = Logical.Or;
                        }

                        if (trackCodes)
                            trackCodesCondition.Add(newCond);
                        else
                            codesCondition.Add(newCond);
                    }
                }
            }
        }
    }
}
