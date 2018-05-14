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

namespace Big3.Hitbase.Controls
{
    /// <summary>
    /// Interaction logic for RatingUserControl.xaml
    /// </summary>
    public partial class RatingUserControl : UserControl
    {
        public event EventHandler ValueChanged;

        BitmapImage biStar;
        BitmapImage biStarDark;

        public RatingUserControl()
        {
            InitializeComponent();

            biStar = new BitmapImage(new Uri("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/Star.png"));
            biStarDark = new BitmapImage(new Uri("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/StarDark.png"));

            this.MouseMove += new MouseEventHandler(RatingUserControl_MouseMove);
            this.MouseLeave += new MouseEventHandler(RatingUserControl_MouseLeave);
            
            Cursor = Cursors.Hand;
        }

        void RatingUserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            UpdatePanel(Rating);
        }

        void RatingUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (ReadOnly)
                return;

            Image image = e.OriginalSource as Image;
            if (image == null)
                return;

            int previewRating = Convert.ToInt32(image.Tag);

            if (Mouse.GetPosition(image).X >= image.ActualWidth / 4)
                previewRating++;

            if (previewRating > 6)
                previewRating = 6;
            UpdatePanel(previewRating);
        }

        void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ReadOnly)
                return;

            Image image = e.OriginalSource as Image;
            if (image == null)
                return;

            int previewRating = Convert.ToInt32(image.Tag);
            if (Mouse.GetPosition(image).X >= image.ActualWidth / 4)
                previewRating++;

            if (previewRating > 6)
                previewRating = 6;

            Rating = previewRating;
        }

        public int Rating
        {
            get { return (int)GetValue(RatingProperty); }
            set { SetValue(RatingProperty, value); UpdatePanel(value); }
        }

        // Using a DependencyProperty as the backing store for Rating.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register("Rating", typeof(int), typeof(RatingUserControl), new UIPropertyMetadata(-1, new PropertyChangedCallback(UserControl_RatingPropertyChanged)));

        static void UserControl_RatingPropertyChanged(DependencyObject property, 
                DependencyPropertyChangedEventArgs args)
        {
            RatingUserControl ratingUserControl = (RatingUserControl)property;
            ratingUserControl.UpdatePanel(ratingUserControl.Rating);
        }

        private void UpdatePanel(int rating)
        {
            for (int i=1;i<=6;i++)
            {
                Image img = Panel.Children[i-1] as Image;
                if (i <= rating)
                    img.Source = biStar;
                else
                    img.Source = biStarDark;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //UpdatePanel(Rating);
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set 
            { 
                SetValue(ReadOnlyProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(RatingUserControl), new UIPropertyMetadata(false, new PropertyChangedCallback(UserControl_ReadOnlyPropertyChanged)));

        static void UserControl_ReadOnlyPropertyChanged(DependencyObject property, 
                DependencyPropertyChangedEventArgs args)
        {
            RatingUserControl ratingUserControl = (RatingUserControl)property;

            if (ratingUserControl.ReadOnly)
                ratingUserControl.Cursor = Cursors.Arrow;
            else
                ratingUserControl.Cursor = Cursors.Hand;
        }


    }
}
