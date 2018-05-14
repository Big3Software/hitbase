// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Microsoft.SDK.Samples.VistaBridge.Dialogs
{
    /// <summary>
    /// WPF Glass Window
    /// Inherit from this window class to enable glass on a WPF window
    /// </summary>
    /// /// <remarks>
    /// A windowHandle needs to be set later for glass to work
    /// </remarks>
    public class GlassWindow : Window
    {
        private IntPtr windowHandle;

        /// <summary>
        /// Constructor for glass window
        /// </summary>
        /// <remarks>
        /// A windowHandle needs to be set later for glass to work
        /// </remarks>
        public GlassWindow()
        {}

        private bool isGlassEnabled;
        /// <summary>
        /// Enables glass of the entire client area
        /// </summary>
        public bool IsGlassEnabled
        {
            get
            {
                return isGlassEnabled;
            }
            set
            {
                // Is the glass property being cahnged?
                if (isGlassEnabled != value)
                {
                    // Do we have a handle?
                    if (windowHandle != null && windowHandle != IntPtr.Zero)
                    {
                        isGlassEnabled = value;

                        // Reset the Glass
                        DesktopWindowManager.ResetBlur(windowHandle);

                        if (isGlassEnabled)
                        {
                            SetBackGroundTransparent();

                            // Extend the frame to entire client area
                            DesktopWindowManager.FrameThickness(-1, 0, 0, 0, windowHandle);

                        }
                    }
                }       
            }
        }


        private Thickness frameThickness;
        /// <summary>
        /// FrameThickness
        /// Extends the Frame(Non Client Area) of the Window into client area
        /// Thickness should be integer values only
        /// </summary>
        /// <remarks>
        /// Glass should be enabled prior to setting this property for
        /// the extended frame to blend into non client area.
        /// </remarks>
        public Thickness FrameThickness 
        { 
            get 
            {
                return frameThickness;
            }
            set
            {
                // Do we have a valid Window Handle?
                if (windowHandle != null && windowHandle != IntPtr.Zero)
                {
                    frameThickness = value;

                    // Extend the frame into client area
                    // Only integers are allowed as thickness margins.
                    DesktopWindowManager.FrameThickness(Convert.ToInt32(frameThickness.Left),
                                                        Convert.ToInt32(frameThickness.Right),
                                                        Convert.ToInt32(frameThickness.Top),
                                                        Convert.ToInt32(frameThickness.Bottom),
                                                        windowHandle);
                }
            }
        }

        /// <summary>
        /// Makes the background of current window transparent from both Wpf and Windows Perspective
        /// </summary>
        private void SetBackGroundTransparent()
        {
            // Set the Background to transparent from Win32 perpective 
            HwndSource.FromHwnd(windowHandle).CompositionTarget.BackgroundColor = System.Windows.Media.Colors.Transparent;

            // Set the Background to transparent from WPF perpective 
            this.Background = Brushes.Transparent;

        }

        /// <summary>
        /// OnSourceInitialized
        /// Override SourceInitialized to initialize windowHandle for this window.
        /// A valid windowHandle is available only after the sourceInitialized is completed
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowInteropHelper interopHelper = new WindowInteropHelper(this);
            this.windowHandle = interopHelper.Handle;
        }
    }
}