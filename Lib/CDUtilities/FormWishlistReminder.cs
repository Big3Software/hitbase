using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SoundEngine;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormWishlistReminder : Form
    {
        private Wishlist wishlist;
        private WishlistItem wishlistItem;
        private DataBase database;

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

        public FormWishlistReminder(Wishlist wishlist, WishlistItem wishlistItem, DataBase database)
        {
            InitializeComponent();

            this.wishlist = wishlist;
            this.wishlistItem = wishlistItem;
            this.database = database;

            labelWishTitle.Text = wishlistItem.Artist + " - " + wishlistItem.Title;
            labelWishedBy.Text = wishlistItem.From;
            labelWishedAt.Text = wishlistItem.Time.ToShortTimeString();

            AddItemToList(wishlistItem);
            listViewWishlistItems.Items[0].Selected = true;

            foreach (int minutes in Wishlist.RemindAgainMinutes)
            {
                comboBoxRemindAgain.Items.Add(new ReminderComboBoxItem(minutes));
            }
            comboBoxRemindAgain.SelectedIndex = 0;
        }

        private void AddItemToList(WishlistItem wishlistItem)
        {
            ListViewItem newItem = listViewWishlistItems.Items.Add(wishlistItem.Artist);
            newItem.SubItems.Add(wishlistItem.Title);
            newItem.SubItems.Add(wishlistItem.From);
            newItem.Tag = wishlistItem;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            wishlist.Remove(wishlistItem);
            Close();
        }

        private void buttonOpenItem_Click(object sender, EventArgs e)
        {
            FormWishlistItem formWishlistItem = new FormWishlistItem(wishlistItem);
            formWishlistItem.ShowDialog(this);
        }

        private void listViewWishlistItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonOpenItem.Enabled = (listViewWishlistItems.SelectedIndices.Count > 0);
            buttonPlayNext.Enabled = (listViewWishlistItems.SelectedIndices.Count > 0);
            buttonPlayImmediatly.Enabled = (listViewWishlistItems.SelectedIndices.Count > 0);
        }

        private void buttonPlayNext_Click(object sender, EventArgs e)
        {
            WishlistItem wli = (WishlistItem)listViewWishlistItems.SelectedItems[0].Tag;

            Track track = database.GetTrackById(wli.TrackID);
            if (track != null)
            {
                List<string> filenames = new List<string>();
                filenames.Add(track.Soundfile);

                AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                addTracksParams.AddTracksType = AddTracksToPlaylistType.Next;
                addTracksParams.Filenames = filenames;
                HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, System.Windows.Application.Current.MainWindow);
            }
        }

        private void buttonPlayImmediatly_Click(object sender, EventArgs e)
        {
            WishlistItem wli = (WishlistItem)listViewWishlistItems.SelectedItems[0].Tag;

            Track track = database.GetTrackById(wli.TrackID);
            if (track != null)
            {
                List<string> filenames = new List<string>();
                filenames.Add(track.Soundfile);

                AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                addTracksParams.AddTracksType = AddTracksToPlaylistType.None;
                addTracksParams.Filenames = filenames;
                HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, System.Windows.Application.Current.MainWindow);
            }
        }

        private void comboBoxRemindAgain_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonRemindAgain_Click(object sender, EventArgs e)
        {
            ReminderComboBoxItem cbi = (ReminderComboBoxItem)comboBoxRemindAgain.SelectedItem;

            wishlistItem.Reminder = DateTime.Now.AddMinutes(cbi.Minutes);
            wishlistItem.AlreadyReminded = false;
            Close();
        }
    }
}
