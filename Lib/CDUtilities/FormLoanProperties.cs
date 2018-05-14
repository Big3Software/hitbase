using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.DataBaseEngine.CDDataSetTableAdapters;
using Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormLoanProperties : Form
    {
        public bool BringBackClicked = false;

        public bool IsNew = false;

        LoanedCDDataSet.LoanedCDRow loanedCDRow = null;

        public FormLoanProperties()
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);

            comboBoxLoanedTo.Items.AddRange(Settings.Current.LoanedToRecentList.ToArray());
        }

        public FormLoanProperties(DataBase dataBase, LoanedCDDataSet.LoanedCDRow loanedCD)
            : this()
        {
            loanedCDRow = loanedCD;

            CDTableAdapter cdta = new CDTableAdapter(dataBase);
            PersonGroupTableAdapter personGroupta = new PersonGroupTableAdapter(dataBase);
            CDDataSet.CDDataTable cd = cdta.GetDataById(loanedCDRow.CDID);
            PersonGroupDataSet.PersonGroupDataTable personGroup = personGroupta.GetDataById(cd[0].ArtistID);

            textBoxArtist.Text = personGroup[0].Name;
            textBoxTitle.Text = cd[0].Title;

            comboBoxLoanedTo.Text = loanedCDRow.LoanedTo;

            checkBoxActivateBringBack.Checked = !loanedCDRow.IsReturnDateNull();
            if (!loanedCDRow.IsReturnDateNull())
                dateTimePickerBringBack.Value = loanedCDRow.ReturnDate;

            if (!loanedCDRow.IsLoanedDateNull())
                dateTimePickerLoaned.Value = loanedCDRow.LoanedDate;
            textBoxComment.Text = loanedCDRow.Comment;
        }

        private void FormLoanProperties_Load(object sender, EventArgs e)
        {
            if (IsNew && !string.IsNullOrEmpty(Configuration.Settings.Current.LoanedDefaultTimeSpan))
            {
                comboBoxGiveBack.Text = Configuration.Settings.Current.LoanedDefaultTimeSpan;
                dateTimePickerBringBack.Value = SetGiveBackDate(dateTimePickerLoaned.Value, comboBoxGiveBack.Text);
            }

            UpdateWindowState();
        }

        private void checkBoxActivateBringBack_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            dateTimePickerBringBack.Enabled = checkBoxActivateBringBack.Checked;
            comboBoxGiveBack.Enabled = checkBoxActivateBringBack.Checked;
            buttonOK.Enabled = comboBoxLoanedTo.Text.Length > 0;
        }

        private void buttonBringBack_Click(object sender, EventArgs e)
        {
            string str = string.Format(StringTable.GiveBack, textBoxArtist.Text, textBoxTitle.Text);

            if (MessageBox.Show(str, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DialogResult = DialogResult.OK;

                BringBackClicked = true;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (Settings.Current.LoanedToRecentList.IndexOf(comboBoxLoanedTo.Text) < 0)
                Settings.Current.LoanedToRecentList.Add(comboBoxLoanedTo.Text);

            if (!string.IsNullOrEmpty(comboBoxGiveBack.Text))
                Settings.Current.LoanedDefaultTimeSpan = comboBoxGiveBack.Text;

            if (loanedCDRow != null)
            {
                loanedCDRow.LoanedTo = comboBoxLoanedTo.Text;
                loanedCDRow.LoanedDate = dateTimePickerLoaned.Value;

                if (checkBoxActivateBringBack.Checked)
                    loanedCDRow.ReturnDate = dateTimePickerBringBack.Value;
                else
                    loanedCDRow.SetReturnDateNull();

                loanedCDRow.Comment = textBoxComment.Text;
            }
        }

        private void comboBoxLoanedTo_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void comboBoxGiveBack_SelectedIndexChanged(object sender, EventArgs e)
        {
            dateTimePickerBringBack.Value = SetGiveBackDate(dateTimePickerLoaned.Value, comboBoxGiveBack.Text);
        }

        private DateTime SetGiveBackDate(DateTime startDateTime, string timeSpan)
        {
            string[] parts = timeSpan.Split(' ');

            if (parts.Length < 2)
                return startDateTime;

            int value = 0;
            try
            {
                value = Convert.ToInt32(parts[0]);
            }
            catch
            {
            }

            DateTime endDateTime = startDateTime;

            string timeValue = parts[1].ToLower();
            if (timeValue.StartsWith("tag"))
                endDateTime = endDateTime.AddDays(value);
            if (timeValue.StartsWith("woche"))
                endDateTime = endDateTime.AddDays(value * 7);
            if (timeValue.StartsWith("monat"))
                endDateTime = endDateTime.AddMonths(value);

            return endDateTime;
        }

        private void comboBoxGiveBack_Leave(object sender, EventArgs e)
        {
            dateTimePickerBringBack.Value = SetGiveBackDate(dateTimePickerLoaned.Value, comboBoxGiveBack.Text);
        }
    }
}
