using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.SoundEngine
{
    public class SoundEngine
    {
        private FMOD.System system;
        private FMOD.Sound cdSound;
        private MCICDEngine mciCDEngine;
        private string cdSoundDriveLetter = "";
        private FMOD.Sound sound1;
        private FMOD.Sound sound2;
        private FMOD.Channel channel1;
        private FMOD.Channel channel2;

        private int currentSoundIndex = 0;

        private int currentCDTrackNumber;

        public string Filename { get; set; }

//        public List<string> PlayCountFileList = new List<string>();

        private static SoundEngine instance = null;

        public static SoundEngine Instance
        {
            get
            {
                if (instance == null)
                    instance = new SoundEngine();

                return instance;
            }
        }

        private static SoundEngine preListenInstance = null;

        public static SoundEngine PreListen
        {
            get
            {
                if (preListenInstance == null)
                    preListenInstance = new SoundEngine(true); 

                return preListenInstance;
            }
        }

        private FMOD.Sound currentSound
        {
            get
            {
                if (currentSoundIndex == 0)
                    return sound1;
                else
                    return sound2;
            }
        }

        private FMOD.Channel currentChannel
        {
            get
            {
                if (currentSoundIndex == 0)
                    return channel1;
                else
                    return channel2;
            }
        }

        /// <summary>
        /// Spielt die angegebene Musikdatei ab.
        /// </summary>
        /// <param name="filename"></param>
        public void Play(string filename)
        {
            Filename = filename;

            if (!this.isPreListenSoundEngine)
            {
                if (currentSoundIndex == 0)
                    currentSoundIndex = 1;
                else
                    currentSoundIndex = 0;
            }

            if (IsPlaying)
                Stop();

            FMOD.RESULT result = FMOD.RESULT.OK;

            if (filename.StartsWith("cd:"))
            {
                string cdDrive = string.Format("{0}:", filename[3]);
                int trackNumber = Miscellaneous.Misc.Atoi(filename.Substring(5));
                
                if (Settings.Current.UseMCI)
                {
                    if (mciCDEngine == null)
                    {
                        mciCDEngine = new MCICDEngine();
                        mciCDEngine.Open(cdSoundDriveLetter[0]);
                    }

                    currentCDTrackNumber = trackNumber;

                    mciCDEngine.PlayTrack(trackNumber);
                    return;
                }
                else
                {
                    if (cdSound == null)
                    {
                        result = system.createStream(cdDrive, (FMOD.MODE._2D | FMOD.MODE.HARDWARE), ref cdSound);
                        ERRCHECK(result);
                    }
                    else
                    {
                        if (cdDrive != cdSoundDriveLetter)
                        {
                            cdSound.release();

                            result = system.createStream(cdDrive, (FMOD.MODE._2D | FMOD.MODE.HARDWARE), ref cdSound);
                            ERRCHECK(result);
                        }
                    }

                    if (currentSoundIndex == 0)
                        result = cdSound.getSubSound(trackNumber, ref sound1);
                    else
                        result = cdSound.getSubSound(trackNumber, ref sound2);
                    ERRCHECK(result);
                }

                cdSoundDriveLetter = cdDrive;
            }
            else
            {
                if (currentSoundIndex == 0)
                    result = system.createStream(filename, (FMOD.MODE._2D | FMOD.MODE.HARDWARE), ref sound1);
                else
                    result = system.createStream(filename, (FMOD.MODE._2D | FMOD.MODE.HARDWARE), ref sound2);
            }
            ERRCHECK(result);

            if (result == FMOD.RESULT.OK)
            {
                if (currentSoundIndex == 0)
                    system.playSound(FMOD.CHANNELINDEX.FREE, currentSound, false, ref channel1);
                else
                    system.playSound(FMOD.CHANNELINDEX.FREE, currentSound, false, ref channel2);

                // Hiermit wird der neu spielende Track "stumm" geschaltet.
                IsMuted = IsMuted;

                // Aktuelle Lautstärke setzen
                Volume = Volume;

                /*string str = string.Format("Detecting BPM...");
                Big3.Hitbase.SharedResources.HitbaseCommands.SetMainStatusText.Execute(str, System.Windows.Application.Current.MainWindow);

                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += delegate
                {
                    float bpm = BPMDetection(filename);
                    CurrentTrackBPM = (int)(bpm + 0.5);         // Runden
                };
                bw.RunWorkerCompleted += delegate
                {
                    string bpmDetected = string.Format("BPM: {0}", CurrentTrackBPM);
                    Big3.Hitbase.SharedResources.HitbaseCommands.SetMainStatusText.Execute(bpmDetected, System.Windows.Application.Current.MainWindow);
                };
                bw.RunWorkerAsync();*/
            }
        }


#if false
        private float BPMDetection(string filename)
        {
            float bpm = 0.0f;

            const int CHUNKSIZE = 4096;
            FMOD.RESULT result;
            FMOD.System systemBpm = null;
            FMOD.Sound   soundBpm   = null;

            // Create a System object and initialize.
            result = FMOD.Factory.System_Create(ref systemBpm);
            ERRCHECK(result);

            // Zuerst in eine WAVE-Umwandeln
            result = systemBpm.init(1, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
            ERRCHECK(result);

            result = systemBpm.createSound(filename, FMOD.MODE.OPENONLY | FMOD.MODE.ACCURATETIME, ref soundBpm);
            ERRCHECK(result);

            /*
                Decode the sound and write it to a .raw file.
            */
            {
                IntPtr data = Marshal.AllocHGlobal(CHUNKSIZE);
                byte[] buffer = new byte[CHUNKSIZE];
                uint length = 0, read = 0;
                uint bytesread = 0;

                MemoryStream memStream = new MemoryStream();

                result = soundBpm.getLength(ref length, FMOD.TIMEUNIT.PCMBYTES);
                ERRCHECK(result);

                bytesread = 0;
                do
                {
                    result = soundBpm.readData(data, CHUNKSIZE, ref read);

                    Marshal.Copy(data, buffer, 0, CHUNKSIZE);

                    memStream.Write(buffer, 0, (int)read);

                    bytesread += read;
                }
                while (result == FMOD.RESULT.OK && read == CHUNKSIZE);

                /*
                    Loop terminates when either 
                    1. the read function returns an error.  (ie FMOD_ERR_FILE_EOF etc).
                    2. the amount requested was different to the amount returned. (somehow got an EOF without the file error, maybe a non stream file format like mod/s3m/xm/it/midi).

                    If 'bytesread' is bigger than 'length' then it just means that FMOD miscalculated the size, 
                    but this will not usually happen if FMOD_ACCURATETIME is used.  (this will give the correct length for VBR formats)
                */

                memStream.Close();

                // In float umwandeln
                byte[] samples = memStream.ToArray();
                float[] floatSamples = new float[samples.Length / 2];
                //for (int i = 0; i < samples.Length; i += 4)
                for (int i = samples.Length / 4; i < samples.Length*3/4 ; i += 4)
                {
                    short sampleLeft = BitConverter.ToInt16(samples, i);
                    short sampleRight = BitConverter.ToInt16(samples, i+2);

                    floatSamples[i / 2] = (float)sampleLeft / 32768.0f;
                    floatSamples[i / 2+1] = (float)sampleRight / 32768.0f;
                }

                BigMansStuff.PracticeSharp.Core.SoundTouchSharp soundTouch = new BigMansStuff.PracticeSharp.Core.SoundTouchSharp();
                    
                bpm = soundTouch.DetectBPM(2, 44100, floatSamples, floatSamples.Length);

                /*
                    Shut down
                */
                if (soundBpm != null)
                {
                    result = soundBpm.release();
                    ERRCHECK(result);
                }
                if (systemBpm != null)
                {
                    result = systemBpm.close();
                    ERRCHECK(result);
                    result = systemBpm.release();
                    ERRCHECK(result);
                }
            }

            return bpm;
        }
#endif

        public void Uninitialize()
        {
            if (cdSound != null)
            {
                cdSound.release();
                cdSound = null;
            }

            Stop();
        }

        public void Stop()
        {
            if (mciCDEngine != null)
            {
                mciCDEngine.Stop();
                mciCDEngine.Close();
                mciCDEngine = null;
            }

            if (currentChannel != null)
            {
                currentChannel.stop();

                if (currentSoundIndex == 0)
                    channel1 = null;
                else
                    channel2 = null;
            }

            // Bei einer CD den cdSound offen lassen, damit das wechseln zwischen subTracks schneller geht.
            if (cdSound == null)
            {
                if (currentSound != null)
                {
                    currentSound.release();
                    if (currentSoundIndex == 0)
                        sound1 = null;
                    else
                        sound2 = null;
                }
            }

           /* Aktuell keinen PlayCounter schreiben, das die ID3-Routinen Buggy sind!!
            * for (int i = PlayCountFileList.Count - 1; i >= 0;i-- )
            {
                try
                {
                    FileStream fs = File.OpenWrite(PlayCountFileList[i]);
                    fs.Close();

                    SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(PlayCountFileList[i]);

                    sfi.PlayCount++;

                    SoundFileInformation.WriteMP3Tags(sfi, Field.TrackPlayCount);

                    PlayCountFileList.RemoveAt(i);
                }
                catch
                {

                }
            }*/
        }

        /// <summary>
        /// Alles stoppen (auch eventuell laufende Überblendung).
        /// </summary>
        public void StopAll()
        {
            Stop();

            if (!this.isPreListenSoundEngine)
            {
                if (currentSoundIndex == 0)
                    currentSoundIndex = 1;
                else
                    currentSoundIndex = 0;
            }

            Stop();

            Uninitialize();
        }

        public void Pause(bool paused)
        {
            if (IsPlaying)
            {
                if (mciCDEngine != null)
                {
                    mciCDEngine.Pause(paused);
                }
                else
                {
                    currentChannel.setPaused(paused);
                }
            }
        }

        public bool IsPaused
        {
            get
            {
                bool paused = false;

                if (IsPlaying)
                {
                    if (mciCDEngine != null)
                    {
                        return mciCDEngine.IsPaused;
                    }

                    if (currentChannel != null)
                    {
                        currentChannel.getPaused(ref paused);
                    }
                }

                return paused;
            }
        }

        public bool IsPlaying
        {
            get
            {
                bool isPlaying = false;

                if (mciCDEngine != null)
                {
                    isPlaying = mciCDEngine.GetPlayPositionMS() > 0;
                }

                if (currentChannel != null)
                    currentChannel.isPlaying(ref isPlaying);

                return isPlaying;
            }
        }

        private bool isMuted = false;
        public bool IsMuted
        {
            get
            {
                return isMuted;
            }
            set
            {
                if (currentChannel != null)
                    currentChannel.setMute(value);

                isMuted = value;
            }
        }

        public int PlayPosition
        {
            get
            {
                if (!IsPlaying)
                    return 0;

                if (mciCDEngine != null)
                {
                    return mciCDEngine.GetPlayPositionMS();
                }

                uint position = 0;
                currentChannel.getPosition(ref position, FMOD.TIMEUNIT.MS);

                return (int)position;
            }
            set
            {
                if (IsPlaying)
                {
                    if (currentChannel != null)
                    {
                        currentChannel.setPosition((uint)value, FMOD.TIMEUNIT.MS);
                    }

                    if (mciCDEngine != null)
                    {
                        mciCDEngine.SetPlayPositionMS(value);
                    }
                }
            }
        }

        public int Length
        {
            get
            {
                if (!IsPlaying)
                    return 0;

                if (mciCDEngine != null)
                {
                    return mciCDEngine.GetTrackLength(currentCDTrackNumber);
                }

                uint length = 0;
                currentSound.getLength(ref length, FMOD.TIMEUNIT.MS);

                return (int)length;
            }
        }

        private float volume = 1;
        public float Volume
        {
            get
            {
                return volume;
            }
            set
            {
                if (currentChannel != null)
                    currentChannel.setVolume(value);

                volume = value;
            }
        }

        private int currentTrackBPM;

        public int CurrentTrackBPM
        {
            get { return currentTrackBPM; }
            set { currentTrackBPM = value; }
        }

        /// <summary>
        /// Liefert TRUE zurück, wenn es sich um ein unterstütztes Sound-Format handelt.
        /// Dabei wird nur die Extension geprüft.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsSupportedFileType(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();

            if (ext == ".mp3" || ext == ".wav" || ext == ".wma" || ext == ".asf" || ext == ".ogg" ||
                ext == ".mp2" || ext == ".mpg" || ext == ".mpeg" || ext == ".ogm" || ext == ".mp4" || ext == ".lnk" ||
                ext == ".flac")
                return true;

            return false;
        }

        public float[][] GetWaveData()
        {
            int numchannels = 0;
            int dummy = 0;
            FMOD.SOUND_FORMAT dummyformat = FMOD.SOUND_FORMAT.NONE;
            FMOD.DSP_RESAMPLER dummyresampler = FMOD.DSP_RESAMPLER.LINEAR;
            int count = 0;
            const int WAVEDATASIZE = 512;

            system.getSoftwareFormat(ref dummy, ref dummyformat, ref numchannels, ref dummy, ref dummyresampler, ref dummy);

            float[][] wavedata = new float[numchannels][];

            /*
                    DRAW WAVEDATA
            */
            for (count = 0; count < numchannels; count++)
            {
                wavedata[count] = new float[WAVEDATASIZE];

                if (currentChannel != null)
                {
                    currentChannel.getWaveData(wavedata[count], WAVEDATASIZE, count);
                }
            }

            return wavedata;
        }

        const int SPECTRUMSIZE = 2048;

        public float[][] GetSpectrum()
        {
            system.update();

            int numchannels = 0;
            int dummy = 0;
            FMOD.SOUND_FORMAT dummyformat = FMOD.SOUND_FORMAT.NONE;
            FMOD.DSP_RESAMPLER dummyresampler = FMOD.DSP_RESAMPLER.LINEAR;
            int count = 0;
            int count2 = 0;

            system.getSoftwareFormat(ref dummy, ref dummyformat, ref numchannels, ref dummy, ref dummyresampler, ref dummy);
            float[][] spectrum = new float[numchannels][];

            /*
                    DRAW SPECTRUM
            */
            for (count = 0; count < numchannels; count++)
            {
                float max = 0;

                spectrum[count] = new float[SPECTRUMSIZE];

                if (currentChannel != null)
                {
                    currentChannel.getSpectrum(spectrum[count], SPECTRUMSIZE, count, FMOD.DSP_FFT_WINDOW.HAMMING);

                    /*for (count2 = 0; count2 < SPECTRUMSIZE; count2++)
                    {
                        if (max < spectrum[count][count2])
                        {
                            max = spectrum[count][count2];
                        }
                    }*/

                    // Sonst sieht man nicht viel...
                    max = 0.2F;

                    for (count2 = 0; count2 < SPECTRUMSIZE; count2++)
                    {
                        float height;

                        height = spectrum[count][count2] / max;

                        if (height >= 1)
                        {
                            height = 1;
                        }

                        if (height < 0)
                        {
                            height = 0;
                        }

                        spectrum[count][count2] = height;
                    }
                }
            }

            return spectrum;
        }

        public static int GetLengthOfSoundfile(string filename)
        {
            FMOD.Sound sound = null;
            FMOD.RESULT res = SoundEngine.Instance.system.createStream(filename, FMOD.MODE.OPENONLY, ref sound);
            uint length = 0;
            if (sound != null)
            {
                sound.getLength(ref length, FMOD.TIMEUNIT.MS);
                sound.release();

                return (int)length;
            }

            return 0;
        }

        public static void GetSoundTags(string filename, SoundFileInformation soundFileInfo)
        {
            FMOD.Sound sound = null;
            FMOD.RESULT res = SoundEngine.Instance.system.createStream(filename, FMOD.MODE.OPENONLY, ref sound);
            
            if (sound != null)
            {
                int numtags = 0;
                int num2tags = 0;
                sound.getNumTags(ref numtags, ref num2tags);

                for (int i=0;i<numtags;i++)
                {
                    FMOD.TAG tag2 = new FMOD.TAG();
                    res = sound.getTag(null, i, ref tag2);
                    if (res != FMOD.RESULT.ERR_TAGNOTFOUND)
                    {
                        string tagName = tag2.name.ToLower();
                        if (tagName == "title")
                        {
                            soundFileInfo.Title = Marshal.PtrToStringAnsi(tag2.data);
                        }

                        if (tagName == "author" || tagName == "artist")
                        {
                            soundFileInfo.Artist = Marshal.PtrToStringAnsi(tag2.data);
                        }

                        if (tagName == "wm/tracknumber" || tagName == "tracknumber")
                        {
                            if (tag2.datatype == FMOD.TAGDATATYPE.STRING)
                                soundFileInfo.TrackNumber = Big3.Hitbase.Miscellaneous.Misc.Atoi(Marshal.PtrToStringAnsi(tag2.data));

                            if (tag2.datatype == FMOD.TAGDATATYPE.INT)
                                soundFileInfo.TrackNumber = Convert.ToInt32(Marshal.ReadInt32(tag2.data));
                        }

                        if (tagName == "wm/composer")
                        {
                            soundFileInfo.Composer = Marshal.PtrToStringAnsi(tag2.data);
                        }

                        if (tagName == "wm/albumtitle" || tagName == "album")
                        {
                            soundFileInfo.Album = Marshal.PtrToStringAnsi(tag2.data);
                        }

                        if (tagName == "wm/year")
                        {
                            soundFileInfo.Year = Big3.Hitbase.Miscellaneous.Misc.Atoi(Marshal.PtrToStringAnsi(tag2.data));
                        }

                        if (tagName == "wm/genre" || tagName == "genre")
                        {
                            soundFileInfo.Genre = Marshal.PtrToStringAnsi(tag2.data);
                        }
                    }
    
                }

                sound.release();
            }
        }

        public static string[] GetOutputDevices()
        {
            List<string> outputDevices = new List<string>();

            int numdrivers = 0;
            SoundEngine.Instance.system.getNumDrivers(ref numdrivers);

            for (int i = 0; i < numdrivers; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Capacity = 1000;
                FMOD.GUID dummy = new FMOD.GUID();
                SoundEngine.Instance.system.getDriverInfo(i, sb, 1000, ref dummy);
                outputDevices.Add(sb.ToString());
            }

            return outputDevices.ToArray();
        }

        public static CDInfo ReadCDInformationMCI(char driveLetter)
        {
            MCICDEngine mciCDEngine = new MCICDEngine();
            mciCDEngine.Open(driveLetter);
            mciCDEngine.Close();

            return mciCDEngine.CDInfo;
        }

        [DllImport("kernel32.dll")]
        private static extern long GetVolumeInformation(string PathName, StringBuilder VolumeNameBuffer, UInt32 VolumeNameSize, ref UInt32 VolumeSerialNumber, ref UInt32 MaximumComponentLength, ref UInt32 FileSystemFlags, StringBuilder FileSystemNameBuffer, UInt32 FileSystemNameSize);

        /// <summary>
        /// Die Informationen (TOC) einer CD auslesen
        /// </summary>
        public CDInfo ReadCDInformation(char driveLetter)
        {
            if (Settings.Current.UseMCI)
                return ReadCDInformationMCI(driveLetter);

            CDInfo mciCDInfo = ReadCDInformationMCI(driveLetter);

            if (mciCDInfo.Tracks.Any(x => x.TrackType != TrackType.Audio))  // Bei einer Daten-CD nehmen wir MCI
                return mciCDInfo;

            string drive = string.Format("{0}:", driveLetter);
            FMOD.Sound cdSound = null;

            FMOD.RESULT result = system.createStream(drive, FMOD.MODE.OPENONLY, ref cdSound);
            if (result != FMOD.RESULT.OK || cdSound == null)
                return mciCDInfo;

            int numberOfTracks = 0;
            cdSound.getNumSubSounds(ref numberOfTracks);

            FMOD.CDTOC cdToc = new FMOD.CDTOC();
            
            int numtags=0;
            int numtagsupdated=0;
            cdSound.getNumTags(ref numtags, ref numtagsupdated);

            for (int i = 0; i < numtags; i++)
            {
                FMOD.TAG tag = new FMOD.TAG();
                if (cdSound.getTag(null, i, ref tag) != FMOD.RESULT.OK)
                    break;
                if (tag.datatype == FMOD.TAGDATATYPE.CDTOC)
                {
                    cdToc = (FMOD.CDTOC)Marshal.PtrToStructure(tag.data, typeof(FMOD.CDTOC));
                    break;
                }
                
            }

            CDInfo cdInfo = new CDInfo();

            int trackNr = 0;
            for (; trackNr < numberOfTracks; trackNr++)
            {
                CDInfoTrack track = new CDInfoTrack();

                track.StartTime = (((cdToc.min[trackNr] * 60 + cdToc.sec[trackNr]) * 75 + cdToc.frame[trackNr]) * 1000 + 74) / 75;

                if (trackNr > 0)
                {
                    cdInfo.Tracks[trackNr - 1].Length = track.StartTime - cdInfo.Tracks[trackNr - 1].StartTime;
                }
                track.TrackType = TrackType.Audio;
                cdInfo.Tracks.Add(track);
            }

            cdInfo.TotalLength = (((cdToc.min[numberOfTracks] * 60 + cdToc.sec[numberOfTracks]) * 75 + cdToc.frame[numberOfTracks]) * 1000 + 74) / 75;
            cdInfo.Tracks[trackNr - 1].Length = cdInfo.TotalLength - cdInfo.Tracks[trackNr - 1].StartTime;

            cdSound.release();

            /*StringBuilder volumeName = new StringBuilder(1000);
            StringBuilder fileSystemNameBuffer = new StringBuilder(1000);
            uint serialNumber = 0;
            uint maximumComponentLength = 0;
            uint fileSystemFlags = 0;
            GetVolumeInformation(drive, volumeName, 1000, ref serialNumber, ref maximumComponentLength, ref fileSystemFlags, fileSystemNameBuffer, 1000);

            cdInfo.Identity = serialNumber.ToString();*/

            cdInfo.Identity = mciCDInfo.Identity;

            return cdInfo;
        }


        public delegate void ConvertToWaveProgressDelegate(string sourceFile, string targetFile, double percent);

        /// <summary>
        /// Konvertieren von von Sound Dateien zu WAVE mit PCM Header
        /// </summary>
        /// <param name="src">Kompletter Pfad zur Quell Datei</param>
        /// <param name="dest">Kompletter Pfad zur Ziel Datei</param>
        /// <returns></returns>
        public bool ConvertToWAVE(string src_file, string dest_file, ConvertToWaveProgressDelegate convertToWaveProgressDelegate)
        {
            uint version = 0;
            FMOD.RESULT result;
            const int CHUNKSIZE = 262144;

            FMOD.System  system  = null;
            FMOD.Sound   sound   = null;
            /*
                Global Settings
            */
            result = FMOD.Factory.System_Create(ref system);
            if (result != FMOD.RESULT.OK)
                return false;

            result = system.getVersion(ref version);
            ERRCHECK(result);
            if (version < FMOD.VERSION.number)
            {
                //MessageBox.Show("Error!  You are using an old version of FMOD " + version.ToString("X") + ".  This program requires " + FMOD.VERSION.number.ToString("X") + ".");
                return false;
            }

            result = system.init(1, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
            ERRCHECK(result);

            result = system.createSound(src_file, FMOD.MODE.OPENONLY | FMOD.MODE.ACCURATETIME, ref sound);

            if (result != FMOD.RESULT.OK)
                return false;


            /*
                Decode the sound and write it to a .raw file.
            */
            IntPtr data = Marshal.AllocHGlobal(CHUNKSIZE);
            byte[] buffer = new byte[CHUNKSIZE];
            uint length = 0, read = 0;
            uint bytesread = 0;

            FileStream fs = new FileStream(dest_file, FileMode.Create, FileAccess.Write);

            result = sound.getLength(ref length, FMOD.TIMEUNIT.PCMBYTES);
            ERRCHECK(result);

            bytesread = 0;
            do
            {
                result = sound.readData(data, CHUNKSIZE, ref read);

                Marshal.Copy(data, buffer, 0, CHUNKSIZE);

                fs.Write(buffer, 0, (int)read);

                bytesread += read;

                double percent = 100.0 / (double)length * (double)bytesread;

                convertToWaveProgressDelegate(src_file, dest_file, percent);
                System.Windows.Forms.Application.DoEvents();
                //statusBar.Text = "writing " + bytesread + " bytes of " + length + " to output.raw";
            }
            while (result == FMOD.RESULT.OK && read == CHUNKSIZE);

            //statusBar.Text = "done";

            /*
                Loop terminates when either 
                1. the read function returns an error.  (ie FMOD_ERR_FILE_EOF etc).
                2. the amount requested was different to the amount returned. (somehow got an EOF without the file error, maybe a non stream file format like mod/s3m/xm/it/midi).

                If 'bytesread' is bigger than 'length' then it just means that FMOD miscalculated the size, 
                but this will not usually happen if FMOD_ACCURATETIME is used.  (this will give the correct length for VBR formats)
            */

            fs.Close();


            /*
                Shut down
            */
            if (sound != null)
            {
                result = sound.release();
                ERRCHECK(result);
            }
            if (system != null)
            {
                result = system.close();
                ERRCHECK(result);
                result = system.release();
                ERRCHECK(result);
            }

            return true;
        }

        public List<string> GetSoundDevices()
        {
            List<string> soundDevices = new List<string>();
	        int iNumberOfDrivers = 0;
            
            this.system.getNumDrivers(ref iNumberOfDrivers);

            for (int i=0;i<iNumberOfDrivers;i++)
	        {
                StringBuilder name = new StringBuilder(200);
                FMOD.GUID guid = new FMOD.GUID();
                system.getDriverInfo(i, name, 200, ref guid);
        		soundDevices.Add(name.ToString());
	        }

            return soundDevices;
        }

        private bool isPreListenSoundEngine = false;

        #region privater Konstruktor
        private SoundEngine(bool isPreListenSoundEngine = false)
        {
            this.isPreListenSoundEngine = isPreListenSoundEngine;
            InitSoundEngine();
        }
        #endregion

        #region private Methoden
        private void InitSoundEngine()
        {
            uint version = 0;
            FMOD.RESULT result;

            // Create a System object and initialize.
            result = FMOD.Factory.System_Create(ref system);
            ERRCHECK(result);

            result = system.getVersion(ref version);
            ERRCHECK(result);
            if (version < FMOD.VERSION.number)
            {
                throw new Exception("Error!  You are using an old version of FMOD " + version.ToString("X") + ".  This program requires " + FMOD.VERSION.number.ToString("X") + ".");
            }

            if (isPreListenSoundEngine)
                system.setDriver(Big3.Hitbase.Configuration.Settings.Current.OutputDevicePreListen);
            else
                system.setDriver(Big3.Hitbase.Configuration.Settings.Current.OutputDevice);

	        switch (Big3.Hitbase.Configuration.Settings.Current.VirtualCDBufferSize)
	        {
	            case 0:
                    system.setDSPBufferSize(100, 8);
		            break;
	            case 1:
                    system.setDSPBufferSize(150, 8);
		            break;
	            case 2:
                    system.setDSPBufferSize(200, 8);
		            break;
	            case 3:
                default:
                    system.setDSPBufferSize(500, 8);
		            break;
	        }

            result = system.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
            ERRCHECK(result);

            /*
                Bump up the file buffer size a bit from the 16k default for CDDA, because it is a slower medium.
            */
            result = system.setStreamBufferSize(64 * 1024, FMOD.TIMEUNIT.RAWBYTES);
            ERRCHECK(result);
        }

        private void ERRCHECK(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
            {
                throw new Exception("Fehler bei der Wiedergabe (" + result + "): " + FMOD.Error.String(result));
            }
        }
#endregion
    }
}
