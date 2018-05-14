using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.DataBaseEngine
{
    public class Url : INotifyPropertyChanged
    {
        private string urlType;

        public string UrlType
        {
            get { return urlType; }
            set
            {
                urlType = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UrlType"));
            }
        }

        private string link;
        public string Link
        {
            get
            {
                return link;
            }
            set
            {
                link = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Link"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class UrlList : SafeObservableCollection<Url>
    {
    }
}
