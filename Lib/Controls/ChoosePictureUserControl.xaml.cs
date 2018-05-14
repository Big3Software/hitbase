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
using Big3.Hitbase.DataBaseEngine;
using Microsoft.Win32;
using Big3.Hitbase.Miscellaneous;
using System.Drawing.Imaging;
using System.Drawing;
using Big3.Hitbase.SharedResources;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Media.Effects;

namespace Big3.Hitbase.Controls
{
    /// <summary>
    /// Interaction logic for ChoosePictureUserControl.xaml
    /// </summary>
    public partial class ChoosePictureUserControl : UserControl, INotifyPropertyChanged
    {
        public enum ShowCommands : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }

        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(
            IntPtr hwnd,
            string lpOperation,
            string lpFile,
            string lpParameters,
            string lpDirectory,
            ShowCommands nShowCmd);

        public delegate void LoadFromWebDelegate();
        public event LoadFromWebDelegate LoadFromWeb;

        public delegate void ScanDelegate();
        public event ScanDelegate Scan;

        public event EventHandler ImageChanged;

        public ChoosePictureUserControl()
        {
            InitializeComponent();
        }

        public CD CD { get; set; }



        public DataBase DataBase
        {
            get { return (DataBase)GetValue(DataBaseProperty); }
            set { SetValue(DataBaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataBase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataBaseProperty =
            DependencyProperty.Register("DataBase", typeof(DataBase), typeof(ChoosePictureUserControl), new UIPropertyMetadata(null));


        

        public int CDID
        {
            get { return (int)GetValue(CDIDProperty); }
            set { SetValue(CDIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CDID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CDIDProperty =
            DependencyProperty.Register("CDID", typeof(int), typeof(ChoosePictureUserControl), new UIPropertyMetadata(0));



        public string Artist
        {
            get { return (string)GetValue(ArtistProperty); }
            set { SetValue(ArtistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Artist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArtistProperty =
            DependencyProperty.Register("Artist", typeof(string), typeof(ChoosePictureUserControl), new UIPropertyMetadata(""));



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ChoosePictureUserControl), new UIPropertyMetadata(""));




        public PersonGroup PersonGroup { get; set; }

        private CoverType coverType = CoverType.Front;

        public CoverType CoverType 
        {
            get
            {
                return coverType;
            }
            set
            {
                coverType = value;
                if (coverType == DataBaseEngine.CoverType.PersonGroup)
                {
                    ImageWatermark = ImageLoader.FromResource("MissingPersonGroupImage.png");
                    EnableLoadFromWeb = false;
                }
                else
                {
                    ImageWatermark = ImageLoader.FromResource("CDCover.png");
                }
            }
        }

        public string ButtonText
        {
            get
            {
                return textBlock.Text;
            }
            set
            {
                textBlock.Text = value;
            }
        }

        private ImageSource imageWatermark;
        public ImageSource ImageWatermark
        {
            get
            {
                return imageWatermark;
            }
            set
            {
                imageWatermark = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageWatermark"));
                }
            }
        }

        private bool enableLoadFromWeb = true;

        public bool EnableLoadFromWeb
        {
            get { return enableLoadFromWeb; }
            set 
            {
                enableLoadFromWeb = value;
            }
        }



        public string ImageFilename
        {
            get { return (string)GetValue(ImageFilenameProperty); }
            set 
            { 
                SetValue(ImageFilenameProperty, value);

                UpdateImage();
            }
        }

        private bool hasDropShadow;

        public bool HasDropShadow
        {
            get 
            { 
                return hasDropShadow; 
            }
            set 
            { 
                hasDropShadow = value;

                if (hasDropShadow)
                {
                    DropShadowEffect dropShadow = new DropShadowEffect();
                    dropShadow.Color = Colors.LightGray;
                    this.image.Effect = dropShadow;
                }
            }
        }

        private void UpdateImage()
        {
            try
            {
                if (string.IsNullOrEmpty(ImageFilename))
                {
                    image.Source = null;
                }
                else
                {
                    string searchedImageFilename = Misc.FindCover(ImageFilename);
                    if (!File.Exists(searchedImageFilename))
                    {
                        image.Source = ImageLoader.FromResource("InvalidCDCover.png");
                        buttonChoosePicture.Visibility = Visibility.Collapsed;
                        image.Visibility = Visibility.Visible;
                        return;
                    }

                    byte[] imageBytes = File.ReadAllBytes(searchedImageFilename);

                    MemoryStream m = new MemoryStream(imageBytes);

                    image.Source = ImageLoader.GetBitmapImageFromMemoryStream(m);
                    //m.Close();
                }

                //pictureBox.ImageLocation = value;
                if (string.IsNullOrEmpty(ImageFilename))
                {
                    buttonChoosePicture.Visibility = System.Windows.Visibility.Visible;
                    image.Visibility = Visibility.Collapsed;
                }
                else
                {
                    buttonChoosePicture.Visibility = System.Windows.Visibility.Collapsed;
                    image.Visibility = Visibility.Visible;
                }
            }
            catch   // Ignorieren
            {
                image.Source = ImageLoader.FromResource("InvalidCDCover.png");
                buttonChoosePicture.Visibility = Visibility.Collapsed;
                image.Visibility = Visibility.Visible;
            }
        }

        // Using a DependencyProperty as the backing store for ImageFilename.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageFilenameProperty =
            DependencyProperty.Register("ImageFilename", typeof(string), typeof(ChoosePictureUserControl), new UIPropertyMetadata("", new PropertyChangedCallback(UserControl_ImageFilenameChanged)));

        static void UserControl_ImageFilenameChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            ChoosePictureUserControl choosePictureUserControl = (ChoosePictureUserControl)property;

            choosePictureUserControl.UpdateImage();
        }

        private void ChooseImage()
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = StringTable.FilterImages;
            openDlg.FileName = this.ImageFilename;
            if (!string.IsNullOrEmpty(this.ImageFilename) && Directory.Exists(System.IO.Path.GetDirectoryName(this.ImageFilename)))
                openDlg.InitialDirectory = System.IO.Path.GetDirectoryName(this.ImageFilename);
            else
                openDlg.InitialDirectory = Misc.GetCDCoverDirectory();


            openDlg.Title = StringTable.ChooseImage;
            if (openDlg.ShowDialog() == true)
            {
                SetNewCoverFilename(openDlg.FileName);
            }
        }

        private void loadFromWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LoadFromWeb != null)
                LoadFromWeb();
        }

        private void scanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Scan != null)
                Scan();
        }

        private void SearchInWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSearchImageInWeb formSearchImageInWeb = new FormSearchImageInWeb();
            formSearchImageInWeb.CD = CD;
            formSearchImageInWeb.SearchText = PersonGroup.Name;
            formSearchImageInWeb.CoverType = CoverType;

            if (formSearchImageInWeb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filename = GetCoverFilename();

                using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(formSearchImageInWeb.SelectedImage));
                    encoder.QualityLevel = 100;
                    encoder.Save(fileStream);
                }

                SetNewCoverFilename(filename);
            }
        }

        private string GetCoverFilename()
        {
            string filename;
            if (CoverType == CoverType.PersonGroup)
            {
                if (PersonGroup != null)
                    filename = Misc.GetCDCoverFilename(Misc.FilterFilenameChars(PersonGroup.Name) + ".jpg");
                else
                    filename = Misc.GetCDCoverFilename(Misc.FilterFilenameChars(Artist) + ".jpg");
            }
            else
            {
                if (CD != null)
                {
                    filename = CD.GetCDCoverFilename(CoverType);
                }
                else
                {
                    filename = Big3.Hitbase.DataBaseEngine.CD.GetCDCoverFilename(Artist, Title, CoverType);
                }
            }
            return filename;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = sender as ContextMenu;

            MenuItem menuItemPaste = VisualTreeExtensions.FindVisualChildByName<MenuItem>(contextMenu, "menuItemPaste");
            MenuItem menuItemCopy = VisualTreeExtensions.FindVisualChildByName<MenuItem>(contextMenu, "menuItemCopy");
            MenuItem menuItemDelete = VisualTreeExtensions.FindVisualChildByName<MenuItem>(contextMenu, "menuItemDelete");
            MenuItem menuItemShowImage = VisualTreeExtensions.FindVisualChildByName<MenuItem>(contextMenu, "menuItemShowImage");
            MenuItem menuItemEditImage = VisualTreeExtensions.FindVisualChildByName<MenuItem>(contextMenu, "menuItemEditImage");

            MenuItem menuItemLoadFromWeb = VisualTreeExtensions.FindVisualChildByName<MenuItem>(contextMenu, "menuItemLoadFromWeb");

            menuItemCopy.IsEnabled = (image.Source != null);
            menuItemDelete.IsEnabled = (image.Source != null);
            menuItemShowImage.IsEnabled = (image.Source != null);
            menuItemEditImage.IsEnabled = (image.Source != null);
            
            menuItemPaste.IsEnabled = (Clipboard.ContainsImage());

            menuItemLoadFromWeb.IsEnabled = enableLoadFromWeb;

        }

        private void menuItemChooseImage_Click(object sender, RoutedEventArgs e)
        {
            ChooseImage();
        }

        private void buttonChoosePicture_Click(object sender, RoutedEventArgs e)
        {
            ChooseImage();
        }

        private void menuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            ImageFilename = null;
        }

        private void menuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            if (image.Source != null)
                Clipboard.SetImage((BitmapSource)image.Source);
        }

        private void menuItemPaste_Click(object sender, RoutedEventArgs e)
        {
            string filename = GetCoverFilename();

            using (System.Drawing.Image img = System.Windows.Forms.Clipboard.GetImage())
            {
                if (img != null)
                {
                    img.Save(filename, ImageFormat.Jpeg);
                    SetNewCoverFilename(filename);
                }
            }
        }

        private void SearchPictureButton_Click(object sender, RoutedEventArgs e)
        {
            SearchImage();
        }

        private void SearchImage()
        {
            PictureSearchPopup pictureSearchPopup = new PictureSearchPopup();
            pictureSearchPopup.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            pictureSearchPopup.IsOpen = true;
            pictureSearchPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            
            Button searchPictureButton = VisualTreeExtensions.FindVisualChildByName<Button>(this, "SearchPictureButton");
            if (searchPictureButton != null)
                pictureSearchPopup.PlacementTarget = searchPictureButton;
            else
                pictureSearchPopup.PlacementTarget = this;
            pictureSearchPopup.Width = 640;
            pictureSearchPopup.Height = 200;
            pictureSearchPopup.StaysOpen = false;
            pictureSearchPopup.PictureSelected += new EventHandler(pictureSearchPopup_PictureSelected);

            string searchText = GetSearchText();
            pictureSearchPopup.Search(searchText);
        }

        private void ShowPictureButton_Click(object sender, RoutedEventArgs e)
        {
            ShowImage();
        }

        private void ShowImage()
        {
            ShowPicturePopup showPicturePopup = new ShowPicturePopup();
            showPicturePopup.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            showPicturePopup.IsOpen = true;
            showPicturePopup.Placement = System.Windows.Controls.Primitives.PlacementMode.AbsolutePoint;
            //showPicturePopup.PlacementTarget = this.SearchPictureButton;

            System.Windows.Point startPos = image.PointToScreen(new System.Windows.Point(0, 0));
            int endPosX = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width / 2 - 300;
            int endPosY = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height / 2 - 300;
            showPicturePopup.HorizontalOffset = endPosX;
            showPicturePopup.VerticalOffset = endPosY;
            showPicturePopup.Width = 600;
            showPicturePopup.Height = 600;

            showPicturePopup.ImageFilename = this.ImageFilename;
            showPicturePopup.StaysOpen = false;
            
            showPicturePopup.ShowImage();
        }

        void pictureSearchPopup_PictureSelected(object sender, EventArgs e)
        {
            PictureSearchUserControl psp = sender as PictureSearchUserControl;

            BitmapImage bi = psp.DownloadSelectedImage();
            if (bi != null)
            {
                string filename = GetCoverFilename();

                FileStream stream = new FileStream(filename, FileMode.Create);
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.QualityLevel = 30;
                encoder.Frames.Add(BitmapFrame.Create(bi));
                encoder.Save(stream);
                stream.Close();

                SetNewCoverFilename(filename);

                PictureSearchPopup popup = psp.Parent as PictureSearchPopup;
                if (popup != null)
                    popup.IsOpen = false;
            }
        }

        private void SetNewCoverFilename(string filename)
        {
            if (CoverType == CoverType.PersonGroup && PersonGroup == null)
            {
                PersonGroup pg = DataBase.GetPersonGroupByName(Artist, false);
                if (pg != null)
                    pg.ImageFilename = filename;
                pg.Save(DataBase);
            }
            else
            {
                CD theCD = CD;
                bool saveCD = false;

                if (theCD == null && CoverType != DataBaseEngine.CoverType.PersonGroup && CDID != 0)
                {
                    theCD = DataBase.GetCDById(CDID);
                    saveCD = true;
                }

                if (theCD != null)
                {
                    switch (CoverType)
                    {
                        case CoverType.Front:
                            theCD.CDCoverFrontFilename = filename;
                            break;
                        case CoverType.Back:
                            theCD.CDCoverBackFilename = filename;
                            break;
                        case CoverType.Label:
                            theCD.CDCoverLabelFilename = filename;
                            break;
                        case CoverType.PersonGroup:
                            PersonGroup.ImageFilename = filename;
                            break;
                        default:
                            break;
                    }

                    if (saveCD)
                    {
                        theCD.Save(DataBase);
                        Big3.Hitbase.SoundEngine.SoundFileInformation.WriteMP3Tags(theCD);
                    }
                }
            }

            ImageFilename = filename;

            if (ImageChanged != null)
                ImageChanged(this, new EventArgs());
        }

        private string GetSearchText()
        {
            if (CoverType == DataBaseEngine.CoverType.PersonGroup)
            {
                if (PersonGroup != null)
                    return PersonGroup.Name;
                else
                    return Artist;
            }
            else
            {
                if (CD != null)
                {
                    if (CD.Tracks.Count == 1)
                        return CD.Artist + " " + CD.Tracks[0].Title;
                    else
                        return CD.Artist + " " + CD.Title;
                }
                else
                {
                    if (CDID != 0)
                    {
                        CD cd = DataBase.GetCDById(CDID);

                        if (cd.Tracks.Count == 1)
                            return cd.Artist + " " + cd.Tracks[0].Title;
                        else
                            return cd.Artist + " " + cd.Title;
                    }
                    else
                    {
                        return Artist + " " + Title;
                    }
                }
            }
        }

        private void menuItemSearchInWeb_Click(object sender, RoutedEventArgs e)
        {
            SearchImage();
        }

        private void menuItemShowImage_Click(object sender, RoutedEventArgs e)
        {
            ShowImage();
        }

        private void menuItemEditImage_Click(object sender, RoutedEventArgs e)
        {
            ShellExecute(new NativeWindowWrapper(Window.GetWindow(this)).Handle, "edit", this.ImageFilename, "", "", ShowCommands.SW_NORMAL);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = FindResource("myContextMenu") as ContextMenu;

            if (contextMenu != null)
                contextMenu.IsOpen = true;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Button searchPictureButton = VisualTreeExtensions.FindVisualChildByName<Button>(sender as DependencyObject, "SearchPictureButton");
            if (searchPictureButton == null)
            {
                searchPictureButton = FindResource("mySearchPictureButton") as Button;

                theGrid.Children.Add(searchPictureButton);
            }

            Button showPictureButton = VisualTreeExtensions.FindVisualChildByName<Button>(sender as DependencyObject, "ShowPictureButton");
            if (showPictureButton == null)
            {
                showPictureButton = FindResource("myShowPictureButton") as Button;

                theGrid.Children.Add(showPictureButton);
            }

            if (this.buttonChoosePicture.Visibility == System.Windows.Visibility.Visible)
            {
                searchPictureButton.BeginAnimation(Button.OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(500).Duration()));
                searchPictureButton.Visibility = System.Windows.Visibility.Visible;
                showPictureButton.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                showPictureButton.BeginAnimation(Button.OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(500).Duration()));
                searchPictureButton.Visibility = System.Windows.Visibility.Collapsed;
                showPictureButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.buttonChoosePicture.Visibility == System.Windows.Visibility.Visible)
            {
                Button b = VisualTreeExtensions.FindVisualChildByName<Button>(sender as DependencyObject, "SearchPictureButton");
                b.BeginAnimation(Button.OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(500).Duration()));
            }
            else
            {
                Button b = VisualTreeExtensions.FindVisualChildByName<Button>(sender as DependencyObject, "ShowPictureButton");
                b.BeginAnimation(Button.OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(500).Duration()));
            }
        }

        Big3.Hitbase.Miscellaneous.Twain.WpfTwain wpfTwain;

        private void menuItemScan_Click(object sender, RoutedEventArgs e)
        {
            if (wpfTwain == null)
                wpfTwain = new Miscellaneous.Twain.WpfTwain();

            if (wpfTwain.Select() == false)
                return;
            wpfTwain.Acquire(true);
            wpfTwain.TwainCloseRequest += new Miscellaneous.Twain.TwainEventHandler(wpfTwain_TwainCloseRequest);
            wpfTwain.TwainCloseOk += new Miscellaneous.Twain.TwainEventHandler(wpfTwain_TwainCloseOk);
            wpfTwain.TwainTransferReady += new Miscellaneous.Twain.TwainTransferReadyHandler(wpfTwain_TwainTransferReady);
        }

        void wpfTwain_TwainTransferReady(Miscellaneous.Twain.WpfTwain sender, List<ImageSource> imageSources)
        {
            if (imageSources.Count > 0)
            {
                string filename = GetCoverFilename();

                using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSources[0]));
                    encoder.QualityLevel = 100;
                    encoder.Save(fileStream);
                }

                SetNewCoverFilename(filename);
            }
        }

        void wpfTwain_TwainCloseOk(Miscellaneous.Twain.WpfTwain sender)
        {
            
        }

        void wpfTwain_TwainCloseRequest(Miscellaneous.Twain.WpfTwain sender)
        {
            
        }

        private void menuItemLoadFromWeb_Click(object sender, RoutedEventArgs e)
        {
            CDCoverAmazon.GetCDCover(this.CD);
        }
    }
}
