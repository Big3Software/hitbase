using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Big3.Hitbase.Miscellaneous
{
    public class BindingDirect : Binding
    {
        public BindingDirect()
        {
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }

        public BindingDirect(string path) : base(path)
        {
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
    }
}
