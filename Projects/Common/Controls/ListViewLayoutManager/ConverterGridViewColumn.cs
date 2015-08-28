using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Controls.ListViewLayoutManager
{
	public abstract class ConverterGridViewColumn : GridViewColumn, IValueConverter
	{
		protected ConverterGridViewColumn(Type bindingType)
		{
			if (bindingType == null)
			{
				throw new ArgumentNullException("bindingType");
			}

			this.bindingType = bindingType;

			// binding
			Binding binding = new Binding();
			binding.Mode = BindingMode.OneWay;
			binding.Converter = this;
			DisplayMemberBinding = binding;
		}

		public Type BindingType
		{
			get { return bindingType; }
		}

		protected abstract object ConvertValue(object value);

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!bindingType.IsInstanceOfType(value))
			{
				throw new InvalidOperationException();
			}
			return ConvertValue(value);
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		// members
		private readonly Type bindingType;

	}
}
