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
	/// A localized category attribute for displaying properties or events in a 
	/// visual designer.
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class CGCategoryAttribute : CategoryAttribute
	{
	
		// ******************************************************************
		// Attributes.
		// ******************************************************************

		#region Attributes

		/// <summary>
		/// Indentifies which class to load resources from.
		/// </summary>
		Type m_classType;

		#endregion

		// ******************************************************************
		// Constructors.
		// ******************************************************************
	
		#region Constructors

		/// <summary>
		/// Creates a new instance of the CGCategoryAttribute class.
		/// </summary>
		/// <param name="classType">Identifies which class to load resources from.</param>
		/// <param name="category">Identifies the unlocalized name of the catagory.</param>
		public CGCategoryAttribute(
			Type classType,
			string category
			) 
			: base(category)
		{
			m_classType = classType;
		} // End CGCategoryAttribute()

		#endregion

		// ******************************************************************
		// Protected methods.
		// ******************************************************************

		#region Protected methods

		/// <summary>
		/// Overridden in order to lookup up the localized name of a given category.  
		/// </summary>
		/// <param name="value">The unlocalized category name.</param>
		/// <returns>The localized category name.</returns>
		protected override string GetLocalizedString(
			string value
			)
		{

			return CGResource.GetString(
				m_classType,
				value
				);

		} // End GetLocalizedString()
 
		#endregion

	} // End class CGCategoryAttribute

} // End namespace CG.Core.Resource
