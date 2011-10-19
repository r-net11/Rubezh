using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Windows.Controls;
using FiresecAPI.Models;
using System.Windows.Media;

namespace PlansModule.ViewModels
{
    public class TextBlockPropertiesViewModel : SaveCancelDialogContent
    {
        TextBlock _textBlock;
        ElementTextBlock _elementTextBlock;

        public TextBlockPropertiesViewModel(TextBlock textBlock, ElementTextBlock elementTextBlock)
        {
            Title = "Свойства элемента Надпись";
            _textBlock = textBlock;
            _elementTextBlock = elementTextBlock;
            CopyProperties();
        }

        void CopyProperties()
        {
            BackgroundColor = _elementTextBlock.BackgroundColor;
            BorderColor = _elementTextBlock.BorderColor;
            StrokeThickness = _elementTextBlock.BorderThickness;
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
            _elementTextBlock.BackgroundColor = BackgroundColor;
            _elementTextBlock.BorderColor = BorderColor;
            _elementTextBlock.BorderThickness = StrokeThickness;

            //_textBlock.Fill = new SolidColorBrush(BackgroundColor);
            //_textBlock.Stroke = new SolidColorBrush(BorderColor);
            //_textBlock.StrokeThickness = StrokeThickness;
        }
    }
}
