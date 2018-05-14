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
    /// Interaction logic for WindowMP3HowTo.xaml
    /// </summary>
    public partial class WindowFLACHowTo : Window
    {
        DispatcherTimer _Timer;

        public WindowFLACHowTo()
        {
            InitializeComponent();

            //textMP3Found.Visibility = Visibility.Hidden;
            //mp3dllFound.Visibility = Visibility.Hidden;

            _Timer = new DispatcherTimer();
            _Timer.Interval = TimeSpan.FromMilliseconds(1000);
            _Timer.Tick += new EventHandler(_Timer_Tick);
            _Timer.Start();
        }
        
        void _Timer_Tick(object sender, EventArgs e)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "flac.exe"))
            {
                //mp3dllFound.Visibility = Visibility.Visible;
                //textMP3Found.Visibility = Visibility.Visible;
                flacdllFound.Source = new BitmapImage(new Uri("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/Success.png"));
                textFlacFound.Text = "Flac.exe im Hitbase Verzeichnis gefunden, Sie können nun das Flac-Format erzeugen.";
                button1.Content = "OK";
            }
            else
            {
                //mp3dllFound.Visibility = Visibility.Hidden;
                //textMP3Found.Visibility = Visibility.Hidden;
                flacdllFound.Source = new BitmapImage(new Uri("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/Warning32.png"));
                textFlacFound.Text = "Flac.exe nicht im Hitbase Verzeichnis! Flac Format kann nicht erzeugt werden!";
                button1.Content = "Abbrechen";
            }
        }

        private void hyperSearchForFlac_Click(object sender, RoutedEventArgs e)
        {
            // MP3 DLL
            Process.Start("http://www.rarewares.org/lossless.php");
        }

        private void hyperOpenHitbaseDir_Click(object sender, RoutedEventArgs e)
        {
            string hitbasedir;
            hitbasedir = "file://" + AppDomain.CurrentDomain.BaseDirectory;

            Process.Start(hitbasedir);
        }
    }
}
