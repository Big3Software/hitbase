using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Big3.Hitbase.Controls
{
    public partial class TextBoxCodes : TextBox
    {
        public TextBoxCodes()
        {
            InitializeComponent();
        }

        public TextBoxCodes(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            char upperKey = Char.ToUpper(e.KeyChar);
            if (upperKey > 31 && (upperKey < 'A' || upperKey > 'Z' || Text.IndexOf(upperKey) >= 0 || Text.Length >= 5))
            {
                e.Handled = true;
                return;
            }

            base.OnKeyPress(e);
        }
    }
}
