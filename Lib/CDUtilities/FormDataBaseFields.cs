using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormDataBaseFields : Form
    {
        private class ComboBoxItem
        {
            public ComboBoxItem(UserFieldType userFieldType)
            {
                UserFieldType = userFieldType;
            }

            public UserFieldType UserFieldType { get; set; }

            public override string ToString()
            {
                switch (UserFieldType)
                {
                    case UserFieldType.Text:
                        return StringTable.Text;
                    case UserFieldType.Number:
                        return StringTable.Number;
                    case UserFieldType.Boolean:
                        return StringTable.Boolean;
                    case UserFieldType.Currency:
                        return StringTable.Currency;
                    case UserFieldType.Date:
                        return StringTable.Date;
                    default:
                        return "";
                }
            }
        }

        private DataBase dataBase;

        public FormDataBaseFields(DataBase db)
        {
            dataBase = db;

            InitializeComponent();

            FormThemeManager.SetTheme(this);
        }

        private void FormDataBaseFields_Load(object sender, EventArgs e)
        {
            textBoxDate.Text = dataBase.Master.DateName;

            comboBoxDateFormat.Items.Add(StringTable.DDMMYYYY);
            comboBoxDateFormat.Items.Add(StringTable.MMYYYY);
            comboBoxDateFormat.Items.Add(StringTable.YYYY);
            comboBoxDateFormat.Items.Add("<" + StringTable.Free + ">");
            comboBoxDateFormat.SelectedIndex = (int)dataBase.Master.DateType;

            FillUserField(textBoxCDField1, comboBoxCDField1, dataBase.Master.UserCDFields[0]);
            FillUserField(textBoxCDField2, comboBoxCDField2, dataBase.Master.UserCDFields[1]);
            FillUserField(textBoxCDField3, comboBoxCDField3, dataBase.Master.UserCDFields[2]);
            FillUserField(textBoxCDField4, comboBoxCDField4, dataBase.Master.UserCDFields[3]);
            FillUserField(textBoxCDField5, comboBoxCDField5, dataBase.Master.UserCDFields[4]);
            FillUserField(textBoxTrackField1, comboBoxTrackField1, dataBase.Master.UserTrackFields[0]);
            FillUserField(textBoxTrackField2, comboBoxTrackField2, dataBase.Master.UserTrackFields[1]);
            FillUserField(textBoxTrackField3, comboBoxTrackField3, dataBase.Master.UserTrackFields[2]);
            FillUserField(textBoxTrackField4, comboBoxTrackField4, dataBase.Master.UserTrackFields[3]);
            FillUserField(textBoxTrackField5, comboBoxTrackField5, dataBase.Master.UserTrackFields[4]);
        }

        private void FillUserField(TextBox textBox, ComboBox comboBox, UserField userField)
        {
            textBox.Text = userField.Name;
            FillFieldTypesInComboBox(comboBox);

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                ComboBoxItem item = (ComboBoxItem)comboBox.Items[i];
                if (item.UserFieldType == userField.Type)
                {
                    comboBox.SelectedIndex = i;
                    break;
                }
            }
        }

        private void FillFieldTypesInComboBox(ComboBox comboBox)
        {
            comboBox.Items.Add(new ComboBoxItem(UserFieldType.Text));
            comboBox.Items.Add(new ComboBoxItem(UserFieldType.Number));
            comboBox.Items.Add(new ComboBoxItem(UserFieldType.Boolean));
            comboBox.Items.Add(new ComboBoxItem(UserFieldType.Currency));
            comboBox.Items.Add(new ComboBoxItem(UserFieldType.Date));
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            dataBase.Master.DateName = textBoxDate.Text;
            dataBase.Master.DateType = (DateType)comboBoxDateFormat.SelectedIndex;

            GetUserField(textBoxCDField1, comboBoxCDField1, dataBase.Master.UserCDFields[0]);
            GetUserField(textBoxCDField2, comboBoxCDField2, dataBase.Master.UserCDFields[1]);
            GetUserField(textBoxCDField3, comboBoxCDField3, dataBase.Master.UserCDFields[2]);
            GetUserField(textBoxCDField4, comboBoxCDField4, dataBase.Master.UserCDFields[3]);
            GetUserField(textBoxCDField5, comboBoxCDField5, dataBase.Master.UserCDFields[4]);

            GetUserField(textBoxTrackField1, comboBoxTrackField1, dataBase.Master.UserTrackFields[0]);
            GetUserField(textBoxTrackField2, comboBoxTrackField2, dataBase.Master.UserTrackFields[1]);
            GetUserField(textBoxTrackField3, comboBoxTrackField3, dataBase.Master.UserTrackFields[2]);
            GetUserField(textBoxTrackField4, comboBoxTrackField4, dataBase.Master.UserTrackFields[3]);
            GetUserField(textBoxTrackField5, comboBoxTrackField5, dataBase.Master.UserTrackFields[4]);

            dataBase.Master.WriteConfig(dataBase);
        }

        private void GetUserField(TextBox textBox, ComboBox comboBox, UserField userField)
        {
            userField.Name = textBox.Text;
            userField.Type = ((ComboBoxItem)comboBox.SelectedItem).UserFieldType;
        }
    }
}
