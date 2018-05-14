// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.SDK.Samples.VistaBridge.Interop;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Common class for all CheckButton controls on Common File Dialog
    /// </summary>
    public class CommonFileDialogCheckBox : CommonFileDialogProminentControl
    {
        /// <summary>
        /// Gets or sets the check state of CheckButton
        /// </summary>
        private bool isChecked = false;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                // Check if property has changed
                if (isChecked != value)
                {
                    isChecked = value;
                    ApplyPropertyChange("IsChecked");
                }
            }
        }

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        public CommonFileDialogCheckBox()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text
        /// </summary>
        /// <param name="text">Text to display for this control</param>
        public CommonFileDialogCheckBox(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name and text
        /// </summary>
        /// <param name="name">The name of this control</param>
        /// <param name="text">Text to display for this control</param>
        public CommonFileDialogCheckBox(string name, string text)
            : base(name, text)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text and check state.
        /// </summary>
        /// <param name="text">Text to display for this control</param>
        /// <param name="isChecked">Check state of this control</param>
        public CommonFileDialogCheckBox(string text, bool isChecked)
            : base(text)
        {
            this.isChecked = isChecked;
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name, text and checked state.
        /// </summary>
        /// <param name="name">The name of this control</param>
        /// <param name="text">Text to display for this control</param>
        /// <param name="isChecked">Check state of this control</param>
        public CommonFileDialogCheckBox(string name, string text, bool isChecked)
            : base(name, text)
        {
            this.isChecked = isChecked;
        }

        /// <summary>
        /// Occurs when the check state is changed
        /// </summary>
        public event EventHandler CheckedChanged = delegate { };
        internal void RaiseCheckedChangedEvent()
        {
            // Make sure that this control is enabled and has a specified delegate
            if (Enabled)
                this.CheckedChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Attach the CheckButton control to the dialog object.
        /// </summary>
        /// <param name="dialog">the target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogCheckBox.Attach: dialog parameter can not be null");
            
            // Add a check button control
            dialog.AddCheckButton(this.Id, this.Text, this.isChecked);

            // Make this control prominent if needed
            if (IsProminent)
                dialog.MakeProminent(this.Id);

            // Make sure this property is set
            ApplyPropertyChange("IsChecked");

            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();
        }
    }
}