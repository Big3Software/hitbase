/*****************************************************************************
  Copyright © 2001 - 2005 by Martin Cook. All rights are reserved. If you like
  this code then feel free to go ahead and use it. The only thing I ask is 
  that you don't remove or alter my copyright notice. Your use of this 
  software is entirely at your own risk. I make no claims about the 
  reliability or fitness of this code for any particular purpose. If you 
  make changes or additions to this code then please clearly mark your code 
  as yours. If you have questions or comments then please contact me at: 
  martin@codegator.com
  
  Have Fun! :o)
*****************************************************************************/

#region Using directives

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

#endregion

namespace Big3.Hitbase.Miscellaneous
{

	/// <summary>
	/// Provides access to native WIN32 and COM stuff.
	/// </summary>
	[ComVisible(false)]
	internal class NativeMethods
	{

		// ******************************************************************
		// Attributes.
		// ******************************************************************

		#region Attributes

		public const int ICC_ANIMATE_CLASS = 0x00000080;
		public const int WM_NCHITTEST = 0x84;
		public const int ACM_OPENA = 0x464;
		public const int ACM_OPENW = 0x467;
		public const int ACM_PLAY = 0x465;
		public const int ACM_STOP = 0x466;
		public static readonly int ACM_OPEN;
		public const int ACN_START = 1;
		public const int ACN_STOP = 2;
		public const int ACS_AUTOPLAY = 4;
		public const int ACS_CENTER = 1;
		public const int ACS_TIMER = 8;
		public const int ACS_TRANSPARENT = 2;
		public const int HTCLIENT = 1;
		public const int HTBORDER = 0x12;

		#endregion

		// ******************************************************************
		// Constructors.
		// ******************************************************************

		#region Constructors

		static NativeMethods()
		{

			// Initialize message identifiers.

			if (Marshal.SystemDefaultCharSize == 1)
			{
				ACM_OPEN = ACM_OPENA;
			} // End if chars are 1 byte.
			else
			{
				ACM_OPEN = ACM_OPENW;
			} // End else chars are 2 bytes.

		} // End NativeMethods()

		// **************************************************************

		private NativeMethods()
		{

		} // End NativeMethods

		#endregion

		// ******************************************************************
		// PInvoke.
		// ******************************************************************

		#region PInvoke

		[DllImport("user32", CharSet=CharSet.Auto)]
		public static extern int SendMessage(HandleRef hWnd, int wMsg, int wParam, int lParam);

		[DllImport("user32", CharSet=CharSet.Auto)]
		public static extern int SendMessage(HandleRef hWnd, int wMsg, int wParam, IntPtr lParam);

		[DllImport("comctl32.dll")]
		public static extern void InitCommonControls();

		[DllImport("comctl32.dll")]
		public static extern bool InitCommonControls(INITCOMMONCONTROLSEX iccex);

		[DllImport("user32", CharSet=CharSet.Auto)]
		public static extern int SendMessage(HandleRef hWnd, int wMsg, int wParam, string lParam);

		[DllImport("kernel32", CharSet=CharSet.Auto)]
		public static extern int LoadLibraryEx(
			string lpLibFileName, 
			int hFile, 
			int dwFlags
			);

		[DllImport("kernel32", CharSet=CharSet.Auto)]
		public static extern int FreeLibrary(
			int hLibModule
			);

		#endregion

		// ******************************************************************
		// INITCOMMONCONTROLSEX.
		// ******************************************************************

		#region INITCOMMONCONTROLSEX

		/// <summary>
		/// Carries information used to load common control classes from the 
		/// dynamic-link library (DLL). This structure is used with the 
		/// InitCommonControlsEx function. 
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
			public class INITCOMMONCONTROLSEX 
		{
			
			public int dwSize;
			public int dwICC;

			public INITCOMMONCONTROLSEX()
			{
				this.dwSize = 8;
			} // End INITCOMMONCONTROLSEX()
 
			public INITCOMMONCONTROLSEX(
				int icc
				) 
				: this()
			{
				this.dwICC = icc;
			} // End INITCOMMONCONTROLSEX()
 
		} // End struct INITCOMMONCONTROLSEX

		#endregion

		// ******************************************************************
		// Public methods.
		// ******************************************************************

		#region Public methods

		public static int MakeLong(int lo, int hi)
		{
			return ((lo & 0xffff) | (((short) hi) << 0x10));
		} // End MakeLong()

		#endregion

	} // End class NativeMethods

} // End namespace CG.Animation
