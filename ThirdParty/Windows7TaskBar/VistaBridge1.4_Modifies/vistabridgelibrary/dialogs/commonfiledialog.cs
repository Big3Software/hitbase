// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using Microsoft.SDK.Samples.VistaBridge.Interop;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// The abstract base class for the common file dialogs.
    /// </summary>
    [ContentProperty("Controls")]
    public abstract class CommonFileDialog : IDialogControlHost
    {
        /// <summary>
        /// The collection of names selected by the user.
        /// </summary>
        protected readonly Collection<string> fileNames;
        internal NativeDialogShowState showState = 
            NativeDialogShowState.PreShow;

        private IFileDialog nativeDialog;
        private IFileDialogCustomize customize;
        private NativeDialogEventSink nativeEventSink;
        private bool? canceled;
        private bool resetSelections;
        private Window parentWindow;

        /// <summary>
        /// Contains a common error message string shared by classes that 
        /// inherit from this class.
        /// </summary>
        protected const string IllegalPropertyChangeString = 
            " cannot be changed while dialog is showing";

        #region Constructors

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialog()
        {
            if (!Helpers.RunningOnVista)
                throw new PlatformNotSupportedException(
                    "Common File Dialog requires Windows Vista or later.");

            fileNames = new Collection<string>();
            filters = new CommonFileDialogFilterCollection();
            controls = new DialogControlCollection<CommonFileDialogControl>(this);
        }
        
		/// <summary>
        /// Creates a new instance of this class with the specified title.
        /// </summary>
        /// <param name="title">The title to display in the dialog.</param>
        protected CommonFileDialog(string title) : this()
        {
            this.title = title;
        }

        #endregion

        // Template method to allow derived dialog to create actual
        // specific COM coclass (e.g. FileOpenDialog or FileSaveDialog).
        internal abstract void InitializeNativeFileDialog();
        internal abstract IFileDialog GetNativeFileDialog();
        internal abstract void PopulateWithFileNames(
            Collection<string> names);
        internal abstract void CleanUpNativeFileDialog();
        internal abstract SafeNativeMethods.FOS GetDerivedOptionFlags(SafeNativeMethods.FOS flags);

        #region Public API

        // Events.
        /// <summary>
        /// Raised just before the dialog is about to return with a result.
        /// </summary>
        public event CancelEventHandler FileOk;
        /// <summary>
        /// Raised just before the user navigates to a new folder.
        /// </summary>
        public event EventHandler<CommonFileDialogFolderChangeEventArgs> FolderChanging;
        /// <summary>
        /// Raised when the user navigates to a new folder.
        /// </summary>
        public event EventHandler FolderChanged;
        /// <summary>
        /// Raised when the user changes the selection in the dialog's view.
        /// </summary>
        public event EventHandler SelectionChanged;
        /// <summary>
        /// Raised when the dialog is opened to notify the application of the initial chosen filetype.
        /// </summary>
        public event EventHandler FileTypeChanged;
        /// <summary>
        /// Raised when the dialog is opening.
        /// </summary>
        public event EventHandler Opening;

        private DialogControlCollection<CommonFileDialogControl> controls;
        /// <summary>
        /// Gets the collection of controls for the dialog.
        /// </summary>
        public DialogControlCollection<CommonFileDialogControl> Controls
        {
            get { return controls; }
        }

        private CommonFileDialogFilterCollection filters;
        /// <summary>
        /// Gets the filters used by the dialog.
        /// </summary>
        public CommonFileDialogFilterCollection Filters
        {
            get { return filters; }
        }

        private string title;
        /// <summary>
        /// Gets or sets the dialog title.
        /// </summary>
        /// <value>A <see cref="System.String"/> object.</value>
        public string Title
        {
            get { return title; }
            set 
            { 
                title = value;
                if (NativeDialogShowing)
                    nativeDialog.SetTitle(value);
            }
        }

        private bool addExtension;
        /// <summary>
        /// Gets or sets a value that determines whether the file extension
        /// set via the DefaultExtension property is appended to the file name.
        /// </summary>
        /// <value>A <see cref="System.Boolean"/> value.</value>
		/// 
        public bool AddExtension
        {
            get { return addExtension; }
            set { addExtension = value; }
        }

        // This will also work in Vista, but we need it in order to pick Libraries location
#if WIN7
        private bool forceFileSystem = true;

        /// <summary>
        /// Allow to use not only file system locations 
        /// </summary>
        public bool ForceFileSystem
        {
            get { return forceFileSystem; }
            set
            {
                ThrowIfDialogShowing("ForceFileSystem" + IllegalPropertyChangeString);
                forceFileSystem = value;
            }
        }
#endif

        // This is the first of many properties that are backed by the FOS_*
        // bitflag options set with IFileDialog.SetOptions(). 
        // SetOptions() fails
        // if called while dialog is showing (e.g. from a callback).
        private bool checkFileExists;
        /// <summary>
        /// Gets or sets a value to specify whether 
        /// the file returned must exist.
        /// </summary>
        /// <value>A <see cref="System.Boolean"/> value. <b>True</b> if the file must exist.</value>
        /// <exception cref="System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool CheckFileExists
        {
            get { return checkFileExists; }
            set 
            {
                ThrowIfDialogShowing("CheckFileExists" + IllegalPropertyChangeString);
                checkFileExists = value; 
            }
        }

        private bool checkPathExists;
        /// <summary>
        /// Gets or sets a value to specify whether the file returned 
        /// must be in an existing folder.
        /// </summary>
        /// <value>A <see cref="System.Boolean"/> value. <b>True</b> if the file must exist.</value>
        /// <exception cref="System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool CheckPathExists
        {
            get { return checkPathExists; }
            set 
            {
                ThrowIfDialogShowing("CheckPathExists" + IllegalPropertyChangeString);
                checkPathExists = value;
            }
        }

        private bool checkValidNames;
        /// <summary>Gets or sets a value that controls
        /// validation of file names.
        /// </summary>
        ///<value>A <see cref="System.Boolean"/> value. See remarks.</value>
        /// <exception cref="System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        /// <remarks>Specify <b>True </b>to check for situations that would prevent an application from opening the selected file, such as sharing violations or access denied errors.</remarks>
        public bool CheckValidNames
        {
            get { return checkValidNames; }
            set 
            {
                ThrowIfDialogShowing("CheckValidNames" + IllegalPropertyChangeString);
                checkValidNames = value; 
            }
        }

        private bool checkReadOnly;
        /// <summary>
        /// Gets or sets a value that determines 
        /// whether read-only items are returned.
        /// </summary>
        /// <value>A <see cref="System.Boolean"/> value. <b>True</b> includes read-only items.</value>
        /// <exception cref="System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool CheckReadOnly
        {
            get { return checkReadOnly; }
            set 
            {
                ThrowIfDialogShowing("CheckReadOnly" + IllegalPropertyChangeString);
                checkReadOnly = value; 
            }
        }

        private bool restoreDirectory;
        /// <summary>
        /// Corresponds to the FOS_NOCHANGEDIR 
        /// file dialog option.
        /// </summary>
        /// <remarks>According to the win32 documentation,
        /// this value is not used.d</remarks>
        /// <exception cref="System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool RestoreDirectory
        {
            get { return restoreDirectory; }
            set 
            {
                ThrowIfDialogShowing("RestoreDirectory" + IllegalPropertyChangeString);
                restoreDirectory = value; 
            }
        }

        private bool showPlacesList = true;
        /// <summary>
        /// Gets or sets a value that controls whether 
        /// to show or hide the list of pinned places that
        /// the user can choose from.
        /// </summary>
        /// <value>A <see cref="System.Boolean"/> value.</value>
        /// <exception cref="System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool ShowPlacesList
        {

            get { return showPlacesList; }
            set 
            {
                ThrowIfDialogShowing("ShowPlacesList" + IllegalPropertyChangeString);
                showPlacesList = value; 
            }
        }

        private bool addToMruList = true;
        /// <summary>
        /// Gets or sets a value that controls whether 
        /// to show or hide the list of places from which the 
        /// user has recently opened or saved items.
        /// </summary>
        /// <value>A <see cref="System.Boolean"/> value.</value>
        /// <exception cref="System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool AddToMruList
        {
            get { return addToMruList; }
            set 
            {
                ThrowIfDialogShowing("AddToMruList" + IllegalPropertyChangeString);
                addToMruList = value; 
            }
        }

        private bool showHiddenItems;
        ///<summary>
        /// Gets or sets a value that controls whether 
        /// to show hidden items.
        /// </summary>
        /// <value>A <see cref="System.Boolean"/> value.</value>
        /// <exception cref="System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool ShowHiddenItems
        {
            get { return showHiddenItems; }
            set 
            {
                ThrowIfDialogShowing("ShowHiddenItems" + IllegalPropertyChangeString);
                showHiddenItems = value; 
            }
        }
        private bool allowPropertyEditing;
        /// <summary>
        /// Gets or sets a value that controls whether 
        /// properties can be edited.
        /// </summary>
        /// <value>A <see cref="System.Boolean"/> value. </value>
        public bool AllowPropertyEditing
        {
            get { return allowPropertyEditing; }
            set { allowPropertyEditing = value; }
        }

        private bool dereferenceLinks;
        ///<summary>
        /// Gets or sets a value that controls whether 
        /// shortcuts should be treated as their target items,
        /// allowing an application to open a .lnk file.
        /// </summary>
        /// <value>A <see cref="System.Boolean"/> value. <b>True</b> indicates that shortcuts should be treated as their targets. </value>
        /// <exception cref="System.InvalidOperationException">This property cannot be set when the dialog is visible.</exception>
        public bool DereferenceLinks
        {
            get { return dereferenceLinks; }
            set 
            {
                ThrowIfDialogShowing("DereferenceLinks" + IllegalPropertyChangeString);
                dereferenceLinks = value; }
        }

        private string defaultExtension = null;

        /// <summary>
        /// Gets or sets the default file extension to be added to file names.
        /// </summary>
        public string DefaultExtension
        {
            get { return defaultExtension; }
            set
            {
                // Sanitize - null equals not set.
                if (String.IsNullOrEmpty(value))
                    value = null;

                defaultExtension = value;
            }
        }

        /// <summary>
        /// Tries to set the File(s) Type Combo to match the value in 
        /// 'defaultExtension'.  Only doing this if 'this' is a Save dialog 
        /// as it makes no sense to do this if only Opening a file.
        /// </summary>
        /// 
        /// <param name="dialog">The native/IFileDialog instance.</param>
        /// 
        private void SyncFileTypeComboToDefaultExtension(IFileDialog dialog)
        {
            // make sure it's a Save dialog and that there is a default 
            // extension to sync to.
            if (!(this is CommonSaveFileDialog) || defaultExtension == null ||
                filters.Count <= 0)
            {
                return;
            }

            // The native version of SetFileTypeIndex() requires an
            // unsigned integer as its parameter. This (having it be defined
            // as a uint right up front) avoids a cast, and the potential 
            // problems of casting a signed value to an unsigned one.
            uint filtersCounter = 0;

            CommonFileDialogFilter filter = null;

            for (filtersCounter = 0; filtersCounter < filters.Count; 
                filtersCounter++)
            {
                filter = (CommonFileDialogFilter)filters[(int)filtersCounter];

                if (filter.Extensions.Contains(defaultExtension))
                {
                    // set the docType combo to match this 
                    // extension. property is a 1-based index.
                    dialog.SetFileTypeIndex( filtersCounter + 1 );

                    // we're done, exit for
                    break;
                }
            }
            
        }

        /// <summary>
        /// Gets the selected filename.
        /// </summary>
        /// <value>A <see cref="System.String"/> object.</value>
        /// <exception cref="System.InvalidOperationException">This property cannot be used when multiple files are selected.</exception>
        public string FileName
        {
            get
            {
                CheckFileNamesAvailable();
                
                if (fileNames.Count > 1)
                    throw new InvalidOperationException("Multiple files selected - the FileNames property should be used instead.");

                string returnFilename = fileNames[0];

                // "If extension is a null reference (Nothing in Visual 
                // Basic), the returned string contains the specified 
                // path with its extension removed."  Since we do not want 
                // to remove any existing extension, make sure the 
                // defaultExtension property is NOT null.

                // if we should, and there is one to set...
                if (addExtension && !string.IsNullOrEmpty(defaultExtension))
                {
                    returnFilename = System.IO.Path.ChangeExtension(returnFilename, defaultExtension);
                }

                return returnFilename;
            }
        }

        // Null = use default directory.
        private string initialDirectory;
        /// <summary>
        /// Gets or sets the initial directory displayed when the 
        /// dialog is shown.
        /// </summary>
        /// <value>A <see cref="System.String"/> object.</value>
        public string InitialDirectory
        {
            get { return initialDirectory; }
            set { initialDirectory = value; }
        }

        // Null = use default identifier.
        private Guid usageIdentifier;
        /// <summary>
        /// Gets or sets a value that enables a calling application 
        /// to associate a GUID with a dialog's persisted state.
        /// </summary>
        public Guid UsageIdentifier
        {
            get { return usageIdentifier; }
            set { usageIdentifier = value; }
        }

        
        
        /// <summary>
        /// Displays the dialog.
        /// </summary>
        /// <returns>A <see cref="Microsoft.SDK.Samples.VistaBridge.Library.CommonFileDialogResult"/> object.</returns>
        public CommonFileDialogResult ShowDialog()
        {
            CommonFileDialogResult result;

            try
            {
                // Fetch derived native dialog (i.e. Save or Open).
                InitializeNativeFileDialog();
                nativeDialog = GetNativeFileDialog();

                // Apply outer properties to native dialog instance.
                ApplyNativeSettings(nativeDialog);
                InitializeEventSink(nativeDialog);

                // Clear user data if Reset has been called 
                // since the last show.
                if (resetSelections)
                {
                    resetSelections = false;
                }

                // Show dialog.
                showState = NativeDialogShowState.Showing;
                int hresult = nativeDialog.Show(
                    CommonFileDialog.GetHandleFromWindow(parentWindow));
                showState = NativeDialogShowState.Closed;

                // Create return information.
                if (ErrorHelper.Matches(hresult, 
                    Win32ErrorCode.ERROR_CANCELLED))
                {
                    canceled = true;
                    fileNames.Clear();
                }
                else
                {
                    canceled = false;

                    // Populate filenames if user didn't cancel.
                    PopulateWithFileNames(fileNames);
                }
                result = new CommonFileDialogResult(canceled.Value);
            }
            finally
            {
                CleanUpNativeFileDialog();
            }
            return result;
        }
        /// <summary>
        /// Removes the current selection.
        /// </summary>
        public void ResetUserSelections()
        {
            resetSelections = true;
        }
        

        #endregion

        #region Configuration

        private void InitializeEventSink(IFileDialog nativeDlg)
        {
            // Check if we even need to have a sink.
            if (FileOk != null
                || FolderChanging != null
                || FolderChanged != null
                || SelectionChanged != null
                || FileTypeChanged != null
                || Opening != null
                || (controls != null && controls.Count > 0))
            {
                uint cookie;
                nativeEventSink = new NativeDialogEventSink(this);
                nativeDlg.Advise(nativeEventSink, out cookie);
                nativeEventSink.Cookie = cookie;
            }
        }

        private void ApplyNativeSettings(IFileDialog dialog)
        {
            Debug.Assert(dialog != null, "No dialog instance to configure");

            if (parentWindow == null)
                parentWindow = Helpers.GetDefaultOwnerWindow();

            // Apply option bitflags.
            dialog.SetOptions(CalculateNativeDialogOptionFlags());

            // Other property sets.
            dialog.SetTitle(title);
            if (!String.IsNullOrEmpty(initialDirectory))
            {
                IShellItem initialDirectoryShellItem = Helpers.GetShellItemFromPath(initialDirectory);

                // If we get a real shell item back, 
                // then use that as the initial folder - otherwise,
                // we'll allow the dialog to revert to the default folder. 
                // (OR should we fail loudly?)
                if (initialDirectoryShellItem != null)
                    dialog.SetFolder(initialDirectoryShellItem);
            }

            // Apply file type filters, if available.
            if (filters.Count > 0)
            {
                dialog.SetFileTypes(
                    (uint)filters.Count,
                    filters.GetAllFilterSpecs());

                SyncFileTypeComboToDefaultExtension(dialog);
            }
                
            if (usageIdentifier != Guid.Empty)
                dialog.SetClientGuid(ref usageIdentifier);

        }

        private SafeNativeMethods.FOS CalculateNativeDialogOptionFlags()
        {
            // We start with only a few flags set by default, 
            // then go from there based on the current state
            // of the managed dialog's property values.
            SafeNativeMethods.FOS flags = 
                SafeNativeMethods.FOS.FOS_NOTESTFILECREATE
                | SafeNativeMethods.FOS.FOS_FORCEFILESYSTEM;

            // Call to derived (concrete) dialog to 
            // set dialog-specific flags.
            flags = GetDerivedOptionFlags(flags);

#if WIN7
            if (!forceFileSystem)
                flags &= ~SafeNativeMethods.FOS.FOS_FORCEFILESYSTEM;
#endif

            // Apply other optional flags.
            if (checkFileExists)
                flags |= SafeNativeMethods.FOS.FOS_FILEMUSTEXIST;
            if (checkPathExists)
                flags |= SafeNativeMethods.FOS.FOS_PATHMUSTEXIST;
            if (!checkValidNames)
                flags |= SafeNativeMethods.FOS.FOS_NOVALIDATE;
            if (!CheckReadOnly)
                flags |= SafeNativeMethods.FOS.FOS_NOREADONLYRETURN;
            if (restoreDirectory)
                flags |= SafeNativeMethods.FOS.FOS_NOCHANGEDIR;
            if (!showPlacesList)
                flags |= SafeNativeMethods.FOS.FOS_HIDEPINNEDPLACES;
            if (!addToMruList)
                flags |= SafeNativeMethods.FOS.FOS_DONTADDTORECENT;
            if (showHiddenItems)
                flags |= SafeNativeMethods.FOS.FOS_FORCESHOWHIDDEN;
            if (!dereferenceLinks)
                flags |= SafeNativeMethods.FOS.FOS_NODEREFERENCELINKS;
            return flags;
        }

        #endregion

        #region IDialogControlHost Members

        private static void GenerateNotImplementedException()
        {
            throw new NotImplementedException(
                "The method or operation is not implemented.");
        }
        
        bool IDialogControlHost.IsCollectionChangeAllowed()
        {
            return true;
        }

        void IDialogControlHost.ApplyCollectionChanged()
        {
            // Query IFileDialogCustomize interface before adding controls
            GetCustomizedFileDialog();
            // Populate all the custom controls and add them to the dialog
            foreach (CommonFileDialogControl control in controls)
            {
                if (!control.IsAdded)
                {
                    control.Attach(customize);
                    control.IsAdded = true;
                }
            }

        }

        bool IDialogControlHost.IsControlPropertyChangeAllowed(string propertyName, DialogControl control)
        {
            CommonFileDialog.GenerateNotImplementedException();
            return false;
        }

        void IDialogControlHost.ApplyControlPropertyChange(string propertyName, DialogControl control)
        {
            if (propertyName == "Text")
            {
                customize.SetControlLabel(control.Id, ((CommonFileDialogControl)control).Text);
            }
            else if (propertyName == "Enabled" || propertyName == "Visible")
            {
                CommonFileDialogControl dialogControl = control as CommonFileDialogControl;
                SafeNativeMethods.CDCONTROLSTATE state = SafeNativeMethods.CDCONTROLSTATE.CDCS_INACTIVE;
                
                if (dialogControl.Enabled == true)
                    state |= SafeNativeMethods.CDCONTROLSTATE.CDCS_ENABLED;
                
                if (dialogControl.Visible == true)
                    state |= SafeNativeMethods.CDCONTROLSTATE.CDCS_VISIBLE;
                
                customize.SetControlState(control.Id, state);
            }
            else if (propertyName == "SelectedIndex")
            {
                if (control is CommonFileDialogRadioButtonList)
                {
                    CommonFileDialogRadioButtonList list = control as CommonFileDialogRadioButtonList;
                    customize.SetSelectedControlItem(control.Id, list.SelectedIndex);
                }
                else if (control is CommonFileDialogComboBox)
                {
                    CommonFileDialogComboBox box = control as CommonFileDialogComboBox;
                    customize.SetSelectedControlItem(control.Id, box.SelectedIndex);
                }
            }
            else if (propertyName == "IsChecked")
            {
                if (control is CommonFileDialogCheckBox)
                {
                    CommonFileDialogCheckBox checkBox = control as CommonFileDialogCheckBox;
                    customize.SetCheckButtonState(control.Id, checkBox.IsChecked);
                }
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Ensures that the user has selected one or more files.
        /// </summary>
        /// <permission cref="System.InvalidOperationException">
        /// The dialog has not been dismissed yet -or-
        /// the dialog was canceled.</permission>
        protected void CheckFileNamesAvailable()
        {
            if (showState != NativeDialogShowState.Closed)
                throw new InvalidOperationException(
                    "Filename not available - dialog has not closed yet.");

            if (canceled.GetValueOrDefault())
                throw new InvalidOperationException(
                    "Filename not available - dialog was canceled.");

            Debug.Assert(fileNames.Count != 0,
              "FileNames empty - shouldn't happen unless dialog canceled or not yet shown.");
        }

        static IntPtr GetHandleFromWindow(Window window)
        {
            if (window == null)
                return SafeNativeMethods.NO_PARENT;

            return (new WindowInteropHelper(window)).Handle;
        }

        internal static bool IsOptionSet(IFileDialog dialog, SafeNativeMethods.FOS flag)
        {
            SafeNativeMethods.FOS currentFlags = GetCurrentOptionFlags(dialog);

            return (currentFlags & flag) == flag;
        }

        internal static SafeNativeMethods.FOS GetCurrentOptionFlags(IFileDialog dialog)
        {
            SafeNativeMethods.FOS currentFlags;
            dialog.GetOptions(out currentFlags);
            return currentFlags;
        }

        #endregion

        #region Helpers

        private bool NativeDialogShowing
        {
            get
            {
                return (nativeDialog != null)
                    && (showState == NativeDialogShowState.Showing ||
                    showState == NativeDialogShowState.Closing);
            }
        }

        internal static string GetFileNameFromShellItem(IShellItem item)
        {
            string filename;
            item.GetDisplayName(SafeNativeMethods.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out filename);
            return filename;
        }

        internal static IShellItem GetShellItemAt(IShellItemArray array, int i)
        {
            IShellItem result;
            uint index = (uint)i;
            array.GetItemAt(index, out result);
            return result;
        }

        /// <summary>
        /// Throws an exception when the dialog is showing preventing
        /// a requested change to a property or the visible set of controls.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        /// <permission cref="System.InvalidOperationException"> The dialog is in an
        /// invalid state to perform the requested operation.</permission>
        protected void ThrowIfDialogShowing(string message)
        {
            if (NativeDialogShowing)
                throw new InvalidOperationException(message);
        }
        /// <summary>
        /// Get the IFileDialogCustomize interface, preparing to add controls.
        /// </summary>
        private void GetCustomizedFileDialog()
        {
            if (customize == null)
            {
                if (nativeDialog == null)
                {
                    InitializeNativeFileDialog();
                    nativeDialog = GetNativeFileDialog();
                }
                customize = (IFileDialogCustomize)nativeDialog;
            }
        }
        #endregion


        #region CheckChanged handling members
        /// <summary>
        /// Called just before the dialog is about to return with a result.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnFileOk(CancelEventArgs e)
        {
            CancelEventHandler handler = FileOk;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// Called before <see cref="Microsoft.SDK.Samples.VistaBridge.Library.CommonFileDialog.OnFolderChanged"/>. This allows the implementer to stop navigation to a particular location.
        /// </summary>
        /// <param name="e">Cancelable event arguments.</param>
        protected virtual void OnFolderChanging(CommonFileDialogFolderChangeEventArgs e)
        {
            EventHandler<CommonFileDialogFolderChangeEventArgs> handler = FolderChanging;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// Called when the user navigates to a new folder.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnFolderChanged(EventArgs e)
        {
            EventHandler handler = FolderChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// Called when the user changes the selection in the dialog's view.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            EventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// Called when the dialog is opened to notify the 
        /// application of the initial chosen filetype.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnFileTypeChanged(EventArgs e)
        {
            EventHandler handler = FileTypeChanged;
            if (handler != null)
            {
                handler(this, e);
            }   
        }
        /// <summary>
        /// Called when the dialog is opened.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnOpening(EventArgs e)
        {
            EventHandler handler = Opening;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region NativeDialogEventSink Nested Class

        private class NativeDialogEventSink : IFileDialogEvents, IFileDialogControlEvents
        {
            private CommonFileDialog parent;
            private bool firstFolderChanged = true; 

            public NativeDialogEventSink(CommonFileDialog commonDialog)
            {
                this.parent = commonDialog;
            }

            private uint cookie;
            public uint Cookie
            {
                get { return cookie; }
                set { cookie = value; }
            }
	
            public HRESULT OnFileOk(IFileDialog pfd)
            {
                CancelEventArgs args = new CancelEventArgs();
                parent.OnFileOk(args);
                return (args.Cancel ? HRESULT.S_FALSE : HRESULT.S_OK);
            }

            public HRESULT OnFolderChanging(IFileDialog pfd, IShellItem psiFolder)
            {
                CommonFileDialogFolderChangeEventArgs args =
                    new CommonFileDialogFolderChangeEventArgs(CommonFileDialog.GetFileNameFromShellItem(psiFolder));
                if (!firstFolderChanged)
                    parent.OnFolderChanging(args);
                return (args.Cancel ? HRESULT.S_FALSE : HRESULT.S_OK);
            }

            public void OnFolderChange(IFileDialog pfd)
            {
                if (firstFolderChanged)
                {
                    firstFolderChanged = false;
                    parent.OnOpening(EventArgs.Empty);
                }
                else
                    parent.OnFolderChanged(EventArgs.Empty);
            }

            public void OnSelectionChange(IFileDialog pfd)
            {
                parent.OnSelectionChanged(EventArgs.Empty);
            }

            public void OnShareViolation(
                IFileDialog pfd, 
                IShellItem psi, 
                out SafeNativeMethods.FDE_SHAREVIOLATION_RESPONSE pResponse)
            {
                // Do nothing: we will ignore share violations, 
                // and don't register
                // for them, so this method should never be called.
                pResponse = SafeNativeMethods.FDE_SHAREVIOLATION_RESPONSE.FDESVR_ACCEPT;
            }

            public void OnTypeChange(IFileDialog pfd)
            {
                parent.OnFileTypeChanged(EventArgs.Empty);
            }

            public void OnOverwrite(IFileDialog pfd, IShellItem psi, out SafeNativeMethods.FDE_OVERWRITE_RESPONSE pResponse)
            {
                pResponse = SafeNativeMethods.FDE_OVERWRITE_RESPONSE.FDEOR_ACCEPT;
            }
            
            public void OnItemSelected(IFileDialogCustomize pfdc, int dwIDCtl, int dwIDItem)
            {
                // Find control
                DialogControl control = this.parent.controls.GetControlbyId(dwIDCtl);

                // Process ComboBox and/or RadioButtonList
                if (control is ICommonFileDialogIndexedControls)
                {
                    // Update selected item and raise SelectedIndexChanged event
                    ICommonFileDialogIndexedControls controlInterface = control as ICommonFileDialogIndexedControls;
                    controlInterface.SelectedIndex = dwIDItem;
                    controlInterface.RaiseSelectedIndexChangedEvent();
                }
                // Process Menu
                else if (control is CommonFileDialogMenu)
                {
                    CommonFileDialogMenu menu = control as CommonFileDialogMenu;

                    // Find the menu item that was clicked and invoke it's click event
                    foreach (CommonFileDialogMenuItem item in menu.Items)
                    {
                        if (item.Id == dwIDItem)
                        {
                            item.RaiseClickEvent();
                            break;
                        }
                    }
                }
            }

            public void OnButtonClicked(IFileDialogCustomize pfdc, int dwIDCtl)
            {
                // Find control
                DialogControl control = this.parent.controls.GetControlbyId(dwIDCtl);

                // Call corresponding event
                if (control is CommonFileDialogButton)
                {
                    ((CommonFileDialogButton)control).RaiseClickEvent();
                }
            }

            public void OnCheckButtonToggled(IFileDialogCustomize pfdc, int dwIDCtl, bool bChecked)
            {
                // Find control
                DialogControl control = this.parent.controls.GetControlbyId(dwIDCtl);

                // Update control and call corresponding event
                if (control is CommonFileDialogCheckBox)
                {
                    CommonFileDialogCheckBox box = control as CommonFileDialogCheckBox;
                    box.IsChecked = bChecked;
                    box.RaiseCheckedChangedEvent();
                }
            }

            public void OnControlActivating(IFileDialogCustomize pfdc, int dwIDCtl)
            {
            }
        }

        #endregion

        #region Add Custom Controls
        /// <summary>
        /// Start the visual group on the dialog
        /// </summary>
        /// <param name="text"></param>
        public void StartVisualGroup(string text)
        {
            GetCustomizedFileDialog(); 
            CommonFileDialogLabel control = new CommonFileDialogLabel(text);
            customize.StartVisualGroup(control.Id, text);
        }
        /// <summary>
        /// End the visual group on the dialog
        /// </summary>
        public void EndVisualGroup()
        {
            GetCustomizedFileDialog();
            customize.EndVisualGroup();
        }
        #endregion
    }

    /// <summary>
    /// Specifies a property, event and method that indexed controls need
    /// to implement.
    /// </summary>
    /// 
    /// <remarks>
    /// not sure where else to put this, so leaving here for now.
    /// </remarks>
    interface ICommonFileDialogIndexedControls
    {
        int SelectedIndex
        {
            get;
            set;
        }

        event EventHandler SelectedIndexChanged;

        void RaiseSelectedIndexChangedEvent();
    }

}