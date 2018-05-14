using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Big3.Hitbase.MainWindowDesigner.View;
using System.Windows;

namespace Big3.Hitbase.MainWindowDesigner.Model
{
    [Serializable]
    public class HitbaseLabel : HitbaseControl
    {
        [NonSerialized]
        private MyLabel label;

        public HitbaseLabel(MainCDUserControl dlg)
            : base(dlg)
        {
            label = new MyLabel(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // Hitbase properties

        private bool showValue;
        [Description("Show value or name of field")]
        [Category("Hitbase")]
        public bool ShowValue
        {
            get
            {
                return showValue;
            }
            set
            {
                showValue = value;
            }
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
                label.Text = value;
                text = value;
            }
        }

        public override int Baseline
        {
            get
            {
                return 0;
/*TODO_WPF!!!!!!!!!!!!!!!                int ascent = Control.Font.FontFamily.GetCellAscent(Control.Font.Style);
                int ascentPixel = (int)(Control.Font.Size * ascent / Control.Font.FontFamily.GetEmHeight(Control.Font.Style) + 0.5);

                return (Height + 1) / 2 - Control.Font.Height / 2 + ascentPixel + 2;*/
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////

        protected override FrameworkElement Control
        {
            get
            {
                return label;
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
            try
            {
                if (Field == Big3.Hitbase.DataBaseEngine.Field.Date)
                    Text = hitbaseMainWindowControl.DataBase.Master.DateName + ":";

                if (Field == Big3.Hitbase.DataBaseEngine.Field.User1)
                    Text = hitbaseMainWindowControl.DataBase.Master.UserCDFields[0].Name + ":";
                if (Field == Big3.Hitbase.DataBaseEngine.Field.User2)
                    Text = hitbaseMainWindowControl.DataBase.Master.UserCDFields[1].Name + ":";
                if (Field == Big3.Hitbase.DataBaseEngine.Field.User3)
                    Text = hitbaseMainWindowControl.DataBase.Master.UserCDFields[2].Name + ":";
                if (Field == Big3.Hitbase.DataBaseEngine.Field.User4)
                    Text = hitbaseMainWindowControl.DataBase.Master.UserCDFields[3].Name + ":";
                if (Field == Big3.Hitbase.DataBaseEngine.Field.User5)
                    Text = hitbaseMainWindowControl.DataBase.Master.UserCDFields[4].Name + ":";

                if (ShowValue)
                {
                    Object data = GetDataFromCD();
    
                    if (data != null && (!(data is int) || (int)data != 0))
                        Text = data.ToString();
                }
            }
            catch
            {
                Text = null;
            }
        }

        /// <summary>
        /// Läd die aktuellen Daten aus dem Control und speichert sie.
        /// </summary>
        public override void SaveControlData()
        {
        }

        public override string ControlName
        {
            get
            {
                return "Label";
            }
        }
    }
}
