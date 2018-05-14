using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Controls;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.Controls
{
    public partial class TextBoxAutoComplete : TextBox
    {
        public TextBoxAutoComplete()
        {
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

        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key < System.Windows.Input.Key.A || e.Key > System.Windows.Input.Key.Z)
                return;

            bool somethingFound = false;
            string foundText = null;

            string newText = Text;

            switch (AutoCompleteTextBoxType)
            {
                case AutoCompleteTextBoxType.PersonGroup:
                    {
                        if (!Settings.Current.AutoCompleteArtist)
                            return;

                        somethingFound = DataBase.FindArtist(newText, out foundText);
                        break;
                    }
                case AutoCompleteTextBoxType.Title:
                    {
                        if (!Settings.Current.AutoCompleteCDTitle)
                            return;

                        somethingFound = DataBase.FindCDTitle(newText, out foundText);
                        break;
                    }
                case AutoCompleteTextBoxType.TrackTitle:
                    {
                        if (!Settings.Current.AutoCompleteTrackname)
                            return;

                        somethingFound = DataBase.FindTrackTitle(newText, out foundText);
                        break;
                    }
                default:
                    break;
            }

            if (somethingFound && !string.IsNullOrEmpty(foundText))
            {
                string saveText = Text;
                Text = foundText;
                SelectionStart = saveText.Length;
                SelectionLength = Text.Length - saveText.Length;
                e.Handled = true;
            }
        }
    }
}
