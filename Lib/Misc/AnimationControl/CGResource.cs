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
using System.Resources;
using System.Globalization;

#endregion

namespace Big3.Hitbase.Miscellaneous
{

	/// <summary>
	/// A class utility containing various resource reading routines.
	/// </summary>
	internal sealed class CGResource
	{

		// ******************************************************************
		// Constructors.
		// ******************************************************************

		#region Constructors

		/// <summary>
		/// Creates a new instance of the CGResource class.
		/// </summary>
		private CGResource()
		{

		} // End CGResource()

		#endregion

		// ******************************************************************
		// Public methods.
		// ******************************************************************
 
		#region Public methods

		/// <summary>
		/// Gets a boolean value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A boolean value.</returns>
		public static bool GetBoolean(
			Type classType,
			string resourceName
			)
		{

			return GetBoolean(
				classType,
				null, 
				resourceName
				);

		} // End GetBoolean()

		// ******************************************************************

		/// <summary>
		/// Gets a boolean value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="culture">An identifier for a specific culture.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A boolean value.</returns>
		public static bool GetBoolean(
			Type classType,
			CultureInfo culture, 
			string resourceName
			)
		{
						
			ResourceManager manager = new ResourceManager(
				classType.FullName,
				classType.Module.Assembly
				);

			object obj = manager.GetObject(
				resourceName, 
				culture
				);
				
			if (obj is bool)
				return (Boolean)obj;
			else
				return false;
		
		} // End GetBoolean()
 
		// ******************************************************************

		/// <summary>
		/// Gets a byte value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A byte value.</returns>
		public static byte GetByte(
			Type classType,
			string resourceName
			)
		{
			
			return GetByte(
				classType,
				null, 
				resourceName
				);

		} // End GetByte()
 
		// ******************************************************************

		/// <summary>
		/// Gets a byte value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="culture">An identifier for a specific culture.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A byte value.</returns>
		public static byte GetByte(
			Type classType,
			CultureInfo culture, 
			string resourceName
			)
		{
			
			ResourceManager manager = new ResourceManager(
				classType.FullName,
				classType.Module.Assembly
				);
		
			object obj = manager.GetObject(
				resourceName, 
				culture
				);

			if (obj is byte)
				return (Byte)obj;
			else
				return 0;

		} // End GetByte()
 
		// ******************************************************************

		/// <summary>
		/// Gets a char value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A char value.</returns>
		public static char GetChar(
			Type classType,
			string resourceName
			)
		{

			return GetChar(
				classType,
				null, 
				resourceName
				);

		} // End GetChar()
 
		// ******************************************************************

		/// <summary>
		/// Gets a char value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="culture">An identifier for a specific culture.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A char value.</returns>
		public static char GetChar(
			Type classType,
			CultureInfo culture, 
			string resourceName
			)
		{
			
			ResourceManager manager = new ResourceManager(
				classType.FullName,
				classType.Module.Assembly
				);

			object obj = manager.GetObject(
				resourceName, 
				culture
				);

			if (obj is char)
				return (Char)obj;
			else
				return '\0';
		
		} // End GetChar()
 
		// ******************************************************************

		/// <summary>
		/// Gets a double value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A double value.</returns>
		public static double GetDouble(
			Type classType,
			string resourceName
			)
		{
			
			return GetDouble(
				classType,
				null, 
				resourceName
				);

		} // End GetDouble()
 
		// ******************************************************************

		/// <summary>
		/// Gets a double value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="culture">An identifier for a specific culture.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A double value.</returns>
		public static double GetDouble(
			Type classType,
			CultureInfo culture, 
			string resourceName
			)
		{
			
			ResourceManager manager = new ResourceManager(
				classType.FullName,
				classType.Module.Assembly
				);

			object obj = manager.GetObject(
				resourceName, 
				culture
				);

			if (obj is double)
				return (Double)obj;
			else
				return 0;

		} // End GetDouble()
 
		// ******************************************************************

		/// <summary>
		/// Gets a float value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A float value.</returns>
		public static float GetFloat(
			Type classType,
			string resourceName
			)
		{
			
			return GetFloat(
				classType,
				null, 
				resourceName
				);

		} // End GetFloat()
 
		// ******************************************************************

		/// <summary>
		/// Gets a float value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="culture">An identifier for a specific culture.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A float value.</returns>
		public static float GetFloat(
			Type classType,
			CultureInfo culture, 
			string resourceName
			)
		{
			
			ResourceManager manager = new ResourceManager(
				classType.FullName,
				classType.Module.Assembly
				);
				
			object obj = manager.GetObject(
				resourceName, 
				culture
				);

			if (obj is float)
				return (Single)obj;
			else
				return 0f;

		} // End GetFloat()
 
		// ******************************************************************

		/// <summary>
		/// Gets an integer value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>An integer value.</returns>
		public static int GetInt(
			Type classType,
			string resourceName
			)
		{

			return GetInt(
				classType,
				null, 
				resourceName
				);

		} // End GetInt()
 
		// ******************************************************************

		/// <summary>
		/// Gets an integer value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="culture">An identifier for a specific culture.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A integer value.</returns>
		public static int GetInt(
			Type classType,
			CultureInfo culture, 
			string resourceName
			)
		{
			
			ResourceManager manager = new ResourceManager(
				classType.FullName,
				classType.Module.Assembly
				);
			
			object obj = manager.GetObject(
				resourceName, 
				culture
				);

			if (obj is int)
				return (Int32)obj;
			else
				return 0;

		} // End GetInt()
 
		// ******************************************************************

		/// <summary>
		/// Gets a long value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A long value.</returns>
		public static long GetLong(
			Type classType,
			string resourceName
			)
		{
			
			return GetLong(
				classType,
				null, 
				resourceName
				);

		} // End GetLong()
 
		// ******************************************************************

		/// <summary>
		/// Gets a long value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="culture">An identifier for a specific culture.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A long value.</returns>
		public static long GetLong(
			Type classType,
			CultureInfo culture, 
			string resourceName
			)
		{
			
			ResourceManager manager = new ResourceManager(
				classType.FullName,
				classType.Module.Assembly
				);
	
			object obj = manager.GetObject(
				resourceName, 
				culture
				);

			if (obj is long)
				return (Int64)obj;
			else
				return 0;

		} // End GetLong()
 
		// ******************************************************************

		/// <summary>
		/// Gets an object value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>An object value.</returns>
		public static object GetObject(
			Type classType,
			string resourceName
			)
		{
			
			return GetObject(
				classType,
				null, 
				resourceName
				);

		} // End GetObject()
 
		// ******************************************************************

		/// <summary>
		/// Gets an object value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="culture">An identifier for a specific culture.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>An object value.</returns>
		public static object GetObject(
			Type classType,
			CultureInfo culture, 
			string resourceName
			)
		{
			
			ResourceManager manager = new ResourceManager(
				classType.FullName,
				classType.Module.Assembly
				);
	
			return manager.GetObject(
				resourceName, 
				culture
				);

		} // End GetObject()
 
		// ******************************************************************

		/// <summary>
		/// Gets a short value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A short value.</returns>
		public static short GetShort(
			Type classType,
			string resourceName
			)
		{

			return GetShort(
				classType,
				null, 
				resourceName
				);

		} // End GetShort()
 
		// ******************************************************************

		/// <summary>
		/// Gets a short value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="culture">An identifier for a specific culture.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A short value.</returns>
		public static short GetShort(
			Type classType,
			CultureInfo culture, 
			string resourceName
			)
		{
			
			ResourceManager manager = new ResourceManager(
				classType.FullName,
				classType.Module.Assembly
				);
	
			object obj = manager.GetObject(
				resourceName, 
				culture
				);
			
			if (obj is short)
				return (Int16)obj;
			else
				return 0;

		} // End GetShort()
 
		// ******************************************************************

		/// <summary>
		/// Gets a string value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A string value.</returns>
		public static string GetString(
			Type classType,
			string resourceName
			)
		{

			return GetString(
				classType,
				null, 
				resourceName
				);

		} // End GetString()
 
		// ******************************************************************

		/// <summary>
		/// Gets a string value from the resource file associated with the 
		/// specified class.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="culture">An identifier for a specific culture.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <returns>A string value.</returns>
		public static string GetString(
			Type classType,
			CultureInfo culture, 
			string resourceName
			)
		{
			
			ResourceManager manager = new ResourceManager(
				classType.FullName,
				classType.Module.Assembly
				);
	
			return manager.GetString(
				resourceName, 
				culture
				);

		} // End GetString()
 
		// ******************************************************************

		/// <summary>
		/// Gets a string value from the resource file associated with the 
		/// specified class and formats the value using the specified parameters.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <param name="args">An array of values to be used when formatting
		/// the resource value.</param>
		/// <returns>A formatted string value.</returns>
		public static string GetString(
			Type classType,
			string resourceName, 
			params object[] args
			)
		{

			return GetString(
				classType,
				null, 
				resourceName, 
				args
				);

		} // End GetString()
 
		// ******************************************************************

		/// <summary>
		/// Gets a string value from the resource file associated with the 
		/// specified class and formats the value using the specified parameters.
		/// </summary>
		/// <param name="classType">The class that contains the resource.</param>
		/// <param name="culture">An identifier for a specific culture.</param>
		/// <param name="resourceName">The name of the resource to read.</param>
		/// <param name="args">An array of values to be used when formatting
		/// the resource value.</param>
		/// <returns>A formatted string value.</returns>
		public static string GetString(
			Type classType,
			CultureInfo culture, 
			string resourceName, 
			params object[] args
			)
		{
			
			string text = GetString(
				classType,
				resourceName, 
				culture
				);

			if ((args != null) && (args.Length > 0))
				return string.Format(
					culture,
					text, 
					args
					);

			return text;
		
		} // End GetString()

		#endregion

	} // End class CGResource
    
} // End namespace CG.Core.Resource


