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
using Big3.Hitbase.Controls.net.live.search.api;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Collections.ObjectModel;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using System.Text.RegularExpressions;

namespace Big3.Hitbase.Controls
{
    /// <summary>
    /// Interaction logic for PictureSearchPopup.xaml
    /// </summary>
    public partial class PictureSearchUserControl : UserControl
    {
        SafeObservableCollection<PictureSearchItem> PictureSearchItems = new SafeObservableCollection<PictureSearchItem>();

        const string AppId = "7FA6938E43CC48495B56FC5C397649DB0512D0C0";
        LiveSearchService liveSearchService;
        SearchRequest request;
        int currentPage = 0;
        string currentSearch = "";

        BitmapImage formSearchSelectedImage = null;

        public event EventHandler PictureSelected;
        public event EventHandler CloseClicked;

        public PictureSearchUserControl()
        {
            InitializeComponent();

            this.ListBoxCover.ItemsSource = PictureSearchItems;

            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            ButtonPrev.Visibility = currentPage > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Search(string searchText)
        {
            currentPage = 0;
            Search(searchText, 5);
            UpdateWindowState();
        }

        // searchText = Search for Text 
        // count = number of images per search request
        private void Search (string searchText, int maxCount)
        {
            currentSearch = searchText;

            PictureSearchItems.Clear();

            BackgroundWorker bwSearch = new BackgroundWorker();
            bwSearch.DoWork += delegate
            {
                request = new SearchRequest();
                // Common request fields (required)
                request.AppId = AppId;
                request.Query = searchText;
                request.Sources = new SourceType[] { SourceType.Image };

                // Common request fields (optional)
                //request.Version = "2.0";
                request.Market = "en-us";
                request.Adult = AdultOption.Moderate;
                request.AdultSpecified = true;

                // Image-specific request fields (optional)
                request.Image = new ImageRequest();
                request.Image.Count = (uint)maxCount;
                request.Image.CountSpecified = true;
                request.Image.Offset = (uint)currentPage*(uint)maxCount;
                request.Image.OffsetSpecified = true;

                /*TODO_WPF!!!!!!!!!!!!!!if (comboBoxSize.SelectedIndex == 1)
                    request.Image.Filters = new String[] { "Size:Small" };
                if (comboBoxSize.SelectedIndex == 2)
                    request.Image.Filters = new String[] { "Size:Medium" };
                if (comboBoxSize.SelectedIndex == 3)
                    request.Image.Filters = new String[] { "Size:Large" };*/

                // Send the request; display the response.
                //if (liveSearchService == null)
                //{
                //    liveSearchService = new LiveSearchService();
                //    liveSearchService.UseDefaultCredentials = true;
                //    liveSearchService.Proxy = HttpWebRequest.DefaultWebProxy;
                //    liveSearchService.Proxy.Credentials = CredentialCache.DefaultCredentials;
                //}

                //SearchResponse response = liveSearchService.Search(request);
                SearchImages(searchText, maxCount);

                /*if (response.Image != null && response.Image.Results != null)
                {
                    int count = 0;

                    // Obwohl nur maxCount Elemente angefragt werden, können wohl manchmal mehr zurückkommen.
                    foreach (ImageResult result in response.Image.Results)
                    {
                        if (count >= maxCount)
                            break;

                        LoadImage(result);

                        count++;
                    }
                }
                 */
            };
            bwSearch.RunWorkerCompleted += delegate
            {
                WaitProgress.Visibility = System.Windows.Visibility.Collapsed;

                if (PictureSearchItems.Count == 0)
                {
                    TextBlockNoImageFound.Visibility = System.Windows.Visibility.Visible;
                    ButtonPrev.Visibility = System.Windows.Visibility.Collapsed;
                    ButtonNext.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    TextBlockNoImageFound.Visibility = System.Windows.Visibility.Collapsed;
                    ButtonPrev.Visibility = System.Windows.Visibility.Visible;
                    ButtonNext.Visibility = System.Windows.Visibility.Visible;
                }
                UpdateWindowState();
            };
            bwSearch.RunWorkerAsync();
        }

        void SearchImages(string searchText, int maxCount)
        {
            WebClient wc = new WebClient();
            string[] allimages;

            // TODO OFFSET!!!
            string result = wc.DownloadString("http://www.bing.com/images/search?q=" + Uri.EscapeUriString(searchText));

            int index = 0;

            var m = Regex.Match(result, "(?<=img+.+src\\=[\x27\x22])(?<Url>[^\x27\x22]*)(?=[\x27\x22])", RegexOptions.IgnoreCase);

            int matchCount = 0;
            while (m.Success && matchCount < currentPage * 5 + 5)
            {
                for (int i = 1; i <= 2; i++)
                {
                    Group g = m.Groups[i];
                    CaptureCollection cc = g.Captures;
                    for (int j = 0; j < cc.Count; j++)
                    {
                        Capture c = cc[j];
                        string turl = c.ToString();
                        if (turl.StartsWith("http://"))
                        {
                            if (matchCount >= currentPage * 5)
                            {
                                LoadImage(turl, "", "", turl, "");
                            }
                            matchCount++;
                        }
                    }

                }
                m = m.NextMatch();
            }
            return;
            for (int i = 0; i < currentPage*5 + 5; i++)
            {
                int imageindex = result.IndexOf("mid=", index + 1);
                int imageindexEnd = result.IndexOf("mid=", imageindex + 10);
                if (imageindex < 1)
                {
                    continue;
                }

                if (imageindexEnd < imageindex)
                {
                    imageindexEnd = imageindex + 1000;
                }
                string foundimage = result.Substring(imageindex, imageindexEnd - imageindex);

                index = imageindexEnd - 10;

                int tindex = foundimage.IndexOf("img src=");
                int tindexEnd = foundimage.IndexOf("\"", tindex + 10);
                string turl = foundimage.Substring(tindex + 9, tindexEnd - tindex - 9);

                int nextindex = result.IndexOf("imgurl:&quot;", index + 13);

                // Größe, Ausmaße, Name, etc. ermitteln
                int indextitleend = 0;
                int indexsize = 0;
                int indexsizeend = 0;

                int indextitle = foundimage.IndexOf("t1=");
                indextitleend = foundimage.IndexOf("\"", indextitle + 4);
                string imageTitle = foundimage.Substring(indextitle + 4, indextitleend - indextitle - 4);

                indexsize = foundimage.IndexOf("t2=");
                indexsizeend = foundimage.IndexOf("\"", indexsize + 4);
                string imageSize2 = foundimage.Substring(indexsize + 4, indexsizeend - indexsize - 4);

                int sizewidthEnd = foundimage.IndexOf("x", indexsize + 4);
                int sizeheightEnd = foundimage.IndexOf(" ", sizewidthEnd + 2);
                string imageSize = foundimage.Substring(indexsize + 4, sizeheightEnd - indexsize - 4);

                int indexsizeFile = foundimage.IndexOf(" ", sizeheightEnd + 1);
                int indexsizeFileEnd = foundimage.IndexOf(" ", indexsizeFile + 1);
                string sizeFile = foundimage.Substring(indexsizeFile + 1, indexsizeFileEnd - indexsizeFile);
                uint kSize = Convert.ToUInt32(sizeFile);

                int indexurl = foundimage.IndexOf("t3=");
                int indexurlend = foundimage.IndexOf("\"", indexurl + 4);
                string orgurl = foundimage.Substring(indexurl + 4, indexurlend - indexurl - 4);

                int midurl = foundimage.IndexOf("mid=");
                int midurlend = foundimage.IndexOf("\"", midurl + 5);
                string mid = foundimage.Substring(midurl + 5, midurlend - midurl - 5);

                int nPos = 0;
                string iSize = "";
                string iTitle = "";

                if (i < currentPage * 5)
                    continue;

                if (indexsize > 0)
                {
                    if (imageSize.Length > 4)
                    {
                        string newurl = "http://" + orgurl;
                        LoadImage(turl, imageSize, imageTitle, newurl, mid);
                    }
                }

                if (index < 0)
                    break;
                /*                int tindex = result.IndexOf("turl:&quot;", index + 1);
                                int tindexEnd = result.IndexOf("&quot;", tindex + 11);
                                if (tindex < 1)
                                {
                                    continue;
                                }
                                string turl = result.Substring(tindex + 11, tindexEnd - tindex - 11);

                                index = result.IndexOf("imgurl:&quot;", index + 1);
                                int indexEnd = result.IndexOf("&quot;", index + 13);
                                string url = result.Substring(index + 13, indexEnd - index - 13);

                                int nextindex = result.IndexOf("imgurl:&quot;", index + 13);
                                //TODO!!!!!!!!!!!
                                // Größe, Ausmaße, Name, etc. ermitteln
                                int indexwidth = 0;
                                int indexheight = 0;
                                int indextitle = 0;
                                int indexwidthend = 0;
                                int indexheightend = 0;
                                int indextitleend = 0;
                                int nPos = result.IndexOf("offset:&quot;", indexEnd) + 13;
                                string iSize = "";
                                string iTitle = "";

                                indextitle = result.IndexOf("t:&quot;", nPos);
                                indexwidth = result.IndexOf("w:&quot;", nPos);
                                indexheight = result.IndexOf("h:&quot;", nPos);

                                if (i < currentPage * 5)
                                    continue;

                                if (nPos < index - 100)
                                {
                                    // No image width, height found
                                    break;
                                }
                                else
                                {
                                    if (indexwidth > indexEnd && indexheight > indexEnd)
                                    {
                                        indexwidthend = result.IndexOf("&quot;", indexwidth + 8);
                                        indexheightend = result.IndexOf("&quot;", indexheight + 8);
                                        indextitleend = result.IndexOf("&quot;", indextitle + 8);
                                        iSize = result.Substring(indexwidth + 8, indexwidthend - indexwidth - 8) + "x" + result.Substring(indexheight + 8, indexheightend - indexheight - 8);
                                        iTitle = result.Substring(indextitle + 8, indextitleend - indextitle - 8);
                                        LoadImage(turl, iSize, iTitle, url);
                                    }
                                }

                                if (index < 0)
                                    break;
                 */
            }
        }

        void LoadImage(string imageUrl, string iSize, string iTitle, string mediaUrl, string id)
        {
            try
            {
                PictureSearchItem psi = new PictureSearchItem();

                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    try
                    {
                        WebClient wc = new WebClient();
                        wc.UseDefaultCredentials = true;
                        wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
                        byte[] imageBytes = wc.DownloadData(imageUrl);

                        MemoryStream m = new MemoryStream(imageBytes);

                        psi.BitmapImage = ImageLoader.GetBitmapImageFromMemoryStream(m);
                        psi.BitmapImage.Freeze();

                        psi.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    catch
                    {
                    }
                };
                psi.Title = iTitle;
                psi.Size = iSize;
                
                psi.Visibility = System.Windows.Visibility.Visible;
                psi.ImageResult = new ImageResult();
                psi.ImageResult.MediaUrl = mediaUrl;
                psi.Id = id;
                PictureSearchItems.AddItemFromThread(psi);
                bw.RunWorkerAsync();
            }
            catch
            {
            }
        }


        void LoadImage(ImageResult result)
        {
            try
            {
                PictureSearchItem psi = new PictureSearchItem();

                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    try
                    {
                        WebClient wc = new WebClient();
                        wc.UseDefaultCredentials = true;
                        wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
                        byte[] imageBytes = wc.DownloadData(result.Thumbnail.Url);

                        MemoryStream m = new MemoryStream(imageBytes);

                        psi.BitmapImage = ImageLoader.GetBitmapImageFromMemoryStream(m);
                        psi.BitmapImage.Freeze();

                        psi.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    catch
                    {
                    }
                };
                psi.Title = result.Title;
                psi.Size = string.Format("{0} x {1} · {2} KB", result.Width, result.Height, (result.FileSize + 1023) / 1024);
                psi.Visibility = System.Windows.Visibility.Visible;
                psi.ImageResult = result;
                PictureSearchItems.AddItemFromThread(psi);
                bw.RunWorkerAsync();
            }
            catch
            {
            }
        }

        public PictureSearchItem SelectedItem
        {
            get
            {
                return ListBoxCover.SelectedItem as PictureSearchItem;
            }
        }

        public BitmapImage DownloadSelectedImage()
        {
            try
            {
                if (formSearchSelectedImage != null)
                {
                    return formSearchSelectedImage;
                }

                // Im Moment nur Vorschau-Bilder... Bing ist doof! :-)
                return SelectedItem.BitmapImage;

                if (SelectedItem == null)
                    return null;

                Mouse.OverrideCursor = Cursors.Wait;
                ImageResult imgRes = SelectedItem.ImageResult;

                WebClient wc = new WebClient();
                wc.UseDefaultCredentials = true;
                wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
                byte[] imageBytes = wc.DownloadData(imgRes.MediaUrl);

                MemoryStream m = new MemoryStream(imageBytes);
                BitmapImage bi = new BitmapImage();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.BeginInit();
                bi.StreamSource = m;
                bi.EndInit();
                bi.Freeze();

                return bi;
            }
            catch
            {
                Mouse.OverrideCursor = null;
                if (MessageBox.Show(StringTable.ErrorDownloadImage, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    return SelectedItem.BitmapImage;
                }

                return null;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            FormSearchImageInWeb formSearchInWeb = new FormSearchImageInWeb();
            formSearchInWeb.CoverType = DataBaseEngine.CoverType.PersonGroup;
            formSearchInWeb.SearchText = currentSearch;
            if (formSearchInWeb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                formSearchSelectedImage = formSearchInWeb.SelectedImage;
                if (PictureSelected != null)
                    PictureSelected(this, new EventArgs());
                if (CloseClicked != null)
                    CloseClicked(this, new EventArgs());
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CloseClicked != null)
                CloseClicked(this, new EventArgs());
        }

        private void ButtonPrev_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 0)
                currentPage--;

            Search(currentSearch, 5);

            UpdateWindowState();
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            currentPage++;

            Search(currentSearch, 5);

            UpdateWindowState();
        }

        private void ItemContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = VisualTreeExtensions.FindParent<ListBoxItem>(e.OriginalSource as DependencyObject);

            if (item != null)
            {
                PictureSearchItem pictureSearchItem = item.DataContext as PictureSearchItem;

                LoadOriginalImage(pictureSearchItem);
            }
        }

        private void LoadOriginalImage(PictureSearchItem pictureSearchItem)
        {
         /*   WebClient wc = new WebClient();
            string result = wc.DownloadString("http://www.bing.com/images/search?q=" + Uri.EscapeUriString(currentSearch) + "&view=detail&id=" + pictureSearchItem.Id);

            int aIndex = result.IndexOf("a class=\"i_iw");

            int tindex = result.IndexOf("img src=");
            int tindexEnd = result.IndexOf("\"", tindex + 10);
            string turl = result.Substring(tindex + 9, tindexEnd - tindex - 9);

            //....  img suchen und src extrahieren
            //pictureSearchItem.ImageResult.MediaUrl = "....";

            int orgindex = turl.IndexOf("url=");
            string orgUrl = turl.Substring(orgindex + 4);
            orgUrl = orgUrl.Replace("%2f", "/");
            orgUrl = orgUrl.Replace("%3a", ":");
            orgUrl = orgUrl.Replace("%2F", "/");
            orgUrl = orgUrl.Replace("%3A", ":");

            pictureSearchItem.ImageResult.MediaUrl = orgUrl;
            */


            if (PictureSelected != null)
                PictureSelected(this, new EventArgs());
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseImage.Source = ImageLoader.FromResource("CloseHover.png");
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseImage.Source = ImageLoader.FromResource("Close.png");
        }

    }

    public class PictureSearchItem : INotifyPropertyChanged
    {
        public string Title { get; set; }

        public string Size { get; set; }

        private BitmapImage bitmapImage;
        public BitmapImage BitmapImage 
        {
            get
            {
                return bitmapImage;
            }
            set
            {
                bitmapImage = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("BitmapImage"));
            }
        }

        private Visibility visibility = Visibility.Collapsed;
        public Visibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Visibility"));
            }
        }

        public ImageResult ImageResult { get; set; }

        public string Id { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
