using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Big3.Hitbase.DataBaseEngine
{
    public partial class FormConvertProgress : Form
    {
        public FormConvertProgress()
        {
            InitializeComponent();
        }

        public string CurrentTable
        {
            set
            {
                labelTable.Text = value;
            }
        }

        public int MaximumValue
        {
            set
            {
                progressBar.Maximum = value;
            }
        }

        public int Value
        {
            get
            {
                return progressBar.Value;
            }
            set
            {
                progressBar.Value = value;
            }
        }
    }
}
