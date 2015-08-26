using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Controls.ListViewLayoutManager
{
	public abstract class ImageGridViewColumn : GridViewColumn, IValueConverter
	{
		protected ImageGridViewColumn() :
			this(Stretch.None)
		{
		}

		protected ImageGridViewColumn(Stretch imageStretch)
		{
			FrameworkElementFactory imageElement = new FrameworkElementFactory(typeof(Image));

			// image source
			Binding imageSourceBinding = new Binding();
			imageSourceBinding.Converter = this;
			imageSourceBinding.Mode = BindingMode.OneWay;
			imageElement.SetBinding(Image.SourceProperty, imageSourceBinding);

			// image stretching
			Binding imageStretchBinding = new Binding();
			imageStretchBinding.Source = imageStretch;
			imageElement.SetBinding(Image.StretchProperty, imageStretchBinding);

			DataTemplate template = new DataTemplate();
			template.VisualTree = imageElement;
			CellTemplate = template;
		}

		protected abstract ImageSource GetImageSource(object value);

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return GetImageSource(value);
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
