using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls;
using System.ComponentModel;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for UserFieldsUserControl.xaml
    /// </summary>
    public partial class UserFieldsUserControl : UserControl, INotifyPropertyChanged
    {
        public UserFieldsUserControl()
        {
            InitializeComponent();
        }

        private List<UserField> userFields;

        private Control[] contentControl = new Control[MasterConfig.MaximumNumberOfUserFields];

        public bool DontShowEmptyFieldsMessage { get; set; }

        private DataBase dataBase;

        private bool trackUserFields;

        public bool TrackUserFields
        {
            get { return trackUserFields; }
            set { trackUserFields = value; }
        }

        private CD cd;

        public CD CD
        {
            get { return cd; }
            set 
            { 
                cd = value;
                this.DataContext = value;
                
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CD"));
            }
        }

        private Track track;

        public Track Track
        {
            get { return track; }
            set 
            { 
                track = value;
                this.DataContext = value;
                
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Track"));
            }
        }

        /// <summary>
        /// true, wenn die UserControls in einem Filter benutzt werden.
        /// </summary>
        public bool UsedAsFilter { get; set; }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            FormDataBaseFields formDataBaseFields = new FormDataBaseFields(dataBase);

            if (formDataBaseFields.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataBase.Master.WriteConfig(dataBase);

                SetFields(dataBase, userFields);
            }
        }

        public void SetFields(DataBase db, List<UserField> userFields)
        {
            dataBase = db;

            GridControls.Children.Clear();

            if (DontShowEmptyFieldsMessage)
                TextBlockLink.Visibility = System.Windows.Visibility.Collapsed;

            this.userFields = userFields;

            bool atLeastOneUserField = false;

            int row = 0;
            foreach (UserField userField in userFields)
            {
                if (!string.IsNullOrEmpty(userField.Name))
                {
                    if (!atLeastOneUserField)
                    {
                        GridControls.Visibility = System.Windows.Visibility.Visible;
                        TextBlockLink.Visibility = System.Windows.Visibility.Collapsed;
                        atLeastOneUserField = true;
                    }

                    TextBlock tb = new TextBlock();

                    tb.Text = userField.Name + ":";
                    tb.Margin = new Thickness(5);
                    tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    tb.SetValue(Grid.RowProperty, row);
                    tb.SetValue(Grid.ColumnProperty, 0);
                    GridControls.Children.Add(tb);

                    switch (userField.Type)
                    {
                        case UserFieldType.Boolean:
                            if (UsedAsFilter)
                            {
                                ComboBox comboBox = new ComboBox();
                                comboBox.IsEditable = false;
                                comboBox.Items.Add("<" + StringTable.All + ">");
                                comboBox.Items.Add(StringTable.Unchecked);
                                comboBox.Items.Add(StringTable.Checked);
                                comboBox.SelectedIndex = 0;
                                contentControl[row] = comboBox;
                            }
                            else
                            {
                                CheckBox checkBoxCurrency = new CheckBox();

                                Binding binding = new Binding("UserField" + (row + 1).ToString());
                                if (trackUserFields)
                                    checkBoxCurrency.DataContext = this.Track;
                                else
                                    checkBoxCurrency.DataContext = this.CD;

                                binding.Mode = BindingMode.TwoWay;
                                binding.Converter = new UserFieldBoolConverter();
                                checkBoxCurrency.SetBinding(CheckBox.IsCheckedProperty, binding);
                                contentControl[row] = checkBoxCurrency;

                                contentControl[row].HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            }
                            break;
                        case UserFieldType.Currency:
                            {
                                TextBox textBoxCurrency = new TextBox();

                                Binding binding = new Binding("UserField" + (row + 1).ToString());
                                if (trackUserFields)
                                    textBoxCurrency.DataContext = this.Track;
                                else
                                    textBoxCurrency.DataContext = this.CD;
                                binding.Mode = BindingMode.TwoWay;
                                binding.Converter = new UserFieldPriceConverter();
                                textBoxCurrency.SetBinding(TextBox.TextProperty, binding);
                                contentControl[row] = textBoxCurrency;
                            }
                            break;
                        case UserFieldType.Date:
                            {
                                TextBox textBoxDate = new TextBox();

                                Binding binding = new Binding("UserField" + (row + 1).ToString());
                                if (trackUserFields)
                                    textBoxDate.DataContext = this.Track;
                                else
                                    textBoxDate.DataContext = this.CD;
                                binding.Mode = BindingMode.TwoWay;
                                binding.Converter = new DateConverter();
                                textBoxDate.SetBinding(TextBox.TextProperty, binding);
                                contentControl[row] = textBoxDate;
                                break;
                            }
                        case UserFieldType.Number:
                        default:
                            {
                                TextBox textBox = new TextBox();
                                Binding binding = new Binding("UserField" + (row + 1).ToString());
                                if (trackUserFields)
                                    textBox.DataContext = this.Track;
                                else
                                    textBox.DataContext = this.CD;
                                binding.Mode = BindingMode.TwoWay;

                                if (userField.Type == UserFieldType.Number)
                                    binding.Converter = new NumberConverter();

                                textBox.SetBinding(TextBox.TextProperty, binding);
                                contentControl[row] = textBox;
                                break;
                            }
                    }

                    contentControl[row].Margin = new Thickness(5);
                    contentControl[row].VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    //                    .Height = 25;
                    contentControl[row].SetValue(Grid.RowProperty, row);
                    contentControl[row].SetValue(Grid.ColumnProperty, 1);

                    GridControls.Children.Add(contentControl[row]);
                }

                row++;
            }
        }

        /*public string GetValue(int fieldNumber)
        {
            if (contentControl[fieldNumber] != null)
            {
                switch (userFields[fieldNumber].Type)
                {
                    case UserFieldType.Number:
                        {
                            if (!string.IsNullOrEmpty(((TextBox)contentControl[fieldNumber]).Text))
                            {
                                return Misc.Atoi(((TextBox)contentControl[fieldNumber]).Text).ToString();
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
                            if (((CheckBox)contentControl[fieldNumber]).IsChecked.Value)
                                return "1";
                            else
                                return "0";
                        }
                    case UserFieldType.Currency:
                        if (((TextBoxCurrencyWPF)contentControl[fieldNumber]).Value == 0)
                            return "";
                        else
                            return ((TextBoxCurrencyWPF)contentControl[fieldNumber]).Value.ToString();
                    case UserFieldType.Date:
                        return ((TextBoxDateWPF)contentControl[fieldNumber]).Value;
                    default:
                        return ((TextBox)contentControl[fieldNumber]).Text;
                }
            }

            return "";
        }*/

        /*public void SetValue(int fieldNumber, string value)
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
                                ((CheckBox)contentControl[fieldNumber]).IsChecked = true;

                            if (value == "0")
                                ((CheckBox)contentControl[fieldNumber]).IsChecked = false;
                        }
                        break;
                    case UserFieldType.Currency:
                        if (!string.IsNullOrEmpty(value))
                            ((TextBoxCurrencyWPF)contentControl[fieldNumber]).Value = Misc.Atoi(value);
                        break;
                    case UserFieldType.Date:
                        ((TextBoxDateWPF)contentControl[fieldNumber]).Value = value;
                        break;
                    default:
                        ((TextBox)contentControl[fieldNumber]).Text = value;
                        break;
                }
            }
        }*/

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
