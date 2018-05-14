using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Management;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;
using System.Diagnostics;
using System.Xml.Serialization;
using Big3.Hitbase.MediaRipper.Mp3;
using Big3.Hitbase.MediaRipper.WMFSdk;
using Big3.Hitbase.Controls;

//using System.Directory

namespace Big3.Hitbase.RecordMedium
{
    public partial class RecordMedium : Form
    {
        private DataBase DataBase;
        private CD CD;
        //Big3.Hitbase.RecordMedium.RecordFormProgress formProgress;
        String CurrentDriveLetter;
        int CurrentDriveIndex;
        IntPtr mainWindow;
        private Mp3WriterConfig mp3Config = null;
        private WmaWriterConfig wmaConfig = null;

        public bool StartRecord { get; set; }

        public string TrackList
        {
            set
            {
                // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //textSomeTracks.Text = value;
            }
        }

        public RecordMedium(CD cd, DataBase database, char DriveLetter, IntPtr mainWindow)
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);

            textFileDir.Text = Settings.Current.RecordSelectedPath;
            LoadMP3ConfigFromRegistry();
            if (string.IsNullOrEmpty(textFileDir.Text))
                textFileDir.Text = Misc.GetPersonalHitbaseMusicFolder();

            //comboDrives = allDrives;
            textRecordFormatFile.Text = Settings.Current.RecordFormatFile;
            CurrentDriveLetter = DriveLetter.ToString();
            this.CD = cd;
            this.mainWindow = mainWindow;
            this.DataBase = database;
//            if (Settings.Current.RecordMethod == 0)
  //              radioRecordDrive.Checked = true;
    //        if (Settings.Current.RecordMethod == 1)
      //          radioRecordSound.Checked = true;
            //if (Settings.Current.RecordWriteID3Tags == 1)
            //    checkWriteID3Tags.Checked = true;
            //checkAutoEject.Checked = Settings.Current.RecordAutoEject;

            checkFiles();

            //listViewRecordFormat.SelectedIndices.Add(Settings.Current.RecordLastSelectedFormat);
            //listViewRecordFormat.Items[].Selected = true;

            // Standard immer Full CD
            
            // Noch ermitteln
            // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //radioFullCD.Checked = true;

            //if (Settings.Current.RecordCopyRegion == 1)
            //    radioSomeTracks.Checked = true;
            //if (Settings.Current.RecordCopyRegion == 2)
            //    radioAB.Checked = true;
 //           if (Settings.Current.RecordStartPosition == 0)
   //             radioTrackMiddle.Checked = true;
     //       if (Settings.Current.RecordStartPosition == 1)
       //         radioTrackStartTime.Checked = true;
         //   if (Settings.Current.RecordLengthPosition == 0)
    //            radioCompleteTrack.Checked = true;
      //      if (Settings.Current.RecordLengthPosition == 1)
        //        radioTrackPart.Checked = true;
//            if (Settings.Current.RecordFormat == 0)
//                radioWavFormat.Checked = true;
//            if (Settings.Current.RecordFormat == 1)
  //              radioWMAFormat.Checked = true;
    //        if (Settings.Current.RecordFormat == 2)
      //          radioACMFormat.Checked = true;
//            if (Settings.Current.RecordFormat == 3)
  //              radioMP3Format.Checked = true;

            listTemplates.Items.AddRange(Settings.Current.RecordFilenameFormat);

          /*  // Get a list of all capture devices
            CaptureDevicesCollection captureDevice = new CaptureDevicesCollection();
            
            foreach (DeviceInformation devInfo in captureDevice)
            {
                RecordCaptureDevice rcd = new RecordCaptureDevice();
                rcd.devGuid = devInfo.DriverGuid;
                rcd.Description = devInfo.Description;

                comboCaptureDevice.Items.Add(rcd);
            }
           * */
            //Settings.Current.RecordAnalogFormat
        }


        private void checkFiles()
        {
            /*
            string basedir = AppDomain.CurrentDomain.BaseDirectory;

            if (!File.Exists(basedir + "\\lame_enc.dll"))
            {
                listViewRecordFormat.Items[0].ToolTipText = "lame_enc.dll nach '" + basedir + "' kopieren";
                listViewRecordFormat.Items[0].Text = "lame_enc.dll nach '" + basedir + "' kopieren";
                listViewRecordFormat.Items[0].ImageIndex = 6;
                
            }
            else
            {
                listViewRecordFormat.Items[0].ToolTipText = "MP3 - Komprimierung mit lame_enc.dll";
                listViewRecordFormat.Items[0].Text = "MP3 - Komprimierung mit lame_enc.dll";
                listViewRecordFormat.Items[0].ImageIndex = 1;
               
            }
            if (!File.Exists(basedir + "\\flac.exe"))
            {
                listViewRecordFormat.Items[1].ToolTipText = "flac.exe nach '" + basedir + "' kopieren";
                listViewRecordFormat.Items[1].Text = "flac.exe nach '" + basedir + "' kopieren";
                listViewRecordFormat.Items[1].ImageIndex = 5;
            }
            else
            {
                listViewRecordFormat.Items[1].ToolTipText = "Flac - verlustfreie Komprimierung mit flac.exe";
                listViewRecordFormat.Items[1].Text = "Flac - verlustfreie Komprimierung mit flac.exe";
                listViewRecordFormat.Items[1].ImageIndex = 0;
                
            }
            if (!File.Exists(basedir + "\\oggenc.exe"))
            {
                listViewRecordFormat.Items[2].ToolTipText = "oggenc.exe nach '" + basedir + "' kopieren";
                listViewRecordFormat.Items[2].Text = "oggenc.exe nach '" + basedir + "' kopieren";
                listViewRecordFormat.Items[2].ImageIndex = 7;
            }
            else
            {
                listViewRecordFormat.Items[2].ToolTipText = "OGG - Komprimierung mit oggenc.exe";
                listViewRecordFormat.Items[2].Text = "OGG - Komprimierung mit oggenc.exe";
                listViewRecordFormat.Items[2].ImageIndex = 2;
               
            }

            if (!File.Exists(basedir + "\\lame.exe"))
            {
                listViewRecordFormat.Items[3].ToolTipText = "lame.exe nach '" + basedir + "' kopieren";
                listViewRecordFormat.Items[3].Text = "lame.exe nach '" + basedir + "' kopieren";
                listViewRecordFormat.Items[3].ImageIndex = 6;
            }
            else
            {
                listViewRecordFormat.Items[3].ToolTipText = "MP3 - Komprimierung mit lame.exe";
                listViewRecordFormat.Items[3].Text = "MP3 - Komprimierung mit lame.exe";
                listViewRecordFormat.Items[3].ImageIndex = 1;
                
            }
            */
        }

        private void buttonMP3Format_Click(object sender, EventArgs e)
        {
            // MP3 DLL
            /*
            if (listViewRecordFormat.SelectedIndices[0] == 0)
            {
                MP3Settings mp3set = new MP3Settings();
                if (mp3Config != null)
                    mp3set.MP3Config = mp3Config;
                mp3set.ShowDialog(this);

                mp3Config = (Mp3WriterConfig)mp3set.editMp3Writer1.Config;

            }
            // Flac Exe
            if (listViewRecordFormat.SelectedIndices[0] == 1)
            {
                FlacSettings FlacExeSet = new FlacSettings();
                FlacExeSet.ShowDialog(this);
            }
            // Ogg Exe
            if (listViewRecordFormat.SelectedIndices[0] == 2)
            {
                OggVorbisSettings LameExeSet = new OggVorbisSettings();
                LameExeSet.ShowDialog(this);
            }
            // LAME Exe
            if (listViewRecordFormat.SelectedIndices[0] == 3)
            {
                LameExeSettings LameExeSet = new LameExeSettings();
                LameExeSet.ShowDialog(this);
            }
            // WMA Settings
            if (listViewRecordFormat.SelectedIndices[0] == 5)
            {
                //WMASettings wmaset = new WMASettings();

                //wmaset.ShowDialog(this);
                //TODO
                //wmaConfig = (WmaWriterConfig)
            }
            if (listViewRecordFormat.SelectedIndices[0] == 6)
            {
                ExternalEncoder ExternalEncoder = new ExternalEncoder(6);
                ExternalEncoder.Text = StringTable.ExternalProgram1;
                ExternalEncoder.ShowDialog(this);
            }
            if (listViewRecordFormat.SelectedIndices[0] == 7)
            {
                ExternalEncoder ExternalEncoder = new ExternalEncoder(7);
                ExternalEncoder.Text = StringTable.ExternalProgram2;
                ExternalEncoder.ShowDialog(this);
            }
             * */
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //Settings.Current.RecordMediumLastDriveLetter = ((string)comboDrives.SelectedItem).Substring(0, 2);
            
//            if (comboAnalogFormat.SelectedIndex >= 0)
  //              Settings.Current.RecordAnalogFormat = (string)comboAnalogFormat.SelectedItem.ToString();

            Settings.Current.RecordSelectedPath = textFileDir.Text;

            Settings.Current.RecordFormatFile = textRecordFormatFile.Text;
            //Settings.Current.RecordWriteID3Tags = Convert.ToInt32(checkWriteID3Tags.Checked);
            //Settings.Current.RecordAutoEject = checkAutoEject.Checked;
            //Settings.Current.RecordLastSelectedFormat = listViewRecordFormat.SelectedIndices[0];

//            if (radioRecordDrive.Checked)
  //              Settings.Current.RecordMethod = 0;
    //        if (radioRecordSound.Checked)
      //          Settings.Current.RecordMethod = 1;
            // TODO!!!!!!!!!!!!!!!!!!!!!!
            //if (radioFullCD.Checked)
            //    Settings.Current.RecordCopyRegion = 0;
            //if (radioSomeTracks.Checked)
            //    Settings.Current.RecordCopyRegion = 1;
  //          if (radioAB.Checked)
    //            Settings.Current.RecordCopyRegion = 2;
      //      if (radioTrackMiddle.Checked)
        //        Settings.Current.RecordStartPosition = 0;
   //         if (radioTrackStartTime.Checked)
     //           Settings.Current.RecordStartPosition = 1;
       //     if (radioCompleteTrack.Checked)
         //       Settings.Current.RecordLengthPosition = 0;
  //          if (radioTrackPart.Checked)
    //            Settings.Current.RecordLengthPosition = 1;
//            if (radioWavFormat.Checked)
  //              Settings.Current.RecordFormat = 0;
//            if (radioWMAFormat.Checked)
  //              Settings.Current.RecordFormat = 1;
    //        if (radioACMFormat.Checked)
      //          Settings.Current.RecordFormat = 2;
    //        if (radioMP3Format.Checked)
      //          Settings.Current.RecordFormat = 3;
            SaveMP3ConfigToRegistry();
            Settings.Current.Save();
            //Settings.Current
            //if (mp3Config.Mp3Config.format.lhv1.dwBitrate < 1000)
            //mp3Config.Mp3Config.format.lhv1.dwBitrate = mp3Config.Mp3Config.format.lhv1.dwBitrate * 1000;

            // VOrher mal die Einstellungen korrigieren
            if (mp3Config != null && mp3Config.Mp3Config != null)
            {
                if (mp3Config.Mp3Config.format.lhv1.dwMaxBitrate < 1000)
                    mp3Config.Mp3Config.format.lhv1.dwMaxBitrate = mp3Config.Mp3Config.format.lhv1.dwMaxBitrate * 1000;
                if (mp3Config.Mp3Config.format.lhv1.dwVbrAbr_bps < 1000)
                    mp3Config.Mp3Config.format.lhv1.dwVbrAbr_bps = mp3Config.Mp3Config.format.lhv1.dwVbrAbr_bps * 1000;
            }

            this.Hide();
            //RecordNow();
            Close();
        }

        public void LoadMP3ConfigFromRegistry()
        {
            try
            {
                object mp3Config = Settings.GetValue("RecordMp3WriterConfig", null);
                if (mp3Config == null)
                    return;
                XmlSerializer bf = new XmlSerializer(typeof(Mp3WriterConfig));
                StringReader sr = new StringReader(mp3Config.ToString());

                this.mp3Config = (Mp3WriterConfig)bf.Deserialize(sr);
                if (this.mp3Config == null)
                    return;

                this.mp3Config.Mp3Config.format.lhv1.dwSampleRate = 44100;
                this.mp3Config.Mp3Config.format.lhv1.dwStructSize = 332;
                this.mp3Config.Mp3Config.format.mp3.bPrivate = 44100;

                this.mp3Config.Mp3Config.format.lhv1.bWriteVBRHeader = 1;
            }
            catch (Exception e)
            {
                // Wenn das Laden der Parameter fehlschlägt, ist das nicht so schlimm.
                FormUnhandledException formUnhandledException = new FormUnhandledException(e);
                formUnhandledException.ShowDialog();
            }
        }

        public void SaveMP3ConfigToRegistry()
        {
            XmlSerializer bf = new XmlSerializer(typeof(Mp3WriterConfig));
            StringWriter sw = new StringWriter();
            bf.Serialize(sw, mp3Config);

            Settings.SetValue("RecordMp3WriterConfig", sw.ToString());
        }


        private void RecordMedium_Load(object sender, EventArgs e)
        {
            /*
            comboDrives.BeginUpdate();
            
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_CDROMDrive");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    string CdromDrive;
                    CdromDrive = string.Format("{0} - {1}", queryObj["Drive"], queryObj["Name"]);
                    int index = comboDrives.Items.Add(CdromDrive);

                    if ((string)queryObj["Drive"] == CurrentDriveLetter + ":")
                    {
                        CurrentDriveIndex = index;
                        comboDrives.SelectedIndex = index;
                    }

                    //string h = Directory.GetDirectoryRoot("c:\\");                    
                }
            }
            catch (ManagementException evt)
            {
                MessageBox.Show("An error occurred while querying for WMI data: " + evt.Message);
            }
            comboDrives.EndUpdate();
            if (comboDrives.Items.Count > 1 && comboDrives.SelectedIndex < 0)
                comboDrives.SelectedIndex = 0;
            */
            if (StartRecord)
                buttonOK.PerformClick();
        }

        private void buttonErweitert_Click(object sender, EventArgs e)
        {
        }

        private void buttonSelectFileDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selectdir = new FolderBrowserDialog();
            selectdir.Description = "Bitte wählen Sie ein Verzeichnis zur Speicherung der Aufnahme.";
            if (selectdir.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            { 
                textFileDir.Text = selectdir.SelectedPath.ToString();
                Settings.Current.RecordSelectedPath = selectdir.SelectedPath.ToString();
            }
        }

        private void comboCaptureDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            RecordCaptureDevice rcd = (RecordCaptureDevice)comboCaptureDevice.SelectedItem;
            RecordAnalogFormat raf = new RecordAnalogFormat();
            Capture dsCapture = new Capture(rcd.devGuid);
            comboAnalogFormat.Items.Clear();
            
            
            
            if (dsCapture.Caps.Format11KhzMono8Bit == true)
            {
                raf.Herz = 11000;
                raf.Bits8_16 = 8;
                raf.Channels1_2 = 1;
                raf.Description = "11 Khz Mono 8Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format11KhzMono16Bit == true)
            {
                raf.Herz = 11000;
                raf.Bits8_16 = 16;
                raf.Channels1_2 = 1;
                raf.Description = "11 Khz Mono 16Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format11KhzStereo8Bit == true)
            {
                raf.Herz = 11000;
                raf.Bits8_16 = 8;
                raf.Channels1_2 = 2;
                raf.Description = "11 Khz Stereo 8Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format11KhzStereo16Bit == true)
            {
                raf.Herz = 11000;
                raf.Bits8_16 = 16;
                raf.Channels1_2 = 2;
                raf.Description = "11 Khz Stereo 16Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format22KhzMono8Bit == true)
            {
                raf.Herz = 22000;
                raf.Bits8_16 = 8;
                raf.Channels1_2 = 1;
                raf.Description = "22 Khz Mono 8Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format22KhzMono16Bit == true)
            {
                raf.Herz = 22000;
                raf.Bits8_16 = 16;
                raf.Channels1_2 = 1;
                raf.Description = "22 Khz Mono 16Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format22KhzStereo8Bit == true)
            {
                raf.Herz = 22000;
                raf.Bits8_16 = 8;
                raf.Channels1_2 = 2;
                raf.Description = "22 Khz Stereo 8Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format22KhzStereo16Bit == true)
            {
                raf.Herz = 22000;
                raf.Bits8_16 = 16;
                raf.Channels1_2 = 2;
                raf.Description = "22 Khz Stereo 16Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format44KhzMono8Bit == true)
            {
                raf.Herz = 44000;
                raf.Bits8_16 = 8;
                raf.Channels1_2 = 1;
                raf.Description = "44 Khz Mono 8Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format44KhzMono16Bit == true)
            {
                raf.Herz = 44000;
                raf.Bits8_16 = 16;
                raf.Channels1_2 = 1;
                raf.Description = "44 Khz Mono 16Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format44KhzStereo8Bit == true)
            {
                raf.Herz = 44000;
                raf.Bits8_16 = 8;
                raf.Channels1_2 = 2;
                raf.Description = "44 Khz Stereo 8Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format44KhzStereo16Bit == true)
            {
                raf.Herz = 44000;
                raf.Bits8_16 = 16;
                raf.Channels1_2 = 2;
                raf.Description = "44 Khz Stereo 16Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format48KhzMono8Bit == true)
            {
                raf.Herz = 48000;
                raf.Bits8_16 = 8;
                raf.Channels1_2 = 1;
                raf.Description = "48 Khz Mono 8Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format48KhzMono16Bit == true)
            {
                raf.Herz = 48000;
                raf.Bits8_16 = 16;
                raf.Channels1_2 = 1;
                raf.Description = "48 Khz Mono 16Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format48KhzStereo8Bit == true)
            {
                raf.Herz = 48000;
                raf.Bits8_16 = 8;
                raf.Channels1_2 = 2;
                raf.Description = "48 Khz Stereo 8Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format48KhzStereo16Bit == true)
            {
                raf.Herz = 48000;
                raf.Bits8_16 = 16;
                raf.Channels1_2 = 2;
                raf.Description = "48 Khz Stereo 16Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format96KhzMono8Bit == true)
            {
                raf.Herz = 96000;
                raf.Bits8_16 = 8;
                raf.Channels1_2 = 1;
                raf.Description = "96 Khz Mono 8Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format96KhzMono16Bit == true)
            {
                raf.Herz = 96000;
                raf.Bits8_16 = 16;
                raf.Channels1_2 = 1;
                raf.Description = "96 Khz Mono 16Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format96KhzStereo8Bit == true)
            {
                raf.Herz = 96000;
                raf.Bits8_16 = 8;
                raf.Channels1_2 = 2;
                raf.Description = "96 Khz Stereo 8Bit";
                comboAnalogFormat.Items.Add(raf);
            }
            if (dsCapture.Caps.Format96KhzStereo16Bit == true)
            {
                raf.Herz = 96000;
                raf.Bits8_16 = 16;
                raf.Channels1_2 = 2;
                raf.Description = "96 Khz Stereo 16Bit";
                comboAnalogFormat.Items.Add(raf);
            }
             * */
        }

        private void buttonCustomFileFormat_Click(object sender, EventArgs e)
        {
            RecordFreeNameFormat FreeName = new RecordFreeNameFormat(CD, DataBase, this);
            FreeName.ShowDialog();
            textRecordFormatFile.Text = FreeName.SelectedNameFormat;
            listTemplates.Items.Clear();
            listTemplates.Items.AddRange(Settings.Current.RecordFilenameFormat);
            textRecordFormatFile.Text = Settings.Current.RecordFormatFile;
        }

        private void listTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            textRecordFormatFile.Text = listTemplates.Text;
        }

        private void linkLabelMP3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*
            //linkLabelMP3.Links.Remove(linkLabelMP3.Links(0));
            //linkLabelMP3.Links.Add(0, linkLabelMP3.Text.Length, "http://www.google.de");
            // MP3 DLL
            if (listViewRecordFormat.SelectedIndices[0] == 0)
            {
                Process.Start("http://www.bing.com/search?q=lame_enc.dll+3.98.2&go=&form=QBRE&filt=all");
            }
            if (listViewRecordFormat.SelectedIndices[0] == 1)
            {
                Process.Start("http://www.bing.com/search?q=flac.exe&go=&form=QBRE&filt=all");
            }
            if (listViewRecordFormat.SelectedIndices[0] == 2)
            {
                Process.Start("http://www.bing.com/search?q=oggenc.exe&go=&form=QBRE&filt=all");
            }
            if (listViewRecordFormat.SelectedIndices[0] == 3)
            {
                Process.Start("http://www.bing.com/search?q=lame.exe+3.98.2&go=&form=QBRE&filt=all");
            }
            */
        }

        private void textRecordFormatFile_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Charmap charmap = new Charmap();
            charmap.ShowDialog();
        }

        // TODO!!!!!!!!!!!!!!!!!!
        // Löschen
        private void buttonSelectTracks_Click_1(object sender, EventArgs e)
        {
            RecordSelectTracks SelectedTracks = new RecordSelectTracks(CD);
            SelectedTracks.ShowDialog();
            //textSomeTracks.Text = SelectedTracks.SelectedTracks;
        }

        private void listViewRecordFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkFiles();

            
            
            /*
             * if (listViewRecordFormat.SelectedItems.Count <= 0)
                return;

            //Settings.Current.RecordLastSelectedFormat = listViewRecordFormat.SelectedIndices[0];

            //buttonMP3Format.Enabled = false;

            if (listViewRecordFormat.SelectedIndices[0] == 0)
            {
                //linkLabelMP3.Text = "lame_enc.dll im Internet suchen...";
                //buttonMP3Format.Enabled = true;
            }
            if (listViewRecordFormat.SelectedIndices[0] == 1)
            {
               // linkLabelMP3.Text = "flac.exe im Internet suchen...";
                //buttonMP3Format.Enabled = true;
            }
            if (listViewRecordFormat.SelectedIndices[0] == 2)
            {
                linkLabelMP3.Text = "oggenc.exe im Internet suchen...";
                buttonMP3Format.Enabled = true;
            }
            if (listViewRecordFormat.SelectedIndices[0] == 3)
            {
                linkLabelMP3.Text = "lame.exe im Internet suchen...";
                buttonMP3Format.Enabled = true;
            }
            if (listViewRecordFormat.SelectedIndices[0] == 6)
            {
                buttonMP3Format.Enabled = true;            
            }
            if (listViewRecordFormat.SelectedIndices[0] == 7)
            {
                buttonMP3Format.Enabled = true;
            }
             */
        }

        // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // Löschen?
        private void textSomeTracks_Enter(object sender, EventArgs e)
        {
            //radioSomeTracks.Checked = true;
        }

        private void textSomeTracks_TextChanged(object sender, EventArgs e)
        {
            //radioSomeTracks.Checked = true;
        }

        private void comboDrives_SelectedIndexChanged(object sender, EventArgs e)
        {
            //comboDrives.SelectedIndex = CurrentDriveIndex;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string hitbasedir;
            hitbasedir = "file://" + AppDomain.CurrentDomain.BaseDirectory;

            Process.Start(hitbasedir);
        }
    }
    // list of capture devices for combobox
    public class RecordCaptureDevice
    {
        public Guid devGuid;
        public string Description;
        /// <summary>
        /// Anzeige in Combobox
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description;
        }
    }

    // List of possible record formats
    public class RecordAnalogFormat
    {
        public int Channels1_2;
        public int Bits8_16;
        public int Herz;
        public string Description;
        /// <summary>
        /// Anzeige in Combobox
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
