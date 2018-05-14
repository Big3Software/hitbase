using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.SoundEngine
{
    public class MCICDEngine
    {
        uint wDeviceID;
        char driveLetter;

        string deviceName;

        int numberOfTracks;

        private CDInfo cdInfo = new CDInfo();

        public CDInfo CDInfo { get { return cdInfo;  } }


        #region WINMM MCI
        // Gibt wohl im Moment leider keine andere Möglichkeit.
        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        [DllImport("winmm.dll", CharSet = CharSet.Ansi,
            BestFitMapping = true, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool mciGetErrorString(
            uint mcierr,
            [MarshalAs(UnmanagedType.LPStr)]
            System.Text.StringBuilder pszText,
            uint cchText);

        #endregion

        public MCICDEngine()
        {
        }

        // Der Dateiname muß hier folgendes Format haben.
        // DEVICE=<devicenumber>;TRACK=<tracknumber>
        // Die Variable devicenumber und tracknumber sind relativ zu 0
        public void Open(char driveLetter)
        {
	        int ret;
	        deviceName = "myDrive" + driveLetter;

            ret = mciSendString("open " + driveLetter + ": type CDAudio alias " + deviceName + " wait shareable", null, 0, IntPtr.Zero);

		    if (ret != 0)
		    {
			    return;
		    }

            ret = mciSendString("set " + deviceName + " time format milliseconds wait", null, 0, IntPtr.Zero);

            ReadInformation();

            IsOpened = true;
        }

        private bool IsOpened { get; set; }

        public void Close()
        {
	        if (!IsOpened)
		        return;

            int ret;

            ret = mciSendString("close " + deviceName + " wait", null, 0, IntPtr.Zero);

            IsOpened = false;
        }

        public void PlayTrack(int track)
        {
            if (track >= CDInfo.Tracks.Count)
                return;

            Play(CDInfo.Tracks[track].StartTime, 0);
        }

        public void Play(int from, int to)
        {
	        int ret;
	        int sort;
	
	        if (to < from && to != from && to != 0)
	        {
		        sort = from;            /* Second value lower than first! Ring-Tausch */
		        from = to;
		        to = sort;
	        }

            string mciCommand = "play " + deviceName;
            if (from > 0)
                mciCommand += " from " + from.ToString();
            if (to > 0)
                mciCommand += " to " + to.ToString();

            ret = mciSendString(mciCommand, null, 0, IntPtr.Zero);
        }

        public void Stop()
        {
	        int ret;

            ret = mciSendString("stop " + deviceName + " wait", null, 0, IntPtr.Zero);

            Close();
		    Open(this.driveLetter);

            IsPaused = false;
        }

        public bool IsPaused { get; set; }

        public void Pause(bool bPause)
        {
            IsPaused = bPause;
            
            if (!bPause)
            {
                int ret;
                ret = mciSendString("resume " + deviceName, null, 0, IntPtr.Zero);
                //Play(0, 0);
                return;
            }

	        int ret2;
            ret2 = mciSendString("pause " + deviceName, null, 0, IntPtr.Zero);

        }

        public int GetPlayPositionMS()
        {
            int ret;

            StringBuilder retValue = new StringBuilder(200);
            ret = mciSendString("status " + deviceName + " position", retValue, 200, IntPtr.Zero);

            if (ret != 0)
                return 0;

            int totalPosition = Misc.Atoi(retValue.ToString());

            int trackNumber = GetTrackNumberByPosition(totalPosition);

            int trackPosition = totalPosition - this.CDInfo.Tracks[trackNumber].StartTime;

            return trackPosition;
        }

        public void SetPlayPositionMS(int position)
        {
            int ret;

            StringBuilder retValue = new StringBuilder(200);
            ret = mciSendString("status " + deviceName + " position", retValue, 200, IntPtr.Zero);

            int totalPosition = Misc.Atoi(retValue.ToString());

            int trackNumber = GetTrackNumberByPosition(totalPosition);

            Play(this.CDInfo.Tracks[trackNumber].StartTime + position, 0);
        }

        private int GetTrackNumberByPosition(int totalPosition)
        {
            int trackNumber = 0;

            for (int i = 0; i < this.CDInfo.Tracks.Count; i++)
            {
                if (totalPosition >= this.CDInfo.Tracks[i].StartTime && totalPosition < this.CDInfo.Tracks[i].StartTime + this.CDInfo.Tracks[i].Length)
                {
                    trackNumber = i;
                    break;
                }
            }

            return trackNumber;
        }

        public int GetTrackLength(int track)
        {
            int ret;

            StringBuilder retValue = new StringBuilder(200);
            ret = mciSendString("status " + deviceName + " length track " + (track + 1).ToString(), retValue, 200, IntPtr.Zero);

            return Misc.Atoi(retValue.ToString());
        }

        /*
         * Liefert die eindeutige Identity der CD zurück.
         */
 
        public string GetIdentity()
        {
	        int ret;
            StringBuilder retValue = new StringBuilder(200);
            ret = mciSendString("info " + deviceName + " identity", retValue, 200, IntPtr.Zero);

            return retValue.ToString();
        }

        /*
         * Liest alle vorhandenen Informationen von der CD.
         * Liefert FALSE zurück, wenn mindestens ein Lied der CD kein AUDIO-Track ist.
         */

        bool ReadInformation()
        {
            numberOfTracks = GetNumberOfTracks();
            cdInfo.TotalLength = GetTotalLength();

            if (cdInfo.TotalLength == 0 || numberOfTracks == 0)
	        {
		        return false;
	        }
	
            CDInfo.Tracks.Clear();

            cdInfo.TotalLength = 0;

	        for (int i=0;i<numberOfTracks;i++)
	        {
		        int startTime = GetTrackPosition(i);
		
                CDInfoTrack newItem = new CDInfoTrack();
		        newItem.StartTime = startTime;

		        // Die Funktion MCI_STATUS_LENGTH liefert manchmal eine Millisekunde zuviel zurück
		        // (vermutlich rundungsprobleme). Deshalb berechnen wir uns die Länge der Tracks
		        // selbst (außer beim letzten Track, da geht es nicht anders).
		        if (i == numberOfTracks-1)
		        {
			        newItem.Length = GetTrackLength(i);
		        }
		        else
		        {	
			        int startTimeNext;
			        startTimeNext = GetTrackPosition(i + 1);
			        newItem.Length = startTimeNext - startTime;
		        }

                newItem.TrackType = GetTrackType(i);

                cdInfo.TotalLength += newItem.Length;

                CDInfo.Tracks.Add(newItem);
	        }

            cdInfo.Identity = GetIdentity();

	        return true;
        }

        private TrackType GetTrackType(int track)
        {
            int ret;

            StringBuilder retValue = new StringBuilder(200);
            ret = mciSendString("status " + deviceName + " type track " + (track + 1).ToString(), retValue, 200, IntPtr.Zero);

            TrackType trackType = TrackType.Unknown;

            if (retValue.ToString().ToLower() == "audio")
                trackType = TrackType.Audio;
            if (retValue.ToString().ToLower() == "other")
                trackType = TrackType.Data;

            return trackType;
        }

        private int GetTrackPosition(int i)
        {
            int ret;

            StringBuilder retValue = new StringBuilder(200);
            ret = mciSendString("status " + deviceName + " position track " + (i + 1).ToString(), retValue, 200, IntPtr.Zero);

            return Misc.Atoi(retValue.ToString());
        }

        private int GetTotalLength()
        {
            int ret;

            StringBuilder retValue = new StringBuilder(200);
            ret = mciSendString("status " + deviceName + " length", retValue, 200, IntPtr.Zero);

            return Misc.Atoi(retValue.ToString());
        }

        private int GetNumberOfTracks()
        {
            int ret;

            StringBuilder retValue = new StringBuilder(200);
            ret = mciSendString("status " + deviceName + " number of tracks", retValue, 200, IntPtr.Zero);

            return Misc.Atoi(retValue.ToString());
        }

        private bool IsMediaPresent()
        {
            int ret;

            StringBuilder retValue = new StringBuilder(200);
            ret = mciSendString("status " + deviceName + " media present", retValue, 200, IntPtr.Zero);

            return retValue.ToString() == "true";
        }
    }
}
