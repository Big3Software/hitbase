using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.Controls
{
    public partial class TextBoxCurrency : TextBox
    {
        public TextBoxCurrency()
        {
            InitializeComponent();
        }

        public TextBoxCurrency(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
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

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            if (!string.IsNullOrEmpty(Text))
                Value = Misc.ParseCurrencyValue(Text);
        }
    }
}
