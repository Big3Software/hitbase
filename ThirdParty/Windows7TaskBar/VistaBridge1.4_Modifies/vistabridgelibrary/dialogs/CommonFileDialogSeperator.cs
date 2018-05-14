// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System.Diagnostics;
using Microsoft.SDK.Samples.VistaBridge.Interop;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Common class for the simplest seperator control
    /// </summary>
    public class CommonFileDialogSeparator : CommonFileDialogControl
    {
        // <summary>
        /// Attach the Separator control to the dialog object
        /// </summary>
        /// <param name="dialog">Target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogSeparator.Attach: dialog parameter can not be null");

            // Add a separator
            dialog.AddSeparator(this.Id);

            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();
        }
    }
}