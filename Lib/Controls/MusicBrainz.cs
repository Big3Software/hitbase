using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;

namespace Big3.Hitbase.Controls
{
    // http://musicbrainz.org/doc/XML_Web_Service
    // http://musicbrainz.org/ws/1/artist/?type=xml&name=bap
    // http://musicbrainz.org/ws/1/artist/c0b2500e-0cef-4130-869d-732b23ed9df5?type=xml&inc=url-rels+artist-rels
    // http://musicbrainz.org/ws/1/artist/70261540-4efc-4f62-8573-fdb68d6c2213?type=xml&inc=aliases+release-groups+artist-rels+label-rels+release-rels+track-rels+url-rels+tags+ratings+counts+release-events+discs+labels
    // http://musicbrainz.org/ws/1/label/?type=xml&name=emi
    // http://musicbrainz.org/ws/1/label/3b76dfe7-0a2d-401a-86a7-f1a30c480e0d?type=xml&inc=aliases+artist-rels+label-rels+release-rels+track-rels+url-rels+tags+ratings
    // http://www.w3schools.com/xpath/xpath_syntax.asp
    // http://musicbrainz.org/ws/1/release/?type=xml&artist=toto&releasetypes=Official&limit=10&title=the%20seventh%20one
    // http://musicbrainz.org/ws/1/release/a0691875-152e-45a6-a30d-ab2b57c0648e?type=xml&inc=artist+counts+release-events+discs+tracks+release-groups+artist-rels+label-rels+release-rels+track-rels+url-rels+track-level-rels+labels+tags+ratings+isrcs

    public class MusicBrainzArtistBaseData
    {
        public string Score;
        public string MBID;
        public string ArtistName;
        public string SortName;
        public string Type; // person/group
        public string Gender; // only for Type=person - male or female
        public string Country; // 2-digit country code
        public string BeginDate; // Begin date, a birth date or formation date, depending on type
        public string EndDate; // End date, a death date or dissolution date, depending on type
        public string BeginGroupMember; // Only set when added to Group
        public string EndGroupMember;   // Only set when added to Group
        public string Comment; // disambiguation
    }

    public class MusicbrainzURLs
    {
        public string Type;  // Fanpage, wikipedia, Dicogs, biography, officalHomepage, dicography, ...
        public string Target;
    }

    public class MusicBrainzArtist
    {
        public string Score;
        public string MBID;
        public string ArtistName;
        public string SortName;
        public string Type; // person/group
        public string Gender; // only for Type=person - male or female
        public string Country; // 2-digit country code
        public string BeginDate; // Begin date, a birth date or formation date, depending on type
        public string EndDate; // End date, a death date or dissolution date, depending on type
        public List<string> Tags; // List of tags for artist - no count stored - i.e.: uk, british, boy band
        public List<MusicBrainzArtistBaseData> BandMember; // Currently not used - but in DB of musicbrainz - relation-list target-type="Artist" 
        public string Homepage;
        public List<MusicbrainzURLs> miscURLs;
    }

    public class MusicBrainzLabel
    {
        public string Score;  // max. 100 -  Quality of search
        public string MBID;
        public string Name;
        public string SortName;
        public string Type; // original production, bootleg production, reissue production, distributor, holding
        public string Code; // the IFPI label code, e.g. LC-0193 = Electrola; LC-0233 = His Master's Voice - currently not used - have to read label MBID: i.e.: http://musicbrainz.org/ws/1/label/3b76dfe7-0a2d-401a-86a7-f1a30c480e0d?type=xml
        public string BeginDate; // Begin date, a birth date or formation date, depending on type
        public string EndDate; // End date, a death date or dissolution date, depending on type
        public string Country; // (ISO 3166 Codes) - currently not used - have to read label MBID: i.e.: http://musicbrainz.org/ws/1/label/3b76dfe7-0a2d-401a-86a7-f1a30c480e0d?type=xml
    }

    /// <summary>
    /// Read first 25 entries of artist data - persons or groups
    /// </summary>
    public class ResultMusicBrainzLabel
    {
        public List<MusicBrainzLabel> mbLabels;
    }

    public class ResultMusicBrainzArtist
    {
        public List<MusicBrainzArtistBaseData> mbArtists;
    }

    public class MusicBrainzReleaseEvent
    {
        public string ReleaseDate;
        public string ReleaseCountry;
        public string Label;
        public string CatalogNumber;
        public string Barcode;
        public string Format; // Format (CD, cassette, vinyl, wax cylinder, etc.)
    }

    public class MusicBrainzRelease
    {
        public string MBID;
        public string Title;
        public string Artist;
        public string Type; // person/group
        public string Status;
        public string Language;
        public string Annotation; // 
        public string DiscID;
        public string AmazonASIN;
    }

    public class MusicBrainzTrack
    {
        public string MBID;
        public string Title;
        public string Artist;
        public string Duration; // millisec.
        public string Annotation;
        public string PUID; // PUID, the MusicIP acoustic fingerprint identifier for the track
        public string ISRC;
    }

    public class MusicBrainzData
    {
        private XmlDocument GetMusicBrainzData(string dataURL)
        {
            StreamReader streamRead;
            string readXML;
            XmlDocument xmldata;
            // ICredentials credential = new NetworkCredential("login", "pwd");
            //IWebProxy proxyServer = new WebProxy("proxy:NNNN", true, null, credential);

            WebClient client = new WebClient();
            client.Headers["User-Agent"] = System.Windows.Forms.Application.ProductName;
            client.UseDefaultCredentials = true;
            client.Proxy.Credentials = CredentialCache.DefaultCredentials;

            //client.Proxy = proxyServer;
            //client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadDataCompleted);
            //client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");

            try
            {
                // Testen auf ZIP??? Wie geht das?
                //GZipStream zipStream = new GZipStream(client.OpenRead("http://www.discogs.com/label/emi?f=xml&api_key=5a6018d1fd"), CompressionMode.Decompress, false);

                //streamRead = new StreamReader(zipStream);
                // Testen auf ZIP
                //streamRead.ReadToEnd();
                // http://www.discogs.com/artist/bap?f=xml&api_key=5a6018d1fd
                Uri myUri = new Uri(dataURL);

                streamRead = new StreamReader(client.OpenRead(myUri));
                readXML = streamRead.ReadToEnd();
            }
            catch (Exception ex)
            {
                return null;
            }
            //byte[] buffer = client.DownloadData("http://www.discogs.com/artist/bap?f=xml&api_key=5a6018d1fd");
            //msRead.Write(buffer, 0, buffer.Length);
            //streamRead.Position = 0L;
            //streamRead = new StreamReader(zipStream);
            //GZipStream zipStream = new GZipStream(msRead, CompressionMode.Decompress);
            // http://www.discogs.com/help/api
            // http://www.discogs.com/label/EMI?f=xml&api_key=5a6018d1fd
            // http://www.discogs.com/artist/bap?f=xml&api_key=5a6018d1fd
            // http://www.discogs.com/release/1097554?f=xml&api_key=5a6018d1fd
            // http://www.discogs.com/search?type=all&q=afx&f=xml&api_key=5a6018d1fd&page=2

            xmldata = new XmlDocument();

            xmldata.LoadXml(readXML);

            //XPathNavigator navi = xmllabel.CreateNavigator();

            return xmldata;
        }

        // http://musicbrainz.org/ws/1/artist/?type=xml&name=Toto
        public ResultMusicBrainzArtist SearchMusicBrainzArtistData(string searchArtistName)
        {
            string newURL;

            if (searchArtistName.Length < 1)
                return null;

            newURL = "http://musicbrainz.org/ws/2/artist/?query=artist:" + searchArtistName;
            XmlDocument xmldata;

            // Get data from WEB
            xmldata = GetMusicBrainzData(newURL);

            if (xmldata == null)
                return null;

            ResultMusicBrainzArtist mbArtists = new ResultMusicBrainzArtist();
            mbArtists.mbArtists = new List<MusicBrainzArtistBaseData>();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmldata.NameTable);
            namespaceManager.AddNamespace("r", xmldata.DocumentElement.NamespaceURI);
            namespaceManager.AddNamespace("s", xmldata.DocumentElement.GetNamespaceOfPrefix("ext"));

            // http://www.w3schools.com/xpath/xpath_syntax.asp
            // Get name of label
            //XmlNodeList elements = xmllabel.SelectNodes("//r:metadata/r:label-list/r:label[@s:score>90]", namespaceManager);
            XmlNodeList elements = xmldata.SelectNodes("//r:metadata/r:artist-list/r:artist", namespaceManager);
            foreach (XmlElement element in elements)
            {
                if (element.HasAttribute("ext:score") == true)
                {
                    if (element.GetAttribute("ext:score") == "100")
                    {
                        XmlNodeList xml2 = element.ChildNodes;
                        MusicBrainzArtistBaseData mb = new MusicBrainzArtistBaseData();
                        mb.Score = "";
                        mb.MBID = "";
                        mb.ArtistName = "";
                        mb.SortName = "";
                        mb.Type = "";
                        mb.BeginDate = "";
                        mb.EndDate = "";
                        mb.Comment = "";
                        mb.Gender = "";
                        mb.Country = "";
                        

                        mb.MBID = element.GetAttribute("id");
                        mb.Type = element.GetAttribute("type");
                        mb.Score = element.GetAttribute("ext:score");

                        foreach (XmlElement element2 in xml2)
                        {
                            if (element2.LocalName.ToLower() == "gender")
                            {
                                mb.Gender = element2.InnerText;
                            }
                            if (element2.LocalName.ToLower() == "country")
                            {
                                mb.Country = element2.InnerText;
                            }
                            if (element2.LocalName == "name")
                            {
                                mb.ArtistName = element2.InnerText;
                            }
                            if (element2.LocalName == "sort-name")
                            {
                                mb.SortName = element2.InnerText;
                            }
                            if (element2.LocalName == "life-span")
                            {
                                /*
                                XmlNodeList elements2 = xmldata.SelectNodes("//r:metadata/r:artist-list/r:artist/r:life-span", namespaceManager);
                                foreach (XmlElement element3 in elements2)
                                {
                                    if (element3.LocalName == "begin")
                                    {
                                        mb.BeginDate = element3.InnerText;
                                    }
                                    if (element3.LocalName == "end")
                                    {
                                        mb.EndDate = element3.InnerText;
                                    }
                                }
                                
                                if (element2.HasAttribute("begin"))
                                {
                                    mb.BeginDate = element2.GetAttribute("begin");
                                }
                                
                                if (element2.HasAttribute("end"))
                                {
                                    mb.EndDate = element2.GetAttribute("end");
                                }
                                */
                            }
                        }
                        mbArtists.mbArtists.Add(mb);
                    }
                }
            }

            return mbArtists;
        }

        // http://musicbrainz.org/ws/1/artist/?type=xml&name=Toto
        /// <summary>
        /// Get Full Artist data by ID
        /// </summary>
        /// <param name="MBID"></param>
        /// <returns></returns>
        public MusicBrainzArtist GetMusicBrainzArtistByID(string MBID)
        {
            string newURL;
            newURL = "http://musicbrainz.org/ws/2/artist/" + MBID + "?inc=artist-rels+url-rels+tags";
            XmlDocument xmldata;

            // Get data from WEB
            xmldata = GetMusicBrainzData(newURL);

            if (xmldata == null)
                return null;

            MusicBrainzArtist mbArtist = new MusicBrainzArtist();
            // mbArtists.mbArtists = new List<MusicBrainzArtist>();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmldata.NameTable);
            namespaceManager.AddNamespace("r", xmldata.DocumentElement.NamespaceURI);

            // http://www.w3schools.com/xpath/xpath_syntax.asp
            // Get name of label
            //XmlNodeList elements = xmllabel.SelectNodes("//r:metadata/r:label-list/r:label[@s:score>90]", namespaceManager);

            mbArtist.MBID = "";
            mbArtist.ArtistName = "";
            mbArtist.SortName = "";
            mbArtist.Type = "";
            mbArtist.BeginDate = "";
            mbArtist.EndDate = "";
            mbArtist.BandMember = new List<MusicBrainzArtistBaseData>();
            mbArtist.miscURLs = new List<MusicbrainzURLs>();
            mbArtist.Tags = new List<string>();

            XmlNodeList nodelist = xmldata.SelectNodes("//r:metadata/r:artist", namespaceManager);

            if (nodelist.Count > 0)
            {
                if (((XmlElement)nodelist[0]).HasAttribute("type"))
                {
                    mbArtist.Type = ((XmlElement)nodelist[0]).GetAttribute("type");
                }
                if (((XmlElement)nodelist[0]).HasAttribute("id"))
                {
                    mbArtist.MBID = ((XmlElement)nodelist[0]).GetAttribute("id");
                }
            }

            nodelist = xmldata.SelectNodes("//r:metadata/r:artist/*", namespaceManager);

            foreach (XmlElement element in nodelist)
            {
                if (element.LocalName.ToLower() == "gender")
                {
                    mbArtist.Gender = element.InnerText;
                }
                if (element.LocalName.ToLower() == "country")
                {
                    mbArtist.Country = element.InnerText;
                }
                if (element.LocalName == "name")
                {
                    mbArtist.ArtistName = element.InnerText;
                }
                if (element.LocalName == "sort-name")
                {
                    mbArtist.SortName = element.InnerText;
                }
                if (element.LocalName == "life-span")
                {
                    XmlNodeList elementsLife = xmldata.SelectNodes("//r:metadata/r:artist/r:life-span/*", namespaceManager);
                    foreach (XmlElement element3 in elementsLife)
                    {
                        if (element3.LocalName == "begin")
                        {
                            mbArtist.BeginDate = element3.InnerText;
                        }
                        if (element3.LocalName == "end")
                        {
                            mbArtist.EndDate = element3.InnerText;
                        }
                    }
                }
            }

            nodelist = xmldata.SelectNodes("//r:metadata/r:artist/r:tag-list/*", namespaceManager);

            foreach (XmlElement element in nodelist)
            {
                if (element.LocalName == "tag")
                {
                    mbArtist.Tags.Add(element.InnerText);
                }
            }
            XmlNodeList xnode22 = xmldata.SelectNodes("/r:metadata/r:artist/r:relation-list/r:relation[@type='is person']/r:artist", namespaceManager);

            foreach (XmlElement element22 in xnode22)
            {
                {
                    MusicBrainzArtistBaseData mb = new MusicBrainzArtistBaseData();
                    mb.MBID = element22.GetAttribute("id");
                    mb.Type = "Person";
                    
                    //XmlNodeList xmlParent = element2.ParentNode;
                    XmlNode xmlParent = element22.ParentNode;
                    XmlNodeList xml32 = xmlParent.ChildNodes;
                    foreach (XmlElement eattribute in xml32)
                    {
                        if (eattribute.Name == "begin")
                        {
                            mb.BeginGroupMember = eattribute.InnerText;
                        }
                        if (eattribute.Name == "end")
                        {
                            mb.EndGroupMember = eattribute.InnerText;
                        }
                    }
                    //string xac = xmlParent.Attributes.GetNamedItem("begin");
                    mb.Type = "Person";
                    XmlNodeList xml2 = element22.ChildNodes;
                    foreach (XmlElement element3 in xml2)
                    {
                        if (element3.LocalName.ToLower() == "gender")
                        {
                            mbArtist.Gender = element3.InnerText;
                        }
                        if (element3.LocalName.ToLower() == "country")
                        {
                            mbArtist.Country = element3.InnerText;
                        }
                        if (element3.Name == "name")
                        {
                            mb.ArtistName = element3.InnerText;
                        }
                        if (element3.Name == "sort-name")
                        {
                            mb.SortName = element3.InnerText;
                        }
                        if (element3.Name == "life-span")
                        {
                            if (element3.HasAttribute("begin"))
                            {
                                mb.BeginDate = element3.GetAttribute("begin");
                            }

                            if (element3.HasAttribute("end"))
                            {
                                mb.EndDate = element3.GetAttribute("end");
                            }
                        }
                    }

                    if (mb != null)
                    {
                        mbArtist.BandMember.Add(mb);
                    }
                }
            }
            XmlNodeList xnode2 = xmldata.SelectNodes("/r:metadata/r:artist/r:relation-list/r:relation[@type='member of band']/r:artist", namespaceManager);

            foreach (XmlElement element2 in xnode2)
            {
                //if (element2.GetAttribute("type") == "Person")
                {
                    MusicBrainzArtistBaseData mb = new MusicBrainzArtistBaseData();
                    mb.MBID = element2.GetAttribute("id");
                    mb.Type = "Bandmitglied";
                    //XmlNodeList xmlParent = element2.ParentNode;
                    XmlNode xmlParent = element2.ParentNode;

                    XmlNodeList xml32 = xmlParent.ChildNodes;
                    foreach (XmlElement eattribute in xml32)
                    {
                        if (eattribute.Name == "begin")
                        {
                            mb.BeginGroupMember = eattribute.InnerText;
                        }
                        if (eattribute.Name == "end")
                        {
                            mb.EndGroupMember = eattribute.InnerText;
                        }
                    }
                    //string xac = xmlParent.Attributes.GetNamedItem("begin");
                    
                    XmlNodeList xml2 = element2.ChildNodes;

                    foreach (XmlElement element3 in xml2)
                    {
                        if (element3.LocalName.ToLower() == "gender")
                        {
                            mbArtist.Gender = element3.InnerText;
                        }
                        if (element3.LocalName.ToLower() == "country")
                        {
                            mbArtist.Country = element3.InnerText;
                        }
                        if (element3.Name == "name")
                        {
                            mb.ArtistName = element3.InnerText;
                        }
                        if (element3.Name == "sort-name")
                        {
                            mb.SortName = element3.InnerText;
                        }
                        if (element3.Name == "life-span")
                        {
                            if (element3.HasAttribute("begin"))
                            {
                                mb.BeginDate = element3.GetAttribute("begin");
                            }

                            if (element3.HasAttribute("end"))
                            {
                                mb.EndDate = element3.GetAttribute("end");
                            }
                        }
                    }

                    if (mb != null)
                    {
                        mbArtist.BandMember.Add(mb);
                    }
                }
            }

            XmlNodeList xnode3 = xmldata.SelectNodes("/r:metadata/r:artist/r:relation-list[@target-type='url']/r:relation", namespaceManager);

            foreach (XmlElement element2 in xnode3)
            {
                if (element2.HasAttribute("type"))
                {
                    MusicbrainzURLs mburl = new MusicbrainzURLs();

                    if (element2.GetAttribute("type").ToLower() == "official homepage")
                    {
                        mbArtist.Homepage = element2.InnerText;
                        mburl.Type = element2.GetAttribute("type");
                        mburl.Target = element2.InnerText;
                        mbArtist.miscURLs.Add(mburl);
                    }
                    else
                    {
                        mburl.Type = element2.GetAttribute("type");
                        mburl.Target = element2.InnerText;
                        mbArtist.miscURLs.Add(mburl);
                    }
                }
            }
            return mbArtist;
        }

        // http://musicbrainz.org/ws/1/label/?type=xml&name=emi
        public ResultMusicBrainzLabel GetMusicBrainzLabelData(string searchLabelName)
        {
            string newURL;
            newURL = "http://musicbrainz.org/ws/1/label/?type=xml&name=" + searchLabelName;
            XmlDocument xmldata;

            xmldata = GetMusicBrainzData(newURL);

            if (xmldata == null)
                return null;

            ResultMusicBrainzLabel mblabels = new ResultMusicBrainzLabel();
            mblabels.mbLabels = new List<MusicBrainzLabel>();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmldata.NameTable);
            namespaceManager.AddNamespace("r", xmldata.DocumentElement.NamespaceURI);
            namespaceManager.AddNamespace("s", xmldata.DocumentElement.GetNamespaceOfPrefix("ext"));

            // http://www.w3schools.com/xpath/xpath_syntax.asp
            // Get name of label
            //XmlNodeList elements = xmllabel.SelectNodes("//r:metadata/r:label-list/r:label[@s:score>90]", namespaceManager);
            XmlNodeList elements = xmldata.SelectNodes("//r:metadata/r:label-list/r:label", namespaceManager);
            foreach (XmlElement element in elements)
            {
                if (element.HasAttribute("ext:score") == true)
                {
                    if (element.GetAttribute("ext:score") == "100")
                    {
                        XmlNodeList xml2 = element.ChildNodes;
                        MusicBrainzLabel mb = new MusicBrainzLabel();
                        mb.Score = "";
                        mb.MBID = "";
                        mb.Name = "";
                        mb.SortName = "";
                        mb.Type = "";
                        mb.Code = "";
                        mb.BeginDate = "";
                        mb.EndDate = "";
                        mb.Country = "";

                        mb.MBID = element.GetAttribute("id");
                        mb.Type = element.GetAttribute("type");
                        mb.Score = element.GetAttribute("ext:score");

                        foreach (XmlElement element2 in xml2)
                        {
                            if (element2.LocalName == "name")
                            {
                                mb.Name = element2.InnerText;
                            }
                            if (element2.LocalName == "sort-name")
                            {
                                mb.SortName = element2.InnerText;
                            }
                            if (element2.LocalName == "life-span")
                            {
                                if (element2.HasAttribute("begin"))
                                {
                                    mb.BeginDate = element2.GetAttribute("begin");
                                }
                                if (element2.HasAttribute("end"))
                                {
                                    mb.EndDate = element2.GetAttribute("end");
                                }
                            }
                        }
                        mblabels.mbLabels.Add(mb);
                    }
                }
            }

            return mblabels;
        }
    }
}
