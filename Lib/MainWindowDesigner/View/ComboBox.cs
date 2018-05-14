using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Big3.Hitbase.MainWindowDesigner.Model;

namespace Big3.Hitbase.MainWindowDesigner.View
{
    public class ComboBoxItem
    {
        public ComboBoxItem(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public String Name;
        public int Id;

        public override string ToString()
        {
            return Name;
        }
    }

    public class MyComboBox : ComboBox, IHitbaseControl
    {
        HitbaseControl hitbaseControl = null;

        public MyComboBox(HitbaseControl ctl)
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
