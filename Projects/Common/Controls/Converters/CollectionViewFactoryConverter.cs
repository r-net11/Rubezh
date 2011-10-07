using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Controls
{
    [ValueConversion(typeof(System.Collections.IList), typeof(System.Collections.IEnumerable))]
    public class CollectionViewFactoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as System.Collections.IList;
            var view = new ListCollectionView(collection);
            var parameters = (parameter as string).Split(';');

            foreach (var param in parameters)
            {
                view.SortDescriptions.Add(new SortDescription(param.ToString(), ListSortDirection.Ascending));
            }

            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}