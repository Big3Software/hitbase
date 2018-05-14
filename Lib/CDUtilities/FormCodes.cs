using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormCodes : Form
    {
        List<Label> labelCodes = new List<Label>();
        List<TextBox> textBoxCodes = new List<TextBox>();
        DataBase dataBase;

        public FormCodes(DataBase db)
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);
            
            dataBase = db;
        }

        private void FormCodes_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 26; i++)
            {
                Label lbl = new Label();
                lbl.Top = (i % 13) * 26 + 23;
                lbl.Left = (i / 13) * 200 + 10;
                lbl.Text = ((char)(i + 65)).ToString() + ":";
                lbl.AutoSize = true;
                labelCodes.Add(lbl);
                groupBoxCodes.Controls.Add(lbl);

                TextBox tb = new TextBox();
                tb.Top = (i % 13) * 26 + 20;
                tb.Left = 30 + (i / 13) * 200;
                tb.Width = 150;
                tb.Text = dataBase.Codes[i];
                textBoxCodes.Add(tb);
                groupBoxCodes.Controls.Add(tb);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 26; i++)
            {
                dataBase.Codes[i] = textBoxCodes[i].Text;
            }

            dataBase.SaveCodes();
        }
    }
}
