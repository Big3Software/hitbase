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
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace UpdateHitbase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dt = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();

            dt.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
            buttonPrev.IsEnabled = false;
        }

        void dt_Tick(object sender, EventArgs e)
        {
            SetPage(0);
            dt.Stop();
        }

        private UserControl[] pages = { new PageWelcome(), new PageReadme(), new PageInstallNow(), new PageFinish() };
        int pageIndex = 0;
        int currentFlipPage = 0;

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            if (pageIndex == pages.Length - 1)
            {
                Close();
                return;
            }
            pageIndex++;

            SetPage(pageIndex);
        }

        private void SetPage(int pageIndex)
        {
            if (pageIndex == 0)
            {
                buttonPrev.IsEnabled = false;
            }
            else
            {
                buttonPrev.IsEnabled = true;
            }

            if (pageIndex == pages.Length - 1)
            {
                buttonNext.IsEnabled = false;
            }
            else
            {
                buttonNext.IsEnabled = true;
            }

            IWizardPage wizardPage = pages[pageIndex] as IWizardPage;
            if (wizardPage != null && wizardPage.NextButtonDisabled)
                buttonNext.IsEnabled = false;

            if (currentFlipPage == 0)
            {
                page1.Content = pages[pageIndex];
                PageTransition(page2, page1);
                currentFlipPage = 1;
            }
            else
            {
                page2.Content = pages[pageIndex];
                PageTransition(page1, page2);
                currentFlipPage = 0;
            }
        }

        private void PageTransition(ContentControl page1, ContentControl page2)
        {
            page1.Opacity = 0;
            page2.Opacity = 0;
            Duration duration = TimeSpan.FromMilliseconds(200).Duration();
            DoubleAnimation daOut = new DoubleAnimation(0, duration);
            daOut.Completed += delegate
            {
                page1.Visibility = System.Windows.Visibility.Collapsed;
                page2.Visibility = System.Windows.Visibility.Visible;
                page1.Opacity = 0;
                page2.Opacity = 1;
            };
            page1.BeginAnimation(ContentControl.OpacityProperty, daOut);

            DoubleAnimation daIn = new DoubleAnimation(1, duration);
            daIn.BeginTime = TimeSpan.FromMilliseconds(200);
            page2.Visibility = System.Windows.Visibility.Visible;
            page2.BeginAnimation(ContentControl.OpacityProperty, daIn);

            TranslateTransform translateTransform = new TranslateTransform();
            page2.RenderTransform = translateTransform;
            DoubleAnimation daMoveIn = new DoubleAnimation(20, 0, duration);
            daMoveIn.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
            daMoveIn.BeginTime = TimeSpan.FromMilliseconds(200);
            translateTransform.BeginAnimation(TranslateTransform.XProperty, daMoveIn);
        }

        private void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            pageIndex--;

            SetPage(pageIndex);
        }

        private void CommandBindingNextPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            pageIndex++;
            SetPage(pageIndex);
        }

        private void CommandBindingHideNavigationButtons_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            buttonNext.Visibility = System.Windows.Visibility.Collapsed;
            buttonPrev.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void CommandBindingShowCloseButton_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            buttonNext.Visibility = System.Windows.Visibility.Visible;
            buttonNext.IsEnabled = true;
            buttonNext.Content = "Schließen";
        }

    }
}
