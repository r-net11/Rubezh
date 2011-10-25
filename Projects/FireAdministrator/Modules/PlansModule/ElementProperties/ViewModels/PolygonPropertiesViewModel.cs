using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class PolygonPropertiesViewModel : SaveCancelDialogContent
    {
        ElementPolygon _elementPolygon;
        public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

        public PolygonPropertiesViewModel(ElementPolygon elementPolygon)
        {
            Title = "Свойства фигуры: Полигон";
            ImagePropertiesViewModel = new ImagePropertiesViewModel();
            _elementPolygon = elementPolygon;
            CopyProperties();
        }

        void CopyProperties()
        {
            BackgroundColor = _elementPolygon.BackgroundColor;
            BorderColor = _elementPolygon.BorderColor;
            StrokeThickness = _elementPolygon.BorderThickness;
            ImagePropertiesViewModel.BackgroundPixels = _elementPolygon.BackgroundPixels;
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
            _elementPolygon.BackgroundColor = BackgroundColor;
            _elementPolygon.BorderColor = BorderColor;
            _elementPolygon.BorderThickness = StrokeThickness;
            _elementPolygon.BackgroundPixels = ImagePropertiesViewModel.BackgroundPixels;
        }
    }
}
