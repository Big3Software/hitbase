using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Big3.Hitbase.MainWindowDesigner.Model;

namespace Big3.Hitbase.MainWindowDesigner.View
{
    public class MyButton : Button, IHitbaseControl
    {
        HitbaseControl hitbaseControl = null;

        public MyButton(HitbaseControl ctl)
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
