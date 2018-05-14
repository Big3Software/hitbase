using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using System.Net;
using Big3.Hitbase.Controls.net.live.search.api;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Web;
using Big3.Hitbase.Configuration;
using System.Windows.Media.Imaging;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Miscellaneous;
using System.Text.RegularExpressions;

namespace Big3.Hitbase.Controls
{
    public partial class FormSearchImageInWeb : Form
    {
        // Das ist unsere AppId von Microsoft
        SearchRequest request;
        uint lastCount;
        int currentPage = 0;
        ImageResult selectedImageResult = null;
        Panel selectedPanel = null;
        string queryString;
        SafeObservableCollection<PictureSearchItem> PictureSearchItems = new SafeObservableCollection<PictureSearchItem>();

        public FormSearchImageInWeb()
        {
            InitializeComponent();
            Settings.RestoreWindowPlacement(this, "SearchImageInWeb");
        }

        // searchText = Search for Text 
        // count = number of images per search request
        private SearchResponse SearchImageRequest (string searchText, int count)
        {
            tableLayoutPanel.Controls.Clear();

            if (count > 0)
            {
                request = new SearchRequest();
                // Common request fields (required)
                //request.AppId = AppId;
                request.Query = searchText;
                request.Sources = new SourceType[] { SourceType.Image };

                // Common request fields (optional)
                //request.Version = "2.1";
                request.Market = "en-us";
                request.Adult = AdultOption.Moderate;
                request.AdultSpecified = true;

                // Image-specific request fields (optional)
                request.Image = new ImageRequest();
                request.Image.Count = (uint)count;
                request.Image.CountSpecified = true;
                request.Image.Offset = (uint)currentPage*(uint)count;
                request.Image.OffsetSpecified = true;

                if (comboBoxSize.SelectedIndex == 1)
                    request.Image.Filters = new String[] { "Size:Small" };
                if (comboBoxSize.SelectedIndex == 2)
                    request.Image.Filters = new String[] { "Size:Medium" };
                if (comboBoxSize.SelectedIndex == 3)
                    request.Image.Filters = new String[] { "Size:Large" };

                //request.Image.Filters = new String[] { "Face:Portrait", "Size:Medium" };

                lastCount = (uint)count;
            }
            else
            {
                request.Image.Offset = request.Image.Offset + lastCount;
            }

            // Send the request; display the response.
            //if (liveSearchService == null)
            //    liveSearchService = new LiveSearchService();
            //SearchResponse response = liveSearchService.Search(request);
            PictureSearchItems.Clear();
            SearchImages(searchText, 20);
            int col = 0;
            int row = 0;

            foreach (PictureSearchItem psi in PictureSearchItems)
            {
                Panel panel = new Panel();
                panel.BackColor = Color.White;
                panel.Dock = DockStyle.Fill;
                panel.Padding = new Padding(10);
                panel.Tag = psi.ImageResult;

                PictureBox pb = new PictureBox();
                pb.Tag = psi.ImageResult.Url;
                pb.Image = null;
                pb.SizeMode = PictureBoxSizeMode.Zoom;
                pb.Dock = DockStyle.Fill;
                pb.MouseEnter += new EventHandler(panel_MouseEnter);
                pb.MouseLeave += new EventHandler(panel_MouseLeave);
                pb.MouseClick += new MouseEventHandler(item_MouseClick);
                pb.MouseDoubleClick += new MouseEventHandler(item_MouseDoubleClick);

                panel.Controls.Add(pb);

                Label lbl = new Label();
                lbl.AutoSize = false;
                lbl.Height = 20;
                lbl.BackColor = Color.Transparent;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = psi.ImageResult.Title;
                lbl.Dock = DockStyle.Bottom;
                lbl.MouseEnter += new EventHandler(panel_MouseEnter);
                lbl.MouseLeave += new EventHandler(panel_MouseLeave);
                lbl.MouseClick += new MouseEventHandler(item_MouseClick);
                lbl.MouseDoubleClick += new MouseEventHandler(item_MouseDoubleClick);
                panel.Controls.Add(lbl);

                Label lblSize = new Label();
                lblSize.AutoSize = false;
                lblSize.Height = 20;
                lblSize.BackColor = Color.Transparent;
                lblSize.TextAlign = ContentAlignment.MiddleCenter;
                if (psi.Size != "" || psi.ImageResult.FileSize != 0)
                {
                    lblSize.Text = string.Format("{0} · {1} KB", psi.Size, psi.ImageResult.FileSize);
                }
                else
                {
                    lblSize.Text = "";
                }
                lblSize.Dock = DockStyle.Bottom;
                lblSize.MouseEnter += new EventHandler(panel_MouseEnter);
                lblSize.MouseLeave += new EventHandler(panel_MouseLeave);
                lblSize.MouseClick += new MouseEventHandler(item_MouseClick);
                lblSize.MouseDoubleClick += new MouseEventHandler(item_MouseDoubleClick);
                panel.Controls.Add(lblSize);

                tableLayoutPanel.Controls.Add(panel);
                tableLayoutPanel.SetCellPosition(panel, new TableLayoutPanelCellPosition(col, row));
                Console.WriteLine(psi.ImageResult.MediaUrl);
                Console.WriteLine("Page Title: " + psi.ImageResult.Title);
                Console.WriteLine("Page URL: " + psi.ImageResult.Url);
                Console.WriteLine(
                    "Dimensions: "
                    + psi.ImageResult.Width
                    + "x"
                    + psi.ImageResult.Height);
                Console.WriteLine("Thumbnail URL: " + psi.ImageResult.DisplayUrl);
                Console.WriteLine();
                col++;
                if (col == 5)
                {
                    col = 0;
                    row++;
                }

                LoadImage(pb);
               // LoadImage(psi.ImageResult.Url, psi.Size, psi.Title, psi.ImageResult.MediaUrl);
                //Thread thread = new Thread(new ParameterizedThreadStart(LoadImage));
                //thread.Start(pb);
            }

            pagerControl.NumberOfPages = 2;// (int)PictureSearchItems.Count / count;

            /*if (response.Image != null && response.Image.Results != null)
            {
                int col = 0;
                int row = 0;
                foreach (ImageResult result in response.Image.Results)
                {
                    Panel panel = new Panel();
                    panel.BackColor = Color.White;
                    panel.Dock = DockStyle.Fill;
                    panel.Padding = new Padding(10);
                    panel.Tag = result;

                    PictureBox pb = new PictureBox();
                    pb.Tag = result.Thumbnail.Url;
                    pb.Image = null;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;
                    pb.Dock = DockStyle.Fill;
                    pb.MouseEnter += new EventHandler(panel_MouseEnter);
                    pb.MouseLeave += new EventHandler(panel_MouseLeave);
                    pb.MouseClick += new MouseEventHandler(item_MouseClick);
                    pb.MouseDoubleClick += new MouseEventHandler(item_MouseDoubleClick);

                    panel.Controls.Add(pb);

                    Label lbl = new Label();
                    lbl.AutoSize = false;
                    lbl.Height = 20;
                    lbl.BackColor = Color.Transparent;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.Text = result.Title;
                    lbl.Dock = DockStyle.Bottom;
                    lbl.MouseEnter += new EventHandler(panel_MouseEnter);
                    lbl.MouseLeave += new EventHandler(panel_MouseLeave);
                    lbl.MouseClick += new MouseEventHandler(item_MouseClick);
                    lbl.MouseDoubleClick += new MouseEventHandler(item_MouseDoubleClick);
                    panel.Controls.Add(lbl);

                    Label lblSize = new Label();
                    lblSize.AutoSize = false;
                    lblSize.Height = 20;
                    lblSize.BackColor = Color.Transparent;
                    lblSize.TextAlign = ContentAlignment.MiddleCenter;
                    lblSize.Text = string.Format("{0} x {1} · {2} KB", result.Width.ToString(), result.Height.ToString(), (result.FileSize + 1023) / 1024);
                    lblSize.Dock = DockStyle.Bottom;
                    lblSize.MouseEnter += new EventHandler(panel_MouseEnter);
                    lblSize.MouseLeave += new EventHandler(panel_MouseLeave);
                    lblSize.MouseClick += new MouseEventHandler(item_MouseClick);
                    lblSize.MouseDoubleClick += new MouseEventHandler(item_MouseDoubleClick);
                    panel.Controls.Add(lblSize);

                    tableLayoutPanel.Controls.Add(panel);
                    tableLayoutPanel.SetCellPosition(panel, new TableLayoutPanelCellPosition(col, row));
                    Console.WriteLine(result.MediaUrl);
                    Console.WriteLine("Page Title: " + result.Title);
                    Console.WriteLine("Page URL: " + result.Url);
                    Console.WriteLine(
                        "Dimensions: "
                        + result.Width
                        + "x"
                        + result.Height);
                    Console.WriteLine("Thumbnail URL: " + result.Thumbnail.Url);
                    Console.WriteLine();
                    col++;
                    if (col == 5)
                    {
                        col = 0;
                        row++;
                    }

                    LoadImage(pb);
                    //Thread thread = new Thread(new ParameterizedThreadStart(LoadImage));
                    //thread.Start(pb);
                }

                pagerControl.NumberOfPages = (int)response.Image.Total / count;
            }
            else
            {
                pagerControl.NumberOfPages = 0;
            }

            return response;
             * */
            return null;
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
            while (m.Success && matchCount < currentPage * 20 + 20)
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
                            if (matchCount >= currentPage * 20)
                            {
                                LoadImage(turl, "", "", 0, turl, "");
                            }
                            matchCount++;
                        }
                    }

                }
                m = m.NextMatch();
            }
            return;

            for (int i = 0; i < currentPage * 20 + 20; i++)
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

                int idurl = foundimage.IndexOf("mid=");
                int idurlend = foundimage.IndexOf("\"", idurl + 5);
                string id = foundimage.Substring(idurl + 5, idurlend - idurl - 5);

                int nPos = 0;
                string iSize = "";
                string iTitle = "";

                if (i < currentPage * 20)
                    continue;

                if (indexsize > 0)
                {
                    if (imageSize.Length > 4)
                    {
                        LoadImage(turl, imageSize, imageTitle, kSize, orgurl, id);
                    }
                }

                if (index < 0)
                    break;
            }
        }

        void LoadImage(string imageUrl, string iSize, string iTitle, uint kSize, string mediaUrl, string id)
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
                psi.ImageResult.DisplayUrl = imageUrl;
                psi.ImageResult.Url = imageUrl;
                psi.ImageResult.Thumbnail = new Thumbnail();
                psi.ImageResult.Thumbnail.Url = imageUrl;
                psi.ImageResult.FileSize = kSize;
                //psi.ImageResult.Id = id;
                psi.Id = id;
                PictureSearchItems.AddItemFromThread(psi);
                bw.RunWorkerAsync();
            }
            catch
            {
            }
        }

        void item_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //PictureSearchItem pictureSearchItem = PictureSearchItems[0];

            if (!DownloadSelectedImage())
                return;

            DialogResult = DialogResult.OK;
            Close();
        }

        void item_MouseClick(object sender, MouseEventArgs e)
        {
            Control c = (Control)sender;
            if (!(c is Panel))
            {
                Panel p = (Panel)c.Parent;
                ImageResult imgRes = (ImageResult)p.Tag;
                selectedImageResult = imgRes;
                if (selectedPanel != null)
                    selectedPanel.BackColor = Color.White;
                selectedPanel = p;
                c.Parent.BackColor = Color.FromArgb(162,199,235);
                UpdateWindowState();
            }
        }

        void LoadImage(object data)
        {
            try
            {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    try
                    {
                        PictureBox pb = (PictureBox)data;
                        string url = (string)pb.Tag;
                        WebClient wc = new WebClient();
                        wc.UseDefaultCredentials = true;
                        wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
                        byte[] imageBytes = wc.DownloadData(url);

                        MemoryStream m = new MemoryStream(imageBytes);
                        Image image = Image.FromStream(m);

                        args.Result = image;
                    }
                    catch
                    {
                    }
                };
                bw.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    PictureBox pb = (PictureBox)data;
                    pb.Image = args.Result as Image;
                };
                bw.RunWorkerAsync();
            }
            catch
            {
            }
        }

        void panel_MouseLeave(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            if (!(c is Panel))
            {
                Panel p = (Panel)c.Parent;
                ImageResult imgRes = (ImageResult)p.Tag;
                if (selectedImageResult == imgRes)
                    p.BackColor = Color.FromArgb(162, 199, 235);
                else
                    p.BackColor = Color.White;
            }
        }

        void panel_MouseEnter(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            if (!(c is Panel))
            {
                Panel p = (Panel)c.Parent;
                ImageResult imgRes = (ImageResult)p.Tag;
                if (selectedImageResult != imgRes)
                    c.Parent.BackColor = Color.WhiteSmoke;
            }
        }

        #region Properties
        /// <summary>
        /// Die zugehörige CD (nur wenn CoverType == Front, Back oder Label)
        /// </summary>
        public CD CD { get; set; }

        /// <summary>
        /// Der Suchstring
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Der Bildtyp, nachdem gesucht wird.
        /// </summary>
        public CoverType CoverType { get; set; }

        private BitmapImage selectedImage;
        public BitmapImage SelectedImage 
        {
            get
            {
                return selectedImage;
            }
        }
        #endregion

        private void FormSearchImageInWeb_Load(object sender, EventArgs e)
        {
            comboBoxSize.Items.Add(StringTable.All);
            comboBoxSize.Items.Add(StringTable.Small);
            comboBoxSize.Items.Add(StringTable.Medium);
            comboBoxSize.Items.Add(StringTable.Large);

            if (CoverType == CoverType.PersonGroup)
            {
                queryString = SearchText;
            }
            else
            {
                queryString = CD.Artist + " " + CD.Title;
            }

            textBoxSearchString.Text = queryString;

            // Führt implizit die Suche aus, weil die Selection geändert wird.
            comboBoxSize.SelectedIndex = 0;

            UpdateWindowState();
        }

        private void SearchNow()
        {
            try
            {
                SearchImageRequest(queryString, 20);
            }
            catch
            {
            }
        }

        private void pagerControl_PageChanged()
        {
            currentPage = pagerControl.CurrentPage;

            selectedImageResult = null;
            UpdateWindowState();

            SearchNow();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //PictureSearchItem pictureSearchItem = PictureSearchItems[0];

            if (!DownloadSelectedImage())
                DialogResult = DialogResult.None;
            else
                DialogResult = DialogResult.OK;
        }

        private bool DownloadSelectedImage()
        {
            try
            {
                // Nur Vorschau im Moment....
                WebClient wc = new WebClient();
                wc.UseDefaultCredentials = true;
                wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
                byte[] imageBytes = wc.DownloadData(selectedImageResult.Thumbnail.Url);

                MemoryStream m = new MemoryStream(imageBytes);

                BitmapImage bi = new BitmapImage();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.BeginInit();
                bi.StreamSource = m;
                bi.EndInit();
                bi.Freeze();

                selectedImage = bi;

                return true;
                /*
                
                WebClient wcneu = new WebClient();

                string result = wcneu.DownloadString("http://www.bing.com/images/search?q=" + Uri.EscapeUriString(textBoxSearchString.Text) + "&view=detail&id=" + selectedImageResult.Id);
                //PictureSearchItems.Id
                 
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

                Cursor.Current = Cursors.WaitCursor;
                ImageResult imgRes = selectedImageResult;
                
                // Neue URL
                imgRes.MediaUrl = orgUrl;

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

                selectedImage = bi;*/
            }
            catch
            {
                Cursor.Current = Cursors.Default;
                if (MessageBox.Show(StringTable.ErrorDownloadImage, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WebClient wc = new WebClient();
                    wc.UseDefaultCredentials = true;
                    wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    byte[] imageBytes = wc.DownloadData(selectedImageResult.Thumbnail.Url);

                    MemoryStream m = new MemoryStream(imageBytes);

                    BitmapImage bi = new BitmapImage();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.BeginInit();
                    bi.StreamSource = m;
                    bi.EndInit();
                    bi.Freeze();

                    selectedImage = bi;

                    return true;
                }

                return false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return true;

        }

        private void UpdateWindowState()
        {
            buttonOK.Enabled = (selectedImageResult != null);
        }

        private void comboBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 0;
            pagerControl.CurrentPage = 0;

            SearchNow();
        }

        private void pictureBoxBing_Click(object sender, EventArgs e)
        {
            string url = string.Format("http://www.bing.com/images/search?q={0}", System.Uri.EscapeDataString(queryString));
            Process.Start(url);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            queryString = textBoxSearchString.Text;
            SearchNow();
        }

        private void FormSearchImageInWeb_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.SaveWindowPlacement(this, "SearchImageInWeb");
        }
    }
}
