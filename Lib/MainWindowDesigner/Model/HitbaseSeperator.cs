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
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    [Serializable]
    public class HitbaseSeperator : HitbaseControl
    {
        [NonSerialized]
        private MySeperator seperator;

        private bool directionAlreadySet = false;

        public HitbaseSeperator(MainCDUserControl dlg)
            : base(dlg)
        {
            seperator = new MySeperator(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // HelpLine properties

        public override int Baseline
        {
            get
            {
                return 0;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////

        protected override FrameworkElement Control
        {
            get
            {
                return seperator;
            }
        }

        private Direction direction;
        [Category("Hitbase")]
        public Direction Direction
        {
            get { return direction; }
            set 
            {
                Direction oldDirection = Direction;
                int oldWidth = Width;
                int oldHeight = Height;
                direction = value;

                if (directionAlreadySet)
                {
                    if (oldDirection == Direction.Vertical && value == Direction.Horizontal)
                    {
                        Width = oldHeight;
                        Height = 2;
                    }
                    if (oldDirection == Direction.Horizontal && value == Direction.Vertical)
                    {
                        Height = oldWidth;
                        Width = 2;
                    }
                }

                directionAlreadySet = true;
            }
        }

        [Category("Hitbase")]
        public override int Width
        {
            get
            {
                if (Direction == Direction.Horizontal)
                    return base.Width;
                else
                    return 2;
            }
            set
            {
                if (Direction == Direction.Horizontal)
                    base.Width = value;
                else
                    base.Width = 2;
            }
        }

        [Category("Hitbase")]
        public override int Height
        {
            get
            {
                if (Direction == Direction.Vertical)
                    return base.Height;
                else
                    return 2;
            }
            set
            {
                if (Direction == Direction.Vertical)
                    base.Height = value;
                else
                    base.Height = 2;
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

        public override string ControlName
        {
            get
            {
                return "Seperator";
            }
        }
    }
}
