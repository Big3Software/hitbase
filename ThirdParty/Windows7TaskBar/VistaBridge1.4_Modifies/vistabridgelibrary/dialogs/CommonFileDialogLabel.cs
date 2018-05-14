// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System.Diagnostics;
using Microsoft.SDK.Samples.VistaBridge.Interop;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Common class for label controls on Common File Dialog
    /// </summary>
    public class CommonFileDialogLabel : CommonFileDialogControl
    {
        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        public CommonFileDialogLabel()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text
        /// </summary>
        /// <param name="text">the text of text control</param>
        public CommonFileDialogLabel(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name="name">The name of this control</param>
        /// <param name="text">Text to display for this control</param>
        public CommonFileDialogLabel(string name, string text)
            : base(name, text)
        {
        }

        // <summary>
        /// Attach this control to the dialog object
        /// </summary>
        /// <param name="dialog">Target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialog.Attach: dialog parameter can not be null");

            // Add a text control
            dialog.AddText(this.Id, this.Text);

            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();
        }
    }
}