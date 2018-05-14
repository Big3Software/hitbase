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
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SoundEngine;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.SoundEngineGUI
{
    /// <summary>
    /// Interaction logic for MiniPlayerWindow.xaml
    /// </summary>
    public partial class MiniPlayerWindow : Window
    {
        public MiniPlayerWindow(DataBase db)
        {
            InitializeComponent();

            MiniPlayerUserControl.DataBase = db;
        }

        public static MiniPlayerWindow PreListenWindow = null;

        public static void PreListen(Track track, DataBase db, Window parent)
        {
            if (PreListenWindow == null)
                PreListenWindow = new MiniPlayerWindow(db);

            PreListenWindow.Owner = parent;
            PreListenWindow.Show();
            //PreListenWindow.Topmost = true;
            //PreListenWindow.Play(track);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ImageClose_MouseEnter(object sender, MouseEventArgs e)
        {
            ImageClose.Source = new BitmapImage(new Uri("pack://application:,,,/SoundEngineGUI;component/Images/CloseButtonHover.png"));
        }

        private void ImageClose_MouseLeave(object sender, MouseEventArgs e)
        {
            ImageClose.Source = new BitmapImage(new Uri("pack://application:,,,/SoundEngineGUI;component/Images/CloseButton.png"));
        }

        internal void HideWindow()
        {
            Hide();
            SoundEngine.SoundEngine.PreListen.Stop();
        }

        private void ImageClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HideWindow();
            e.Handled = true;
        }

        private void ProgressBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ImageClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
