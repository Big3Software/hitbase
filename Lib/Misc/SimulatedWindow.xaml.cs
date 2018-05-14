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

namespace Big3.Hitbase.Miscellaneous
{
    /// <summary>
    /// Interaction logic for SimulatedWindow.xaml
    /// </summary>
    public partial class SimulatedWindow : UserControl
    {
        public ModalDialogResult DialogResult = ModalDialogResult.None;

        public SimulatedWindow()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(SimulatedWindow_Loaded);
        }

        void SimulatedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FadeIn();
        }

        private void FadeIn()
        {
            DoubleAnimation da = new DoubleAnimation(0.8, 1, TimeSpan.FromMilliseconds(400).Duration());
            BackEase backEase = new BackEase();
            backEase.Amplitude = 0.5;
            da.EasingFunction = backEase;
            ScaleTransform st = new ScaleTransform();
            st.CenterX = ActualWidth / 2;
            st.CenterY = ActualHeight / 2;
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da);
            this.RenderTransform = st;
        }

        private void FadeOut()
        {
            DoubleAnimation da = CreateFadeOutAnimation();
            DoubleAnimation da2 = CreateFadeOutAnimation();

            da.Completed += new EventHandler(da_Completed);

            ScaleTransform st = new ScaleTransform();
            st.CenterX = ActualWidth / 2;
            st.CenterY = ActualHeight / 2;
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da2);
            
            this.RenderTransform = st;
        }

        void da_Completed(object sender, EventArgs e)
        {
            Window window = Window.GetWindow(this);

            IModalService modalService = window as IModalService;

            if (DialogResult == Miscellaneous.ModalDialogResult.OK)
                modalService.GoBackward(true);

            if (DialogResult == Miscellaneous.ModalDialogResult.Cancel)
                modalService.GoBackward(false);
        }

        private static DoubleAnimation CreateFadeOutAnimation()
        {
            DoubleAnimation da = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200).Duration());
            BackEase backEase = new BackEase();
            backEase.EasingMode = EasingMode.EaseIn;
            backEase.Amplitude = 0.5;
            da.EasingFunction = backEase;
            return da;
        }

        public UserControl WindowContent
        {
            get
            {
                return (UserControl)windowContent.Content;
            }
            set
            {
                windowContent.Content = value;

                IModalUserControl modalUserControl = value as IModalUserControl;
                if (modalUserControl != null)
                {
                    modalUserControl.OKClicked += new EventHandler(modalUserControl_OKClicked);
                    modalUserControl.CancelClicked += new EventHandler(modalUserControl_CancelClicked);
                }
            }
        }

        public void Close()
        {
            DialogResult = Miscellaneous.ModalDialogResult.Cancel;

            FadeOut();
        }

        void modalUserControl_CancelClicked(object sender, EventArgs e)
        {
            DialogResult = Miscellaneous.ModalDialogResult.Cancel;
            FadeOut();
        }

        void modalUserControl_OKClicked(object sender, EventArgs e)
        {
            DialogResult = Miscellaneous.ModalDialogResult.OK;
            FadeOut();
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                WindowTextBlock.Text = value;
            }
        }

        #region Fenster verschieben
        bool moveActive = false;
        Point moveMouseStartPosition;
        Thickness moveStartMargin;
        private void MoveGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !moveActive)
            {
                moveActive = true;
                moveMouseStartPosition = Mouse.GetPosition(null);
                moveStartMargin = Margin;
                MoveGrid.CaptureMouse();
            }
        }

        private void MoveGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveActive)
            {
                Point curPos = Mouse.GetPosition(null);
                Margin = new Thickness(moveStartMargin.Left + (curPos.X - moveMouseStartPosition.X) * 2,
                                       moveStartMargin.Top + (curPos.Y - moveMouseStartPosition.Y) * 2, 0, 0);
            }
        }

        private void MoveGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (moveActive)
            {
                MoveGrid.ReleaseMouseCapture();
                moveActive = false;
            }
        }
        #endregion

        #region Fenster vergrößern/verkleinern
        bool resizeActive = false;
        Point resizeMouseStartPosition;
        Size resizeStartSize;
        private void ResizeGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !resizeActive)
            {
                resizeActive = true;
                resizeMouseStartPosition = Mouse.GetPosition(null);
                resizeStartSize = new Size(((UserControl)windowContent.Content).Width, ((UserControl)windowContent.Content).Height);
                ResizeGrid.CaptureMouse();
            }
        }

        private void ResizeGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizeActive)
            {
                Point curPos = Mouse.GetPosition(null);

                double newWidth = resizeStartSize.Width + (curPos.X - resizeMouseStartPosition.X) * 2;
                if (newWidth >= 200)
                    ((UserControl)windowContent.Content).Width = newWidth;

                double newHeight = resizeStartSize.Height + (curPos.Y - resizeMouseStartPosition.Y) * 2;
                if (newHeight >= 200)
                    ((UserControl)windowContent.Content).Height = newHeight;
            }
        }

        private void ResizeGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (resizeActive)
            {
                ResizeGrid.ReleaseMouseCapture();
                resizeActive = false;
            }
        }
        #endregion

        private void CloseImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DialogResult = Miscellaneous.ModalDialogResult.Cancel;
            FadeOut();
            e.Handled = true;
        }

        private bool allowClose;
        public bool AllowClose
        {
            get
            {
                return allowClose;
            }
            set
            {
                allowClose = value;

                if (allowClose == false)
                {
                    CloseImage.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    CloseImage.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
    }

    public interface IModalUserControl
    {
        event EventHandler OKClicked;
        event EventHandler CancelClicked;
    }

    public enum ModalDialogResult
    {
        None,
        OK,
        Cancel
    }
}
