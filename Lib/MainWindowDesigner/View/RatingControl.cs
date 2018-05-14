using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.MainWindowDesigner.Model;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.MainWindowDesigner.View
{
    public class MyRatingControl : RatingUserControl, IHitbaseControl
    {
        HitbaseRating hitbaseControl = null;

        public MyRatingControl(HitbaseRating ctl) : base()
        {
            hitbaseControl = ctl;
        }

        public HitbaseControl HitbaseControl
        {
            get
            {
                return hitbaseControl;
            }
        }
    }

}
