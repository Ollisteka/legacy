using System;
using System.Text;
using StatePrinting.ValueConverters;

namespace Emails
{
	public class StringBuilderConverter : IValueConverter

	{
		public bool CanHandleType(Type type)
		{
			return type == typeof(StringBuilder);
		}

		public string Convert(object source)
		{
			if (!CanHandleType(source.GetType()))
				throw new ArgumentException();
			return ((StringBuilder) source).ToString();
		}
	}
}