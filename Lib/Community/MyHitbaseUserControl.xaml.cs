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
using Big3.Hitbase.DataBaseEngine;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Diagnostics;
using System.ComponentModel;
using Big3.Hitbase.SharedResources;
using System.Threading;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.Controls;
using Big3.Hitbase.Configuration;

namespace Community
{
    /// <summary>
    /// Interaction logic for MyHitbaseUserControl.xaml
    /// </summary>
    public partial class MyHitbaseUserControl : UserControl, IModalUserControl
    {
        Exception lastException = null;
        private bool uploadInProgress = false;

        private const string myHitbaseHostName = "myhitbase.de";

        WebClient webClient = new WebClient();

        public MyHitbaseUserControl()
        {
            InitializeComponent();

            webClient.UseDefaultCredentials = true;
            webClient.Proxy.Credentials = CredentialCache.DefaultCredentials;

            UpdateWindowState();

            textBoxUser.Text = Settings.Current.MyHitbaseLastUserName;
            if (!string.IsNullOrEmpty(Settings.Current.MyHitbaseLastPassword))
            {
                textBoxPassword.Password = EncodeDecode.Decrypt(Settings.Current.MyHitbaseLastPassword, "myHitbase");
                checkBoxSaveCredentials.IsChecked = true;
            }

            checkBoxDontSendCover.IsChecked = Settings.Current.MyHitbaseDontSendImages;

            Loaded += new RoutedEventHandler(MyHitbaseUserControl_Loaded);
        }

        void MyHitbaseUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxUser.Focus();
        }

        public DataBase DataBase { get; set; }

        private void buttonUpload_Click(object sender, RoutedEventArgs e)
        {
            if (uploadInProgress)
            {
                webClient.CancelAsync();
                buttonUpload.Content = StringTable.UploadNow;
                progressBar.Visibility = System.Windows.Visibility.Collapsed;
                TextBlockError.Visibility = System.Windows.Visibility.Collapsed;
                textBlockStatus.Text = "";
                textBlockStatus.Visibility = System.Windows.Visibility.Visible;
                progressBar.Value = 0;

                uploadInProgress = false;

                UpdateWindowState();

                return;
            }

            if (checkBoxSaveCredentials.IsChecked == true)
            {
                Settings.Current.MyHitbaseLastUserName = textBoxUser.Text;

                Settings.Current.MyHitbaseLastPassword = Big3.Hitbase.Miscellaneous.EncodeDecode.Encrypt(textBoxPassword.Password, "myHitbase");
            }
            else
            {
                Settings.Current.MyHitbaseLastUserName = "";

                Settings.Current.MyHitbaseLastPassword = "";
            }

            Settings.Current.MyHitbaseDontSendImages = checkBoxDontSendCover.IsChecked.Value;

            uploadInProgress = true;
            UpdateWindowState();

            string userName = textBoxUser.Text;
            string password = textBoxPassword.Password;
            progressBar.Visibility = System.Windows.Visibility.Visible;

            buttonUpload.Content = StringTable.Cancel;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                string uploadFilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(DataBase.DataBasePath), "myhitbase.sdf");

                UpdateStatusText(StringTable.PrepareCatalog + "...");

                PrepareDataBaseForUpload(uploadFilename);

                if (!uploadInProgress)
                    return;

                FileInfo fi = new FileInfo(uploadFilename);
                string compressedFilename = Compress(fi);

                if (!uploadInProgress)
                    return;

                UpdateStatusText(StringTable.SendCatalog + "...");

                if (!string.IsNullOrEmpty(compressedFilename))
                    UploadCatalog(compressedFilename, userName, password);

            };
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerAsync();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                progressBar.Visibility = System.Windows.Visibility.Collapsed;
                TextBlockError.Visibility = System.Windows.Visibility.Visible;
                textBlockStatus.Visibility = System.Windows.Visibility.Collapsed;
                lastException = e.Error;
            }
        }

        private void UpdateStatusText(string text)
        {
            if (Thread.CurrentThread == Dispatcher.Thread)
            {
                textBlockStatus.Text = text;
            }
            else
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    UpdateStatusText(text);
                }));
            }

        }

        private void UploadCatalog(string catalogFilename, string userName, string password)
        {
            if (!this.uploadInProgress)
                return;

            FileInfo fi = new FileInfo(catalogFilename);
            
            this.Dispatcher.Invoke((Action)(() =>
            {
                progressBar.Maximum = (int)fi.Length;
            }));

            string hostName = "http://" + myHitbaseHostName + "/myhitbase/UploadCatalog.cshtml";

#if DEBUG
//            if (Environment.MachineName == "JUS2")
//                hostName = "http://localhost:9694/UploadCatalog.cshtml";
#endif

            Uri uri = new Uri(hostName);
            webClient.Headers.Add("user", userName);
            webClient.Headers.Add("password", password);
            webClient.UploadProgressChanged += new UploadProgressChangedEventHandler(wc_UploadProgressChanged);
            webClient.UploadFileCompleted += new UploadFileCompletedEventHandler(wc_UploadFileCompleted);
            webClient.UploadFileAsync(uri, "POST", catalogFilename);
        }

        void wc_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            if (e.Error != null && !e.Cancelled)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    buttonUpload.Content = StringTable.UploadNow;
                    progressBar.Visibility = System.Windows.Visibility.Collapsed;
                    TextBlockError.Visibility = System.Windows.Visibility.Visible;
                    textBlockStatus.Visibility = System.Windows.Visibility.Collapsed;
                    lastException = e.Error;
                }));

            }
            else
            {
                string resultString = "";

                if (!e.Cancelled)
                    resultString = Encoding.Default.GetString(e.Result);
                else
                    resultString = "CANCELLED";
                bool success = false;
                switch (resultString)
                {
                    case "USER_OR_PASSWORD_INCORRECT":
                        UpdateStatusText(StringTable.UserOrPasswordIncorrect);

                        break;
                    case "CANCELLED":
                        break;
                    default:
                        UpdateStatusText(StringTable.Ready);
                        success = true;
                        break;
                }

                uploadInProgress = false;

                Dispatcher.Invoke((Action)(() =>
                {
                    buttonUpload.Content = StringTable.UploadNow;
                    progressBar.Visibility = System.Windows.Visibility.Collapsed;
                    TextBlockError.Visibility = System.Windows.Visibility.Collapsed;
                    textBlockStatus.Visibility = System.Windows.Visibility.Visible;

                    if (success)
                    {
                        GridLeftColumn.Width = new GridLength(0);
                        GridCenterColumn.Width = new GridLength(0);
                        GridRightColumn.Width = new GridLength(1, GridUnitType.Star);

                    }

                    UpdateWindowState();
                }));
            }
        }

        void wc_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            

            this.Dispatcher.Invoke((Action)(() =>
            {
                progressBar.Value = e.BytesSent;
            }));            
        }

        public static string Compress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Prevent compressing hidden and 
                // already compressed files.
                if ((File.GetAttributes(fi.FullName)
                    & FileAttributes.Hidden)
                    != FileAttributes.Hidden & fi.Extension != ".gz")
                {
                    // Create the compressed file.
                    using (FileStream outFile =
                                File.Create(fi.FullName + ".gz"))
                    {
                        using (GZipStream Compress =
                            new GZipStream(outFile,
                            CompressionMode.Compress))
                        {
                            // Copy the source file into 
                            // the compression stream.
                            inFile.CopyTo(Compress);
                            return fi.FullName + ".gz";
                        }
                    }
                }
            }

            return "";
        }

        private void PrepareDataBaseForUpload(string uploadFilename)
        {
            bool dontSendCover = false;
            File.Copy(DataBase.DataBasePath, uploadFilename, true);

            Dispatcher.Invoke((Action)(() =>
            {
                progressBar.Maximum = 100;
                dontSendCover = checkBoxDontSendCover.IsChecked == true;
            }));

            DataBase db = new Big3.Hitbase.DataBaseEngine.DataBase();
            db.Open(uploadFilename);
            db.PrepareForUpload(OnPrepareProgress, dontSendCover);
            db.Close();

            if (!uploadInProgress)
                return;

            UpdateStatusText(StringTable.CompressCatalog + "...");

            db.Compress();
        }

        private bool OnPrepareProgress(double percentage)
        {
            if (!this.uploadInProgress)
                return false;

            Dispatcher.Invoke((Action)(() =>
            {
                progressBar.Value = percentage;
            }));

            return true;
        }

        private void HyperlinkRegister_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://" + myHitbaseHostName + "/myhitbase" );
        }

        private void HyperlinkErrorDetails_Click(object sender, RoutedEventArgs e)
        {
            WindowShowDetails windowShowDetails = new WindowShowDetails(lastException.ToString());

            windowShowDetails.ShowDialog();
        }

        private void textBoxUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void textBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonUpload.IsEnabled = textBoxUser.Text.Length > 0 && textBoxPassword.Password.Length > 0;

            textBoxUser.IsEnabled = !uploadInProgress;
            textBoxPassword.IsEnabled = !uploadInProgress;
            this.checkBoxSaveCredentials.IsEnabled = !uploadInProgress;
            this.checkBoxDontSendCover.IsEnabled = !uploadInProgress;

            ButtonClose.IsEnabled = !uploadInProgress;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(this, new EventArgs());
        }

        public event EventHandler OKClicked;

        public event EventHandler CancelClicked;

        private void HyperlinkShowOnline_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://" + myHitbaseHostName + "/myhitbase/ListAlbums?user=" + Uri.EscapeUriString(textBoxUser.Text));
        }
    }
}
