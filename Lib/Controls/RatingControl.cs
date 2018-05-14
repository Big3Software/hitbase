using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Big3.Hitbase.Controls
{
    public partial class RatingControl : Control
    {
        public RatingControl()
        {
            InitializeComponent();

            // Setup the appropriate styles.
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        private int value;

        public int Value
        {
            get { return this.value; }
            set 
            {
                if (value > 6)
                    this.value = 6;
                else
                    this.value = value;
                Invalidate(); 
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            for (int i = 0; i < value; i++)
            {
                pe.Graphics.DrawImage(Images.Star, i * Images.Star.Width, 0, Images.Star.Width, Images.Star.Height);
            }

            for (int i = value; i < 6; i++)
            {
                pe.Graphics.DrawImage(Images.StarDark, i * Images.StarDark.Width, 0, Images.StarDark.Width, Images.StarDark.Height);
            }

            //base.OnPaint(pe);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }

        private void RatingControl_MouseClick(object sender, MouseEventArgs e)
        {
            int newValue = e.X / Images.Star.Width + 1;

            if (newValue == Value && newValue > 0)		// Letzten Stern wieder wegnehmen, wenn ich nochmal auf ihn klicke
                newValue--;

            Value = newValue;
        }
    }
}
