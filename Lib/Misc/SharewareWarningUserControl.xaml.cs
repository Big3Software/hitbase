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
using Big3.Hitbase.SharedResources;
using System.Windows.Threading;

namespace Big3.Hitbase.Miscellaneous
{
    /// <summary>
    /// Interaction logic for SharewareWarningUserControl.xaml
    /// </summary>
    public partial class SharewareWarningUserControl : UserControl, IModalUserControl
    {
        DispatcherTimer dt = new DispatcherTimer();

        int waitSeconds;

        public SharewareWarningUserControl(int waitSeconds)
        {
            InitializeComponent();

            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();

            this.waitSeconds = waitSeconds;
        }



        void dt_Tick(object sender, EventArgs e)
        {
            waitSeconds--;
            TextBlockPleaseWait.Text = string.Format(StringTable.PleaseWait, waitSeconds);
            if (waitSeconds == 0)
            {
                dt.Stop();
                TextBlockPleaseWait.Visibility = System.Windows.Visibility.Collapsed;
                buttonOK.IsEnabled = true;
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (OKClicked != null)
                OKClicked(this, new EventArgs());
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            dt.Stop();
        }

        public event EventHandler OKClicked;

        public event EventHandler CancelClicked;
    }
}
