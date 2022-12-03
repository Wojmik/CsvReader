using System;
using System.Collections.Generic;
using System.Text;

namespace WojciechMikołajewicz.CsvReader.Exceptions
{
	/// <summary>
	/// Binding configuration exception
	/// </summary>
	public class BindingConfigurationException : Exception
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public BindingConfigurationException(string message)
			: base(message)
		{ }
	}
}