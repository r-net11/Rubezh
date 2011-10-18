using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models.Plans;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class RectanglePropertiesViewModel : SaveCancelDialogContent
    {
        Rectangle _rectangle;
        ElementRectangle _elementRectangle;

        public RectanglePropertiesViewModel(Rectangle rectangle, ElementRectangle elementRectangle)
        {
            Title = "Свойства элемента Прямоугольник";
            _rectangle = rectangle;
            _elementRectangle = elementRectangle;
            CopyProperties();
        }

        void CopyProperties()
        {
            BackgroundColor = _elementRectangle.BackgroundColor;
            BorderColor = _elementRectangle.BorderColor;
            StrokeThickness = _elementRectangle.BorderThickness;
        }

        Color _backgroundColor;
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        Color _borderColor;
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                OnPropertyChanged("BorderColor");
            }
        }

        double _strokeThickness;
        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                _strokeThickness = value;
                OnPropertyChanged("StrokeThickness");
            }
        }

        protected override void Save(ref bool cancel)
        {
            _elementRectangle.BackgroundColor = BackgroundColor;
            _elementRectangle.BorderColor = BorderColor;
            _elementRectangle.BorderThickness = StrokeThickness;

            _rectangle.Fill = new SolidColorBrush(BackgroundColor);
            _rectangle.Stroke = new SolidColorBrush(BorderColor);
            _rectangle.StrokeThickness = StrokeThickness;
        }
    }
}
