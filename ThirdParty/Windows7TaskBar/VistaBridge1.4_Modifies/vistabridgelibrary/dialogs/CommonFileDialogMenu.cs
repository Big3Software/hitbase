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
    /// Common class for all Menu controls on Common File Dialog
    /// </summary>
    [ContentProperty("Items")]
    public class CommonFileDialogMenu : CommonFileDialogProminentControl
    {
        /// <summary>
        /// Gets the collection of CommonFileDialogMenuItem objects
        /// </summary>
        private Collection<CommonFileDialogMenuItem> items;
        public Collection<CommonFileDialogMenuItem> Items
        {
            get { return items; }
        }

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        public CommonFileDialogMenu()
            : base()
        {
            Initialize();
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text
        /// </summary>
        /// <param name="text">Text to display for this control</param>
        public CommonFileDialogMenu(string text)
            : base(text)
        {
            Initialize();
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name and text
        /// </summary>
        /// <param name="name">The name of this control</param>
        /// <param name="text">Text to display for this control</param>
        public CommonFileDialogMenu(string name, string text)
            : base(name, text)
        {
            Initialize();
        }
        
        /// <summary>
        /// Initializes the item collection for this class
        /// </summary>
        private void Initialize()
        {
            items = new Collection<CommonFileDialogMenuItem>();
        }

        /// <summary>
        /// Attach the Menu control to the dialog object.
        /// </summary>
        /// <param name="dialog">the target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogMenu.Attach: dialog parameter can not be null");
            
            // Add the menu control
            dialog.AddMenu(this.Id, this.Text);
            
            // Add the menu items
            foreach (CommonFileDialogMenuItem item in this.items)
                dialog.AddControlItem(this.Id, item.Id, item.Text);
            
            // Make prominent as needed
            if (IsProminent)
                dialog.MakeProminent(this.Id);

            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();
        }
    }

    /// <summary>
    /// Common class for the menu items for the Common File Dialog
    /// </summary>
    public class CommonFileDialogMenuItem : CommonFileDialogControl
    {
        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        public CommonFileDialogMenuItem()
            : base(String.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text
        /// </summary>
        /// <param name="text">Text to display for this control</param>
        public CommonFileDialogMenuItem(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Called when a menu item is clicked
        /// </summary>
        public event EventHandler Click = delegate { };
        internal void RaiseClickEvent()
        {
            // Make sure that this control is enabled and has a specified delegate
            if (Enabled)
                Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Attach this control to the dialog object
        /// </summary>
        /// <param name="dialog">Target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            // Items are added via the menu itself
        }
    }
}