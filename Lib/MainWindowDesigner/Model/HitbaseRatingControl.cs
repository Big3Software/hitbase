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
    public class HitbaseRating : HitbaseControl
    {
        [NonSerialized]
        private MyRatingControl ratingControl;

        public HitbaseRating(MainCDUserControl dlg)
            : base(dlg)
        {
            ratingControl = new MyRatingControl(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // HelpLine properties

        public override int Baseline
        {
            get
            {
                /*TODO_WPF!!!!!!!!!!!!!!!!int ascent = Control.Font.FontFamily.GetCellAscent(Control.Font.Style);
                int ascentPixel = (int)(Control.Font.Size * ascent / Control.Font.FontFamily.GetEmHeight(Control.Font.Style) + 0.5); 

                return ascentPixel+3;*/
                return 0;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////

        protected override FrameworkElement Control
        {
            get
            {
                return ratingControl;
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
            Object data = GetDataFromCD();

            ratingControl.Value = Convert.ToInt32(data);
        }

        /// <summary>
        /// Läd die aktuellen Daten aus dem Control und speichert sie.
        /// </summary>
        public override void SaveControlData()
        {
            SaveDataToCD(ratingControl.Value);
        }

        public override string ControlName
        {
            get
            {
                return "Rating";
            }
        }
    }
}
