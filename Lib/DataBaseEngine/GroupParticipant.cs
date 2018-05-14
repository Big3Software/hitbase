using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.Miscellaneous;
using System.ComponentModel;

namespace Big3.Hitbase.DataBaseEngine
{
    public class GroupParticipant : INotifyPropertyChanged
    {
        private string role;
        /// <summary>
        /// Rolle
        /// </summary>
        public string Role
        {
            get
            {
                return role;
            }
            set
            {
                role = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Role"));
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private string begin;
        public string Begin
        {
            get
            {
                return begin;
            }
            set
            {
                begin = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Begin"));
            }
        }

        private string end;
        public string End
        {
            get
            {
                return end;
            }
            set
            {
                end = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("End"));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public class GroupParticipantList : SafeObservableCollection<GroupParticipant>
    {
    }
}
