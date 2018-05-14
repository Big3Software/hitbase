using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormGeneralTextEdit : Form
    {
        private String _nameValue;

        public String NameValue
        {
            get { return _nameValue; }
            set { _nameValue = value; }
        }

        public FormGeneralTextEdit()
        {
            InitializeComponent();
        }

        public FormGeneralTextEdit(string nameTitle, string nameLabel, string nameValue)
            : this()
        {
            this.Text = nameTitle;
            labelName.Text = nameLabel;
            NameValue = nameValue;

            textBoxName.DataBindings.Add("Text", this, "NameValue");
        }
    }
}