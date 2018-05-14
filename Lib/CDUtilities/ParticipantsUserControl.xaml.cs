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
using Big3.Hitbase.Configuration;
using Big3.Hitbase.DataBaseEngine;
using System.ComponentModel;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for ParticipantsUserControl.xaml
    /// </summary>
    public partial class ParticipantsUserControl : UserControl, INotifyPropertyChanged
    {
        public ParticipantsUserControl()
        {
            InitializeComponent();

            UpdateWindowState();

            this.DataContext = this;
        }

        private bool isReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }
            set
            {
                isReadOnly = value;
                UpdateWindowState();
            }
        }

        private DataBase dataBase;

        public DataBase DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }

        private PersonGroup personGroup;

        public PersonGroup PersonGroup
        {
            get 
            {
                return personGroup; 
            }
            set 
            {
                personGroup = value;
                if (personGroup != null)
                {
                    this.listboxParticipantPicture.ItemsSource = personGroup.Participants;
                    this.dataGridParticipants.ItemsSource = personGroup.Participants;
                }
                else
                {
                    this.listboxParticipantPicture.ItemsSource = null;
                    this.dataGridParticipants.ItemsSource = null;
                }
            }
        }

        private void UpdateWindowState()
        {
            dataGridParticipants.Visibility = Settings.Current.ShowParticipantPictures ? Visibility.Collapsed : Visibility.Visible;
            listboxParticipantPicture.Visibility = Settings.Current.ShowParticipantPictures ? Visibility.Visible : Visibility.Collapsed;

            ButtonShowParticipantsPictures.IsChecked = Settings.Current.ShowParticipantPictures;
            ButtonShowParticipantsTable.IsChecked = !Settings.Current.ShowParticipantPictures;

            ButtonAddParticipant.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            ButtonDeleteParticipant.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ButtonAddParticipant_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeleteParticipant_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonShowParticipantsTable_Click(object sender, RoutedEventArgs e)
        {
            Settings.Current.ShowParticipantPictures = false;

            UpdateWindowState();
        }

        private void ButtonShowParticipantsPictures_Click(object sender, RoutedEventArgs e)
        {
            Settings.Current.ShowParticipantPictures = true;

            UpdateWindowState();
        }

        private void dataGridParticipants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void dataGridParticipants_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void listboxParticipantPicture_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listboxParticipantPicture.SelectedItem == null)
                return;

            GroupParticipant groupParticipant = listboxParticipantPicture.SelectedItem as GroupParticipant;

            if (groupParticipant == null)
                return;

            PersonGroup personGroup = DataBase.GetPersonGroupByName(groupParticipant.Name, false);
            PersonGroupWindow pgw = new PersonGroupWindow(DataBase, PersonType.Unknown, personGroup);
            pgw.Owner = Window.GetWindow(this);
            pgw.ShowDialog();

            // Refresh
            this.PersonGroup = DataBase.GetPersonGroupByName(this.PersonGroup.Name, false);
        }

        private void listboxParticipantPicture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ConvertBeginEndToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            GroupParticipant gp = value as GroupParticipant;

            return string.Format("{0} - {1}", Misc.FormatDate(gp.Begin), Misc.FormatDate(gp.End));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
