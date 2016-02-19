using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecService.Views.TypeConverters
{
	public class BooleanTypeConverter: BooleanConverter
	{
		public const string TRUE = "Да";
		public const string FALSE = "Нет";

		public override object ConvertTo(ITypeDescriptorContext context, 
			System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			return (bool)value ? TRUE : FALSE;
			//return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, 
			System.Globalization.CultureInfo culture, object value)
		{
			return (string)value == TRUE;
			//return base.ConvertFrom(context, culture, value);
		}
	}
}