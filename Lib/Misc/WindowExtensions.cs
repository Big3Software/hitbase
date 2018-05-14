using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;

namespace Big3.Hitbase.Miscellaneous
{
    public static class WindowExtensions 
    { 
        [DllImport("user32.dll")]
        internal extern static int SetWindowLong(IntPtr hwnd, int index, int value);
        
        [DllImport("user32.dll")] 
        internal extern static int GetWindowLong(IntPtr hwnd, int index); 
        
        [DllImport("user32.dll")]
        public static extern int RegisterWindowMessage(String strMessage);

        public static void HideMinimizeAndMaximizeButtons(this Window window) 
        { 
            const int GWL_STYLE = -16; 
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle; 
            long value = GetWindowLong(hwnd, GWL_STYLE); 
            SetWindowLong(hwnd, GWL_STYLE, (int)(value & -131073 & -65537)); 
        } 

        public static void DisableAutoplay()
        {
            RegisterWindowMessage("QueryCancelAutoPlay");
        }
    }
}
