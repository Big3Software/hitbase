using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Threading;

namespace Big3.Hitbase.SoundEngine
{
    public class CDEngine
    {
        public event EventHandler CDInserted;

        public event EventHandler CDEjected;

        public bool IsCDInDrive { get; set; }

        public string Title { get; set; }

        DispatcherTimer dt = new DispatcherTimer();

        public CDEngine(char driveLetter)
        {
            DriveLetter = driveLetter;

            IsCDInDrive = CheckCDInDrive(driveLetter);

            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            bool oldCDIsInDrive = IsCDInDrive;

            bool currentCDIsInDrive = CheckCDInDrive(DriveLetter);

            if (!oldCDIsInDrive && currentCDIsInDrive)
            {
                Console.WriteLine("CD has been inserted");

                IsCDInDrive = true;

                if (CDInserted != null)
                    CDInserted(this, new EventArgs());
            }

            if (oldCDIsInDrive && !currentCDIsInDrive)
            {
                Console.WriteLine("CD has been ejected");

                IsCDInDrive = false;

                if (CDEjected != null)
                    CDEjected(this, new EventArgs());
            }
        }

        private bool CheckCDInDrive(char driveLetter)
        {
            DriveInfo di = new DriveInfo(driveLetter.ToString());
            return di.IsReady;
        }

        public void Close()
        {
            dt.Stop();
        }

        /// <summary>
        /// Diese Event wird ausgelöst, wenn eine CD eingelegt wurde.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CDREventArrived(object sender, EventArrivedEventArgs e)
        {
            // Get the Event object and display it
            PropertyData pd = e.NewEvent.Properties["TargetInstance"];

            if (pd != null)
            {
                ManagementBaseObject mbo = pd.Value as ManagementBaseObject;

                foreach (PropertyData pd2 in mbo.Properties)
                {
                    if (pd2.Value != null)
                        System.Diagnostics.Debug.WriteLine(pd2.Name + ": " + pd2.Value.ToString());
                    else
                        System.Diagnostics.Debug.WriteLine(pd2.Name + ": <null>");

                }

                // if CD removed VolumeName == null
                if (mbo.Properties["VolumeName"].Value != null)
                {
                    Console.WriteLine("CD has been inserted");
                    
                    IsCDInDrive = true;

                    if (CDInserted != null)
                        CDInserted(this, new EventArgs());
                }
                else
                {
                    Console.WriteLine("CD has been ejected");

                    IsCDInDrive = false;

                    if (CDEjected != null)
                        CDEjected(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Der Laufwerksbuchstabe des CD-Laufwerkes
        /// </summary>
        public char DriveLetter { get; set; }

        // Gibt wohl im Moment leider keine andere Möglichkeit.
        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        static void EjectMedia(char driveLetter)
        {
            int ret = 0;

            ret = mciSendString("open " + driveLetter + ": type CDAudio alias myDrive", null, 0, IntPtr.Zero);
            ret = mciSendString("set myDrive door open", null, 0, IntPtr.Zero);
            ret = mciSendString("close myDrive", null, 0, IntPtr.Zero);
        }

        static void InsertMedia(char driveLetter)
        {
            int ret = 0;

            ret = mciSendString("open " + driveLetter + ": type CDAudio alias myDrive", null, 0, IntPtr.Zero);
            ret = mciSendString("set myDrive door closed", null, 0, IntPtr.Zero);
            ret = mciSendString("close myDrive", null, 0, IntPtr.Zero);
        }

        public void OpenTray()
        {
            EjectMedia(DriveLetter);
        }

        public void CloseTray()
        {
            InsertMedia(DriveLetter);
        }
    }

    public class CDInfo
    {
        public CDInfo()
        {
            Tracks = new List<CDInfoTrack>();
        }

        public string Identity { get; set; }
        public int TotalLength { get; set; }
        public List<CDInfoTrack> Tracks { get; set; }

        public bool IsPureDataCD()
        {
            bool isPureDataCD = Tracks.All(x => x.TrackType != TrackType.Audio);

            return isPureDataCD;
        }
    }

    public class CDInfoTrack
    {
        public int StartTime { get; set; }
        public int Length { get; set; }
        public TrackType TrackType { get; set; }

    }

    public enum TrackType
    {
        Unknown,
        Audio,
        Data
    }
}
