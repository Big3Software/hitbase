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

#endregion

namespace Big3.Hitbase.Miscellaneous
{

	/// <summary>
	/// A localized description attribute for describing properties or 
	/// events in a visual designer.  
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class CGDescriptionAttribute : DescriptionAttribute
	{

		// ******************************************************************
		// Attributes.
		// ******************************************************************

		#region Attributes

		/// <summary>
		/// Flag used to indicate that the description has been replaced.
		/// </summary>
		private bool m_replaced;

		/// <summary>
		/// Indentifies which class to load resources from.
		/// </summary>
		Type m_classType;

		#endregion

		// ******************************************************************
		// Properties.
		// ******************************************************************

		#region Properties

		/// <summary>
		/// Overridden in order to get a localized description.
		/// </summary>
		public override string Description
		{
			get
			{

				// Should we replace the value?
				if (!m_replaced)
				{
					
					m_replaced = true;

					base.DescriptionValue = CGResource.GetString(
						m_classType,
						base.Description
						);

				} // End if we should replace the value.

				return base.Description;
			
			} // End get
		
		} // End Description
	
		#endregion

		// ******************************************************************
		// Constructors.
		// ******************************************************************

		#region Constructors

		/// <summary>
		/// Creates a new instance of the CGDescriptionAttribute class.
		/// </summary>
		/// <param name="classType">Identifies which class to load resources from.</param>
		/// <param name="description">The unlocalized description.</param>
		public CGDescriptionAttribute(
			Type classType,
			string description
			) 
			: base(description)
		{
			
			m_classType = classType;

		} // End ERDescriptionAttribute()
 
		#endregion

	} // End class CGDescriptionAttribute

} // End namespace CG.Core.Resource
