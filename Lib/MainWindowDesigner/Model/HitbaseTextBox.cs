using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Reflection;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.MainWindowDesigner.View;
using System.Windows;
using System.Windows.Input;

namespace Big3.Hitbase.MainWindowDesigner.Model
{
    [Serializable]
    public class HitbaseTextBox : HitbaseControl
    {
        [NonSerialized]
        private MyTextBox textBox;

        public HitbaseTextBox(MainCDUserControl dlg)
            : base(dlg)
        {
            textBox = new MyTextBox(this);

            textBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(textBox_TextChanged);
            textBox.LostFocus += new RoutedEventHandler(textBox_LostFocus);
            if (IsInDesignMode)
                textBox.Cursor = Cursors.Arrow;
        }

        public override void ControlCreated()
        {
            base.ControlCreated();

            if (Field == Big3.Hitbase.DataBaseEngine.Field.Codes)
                textBox.CharacterCasing = System.Windows.Controls.CharacterCasing.Upper;
        }

        void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Field == Big3.Hitbase.DataBaseEngine.Field.Price)
            {
                SaveControlData();
                UpdateControlData();
            }

            if (Field == Field.ArtistCDName)
            {
                hitbaseMainWindowControl.UpdateAllControls();
            }

            if (Field == Field.ArchiveNumber)
            {
                if (Configuration.Settings.Current.NoDuplicateArchiveNumbers)
                {
                    int recordFound = hitbaseMainWindowControl.DataBase.CheckArchiveNumber(textBox.Text);

                    if (recordFound > 0 && recordFound != hitbaseMainWindowControl.theCd.ID)           // Doppelte Archiv-Nummer gefunden!!
                    {
                        MessageBox.Show(StringTable.ArchiveNumberAlreadyExists, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
                        textBox.Focus();
                    }
                }
            }
        }

        /// <summary>
        /// Direkt ins HelpLine-Objekt übertragen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SaveControlData();
        }

        protected override FrameworkElement Control
        {
            get
            {
                return textBox;
            }
        }

        private bool multiline;
        [Description("Multiline")]
        [Category("Hitbase")]
        public bool Multiline
        {
            get
            {
                return multiline;
            }
            set
            {
                if (value == true)
                {
                    textBox.TextWrapping = TextWrapping.Wrap;
                    textBox.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
                    textBox.AcceptsReturn = true;
                }

                multiline = value;
            }
        }

        [Category("Hitbase")]
        public override Field Field
        {
            get
            {
                return base.Field;
            }
            set
            {
                base.Field = value;
                /*TODO_WPF!!!!!!!!!!!!!!!!!!if (value == Field.ArtistCDName)
                    textBox.AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;
                if (value == Field.ComposerCDName)
                    textBox.AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;
                if (value == Field.Title)
                    textBox.AutoCompleteTextBoxType = AutoCompleteTextBoxType.Title;
                if (value == Field.TrackTitle)
                    textBox.AutoCompleteTextBoxType = AutoCompleteTextBoxType.TrackTitle;*/
            }
        }

        public override bool Enabled
        {
            get
            {
                if (textBox != null)
                    return !textBox.IsReadOnly;
                else
                    return true;
            }
            set
            {
                textBox.IsReadOnly = value ? false : true;
            }
        }

        public override int Baseline
        {
            get
            {
                /*TODO_WPF!!!!!!!!!!!!!!!!int ascent = Control.Font.FontFamily.GetCellAscent(Control.Font.Style);
                int ascentPixel = (int)(Control.Font.Size * ascent / Control.Font.FontFamily.GetEmHeight(Control.Font.Style) + 0.5);

                return ascentPixel + 6;*/
                return 0;
            }
        }

        public override void SetDesignMode(bool activateDesignMode)
        {
            if (activateDesignMode)
            {
                textBox.Cursor = Cursors.Arrow;
            }
            else
            {
                textBox.Cursor = Cursors.IBeam;
            }

            base.SetDesignMode(activateDesignMode);
        }

        /// <summary>
        /// Füllt die Daten in das Control.
        /// </summary>
        public override void UpdateControlData()
        {
            try
            {
                Object data = GetDataFromCD();

                if (this.Field == Big3.Hitbase.DataBaseEngine.Field.Price)
                {
                    textBox.Text = FormatCurrencyValue((int)data);
                }
                else
                {
                    if (this.Field == Field.Date)
                    {
                        textBox.Text = DataBase.FormatDate((string)data);
                    }
                    else
                    {
                        if (data != null && (!(data is int) || (int)data != 0))
                            textBox.Text = data.ToString();
                        else
                            textBox.Text = "";
                    }
                }
            }
            catch
            {
                textBox.Text = "";
            }
        }

        /// <summary>
        /// Läd die aktuellen Daten aus dem Control und speichert sie.
        /// </summary>
        public override void SaveControlData()
        {
            try
            {
                if (Field == Big3.Hitbase.DataBaseEngine.Field.Price)
                    SaveDataToCD(ParseCurrencyValue(textBox.Text));
                else
                    if (Field == Field.Date)
                        SaveDataToCD(DataBase.ParseDate(textBox.Text));
                    else
                        SaveDataToCD(textBox.Text);
            }
            catch
            {
            }
        }

        private int ParseCurrencyValue(string text)
        {
            int value;

            // Alle Nicht-Zahlen entfernen (bis auf das Komma)
            for (int i = 0; i < text.Length; )
            {
                if (!Char.IsDigit(text[i]) && text[i] != ',')
                    text = text.Remove(i, 1);
                else
                    i++;
            }

            int iKomma = text.IndexOf(",");

            String sEuro = text;

            if (iKomma >= 0)
                sEuro = text.Substring(0, iKomma);

            try
            {
                value = Convert.ToInt32(sEuro) * 100;
            }
            catch
            {
                value = 0;
            }

            if (iKomma >= 0)
            {
                try
                {
                    if (text.Substring(iKomma + 1).Length == 1)
                        value += Convert.ToInt32(text.Substring(iKomma + 1)) * 10;
                    else
                        value += Convert.ToInt32(text.Substring(iKomma + 1));
                }
                catch
                {
                }
            }

            return value;
        }

        private string FormatCurrencyValue(int lValue)
        {
            String str;
            String sEuro;

            sEuro = String.Format("{0}", lValue / 100);
            if (sEuro.Length > 3)
                sEuro = sEuro.Insert(sEuro.Length - 3, ".");
            if (sEuro.Length > 7)
                sEuro = sEuro.Insert(sEuro.Length - 7, ".");

            str = String.Format("{0},{1:D2} {2}", sEuro, lValue % 100, GetCurrencySymbol());

            return str;
        }

        private string GetCurrencySymbol()
        {
            return System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol;
        }

        /*TODO_WPF!!!!!!!!!!!!!!!!!public override GripperDirection AllowedResizeDirection()
        {
            GripperDirection direction = base.AllowedResizeDirection();

            if (Multiline)
                return direction & (GripperDirection.All | GripperDirection.Move);
            else
                return direction & (GripperDirection.Horizontal | GripperDirection.Move);
        }*/

        public override string ControlName
        {
            get
            {
                return "TextBox";
            }
        }
    }
}
