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
using Big3.Hitbase.SharedResources;
using Microsoft.Win32;

namespace Big3.Hitbase.Miscellaneous
{
    public enum WpfMessageBoxButtons
    {
        None,
        YesNo,
        DeleteCancel,
        OK
    }

    public enum WpfMessageBoxResult
    {
        None,
        OK,
        Cancel,
        Yes,
        No,
        Delete
    }

    /// <summary>
    /// Interaction logic for WPFMessageBox.xaml
    /// </summary>
    public partial class WPFMessageBox : Window
    {
        public WPFMessageBox()
        {
            InitializeComponent();

            DataContext = this;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.HideMinimizeAndMaximizeButtons();

            switch (WpfMessageBoxButtons)
            {
                case WpfMessageBoxButtons.None:
                    break;
                case WpfMessageBoxButtons.YesNo:
                    ButtonOK.Content = StringTable.Yes;
                    ButtonCancel.Content = StringTable.No;
                    break;
                case WpfMessageBoxButtons.DeleteCancel:
                    ButtonOK.Content = StringTable.Delete;
                    ButtonCancel.Content = StringTable.Cancel;
                    break;
                case WpfMessageBoxButtons.OK:
                    ButtonCancel.Visibility = System.Windows.Visibility.Collapsed;
                    ButtonOK.Content = StringTable.OK;
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(DontShowAgainText))
            {
                CheckBoxDontShowAgain.Content = DontShowAgainText;
            }
        }

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(WPFMessageBox), new UIPropertyMetadata(""));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WPFMessageBox), new UIPropertyMetadata(""));




        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(string), typeof(WPFMessageBox), new UIPropertyMetadata(""));




        public WpfMessageBoxButtons WpfMessageBoxButtons
        {
            get { return (WpfMessageBoxButtons)GetValue(WpfMessageBoxButtonsProperty); }
            set { SetValue(WpfMessageBoxButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WpfMessageBoxButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WpfMessageBoxButtonsProperty =
            DependencyProperty.Register("WpfMessageBoxButtons", typeof(WpfMessageBoxButtons), typeof(WPFMessageBox), new UIPropertyMetadata(WpfMessageBoxButtons.None));





        public WpfMessageBoxResult WpfMessageBoxResult
        {
            get { return (WpfMessageBoxResult)GetValue(WpfMessageBoxResultProperty); }
            set { SetValue(WpfMessageBoxResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WpfMessageBoxResult.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WpfMessageBoxResultProperty =
            DependencyProperty.Register("WpfMessageBoxResult", typeof(WpfMessageBoxResult), typeof(WPFMessageBox), new UIPropertyMetadata(WpfMessageBoxResult.None));




        public string DontShowAgainText
        {
            get { return (string)GetValue(DontShowAgainTextProperty); }
            set { SetValue(DontShowAgainTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DontShowAgainText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DontShowAgainTextProperty =
            DependencyProperty.Register("DontShowAgainText", typeof(string), typeof(WPFMessageBox), new UIPropertyMetadata(null));



        private bool saveAnswer;

        public bool SaveAnswer
        {
            get 
            { 
                return saveAnswer; 
            }
            set 
            { 
                saveAnswer = value;
                if (value == true)
                    CheckBoxDontShowAgain.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private bool zoomImage;

        public bool ZoomImage
        {
            get { return zoomImage; }
            set 
            { 
                zoomImage = value;
                this.image.Stretch = (value == true) ? Stretch.Uniform : Stretch.None;
            }
        }

        public string SaveAnswerInRegistryKey { get; set; }

        public static WpfMessageBoxResult Show(Window parent, string headerText, string text, string image, WpfMessageBoxButtons wpfMessageBoxButtons, string saveAnswerInRegistryKey = "", bool zoomImage = true, int height = -1)
        {
            if (!string.IsNullOrEmpty(saveAnswerInRegistryKey))
            {
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Global.HitbaseRegistryKey))
                {
                    int show = (int)regKey.GetValue(saveAnswerInRegistryKey + "Show", 1);

                    WpfMessageBoxResult value = (WpfMessageBoxResult)regKey.GetValue(saveAnswerInRegistryKey + "Value", (int)WpfMessageBoxResult.None);

                    if (show == 0)
                        return value;
                }
            }

            WPFMessageBox msgBox = new WPFMessageBox();
            msgBox.HeaderText = headerText;
            msgBox.Text = text;
            msgBox.Image = image;
            msgBox.Owner = parent;
            msgBox.WpfMessageBoxButtons = wpfMessageBoxButtons;
            msgBox.SaveAnswer = !string.IsNullOrEmpty(saveAnswerInRegistryKey);
            msgBox.SaveAnswerInRegistryKey = saveAnswerInRegistryKey;
            msgBox.ZoomImage = zoomImage;
            if (height != -1)
                msgBox.Height = height;
            msgBox.ShowDialog();

            return msgBox.WpfMessageBoxResult;
        }

        public WpfMessageBoxResult ShowDialogEventually()
        {
            if (!string.IsNullOrEmpty(SaveAnswerInRegistryKey))
            {
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Global.HitbaseRegistryKey))
                {
                    int show = (int)regKey.GetValue(SaveAnswerInRegistryKey + "Show", 1);

                    WpfMessageBoxResult value = (WpfMessageBoxResult)regKey.GetValue(SaveAnswerInRegistryKey + "Value", (int)WpfMessageBoxResult.None);

                    if (show == 0)
                        return value;
                }
            }

            ShowDialog();

            return WpfMessageBoxResult;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (WpfMessageBoxButtons == WpfMessageBoxButtons.YesNo)
                WpfMessageBoxResult = WpfMessageBoxResult.Yes;

            if (WpfMessageBoxButtons == WpfMessageBoxButtons.DeleteCancel)
                WpfMessageBoxResult = WpfMessageBoxResult.Delete;

            if (WpfMessageBoxButtons == WpfMessageBoxButtons.OK)
                WpfMessageBoxResult = WpfMessageBoxResult.OK;

            if (!string.IsNullOrEmpty(SaveAnswerInRegistryKey))
            {
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Global.HitbaseRegistryKey, true))
                {
                    regKey.SetValue(SaveAnswerInRegistryKey + "Show", this.CheckBoxDontShowAgain.IsChecked == true ? 0 : 1);

                    regKey.SetValue(SaveAnswerInRegistryKey + "Value", (int)WpfMessageBoxResult);
                }
            }

            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (WpfMessageBoxButtons == WpfMessageBoxButtons.YesNo)
                WpfMessageBoxResult = WpfMessageBoxResult.No;

            if (WpfMessageBoxButtons == WpfMessageBoxButtons.DeleteCancel)
                WpfMessageBoxResult = WpfMessageBoxResult.Cancel;

            Close();
        }


    }
}
