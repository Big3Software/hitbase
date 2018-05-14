using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.Controls
{
    public class TextBoxDateWPF : TextBox
    {
        public TextBoxDateWPF()
        {
        }

        /// <summary>
        /// Der Datumswert (z.B. "20080203" => "03.02.2008" oder "20070000" => "2007")
        /// </summary>
        public string Value
        {
            get
            {
                return Misc.ParseDate(this.Text);
            }
            set
            {
                this.Text = Misc.FormatDate(value);
            }
        }
    }
}
