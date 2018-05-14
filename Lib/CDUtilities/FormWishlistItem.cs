using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormWishlistItem : Form
    {
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

        private WishlistItem wishlistItem;

        public FormWishlistItem(WishlistItem wishlistItem)
        {
            InitializeComponent();

            this.wishlistItem = wishlistItem;
            textBoxArtist.Text = wishlistItem.Artist;
            textBoxTitle.Text = wishlistItem.Title;
            textBoxFrom.Text = wishlistItem.From;

            if (wishlistItem.Reminder != DateTime.MinValue)
            {
                checkBoxEnableReminder.Checked = true;
                dateTimePickerReminder.Value = wishlistItem.Reminder;
            }

            foreach (int minutes in Wishlist.RemindAgainMinutes)
            {
                comboBoxReminderRelative.Items.Add(new ReminderComboBoxItem(minutes));
            }

            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            comboBoxReminderRelative.Enabled = checkBoxEnableReminder.Checked;
            dateTimePickerReminder.Enabled = checkBoxEnableReminder.Checked;
        }

        private void checkBoxEnableReminder_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            wishlistItem.From = textBoxFrom.Text;
            wishlistItem.Comment = textBoxComment.Text;
    		if (checkBoxEnableReminder.Checked)
	    		wishlistItem.Reminder = dateTimePickerReminder.Value;
		    else
    			wishlistItem.Reminder = DateTime.MinValue;
        }

        private void comboBoxReminderRelative_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReminderComboBoxItem cbi = (ReminderComboBoxItem)comboBoxReminderRelative.SelectedItem;

            dateTimePickerReminder.Value = DateTime.Now.AddMinutes(cbi.Minutes);
        }
    }
}
