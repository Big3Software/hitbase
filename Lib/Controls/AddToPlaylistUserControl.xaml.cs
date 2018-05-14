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

namespace Big3.Hitbase.Controls
{
    /// <summary>
    /// Interaction logic for AddToPlaylistUserControl.xaml
    /// </summary>
    public partial class AddToPlaylistUserControl : UserControl
    {
        public static readonly RoutedEvent PlayNowEvent = EventManager.RegisterRoutedEvent(
            "PlayNow", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddToPlaylistUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler PlayNow
        {
            add { AddHandler(PlayNowEvent, value); }
            remove { RemoveHandler(PlayNowEvent, value); }
        }

        public static readonly RoutedEvent PlayNextEvent = EventManager.RegisterRoutedEvent(
            "PlayNext", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddToPlaylistUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler PlayNext
        {
            add { AddHandler(PlayNextEvent, value); }
            remove { RemoveHandler(PlayNextEvent, value); }
        }

        public static readonly RoutedEvent PlayLastEvent = EventManager.RegisterRoutedEvent(
            "PlayLast", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddToPlaylistUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler PlayLast
        {
            add { AddHandler(PlayLastEvent, value); }
            remove { RemoveHandler(PlayLastEvent, value); }
        }

        public static readonly RoutedEvent PreListenEvent = EventManager.RegisterRoutedEvent(
            "PreListen", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddToPlaylistUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler PreListen
        {
            add { AddHandler(PreListenEvent, value); }
            remove { RemoveHandler(PreListenEvent, value); }
        }

        public static readonly RoutedEvent AddToWishlistEvent = EventManager.RegisterRoutedEvent(
    "AddToWishlist", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddToPlaylistUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler AddToWishlist
        {
            add { AddHandler(AddToWishlistEvent, value); }
            remove { RemoveHandler(AddToWishlistEvent, value); }
        }

        private bool showAddToWishlist;
        public bool ShowAddToWishlist
        {
            get
            {
                return showAddToWishlist;
            }
            set
            {
                showAddToWishlist = value;
                ImageAddToWishlist.Visibility = (showAddToWishlist == true) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public AddToPlaylistUserControl()
        {
            InitializeComponent();
        }
        private void ImagePlayNow_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/PlayNowHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayNow_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/PlayNow.png"));
            img.Source = bmp;
        }

        private void ImagePlayNext_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/PlayNextHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayNext_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/PlayNext.png"));
            img.Source = bmp;
        }

        private void ImagePlayLast_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/PlayLastHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayLast_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/PlayLast.png"));
            img.Source = bmp;
        }

        private void ImagePlayPreListen_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/PlayPreListenHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayPreListen_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/PlayPreListen.png"));
            img.Source = bmp;
        }

        private void ImageAddWish_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/AddWishHover.png"));
            img.Source = bmp;
        }

        private void ImageAddWish_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/AddWish.png"));
            img.Source = bmp;
        }

        private void ImagePlayNow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(AddToPlaylistUserControl.PlayNowEvent);
            RaiseEvent(newEventArgs);

            e.Handled = true;
        }

        private void ImagePlayNext_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(AddToPlaylistUserControl.PlayNextEvent);
            RaiseEvent(newEventArgs);

            e.Handled = true;
        }

        private void ImagePlayLast_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(AddToPlaylistUserControl.PlayLastEvent);
            RaiseEvent(newEventArgs);

            e.Handled = true;
        }

        private void ImagePlayPreListen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(AddToPlaylistUserControl.PreListenEvent);
            RaiseEvent(newEventArgs);

            e.Handled = true;
        }

        private void ImageAddWish_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(AddToPlaylistUserControl.AddToWishlistEvent);
            RaiseEvent(newEventArgs);

            e.Handled = true;
        }
    }
}
