using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Big3.Hitbase.Miscellaneous;
using System.Windows;

namespace Big3.Hitbase.Controls
{
    public class TextBoxCurrencyWPF : TextBox
    {
        public TextBoxCurrencyWPF()
        {

        }

        // Der Wert des Preises (z.B. 140 = 1,40€)
        public int Value
        {
            get
            {
                return Misc.ParseCurrencyValue(this.Text);
            }
            set
            {
                if (value > 0)
                    this.Text = Misc.FormatCurrencyValue(value);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (!string.IsNullOrEmpty(Text))
                Value = Misc.ParseCurrencyValue(Text);
        }
    }

}
