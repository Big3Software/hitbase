using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Big3.Hitbase.MainWindowDesigner.Controls;
using System.Windows.Controls;
using System.Collections;
using System.Windows;

namespace Big3.Hitbase.MainWindowDesigner.Model
{
    public class HitbaseMainCDControl : HitbaseControl
    {
        private Canvas canvas;

        public HitbaseMainCDControl(MainCDUserControl mainCDUserControl)
        {
            this.canvas = new Canvas();

            this.hitbaseMainWindowControl = mainCDUserControl;
        }

        protected override System.Windows.FrameworkElement Control
        {
            get { return canvas; }
        }

        /// <summary>
        /// Breite des Client-Bereichs des Dialogs.
        /// </summary>
        [Category("Hitbase")]
        public virtual int ClientWidth
        {
            get
            {
                return Width;
            }
            set
            {
                Width = value;
            }
        }

        /// <summary>
        /// Höhe des Client-Bereichs des Dialogs.
        /// </summary>
        [Category("Hitbase")]
        public virtual int ClientHeight
        {
            get
            {
                return Height;
            }
            set
            {
                Height = value;
            }
        }

        public override System.Collections.ArrayList Children
        {
            get
            {
                ArrayList al = new ArrayList();
                foreach (FrameworkElement fe in canvas.Children)
                {
                    IHitbaseControl hlpCtl = fe as IHitbaseControl;
                    if (hlpCtl != null)
                        al.Add(hlpCtl.HitbaseControl);
                }

                return al;
            }
        }

        public override string ControlName
        {
            get { return "model"; }
        }

        public override void UpdateControlData()
        {
        }

        public override void SaveControlData()
        {
        }
    }
}
