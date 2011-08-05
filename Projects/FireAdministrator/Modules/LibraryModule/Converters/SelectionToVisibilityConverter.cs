using System;
using System.Windows;
using System.Windows.Data;
using LibraryModule.ViewModels;

namespace LibraryModule.Converters
{
    class SelectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var selectedItem = value;
            switch ((SelectionType) parameter)
            {
                case SelectionType.None:
                    if (selectedItem == null ||
                        (selectedItem as DeviceViewModel) != null)
                        return Visibility.Visible;
                    break;

                case SelectionType.Device:
                    if ((selectedItem as DeviceViewModel) != null)
                        return Visibility.Visible;
                    break;

                case SelectionType.DeviceState:
                    if ((selectedItem as StateViewModel) != null)
                        return Visibility.Visible;
                    break;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
