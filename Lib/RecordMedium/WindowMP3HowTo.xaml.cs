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
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.RecordMedium
{
    /// <summary>
    /// Interaction logic for WindowMP3HowTo.xaml
    /// </summary>
    public partial class WindowMP3HowTo : Window
    {
        DispatcherTimer _Timer;

        public WindowMP3HowTo()
        {
            InitializeComponent();

            //textMP3Found.Visibility = Visibility.Hidden;
            //mp3dllFound.Visibility = Visibility.Hidden;

            _Timer = new DispatcherTimer();
            _Timer.Interval = TimeSpan.FromMilliseconds(1000);
            _Timer.Tick += new EventHandler(_Timer_Tick);
            _Timer.Start();
        }

        public bool IsOK { get; set; }

        void _Timer_Tick(object sender, EventArgs e)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "lame_enc.dll"))
            {
                int prozType = 0;

                prozType = GetDLLInfo.GetUnmanagedDllType(AppDomain.CurrentDomain.BaseDirectory + "lame_enc.dll");

                switch (prozType)
                {
                    case 32:
                            //mp3dllFound.Visibility = Visibility.Visible;
                            //textMP3Found.Visibility = Visibility.Visible;
                            mp3dllFound.Source = new BitmapImage(new Uri("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/Success.png"));
                            textMP3Found.Text = "lame_enc.dll im Hitbase Verzeichnis gefunden.\nSie können nun MP3s erzeugen.";
                            IsOK = true;
                            button1.Content = "OK";
                            break;
                    case 64:
                            //mp3dllFound.Visibility = Visibility.Visible;
                            //textMP3Found.Visibility = Visibility.Visible;
                            mp3dllFound.Source = new BitmapImage(new Uri("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/Warning32.png"));
                            textMP3Found.Text = "lame_enc.dll 64-Bit im Hitbase Verzeichnis gefunden.\nBitte kopieren Sie die 32-Bit Version ins Verzeichnis!";
                            IsOK = false;
                            button1.Content = "Abbrechen";
                            break;
                    default:
                            //mp3dllFound.Visibility = Visibility.Visible;
                            //textMP3Found.Visibility = Visibility.Visible;
                            mp3dllFound.Source = new BitmapImage(new Uri("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/Warning32.png"));
                            textMP3Found.Text = "lame_enc.dll liegt im Hitbase Verzeichnis.\nDies scheint aber keine korrekte DLL zu sein!";
                            IsOK = false;
                            button1.Content = "Abbrechen";
                            break;
                }
            }
            else
            {
                //mp3dllFound.Visibility = Visibility.Hidden;
                //textMP3Found.Visibility = Visibility.Hidden;
                mp3dllFound.Source = new BitmapImage(new Uri("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/Warning32.png"));
                textMP3Found.Text = "lame_enc.dll nicht im Hitbase Verzeichnis!\nMP3s können nicht erzeugt werden!";
                IsOK = false;
                button1.Content = "Abbrechen";
            }
        }

        private void hyperSearchForLame_Click(object sender, RoutedEventArgs e)
        {
            // MP3 DLL
            Process.Start("http://www.bing.com/search?q=lame_enc.dll+3.98.4&go=&form=QBRE&filt=all");
        }

        private void hyperOpenHitbaseDir_Click(object sender, RoutedEventArgs e)
        {
            string hitbasedir;
            hitbasedir = "file://" + AppDomain.CurrentDomain.BaseDirectory;

            Process.Start(hitbasedir);
        }
    }
}
