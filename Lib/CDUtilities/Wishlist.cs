using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Big3.Hitbase.CDUtilities
{
    [Serializable]
    public class WishlistItem : INotifyPropertyChanged
    {
        [XmlIgnore]
        public int TrackID { get; set; }				// Lied ID

        public string Artist { get; set; }				// Interpret
        public string Title { get; set; }				// Titel
        public DateTime Time { get; set; }			// Zeit des Wunsches

        private string from;

        public string From
        {
            get { return from; }
            set
            {
                from = value; 
                
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("From"));
            }
        }
        //public string From { get; set; }				// Gewünscht von
        public DateTime Reminder { get; set; }      // Erinnerung
        public string Comment { get; set; }				// Kommentar

        [XmlIgnore]
        public string ImageFilename { get; set; }       // Bildchen

        [XmlIgnore]
        public bool AlreadyReminded { get; set; }       // Schon erinnert
        [XmlIgnore]
        public bool Deleted { get; set; }				// Gelöscht

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [Serializable]
    [XmlRoot("Wishlist")]
    public class Wishlist : ObservableCollection<WishlistItem>
    {
        public static int[] RemindAgainMinutes = { 5, 10, 15, 20, 30, 45, 60, 90, 120 };
    }

    public delegate void OnAddWishlistItemToWishlist(object sender, WishlistItem wishlistItem);
}
