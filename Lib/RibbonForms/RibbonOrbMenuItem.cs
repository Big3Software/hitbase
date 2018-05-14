using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    public class RibbonOrbMenuItem
        : RibbonButton
    {
        #region Fields

        #endregion

        #region Ctor

        public RibbonOrbMenuItem()
        {
            DropDownArrowDirection = RibbonArrowDirection.Left;
            SetDropDownMargin(new Padding(10));
        }

        public RibbonOrbMenuItem(string text)
            : this()
        {
            Text = text;
        }

        #endregion

        #region Props

        public override System.Drawing.Image Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                base.Image = value;

                SmallImage = value;
            }
        }

        [Browsable(false)]
        public override System.Drawing.Image SmallImage
        {
            get
            {
                return base.SmallImage;
            }
            set
            {
                base.SmallImage = value;
            }
        }

        #endregion

        #region Methods

        public override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (RibbonDesigner.Current == null)
            {
                if (Owner.OrbDropDown.LastPoppedMenuItem != null)
                {
                    Owner.OrbDropDown.LastPoppedMenuItem.CloseDropDown();
                }

                ShowDropDown();

                Owner.OrbDropDown.LastPoppedMenuItem = this;
            }
            
        }

        public override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            //if (RibbonDesigner.Current == null)
            //{
            //    CloseDropDown();
            //}
        }

        internal override System.Drawing.Point OnGetDropDownMenuLocation()
        {
            if (Owner == null) return base.OnGetDropDownMenuLocation();

            Rectangle b = Owner.RectangleToScreen(Bounds);
            Rectangle c = Owner.OrbDropDown.RectangleToScreen(Owner.OrbDropDown.ContentRecentItemsBounds);

            return new Point(b.Right, c.Top);
        }

        internal override Size OnGetDropDownMenuSize()
        {
            Rectangle r = Owner.OrbDropDown.ContentRecentItemsBounds;
            r.Inflate(-2, -2);
            return r.Size;
        }

        #endregion
    }
}
