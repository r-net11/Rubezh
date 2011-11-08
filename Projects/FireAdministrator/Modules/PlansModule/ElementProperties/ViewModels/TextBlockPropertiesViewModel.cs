using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class TextBlockPropertiesViewModel : SaveCancelDialogContent
    {
        ElementTextBlock _elementTextBlock;

        public TextBlockPropertiesViewModel(ElementTextBlock elementTextBlock)
        {
            Title = "Свойства фигуры: Надпись";
            _elementTextBlock = elementTextBlock;
            CopyProperties();
        }

        void CopyProperties()
        {
            BackgroundColor = _elementTextBlock.BackgroundColor;
            BorderColor = _elementTextBlock.BorderColor;
            StrokeThickness = _elementTextBlock.BorderThickness;
            Text = _elementTextBlock.Text;
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

        string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        protected override void Save(ref bool cancel)
        {
            _elementTextBlock.BackgroundColor = BackgroundColor;
            _elementTextBlock.BorderColor = BorderColor;
            _elementTextBlock.BorderThickness = StrokeThickness;
            _elementTextBlock.Text = Text;
        }
    }
}
