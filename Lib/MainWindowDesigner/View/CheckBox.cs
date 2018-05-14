using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.MainWindowDesigner.Model;
using System.Windows.Controls;

namespace Big3.Hitbase.MainWindowDesigner.View
{
    public class MyCheckBox : CheckBox, IHitbaseControl
    {
        HitbaseControl hitbaseControl = null;

        public MyCheckBox(HitbaseControl ctl)
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
