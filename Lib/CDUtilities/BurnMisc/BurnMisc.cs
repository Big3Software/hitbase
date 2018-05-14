using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using IMAPI2.Interop;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;

namespace Big3.Hitbase.CDUtilities
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // Large icon
        public const uint SHGFI_SMALLICON = 0x1; // Small icon
        public const uint SHGFI_TYPENAME = 0x400; // Retrieve the string that describes the file's type. The string is copied to the szTypeName 

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr handle);

        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        public const uint STGM_DELETEONRELEASE = 0x04000000;

        public const uint STGM_DIRECT = 0x00000000;
        public const uint STGM_TRANSACTED = 0x00010000;
        public const uint STGM_SIMPLE = 0x08000000;
    
        public const uint STGM_READ = 0x00000000;
        public const uint STGM_WRITE = 0x00000001;
        public const uint STGM_READWRITE = 0x00000002;

        public const uint STGM_SHARE_DENY_NONE = 0x00000040;
        public const uint STGM_SHARE_DENY_READ = 0x00000030;
        public const uint STGM_SHARE_DENY_WRITE = 0x00000020;
        public const uint STGM_SHARE_EXCLUSIVE = 0x00000010;

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false, EntryPoint = "SHCreateStreamOnFileW")]
        public static extern void SHCreateStreamOnFile(string fileName, uint mode, ref IStream stream);

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false, EntryPoint = "SHCreateStreamOnFileEx")]
        public static extern void SHCreateStreamOnFileEx(string fileName, uint mode, uint attributes, bool create, IStream template, ref IStream stream);
    }
 }
