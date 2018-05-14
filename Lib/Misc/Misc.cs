using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Big3.Hitbase.Configuration;
using System.Windows.Media.Imaging;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.Miscellaneous
{
    public class Misc
    {
        /// <summary>
        /// Liefert die kurze Zeit-Angabe ("MM:SS") der angegebenen Millisekunden zurück.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static string GetShortTimeString(int ms)
        {
        	if (ms < 0)
		        ms = 0;
	
            if (ms/60000 > 59 && Settings.Current.TimeFormat == TimeFormat.HHMMSS)
                return string.Format("{0}:{1:D2}:{2:D2}", ms / 3600000, ((ms % 3600000) / 60000) % 100, ms % 60000 / 1000);
            else
                return string.Format("{0:D2}:{1:D2}", ms / 60000, ms % 60000 / 1000);
        }

        public static string GetShortTimeString(long ms)
        {
            if (ms < 0)
                ms = 0;

            if (ms / 60000L > 59 && Settings.Current.TimeFormat == TimeFormat.HHMMSS)
                return string.Format("{0}:{1:D2}:{2:D2}", ms / 3600000L, ((ms % 3600000L) / 60000L) % 100L, ms % 60000L / 1000L);
            else
                return string.Format("{0:D2}:{1:D2}", ms / 60000L, ms % 60000L / 1000L);
        }

        public static string GetLongTimeString(int ms)
        {
            if (ms < 0)
                ms = 0;

            string str;
            if (Settings.Current.TimeFormat == TimeFormat.HHMMSS)
            {
                if (ms / 3600000 >= 100)
                    str = string.Format(">{0:D2}:{1:D2}:{2:D2}", (ms / 3600000) % 100, ((ms % 3600000) / 60000) % 100, ms % 60000 / 1000);
                else
                    str = string.Format("{0:D2}:{1:D2}:{2:D2}", (ms / 3600000) % 100, ((ms % 3600000) / 60000) % 100, ms % 60000 / 1000);
            }
            else
            {
                if (ms / 60000 >= 100)
                    str = string.Format(">{0:D2}:{1:D2}", ms / 60000, ms % 60000 / 1000);
                else
                    str = string.Format("{0:D2}:{1:D2}", ms / 60000, ms % 60000 / 1000);
            }

            return str;
        }

        /// <summary>
        /// Wandelt einen Zeit-String (HH:MM:SS oder MM:SS) in eine Zahl um (Anzahl Millisekunden)
        /// </summary>
        /// <param name="timeString"></param>
        /// <returns></returns>
        public static int ParseTimeString(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return 0;

            string[] parts = timeString.Split(':');
            int milliSeconds = 0;
            if (parts.Length == 3)
            {
                milliSeconds = Misc.Atoi(parts[0]) * 3600000 + Misc.Atoi(parts[1]) * 60000 + Misc.Atoi(parts[2]) * 1000;
            }
            if (parts.Length == 2)
            {
                milliSeconds = Misc.Atoi(parts[0]) * 60000 + Misc.Atoi(parts[1]) * 1000;
            }
            
            return milliSeconds;
        }

        /* Wer braucht das hier? Erst mal auskommentieren, mal schauen, wo es knallt...
         * public static string GetLongTimeString(double ms)
        {
            if (ms < 0)
                ms = 0;

            return string.Format("{0:F0}:{1:F0}", ms / 60000, ms % 60000 / 1000);
        }*/

        /// <summary>
        /// Das Datum wird geparsed. Erlaubt sind Längen von 4, 7 und 10
        /// "2007" => "20070000"
        /// "01.2008" => "20080100"
        /// "19.02.2008" => "20080219"
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ParseDate(string text)
        {
            if (text.Length == 4)
                return string.Format("{0}0000", text);
            if (text.Length == 7)
                return string.Format("{0}{1}00", text.Substring(3, 4), text.Substring(0, 2));
            if (text.Length == 10)
                return string.Format("{0}{1}{2}", text.Substring(6, 4), text.Substring(3, 2), text.Substring(0, 2));
            
            // wenn 2 Punkte drin sind, ist es vielleicht ein normales Datum
            string[] dateParts = text.Split('.');
            if (dateParts.Length == 3)
            {
                int day = Misc.Atoi(dateParts[0]);
                int month = Misc.Atoi(dateParts[1]);
                int year = Misc.Atoi(dateParts[2]);
                return string.Format("{0:d4}{1:d2}{2:d2}", year, month, day);
            }

            // Unschön, aber was soll man machen....
            return text;
        }

        /// <summary>
        /// Formatiert das Datum
        /// "20070000" => "2007"
        /// "20080100" => "01.2008"
        /// "20080219" => "19.02.2008"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatDate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (value.Length != 8)
                return value;    // Unschön, aber was soll man machen....

            if (value.EndsWith("0000"))
                return value.Substring(0, 4);
            if (value.EndsWith("00"))
                return string.Format("{0}.{1}", value.Substring(4, 2), value.Substring(0, 4));

            return string.Format("{0}.{1}.{2}", value.Substring(6, 2), value.Substring(4, 2), value.Substring(0, 4));
        }

        public static int ParseCurrencyValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            int value;

            // Alle Nicht-Zahlen entfernen (bis auf das Komma)
            for (int i = 0; i < text.Length; )
            {
                if (!Char.IsDigit(text[i]) && text[i] != ',')
                    text = text.Remove(i, 1);
                else
                    i++;
            }

            int iKomma = text.IndexOf(",");

            String sEuro;
            if (iKomma < 0)
                sEuro = text;
            else
                sEuro = text.Substring(0, iKomma);

            try
            {
                value = Convert.ToInt32(sEuro) * 100;
            }
            catch
            {
                value = 0;
            }

            if (iKomma >= 0)
            {
                try
                {
                    if (text.Substring(iKomma + 1).Length == 1)
                        value += Convert.ToInt32(text.Substring(iKomma + 1)) * 10;
                    else
                        value += Convert.ToInt32(text.Substring(iKomma + 1));
                }
                catch
                {
                }
            }

            return value;
        }

        public static string FormatCurrencyValue(int lValue)
        {
            String str;
            String sEuro;

            sEuro = String.Format("{0}", lValue / 100);
            if (sEuro.Length > 3)
                sEuro = sEuro.Insert(sEuro.Length - 3, ".");
            if (sEuro.Length > 7)
                sEuro = sEuro.Insert(sEuro.Length - 7, ".");

            str = String.Format("{0},{1:D2} {2}", sEuro, lValue % 100, Misc.GetCurrencySymbol());

            return str;
        }

        public static string GetCurrencySymbol()
        {
            return System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol;
        }

        /// <summary>
        /// Filtert alle ungültigen Zeichen aus dem angegebenen Dateinamen heraus.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string FilterFilenameChars(string filename)
        {
	        const string invalidCharacters = "\\/:*?\"<>|";
	        string cleanFilename = "";

	        int i=0;
	        while (i < filename.Length)
	        {
		        if (invalidCharacters.IndexOf(filename[i]) < 0)
			        cleanFilename += filename[i];

		        i++;
	        }

            return cleanFilename;
        }


        /// <summary>
        /// Liefert die Versionsnummer der angegebenen EXE oder DLL zurück.
        /// </summary>
        /// <param name="sFilename"></param>
        /// <returns></returns>
        public static string GetFileVersion(String fileName)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fileName);

            return string.Format("{0}.{1}.{2}.{3}", versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart);
        }

        /// <summary>
        /// Simmuliert die C++ atoi-Klasse
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public int Atoi(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                    return 0;

                int pos;

                for (pos = 0; pos < str.Length; pos++)
                    if (!Char.IsDigit(str[pos]))
                        break;

                int val = 0;

                Int32.TryParse(str.Substring(0, pos), out val);

                return val;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Startet die angegebene EXE und liefert den Rückgabewert zurück.
        /// </summary>
        /// <param name="executableFilename"></param>
        /// <returns></returns>
        static public int StartProcessAndWait(string executableFilename)
        {
            Process process = new Process();
            process.StartInfo.FileName = executableFilename; 
            process.Start();
            while (!process.HasExited)
            {
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(1000);
            }

            return process.ExitCode;
        }

        /// <summary>
        /// Macht eine Kopie eines beliebigen Objekts.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        static public Object CopyObject(Object source)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            bf.Serialize(mem, source);

            mem.Position = 0;
            Object ctl = bf.Deserialize(mem);

            mem.Close();

            return ctl;
        }

        /// <summary>
        /// Liefert das Verzeichnis zurück, aus dem Hitbase gestartet wurde.
        /// </summary>
        /// <returns></returns>
        static public string GetApplicationPath()
        {
            string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            return appPath;
        }

        // Liefert das aktuelle Tagesdatum in der Form (JJJJMMTT) zurück.
        static public string GetDate()
        {
	        DateTime now = DateTime.Now;
	        String str;

	        str = string.Format("{0:D4}{1:D2}{2:D2}", now.Year, now.Month, now.Day);

	        return str;
        }


        /// <summary>
        /// Liefert den kompletten Pfad zurück, für den Speicherort von CD-Covern
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static public String GetCDCoverFilename(String filename)
        {
            String directory = GetCDCoverDirectory();

	        // Verzeichnis anlegen, muss immer da sein.
	        Misc.CreateDirectory(directory);

	        String coverFilename = Path.Combine(directory, filename);

	        return coverFilename;
        }

        /// <summary>
        /// Liefert das CD-Cover-Verzeichnis zurück.
        /// </summary>
        /// <returns></returns>
        static public String GetCDCoverDirectory()
        {
            String directory;

            if (string.IsNullOrEmpty(Settings.Current.DefaultCDCoverPath))
            {
                directory = Misc.GetPersonalHitbaseFolder() + "\\Images";
            }
            else
            {
                directory = Settings.Current.DefaultCDCoverPath;
            }
            return directory;
        }

        /// <summary>
        /// Wenn der angegebene Dateiname vorhanden ist, wird er einfach zurückgeliefert,
        /// ansonsten wird noch geprüft, ob der Name im Standard CD-Cover-Verzeichnis vorhanden ist.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static public String FindCover(String filename)
        {
            if (string.IsNullOrEmpty(filename) || File.Exists(filename))
                return filename;

            string defaultCoverPathFilename = Path.Combine(Settings.Current.DefaultCDCoverPath, Path.GetFileName(filename));
            if (File.Exists(defaultCoverPathFilename))
                return defaultCoverPathFilename;

            return filename;
        }

        /// <summary>
        /// Liefert den Pfad zurück, indem Hitbase Daten speichern kann.
        /// </summary>
        /// <returns></returns>
        static public String GetPersonalHitbaseFolder()
        {
	        String sMyDocs = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);

	        String path = Path.Combine(sMyDocs, "Hitbase");

        	Misc.CreateDirectory(path);

        	return path;
        }

        /// <summary>
        /// Liefert den Pfad zurück, in dem die Playlisten gespeichert sind.
        /// </summary>
        /// <returns></returns>
        static public String GetPersonalHitbasePlaylistFolder()
        {
            string path = System.IO.Path.Combine(Misc.GetPersonalHitbaseFolder(), "Playlists");

            Misc.CreateDirectory(path);

            return path;
        }

        /// <summary>
        /// Liefert den Pfad zurück, indem Hitbase Daten speichern kann.
        /// </summary>
        /// <returns></returns>
        static public String GetPersonalHitbaseMusicFolder()
        {
            String sMyDocs = System.Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            String path = Path.Combine(sMyDocs, "Hitbase");

            Misc.CreateDirectory(path);

            return path;
        }

        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }


        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathRelativePathTo(
             [Out] StringBuilder pszPath,
             [In] string pszFrom,
             [In] uint dwAttrFrom,
             [In] string pszTo,
             [In] uint dwAttrTo
        );



        public static string GetRelativePath(string path1, string path2)
        {
            StringBuilder relPath = new StringBuilder(500);
            
            bool result = PathRelativePathTo(relPath, path1, 0, path2, 0);

            // Dann ist der path2 wohl auf einem anderen Laufwerk als path1.
            if (result == false)
                return path2;

            return relPath.ToString();
        }

        /// <summary>
        /// Wandelt die angegebenen Sekunden in einen Klartext um. Maximal werden Tage angezeigt.
        /// z.B.: 5 Tage 3 Stunden 1 Minute 20 Sekunden
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string GetTextFromSeconds(int seconds, bool compactForm = true)
        {
            const int AnzahlSekundenTag = 86400;
            const int AnzahlSekundenStunde = 3600;
            const int AnzahlSekundenMinute = 60;

            int anzahlTage = 0;
            int anzahlStunden = 0;
            int anzahlMinuten = 0;
            int anzahlSekunden = 0;

            // Tage ausrechnen
            anzahlTage = seconds / AnzahlSekundenTag;
            seconds -= (anzahlTage * AnzahlSekundenTag);

            // Stunden ausrechnen
            anzahlStunden = seconds / AnzahlSekundenStunde;
            seconds -= (anzahlStunden * AnzahlSekundenStunde);

            // Minuten ausrechnen
            anzahlMinuten = seconds / AnzahlSekundenMinute;
            seconds -= (anzahlMinuten * AnzahlSekundenMinute);

            // Sekunden nicht ausrechnen sondern (nur) eintragen
            anzahlSekunden = seconds;

            // Jetzt den Text zusammenbauen
            string text = "";

           // Tage
           if (anzahlTage > 0)
           {
               text += string.Format("{0} {1} ", anzahlTage, anzahlTage == 1 ? StringTable.Day : StringTable.Days);
           }

           // Stunden
           if (anzahlStunden > 0)
           {
               text += string.Format("{0} {1} ", anzahlStunden, anzahlStunden == 1 ? StringTable.Hour : StringTable.Hours);
           }

           // Minuten
           if (anzahlMinuten > 0 && (!compactForm || anzahlTage == 0))
           {
               text += string.Format("{0} {1} ", anzahlMinuten, anzahlMinuten == 1 ? StringTable.Minute : StringTable.Minutes);
           }

           // Sekunden
           if (anzahlSekunden > 0 && (!compactForm || anzahlTage == 0 && anzahlStunden == 0))
           {
               text += string.Format("{0} {1}", anzahlSekunden, anzahlSekunden == 1 ? StringTable.Second : StringTable.Seconds);
           }

           return text.Trim();
        }

        public static void GetAlbumInfoByFilename(string filename, string delimiter, out string artist, out string title)
        {
            String sPureFilename = Path.GetFileNameWithoutExtension(filename);

            title = "";
            artist = "";
            int iPos;

            // Irgendwelche Unix Gurus haben scheinbar was gegen Leerzeichen
            // Das ändern wir jetzt erst mal wieder
            sPureFilename = sPureFilename.Replace("_", " ");

            // Die Ermittlung der Titels und des Artist machen wir am besten mal
            // rückwärts...
            int iPos1 = sPureFilename.LastIndexOf(delimiter);

            if (iPos1 < 0)
                title = sPureFilename.Mid(0);
            else
                title = sPureFilename.Mid(iPos1 + delimiter.Length);

            if (iPos1 >= 0)
                artist = sPureFilename.Left(iPos1);

            int iPos2 = artist.LastIndexOf(delimiter);

            if (iPos2 > 0)
                artist = artist.Mid(iPos2 + delimiter.Length);

            // Zahlen am Anfang eines Titels oders Artist werden
            // gelöscht... Das stimmt zwar nicht immer aber besser als
            // die Zahlen drin lassen
            if (Settings.Current.DeleteLeadingNumbers)
            {
                while (title.IndexOfAny("0123456789 -".ToCharArray()) == 0)
                {
                    title = title.Mid(1);
                }
                while (artist.IndexOfAny("0123456789 -".ToCharArray()) == 0)
                {
                    artist = artist.Mid(1);
                }
            }

            // Wenn beim Artist noch ein Komma drin ist den Namen drehen
            if ((iPos = artist.IndexOf(',')) > 0)
            {
                string s1, s2;
                s1 = artist.Left(iPos);
                s1 = s1.Trim();
                s2 = artist.Mid(iPos + 1);
                s2 = s2.Trim();
                artist = s2 + " " + s1;
            }

            // So jetzt checken wir mal ob da irgendwelche Kommentare
            // noch dranhängen z.B.: Afrika(192 kbit)
            // d.h. alle Sachen beginnend mit '(', '{' und '[' fliegen raus!
            if ((iPos = title.IndexOf('(')) > 0)
                title = title.Mid(0, iPos);
            if ((iPos = title.IndexOf('[')) > 0)
                title = title.Mid(0, iPos);
            if ((iPos = title.IndexOf('{')) > 0)
                title = title.Mid(0, iPos);

            // Zum Schluß mal ein paar Leerzeichen weg...
            artist = artist.Trim();
            title = title.Trim();

            /*string[] tokens = Path.GetFileNameWithoutExtension(filename).Split('-');
            List<string> infos = new List<string>();

            for (int i = 0; i < tokens.Length; i++)
            {
                string formattedToken = tokens[i];
                
                formattedToken = formattedToken.Replace("_", " ");
                formattedToken = formattedToken.Trim();

                if (formattedToken.IsNumeric())
                    sfi.TrackNumber = Misc.Atoi(formattedToken);
                else
                    infos.Add(formattedToken);
            }

            if (infos.Count == 1)
                sfi.Title = infos[0];
            if (infos.Count == 2)
            {
                sfi.Artist = infos[0];
                sfi.Title = infos[1];
            }
            if (infos.Count == 3)
            {
                sfi.Artist = infos[0];
                sfi.Album = infos[1];
                sfi.Title = infos[2];
            }
            */
        }

        /*public static void WriteInstallationTime()
        {
            // Installationsdatum in die Registry schreiben
            RegistryKey reg = Registry.ClassesRoot.OpenSubKey(".hdbx", true);

            int TimeBuf = DateTime.Now.Year * 365 + DateTime.Now.Month * 31 + DateTime.Now.Day;

            // Dieser Key muss bei jeder Hitbase-Version eindeutig neu vergeben werden, damit
            // nicht das Datum einer alten Shareware-Version erkannr wird.
            // Bisher:
            // Hitbase 2010: OpenEx
            // Hitbase 2007: MoveEx
            // Hitbase 2005: CopyEx
            // Hitbase 2003: send
            // Hitbase 2001: print

            if (reg.GetValue("OpenEx") == null)
                reg.SetValue("OpenEx", TimeBuf);

            reg.Close();
        }*/

        /// <summary>
        /// Speichert die angegebene Bitmap in der angegebenen Datei als JPG.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="bi"></param>
        public static void WriteBitmapImageToFile(string filename, BitmapImage bi)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Create))
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bi));
                encoder.QualityLevel = 100;
                encoder.Save(fileStream);
            }
        }

        /// <summary>
        /// Liefert true zurück, wenn das Betriebssystem Windows 7 oder später ist.
        /// </summary>
        public static bool IsWindows7OrLater()
        {
            return (System.Environment.OSVersion.Version.Major == 6 && System.Environment.OSVersion.Version.Minor >= 1 ||
                System.Environment.OSVersion.Version.Major > 6);
        }

        #region IconToAlphaBitmap
        [DllImport("gdi32.dll", SetLastError = true)]
        static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        public struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }


        public static Bitmap IconToAlphaBitmap(Icon ico)
        {
            ICONINFO ii = new ICONINFO();
            GetIconInfo(ico.Handle, out ii);
            Bitmap bmp = Bitmap.FromHbitmap(ii.hbmColor);
            DeleteObject(ii.hbmColor);
            DeleteObject(ii.hbmMask);

            if (Bitmap.GetPixelFormatSize(bmp.PixelFormat) < 32)
                return ico.ToBitmap();

            BitmapData bmData;
            Rectangle bmBounds = new Rectangle(0, 0, bmp.Width, bmp.Height);

            bmData = bmp.LockBits(bmBounds, ImageLockMode.ReadOnly, bmp.PixelFormat);

            Bitmap dstBitmap = new Bitmap(bmData.Width, bmData.Height, bmData.Stride, PixelFormat.Format32bppArgb, bmData.Scan0);

            bool IsAlphaBitmap = false;

            for (int y = 0; y <= bmData.Height - 1; y++)
            {
                for (int x = 0; x <= bmData.Width - 1; x++)
                {
                    Color PixelColor = Color.FromArgb(Marshal.ReadInt32(bmData.Scan0, (bmData.Stride * y) + (4 * x)));
                    if (PixelColor.A > 0 & PixelColor.A < 255)
                    {
                        IsAlphaBitmap = true;
                        break;
                    }
                }
                if (IsAlphaBitmap) break;
            }

            bmp.UnlockBits(bmData);

            if (IsAlphaBitmap == true)
                return new Bitmap(dstBitmap);
            else
                return new Bitmap(ico.ToBitmap());

        }
        #endregion

        /// <summary>
        /// Generiert aus dem angegebenen Bild einen Thumbnail in der angegebenen Größe
        /// </summary>
        /// <param name="frontCover"></param>
        /// <returns></returns>
        public static byte[] CreateThumbnail(string frontCover, int width, int height, int quality = 100)
        {
            // Create a BitmapImage and sets its DecodePixelWidth and DecodePixelHeight

            BitmapImage bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.UriSource = new Uri(frontCover, UriKind.RelativeOrAbsolute);
            bmpImage.DecodePixelWidth = width;
            bmpImage.DecodePixelHeight = height;
            bmpImage.EndInit();

            using (MemoryStream memStream = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapImage)bmpImage));
                encoder.QualityLevel = quality;
                encoder.Save(memStream);

                return memStream.ToArray();
            }
        }

        public static void ShowFileInExplorer(string filename)
        {
            // combine the arguments together
            // it doesn't matter if there is a space after ','
            string argument = "/select,\"" + filename + "\"";

            Process.Start("explorer.exe", argument);
        }
    }
}
