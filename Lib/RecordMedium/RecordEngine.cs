using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.MediaRipper;
using Big3.Hitbase.Configuration;
using System.Windows;
using System.Diagnostics;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.MediaRipper.Mp3;
using Big3.Hitbase.MediaRipper.MMedia;
using Big3.Hitbase.MediaRipper.WMFSdk;
using System.IO;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.SoundEngine;
using Big3.Hitbase.Controls;
using Big3.Hitbase;

namespace Big3.Hitbase.RecordMedium
{
    public class RecordEngine
    {
        private CD cd;
        private DataBase dataBase;
        ulong currentRipTrack;
        int numRipTracks = 0;
        Int64 gesamtRipBytes;
        DateTime StartRipTime;
        WaveWriter wavwriter = null;
        Mp3Writer mp3writer = null;
        private Mp3WriterConfig mp3Config = null;
        private WmaWriterConfig wmaConfig = null;
        WmaWriter wmawriter = null;
        string currentRipname = "";

        public bool Canceled { get; set; }
        public bool RecordRunning { get; set; }
        public int CurrentTrackRecording { get; set; }
        public double CurrentTrackRecordingProgress { get; set; }

        public RecordEngine()
        {
            if (string.IsNullOrEmpty(Settings.Current.RecordSelectedPath))
                Settings.Current.RecordSelectedPath = Misc.GetPersonalHitbaseMusicFolder();
        }

        public void RecordNow(CD cd, DataBase database, char DriveLetter, int[] selectedTracks = null)
        {
            if (RecordRunning)
                return;

            Canceled = false;
            RecordRunning = true;

            try
            {
                this.cd = cd;
                this.dataBase = database;

                // FIrst check for selected record method
                // CDROM???

                CDDrive drive = new CDDrive();
                drive.Open(DriveLetter);

                if (Settings.Current.RecordLastSelectedFormat < 0)
                {
                    return;
                }

                if (drive.LockCD() == false)
                {
                    MessageBox.Show(StringTable.CannotOpenDrive);
                }
                if (drive.Refresh() == false)
                {
                    MessageBox.Show("Kann CD nicht lesen. Liegt eine CD im Laufwerk?");
                }
                
                System.Windows.Application.Current.Dispatcher.BeginInvoke(
                    new Action(() =>
                        Big3.Hitbase.SharedResources.HitbaseCommands.ShowMainStatusText.Execute(true, System.Windows.Application.Current.MainWindow)),
                        null);

                /*formProgress = new RecordFormProgress();
                formProgress.Text = "CD Ripping läuft...";
            
                formProgress.ProgressBar.Maximum = 100;
                formProgress.LabelProgress.Text = "";
                formProgress.Text = "";
            
                formProgress.Show();*/


                string ripfilename;
                string fullfilename;
                string riptracks = "";
                int nPos;
                string[] wavelist = new string[cd.NumberOfTracks];
                Process ProcessDoConvert = new Process();
                string EncodedFile;
                string ExeFilename;
                string ExeArgs;
                string WavFilename;

                if (selectedTracks != null && selectedTracks.Length > 0)
                {
                    riptracks = "";
                    for (nPos = 0; nPos < selectedTracks.Length; nPos++)
                    {
                        riptracks = riptracks + (selectedTracks[nPos]+1).ToString() + ",";
                    }
                    riptracks = riptracks.Substring(0, riptracks.Length - 1);
                }

                if (selectedTracks != null && selectedTracks.Length < 1)
                {
                    return;
                }

                // Alles aufnehmen?
                if (selectedTracks == null)
                {
                    riptracks = "";
                    for (nPos = 1; nPos <= cd.Tracks.Count; nPos++)
                    {
                        riptracks = riptracks + nPos.ToString() + ",";
                    }
                    riptracks = riptracks.Substring(0, riptracks.Length - 1);
                }

                numRipTracks = 0;

                foreach (string track in riptracks.Split(','))
                {
                    numRipTracks = numRipTracks + 1;
                }

                gesamtRipBytes = 0;
                StartRipTime = DateTime.Now;

                int currentTrack = 0;

                string outputFilename;
                
                if (Misc.IsWindows7OrLater())
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(
                        new Action(delegate
                        {
                            // Jetzt haben wir Windows 7
                            System.Windows.Application.Current.MainWindow.TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo();
                            System.Windows.Application.Current.MainWindow.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                            System.Windows.Application.Current.MainWindow.TaskbarItemInfo.ProgressValue = 0.0;
                        }
                        ));
                }

                foreach (string track in riptracks.Split(','))
                {
                    outputFilename = "";
                    nPos = Convert.ToInt32(track) - 1;

                    CurrentTrackRecording = nPos;
                    currentRipTrack = (ulong)currentTrack;

                    string text = "CD wird kopiert...\nTrack " + (currentTrack + 1).ToString() + " von " + numRipTracks.ToString();
                    //formProgress.LabelProgressAll.Text = text;

                    System.Windows.Application.Current.Dispatcher.BeginInvoke(
                        new Action(delegate
                            {
                                Big3.Hitbase.SharedResources.HitbaseCommands.SetMainStatusText.Execute(text, System.Windows.Application.Current.MainWindow);
                            }));

                    ripfilename = BuildFileName(nPos) + ".wav";
                    currentRipname = ripfilename;

                    string completeFilename = Path.Combine(Settings.Current.RecordSelectedPath, ripfilename);

                    if (Settings.Current.RecordLastQuickSelectedFormat == 0)
                    {
                        WriteMP3(drive, nPos, completeFilename);

                        fullfilename = System.IO.Path.ChangeExtension(completeFilename, ".mp3");
                        Big3.Hitbase.SoundEngine.SoundFileInformation.WriteMP3Tags(fullfilename, cd, nPos);

                        outputFilename = System.IO.Path.ChangeExtension(completeFilename, ".mp3");
                    }

                    if (Settings.Current.RecordLastQuickSelectedFormat == 4)
                    {
                        WriteWav(drive, nPos, completeFilename);
                        outputFilename = completeFilename;
                        outputFilename = System.IO.Path.ChangeExtension(outputFilename, ".wav");
                    }

                    if (Settings.Current.RecordLastQuickSelectedFormat == 2 ||
                        Settings.Current.RecordLastQuickSelectedFormat == 3 ||
                        Settings.Current.RecordLastQuickSelectedFormat == 5)
                    {
                        WriteWav(drive, nPos, completeFilename);

                        // Check now for additional tasks - convert to MP3, Flac, Ogg, Custom
                        ExeFilename = "";
                        EncodedFile = "";
                        ExeArgs = "";
                        // Lame.exe batch convert
                        if (Settings.Current.RecordLastQuickSelectedFormat == 3)
                        {
                            EncodedFile = System.IO.Path.ChangeExtension(completeFilename, ".flac");
                            ExeArgs = Settings.Current.RecordFlacExeParameter;

                            ExeFilename = "flac.exe";
                        }
                        // OGG.exe
                        if (Settings.Current.RecordLastQuickSelectedFormat == 2)
                        {
                            EncodedFile = System.IO.Path.ChangeExtension(completeFilename, ".ogg");
                            ExeArgs = Settings.Current.RecordOggExeParameter;

                            ExeFilename = "oggenc2.exe";
                        }
                        // Lame.exe batch convert
                        /*if (Settings.Current.RecordLastSelectedFormat == 3)
                        {
                            EncodedFile = System.IO.Path.ChangeExtension(completeFilename, ".mp3");
                            ExeArgs = Settings.Current.RecordLameExeParameter;

                            ExeFilename = "lame.exe";
                        }*/
                        // External 1 batch convert
                        if (Settings.Current.RecordLastQuickSelectedFormat == 5)
                        {
                            EncodedFile = System.IO.Path.ChangeExtension(completeFilename, ".mp3");
                            int ParamStartPos;
                            ParamStartPos = Settings.Current.RecordUser1ExeParameter.IndexOf(' ');

                            if (ParamStartPos > 0)
                                ExeFilename = Settings.Current.RecordUser1ExeParameter.Left(Settings.Current.RecordUser1ExeParameter.IndexOf(' '));
                            if (Settings.Current.RecordUser1ExeParameter.Length > ParamStartPos + 1)
                                ExeArgs = Settings.Current.RecordUser1ExeParameter.Mid(Settings.Current.RecordUser1ExeParameter.IndexOf(' '));
                            if (ExeFilename.Length < 2)
                                return;
                        }
                        // External 2 batch convert
                        /*if (Settings.Current.RecordLastSelectedFormat == 7)
                        {
                            EncodedFile = System.IO.Path.ChangeExtension(@"\" + ripfilename, ".wav");
                            int ParamStartPos;
                            ParamStartPos = Settings.Current.RecordUser2ExeParameter.IndexOf(' ');

                            if (ParamStartPos > 0)
                                ExeFilename = Settings.Current.RecordUser2ExeParameter.Left(ParamStartPos);
                            if (Settings.Current.RecordUser2ExeParameter.Length > ParamStartPos + 1)
                                ExeArgs = Settings.Current.RecordUser2ExeParameter.Mid(ParamStartPos);

                            if (ExeFilename.Length < 2)
                                return;
                        }*/

                        // Wait for last process
                        /*                    if (nPos > 0)
                                            {
                                                while (!ProcessDoConvert.HasExited)
                                                {
                                                    System.Windows.Forms.Application.DoEvents();
                                                    System.Threading.Thread.Sleep(1000);
                                                }
                                            }*/

                        WavFilename = System.IO.Path.ChangeExtension(completeFilename, ".wav");
                        string NoExtension = System.IO.Path.ChangeExtension(completeFilename, "");
                        NoExtension = NoExtension.TrimEnd('.');
                        //string NoFilename = textFileDir.Text;

                        if (Settings.Current.RecordLastQuickSelectedFormat == 5)
                        {
                            int SlashPos = WavFilename.LastIndexOf('\\');
                            string PathOnly = WavFilename.Left(SlashPos);
                            string FilenameOnly = WavFilename.Mid(SlashPos + 1);
                            string NameWithoutExt = System.IO.Path.ChangeExtension(FilenameOnly, null);

                            ExeArgs = ExeArgs.Replace("%0", "\"" + PathOnly + "\"");
                            ExeArgs = ExeArgs.Replace("%1", "\"" + WavFilename + "\"");
                            ExeArgs = ExeArgs.Replace("%2", FilenameOnly);
                            ExeArgs = ExeArgs.Replace("%3", NameWithoutExt);
                        }
                        else
                        {
                            ExeArgs = ExeArgs.Replace("%1", "\"" + WavFilename + "\"");
                            ExeArgs = ExeArgs.Replace("%2", "\"" + EncodedFile + "\"");
                        }

                        ProcessDoConvert.StartInfo.Arguments = ExeArgs;
                        ProcessDoConvert.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        ProcessDoConvert.StartInfo.FileName = ExeFilename;
                        ProcessDoConvert.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

                        ProcessDoConvert.Start();

                        while (!ProcessDoConvert.HasExited)
                        {
                            System.Windows.Forms.Application.DoEvents();
                            System.Threading.Thread.Sleep(1000);
                        }// ProcessDoConvert.WaitForExit();

                        // Always write ID3 Tags
                        //if (checkWriteID3Tags.Checked == true)
                        //{
                        Big3.Hitbase.SoundEngine.SoundFileInformation.WriteMP3Tags(EncodedFile, cd, nPos);
                        //}

                        //ProcessDoConvert.PriorityClass = ProcessPriorityClass.BelowNormal;

                        wavelist[nPos] = WavFilename;

                        outputFilename = EncodedFile;
                    }

                    // Write WMA
                    if (Settings.Current.RecordLastQuickSelectedFormat == 1)
                    {
                        WriteWMA(drive, nPos, completeFilename);

                        outputFilename = System.IO.Path.ChangeExtension(completeFilename, ".wma");
                    }

                    if (Canceled)
                    {
                        // Wenn die Output-datei schon (teilweise) angelegt wurde, dann wieder löschen.
                        if (File.Exists(outputFilename))
                        {
                            File.Delete(outputFilename);
                        }
                        break;
                    }
                    //if (formProgress.Canceled == true)
                    //    break;

                    cd.Tracks[nPos].Soundfile = outputFilename;
                    currentTrack += 1;
                    //formProgress.ProgressBarAll.Value = currentTrack;
                    // OS auf WindowsFormsSection 7 abfragen
                    //if (System.Environment.OSVersion.Version.Major == 6 && System.Environment.OSVersion.Version.Minor == 1)
                    //{
                    // Jetzt haben wir Windows 7
                    //    Windows7.DesktopIntegration.Windows7Taskbar.SetProgressState(mainWindow, Windows7.DesktopIntegration.Windows7Taskbar.ThumbnailProgressState.Normal);
                    //    Windows7.DesktopIntegration.Windows7Taskbar.SetProgressValue(mainWindow, Convert.ToUInt32(currentTrack), Convert.ToUInt32(formProgress.ProgressBarAll.Maximum));
                    //}
                }

                if (Settings.Current.RecordLastQuickSelectedFormat == 2 ||
                    Settings.Current.RecordLastQuickSelectedFormat == 3 ||
                    Settings.Current.RecordLastQuickSelectedFormat == 5)
                {
                    while (!ProcessDoConvert.HasExited)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        System.Threading.Thread.Sleep(1000);
                    }

                    foreach (string delwave in wavelist)
                    {
                        if (delwave != null)
                            File.Delete(delwave);
                    }
                }

                //formProgress.Close();
                drive.UnLockCD();

                if (Misc.IsWindows7OrLater())
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(
                        new Action(delegate
                            {
                                System.Windows.Application.Current.MainWindow.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                                System.Windows.Application.Current.MainWindow.TaskbarItemInfo.ProgressValue = 0;
                            }
                    ));

                }

                // access current selektion main window
                if (Settings.Current.RecordAutoEject == true)
                    drive.EjectCD();

                string ripDir = BuildFileName(0);
                string copyfilename;
                string imageFilename;

                if (ripDir.LastIndexOf('\\') >= 0)
                    ripDir = Settings.Current.RecordSelectedPath + @"\" + ripDir.Left(ripDir.LastIndexOf('\\'));
                else
                    ripDir = Settings.Current.RecordSelectedPath;

                if (!string.IsNullOrEmpty(cd.CDCoverFrontFilename) && Settings.Current.RecordSaveCDCover == true)
                {
                    if (!string.IsNullOrEmpty(cd.CDCoverFrontFilename))
                    {
                        imageFilename = Misc.GetCDCoverFilename(cd.CDCoverFrontFilename);
                        // Bild kopieren
                        if (!string.IsNullOrEmpty(imageFilename) && File.Exists(imageFilename))
                        {
                            copyfilename = ripDir + @"\" + imageFilename.Mid(imageFilename.LastIndexOf('\\') + 1);
                            File.Copy(imageFilename, copyfilename, true);
                        }
                    }
                }

                if (Settings.Current.RecordAutoCreateM3U == true)
                {
                    string filenameM3U = ripDir.Mid(ripDir.LastIndexOf('\\') + 1);
                    //filenameM3U = filenameM3U.Mid(filenameM3U.LastIndexOf('\\'));

                    filenameM3U = ripDir + @"\" + filenameM3U + ".m3u";
                    Playlist pl = new Playlist();

                    for (int i = 0; i < cd.NumberOfTracks; i++)
                    {
                        PlaylistItem item = new PlaylistItem(pl);
                        item.Info = new SoundFileInformation();
                        item.Info.Artist = cd.Tracks[i].Artist;
                        item.Info.Title = cd.Tracks[i].Title;
                        item.Info.Length = cd.Tracks[i].Length;
                        item.Info.Filename = cd.Tracks[i].Soundfile;

                        pl.Add(item);
                    }
                    pl.SaveToFile(filenameM3U);
                }

                System.Windows.Application.Current.Dispatcher.BeginInvoke(
                    new Action(delegate
                        {
                            Big3.Hitbase.SharedResources.HitbaseCommands.ShowMainStatusText.Execute(false, System.Windows.Application.Current.MainWindow);
                        }));


                // Get selected device
                // look here http://msdn.microsoft.com/en-us/library/bb318690(VS.85).aspx
                //Capture dsCapture = new Capture(deviceGuid);
            }
            catch (Exception e)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(
                 new Action(delegate
                 {
                     UnhandledExceptionWindow unhandledExceptionWindow = new UnhandledExceptionWindow(e);
                     unhandledExceptionWindow.ShowDialog();
                 }));
            }
            finally
            {
                RecordRunning = false;
                System.Windows.Application.Current.Dispatcher.BeginInvoke(
                 new Action(delegate
                        {
                            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                        }));
            }
        }

        public bool CheckEmptyFields(CD cd, DataBase database, char DriveLetter, int[] selectedTracks = null)
        {
            this.cd = cd;
            this.dataBase = database;

            string riptracks = "";
            int nPos;

            if (selectedTracks != null && selectedTracks.Length > 0)
            {
                riptracks = "";
                for (nPos = 0; nPos < selectedTracks.Length; nPos++)
                {
                    riptracks = riptracks + (selectedTracks[nPos] + 1).ToString() + ",";
                }
                riptracks = riptracks.Substring(0, riptracks.Length - 1);
            }

            if (selectedTracks != null && selectedTracks.Length < 1)
            {
                return false;
            }

            // Alles aufnehmen?
            if (selectedTracks == null)
            {
                riptracks = "";
                for (nPos = 1; nPos <= cd.Tracks.Count; nPos++)
                {
                    riptracks = riptracks + nPos.ToString() + ",";
                }
                riptracks = riptracks.Substring(0, riptracks.Length - 1);
            }

            string emptyfields = "";

            foreach (string track in riptracks.Split(','))
            {
                nPos = Convert.ToInt32(track) - 1;
                emptyfields = BuildFileName(nPos, true);
            }

            if (emptyfields.Length > 0)
            {
                string[] allemptyfields = emptyfields.Split(';');
                string allcd=null, alltracks=null;
                int cdcount=0, trackcount=0;
                foreach (string item in allemptyfields)
	            {
                    if (item.StartsWith("CD:"))
                    {
                        cdcount++;
                        if (string.IsNullOrEmpty(allcd))
                        {
                            allcd = "Folgende CD Felder sind nicht gefüllt:\n";
                            allcd = allcd + item.Substring(3);
                        }
                        else
                        {
                            allcd = allcd + ", " + item.Substring(3);
                            if ((cdcount % 4) == 0)
                            {
                                allcd = allcd + "\n";
                            }
                        }
                    }
                    if (item.StartsWith("Track:"))
                    {
                        trackcount++;
                        if (string.IsNullOrEmpty(alltracks))
                        {
                            alltracks = "Folgende Track Felder sind nicht gefüllt:\n";
                            alltracks = alltracks + item.Substring(6);
                        }
                        else
                        {
                            alltracks = alltracks + ", " + item.Substring(6);
                            if ((trackcount % 4) == 0)
                            {
                                alltracks = alltracks + "\n";
                            }
                        }
                    }
                }
                WpfMessageBoxResult result = WPFMessageBox.Show(System.Windows.Application.Current.MainWindow,
                                                "Felder, die für das Erzeugen der Dateinamen benötigt werden, sind nicht gefüllt.", allcd + "\n\n" + alltracks + "\n\nSoll die CD trotzdem kopieren werden?" ,
                                                "/Big3.Hitbase.SharedResources;component/Images/Warning.png",
                                                WpfMessageBoxButtons.YesNo, "RIPFileNameEmptyfieldsWarning", false, 300);

                if (result != WpfMessageBoxResult.Yes)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateMP3Struct()
        {
            mp3Config.Mp3Config.format.lhv1.dwMpegVersion = 1;
            mp3Config.Mp3Config.format.lhv1.bWriteVBRHeader = 1;

            if (Settings.Current.RecordMP3CustomChannels == 0)
            {
                mp3Config.Mp3Config.format.lhv1.nMode = MediaRipper.Lame.MpegMode.STEREO;
            }
            if (Settings.Current.RecordMP3CustomChannels == 1)
            {
                mp3Config.Mp3Config.format.lhv1.nMode = MediaRipper.Lame.MpegMode.JOINT_STEREO;
            }

            if (Settings.Current.RecordMP3CustomCRC == true)
                mp3Config.Mp3Config.format.lhv1.bCRC = 1;
            else
                mp3Config.Mp3Config.format.lhv1.bCRC = 0;

            if (Settings.Current.RecordMP3CustomPrivate == true)
                mp3Config.Mp3Config.format.lhv1.bPrivate = 1;
            else
                mp3Config.Mp3Config.format.lhv1.bPrivate = 0;

            if (Settings.Current.RecordMP3CustomOriginal == true)
                mp3Config.Mp3Config.format.lhv1.bOriginal = 1;
            else
                mp3Config.Mp3Config.format.lhv1.bOriginal = 0;

            if (Settings.Current.RecordMP3CustomCopyright == true)
                mp3Config.Mp3Config.format.lhv1.bCopyright = 1;
            else
                mp3Config.Mp3Config.format.lhv1.bCopyright = 0;
            if (Settings.Current.RecordLastQuickSelectedMP3Quality == 7)
            {
                if (Settings.Current.RecordMP3CustomCBR == true)
                {
                    if (Settings.Current.RecordMP3CustomCBRBitRates >= 0)
                        mp3Config.Mp3Config.format.lhv1.dwBitrate = (uint)Settings.Current.RecordMP3CustomCBRBitRates;
                    mp3Config.Mp3Config.format.lhv1.bEnableVBR = 0;
                }
                else
                {
                    mp3Config.Mp3Config.format.lhv1.bEnableVBR = 1;

                    if (Settings.Current.RecordMP3CustomVBRMethod == 0)
                    {
                        mp3Config.Mp3Config.format.lhv1.nVbrMethod = MediaRipper.Lame.VBRMETHOD.VBR_METHOD_NEW;
                        // TODO????? 
                        mp3Config.Mp3Config.format.lhv1.nQuality = 0xFF00;
                        // 0 = Highest Quality - 9 = Lowest Quality
                        mp3Config.Mp3Config.format.lhv1.nVBRQuality = 9 - (int)Settings.Current.RecordMP3CustomVBRQuality;
                    }
                    if (Settings.Current.RecordMP3CustomVBRMethod == 1)
                    {
                        mp3Config.Mp3Config.format.lhv1.nVbrMethod = MediaRipper.Lame.VBRMETHOD.VBR_METHOD_ABR;
                        mp3Config.Mp3Config.format.lhv1.dwVbrAbr_bps = (uint)Settings.Current.RecordMP3CustomVBRAverage;
                    }

                    mp3Config.Mp3Config.format.lhv1.dwBitrate = (uint)Settings.Current.RecordMP3CustomVBRMin;
                    mp3Config.Mp3Config.format.lhv1.dwMaxBitrate = (uint)Settings.Current.RecordMP3CustomVBRMax;

                    //TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! check following comment:
                    //bWriteVBRHeader - Specifes if the XING VBR header should be written or not. When this option is enabled, you have to call the beWriteVBRHeader function when encoding has been completed. Keep in mind that the VBR info tag can also be written for CBR encoded files, the TAG info can be useful for additional info like encoder delay and the like.
                }
            }
            else
            {
                switch ((int)Settings.Current.RecordLastQuickSelectedMP3Quality)
                {
                    case 0:
                        mp3Config.Mp3Config.format.lhv1.dwBitrate = (uint)64;
                        break;
                    case 1:
                        mp3Config.Mp3Config.format.lhv1.dwBitrate = (uint)96;
                        break;
                    case 2:
                        mp3Config.Mp3Config.format.lhv1.dwBitrate = (uint)128;
                        break;
                    case 3:
                        mp3Config.Mp3Config.format.lhv1.dwBitrate = (uint)160;
                        break;
                    case 4:
                        mp3Config.Mp3Config.format.lhv1.dwBitrate = (uint)192;
                        break;
                    case 5:
                        mp3Config.Mp3Config.format.lhv1.dwBitrate = (uint)256;
                        break;
                    case 6:
                        mp3Config.Mp3Config.format.lhv1.dwBitrate = (uint)320;
                        break;
                    default:
                        mp3Config.Mp3Config.format.lhv1.dwBitrate = (uint)192;
                        break;
                }

                mp3Config.Mp3Config.format.lhv1.bEnableVBR = 0;
            }
        }

        public void WriteWav(CDDrive drive, int track, string fname)
        {
            //string trackName1 = CD.Tracks[0].Title;
            //string completeFilename = textFileDir.Text + @"\" + fname;
            fname = System.IO.Path.ChangeExtension(fname, ".wav");

            string newdir = fname;
            newdir = newdir.Substring(0, newdir.LastIndexOf('\\'));

            Directory.CreateDirectory(newdir);

            Stream WaveFile = new FileStream(fname, FileMode.Create, FileAccess.Write);

            Big3.Hitbase.MediaRipper.WaveFormat waveform = new Big3.Hitbase.MediaRipper.WaveFormat(44100, 16, 2);

            //qqq.nChannels = 1;
            //qqq.nSamplesPerSec = 11025;
            //qqq.wBitsPerSample = 8;

            wavwriter = new WaveWriter(WaveFile, waveform/*, m_Drive.TrackSize(track)*/);
            int result = drive.ReadTrack(track + 1, new CdDataReadEventHandler(WriteWaveData), new CdReadProgressEventHandler(CdReadProgress));
            //drive.UnLockCD();
            wavwriter.Close();
            wavwriter = null;
            //WaveFile.Close();
            //drive.Close();
        }

        public void WriteMP3(CDDrive drive, int track, string fname)
        {
            try
            {
                fname = System.IO.Path.ChangeExtension(fname, ".mp3");

                if (mp3Config == null)
                {
                    mp3Config = new Mp3WriterConfig();
                }
                UpdateMP3Struct();
                //fname = textFileDir.Text + @"\" + fname;
                // TEST
                //m_Config.Mp3Config.format.lhv1.dwBitrate = 192;

                string newdir = fname;
                // Das hier ist schlecht.....
                newdir = newdir.Substring(0, newdir.LastIndexOf('\\'));

                Directory.CreateDirectory(newdir);

                //Big3.Hitbase.MediaRipper.WaveFormat Format = new Big3.Hitbase.MediaRipper.WaveFormat(44100, 16, 2);
                mp3writer = new Mp3Writer(new FileStream(fname, FileMode.Create), mp3Config);
                //
                try
                {
                    //writer = new WaveWriter(WaveFile, Format/*, m_Drive.TrackSize(track)*/);
                    try
                    {
                        //statusBar.Text = string.Format("Reading track {0}", track);
                        DateTime InitTime = DateTime.Now;
                        if (drive.ReadTrack(track + 1, new CdDataReadEventHandler(WriteWaveData), new CdReadProgressEventHandler(this.CdReadProgress)) > 0)
                        {
                            TimeSpan Duration = DateTime.Now - InitTime;
                            //double Speed = drive.TrackSize(1) / Duration.TotalSeconds / Format.nAvgBytesPerSec;
                            //statusBar.Text = string.Format("Track {0} read at {1:0.00} X", track, Speed);
                        }
                        else
                        {
                            //statusBar.Text = string.Format("There was an error readind track {0}", track);
                            mp3writer.Close();
                        }
                    }
                    finally
                    {
                        mp3writer.Close();
                        mp3writer = null;
                    }
                }
                finally
                {
                }
            }
            finally
            {
            }
        }

        // Write WMA compressed file
        public void WriteWMA(CDDrive drive, int track, string fname)
        {
            try
            {
                fname = System.IO.Path.ChangeExtension(fname, ".wma");

                System.Collections.ArrayList List = new System.Collections.ArrayList();

                //Check if the original file had a title
                string AttrValue = cd.Tracks[track].Title;
                if (AttrValue != null)
                {
                    //The title will be added to the destination file
                    WM_Attr Attr = new WM_Attr(WM.g_wszWMTitle,
                                               WMT_ATTR_DATATYPE.WMT_TYPE_STRING,
                                               string.Copy(AttrValue));
                    List.Add(Attr);
                }

                AttrValue = cd.Composer;
                if (AttrValue != null)
                {
                    //The title will be added to the destination file
                    WM_Attr Attr = new WM_Attr(WM.g_wszWMComposer,
                                               WMT_ATTR_DATATYPE.WMT_TYPE_STRING,
                                               string.Copy(AttrValue));
                    List.Add(Attr);
                }

                AttrValue = cd.Language;
                if (AttrValue != null)
                {
                    //The title will be added to the destination file
                    WM_Attr Attr = new WM_Attr(WM.g_wszWMLanguage,
                                               WMT_ATTR_DATATYPE.WMT_TYPE_STRING,
                                               string.Copy(AttrValue));
                    List.Add(Attr);
                }

                AttrValue = cd.YearRecorded.ToString();
                if (AttrValue != null)
                {
                    //The title will be added to the destination file
                    WM_Attr Attr = new WM_Attr(WM.g_wszWMYear,
                                               WMT_ATTR_DATATYPE.WMT_TYPE_STRING,
                                               string.Copy(AttrValue));
                    List.Add(Attr);
                }

                AttrValue = cd.Copyright;
                if (AttrValue != null)
                {
                    //The title will be added to the destination file
                    WM_Attr Attr = new WM_Attr(WM.g_wszWMCopyright,
                                               WMT_ATTR_DATATYPE.WMT_TYPE_STRING,
                                               string.Copy(AttrValue));
                    List.Add(Attr);
                }

                AttrValue = cd.Title;
                if (AttrValue != null)
                {
                    //The title will be added to the destination file
                    WM_Attr Attr = new WM_Attr(WM.g_wszWMAlbumTitle,
                                               WMT_ATTR_DATATYPE.WMT_TYPE_STRING,
                                               string.Copy(AttrValue));
                    List.Add(Attr);
                }

                AttrValue = cd.Category;
                if (AttrValue != null)
                {
                    //The title will be added to the destination file
                    WM_Attr Attr = new WM_Attr(WM.g_wszWMGenre,
                                               WMT_ATTR_DATATYPE.WMT_TYPE_STRING,
                                               string.Copy(AttrValue));
                    List.Add(Attr);
                }
                AttrValue = cd.Rating.ToString();
                if (AttrValue != null)
                {
                    //The title will be added to the destination file
                    WM_Attr Attr = new WM_Attr(WM.g_wszWMRating,
                                               WMT_ATTR_DATATYPE.WMT_TYPE_STRING,
                                               string.Copy(AttrValue));
                    List.Add(Attr);
                }

                //Check by the author metadata
                AttrValue = cd.Artist;
                if (AttrValue != null)
                { //The actor will be added to the destination file
                    WM_Attr Attr = new WM_Attr(WM.g_wszWMAuthor,
                                               WMT_ATTR_DATATYPE.WMT_TYPE_STRING,
                                               string.Copy(AttrValue));
                    List.Add(Attr);
                }

                //Check by the author metadata
                AttrValue = cd.Tracks[track].TrackNumber.ToString();
                if (AttrValue != null)
                { //The actor will be added to the destination file
                    WM_Attr Attr = new WM_Attr(WM.g_wszWMTrackNumber,
                                               WMT_ATTR_DATATYPE.WMT_TYPE_STRING,
                                               string.Copy(AttrValue));
                    List.Add(Attr);
                }

                string newdir = fname;
                newdir = newdir.Substring(0, newdir.LastIndexOf('\\'));

                Directory.CreateDirectory(newdir);

                // Set Input RIP Format
                Big3.Hitbase.MediaRipper.WaveFormat wavFormat = new Big3.Hitbase.MediaRipper.WaveFormat(44100, 16, 2);

                //WMA Version 8!!
                  
                IWMProfileManagerLanguage iwmLang;
                IWMProfile ppp;

                IWMProfile profile = null;
                Guid WMProfile_V80_128StereoAudio = new Guid("{407B9450-8BDC-4ee5-88B8-6F527BD941F2}");
                Guid WMATYPE = new Guid("{00000162-0000-0010-8000-00AA00389B71}");
                //This line gives the error
                WM.ProfileManager.LoadProfileByID(ref WMProfile_V80_128StereoAudio, out profile);
                uint c;
                profile.GetStreamCount(out c);
                IWMStreamConfig streamConf;
                IWMStreamConfig3 streamConf3;

                //WM.ProfileManager.LoadProfileByID(ref WMATYPE, out ppp);
                //profile.CreateNewStream(ref WMATYPE, out streamConf);

                profile.GetStream(0, out streamConf);
                profile.GetStreamCount(out c);
                streamConf.GetBitrate(out c);
                //streamConf.SetBitrate(160000);
                profile.ReconfigStream(streamConf);
                //streamConf.GetBitrate(out c);
                

                // WMA Version 9
                /*IWMProfile profile = null;
                WM.ProfileManager.CreateEmptyProfile(WMT_VERSION.WMT_VER_9_0, out profile);

                IWMCodecInfo codecInfo = (IWMCodecInfo)WM.ProfileManager;
                Guid mediaTypeGuid = WmMediaTypeId.Audio;
                IWMStreamConfig streamConfig;

                Guid mediaType = MediaTypes.WMMEDIATYPE_Audio;
                profile.CreateNewStream(ref mediaType, out streamConfig);

                //codecInfo.GetCodecFormat(ref mediaTypeGuid, (uint)1, (uint)0, out streamConfig);

                //streamConfig.SetStreamNumber(1);
                //streamConfig.SetStreamName("AudioStream1");
                //streamConfig.SetConnectionName("Audio1");

                uint c;
                streamConfig.GetBitrate(out c);
                streamConfig.SetBitrate(300000);

                uint pmsBufferWindow = 0;
                streamConfig.GetBufferWindow(out pmsBufferWindow);
                //streamConfig.set
                //profile.ReconfigStream(streamConfig);

                // Add the stream to the profile
                profile.AddStream(streamConfig);
                profile.ReconfigStream(streamConfig);
                */
                wmawriter = new WmaWriter(new FileStream(fname, FileMode.Create),
                                                     wavFormat,
                                                     profile,
                                                    (WM_Attr[])List.ToArray(typeof(WM_Attr)));

                try
                {
                    //writer = new WaveWriter(WaveFile, Format/*, m_Drive.TrackSize(track)*/);
                    try
                    {
                        //statusBar.Text = string.Format("Reading track {0}", track);
                        DateTime InitTime = DateTime.Now;
                        if (drive.ReadTrack(track + 1, new CdDataReadEventHandler(WriteWaveData), new CdReadProgressEventHandler(this.CdReadProgress)) > 0)
                        {
                            TimeSpan Duration = DateTime.Now - InitTime;
                            //double Speed = drive.TrackSize(1) / Duration.TotalSeconds / Format.nAvgBytesPerSec;
                            //statusBar.Text = string.Format("Track {0} read at {1:0.00} X", track, Speed);
                        }
                        else
                        {
                            //statusBar.Text = string.Format("There was an error readind track {0}", track);
                            wmawriter.Close();
                        }
                    }
                    finally
                    {
                        wmawriter.Close();
                        wmawriter = null;
                    }
                }
                finally
                {
                }
            }
            finally
            {
            }
        }

        public void WriteWaveData(object sender, DataReadEventArgs ea)
        {
            if (mp3writer != null)
            {
                mp3writer.Write(ea.Data, 0, (int)ea.DataSize);
            }
            if (wavwriter != null)
            {
                wavwriter.Write(ea.Data, 0, (int)ea.DataSize);
            }
            if (wmawriter != null)
            {
                wmawriter.Write(ea.Data, 0, (int)ea.DataSize);
            }
        }

        private void CdReadProgress(object sender, ReadProgressEventArgs ea)
        {
            ulong percent = ((ulong)ea.BytesRead * 100) / ea.Bytes2Read;

            CurrentTrackRecordingProgress = (double)percent / 100.0;

            //progressBar1.Value = (int)Percent;

            if (Canceled)
                ea.CancelRead = true;
            //if (formProgress.Canceled)
            //    ea.CancelRead = true;

            //formProgress.ProgressBar.Value = (int)Percent;

            //formProgress.LabelProgress.Text = System.IO.Path.ChangeExtension(currentRipname, "").TrimEnd('.');

            gesamtRipBytes = ea.BytesRead;

            DateTime Jetzt = DateTime.Now;
            TimeSpan RipTime = Jetzt - StartRipTime;
            //formProgress.labelSpeed.Text = Jetzt.Millisecond.ToString();
            //string formBytes;
            //formBytes = String.Format("{0:00000000000,00}", gesamtRipBytes);
            string formTime;
            int min;
            int sek;
            min = (int)RipTime.TotalSeconds / (int)60;
            sek = (int)RipTime.TotalSeconds - min * (int)60;
            formTime = String.Format("{0:00}:{1:00}", min, sek);

            //formProgress.labelSpeed.Text = "Zeit: " + formTime;
            //Console.WriteLine("Call Time(MS): " + CallTime.Milliseconds.ToString());

            //formProgress.ProgressBarAll.Value = Convert.ToInt32((int)currentRipTrack * 100 + formProgress.ProgressBar.Value);
            // Windows 7
            if (Misc.IsWindows7OrLater())
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(
                 new Action(delegate
                        {
                            // Jetzt haben wir Windows 7
                            System.Windows.Application.Current.MainWindow.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                            System.Windows.Application.Current.MainWindow.TaskbarItemInfo.ProgressValue = 1.0 / ((double)this.numRipTracks * 100.0) * ((double)currentRipTrack * 100 + (double)percent);
                        }
                ));
            }

            //!!!!!!!! TODO ea.CancelRead |= this.m_CancelRipping;
        }

        /// <summary>
        /// Build Rip filename - 
        /// </summary>
        /// <param name="track">Track number</param>
        /// <param name="checkonly">true: Return missing CD or track fields in build file name</param>
        /// <returns></returns>
        private string BuildFileName(int track, bool checkonly = false)
        {
            string form = Settings.Current.RecordFormatFile;
            string fieldcontent;
            string fieldcontentNew;
            string fieldName;
            string missingfields = "";
            FieldCollection fields = FieldHelper.GetAllCDFields(false);
            FieldCollection track_fields = FieldHelper.GetAllTrackFields(false);

            foreach (Field field in fields)
            {
                try
                {
                    fieldName = dataBase.GetNameOfField(field);
                    if (form.IndexOf("[" + fieldName + "]") >= 0)
                    {
                        fieldcontent = this.cd.GetStringByFieldEx(dataBase, field);
                        if (string.IsNullOrEmpty(fieldcontent))
                        {
                            if (missingfields.Length == 0)
                                missingfields = "CD:" + dataBase.GetNameOfField(field);
                            else
                                missingfields = missingfields + ";CD:" + dataBase.GetNameOfField(field);
                        }
                        else
                        {
                            // Hier unbedingt Backslashes aus Feldinhalten entfernen bzw. ersetzen
                            fieldcontent = fieldcontent.Replace("\\", Settings.Current.RecordFileNameCharBackslash);

                            form = form.Replace("[" + fieldName + "]", fieldcontent);
                        }
                    }
                }
                catch
                {
                }
            }

            foreach (Field field in track_fields)
            {
                try
                {
                    fieldName = dataBase.GetNameOfField(field);

                    if (form.IndexOf("[Track - " + fieldName + "]") >= 0)
                    {
                        fieldcontent = cd.GetTrackStringByFieldEx(dataBase, track, field);
                        if (string.IsNullOrEmpty(fieldcontent))
                        {
                            if (missingfields.Length == 0)
                                missingfields = "Track:" + dataBase.GetNameOfField(field);
                            else
                                missingfields = missingfields + ";Track:" + dataBase.GetNameOfField(field);
                        }
                        else
                        {
                            // Hier unbedingt Backslashes aus Feldinhalten entfernen bzw. ersetzen
                            fieldcontent = fieldcontent.Replace("\\", Settings.Current.RecordFileNameCharBackslash);

                            // Sonderbehandlung für Nr. Feld
                            if (fieldName == "Nr.")
                            {
                                //if (CD.NumberOfTracks > 9)
                                // Immer 2 Stellen auch wenn unter 10 Tracks
                                fieldcontentNew = string.Format("{0:00}", Convert.ToInt32(fieldcontent));

                                if (cd.NumberOfTracks > 99)
                                    fieldcontentNew = string.Format("{0:000}", Convert.ToInt32(fieldcontent));

                                if (cd.NumberOfTracks > 999)
                                    fieldcontentNew = string.Format("{0:0000}", Convert.ToInt32(fieldcontent));

                                form = form.Replace("[Track - " + fieldName + "]", fieldcontentNew);
                            }
                            else
                            {
                                form = form.Replace("[Track - " + fieldName + "]", fieldcontent);
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            // Jetzt müssen noch eventuell nicht erlaubte Zeichen entfernt werden
            // Der Backslash hier nicht, nur im Feldnamen, siehe oben
            form = form.Replace("\"", Settings.Current.RecordFileNameCharAnfuehrung);
            //form = form.Replace("\\", Settings.Current.RecordFileNameCharBackslash);
            form = form.Replace(":", Settings.Current.RecordFileNameCharDoppelpunkt);
            form = form.Replace("?", Settings.Current.RecordFileNameCharFragezeichen);
            form = form.Replace(">", Settings.Current.RecordFileNameCharGroesser);
            form = form.Replace("<", Settings.Current.RecordFileNameCharKleiner);
            form = form.Replace("|", Settings.Current.RecordFileNameCharPipe);
            form = form.Replace("/", Settings.Current.RecordFileNameCharSlash);
            form = form.Replace("*", Settings.Current.RecordFileNameCharStern);
            if (Settings.Current.RecordFileNameCharUserOrg1.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg1, Settings.Current.RecordFileNameCharUserNew1);
            if (Settings.Current.RecordFileNameCharUserOrg2.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg2, Settings.Current.RecordFileNameCharUserNew2);
            if (Settings.Current.RecordFileNameCharUserOrg3.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg3, Settings.Current.RecordFileNameCharUserNew3);
            if (Settings.Current.RecordFileNameCharUserOrg4.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg4, Settings.Current.RecordFileNameCharUserNew4);
            if (Settings.Current.RecordFileNameCharUserOrg5.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg5, Settings.Current.RecordFileNameCharUserNew5);
            if (Settings.Current.RecordFileNameCharUserOrg6.Length > 0)
                form = form.Replace(Settings.Current.RecordFileNameCharUserOrg6, Settings.Current.RecordFileNameCharUserNew6);

            // return missing fields string for RIP
            if (checkonly)
                return missingfields;
            else
                return form;
        }

        internal enum WmVersion
        {
            Version4 = 0x00040000,
            Version7 = 0x00070000,
            Version8 = 0x00080000,
            Version9 = 0x00090000
        }
    }

}
