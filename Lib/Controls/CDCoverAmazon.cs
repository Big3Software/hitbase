using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Windows.Forms;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.DataBaseEngine;
using System.IO;
using Big3.Hitbase.Controls.AmazonServiceReference;

namespace Big3.Hitbase.Controls
{
	public class CDCoverAmazon
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

		public static void GetCDCover(CD cd)
		{
            try
            {
                if (cd == null)
                    return;

                Cursor.Current = Cursors.WaitCursor;
                            
                string filenameFront = cd.GetCDCoverFilename(CoverType.Front);
                string filenameBack = cd.GetCDCoverFilename(CoverType.Back);

                AWSECommerceServicePortTypeClient amazonClient = new AWSECommerceServicePortTypeClient();

                ItemSearch itemSearch = new ItemSearch();
                itemSearch.AWSAccessKeyId = "0FN016GTSMZHJD0C7YG2";
                
                ItemSearchRequest itemSearchRequest = new ItemSearchRequest();
                itemSearchRequest.Artist = cd.Artist;
                itemSearchRequest.Keywords = cd.Title;
                itemSearchRequest.SearchIndex = "Music";
                itemSearchRequest.Sort = "salesrank";
                itemSearchRequest.ResponseGroup = new string[] { "Images" };
                itemSearchRequest.ItemPage = "1";
                itemSearch.Request = new ItemSearchRequest[1] { itemSearchRequest };
                itemSearch.AssociateTag = "hitbase-21";

                ItemSearchResponse itemSearchResponse = null;

                bool nothingFound = false;
                try
                {
                    itemSearchResponse = amazonClient.ItemSearch(itemSearch);
                }
                catch
                {
                    nothingFound = true;
                }

                try
                {
                    if (!nothingFound && itemSearchResponse != null && itemSearchResponse.Items != null && itemSearchResponse.Items.Length > 0 &&
                        itemSearchResponse.Items[0].Item != null && itemSearchResponse.Items[0].Item.Length > 0)
                    {
                        WebClient webclient = new WebClient();
                        webclient.UseDefaultCredentials = true;
                        webclient.Proxy.Credentials = CredentialCache.DefaultCredentials;
                        Item item = itemSearchResponse.Items[0].Item[0];

                        if (item.LargeImage != null && !string.IsNullOrEmpty(item.LargeImage.URL))
                            webclient.DownloadFile(item.LargeImage.URL, filenameFront);
                        else
                            if (item.MediumImage != null && !string.IsNullOrEmpty(item.MediumImage.URL))
                                webclient.DownloadFile(item.MediumImage.URL, filenameFront);
                            else
                                if (item.SmallImage != null && !string.IsNullOrEmpty(item.SmallImage.URL))
                                    webclient.DownloadFile(item.SmallImage.URL, filenameFront);

                        if (item.ImageSets != null && item.ImageSets.Length > 0)
                        {
                            foreach (ImageSet imageSet in item.ImageSets)
                            {
                                //foreach (ImageSet imageSet in imgSet)
                                {
                                    if (imageSet.Category == "variant")
                                    {
                                        if (imageSet.LargeImage != null)
                                            webclient.DownloadFile(imageSet.LargeImage.URL, filenameBack);
                                        else
                                            if (imageSet.MediumImage != null)
                                                webclient.DownloadFile(imageSet.MediumImage.URL, filenameBack);
                                            else
                                                if (imageSet.SmallImage != null)
                                                    webclient.DownloadFile(imageSet.SmallImage.URL, filenameBack);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    FormUnhandledException formUnhandledException = new FormUnhandledException(e);
                    formUnhandledException.ShowDialog();
                }

                if (File.Exists(filenameFront))
                    cd.CDCoverFrontFilename = filenameFront;
                if (File.Exists(filenameBack))
                    cd.CDCoverBackFilename = filenameBack;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
		}
	}
}
