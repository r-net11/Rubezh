using System;
using System.Windows.Data;
using XFiresecAPI;
using FiresecAPI.Models;


namespace Controls.Converters
{
    public class DescriptionTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((DescriptionType)value)
            {
                case DescriptionType.Failure:
                    return "/Controls;component/StateClassIcons/Failure.png";

				case DescriptionType.Fire:
					return "/Controls;component/StateClassIcons/Fire1.png";

				case DescriptionType.Information:
					return "/Controls;component/StateClassIcons/Info.png";

				case DescriptionType.User:
					return "/Controls;component/Images/PC.png";

                default:
                    return "/Controls;component/Images/blank.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

