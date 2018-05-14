// BurnData.cs
//
//
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ComponentModel;

namespace IMAPI2.Interop
{
    public class BurnData
    {
        public string uniqueRecorderId;
        public string statusMessage;
        public BURN_MEDIA_TASK task;

        // IDiscFormat2DataEventArgs Interface
        public long elapsedTime;		// Elapsed time in seconds
        public long remainingTime;		// Remaining time in seconds
        public long totalTime;			// total estimated time in seconds
        // IWriteEngine2EventArgs Interface
        public IMAPI_FORMAT2_DATA_WRITE_ACTION currentAction;
        public long startLba;			// the starting lba of the current operation
        public long sectorCount;		// the total sectors to write in the current operation
        public long lastReadLba;		// the last read lba address
        public long lastWrittenLba;	// the last written lba address
        public long totalSystemBuffer;	// total size of the system buffer
        public long usedSystemBuffer;	// size of used system buffer
        public long freeSystemBuffer;	// size of the free system buffer
    }
    public enum BURN_MEDIA_TASK
    {
        BURN_MEDIA_TASK_FILE_SYSTEM,
        BURN_MEDIA_TASK_WRITING
    }
}

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Unterstützte Geschwindigkeiten Medium im Rekorder
    /// </summary>
    public class WriteSpeed : INotifyPropertyChanged
    {
        private string speedx;
        public string SpeedX
        {
            get { return speedx; }
            set
            {
                speedx = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SpeedX"));
            }
        }

        private string speedkb;
        public string SpeedKB
        {
            get { return speedkb; }
            set
            {
                speedkb = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SpeedKB"));
            }
        }

        private int sectorspersecond;
        public int SectorsPerSecond
        {
            get { return sectorspersecond; }
            set
            {
                sectorspersecond = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SectorsPerSecond"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class BurnTreeList : INotifyPropertyChanged
    {
        public BurnTreeList()
        {
            Items = new ObservableCollection<BurnTreeList>();
            FileItems = new ObservableCollection<DirectoryContent>();
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private string fullPath;
        public string FullPath
        {
            get { return fullPath; }
            set
            {
                fullPath = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FullPath"));
            }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        private DateTime lastModified;
        public DateTime LastModified
        {
            get { return lastModified; }
            set
            {
                lastModified = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("LastModified"));
            }
        }

        private bool isDirectory;
        public bool IsDirectory
        {
            get { return isDirectory; }
            set
            {
                isDirectory = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsDirectory"));
            }
        }

        private string elementType;
        public string ElementType
        {
            get { return elementType; }
            set
            {
                elementType = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ElementType"));
            }
        }

        private long size;
        public long Size
        {
            get { return size; }
            set
            {
                size = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Size"));
            }
        }

        private ImageSource image;

        public ImageSource Image
        {
            get { return image; }
            set
            {
                image = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Image"));
            }
        }

        public ObservableCollection<BurnTreeList> Items { get; set; }
        public ObservableCollection<DirectoryContent> FileItems { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


    }

    public class DirectoryContent : INotifyPropertyChanged
    {
        public DirectoryContent()
        {
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }


        private string fullPath;

        public string FullPath
        {
            get { return fullPath; }
            set
            {
                fullPath = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FullPath"));
            }
        }

        private DateTime lastModified;
        public DateTime LastModified
        {
            get { return lastModified; }
            set
            {
                lastModified = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("LastModified"));
            }
        }

        private bool isDirectory;
        public bool IsDirectory
        {
            get { return isDirectory; }
            set
            {
                isDirectory = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsDirectory"));
            }
        }

        private string elementType;
        public string ElementType
        {
            get { return elementType; }
            set
            {
                elementType = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ElementType"));
            }
        }

        private long size;
        public long Size
        {
            get { return size; }
            set
            {
                size = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Size"));
            }
        }

        private string sourcePath;
        public string SourcePath
        {
            get
            {
                return sourcePath;
            }
            set
            {
                sourcePath = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SourcePath"));
            }
        }

        private ImageSource image;

        public ImageSource Image
        {
            get { return image; }
            set
            {
                image = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Image"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class VolumesToDrivesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string devicePaths;
            string hh = value.ToString();

            object[] ss = (object[])value;

            devicePaths = "";
            foreach (string volPath in ss)
            {
                devicePaths += volPath + ",";
            }
            devicePaths = devicePaths.TrimEnd(',');

            return string.Format("{0}", devicePaths);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DataGridRightAlighTextColumn : DataGridTextColumn
    {
        protected override FrameworkElement GenerateElement(System.Windows.Controls.DataGridCell cell, object dataItem)
        {
            FrameworkElement fe = base.GenerateElement(cell, dataItem);

            fe.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Right);

            return fe;
        }
    }

    //private Collection<BurnTreeList> BurnTreeListList;

    //public IEnumerable<BurnTreeList> BurnTreeListList
    //{
    //    get { return BurnTreeListList; }
    //}
}