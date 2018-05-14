using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Big3.Hitbase.Miscellaneous
{
    public partial class FormWaitAnimated : Form
    {
        public FormWaitAnimated()
        {
            InitializeComponent();
        }

        public string Text
        {
            set
            {
                labelText.Text = value;
            }
        }

        public string Status
        {
            set
            {
                labelStatus.Text = value;
            }
        }
    }
}
