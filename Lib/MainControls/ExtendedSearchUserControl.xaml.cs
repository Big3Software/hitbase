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
using System.ComponentModel;
using System.Collections.ObjectModel;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Miscellaneous;
using System.Windows.Threading;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for ExtendedSearchUserControl.xaml
    /// </summary>
    public partial class ExtendedSearchUserControl : UserControl, INotifyPropertyChanged
    {
        DispatcherTimer dtSearchNow = new DispatcherTimer();

        public event EventHandler StartSearch;

        public ExtendedSearchUserControl()
        {
            InitializeComponent();

            this.DataContext = this;
            dtSearchNow.Interval = TimeSpan.FromMilliseconds(500);
            dtSearchNow.Tick += new EventHandler(dtSearchNow_Tick);
        }

        void dtSearchNow_Tick(object sender, EventArgs e)
        {
            dtSearchNow.Stop();

            if (!Condition.IsValid())
                return;

            if (StartSearch != null)
                StartSearch(this, new EventArgs());
        }

        public FieldType FieldType { get; set; }

        private DataBase dataBase;

        public DataBase DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }

        private Big3.Hitbase.DataBaseEngine.Condition condition;
        public Big3.Hitbase.DataBaseEngine.Condition Condition
        {
            get
            {
                return condition;
            }
            set
            {
                condition = value;

                // Dummyfeld anlegen
                if (condition.Count == 0)
                {
                    condition.Add(new DataBaseEngine.SingleCondition());
                }

                ItemsControlConditions.ItemsSource = value;
            }
        }

        public ObservableCollection<ViewModelField> AllFields
        {
            get
            {
                if (DataBase == null)
                    return null;

                ObservableCollection<ViewModelField> allFields = new ObservableCollection<ViewModelField>();

                if (FieldType == DataBaseEngine.FieldType.CD)
                {
                    foreach (Field field in FieldHelper.GetAllCDFields(false))
                    {
                        allFields.Add(new ViewModelField() { Field = field, DisplayName = this.DataBase.GetNameOfField(field) });
                    }
                }
                else
                {
                    foreach (Field field in FieldHelper.GetAllFields())
                    {
                        allFields.Add(new ViewModelField() { Field = field, DisplayName = this.DataBase.GetNameOfFieldFull(field) });
                    }
                }

                return allFields;
            }
        }

        public ObservableCollection<ViewModelLogical> AllLogicals
        {
            get
            {
                ObservableCollection<ViewModelLogical> allLogicals = new ObservableCollection<ViewModelLogical>();

                allLogicals.Add(new ViewModelLogical() { Logical = Logical.And, DisplayName = StringTable.And.ToLower() });
                allLogicals.Add(new ViewModelLogical() { Logical = Logical.Or, DisplayName = StringTable.Or.ToLower() });

                return allLogicals;
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;

            SingleCondition sc = btn.DataContext as SingleCondition;
            Condition.Remove(sc);

            if (Condition.Count == 0)
                Condition.Add(new SingleCondition());

            StartSearchNowTimer();
        }

        private void ComboBoxAndOr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = e.OriginalSource as ComboBox;

            SingleCondition sc = cb.DataContext as SingleCondition;

            // Prüfen, ob wir in der letzten Zeile sind.
            if (Condition[Condition.Count-1] == sc)
            {
                Condition.Add(new DataBaseEngine.SingleCondition());
            }
        }

        private void TextBoxValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartSearchNowTimer();
        }

        private void StartSearchNowTimer()
        {
            if (dtSearchNow.IsEnabled)
                dtSearchNow.Stop();

            dtSearchNow.Start();
        }

        private void ComboBoxFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchNowTimer();
        }

        private void ComboBoxOperator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchNowTimer();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public static class ConditionCommands
    {

        private static RoutedCommand saveCondition = new RoutedCommand("SaveCondition", typeof(ConditionCommands));

        public static RoutedCommand SaveCondition
        {
            get { return saveCondition; }
        }


        private static RoutedCommand executeSearch = new RoutedCommand("ExecuteSearch", typeof(ConditionCommands));

        public static RoutedCommand ExecuteSearch
        {
            get { return executeSearch; }
        }


    }

    public class ViewModelOperator : INotifyPropertyChanged
    {


        private Operator _operator;
        public Operator Operator
        {
            get { return _operator; }
            set
            {
                _operator = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Operator"));
            }
        }


        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ViewModelLogical : INotifyPropertyChanged
    {
        private Logical logical;
        public Logical Logical
        {
            get { return logical; }
            set
            {
                logical = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Logical"));
            }
        }

        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ViewModelField : INotifyPropertyChanged
    {
        private Field field;
        public Field Field
        {
            get { return field; }
            set
            {
                field = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Field"));
            }
        }

        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ObjectToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "";
            else
                return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }
    }

    public class ConditionVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Operator condOperator = (Operator)value;

            if (condOperator == Operator.Empty || condOperator == Operator.NotEmpty)
                return Visibility.Hidden;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}