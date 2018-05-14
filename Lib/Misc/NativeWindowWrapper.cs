using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Big3.Hitbase.Miscellaneous
{
    public class NativeWindowWrapper : System.Windows.Forms.IWin32Window
    {
        public NativeWindowWrapper(System.Windows.Window owner)
        {
            // Create a WindowInteropHelper for the WPF Window
            interopHelper = new WindowInteropHelper(owner);
        }

        private WindowInteropHelper interopHelper;

        public IntPtr Handle
        {
            get
            {
                // Return the surrogate handle
                return interopHelper.Handle;
            }
        }

    }
}
