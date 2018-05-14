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
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Windows.Threading;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.Application
{
    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow : Window
    {
        DispatcherTimer dt = new DispatcherTimer();

        public SplashScreenWindow()
        {
            InitializeComponent();

            FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string version = string.Format("{0}.{1}.{2}", fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart);

            textBlockVersion.Text = string.Format("Version {0}", version);

            // Spätestens nach 5 Sekunden ausblenden
            dt.Interval = TimeSpan.FromSeconds(5);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            CloseSplashScreen();
        }

        public void CloseSplashScreen()
        {
            dt.Stop();

            this.Dispatcher.BeginInvoke((Action)(delegate  
                {
                DoubleAnimation da = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500).Duration());
                da.Completed += new EventHandler(da_Completed);
                this.BeginAnimation(Window.OpacityProperty, da);
                }
            ));
        }

        void da_Completed(object sender, EventArgs e)
        {
            this.Close();

            System.Windows.Application.Current.MainWindow.Activate();
        }
    }
}
