using System.Collections.Generic;
using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows;

namespace PlansModule.ViewModels
{
	public class TextBlockPropertiesViewModel : SaveCancelDialogViewModel
	{
		private ElementTextBlock _elementTextBlock;
		public List<string> Fonts { get; private set; }
		public List<string> TextAlignments { get; private set; }

		public TextBlockPropertiesViewModel(ElementTextBlock elementTextBlock)
		{
			Title = "Свойства фигуры: Надпись";
			_elementTextBlock = elementTextBlock;
			CopyProperties();

			Fonts = new List<string>();
			foreach (var fontfamily in System.Drawing.FontFamily.Families)
				Fonts.Add(fontfamily.Name);
			TextAlignments = new List<string>()
			{
				"По левому краю",
				"По правому краю",
				"По центру",
			};
		}

		void CopyProperties()
		{
			Text = _elementTextBlock.Text;
			BackgroundColor = _elementTextBlock.BackgroundColor;
			ForegroundColor = _elementTextBlock.ForegroundColor;
			BorderColor = _elementTextBlock.BorderColor;
			StrokeThickness = _elementTextBlock.BorderThickness;
			FontSize = _elementTextBlock.FontSize;
			FontFamilyName = _elementTextBlock.FontFamilyName;
			Stretch = _elementTextBlock.Stretch;
			TextAlignment = _elementTextBlock.TextAlignment;
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

		Color _foregroundColor;
		public Color ForegroundColor
		{
			get { return _foregroundColor; }
			set
			{
				_foregroundColor = value;
				OnPropertyChanged("ForegroundColor");
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

		double _fontSize;
		public double FontSize
		{
			get { return _fontSize; }
			set
			{
				_fontSize = value;
				OnPropertyChanged("FontSize");
			}
		}

		bool _stretch;
		public bool Stretch
		{
			get { return _stretch; }
			set
			{
				_stretch = value;
				OnPropertyChanged("Stretch");
			}
		}

		int _textAlignment;
		public int TextAlignment
		{
			get{return _textAlignment;}
			set
			{
				_textAlignment = value;
				OnPropertyChanged("TextAlignment");
			}
		}

		string _fontFamilyName;
		public string FontFamilyName
		{
			get { return _fontFamilyName; }
			set
			{
				_fontFamilyName = value;
				OnPropertyChanged("FontFamilyName");
			}
		}

		protected override bool Save()
		{
			_elementTextBlock.Text = Text;
			_elementTextBlock.BackgroundColor = BackgroundColor;
			_elementTextBlock.ForegroundColor = ForegroundColor;
			_elementTextBlock.BorderColor = BorderColor;
			_elementTextBlock.BorderThickness = StrokeThickness;
			_elementTextBlock.FontSize = FontSize;
			_elementTextBlock.FontFamilyName = FontFamilyName;
			_elementTextBlock.TextAlignment = TextAlignment;
			_elementTextBlock.Stretch = Stretch;
			return base.Save();
		}
	}
}