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
using Microsoft.Win32;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.DataBaseEngine;
using System.Runtime.InteropServices;
using Big3.Hitbase.Miscellaneous;
using System.Drawing.Imaging;
using System.IO;

namespace Big3.Hitbase.Controls
{
    /// <summary>
    /// Interaction logic for ChoosePictureFastUserControl.xaml
    /// </summary>
    public partial class ChoosePictureFastUserControl : UserControl, INotifyPropertyChanged
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

        public ChoosePictureFastUserControl()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler ImageChanged;


        public string ImageFilename
        {
            get { return (string)GetValue(ImageFilenameProperty); }
            set { SetValue(ImageFilenameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageFilename.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageFilenameProperty =
            DependencyProperty.Register("ImageFilename", typeof(string), typeof(ChoosePictureFastUserControl), new UIPropertyMetadata(""));

        private string initialImageFilename;

        public string InitialImageFilename
        {
            get
            {
                return initialImageFilename;
            }
            set
            {
                initialImageFilename = value;
//                image.InitialImage = value;
            }
        }


        public CD CD { get; set; }



        public DataBase DataBase
        {
            get { return (DataBase)GetValue(DataBaseProperty); }
            set { SetValue(DataBaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataBase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataBaseProperty =
            DependencyProperty.Register("DataBase", typeof(DataBase), typeof(ChoosePictureFastUserControl), new UIPropertyMetadata(null));




        public int CDID
        {
            get { return (int)GetValue(CDIDProperty); }
            set { SetValue(CDIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CDID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CDIDProperty =
            DependencyProperty.Register("CDID", typeof(int), typeof(ChoosePictureFastUserControl), new UIPropertyMetadata(0));



        public string Artist
        {
            get { return (string)GetValue(ArtistProperty); }
            set { SetValue(ArtistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Artist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArtistProperty =
            DependencyProperty.Register("Artist", typeof(string), typeof(ChoosePictureFastUserControl), new UIPropertyMetadata(""));



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ChoosePictureFastUserControl), new UIPropertyMetadata(""));




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
                }
                else
                {
                    ImageWatermark = ImageLoader.FromResource("CDCover.png");
                }
            }
        }

        /*public string ButtonText
        {
            get
            {
                return textBlock.Text;
            }
            set
            {
                textBlock.Text = value;
            }
        }*/

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

        private bool enableLoadFromWeb = false;

        public bool EnableLoadFromWeb
        {
            get { return enableLoadFromWeb; }
            set
            {
                enableLoadFromWeb = value;

                //menuItemLoadFromWeb.Visibility = enableLoadFromWeb ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        private void menuItemChooseImage_Click(object sender, RoutedEventArgs e)
        {
            ChooseImage();
        }

        private void ChooseImage()
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = StringTable.FilterImages;
            openDlg.FileName = this.ImageFilename;
            if (!string.IsNullOrEmpty(this.ImageFilename))
                openDlg.InitialDirectory = System.IO.Path.GetDirectoryName(this.ImageFilename);
            openDlg.Title = StringTable.ChooseImage;
            if (openDlg.ShowDialog() == true)
            {
                SetNewCoverFilename(openDlg.FileName);
            }
        }

        private void ShowPictureButton_Click(object sender, RoutedEventArgs e)
        {
            ShowImage();
        }

        private void SearchPictureButton_Click(object sender, RoutedEventArgs e)
        {
            SearchImage();
        }

        private void ShowImage()
        {
            ShowPicturePopup showPicturePopup = new ShowPicturePopup();
            showPicturePopup.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            showPicturePopup.IsOpen = true;
            showPicturePopup.Placement = System.Windows.Controls.Primitives.PlacementMode.AbsolutePoint;
            //showPicturePopup.PlacementTarget = this.SearchPictureButton;

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

        private void SearchImage()
        {
            PictureSearchPopup pictureSearchPopup = new PictureSearchPopup();
            pictureSearchPopup.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            pictureSearchPopup.IsOpen = true;
            pictureSearchPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            pictureSearchPopup.PlacementTarget = this;
            pictureSearchPopup.Width = 640;
            pictureSearchPopup.Height = 200;
            pictureSearchPopup.StaysOpen = false;
            pictureSearchPopup.PictureSelected += new EventHandler(pictureSearchPopup_PictureSelected);

            string searchText = GetSearchText();
            pictureSearchPopup.Search(searchText);
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
            CD theCD = CD;
            bool saveCD = false;

            if (theCD == null && CoverType != DataBaseEngine.CoverType.PersonGroup)
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

            ImageFilename = filename;

            if (ImageChanged != null)
                ImageChanged(this, new EventArgs());
        }

        private string GetSearchText()
        {
            if (CoverType == DataBaseEngine.CoverType.PersonGroup)
            {
                return PersonGroup.Name;
            }
            else
            {
                if (CD != null)
                {
                    if (CD.Tracks.Count == 1)
                        return CD.Artist + CD.Tracks[0].Title;
                    else
                        return CD.Artist + " " + CD.Title;
                }
                else
                {
                    return Artist + " " + Title;
                }
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            /*menuItemCopy.IsEnabled = (image.Source != null);
            menuItemDelete.IsEnabled = (image.Source != null);
            menuItemShowImage.IsEnabled = (image.Source != null);
            menuItemEditImage.IsEnabled = (image.Source != null);
            menuItemPaste.IsEnabled = (Clipboard.ContainsImage());*/
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
                img.Save(filename, ImageFormat.Jpeg);

            SetNewCoverFilename(filename);
        }

        private string GetCoverFilename()
        {
            string filename;
            if (CoverType == CoverType.PersonGroup)
            {
                filename = Misc.GetCDCoverFilename(Misc.FilterFilenameChars(PersonGroup.Name) + ".jpg");
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

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = FindResource("myContextMenu") as ContextMenu;

            if (contextMenu != null)
                contextMenu.IsOpen = true;
        }

        private void buttonChoosePicture_Click(object sender, RoutedEventArgs e)
        {
            ChooseImage();
        }
    }
}
