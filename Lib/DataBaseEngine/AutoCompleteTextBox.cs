using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Big3.Hitbase.DataBaseEngine
{
    public enum AutoCompleteTextBoxType
    {
        Unknown,
        PersonGroup,
        Title,
        TrackTitle
    }

    public partial class AutoCompleteTextBox : TextBox
    {
        public AutoCompleteTextBox()
        {
            InitializeComponent();
        }

        private DataBase dataBase;

        public DataBase DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }

        private AutoCompleteTextBoxType autoCompleteTextBoxType;

        public AutoCompleteTextBoxType AutoCompleteTextBoxType
        {
            get { return autoCompleteTextBoxType; }
            set { autoCompleteTextBoxType = value; }
        }

        protected override bool ProcessKeyMessage(ref Message m)
        {
            return base.ProcessKeyMessage(ref m);
        }

        const int WM_CHAR = 258; 

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_CHAR)
            {
                if ((int)m.WParam >= 32 && (int)m.WParam <= 255)
                {
                    bool somethingFound = false;
                    string foundText = null;

                    switch (AutoCompleteTextBoxType)
                    {
                        case AutoCompleteTextBoxType.PersonGroup:
                            somethingFound = DataBase.FindArtist(Text, out foundText);
                            break;
                        case AutoCompleteTextBoxType.Title:
                            somethingFound = DataBase.FindCDTitle(Text, out foundText);
                            break;
                        case AutoCompleteTextBoxType.TrackTitle:
                            somethingFound = DataBase.FindTrackTitle(Text, out foundText);
                            break;
                        default:
                            break;
                    }

                    if (somethingFound && !string.IsNullOrEmpty(foundText))
                    {
                        string saveText = Text;
                        Text = foundText;
                        SelectionStart = saveText.Length;
                        SelectionLength = Text.Length - saveText.Length;
                    }
                }
            }
        }
    }
}
