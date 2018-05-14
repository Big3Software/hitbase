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
using System.ServiceModel;
using System.IO;
using Community.ChatService;

namespace Community
{
    /// <summary>
    /// Interaction logic for CommunityRoom.xaml
    /// </summary>
    public partial class CommunityRoom : Window
    {
        private bool moveActive = false;
        private Point lastMousePosition;

        ChatServiceClient chatService;
        public Session Session;
        ColorChooseSlider colorChooseSlider = new ColorChooseSlider();

        public CommunityRoom()
        {
            InitializeComponent();

            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(new GradientStop(Color.FromRgb(0xFF,0xFF,0xFF), 0));
            gsc.Add(new GradientStop(Color.FromRgb(0xAA, 0xAA, 0xAA), 0.4));
            gsc.Add(new GradientStop(Color.FromRgb(0x88, 0x88, 0x88), 0.401));
            gsc.Add(new GradientStop(Color.FromRgb(0x77, 0x77, 0x77), 1));
            Brush brush = new LinearGradientBrush(gsc, 90);
            brush.Opacity = 0.6;
            GridHeader.Background = brush;

            Brush mainBrush = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
            mainBrush.Opacity = 0.6;
            MainGrid.Background = mainBrush;

            this.GridBackground.Background = new SolidColorBrush(Colors.Blue);
            this.MouseDown += new MouseButtonEventHandler(CommunityRoom_MouseDown);
            this.MouseMove += new MouseEventHandler(CommunityRoom_MouseMove);
            this.MouseUp += new MouseButtonEventHandler(CommunityRoom_MouseUp);

            this.Closing += new System.ComponentModel.CancelEventHandler(CommunityRoom_Closing);

            buttonSmiley.Click += new RoutedEventHandler(buttonSmiley_Click);

            //MyCallback cb = new MyCallback(this);
            //InstanceContext ic = new InstanceContext(cb);
            chatService = new ChatServiceClient();

            richTextBoxHistory.Document.Blocks.Clear();
            richTextBoxMessage.PreviewKeyDown += new KeyEventHandler(richTextBoxMessage_PreviewKeyDown);
            colorChooseSlider.VerticalAlignment = VerticalAlignment.Top;
            colorChooseSlider.HorizontalAlignment = HorizontalAlignment.Right;
            colorChooseSlider.Margin = new Thickness(0, 40, 10, 0);
            colorChooseSlider.Visibility = Visibility.Hidden;
            this.GridBackground.Children.Add(colorChooseSlider);
            colorChooseSlider.ColorChoosen += new ColorChooseSlider.ColorChoosenHandler(colorChooseSlider_ColorChoosen);
            buttonColor.Click += new RoutedEventHandler(buttonColor_Click);
        }

        void colorChooseSlider_ColorChoosen(Color color)
        {
            this.GridBackground.Background = new SolidColorBrush(color);
        }

        void buttonColor_Click(object sender, RoutedEventArgs e)
        {
            if (colorChooseSlider.Visibility == Visibility.Hidden)
                colorChooseSlider.Visibility = Visibility.Visible;
            else
                colorChooseSlider.Visibility = Visibility.Hidden;
        }

        void richTextBoxMessage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private void SendMessage()
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                richTextBoxMessage.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                richTextBoxMessage.Document.ContentEnd
                );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            string msg = textRange.Text;
            if (msg.EndsWith("\r\n"))
                msg = msg.Substring(0, msg.Length - 2);
            //chatService.SendBroadcastMessage(Session, msg);

            richTextBoxMessage.Document.Blocks.Clear();
        }

        void buttonSmiley_Click(object sender, RoutedEventArgs e)
        {
            Image img = new Image();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("pack://application:,,,/Community;component/Images/Smiley1.png");
            bi.EndInit();
            img.Source = bi;
            img.Stretch = Stretch.None;

            InlineUIContainer uicontainer = new InlineUIContainer(img);
            Span span = new Span(uicontainer);
            TextPointer tp = richTextBoxMessage.CaretPosition;
            tp.Paragraph.Inlines.Add(uicontainer);
        }

        void CommunityRoom_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            chatService.Logout(Session.User.Name);
        }

        #region Mouse
        void CommunityRoom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            moveActive = true;
            lastMousePosition = e.GetPosition(this);
            CaptureMouse();
        }

        void CommunityRoom_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveActive)
            {
                this.Left += e.GetPosition(this).X - lastMousePosition.X;
                this.Top += e.GetPosition(this).Y - lastMousePosition.Y;
                //lastMousePosition = e.GetPosition(this);
            }
        }

        void CommunityRoom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (moveActive)
            {
                ReleaseMouseCapture();
                moveActive = false;
            }
        }

        #endregion

        public void FillUserList()
        {
            listViewUsers.Items.Clear();

            User[] currentUsers = chatService.GetUserList();
            foreach (User user in currentUsers)
            {
                AddUserToListView(user, listViewUsers);
            }
        }

        private void AddUserToListView(User user, ListView listViewUsers)
        {
            Grid userGrid = new Grid();
            ColumnDefinition imageColumn = new ColumnDefinition();
            imageColumn.Width = new GridLength(60);
            userGrid.ColumnDefinitions.Add(imageColumn);
            ColumnDefinition detailsColumn = new ColumnDefinition();
            userGrid.ColumnDefinitions.Add(detailsColumn);

            /*BitmapImage bmpImage = new BitmapImage();
            Stream ImageStream = new MemoryStream(user.Image);
            bmpImage.BeginInit();
            bmpImage.StreamSource = ImageStream;
            bmpImage.EndInit();
            Image img = new Image();
            img.Source = bmpImage;
            userGrid.Children.Add(img);
            Grid.SetRow(img, 0);
            Grid.SetColumn(img, 0);*/

            StackPanel sp = new StackPanel();

            Label lbl = new Label();
            lbl.Content = user.Name;
            sp.Children.Add(lbl);

            Hyperlink hl = new Hyperlink(new Run("Meine Lieblingsmusik"));
            TextBlock tbFavorites = new TextBlock(hl);
            sp.Children.Add(tbFavorites);

            userGrid.Children.Add(sp);
            Grid.SetColumn(sp, 1);
            listViewUsers.Items.Add(userGrid);
        }

        internal void NewMessage(string username, string message)
        {
            string currentTime = string.Format("{0:d2}:{1:d2}", DateTime.Now.Hour, DateTime.Now.Minute);

            Paragraph p = new Paragraph();
            p.Margin = new Thickness(0);
            p.Inlines.Add(string.Format("{0} sagt ({1}):", username, currentTime));
            p.Inlines.Add(new LineBreak());
            p.Inlines.Add("   " + message);
            richTextBoxHistory.Document.Blocks.Add(p);

            richTextBoxHistory.ScrollToEnd();
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        internal void SessionCreated()
        {
            /*BitmapImage bmpImage = new BitmapImage();
            Stream ImageStream = new MemoryStream(Session.User.Image);
            bmpImage.BeginInit();
            bmpImage.StreamSource = ImageStream;
            bmpImage.EndInit();
            imageMe.Source = bmpImage;*/
            textBlockMyName.Text = Session.User.Name;
        }

        public void Login(string username, string password)
        {
            Credentials credentials = new Credentials();
            credentials.Username = username;
            credentials.Password = password;

            Session = chatService.Login(credentials);
        }
    }
}
