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
using Big3.Hitbase.CDUtilities;
using Big3.Hitbase.DataBaseEngine;

namespace Big3.Hitbase.CatalogView3D
{
    /// <summary>
    /// Interaction logic for AddWishUserControl.xaml
    /// </summary>
    public partial class AddWishUserControl : UserControl
    {
        public event EventHandler OKClick;
        public event EventHandler CancelClick;

        private Track track;

        private WishlistItem wishlistItem = null;

        public bool NewWish { get; set; }

        public AddWishUserControl()
        {
            InitializeComponent();

            foreach (int minutes in Wishlist.RemindAgainMinutes)
            {
                ComboBoxReminderRelative.Items.Add(new ReminderComboBoxItem(minutes));
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (OKClick != null)
                OKClick(sender, e);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClick != null)
                CancelClick(sender, e);
        }

        internal void SetTrack(Track track)
        {
            NewWish = true;
            this.track = track;
            TextBoxArtist.Text = track.Artist;
            TextBoxTitle.Text = track.Title;
            TextBoxFrom.Text = "";
            TextBoxComment.Text = "";
            TextBoxReminderTime.Text = "";
            checkBoxReminder.IsChecked = false;
            UpdateWindowState();
        }

        internal void SetWishlistItem(Big3.Hitbase.CDUtilities.WishlistItem wishlistItem)
        {
            NewWish = false;
            this.wishlistItem = wishlistItem;
            TextBoxArtist.Text = wishlistItem.Artist;
            TextBoxTitle.Text = wishlistItem.Title;
            TextBoxFrom.Text = wishlistItem.From;
            TextBoxComment.Text = wishlistItem.Comment;

            if (wishlistItem.Reminder != DateTime.MinValue)
            {
                checkBoxReminder.IsChecked = true;
                TextBoxReminderTime.Text = wishlistItem.Reminder.ToShortTimeString();
            }
        }

        internal Big3.Hitbase.CDUtilities.WishlistItem GetWishlistItem()
        {
            WishlistItem wishlistItem = null;

            if (this.wishlistItem != null)
                wishlistItem = this.wishlistItem;
            else
                wishlistItem = new WishlistItem();

            wishlistItem.Artist = TextBoxArtist.Text;
            wishlistItem.Title = TextBoxTitle.Text;
            wishlistItem.From = TextBoxFrom.Text;
            wishlistItem.Comment = TextBoxComment.Text;
            if (track != null)
                wishlistItem.TrackID = track.ID;

            if (checkBoxReminder.IsChecked == true)
            {
                DateTime dateTime;
                if (DateTime.TryParse(TextBoxReminderTime.Text, out dateTime))
                    wishlistItem.Reminder = dateTime;
            }

            return wishlistItem;
        }

        private void checkBoxReminder_Checked(object sender, RoutedEventArgs e)
        {
            UpdateWindowState();
        }

        private void checkBoxReminder_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            TextBoxReminderTime.IsEnabled = checkBoxReminder.IsChecked == true ? true : false;
            ComboBoxReminderRelative.IsEnabled = checkBoxReminder.IsChecked == true ? true : false;
        }

        public class ReminderComboBoxItem
        {
            public int Minutes { get; set; }

            public ReminderComboBoxItem(int minutes)
            {
                this.Minutes = minutes;
            }

            public override string ToString()
            {
                return string.Format("{0} {1}", Minutes, StringTable.Minutes);
            }
        }

        private void ComboBoxReminderRelative_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReminderComboBoxItem cbi = (ReminderComboBoxItem)ComboBoxReminderRelative.SelectedItem;

            TextBoxReminderTime.Text = DateTime.Now.AddMinutes(cbi.Minutes).ToShortTimeString();
        }
    }
}
