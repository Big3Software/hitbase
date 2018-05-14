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
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Permissions;

#endregion

namespace Big3.Hitbase.Miscellaneous
{
   
	/// <summary>
	/// A managed wrapper for the animation common control.
	/// </summary>
	[DefaultProperty("FileName")]
	[ToolboxBitmap(typeof(CGAnimation), "CGAnimation.bmp")]
	public class CGAnimation : Control
	{

		// ******************************************************************
		// Attributes.
		// ******************************************************************

		#region Attributes

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Indicates that the control should play automatically.
		/// </summary>
		private bool m_autoPlay = true;

		/// <summary>
		/// Indicates that the control should automtically center the
		/// animation.
		/// </summary>
		private bool m_autoCenter = true;

		/// <summary>
		/// Indicates that the control should use a transparent color.
		/// </summary>
		private bool m_isTransparent = true;

		/// <summary>
		/// Indicates that the control is currently playing.
		/// </summary>
		private bool m_isPlaying;

		/// <summary>
		/// Indicates that an animation file is currently open.
		/// </summary>
		private bool m_isOpen;

		/// <summary>
		/// Indicates that the control should use a timer to synchronize
		/// the animation rather than a background thread.
		/// </summary>
		private bool m_useTimer;

		/// <summary>
		/// The path to the current animation.
		/// </summary>
		private string m_fileName;		

		/// <summary>
		/// The identifier of the current animation.
		/// </summary>
		private CGAVIFileType m_aviFileType;

		/// <summary>
		/// The handle to the shell32 library.
		/// </summary>
		private int m_hShellModule;

		#endregion

		// ******************************************************************
		// Properties.
		// ******************************************************************

		#region Properties

		/// <summary>
		/// Overridden in order to hide the property from the visual designer.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage
		{
			get {return base.BackgroundImage;}
			set	{base.BackgroundImage = value;}
		} // End BackgroundImage

		// ******************************************************************

		/// <summary>
		/// Indicates that the control is currently playing.
		/// </summary>
		[Browsable(false)]
		public bool IsPlaying
		{
			get {return m_isPlaying;}
		} // End IsPlaying

		// ******************************************************************

		/// <summary>
		/// Indicates that an animation is currently open.
		/// </summary>
		[Browsable(false)]
		public bool IsOpen
		{
			get {return m_isOpen;}
		} // End IsOpen

		// ******************************************************************

		/// <summary>
		/// Gets and sets the background color for the control.
		/// </summary>
		public override Color BackColor
		{
						
			get {return base.BackColor;}
			
			set
			{
				
				// If nothing has changed then simply exit.
				if (base.BackColor == value)
					return;

				// Save the changes.
				base.BackColor = value;

				// Must recreate the control in order for the changes
				//   to have any effect.
				RecreateHandle();

			} // End set
		
		} // End BackColor

		// ******************************************************************

		/// <summary>
		/// Indicates that the control should play the animation with a 
		/// transparent background.
		/// </summary>
		[CGDescription(typeof(CGAnimation), "transparent_desc")]
		[CGCategory(typeof(CGAnimation), "behavior_cat")]
		[DefaultValue(true)]
		public bool Transparent
		{
			
			get {return m_isTransparent;}
			
			set
			{
				
				// If nothing has changed then simply exit.
				if (m_isTransparent == value)
					return;

				// Save the changes.
				m_isTransparent = value;
				
				// Must recreate the control in order for the changes
				//   to have any effect.
				RecreateHandle();

			} // End set
		
		} // End Transparent

		// ******************************************************************

		/// <summary>
		/// Indicates whether the control should use a timer to synchronize 
		/// the animation playback. When set, the control plays the clip 
		/// without creating a thread.
		/// </summary>
		[CGDescription(typeof(CGAnimation), "use_timer_desc")]
		[CGCategory(typeof(CGAnimation), "behavior_cat")]
		[DefaultValue(false)]
		public bool UseTimer
		{
			
			get {return m_useTimer;}
			
			set
			{

				// If nothing has changed then simply exit.
				if (m_useTimer == value)
					return;
				
				// Save the changes.
				m_useTimer = value;

				// Must recreate the control in order for the changes
				//   to have any effect.
				RecreateHandle();

			} // End set
		
		} // End UseTimer

		// ******************************************************************

		/// <summary>
		/// Indicates that the animation should automtically center itself in
		/// the control.
		/// </summary>
		[CGDescription(typeof(CGAnimation), "auto_center_desc")]
		[CGCategory(typeof(CGAnimation), "behavior_cat")]
		[DefaultValue(true)]
		public bool AutoCenter
		{

			get {return m_autoCenter;}
			
			set
			{
				
				// If nothing has changed then simply exit.
				if (m_autoCenter == value)
					return;

				// Save the changes.
				m_autoCenter = value;
				
				// Must recreate the control in order for the changes
				//   to have any effect.
				RecreateHandle();

				// Must force a resize in order to center the animation.
				if (m_autoCenter)
				{
					Size = new Size(Size.Width - 1, Size.Height - 1);
					Size = new Size(Size.Width + 1, Size.Height + 1);
				} // End if we should center the animation.
                
			} // End set

		} // End AutoCenter

		// ******************************************************************

		/// <summary>
		/// Indicates that the control should start playing an animation as
		/// soon as one is opened.
		/// </summary>
		[CGDescription(typeof(CGAnimation), "auto_play_desc")]
		[CGCategory(typeof(CGAnimation), "behavior_cat")]
		[DefaultValue(true)]
		public bool AutoPlay
		{
		
			get {return m_autoPlay;}
			
			set
			{
				
				// If nothing has changed then simply exit.
				if (m_autoPlay == value)
					return;
				
				// Save the changes.
				m_autoPlay = value;
				
				// Must recreate the control in order for the changes
				//   to have any effect.
				RecreateHandle();
			
			} // End set
		
		} // End AutoPlay

		// ******************************************************************

		/// <summary>
		/// Gets and sets the filename of the current animation.
		/// </summary>
		[CGDescription(typeof(CGAnimation), "file_name_desc")]
		[CGCategory(typeof(CGAnimation), "behavior_cat")]
		[DefaultValue("")]
		public string FileName
		{

			get {return m_fileName;}

			set
			{

				// If nothing has changed then simply exit.
				if (m_fileName == value)
					return;

				// Save the changes.
				m_fileName = value;
				m_aviFileType = CGAVIFileType.ExternalFile;
                
				// Should we stop the current animation?
				if (IsPlaying)
					Stop();

				// Should we close the current animation?
				if (IsOpen)
					Close();

				// Should we automatically play the animation?
				if (AutoPlay)
					Open();
                
			} // End set

		} // End FileName

		// ******************************************************************

		/// <summary>
		/// Gets or sets the type of animation currently open.
		/// </summary>
		[CGDescription(typeof(CGAnimation), "animation_type_desc")]
		[CGCategory(typeof(CGAnimation), "behavior_cat")]
		[DefaultValue(CGAVIFileType.ExternalFile)]
		public CGAVIFileType AVIFileType
		{

			get {return m_aviFileType;}

			set
			{

				// If nothing has changed then simply exit.
				if (m_aviFileType == value)
					return;

				// Save the changes.
				m_aviFileType = value;
				m_fileName = "";
                
				// Should we stop the current animation?
				if (IsPlaying)
					Stop();

				// Should we close the current animation?
				if (IsOpen)
					Close();

				// Should we automatically play the animation?
				if (AutoPlay)
					Open();

			} // End set

		} // End AVIFileType

		#endregion

		// ******************************************************************
		// Constructors.
		// ******************************************************************

		#region Constructors

		/// <summary>
		/// Creates a new instance of the CGAnimation class.
		/// </summary>
		public CGAnimation()
		{
			
			// No reason to be a tab-stop.
			TabStop = false;

			// Setup the appropriate styles.
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.UserPaint, false);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, false);

		} // End CGAnimation()

		#endregion

		// ******************************************************************
		// Overrides.
		// ******************************************************************

		#region Overrides

		/// <summary>
		/// Gets the default size for the control.
		/// </summary>
		protected override Size DefaultSize
		{
			get {return new Size(120, 60);}
		} // End DefaultSize

        // ******************************************************************
        
		/// <summary>
		/// Gets the required creation parameters when the control handle is 
		/// created.  
		/// </summary>
		protected override CreateParams CreateParams
		{

			get
			{
				
				// Create the parameters for the animation control.

				CreateParams parms = base.CreateParams;
				parms.ClassName = "SysAnimate32";

				// Apply the appropriate animation control styles.

				if (AutoCenter)
					parms.Style |= NativeMethods.ACS_CENTER;

				if (AutoPlay)
					parms.Style |= NativeMethods.ACS_AUTOPLAY;

				if (Transparent)
					parms.Style |= NativeMethods.ACS_TRANSPARENT;

				if (UseTimer)
					parms.Style |= NativeMethods.ACS_TIMER;

				return parms;

			} // End get

		} // End CreateParms()

		// ******************************************************************

		/// <summary>
		/// Creates a handle for the control.  
		/// </summary>
		protected override void CreateHandle()
		{
			
			// Should we make sure the common control library is initialized?
			if (!RecreatingHandle)
			{

				
				NativeMethods.INITCOMMONCONTROLSEX iccex = 
					new NativeMethods.INITCOMMONCONTROLSEX(
						NativeMethods.ICC_ANIMATE_CLASS
					);

				NativeMethods.InitCommonControls(iccex);

			} // End if we should init the common control library.
			
			base.CreateHandle();

		} // End CreateHandle()

		// ******************************************************************

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{

			// Make sure the animation is stopped and closed.
			Close();
			
			// Should we cleanup the library?
			if (m_hShellModule != 0)
			{
				NativeMethods.FreeLibrary(m_hShellModule);
				m_hShellModule = 0;
			} // End if we should free the library handle.
			
			// Should we dispose of the container?
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);

		} // End Dispose()

		// ******************************************************************

		/// <summary>
		/// Raises the HandleCreated event.  
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnHandleCreated(
			EventArgs e
			)
		{
		
			// Should we reopen the animation?
			bool shouldOpen = IsOpen;

			// After the control is recreated it will not be open or playing.
			m_isPlaying = false;
			m_isOpen = false;
            
			// Recreate the control.
			base.OnHandleCreated(e);

			// Should we reopen a previous animation?
			if (shouldOpen || AutoPlay)
				Open();

		} // End OnHandleCreated() 

		// ******************************************************************
		
		/// <summary>
		/// Processes Windows messages.  
		/// </summary>
		/// <param name="m">The message to process. </param>
		protected override void WndProc(
			ref Message m
			)
		{
			
			// Should we respond to hittesting?
			if (m.Msg == NativeMethods.WM_NCHITTEST)
			{
				
				Point point = base.PointToClient(
					new Point(
					m.LParam.ToInt32() & 0xffff, 
					m.LParam.ToInt32() >> 0x10
					)
					);
				
				if (base.ClientRectangle.Contains(point))
					m.Result = new IntPtr(NativeMethods.HTCLIENT);
				else
					m.Result = new IntPtr(NativeMethods.HTBORDER);

			} // End if
			else
				base.WndProc(ref m);

		} // End WndProc()

		#endregion

		// ******************************************************************
		// Component Designer generated code.
		// ******************************************************************

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		// ******************************************************************
		// Public methods.
		// ******************************************************************

		#region Public methods

		/// <summary>
		/// Opens the animation specified by the AVIFileType and FileName
		/// properties.
		/// </summary>
		public void Open()
		{

			if (DesignMode)
				return;

			if (AVIFileType != CGAVIFileType.ExternalFile)
				OpenHelper(m_aviFileType);
			else
				OpenHelper(m_fileName);

		} // End Open()		
		
		// ******************************************************************

		/// <summary>
		/// Closes the current animation.
		/// </summary>
		public void Close()
		{
			
			// Should we bother closing?
			if (!IsHandleCreated || !IsOpen)
				return;

			// Stop any currently playing animation.
			Stop();

			// At this point the animation is closed.
			m_isOpen = false;

			// Tell the windows control to close. Yea, it's strange to
			//   send ACM_OPEN in order to close the animation but whatta 
			//   ya gunna do?
			NativeMethods.SendMessage(
				new HandleRef(this, Handle),
				NativeMethods.ACM_OPEN, 
				0, 
				IntPtr.Zero
				);

			// We do not need to recreate the control, we can simply redraw it.
			Invalidate();
		
		} // End Close()

		// ******************************************************************

		/// <summary>
		/// Stops the currently playing animation.
		/// </summary>
		public void Stop()
		{

			// If the control isn't playing then simply exit.
			if (!IsHandleCreated || !IsOpen)
                return;

			// Direct the control to stop playing.
			NativeMethods.SendMessage(
				new HandleRef(this, Handle),
				NativeMethods.ACM_STOP, 
				0, 
				IntPtr.Zero
				);

			// At this point the control is no longer playing.
			m_isPlaying = false;

		} // End Stop()

		// ******************************************************************

		/// <summary>
		/// Play the currently open animation from start to finish in a 
		/// never ending loop.
		/// </summary>
		public void Play()
		{
			PlayHelper(-1, 0, -1);
		} // End Play()

		// ******************************************************************

		/// <summary>
		/// Plays the currently open animation from start to finish the 
		/// specified number of times.
		/// </summary>
		/// <param name="repetitions">The number of times to play the 
		/// animation.</param>
		public void Play(
			int repetitions
			)
		{
			PlayHelper(repetitions, 0, -1);
		} // End Play()

		// ******************************************************************
        
		/// <summary>
		/// Plays the currently open animation.
		/// </summary>
		/// <param name="repetitions">The number of times to play the 
		/// animation.</param>
		/// <param name="startFrame">The starting frame.</param>
		/// <param name="endFrame">The ending frame.</param>
		public void Play(
			int repetitions, 
			int startFrame, 
			int endFrame
			)
		{
			PlayHelper(repetitions, startFrame, endFrame);
		} // End Play()

		// ******************************************************************

		/// <summary>
		/// Seeks to a new position in the currently open animation.
		/// </summary>
		/// <param name="frame">The frame to seek to.</param>
		public void Seek(
			int frame
			)
		{
			PlayHelper(1, frame, frame);
		} // End Seek()

		#endregion

		// ******************************************************************
		// Private methods.
		// ******************************************************************

		#region Private methods

		/// <summary>
		/// Plays the currently open animation.
		/// </summary>
		/// <param name="repetitions"></param>
		/// <param name="startFrame"></param>
		/// <param name="endFrame"></param>
		private void PlayHelper(
			int repetitions, 
			int startFrame, 
			int endFrame
			)
		{

			// If the control hasn't been created or isn't open then 
			//   simply exit.
			if (!IsHandleCreated || !IsOpen)
				return;

			// Did the animation start playing?
			m_isPlaying = (NativeMethods.SendMessage(
				new HandleRef(this, Handle),
				NativeMethods.ACM_PLAY, 
				repetitions, 
				NativeMethods.MakeLong(
					Math.Min(startFrame, 0xffff), 
					Math.Min(endFrame, 0xffff)
					)
				) != 0);

		} // End PlayHelper()

		// ******************************************************************

		/// <summary>
		/// Opens an external AVI file.
		/// </summary>
		/// <param name="fileName">The path to an external AVI file.</param>
		private void OpenHelper(
			string fileName
			)
		{

			// If the control hasn't been created then simply exit.
			if (!IsHandleCreated)
				return;

			// Close any currently open animation.
			Close();
            		
			// Did the animation open?
			m_isOpen = (NativeMethods.SendMessage(
				new HandleRef(this, Handle),
				NativeMethods.ACM_OPEN, 
				0, 
				fileName
				) != 0);

			// Should we automtically play the animation?
			if (AutoPlay)
				Play();

		} // End OpenHelper()

		// ******************************************************************

		/// <summary>
		/// Opens an external AVI file.
		/// </summary>
		/// <param name="animationType">The identifier of a standard 
		/// animation.</param>
		private void OpenHelper(
			CGAVIFileType animationType
			)
		{

			// If the control hasn't been created then simply exit.
			if (!IsHandleCreated)
				return;

			// Close any currently open animation.
			Close();
			
			// Should we load the shell library?
			if (m_hShellModule == 0)
				m_hShellModule = NativeMethods.LoadLibraryEx(
					"shell32.dll", 
					0, 
					2
					);
			
			// Did the animation open?
			m_isOpen = (NativeMethods.SendMessage(
				new HandleRef(this, Handle),
				NativeMethods.ACM_OPEN, 
				m_hShellModule, 
				(int)animationType
				) != 0);

			// Should we automtically play the animation?
			if (AutoPlay)
				Play();

		} // End OpenHelper()

		#endregion
        
	} // End class CGAnimation

} // End namespace CG.Animation
