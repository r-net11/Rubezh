using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Firesec;
using System.Windows.Media;
using FiresecClient.Models;

namespace JournalModule.Converters
{
    public class StateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StateType stateType = (StateType)value;

            Brush brush;

            switch (stateType)
            {
                case StateType.Fire:
                    brush = Brushes.Red;
                    break;

                case StateType.Failure:
                    brush = Brushes.LightPink;
                    break;

                case StateType.Info:
                    brush = Brushes.Transparent;
                    break;

                case StateType.No:
                    brush = Brushes.Transparent;
                    break;

                case StateType.Norm:
                    brush = Brushes.Green;
                    break;

                case StateType.Off:
                    brush = Brushes.Red;
                    break;

                case StateType.Service:
                    brush = Brushes.Yellow;
                    break;

                case StateType.Unknown:
                    brush = Brushes.Gray;
                    break;

                case StateType.Attention:
                    brush = Brushes.Yellow;
                    break;

                default:
                    brush = Brushes.Transparent;
                    break;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
