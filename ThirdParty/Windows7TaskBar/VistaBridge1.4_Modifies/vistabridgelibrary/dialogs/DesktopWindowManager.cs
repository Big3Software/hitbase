// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Interop;
using Microsoft.SDK.Samples.VistaBridge.Interop;
using System.Windows;
using System.Windows.Media;
using System.Drawing;

namespace Microsoft.SDK.Samples.VistaBridge.Dialogs
{
    /// <summary>
    ///  DesktopWindowManager
    ///  Enables Blur/Translusency on the Client and Non Client Areas of Windows and WPF applications
    /// </summary>
    internal static class DesktopWindowManager
    {

        /// <summary>
        /// ResetBlur
        /// Resets the blur on the Window/form
        /// </summary>
        /// <param name="windowHandle">Handle to client window/form</param>
        internal static void ResetBlur(IntPtr windowHandle)
        {
            // Create and populate the BlurBehind structure
            SafeNativeMethods.DWM_BLURBEHIND blurBehind = new SafeNativeMethods.DWM_BLURBEHIND();
            blurBehind.fEnable = false;
            blurBehind.dwFlags = SafeNativeMethods.DwmBlurBehindDwFlags.DWM_BB_ENABLE | SafeNativeMethods.DwmBlurBehindDwFlags.DWM_BB_BLURREGION;
            blurBehind.hRgnBlur = IntPtr.Zero;
            UnsafeNativeMethods.DwmEnableBlurBehindWindow(windowHandle, ref blurBehind);
        }

        /// <summary>
        /// FrameThickness
        /// Extends the Frame(Non Client Area) of the Window/Form into client area
        /// </summary>
        /// <param name="left">Left margin to which extent the frame needs to be extended from left side</param>
        /// <param name="right">Right margin to which extent the frame needs to be extended from right side</param>
        /// <param name="top">Top margin to which extent the frame needs to be extended from top side</param>
        /// <param name="bottom">Bottom margin to which extent the frame needs to be extended from bottom side</param>
        /// <param name="windowHandle">Handle to client window/form</param>
        /// <returns>0 for Success</returns>
        internal static int FrameThickness(int left, int right, int top, int bottom, IntPtr windowHandle)
        {
            // We only enable to set FrameThickness if DWM Composition is enabled.
            if (!UnsafeNativeMethods.DwmIsCompositionEnabled())
                return -1;

            // Create a margin structure
            SafeNativeMethods.MARGINS margins = new SafeNativeMethods.MARGINS();
            margins.cxLeftWidth = left;
            margins.cxRightWidth = right;
            margins.cyTopHeight = top;
            margins.cyBottomHeight = bottom;

            // Extend the Frame into client area
            return UnsafeNativeMethods.DwmExtendFrameIntoClientArea(windowHandle, ref margins); 
        }

    }
}