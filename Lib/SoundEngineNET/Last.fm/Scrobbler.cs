using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Big3.Hitbase.Configuration;
using Lpfm.LastFmScrobbler;
using Lpfm.LastFmScrobbler.Api;
using System.Windows;
using System.Diagnostics;

namespace Big3.Hitbase.SoundEngine.Last.fm
{
    public class ScrobblerClass
    {
        //TODO: Go to http://www.last.fm/api/account and apply for an API account. Then paste the key and secret into these constants
        private const string ApiKey = "0646c123d0f6166d85699a747e673440";
        private const string ApiSecret = "c1013b01796685009fbd12ed37a64186";

        private QueuingScrobbler _scrobbler;

        private static Scrobbler scrobbler = null;

        Track currentTrack = null;

        public ScrobblerClass()
        {
            Init();
        }

        public void Init()
        {
            if (Settings.Current.ScrobbleActive)
            {
                // instantiate a new scrobbler
                if (scrobbler == null)
                {
                    scrobbler = new Scrobbler(ApiKey, ApiSecret);
                }

                if (_scrobbler == null)
                {
                    string sessionKey = GetSessionKey();

                    if (!string.IsNullOrEmpty(sessionKey))
                    {
                        // instantiate the async scrobbler
                        _scrobbler = new QueuingScrobbler(ApiKey, ApiSecret, sessionKey);
                    }
                }
            }
        }

        private string GetSessionKey()
        {
            const string sessionKeyRegistryKeyName = "LastFmSessionKey";

            // try get the session key from the registry
            string sessionKey = GetRegistrySetting(sessionKeyRegistryKeyName, null);

            if (string.IsNullOrEmpty(sessionKey))
            {
                // Try get session key from Last.fm
                try
                {
                    sessionKey = scrobbler.GetSession();

                    // successfully got a key. Save it to the registry for next time
                    SetRegistrySetting(sessionKeyRegistryKeyName, sessionKey);
                }
                catch (LastFmApiException exception)
                {
                    // Wahrscheinlich nicht authorisiert.
                }
            }

            return sessionKey;
        }

        public static void Authorize()
        {
            // instantiate a new scrobbler
            if (scrobbler == null)
            {
                scrobbler = new Scrobbler(ApiKey, ApiSecret);
            }

            // get a url to authenticate this application
            string url = scrobbler.GetAuthorisationUri();

            // open the URL in the default browser
            Process.Start(url);
        }

        private delegate void ProcessScrobblesDelegate();

        private void ProcessScrobbles()
        {
            if (_scrobbler != null)
            {
                // Processes the scrobbles and discards any responses. This could be improved with thread-safe
                //  logging and/or error handling
                _scrobbler.Process();
            }
        }

        public void NowPlaying(PlaylistItem hitbaseTrack)
        {
            if (_scrobbler != null)
            {
                var doProcessScrobbles = new ProcessScrobblesDelegate(ProcessScrobbles);

                currentTrack = HitbaseTrackToLastFmTrack(hitbaseTrack);

                currentTrack.WhenStartedPlaying = DateTime.Now;
                _scrobbler.NowPlaying(currentTrack);
                // we are using the Queuing scrobbler here so that we don't block the form while the scrobble request is being sent
                //  to the Last.fm web service. The request will be sent when the Process() method is invoked
                // Begin invoke with no callback fires and forgets the scrobbler process. Processing runs asynchronously while 
                //  the form thread continues
                doProcessScrobbles.BeginInvoke(null, null);
            }
        }

        public void Scrobble()
        {
            if (_scrobbler != null)
            {
                var doProcessScrobbles = new ProcessScrobblesDelegate(ProcessScrobbles);

                // Scrobble the track that just finished
                _scrobbler.Scrobble(currentTrack);
                doProcessScrobbles.BeginInvoke(null, null);
            }
        }

        private static Track HitbaseTrackToLastFmTrack(PlaylistItem hitbaseTrack)
        {
            var track = new Track
            {
                TrackName = hitbaseTrack.Info.Title,
                ArtistName = hitbaseTrack.Info.Artist,
                TrackNumber = (int)hitbaseTrack.Info.TrackNumber,
                Duration = new TimeSpan(0, 0, 0, 0, (int)hitbaseTrack.Info.Length),
                AlbumArtist = hitbaseTrack.Info.AlbumArtist,
                AlbumName = hitbaseTrack.Info.Album
            };

            return track;
        }

        public static string GetRegistrySetting(string valueName, string defaultValue)
        {
            if (string.IsNullOrEmpty(valueName)) throw new ArgumentException("valueName cannot be empty or null", "valueName");
            valueName = valueName.Trim();

            object regValue = Settings.GetValue(valueName, defaultValue);

            if (regValue == null)
            {
                // Key does not exist
                return defaultValue;
            }
            else
            {
                return regValue.ToString();
            }
        }

        public static void SetRegistrySetting(string valueName, string value)
        {
            if (string.IsNullOrEmpty(valueName)) throw new ArgumentException("valueName cannot be empty or null", "valueName");
            valueName = valueName.Trim();

            Settings.SetValue(valueName, value);
        }
    }
}
