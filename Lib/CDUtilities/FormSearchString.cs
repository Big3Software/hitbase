using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormSearchString : Form
    {
        public EventHandler SearchClicked;

        public FormSearchString()
        {
            InitializeComponent();
        }

        public string SearchString
        {
            get
            {
                return textBoxSearchFor.Text;
            }
            set
            {
                textBoxSearchFor.Text = value;
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (SearchClicked != null)
                SearchClicked(this, e);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
