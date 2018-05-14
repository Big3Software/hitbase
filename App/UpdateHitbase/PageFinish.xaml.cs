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

namespace UpdateHitbase
{
    /// <summary>
    /// Interaction logic for PageWelcome.xaml
    /// </summary>
    public partial class PageFinish : UserControl
    {
        public PageFinish()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Commands.ShowCloseButton.Execute(null, this);

            if (App.ErrorCount > 0)
            {
                image.Source = new BitmapImage(new Uri("pack://application:,,,/UpdateHitbase;component/Error.png"));
                textBlockResult.Text = "Update fehlerhaft!";
                textBlockResult.Foreground = Brushes.DarkRed;
                textBlockResult2.Text = "Beim Aktualisieren von Hitbase 2012 sind Fehler aufgetreten. Die Fehlerdetails sind:";
                textBoxErrorDetails.Visibility = System.Windows.Visibility.Visible;
                textBoxErrorDetails.Text = App.LogMessages.ToString();
            }
        }
    }
}
