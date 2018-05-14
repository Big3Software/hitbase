using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;

namespace Big3.Hitbase.Controls
{
    public class TabItemModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler  PropertyChanged;

        private string imageResourceString;
        public string ImageResourceString
        {
            get
            {
                return imageResourceString;
            }
            set
            {
                imageResourceString = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageResourceString"));
            }
        }

        private string title;

        public string Title
        {
            get 
            { 
                return title; 
            }
            set 
            { 
                title = value;
            
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
            }
        }

        private bool isPinned;

        public bool IsPinned
        {
            get { return isPinned; }
            set 
            { 
                isPinned = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsPinned"));
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
