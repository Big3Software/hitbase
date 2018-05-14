using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormName : Form
    {
        public FormName()
        {
            InitializeComponent();

            UpdateWindowState();

            FormThemeManager.SetTheme(this);
        }

        public delegate void OnValidateName(object sender, ValidateNameEventArgs e);
        public event OnValidateName ValidateName;

        private string originalName;
        public string NameValue
        {
            get
            {
                return textBoxName.Text;
            }
            set
            {
                originalName = value;
                textBoxName.Text = value;
                UpdateWindowState();
            }
        }

        private void UpdateWindowState()
        {
            buttonOK.Enabled = AllowEmpty || !string.IsNullOrEmpty(NameValue);
        }

        public bool AllowEmpty { get; set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (ValidateName != null)
            {
                ValidateNameEventArgs vnea = new ValidateNameEventArgs();
                vnea.Name = NameValue;
                vnea.OriginalName = originalName;
                ValidateName(this, vnea);
                if (vnea.Cancel)
                {
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }
    }

    public class ValidateNameEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
    }

}
