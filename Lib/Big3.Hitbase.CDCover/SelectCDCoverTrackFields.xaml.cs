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
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.DataBaseEngine;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDCover
{
    /// <summary>
    /// Interaction logic for SelectCDCoverTrackFields.xaml
    /// </summary>
    public partial class SelectCDCoverTrackFields : UserControl, IModalUserControl, INotifyPropertyChanged
    {
        public SelectCDCoverTrackFields()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private DataBase dataBase;
        private FieldType fieldType;

        private ColumnFieldCollection currentFields;
        private ColumnFieldCollection defaultFields;
        ObservableCollection<SelectField> fullList = new ObservableCollection<SelectField>();

        public FieldType FieldType
        {
            get { return fieldType; }
            set { fieldType = value; }
        }

        public void Init(DataBase db, FieldType ft, ColumnFieldCollection fields, ColumnFieldCollection defaultFields = null)
        {
            dataBase = db;
            fieldType = ft;

            currentFields = fields;
            this.defaultFields = defaultFields;

            buttonDefault.Visibility = (defaultFields != null) ? Visibility.Visible : Visibility.Collapsed;

            FillList();

            FillComboBox();

            UpdateWindowState();
        }

        private void FillComboBox()
        {
            List<TextAlignmentItem> items = new List<TextAlignmentItem>();
            items.Add(new TextAlignmentItem() { TextAlignment = TextAlignment.Left, DisplayName = StringTable.Left });
            items.Add(new TextAlignmentItem() { TextAlignment = TextAlignment.Right, DisplayName = StringTable.Right });
            items.Add(new TextAlignmentItem() { TextAlignment = TextAlignment.Center, DisplayName = StringTable.Center });
            ComboBoxTextAlignment.ItemsSource = items;
        }

        private void UpdateWindowState()
        {
            buttonMoveDown.IsEnabled = DataGridFields.SelectedItems.Count > 0;
            buttonMoveUp.IsEnabled = DataGridFields.SelectedItems.Count > 0;
        }

        public event EventHandler OKClicked;

        public event EventHandler CancelClicked;

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (OKClicked != null)
                OKClicked(sender, e);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(sender, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void FillList()
        {
            fullList.Clear();

            if (currentFields != null)
            {
                foreach (ColumnField field in currentFields)
                {
                    SelectField sf = new SelectField();
                    if (fieldType == DataBaseEngine.FieldType.TrackAndCD)
                        sf.Name = dataBase.GetNameOfFieldFull(field.Field);
                    else
                        sf.Name = dataBase.GetNameOfField(field.Field);
                    sf.Field = field.Field;
                    sf.Width = field.Width;
                    sf.TextAlignment = field.TextAlignment;
                    sf.IsChecked = true;
                    fullList.Add(sf);
                }
            }

            foreach (Field field in FieldHelper.GetAllFields())
            {
                if (field != Field.None && currentFields.FirstOrDefault(x => x.Field == field) == null && !FieldHelper.IsInternalField(field))
                {
                    string name = null;

                    switch (fieldType)
                    {
                        case FieldType.CD:
                            if (FieldHelper.IsCDField(field))
                                name = dataBase.GetNameOfField(field);
                            break;
                        case FieldType.Track:
                            if (FieldHelper.IsTrackField(field))
                                name = dataBase.GetNameOfField(field);
                            break;
                        case FieldType.TrackMain:
                            if (FieldHelper.IsTrackField(field, true))
                                name = dataBase.GetNameOfField(field);
                            break;
                        default:
                            name = dataBase.GetNameOfFieldFull(field);
                            break;
                    }

                    if (name != null)
                    {
                        SelectField sf = new SelectField();
                        sf.Width = dataBase.GetDefaultColumnWidthOfField(field);
                        sf.Name = name;
                        sf.Field = field;
                        sf.IsChecked = false;
                        fullList.Add(sf);
                    }
                }
            }

            DataGridFields.ItemsSource = fullList;
        }

        public ColumnFieldCollection SelectedFields
        {
            get
            {
                ColumnFieldCollection fields = new ColumnFieldCollection();
                foreach (SelectField lvField in fullList)
                {
                    if (lvField.IsChecked)
                    {
                        fields.Add(lvField);
                    }
                }

                return fields;
            }
        }

        private void DataGridFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private List<int> GetSelectedInidices()
        {
            IList selectedItems = DataGridFields.SelectedItems;
            List<int> selectedIndices = new List<int>();

            foreach (SelectField selectField in selectedItems)
            {
                for (int i = 0; i < this.DataGridFields.Items.Count; i++)
                {
                    if (selectField == this.DataGridFields.Items[i])
                    {
                        selectedIndices.Add(i);
                        break;
                    }
                }
            }

            return selectedIndices;
        }

        private void buttonMoveUp_Click(object sender, RoutedEventArgs e)
        {
            List<int> selectedIndices = GetSelectedInidices();

            for (int i = 0; i < selectedIndices.Count; i++)
            {
                if (selectedIndices[i] > 0)
                {
                    int index = selectedIndices[i];
                    SelectField lvField = fullList[index];
                    fullList.Remove(lvField);
                    fullList.Insert(index - 1, lvField);
                    DataGridFields.SelectedItems.Add(lvField);
                }
            }
        }

        private void buttonMoveDown_Click(object sender, RoutedEventArgs e)
        {
            List<int> selectedIndices = GetSelectedInidices();

            for (int i = 0; i < selectedIndices.Count; i++)
            {
                if (selectedIndices[i] < fullList.Count - 1)
                {
                    int index = selectedIndices[i];
                    SelectField lvField = fullList[index];
                    fullList.Remove(lvField);
                    fullList.Insert(index + 1, lvField);
                    DataGridFields.SelectedItems.Add(lvField);
                }
            }
        }

        private void buttonDefault_Click(object sender, RoutedEventArgs e)
        {
            currentFields = defaultFields;

            FillList();
        }


        public class SelectField : ColumnField
        {
            private string name;

            public string Name
            {
                get { return name; }
                set 
                { 
                    name = value;

                    FirePropertyChanged("Name");
                }
            }

            private bool isChecked;

            public bool IsChecked
            {
                get { return isChecked; }
                set 
                {
                    isChecked = value;

                    FirePropertyChanged("IsChecked");
                }
            }
        }

        public class TextAlignmentItem
        {
            public TextAlignment TextAlignment { get; set; }
            public string DisplayName { get; set; }

            public override string ToString()
            {
                return DisplayName;
            }
        }


    }

    public class TextAlignmentDisplayNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TextAlignment textAlignment = (TextAlignment)value;

            switch (textAlignment)
            {
                case TextAlignment.Center:
                    return StringTable.Center;
                case TextAlignment.Left:
                    return StringTable.Left;
                case TextAlignment.Right:
                    return StringTable.Right;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
