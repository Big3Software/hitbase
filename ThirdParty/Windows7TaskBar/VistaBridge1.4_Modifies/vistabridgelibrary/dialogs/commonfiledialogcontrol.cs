// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using Microsoft.SDK.Samples.VistaBridge.Interop;
using System.Windows.Markup;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Abstract class that supports shared functionality for 
    /// common file dialog controls.
    /// </summary>
    public abstract class CommonFileDialogControl : DialogControl
    {
        /// <summary>
        /// Holds the text that will be displayed for this control.
        /// </summary>
        protected string textValue;
        
        /// <summary>
        /// The text string that shows on the control
        /// </summary>
        public virtual string Text
        {
            get { return textValue; }
            set
            {
                // Don't update this property if it hasn't changed
                if (string.Compare(value, textValue) == 0)
                    return;

                textValue = value;
                ApplyPropertyChange("Text");
            }
        }

        private bool enabled = true;
        /// <summary>
        /// Is this control enabled.  Disabled controls are grayed out.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                // Don't update this property if it hasn't changed
                if (value == enabled)
                    return;
                
                enabled = value;
                ApplyPropertyChange("Enabled");
            }
        }

        private bool visible = true;
        /// <summary>
        /// Gets or sets a boolean value that indicates whether (true) or 
        /// not (false) this control is visible.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set
            {
                // Don't update this property if it hasn't changed
                if (value == visible)
                    return;

                visible = value;
                ApplyPropertyChange("Visible");
            }
        }

        private bool isAdded = false;
        /// <summary>
        /// Has this control been added to the dialog
        /// </summary>
        internal bool IsAdded
        {
            get { return isAdded; }
            set { isAdded = value; }
        }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogControl() : base()
        {
        }

        /// <summary>
        /// Create a new instance of this class with text only.
        /// </summary>
        /// <param name="text">the text of common file dialog control</param>
        protected CommonFileDialogControl(string text) : base() 
        {
            this.textValue = text;
        }

        /// <summary>
        /// Creates a new instance of this class using the specified name and text.
        /// </summary>
        /// <param name="name">the name of common file dialog control</param>
        /// <param name="text">the text of common file dialog control</param>
        protected CommonFileDialogControl(string name, string text) : base(name) 
        {
            this.textValue = text;
        }
        
        /// <summary>
        /// Attach the custom control itself to the specified dialog
        /// </summary>
        /// <param name="dialog">the target dialog</param>
        internal abstract void Attach(IFileDialogCustomize dialog);

        internal virtual void SyncUnmanagedProperties()
        {
            ApplyPropertyChange("Enabled");
            ApplyPropertyChange("Visible");
        }
    }
}