// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Interop;

namespace Microsoft.SDK.Samples.VistaBridge.Dialogs
{
    /// <summary>
    /// Windows Glass Form
    /// Inherit from this form to be able to enable glass on Windows Form
    /// </summary>
    public class GlassForm : Form
    {
        /// <summary>
        /// Constructor for GlassForm
        /// </summary>
        public GlassForm()
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
                    isGlassEnabled = value;

                    // Reset the existing blur
                    DesktopWindowManager.ResetBlur(this.Handle);

                    if (isGlassEnabled)
                    {
                        // Extend the frame to entire client area
                        DesktopWindowManager.FrameThickness(-1, 0, 0, 0, this.Handle);
                    }

                    this.Invalidate();
                }
            }
        }


        private Padding formThickness;
        /// <summary>
        /// FrameThickness
        /// Extends the Frame(Non Client Area) of the Window into client area
        /// </summary>
        public Padding FormThickness 
        {
            get 
            {
                return formThickness;
            }
            set
            {
                formThickness = value;
                
                // Extend the frame into client area
                DesktopWindowManager.FrameThickness(formThickness.Left, 
                                                    formThickness.Right, 
                                                    formThickness.Top, 
                                                    formThickness.Bottom, 
                                                    this.Handle);

                this.Invalidate();
            }
        }

        /// <summary>
        /// Overide OnPaint to paint the background as black.
        /// </summary>
        /// <param name="e">PaintEventArgs</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (isGlassEnabled)
            {
                // Paint the all the regions black to enable glass
                e.Graphics.FillRectangle(Brushes.Black, this.ClientRectangle);
            }

        }
        
    }
}