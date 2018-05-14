// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.SDK.Samples.VistaBridge.Interop
{
    /// <summary>
    /// Base class for Safe handles with Null IntPtr as invalid
    /// </summary>
    internal abstract class ZeroInvalidHandle : SafeHandle
    {
        public ZeroInvalidHandle()
            : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

    }

    /// <summary>
    /// Safe Window Handle
    /// </summary>
    internal class SafeWindowHandle : ZeroInvalidHandle 
    {
        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
            {
                return true;
            }

            if (NativeMethods.DestroyWindow(handle) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    /// <summary>
    /// Safe Region Handle
    /// </summary>
    internal class SafeRegionHandle : ZeroInvalidHandle
    {

        protected override bool ReleaseHandle()
        {
            if ( NativeMethods.DeleteObject(handle))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Safe Icon Handle
    /// </summary>
    internal class SafeIconHandle : ZeroInvalidHandle
    {
        protected override bool ReleaseHandle()
        {
            if (NativeMethods.DestroyIcon(handle))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}