using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Big3.Hitbase.DataBaseEngine;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Big3.Hitbase.SoundEngine;

namespace Big3.Hitbase.RemoteControlService
{
    public class AddToPlaylistEventArgs
    {
        // True = Wird gespielt, False = Fehler
        public bool Result { get; set; }

        // In wieviel Sekunden wird das Lied gespielt
        public int EstimatedPlayTimeInMS { get; set; }
    }

    public class AddToWishlistEventArgs
    {
        /// <summary>
        /// Track-ID des Musikwunschs
        /// </summary>
        public int TrackID { get; set; }

        /// <summary>
        /// Name, der sich das Lied gewünscht hat.
        /// </summary>
        public string WishBy { get; set; }

        /// <summary>
        /// Kommentar zum Musikwunsch (z.B. "Jürgen liebt Nicole")
        /// </summary>
        public string Comment { get; set; }
    }

    public delegate void AddToPlaylistHandler(string soundFilename, AddToPlaylistEventArgs e);

    public delegate void AddToWishlistHandler(object sender, AddToWishlistEventArgs e);

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RemoteControlService : IRemoteControlService
    {
        public event EventHandler OnPlay;

        public event AddToPlaylistHandler OnAddToPlaylist;

        public event AddToWishlistHandler OnAddToWishlist;

        public DataBase DataBase;
        
        public void Play()
        {
            if (OnPlay != null)
                OnPlay(this, new EventArgs());
            //return string.Format("You entered: {0}", value);
        }

        public int AddToPlaylist(string soundFilename)
        {
            AddToPlaylistEventArgs e = new AddToPlaylistEventArgs();
            if (OnAddToPlaylist != null)
                OnAddToPlaylist(soundFilename, e);

            if (e.Result == true)
                return e.EstimatedPlayTimeInMS;
            else
                return -1;
        }

        public void AddToWishlist(int trackID, string wishBy, string comment)
        {
            AddToWishlistEventArgs e = new AddToWishlistEventArgs();
            if (OnAddToWishlist != null)
            {
                e.TrackID = trackID;
                e.WishBy = wishBy;
                e.Comment = comment;
                OnAddToWishlist(this, e);
            }
        }

        public List<SearchResultItem> Search(string searchFor)
        {
            List<SearchResultItem> hits = new List<SearchResultItem>();

            CDQueryDataSet dataSet = DataBase.ExecuteTrackQuery();
            
            foreach (CDQueryDataSet.TrackRow track in dataSet.Track)
            {
                string trackArtist = track.GetStringByField(DataBase, Field.ArtistTrackName) ?? "";
                string trackTitle = (track.Title == null) ? "" : track.Title;
                if (trackArtist.IndexOf(searchFor, StringComparison.InvariantCultureIgnoreCase) >= 0 || trackTitle.IndexOf(searchFor, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    if (track.SoundFile != null && File.Exists(track.SoundFile))
                    {
                        byte[] imageArray = GetCoverFromTrack(track);

                        SearchResultItem sri = new SearchResultItem()
                        {
                            TrackID = track.TrackID,
                            Artist = trackArtist,
                            Title = track.Title,
                            Length = track.Length,
                            SoundFilename = track.SoundFile,
                            CoverImage = imageArray
                        };
                        hits.Add(sri);
                    }
                }
            }


            return hits;
        }

        private byte[] GetCoverFromTrack(CDQueryDataSet.TrackRow track)
        {
            byte[] imageArray = null;

            SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(track.SoundFile);
            if (sfi.Images != null && sfi.Images.Count > 0)
            {
                imageArray =  sfi.Images[0];
            }

            if (!string.IsNullOrEmpty(track.CDRow.FrontCover) && File.Exists(track.CDRow.FrontCover))
            {
                imageArray = File.ReadAllBytes(track.CDRow.FrontCover);
            }

            if (imageArray == null)
                return null;

            // Thumbnail generieren
            MemoryStream m = new MemoryStream(imageArray);
            Image img = Image.FromStream(m);

            MemoryStream msThumb = new MemoryStream();
            EncoderParameters e = new EncoderParameters();
            Image thumb = img.GetThumbnailImage(128, 128, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);

            thumb.Save(msThumb, ImageFormat.Jpeg);

            return msThumb.ToArray();
        }

        /// <summary>
        /// Required, but not used
        /// </summary>
        /// <returns>true</returns>
        public bool ThumbnailCallback()
        {
            return true;
        }

    }
}
