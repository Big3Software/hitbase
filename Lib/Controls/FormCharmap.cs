using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Big3.Hitbase.Controls
{
    public partial class FormCharmap : Form
    {
        private TextBox textBox;

        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();

        public FormCharmap()
        {
            // Aktuellen Focus ermitteln
            if (GetFocus() != IntPtr.Zero)
            {
                Control ctl = Control.FromHandle(GetFocus());

                if (ctl is TextBox)
                {
                    textBox = ctl as TextBox;
                }
            }

            InitializeComponent();
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            if (textBox == null)
                return;

            string text = textBox.Text;

            int textBoxSelStart = textBox.SelectionStart;

            text = text.Insert(textBox.SelectionStart, ctlCharmap.CharSelected.ToString());

            textBox.Text = text;
            textBox.SelectionStart = textBoxSelStart + 1;
            textBox.SelectionLength = 0;
        }
    }
}