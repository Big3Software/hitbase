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
    public partial class FormShowCrashLog : Form
    {
        public FormShowCrashLog()
        {
            InitializeComponent();
        }

        public string Details
        {
            set
            {
                textBoxDetails.Text = value;
            }
        }
    }
}
