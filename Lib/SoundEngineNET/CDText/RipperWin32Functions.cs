using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Big3.Hitbase.SoundEngine.CDText
{
    class RipperWin32Functions
    {
        public const int MINIMUM_CDROM_READ_TOC_EX_SIZE = 2;
        public const int MAXIMUM_NUMBER_TRACKS = 100;

        public const int IOCTL_CDROM_READ_TOC_EX = 0x00024054;

        public enum CDROM_READ_TOC_EX_FORMAT
        {
            TOC,
            SESSION,
            FULL_TOC,
            PMA,
            ATIP,
            CDTEXT
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct CDROM_READ_TOC_EX
        {
            public uint bitVector;

            public CDROM_READ_TOC_EX_FORMAT Format
            {
                get { return ((CDROM_READ_TOC_EX_FORMAT)((this.bitVector & 15u))); }
                set { this.bitVector = (uint)((byte)value | this.bitVector); }
            }

            public uint Reserved1
            {
                get { return ((uint)(((this.bitVector & 112u) / 16))); }
                set { this.bitVector = ((uint)(((value * 16) | this.bitVector))); }
            }

            public uint Msf
            {
                get { return ((uint)(((this.bitVector & 128u) / 128))); }
                set { this.bitVector = ((uint)(((value * 128) | this.bitVector))); }
            }

            public byte SessionTrack;
            public byte Reserved2;
            public byte Reserved3;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class CDROM_TOC_CD_TEXT_DATA
        {
            public ushort Length;
            public byte Reserved1;
            public byte Reserved2;
            public CDROM_TOC_CD_TEXT_DATA_BLOCK_ARRAY Descriptors;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal sealed class CDROM_TOC_CD_TEXT_DATA_BLOCK_ARRAY
        {
            internal CDROM_TOC_CD_TEXT_DATA_BLOCK_ARRAY() { data = new byte[MaxIndex * Marshal.SizeOf(typeof(CDROM_TOC_CD_TEXT_DATA_BLOCK))]; }

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MINIMUM_CDROM_READ_TOC_EX_SIZE * MAXIMUM_NUMBER_TRACKS * 18)]
            private byte[] data;

            public int MaxIndex
            {
                get { return MINIMUM_CDROM_READ_TOC_EX_SIZE * MAXIMUM_NUMBER_TRACKS; }
            }

            public CDROM_TOC_CD_TEXT_DATA_BLOCK this[int idx]
            {
                get
                {
                    if((idx < 0) || (idx >= MaxIndex)) throw new IndexOutOfRangeException();

                    CDROM_TOC_CD_TEXT_DATA_BLOCK res;

                    GCHandle hData = GCHandle.Alloc(data, GCHandleType.Pinned);

                    try
                    {
                        IntPtr buffer = hData.AddrOfPinnedObject();

                        buffer = (IntPtr)(buffer.ToInt32() + (idx * Marshal.SizeOf(typeof(CDROM_TOC_CD_TEXT_DATA_BLOCK))));

                        res = (CDROM_TOC_CD_TEXT_DATA_BLOCK)Marshal.PtrToStructure(buffer, typeof(CDROM_TOC_CD_TEXT_DATA_BLOCK));
                    }
                    finally
                    {
                        hData.Free();
                    }

                    return res;
                }
            }
        }

        public enum CDROM_CD_TEXT_PACK : byte
        {
            ALBUM_NAME  = 0x80,
            PERFORMER   = 0x81,
            SONGWRITER  = 0x82,
            COMPOSER    = 0x83,
            ARRANGER    = 0x84,
            MESSAGES    = 0x85,
            DISC_ID     = 0x86,
            GENRE       = 0x87,
            TOC_INFO    = 0x88,
            TOC_INFO2   = 0x89,
            UPC_EAN     = 0x8e,
            SIZE_INFO   = 0x8f
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct CDROM_TOC_CD_TEXT_DATA_BLOCK
        {
            public CDROM_CD_TEXT_PACK PackType;

            public byte bitVector1;

            public byte TrackNumber
            {
                get { return ((byte)((this.bitVector1 & 127u))); }
                set { this.bitVector1 = ((byte)((value | this.bitVector1))); }
            }

            public byte ExtensionFlag
            {
                get { return ((byte)(((this.bitVector1 & 128u) / 128))); }
                set { this.bitVector1 = ((byte)(((value * 128) | this.bitVector1))); }
            }

            public byte SequenceNumber;

            public byte bitVector2;

            public byte CharacterPosition
            {
                get { return ((byte)((this.bitVector2 & 15u))); }
                set { this.bitVector2 = ((byte)((value | this.bitVector2))); }
            }

            public byte BlockNumber
            {
                get { return ((byte)(((this.bitVector2 & 112u) / 16))); }
                set { this.bitVector2 = ((byte)(((value * 16) | this.bitVector2))); }
            }

            public byte Unicode
            {
                get { return ((byte)(((this.bitVector2 & 128u) / 128))); }
                set { this.bitVector2 = ((byte)(((value * 128) | this.bitVector2))); }
            }

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = UnmanagedType.I1)]
            public byte[] TextBuffer;

            public string Text
            {
                //get { return (Unicode == 1)? ASCIIEncoding.ASCII.GetString(TextBuffer) : UTF32Encoding.UTF8.GetString(TextBuffer); }
                get { return (Unicode == 1)? System.Text.Encoding.Default.GetString(TextBuffer) : UTF32Encoding.UTF7.GetString(TextBuffer); }
            }

            public ushort CRC;
        }

        [System.Runtime.InteropServices.DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int DeviceIoControl(IntPtr hDevice, uint IoControlCode, IntPtr InBuffer, uint InBufferSize,
                                                 [Out] IntPtr OutBuffer, uint OutBufferSize, out uint BytesReturned, IntPtr Overlapped);
    }
}
