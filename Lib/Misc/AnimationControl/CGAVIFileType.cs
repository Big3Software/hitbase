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

#endregion

namespace Big3.Hitbase.Miscellaneous
{
	/// <summary>
	/// Identifies various types of standard animations.
	/// </summary>
	public enum CGAVIFileType
	{

		/// <summary>
		/// An external AVI file.
		/// </summary>
		ExternalFile = 0,

		/// <summary>
		/// The "search for folder" AVI file.
		/// </summary>
		Search4Folder = 150,

		/// <summary>
		/// The "search for computer" AVI file.
		/// </summary>
		Search4Computer = 152,

		/// <summary>
		/// The "seach for file" AVI file.
		/// </summary>
		Search4File = 151,

		/// <summary>
		/// The "search the internet" AVI file.
		/// </summary>
		SearchWeb = 166,

		/// <summary>
		/// The "copy settings" AVI file.
		/// </summary>
		CopySettings = 165,

		/// <summary>
		/// The "copy file" AVI file.
		/// </summary>
		CopyFile = 161,

		/// <summary>
		/// The "delete file" AVI file.
		/// </summary>
		DeleteFile = 164,		

		/// <summary>
		/// The "move file" AVI file.
		/// </summary>
		MoveFile = 160,

		/// <summary>
		/// The "delete to recycle bin" AVI file.
		/// </summary>
		Delete2RecycleBin = 162,

		/// <summary>
		/// The "clean the recycle bin" AVI file.
		/// </summary>
		CleanRecycleBin = 163

	} // End CGFileType

} // End namespace CG.Animation
