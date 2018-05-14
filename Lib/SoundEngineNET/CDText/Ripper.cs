using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Big3.Hitbase.DataBaseEngine;

namespace Big3.Hitbase.SoundEngine.CDText
{
    public class Ripper
    {
        public Ripper(CDDrive drive)
        {
            m_Drive = drive;
        }

        public bool ReadCDText(CD cd)
        {
            // check for at least WinXP (5.1)
            Version version = Environment.OSVersion.Version;
            if(Environment.OSVersion.Platform != PlatformID.Win32NT ||
                version.Major < 5 || (version.Major == 5 && version.Minor < 1))
            {
                return false;
            }

            RipperWin32Functions.CDROM_READ_TOC_EX TOCex = new RipperWin32Functions.CDROM_READ_TOC_EX();
            TOCex.Format = RipperWin32Functions.CDROM_READ_TOC_EX_FORMAT.CDTEXT;

            int nTOCex = Marshal.SizeOf(TOCex);
            IntPtr hTOCex = Marshal.AllocHGlobal(nTOCex);
            Marshal.StructureToPtr(TOCex, hTOCex, true);
            RipperWin32Functions.CDROM_TOC_CD_TEXT_DATA Data = new RipperWin32Functions.CDROM_TOC_CD_TEXT_DATA();
            Data.Length = (ushort)Marshal.SizeOf(Data);
            IntPtr hData = Marshal.AllocHGlobal(Data.Length);
            Marshal.StructureToPtr(Data, hData, true);
            uint BytesRead = 0;

            bool b = RipperWin32Functions.DeviceIoControl(m_Drive.CDHandle, RipperWin32Functions.IOCTL_CDROM_READ_TOC_EX, hTOCex, (uint)nTOCex, hData, Data.Length, out BytesRead, IntPtr.Zero) != 0;
            if(b)
            {
                Marshal.PtrToStructure(hData, Data);
                CreateCDText(cd, Data);
            }

            Marshal.FreeHGlobal(hData);
            Marshal.FreeHGlobal(hTOCex);

            return b;
        }

        private void CreateCDText(CD cd, RipperWin32Functions.CDROM_TOC_CD_TEXT_DATA Data)
        {
            string sText = String.Empty;

            try
            {
                int trackIndex = 0;

                for(int i = 0; i < Data.Descriptors.MaxIndex; i++)
                {
                    RipperWin32Functions.CDROM_TOC_CD_TEXT_DATA_BLOCK block = Data.Descriptors[i];
                    trackIndex = block.TrackNumber - 1;
                    
                    foreach(char c in block.Text)
                    {
                        if (c != 0)
                        {
                            sText += c;
                        }
                        else
                        {
                            string trimmedText = sText.Trim();

                            switch (block.PackType)
                            {
                                case RipperWin32Functions.CDROM_CD_TEXT_PACK.ALBUM_NAME:
                                    {
                                        if (trackIndex == -1)
                                        {
                                            cd.Title = trimmedText;
                                        }
                                        else
                                        {
                                            if (trackIndex < cd.Tracks.Count)
                                                cd.Tracks[trackIndex].Title = trimmedText;
                                        }

                                        break;
                                    }
                                case RipperWin32Functions.CDROM_CD_TEXT_PACK.PERFORMER:
                                    {
                                        if (trackIndex == -1)
                                        {
                                            cd.Artist = trimmedText;
                                        }
                                        else
                                        {
                                            if (trackIndex < cd.Tracks.Count)
                                                cd.Tracks[trackIndex].Artist = trimmedText;
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        // Was gibts sonst noch so??
                                        break;
                                    }
                            }

                            sText = String.Empty;
                            trackIndex++;
                        }
                    }
                }

                // Testen, ob es ein Sampler ist
                foreach (Track track in cd.Tracks)
                {
                    if (track.Artist != cd.Artist && !string.IsNullOrEmpty(track.Artist) && !string.IsNullOrEmpty(track.Artist))
                    {
                        cd.Sampler = true;
                        break;
                    }
                }
            }            
            catch(IndexOutOfRangeException)
            {
                // end of list
            }
            finally
            {
// Todo??
                //if(sText != String.Empty)
                //    list.Add(sText);
            }
        }

        private CDDrive m_Drive;
    }
}
