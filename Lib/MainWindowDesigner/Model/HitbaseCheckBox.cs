using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.MainWindowDesigner.Controls;
using Big3.Hitbase.MainWindowDesigner.View;
using System.Windows;

namespace Big3.Hitbase.MainWindowDesigner.Model
{
    [Serializable]
    public class HitbaseCheckBox : HitbaseControl
    {
        [NonSerialized]
        protected MyCheckBox checkBox;

        public HitbaseCheckBox(MainCDUserControl dlg) : base(dlg)
        {
            checkBox = new MyCheckBox(this);
            checkBox.Checked += new System.Windows.RoutedEventHandler(checkBox_Checked);
        }

        void checkBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (hitbaseMainWindowControl.CurrentUpdatingAllControls)
                return;

            SaveControlData();

            /*TODO_WPF!!!!!!!!!!!!!!!!!if (Field == Big3.Hitbase.DataBaseEngine.Field.Sampler && hitbaseMainWindowControl.OnCDSamplerChanged != null)
                hitbaseMainWindowControl.OnCDSamplerChanged(this, new EventArgs());*/
        }

        protected override FrameworkElement Control
        {
            get
            {
                return checkBox;
            }
        }

        public override void SetDesignMode(bool activateDesignMode)
        {
            base.SetDesignMode(activateDesignMode);
        }

        /// <summary>
        /// Füllt die Daten in das Control.
        /// </summary>
        public override void UpdateControlData()
        {
            object o = GetDataFromCD();

            if (Convert.ToInt32(o) != 0)
                checkBox.IsChecked = true;
            else
                checkBox.IsChecked = false;
        }

        /// <summary>
        /// Läd die aktuellen Daten aus dem Control und speichert sie.
        /// </summary>
        public override void SaveControlData()
        {
            SaveDataToCD(checkBox.IsChecked.Value);
        }

        private string text;
        [Category("Hitbase")]
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                checkBox.Content = value;
                text = value;
            }
        }


        public override int Baseline
        {
            get
            {
                /*TODO_WPF!!!!!!!!!!!!
                int ascent = Control.Font.FontFamily.GetCellAscent(Control.Font.Style);
                int ascentPixel = (int)(Control.Font.Size * ascent / Control.Font.FontFamily.GetEmHeight(Control.Font.Style) + 0.5);

                return Height / 2 - Control.Font.Height / 2 + ascentPixel + 1;*/
                return 0;
            }
        }

        public override string ControlName
        {
            get
            {
                return "CheckBox";
            }
        }
    }
}
