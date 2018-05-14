using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.MainWindowDesigner.View;
using System.Windows;

namespace Big3.Hitbase.MainWindowDesigner.Model
{
    [Serializable]
    public class HitbaseButton : HitbaseControl
    {
        [NonSerialized]
        protected MyButton button = null;

        public HitbaseButton(MainCDUserControl dlg)
            : base(dlg)
        {
            button = new MyButton(this);
            button.Click += new System.Windows.RoutedEventHandler(button_Click);
        }

        void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            /*TODO_WPF!!!!!!!!!!!!!!!if (hitbaseMainWindowControl.OnButtonClicked != null)
            {
                if (sender is MyButton)
                    hitbaseMainWindowControl.OnButtonClicked(((MyButton)sender).HitbaseControl, e);
            }*/
        }

        protected override FrameworkElement Control
        {
            get
            {
                return button;
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
                switch (Field)
                {
                    case Field.ArchiveNumber:
                        button.Content = Images.NextArchiveNumber;
                        button.ToolTip = StringTable.NextFreeArchiveNumber;
                        break;
                    case Field.Homepage:
                        button.Content = Images.Internet;
                        button.ToolTip = StringTable.GoToHomepage;
                        break;
                    case Field.None:
                        break;
                    default:
                        button.Content = "...";
                        break;
                }
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
        }

        /// <summary>
        /// Läd die aktuellen Daten aus dem Control und speichert sie.
        /// </summary>
        public override void SaveControlData()
        {
        }

        public override int Baseline
        {
            get
            {
                /*TODO_WPF!!!!!!!!!!!!!!!int ascent = Control.Font.FontFamily.GetCellAscent(Control.Font.Style);
                int ascentPixel = (int)(Control.Font.Size * ascent / Control.Font.FontFamily.GetEmHeight(Control.Font.Style) + 0.5);

                return Height / 2 - Control.Font.Height / 2 + ascentPixel + 2;*/
                return 0;
            }
        }

        public override string ControlName 
        {
            get
            {
                return "Button";
            }
        }
    }
}
