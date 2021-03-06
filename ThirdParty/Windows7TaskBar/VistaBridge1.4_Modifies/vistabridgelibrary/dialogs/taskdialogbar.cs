// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Common class for all task dialog bar controls, such as the progress and marquee bars.
    /// </summary>
    public class TaskDialogBar : TaskDialogControl
    {
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public TaskDialogBar() {}
        /// <summary>
        /// Creates a new instance of this class with the specified name.
        /// </summary>
        /// <param name="name">The name for this control.</param>
        protected TaskDialogBar(string name) : base(name) { }

        private TaskDialogProgressBarState state;
        /// <summary>
        /// Gets or sets the state of the progress bar.
        /// </summary>
        public TaskDialogProgressBarState State
        {
            get { return state; }
            set 
            {
                CheckPropertyChangeAllowed("State");
                state = value;
                ApplyPropertyChange("State");
            }
        }
        /// <summary>
        /// Resets the state of the control to normal.
        /// </summary>
        protected internal virtual void Reset()
        {
            state = TaskDialogProgressBarState.Normal;
        }
    }
}