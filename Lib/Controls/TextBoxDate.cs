using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.Controls
{
    public partial class TextBoxDate : TextBox
    {
        public TextBoxDate()
        {
            InitializeComponent();
        }

        public TextBoxDate(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
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
