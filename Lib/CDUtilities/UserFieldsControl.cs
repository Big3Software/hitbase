using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Controls;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.CDUtilities;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class UserFieldsControl : UserControl
    {
        public UserFieldsControl()
        {
            InitializeComponent();

            tableLayoutPanel.Visible = false;
        }

        private List<UserField> userFields;

        private Control[] contentControl = new Control[MasterConfig.MaximumNumberOfUserFields];

        public bool DontShowEmptyFieldsMessage { get; set; }

        private DataBase dataBase;

        /// <summary>
        /// true, wenn die UserControls in einem Filter benutzt werden.
        /// </summary>
        public bool UsedAsFilter { get; set; }

        public void SetFields(DataBase db, List<UserField> userFields)
        {
            dataBase = db;

            tableLayoutPanel.Controls.Clear();

            if (DontShowEmptyFieldsMessage)
                linkLabel.Visible = false;

            this.userFields = userFields;

            bool atLeastOneUserField = false;

            int row = 0;
            foreach (UserField userField in userFields)
            {
                if (!string.IsNullOrEmpty(userField.Name))
                {
                    if (!atLeastOneUserField)
                    {
                        tableLayoutPanel.Visible = true;
                        linkLabel.Visible = false;
                        atLeastOneUserField = true;
                    }

                    Label lbl = new Label();

                    lbl.Text = userField.Name + ":";
                    lbl.Padding = new Padding(0, 5, 0, 0);
                    lbl.Margin = new Padding(0);
                    lbl.AutoSize = true;
                    tableLayoutPanel.Controls.Add(lbl, 0, row);

                    switch (userField.Type)
                    {
                        case UserFieldType.Number:
                            contentControl[row] = new TextBox();
                            break;
                        case UserFieldType.Boolean:
                            if (UsedAsFilter)
                            {
                                ComboBox comboBox = new ComboBox();
                                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                                comboBox.Items.Add("<" + StringTable.All + ">");
                                comboBox.Items.Add(StringTable.Unchecked);
                                comboBox.Items.Add(StringTable.Checked);
                                comboBox.SelectedIndex = 0;
                                contentControl[row] = comboBox;
                            }
                            else
                                contentControl[row] = new CheckBox();
                            break;
                        case UserFieldType.Currency:
                            contentControl[row] = new TextBoxCurrency();
                            break;
                        case UserFieldType.Date:
                            contentControl[row] = new TextBoxDate();
                            break;
                        default:
                            contentControl[row] = new TextBox();
                            break;
                    }

                    contentControl[row].Dock = DockStyle.Fill;
                    tableLayoutPanel.Controls.Add(contentControl[row], 1, row);
                }

                row++;
            }
        }

        public string GetValue(int fieldNumber)
        {
            if (contentControl[fieldNumber] != null)
            {
                switch (userFields[fieldNumber].Type)
                {
                    case UserFieldType.Number:
                        {
                            if (!string.IsNullOrEmpty(contentControl[fieldNumber].Text))
                            {
                                return Misc.Atoi(contentControl[fieldNumber].Text).ToString();
                            }
                            else
                            {
                                return "";
                            }
                        }
                    case UserFieldType.Boolean:
                        if (UsedAsFilter)
                        {
                            switch (((ComboBox)contentControl[fieldNumber]).SelectedIndex)
                            {
                                case 0:
                                    return "";
                                case 1:
                                    return "0";
                                case 2:
                                    return "1";
                                default:
                                    return "";
                            }
                        }
                        else
                        {
                            if (((CheckBox)contentControl[fieldNumber]).Checked)
                                return "1";
                            else
                                return "0";
                        }
                        break;
                    case UserFieldType.Currency:
                        if (((TextBoxCurrency)contentControl[fieldNumber]).Value == 0)
                            return "";
                        else
                            return ((TextBoxCurrency)contentControl[fieldNumber]).Value.ToString();
                    case UserFieldType.Date:
                        return ((TextBoxDate)contentControl[fieldNumber]).Value;
                    default:
                        return contentControl[fieldNumber].Text;
                }
            }

            return "";
        }

        public void SetValue(int fieldNumber, string value)
        {
            if (contentControl[fieldNumber] != null)
            {
                switch (userFields[fieldNumber].Type)
                {
                    case UserFieldType.Number:
                        ((TextBox)contentControl[fieldNumber]).Text = value;
                        break;
                    case UserFieldType.Boolean:
                        if (UsedAsFilter)
                        {
                            if (value == "0")
                                ((ComboBox)contentControl[fieldNumber]).SelectedIndex = 1;

                            if (value == "1" || value != null && value.ToLower() == "ja")
                                ((ComboBox)contentControl[fieldNumber]).SelectedIndex = 2;
                        }
                        else
                        {
                            if (value == "1" || value != null && value.ToLower() == "ja")
                                ((CheckBox)contentControl[fieldNumber]).Checked = true;

                            if (value == "0")
                                ((CheckBox)contentControl[fieldNumber]).Checked = false;
                        }
                        break;
                    case UserFieldType.Currency:
                        if (!string.IsNullOrEmpty(value))
                            ((TextBoxCurrency)contentControl[fieldNumber]).Value = Misc.Atoi(value);
                        break;
                    case UserFieldType.Date:
                        ((TextBoxDate)contentControl[fieldNumber]).Value = value;
                        break;
                    default:
                        contentControl[fieldNumber].Text = value;
                        break;
                }
            }
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormDataBaseFields formDataBaseFields = new FormDataBaseFields(dataBase);

            if (formDataBaseFields.ShowDialog() == DialogResult.OK)
            {
                dataBase.Master.WriteConfig(dataBase);

                SetFields(dataBase, userFields);
            }
        }
    }
}
