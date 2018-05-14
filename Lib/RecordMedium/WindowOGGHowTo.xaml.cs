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
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
using System.Reflection;

namespace Big3.Hitbase.RecordMedium
{
    /// <summary>
    /// Interaction logic for WindowOggHowTo.xaml
    /// </summary>
    public partial class WindowOGGHowTo : Window
    {
        DispatcherTimer _Timer;

        public WindowOGGHowTo()
        {
            InitializeComponent();

            //textOggFound.Visibility = Visibility.Hidden;
            //OggdllFound.Visibility = Visibility.Hidden;

            _Timer = new DispatcherTimer();
            _Timer.Interval = TimeSpan.FromMilliseconds(1000);
            _Timer.Tick += new EventHandler(_Timer_Tick);
            _Timer.Start();
        }
        
        void _Timer_Tick(object sender, EventArgs e)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "oggenc2.exe"))
            {
                //OggdllFound.Visibility = Visibility.Visible;
                //textOggFound.Visibility = Visibility.Visible;
                oggdllFound.Source = new BitmapImage(new Uri("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/Success.png"));
                textOggFound.Text = "oggenc2.exe im Hitbase Verzeichnis gefunden, Sie können nun Ogg Dateien erzeugen.";
                button1.Content = "OK";
            }
            else
            {
                //mp3dllFound.Visibility = Visibility.Hidden;
                //textMP3Found.Visibility = Visibility.Hidden;
                oggdllFound.Source = new BitmapImage(new Uri("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/Warning32.png"));
                textOggFound.Text = "oggenc2.exe nicht im Hitbase Verzeichnis! Ogg Dateien können nicht erzeugt werden!";
                button1.Content = "Abbrechen";
            }
        }

        private void hyperSearchForOgg_Click(object sender, RoutedEventArgs e)
        {
            // MP3 DLL
            Process.Start("http://www.rarewares.org/ogg-oggenc.php");
        }

        private void hyperOpenHitbaseDir_Click(object sender, RoutedEventArgs e)
        {
            string hitbasedir;
            hitbasedir = "file://" + AppDomain.CurrentDomain.BaseDirectory;

            Process.Start(hitbasedir);
        }
    }
}
