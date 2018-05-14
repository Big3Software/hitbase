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
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.IO;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using System.Windows.Media.Animation;

namespace Big3.Hitbase.Controls
{
    /// <summary>
    /// Interaction logic for ShowPicturePopup.xaml
    /// </summary>
    public partial class ShowPicturePopup : Popup
    {
        public ShowPicturePopup()
        {
            InitializeComponent();
        }

        private string imageFilename;

        public virtual string ImageFilename
        {
            get { return imageFilename; }
            set
            {
                try
                {
                    imageFilename = value;
                    if (value == null)
                        MyImage.Source = null;
                    else
                    {
                        byte[] imageBytes = File.ReadAllBytes(Misc.FindCover(value));

                        MemoryStream m = new MemoryStream(imageBytes);

                        MyImage.Source = ImageLoader.GetBitmapImageFromMemoryStream(m);
                        //m.Close();
                    }
                }
                catch   // Ignorieren
                {
                    MyImage.Source = ImageLoader.FromResource("InvalidCDCover.png");
                }
            }
        }



        internal void ShowImage()
        {
            MyImage.Width = 600;
            MyImage.Height = 600;
            /*DoubleAnimation daWidth = new DoubleAnimation(startWidth, 600, TimeSpan.FromMilliseconds(2000).Duration());
            daWidth.EasingFunction = new BackEase();
            DoubleAnimation daHeight = new DoubleAnimation(startHeight, 600, TimeSpan.FromMilliseconds(2000).Duration());
            daHeight.EasingFunction = new BackEase();
            MyImage.BeginAnimation(Image.WidthProperty, daWidth);
            MyImage.BeginAnimation(Image.HeightProperty, daHeight);

            int endPosX = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width / 2 - 300;
            int endPosY = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height / 2 - 300;
            */
            //ThicknessAnimation ta = new ThicknessAnimation(new Thickness(startX, startY, 0, 0), new Thickness(endPosX, endPosY, 0, 0), TimeSpan.FromMilliseconds(2000).Duration());
            //ta.EasingFunction = new BackEase();
            //MyImage.BeginAnimation(Image.MarginProperty, ta);
        }

        private void MyImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsOpen = false;
        }
    }
}
