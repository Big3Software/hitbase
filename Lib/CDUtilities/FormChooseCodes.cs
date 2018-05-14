using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormChooseCodes : Form
    {
        DataBase dataBase;

        public string Codes { get; set; }

        public FormChooseCodes(DataBase db, string codes)
        {
            InitializeComponent();

            dataBase = db;
            Codes = codes;
        }

        private void FormChooseCodes_Load(object sender, EventArgs e)
        {
            FillCodeInComboBox(comboBoxCode1, Codes, 0);
            FillCodeInComboBox(comboBoxCode2, Codes, 1);
            FillCodeInComboBox(comboBoxCode3, Codes, 2);
            FillCodeInComboBox(comboBoxCode4, Codes, 3);
            FillCodeInComboBox(comboBoxCode5, Codes, 4);
        }

        private void FillCodeInComboBox(ComboBox comboBox, string codes, int index)
        {
            comboBox.BeginUpdate();
            for (int i = 0; i < 26; i++)
            {
                // Nur belegte Kennzeichen anzeigen
                if (Settings.Current.ShowOnlyUsedCodes && String.IsNullOrEmpty(dataBase.Codes[i]))
                    continue;

                string str = string.Format("{0}: {1}", (char)(i + 65), dataBase.Codes[i]);

                int itemIndex = comboBox.Items.Add(str);

                if (!string.IsNullOrEmpty(codes) && codes.Length > index && 
                    (char)(i + 65) == codes[index])
                    comboBox.SelectedIndex = itemIndex;
            }
            comboBox.EndUpdate();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string code = "";

            code += GetCodeFromComboBox(comboBoxCode1);
            code += GetCodeFromComboBox(comboBoxCode2);
            code += GetCodeFromComboBox(comboBoxCode3);
            code += GetCodeFromComboBox(comboBoxCode4);
            code += GetCodeFromComboBox(comboBoxCode5);

            for (int i = 0; i < code.Length; i++)
            {
                for (int j = i + 1; j < code.Length; j++)
                {
                    if (code[i] == code[j])
                    {
                        MessageBox.Show(StringTable.DuplicateCodeNotAllowed, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DialogResult = DialogResult.None;
                        return;
                    }
                }
            }

            DialogResult = DialogResult.OK;

            Codes = code;
        }

        private string GetCodeFromComboBox(ComboBox comboBox)
        {
            if (comboBox.SelectedItem == null)
                return "";

            string text = comboBox.SelectedItem.ToString();

            return text[0].ToString();
        }
    }
}
