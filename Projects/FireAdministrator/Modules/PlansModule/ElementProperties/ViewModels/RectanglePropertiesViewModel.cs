using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class RectanglePropertiesViewModel : SaveCancelDialogContent
    {
        ElementRectangle _elementRectangle;
        public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

        public RectanglePropertiesViewModel(ElementRectangle elementRectangle)
        {
            Title = "Свойства фигуры: Прямоугольник";
            ImagePropertiesViewModel = new ImagePropertiesViewModel();
            _elementRectangle = elementRectangle;
            CopyProperties();
        }

        void CopyProperties()
        {
            BackgroundColor = _elementRectangle.BackgroundColor;
            BorderColor = _elementRectangle.BorderColor;
            StrokeThickness = _elementRectangle.BorderThickness;
            ImagePropertiesViewModel.BackgroundPixels = _elementRectangle.BackgroundPixels;
            ImagePropertiesViewModel.UpdateImage();
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
            _elementRectangle.BackgroundPixels = ImagePropertiesViewModel.BackgroundPixels;
        }
    }
}