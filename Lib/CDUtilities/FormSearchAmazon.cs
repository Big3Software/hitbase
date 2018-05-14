using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.CDUtilities.AmazonServiceReference;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;
using System.Windows;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormSearchAmazon : Form
    {
        // Use one of the following destinations, according to the region you are
        // interested in:
        // 
        //      US: ecs.amazonaws.com 
        //      CA: ecs.amazonaws.ca 
        //      UK: ecs.amazonaws.co.uk 
        //      DE: ecs.amazonaws.de 
        //      FR: ecs.amazonaws.fr 
        //      JP: ecs.amazonaws.jp
        //
        // Note: protocol must be https for signed SOAP requests.
        const String DESTINATION = "https://ecs.amazonaws.de/onca/soap?Service=AWSECommerceService";

        // Set your AWS Access Key ID and AWS Secret Key here.
        // You can obtain them at:
        // http://aws-portal.amazon.com/gp/aws/developer/account/index.html?action=access-key
        const String MY_AWS_ID = "0FN016GTSMZHJD0C7YG2";
        const String MY_AWS_SECRET = "1TC2Dytk/uauXGMmyivyMA4S4MZkQ4dYlyWtuxuA";

        internal enum AmazonSort
        {
            TitleAscend,
            TitleDescend,
            YearDescend,
            YearAscend,
            BestSelling
        }

        internal class AmazonSortItem
        {
            internal AmazonSortItem(AmazonSort s, String display)
            {
                sort = s;
                sortDisplay = display;
            }

            internal AmazonSort sort;
            internal String sortDisplay;

            public override string ToString()
            {
                return sortDisplay;
            }
        }

        private DataBase dataBase;

        public FormSearchAmazon(DataBase dataBase)
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);

            this.dataBase = dataBase;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            SearchNow(0);
        }

        private void SearchNow(int page)
        {
            listBoxResult.Items.Clear();

            amazonProgressControl.labelStatus.Text = StringTable.SearchWebService;
            amazonProgressControl.progressBar.Value = 0;

            amazonProgressControl.Visible = true;
            amazonProgressControl.Refresh();

            ItemSearchRequest request = new ItemSearchRequest();
            request.Artist = textBoxArtist.Text;
            request.Title = textBoxTitle.Text;
            request.Keywords = textBoxEAN.Text;
            request.ItemPage = (page+1).ToString();
            request.ResponseGroup = new string[]{"Large", "Tracks"}; 
            

            request.SearchIndex = "Music";

            AmazonSort sort = ((AmazonSortItem)comboBoxSort.Items[comboBoxSort.SelectedIndex]).sort;

            switch (sort)
            {
                case AmazonSort.YearDescend:
                    request.Sort = "-releasedate";
                    break;
                case AmazonSort.YearAscend:
                    request.Sort = "releasedate";
                    break;
                case AmazonSort.TitleAscend:
                    request.Sort = "titlerank";
                    break;
                case AmazonSort.TitleDescend:
                    request.Sort = "-titlerank";
                    break;
                case AmazonSort.BestSelling:
                    request.Sort = "salesrank";
                    break;
            }

            AWSECommerceServicePortTypeClient amazonClient = new AWSECommerceServicePortTypeClient();

            ItemSearch itemSearch = new ItemSearch();
            itemSearch.Request = new ItemSearchRequest[] { request };
            itemSearch.AWSAccessKeyId = "0FN016GTSMZHJD0C7YG2";
            itemSearch.AssociateTag = "hitbase-21";
            
            bool nothingFound = false;
            ItemSearchResponse searchResult = null;
            try
            {
                searchResult = amazonClient.ItemSearch(itemSearch);
            }
            catch
            {
                nothingFound = true;
            }

            if (!nothingFound)
            {
                int totalCount = Convert.ToInt32(searchResult.Items[0].TotalResults);
                labelResult.Text = String.Format(StringTable.Result, totalCount);

                if (totalCount < 1)         // Leider nichts gefunden
                {
                    amazonProgressControl.Visible = false;
                    return;
                }

                WebClient webclient = new WebClient();
                webclient.UseDefaultCredentials = true;
                webclient.Proxy.Credentials = CredentialCache.DefaultCredentials;

                amazonProgressControl.progressBar.Maximum = searchResult.Items[0].Item.Length;

                int count = 0;
                foreach (Items searchItems in searchResult.Items)
                { 
                    foreach (Item details in searchItems.Item)
                    {
                        amazonProgressControl.labelStatus.Text = String.Format(StringTable.ReadingResult, count + 1, searchItems.Item.Length);
                        amazonProgressControl.labelStatus.Refresh();

                        CDListBoxWithCoverItem item = new CDListBoxWithCoverItem();
                        if (details.ItemAttributes.Artist != null && details.ItemAttributes.Artist.Length > 0)
                            item.Artist = details.ItemAttributes.Artist[0].ToString();
                        item.Title = details.ItemAttributes.Title;

                        item.Label = details.ItemAttributes.Label;
                        if (details.ItemAttributes.ReleaseDate != null)
                        {
                            item.Year = details.ItemAttributes.ReleaseDate;
                        }
                        else
                        {
                            if (details.ItemAttributes.PublicationDate != null)
                                item.Year = details.ItemAttributes.PublicationDate;
                        }
                        item.ASIN = details.ASIN;
                        item.EAN = details.ItemAttributes.EAN;

                        /*if (details.CustomerReviews != null)
                            item.Ranking = details.CustomerReviews.AverageRating;
                        else
                            item.Ranking = 0;*/

                        if (details.EditorialReviews != null && details.EditorialReviews.Length > 0)
                            item.EditorNotes = details.EditorialReviews[0].Content;

                        if (details.Tracks != null)
                        {
                            item.CD = new CDItem[details.Tracks.Length];
                            foreach (TracksDisc disc in details.Tracks)
                            {
                                int discNumber = Convert.ToInt32(disc.Number) - 1;
                                item.CD[discNumber] = new CDItem();
                                item.CD[discNumber].Tracks = new string[disc.Track.Length];
                                foreach (TracksDiscTrack track in disc.Track)
                                {
                                    int trackNumber = Convert.ToInt32(track.Number) - 1;

                                    // Das hier wird wohl (von WCF?) f‰lschlicherweise als CodePage 1252 interpretiert, obwohl es UTF-8 ist.
                                    // Muss dann konvertiert werden.
                                    Encoding iso = Encoding.GetEncoding("Windows-1252");
                                    Encoding utf8 = Encoding.UTF8;
                                    byte[] utfBytes = iso.GetBytes(track.Value);
                                    string msg = utf8.GetString(utfBytes);

                                    item.CD[discNumber].Tracks[trackNumber] = msg;
                                }
                            }
                        }
                        else
                        {
                            // Wenn keine Track-Daten vorhanden sind, ein Pseudo-Track erzeugen
                            item.CD = new CDItem[1];
                            item.CD[0] = new CDItem();
                            item.CD[0].Tracks = new string[1];
                            item.CD[0].Tracks[0] = StringTable.Unknown;
                        }

                        if (details.SmallImage != null)
                            item.smallImageUrl = details.SmallImage.URL;

                        if (details.MediumImage != null)
                            item.mediumImageUrl = details.MediumImage.URL;

                        if (details.LargeImage != null)
                            item.largeImageUrl = details.LargeImage.URL;

                        if (details.ImageSets != null && details.ImageSets.Length > 0)
                        {
                            //foreach (ImageSet[] imgSet in details.ImageSets)
                            {
                                foreach (ImageSet imageSet in details.ImageSets)
                                {
                                    if (imageSet.Category == "variant")
                                    {
                                        if (imageSet.SmallImage != null)
                                            item.backCoverSmallImageUrl = imageSet.SmallImage.URL;

                                        if (imageSet.MediumImage != null)
                                            item.backCoverMediumImageUrl = imageSet.MediumImage.URL;

                                        if (imageSet.LargeImage != null)
                                            item.backCoverLargeImageUrl = imageSet.LargeImage.URL;

                                    }
                                }
                            }
                        }

                        item.Image = GetImage(webclient, details);

                        listBoxResult.Items.Add(item);

                        amazonProgressControl.progressBar.PerformStep();

                        count++;
                    }
                }

                if (totalCount > 10)
                {
                    pagerControl.NumberOfPages = (totalCount + 9) / 10;
                    pagerControl.Visible = true;
                }
                else
                {
                    pagerControl.Visible = false;
                }
            }
            else
            {
                labelResult.Text = String.Format(StringTable.Result, 0);
                pagerControl.Visible = false;
            }

            amazonProgressControl.Visible = false;
        }

        /// <summary>
        /// Das Bild ermitteln. Es wird zuerst nach dem kleinen, dann dem mittleren und zuletzt dem groﬂen Bild gesucht.
        /// </summary>
        /// <param name="webclient"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        private System.Drawing.Image GetImage(WebClient webclient, Item details)
        {
            System.Drawing.Image image = null;

            try
            {
                if (details.SmallImage != null && details.SmallImage.URL != "")
                {
                    byte[] bytes = webclient.DownloadData(details.SmallImage.URL);
                    if (bytes.Length >= 1000)
                    {
                        MemoryStream memStream = new MemoryStream(bytes);
                        image = System.Drawing.Image.FromStream(memStream);
                    }
                }

                // Noch keins gefunden, vielleicht haben wir ein mittel groﬂes?
                if (image == null)
                {
                    if (details.MediumImage != null && details.MediumImage.URL != "")
                    {
                        byte[] bytes = webclient.DownloadData(details.MediumImage.URL);
                        if (bytes.Length >= 1000)
                        {
                            MemoryStream memStream = new MemoryStream(bytes);
                            image = System.Drawing.Image.FromStream(memStream);
                        }
                    }
                }

                // Immer noch nichts? Dann haben wir zu guter letzt noch ein groﬂes Bild?
                if (image == null)
                {
                    if (details.LargeImage != null && details.LargeImage.URL != "")
                    {
                        byte[] bytes = webclient.DownloadData(details.LargeImage.URL);
                        if (bytes.Length >= 1000)
                        {
                            MemoryStream memStream = new MemoryStream(bytes);
                            image = System.Drawing.Image.FromStream(memStream);
                        }
                    }
                }
            }
            catch
            {
            }

            if (image == null)
                image = Images.NoImage;

            return image;
        }

        /// <summary>
        /// ‹bertragt die selektierten CDs in den eigenen Katalog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTransfer_Click(object sender, EventArgs e)
        {
            if (listBoxResult.SelectedItems.Count < 1)
                return;

            TransferCDs();
        }

        private void TransferCDs()
        {
	        int count = 0;

	        foreach (CDListBoxWithCoverItem item in listBoxResult.SelectedItems)
	        {
		        int cdSetCount = 0;

		        foreach (CDItem cdItem in item.CD)
		        {
			        CD cd = new CD();

			        String artist = StringTable.Unknown;
			        String title = StringTable.Unknown;

			        if (!string.IsNullOrEmpty(item.Artist))
				        artist = item.Artist;
			        if (!string.IsNullOrEmpty(item.Title))
				        title = item.Title;

			        cd.Artist = artist;
			        cd.Title = title;
			        cd.Label = item.Label;
			        cd.UPC = item.EAN;
			
			        if (item.CD.Length > 1)			// Dann ist es ein CD-Set
			        {
				        cd.CDSetNumber = cdSetCount+1;
				        cd.CDSetName = item.Title;
			        }

			        if (item.Year != null && item.Year.Length > 4)
				        cd.YearRecorded = Convert.ToInt32(item.Year.Substring(0, 4));

			        cd.NumberOfTracks = cdItem.Tracks.Length;
                    cd.InitTracks(cd.NumberOfTracks);

			        int iTrack=0;
			        foreach (String trackName in cdItem.Tracks)
			        {
				        cd.Tracks[iTrack].Title = trackName;
				        iTrack++;
			        }

			        // Jetzt noch das Front-Cover
			        WebClient webClient = new WebClient();
                    webClient.UseDefaultCredentials = true;
                    webClient.Proxy.Credentials = CredentialCache.DefaultCredentials;

			        String imageFilename;

			        imageFilename = String.Format("{0} - {1}.jpg", Misc.FilterFilenameChars(artist), Misc.FilterFilenameChars(title));

			        String imageFilepath = Misc.GetCDCoverFilename(imageFilename);

			        byte[] data = null;
			
			        if (item.largeImageUrl != null)
				        data = webClient.DownloadData(item.largeImageUrl);
			        else
				        if (item.mediumImageUrl != null)
					        data = webClient.DownloadData(item.mediumImageUrl);
				        else
					        if (item.smallImageUrl != null)
						        data = webClient.DownloadData(item.smallImageUrl);

			        if (data != null)
			        {
				        FileStream fileStream = new FileStream(imageFilepath, FileMode.Create);
				        fileStream.Write(data, 0, data.Length);
				        fileStream.Close();

				        cd.CDCoverFrontFilename = imageFilepath;
			        }

			        // Jetzt noch das Back-Cover
			        imageFilename = String.Format("{0} - {1} - Back.jpg", Misc.FilterFilenameChars(artist), Misc.FilterFilenameChars(title));

			        imageFilepath = Misc.GetCDCoverFilename(imageFilename);

			        data = null;
			
			        if (item.backCoverLargeImageUrl != null)
				        data = webClient.DownloadData(item.backCoverLargeImageUrl);
			        else
				        if (item.backCoverMediumImageUrl != null)
					        data = webClient.DownloadData(item.backCoverMediumImageUrl);
				        else
					        if (item.backCoverSmallImageUrl != null)
						        data = webClient.DownloadData(item.backCoverSmallImageUrl);

			        if (data != null)
			        {
				        FileStream fileStream = new FileStream(imageFilepath, FileMode.Create);
				        fileStream.Write(data, 0, data.Length);
				        fileStream.Close();

				        cd.CDCoverBackFilename = imageFilepath;
			        }

			        bool saveCD = true;
			        if (this.checkBoxEditBeforeSave.Checked)
			        {
				        WindowAlbum windowAlbum = new WindowAlbum(cd, this.dataBase);
                        windowAlbum.SaveAlbumOnOK = true;
                        saveCD = false;
				        if (windowAlbum.ShowDialog() == true)
                            count++;
			        }
                    
			        if (saveCD)
			        {
				        cd.Save(this.dataBase);

				        count ++;
			        }

			        cdSetCount++;
		        }
	        }

	        String str = string.Format(StringTable.AmazonCopySuccess, count);
	        System.Windows.MessageBox.Show(str, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FormSearchAmazon_Load(object sender, EventArgs e)
        {
            comboBoxSort.Items.Add(new AmazonSortItem(AmazonSort.BestSelling, StringTable.AmazonSortBestSelling));
            comboBoxSort.Items.Add(new AmazonSortItem(AmazonSort.TitleAscend, StringTable.AmazonSortTitleAscend));
            comboBoxSort.Items.Add(new AmazonSortItem(AmazonSort.TitleDescend, StringTable.AmazonSortTitleDescend));
            comboBoxSort.Items.Add(new AmazonSortItem(AmazonSort.YearDescend, StringTable.AmazonSortYearDescend));
            comboBoxSort.Items.Add(new AmazonSortItem(AmazonSort.YearAscend, StringTable.AmazonSortYearAscend));
 
            comboBoxSort.SelectedIndex = 0;

            UpdateWindowState();
        }

        private void textBoxArtist_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonSearch.Enabled = textBoxArtist.Text.Length > 0 || textBoxTitle.Text.Length > 0 || textBoxEAN.Text.Length > 0;
            buttonTransfer.Enabled = listBoxResult.SelectedIndices.Count > 0;
        }

        private void textBoxTitle_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void listBoxResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void listBoxResult_SizeChanged(object sender, EventArgs e)
        {
            listBoxResult.Invalidate();
        }

        private void pagerControl_PageChanged()
        {
            SearchNow(pagerControl.CurrentPage);
        }

        private void textBoxEAN_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }
    }
}