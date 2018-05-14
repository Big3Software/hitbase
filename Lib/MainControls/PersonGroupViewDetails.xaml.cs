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
using System.ComponentModel;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SoundEngine;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.DataBaseEngine.PersonGroupCatalogViewDataSetTableAdapters;
using Big3.Hitbase.CDUtilities;
using System.Windows.Controls.Primitives;
using Big3.Hitbase.Controls;
using System.IO;
using System.Windows.Media.Effects;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for AlbumViewDetails.xaml
    /// </summary>
    public partial class PersonGroupViewDetails : UserControl, IAlbumView
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private CDQueryDataSet CDQuery = null;
        SafeObservableCollection<PersonGroupViewItem> items = new SafeObservableCollection<PersonGroupViewItem>();

        public PersonGroupViewDetails()
        {
            InitializeComponent();
        }

        private void itemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = VisualTreeExtensions.FindParent<ListBoxItem>(e.OriginalSource as DependencyObject);
            if (item != null)
            {
                PersonGroupViewItem pgvi = itemsControl.SelectedItem as PersonGroupViewItem;
                //            OpenCD(avi.ID);

                PersonGroupProperties pgp = new PersonGroupProperties();
                pgp.DataBase = DataBase;
                pgp.PersonGroup = DataBase.GetPersonGroupById(pgvi.ID);
                pgp.PersonType = PersonType.Unknown;
                GlobalServices.ModalService.NavigateTo(pgp, StringTable.EditPersonGroup, delegate(bool returnValue)
                {

                });
            }
        }


        #region IAlbumView Members

        public void FillList()
        {
            this.itemsControl.ItemsSource = null;
            //GridLoadingCircle.Visibility = System.Windows.Visibility.Visible;

            //AsyncOperationManager.SynchronizationContext = new DispatcherSynchronizationContext(this.dataGrid.Dispatcher);

            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker = new BackgroundWorker();
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwAlbumView_RunWorkerCompleted);
                backgroundWorker.DoWork += new DoWorkEventHandler(bwAlbumView_DoWork);
                backgroundWorker.RunWorkerAsync();
            }
        }

        void bwAlbumView_DoWork(object sender, DoWorkEventArgs e)
        {
            PersonGroupCatalogViewDataSet personGroupsDataSet = new PersonGroupCatalogViewDataSet();
            PersonGroupTableAdapter ta = new PersonGroupTableAdapter(DataBase);
            ta.Fill(personGroupsDataSet.PersonGroup);

            foreach (PersonGroupCatalogViewDataSet.PersonGroupRow row in personGroupsDataSet.PersonGroup)
            {
                // Leeren Interpreten nicht anzeigen. Kann schon mal angelegt werden.
                if (string.IsNullOrEmpty(row.Name))
                    continue;

                PersonGroupViewItem newItem = new PersonGroupViewItem();
                FillPersonGroupItem(row, newItem);
                items.AddItemFromThread(newItem);
            }

            e.Result = items;
        }

        private void FillPersonGroupItem(PersonGroupCatalogViewDataSet.PersonGroupRow row, PersonGroupViewItem newItem)
        {
            newItem.ID = row.PersonGroupID;
            newItem.Name = row.Name;
            newItem.ImageFilename = row.ImageFilename;
            newItem.URL = row.URL;
            newItem.NumberOfTracks = row.NumberOfTracks;
            newItem.TypeAndSex = string.Format("{0}, {1}",
                DataBaseEngine.DataBase.GetNameOfPersonGroupType(row.IsTypeNull() ? PersonGroupType.Unknown : (PersonGroupType)row.Type),
                DataBaseEngine.DataBase.GetNameOfPersonGroupSex(row.IsSexNull() ? SexType.Unknown : (SexType)row.Sex));
            newItem.LandOfOrigin = string.Format("{0}: {1}", StringTable.LandOfOrigin, row.Country);

            string birth;
            if (!row.IsTypeNull() && row.Type == (int)PersonGroupType.Single)
                birth = StringTable.DateOfBirthPerson;
            else
                birth = StringTable.DateOfBirthGroup;
            string death;
            if (!row.IsTypeNull() && row.Type == (int)PersonGroupType.Single)
                death = StringTable.DateOfDeathPerson;
            else
                death = StringTable.DateOfDeathGroup;

            newItem.DayOfBirth = string.Format("{0}: {1}", birth, Misc.FormatDate(row.BirthDay));
            newItem.DayOfDeath = string.Format("{0}: {1}", death, Misc.FormatDate(row.DayOfDeath));
        }

        void bwAlbumView_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //GridLoadingCircle.Visibility = System.Windows.Visibility.Collapsed;

            SafeObservableCollection<PersonGroupViewItem> items = e.Result as SafeObservableCollection<PersonGroupViewItem>;
            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;
            itemsControl.ItemsSource = lcv;

            if (FillListCompleted != null)
                FillListCompleted(this, new EventArgs());
        }

        private bool FilterRow(object row)
        {
            if (string.IsNullOrEmpty(FullTextSearch))
                return true;

            PersonGroupViewItem pgvi = row as PersonGroupViewItem;

            if (CatalogView.CompareString(pgvi.Name, FullTextSearch))
                return true;

            if (CatalogView.CompareString(pgvi.LandOfOrigin, FullTextSearch))
                return true;

            return false;
        }

        public void UpdateList()
        {
            if (itemsControl.ItemsSource is ListCollectionView)
                ((ListCollectionView)itemsControl.ItemsSource).Refresh();
        }

        public DataBase DataBase
        {
            get;
            set;
        }

        public string FullTextSearch { get; set; }

        public FieldCollection GroupBy
        {
            get;
            set;
        }

        public SortFieldCollection SortFields { get; set; }

        public Big3.Hitbase.DataBaseEngine.Condition Condition { get; set; }

        public DataBaseEngine.Condition ConditionFromTree
        {
            get;
            set;
        }

        public event EventHandler FillListCompleted;

        public int NumberOfItems
        {
            get
            {
                return this.itemsControl.Items.Count;
            }
        }

        public List<int> SelectedCDIDs
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler OpenCD;

        #endregion

        void PictureSearchPopup_CloseClicked(object sender, EventArgs e)
        {
            Canvas overlayPopupCanvas = VisualTreeExtensions.FindVisualChildByName<Canvas>(Window.GetWindow(this), "OverlayPopupCanvas");
            overlayPopupCanvas.Children.Clear();
            overlayPopupCanvas.Visibility = System.Windows.Visibility.Collapsed;
        }

        void PictureSearchPopup_PictureSelected(object sender, EventArgs e)
        {
            PictureSearchUserControl psp = sender as PictureSearchUserControl;

            if (psp.SelectedItem != null)
            {
                BitmapImage bi = psp.DownloadSelectedImage();
                if (bi != null)
                {
                    PersonGroupViewItem pgvi = itemsControl.SelectedItem as PersonGroupViewItem;
                    string filename = Misc.GetCDCoverFilename(Misc.FilterFilenameChars(pgvi.Name) + ".jpg");
                    FileStream stream = new FileStream(filename, FileMode.Create);
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.QualityLevel = 30;
                    encoder.Frames.Add(BitmapFrame.Create(bi));
                    encoder.Save(stream);
                    stream.Close();

                    DataBase.SetPersonGroupImage(pgvi.ID, filename);
                    UpdateRow(pgvi);
                }
            }

            Canvas overlayPopupCanvas = VisualTreeExtensions.FindVisualChildByName<Canvas>(Window.GetWindow(this), "OverlayPopupCanvas");
            overlayPopupCanvas.Children.Clear();
            overlayPopupCanvas.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void UpdateRow(PersonGroupViewItem pgvi)
        {
            PersonGroupCatalogViewDataSet personGroupsDataSet = new PersonGroupCatalogViewDataSet();
            PersonGroupTableAdapter ta = new PersonGroupTableAdapter(DataBase);
            ta.FillById(personGroupsDataSet.PersonGroup, pgvi.ID);

            FillPersonGroupItem(personGroupsDataSet.PersonGroup[0], pgvi); 
        }

        private void SearchPictureButton_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = VisualTreeExtensions.FindParent<ListBoxItem>(e.OriginalSource as DependencyObject);
            PersonGroupViewItem pgvi = lbi.DataContext as PersonGroupViewItem;

            Canvas overlayPopupCanvas = VisualTreeExtensions.FindVisualChildByName<Canvas>(Window.GetWindow(this), "OverlayPopupCanvas");
            PictureSearchUserControl pictureSearchPopup = new PictureSearchUserControl();
            pictureSearchPopup.Width = 620;
            pictureSearchPopup.Height = 180;
            DropShadowEffect dse = new DropShadowEffect();
            dse.Color = Colors.LightGray;
            pictureSearchPopup.Effect = dse;

            overlayPopupCanvas.Children.Clear();
            overlayPopupCanvas.Children.Add(pictureSearchPopup);

            pictureSearchPopup.SetValue(Canvas.TopProperty, Mouse.GetPosition(overlayPopupCanvas).Y);
            pictureSearchPopup.SetValue(Canvas.LeftProperty, Mouse.GetPosition(overlayPopupCanvas).X);
            overlayPopupCanvas.Visibility = System.Windows.Visibility.Visible;
            pictureSearchPopup.Search(pgvi.Name);
            pictureSearchPopup.PictureSelected += new EventHandler(PictureSearchPopup_PictureSelected);
            pictureSearchPopup.CloseClicked += new EventHandler(PictureSearchPopup_CloseClicked);
        }




        public bool Closing()
        {
            return true;
        }
    }

    public class PersonGroupViewItem : INotifyPropertyChanged
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string PersonGroupType { get; set; }

        public string Sex { get; set; }

        public string TypeAndSex { get; set; }

        public string LandOfOrigin { get; set; }

        private string imageFilename;
        public string ImageFilename
        {
            get
            {
                return imageFilename;
            }
            set
            {
                imageFilename = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageFilename"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Image"));
                }
            }
        }

        public string URL { get; set; }

        public int NumberOfTracks { get; set; }

        private ImageSource defaultImage = ImageLoader.FromResource("MissingPersonGroupImage.png");
        public ImageSource Image
        {
            get
            {
                ImageSource imageSource = defaultImage;

                if (!string.IsNullOrEmpty(ImageFilename) && System.IO.File.Exists(ImageFilename))
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(ImageFilename);
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                    imageSource = img;
                }

                return imageSource;
            }
        }

        public string DayOfBirth { get; set; }

        public string DayOfDeath { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
