﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Big3.Hitbase.RecordMedium
{
    public partial class RecordFormProgress : Form
    {
        public RecordFormProgress()
        {
            InitializeComponent();
        }

        public bool Canceled { get; set; }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Canceled = true;
        }
    }
}
