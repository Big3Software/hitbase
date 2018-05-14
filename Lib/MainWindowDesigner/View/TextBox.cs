using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.MainWindowDesigner.Model;
using System.Windows.Controls;

namespace Big3.Hitbase.MainWindowDesigner.View
{
    public class MyTextBox : /*TODO_WPF!!!!!!!!!!!!!AutoComplete*/TextBox, IHitbaseControl
    {
        HitbaseTextBox hitbaseControl = null;

        public MyTextBox(HitbaseTextBox ctl)
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
