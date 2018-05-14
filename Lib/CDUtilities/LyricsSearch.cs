using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.CDUtilities.LyricWikiService;
using System.Net;
using System.Windows.Forms;
using System.IO;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public class LyricsSearch
    {
        static public string Search(string artist, string title)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                Encoding iso8859 = Encoding.GetEncoding("ISO-8859-1");

                artist = artist.Replace("'", "");
                title = title.Replace("'", "");

                string artistEncoded = "";
                foreach (char c in artist)
                    artistEncoded += Uri.HexEscape(c);
                string titleEncoded = "";
                foreach (char c in title)
                    titleEncoded += Uri.HexEscape(c);
                string url = string.Format("http://webservices.lyrdb.com/lookup.php?q={0}|{1}&for=match&agent=Hitbase", artistEncoded, titleEncoded);
                WebClient wc = new WebClient();
                wc.UseDefaultCredentials = true;
                wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
                wc.Encoding = iso8859;
                Uri uri = new Uri(url);

                string page = wc.DownloadString(uri);

                string[] hits = page.Split("\r\n".ToCharArray());
                foreach (string hit in hits)
                {
                    string[] values = hit.Split('\\');

                    if (values.Length < 3)
                        continue;

                    string urlLyrics = string.Format("http://webservices.lyrdb.com/getlyr.php?q={0}", values[0]);

                    WebClient wcLyrics = new WebClient();
                    wcLyrics.UseDefaultCredentials = true;
                    wcLyrics.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    
                    string lyrics = wcLyrics.DownloadString(urlLyrics);

                    lyrics = lyrics.Trim("\r\n \t".ToCharArray());

                    lyrics = lyrics.Replace("\r", "");
                    lyrics = lyrics.Replace("\n", "\r\n");
                    lyrics = System.Text.RegularExpressions.Regex.Replace(lyrics, "<!--[\\d\\D]*?-->", string.Empty);

                    // ersten treffer zurückliefern
                    return lyrics;
                }

                return "";
            }
            catch
            {
                return "";
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Wird zur Zeit nicht benutzt, ich lass es aber mal drin.
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        static public string SearchLyricsWikia(string artist, string title)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                string lyrics = "";

                Encoding iso8859 = Encoding.GetEncoding("ISO-8859-1");

                string urlArtist = artist.Replace(' ', '_');
                string urlTitle = title.Replace(' ', '_');

                string url = string.Format("http://lyrics.wikia.com/lyrics/{0}:{1}", urlArtist, urlTitle);

                // Leider geht res.Lyrics nicht mehr... scheiß Plattenindustrie!
                // Dann parsen wir halt einfach die URL ;-)

                WebClient wc = new WebClient();
                wc.UseDefaultCredentials = true;
                wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
                byte[] pageData = wc.DownloadData(url);
                MemoryStream ms = new MemoryStream(pageData);
                StreamReader sr = new StreamReader(ms, iso8859);

                string page = Encoding.UTF8.GetString(pageData);

                page = System.Text.RegularExpressions.Regex.Replace(page, "<!--[\\d\\D]*?-->", string.Empty);

                int lyricsboxIndexStart = page.IndexOf("<div class='lyricbox'>");
                if (lyricsboxIndexStart > 0)
                {
                    // Teilweise kommen hier geschachtelte divs
                    int pos = lyricsboxIndexStart + 1;
                    int divCount = 1;
                    int lastDivOpeningPos = -1;
                    while (pos < page.Length && divCount > 0)
                    {
                        int divstart = page.IndexOf("<div", pos);
                        int divend = page.IndexOf("</div>", pos);

                        if (divstart < divend)
                        {
                            lastDivOpeningPos = divstart;
                            divCount++;
                            pos = divstart + 1;
                        }
                        else
                        {
                            if (divCount > 1)
                            {
                                // Geschachteltes DIV wegnehmen!
                                page = page.Remove(lastDivOpeningPos, divend - lastDivOpeningPos);
                                divend = lastDivOpeningPos;
                            }

                            divCount--;
                            pos = divend + 1;
                        }
                    }

                    int lyricsboxIndexEnd = pos;// page.IndexOf("</div>", lyricsboxIndexStart);
                    lyrics = page.Substring(lyricsboxIndexStart, lyricsboxIndexEnd - lyricsboxIndexStart - 1);
                    lyrics = lyrics.Replace("<br />", "\r\n");
                    lyrics = DeleteAllTags(lyrics);
                }

                return lyrics;
            }
            catch
            {
                return "";
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private static string DeleteAllTags(string lyrics)
        {
            return System.Text.RegularExpressions.Regex.Replace(lyrics, "<[^>]*>", string.Empty);

        }

        public static void SearchLyrics(CD cd)
        {
            FormProgress formProgress = new FormProgress();
            formProgress.LabelProgress.Text = StringTable.SearchLyrics;
            formProgress.ProgressBar.Maximum = cd.Tracks.Count;
            formProgress.Show();

            foreach (Track track in cd.Tracks)
            {
                Application.DoEvents();
                string artist = (cd.Sampler) ? track.Artist : cd.Artist;
                string title = track.Title;
                
                if (!string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(title))
                {
                    string lyrics = Search(artist, title);

                    track.Lyrics = lyrics;
                }

                formProgress.ProgressBar.Value++;
                Application.DoEvents();
            }
            formProgress.Close();
        }

        public static void SearchLyrics(CD cd, Track track)
        {
            FormProgress formProgress = new FormProgress();
            formProgress.LabelProgress.Text = StringTable.SearchLyrics;
            formProgress.ProgressBar.Maximum = 1;
            formProgress.Show();

            Application.DoEvents();
            string artist = (cd.Sampler) ? track.Artist : cd.Artist;
            string title = track.Title;

            if (!string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(title))
            {
                string lyrics = Search(artist, title);

                track.Lyrics = lyrics;
            }

            formProgress.ProgressBar.Value++;
            Application.DoEvents();

            formProgress.Close();
        }
    }
}
