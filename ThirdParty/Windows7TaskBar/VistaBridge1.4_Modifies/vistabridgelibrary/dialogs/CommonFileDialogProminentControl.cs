// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Markup;
using Microsoft.SDK.Samples.VistaBridge.Interop;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Common class for all Prominent controls on Common File Dialog
    /// </summary>
    [ContentProperty("Items")]
    public abstract class CommonFileDialogProminentControl : CommonFileDialogControl
    {
        private bool isProminent;

        /// <summary>
        /// Sets or gets the prominent value of this control. Note: Only one control
        /// can be specified as prominent. If more than one control is specified 
        /// prominent, then an 'E_UNEXPECTED' exception will be thrown when these
        /// controls are added to the dialog.  A group box control can only be
        /// specified as prominent if it contains one control and that control is
        /// of the type 'CommonFileDialogProminentControl'.
        /// </summary>
        public bool IsProminent
        {
            get { return isProminent; }
            set { isProminent = value; }
        }


        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        public CommonFileDialogProminentControl()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text
        /// </summary>
        /// <param name="text">Text to display for this control</param>
        public CommonFileDialogProminentControl(string text)
            : base(text)
        {            
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name and text
        /// </summary>
        /// <param name="name">The name of this control</param>
        /// <param name="text">Text to display for this control</param>
        public CommonFileDialogProminentControl(string name, string text)
            : base(name, text)
        {
        }    
    }
}