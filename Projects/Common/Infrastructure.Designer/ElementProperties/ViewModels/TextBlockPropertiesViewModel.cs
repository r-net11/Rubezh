using System.Collections.Generic;
using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;

namespace Infrastructure.Designer.ElementProperties.ViewModels
{
	public class TextBlockPropertiesViewModel : SaveCancelDialogViewModel
	{
		private const int MaxFontSize = 1000;
		public List<string> Fonts { get; private set; }
		public List<string> TextAlignments { get; private set; }
		protected IElementTextBlock ElementTextBlock { get; private set; }

		public TextBlockPropertiesViewModel(IElementTextBlock elementTextBlock)
		{
			Title = "Свойства фигуры: Надпись";
			ElementTextBlock = elementTextBlock;
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

		protected virtual void CopyProperties()
		{
			Text = ElementTextBlock.Text;
			BackgroundColor = ElementTextBlock.BackgroundColor;
			ForegroundColor = ElementTextBlock.ForegroundColor;
			BorderColor = ElementTextBlock.BorderColor;
			StrokeThickness = ElementTextBlock.BorderThickness;
			FontSize = ElementTextBlock.FontSize;
			FontItalic = ElementTextBlock.FontItalic;
			FontBold = ElementTextBlock.FontBold;
			FontFamilyName = ElementTextBlock.FontFamilyName;
			Stretch = ElementTextBlock.Stretch;
			TextAlignment = ElementTextBlock.TextAlignment;
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
				if (_fontSize > MaxFontSize)
					_fontSize = MaxFontSize;
				OnPropertyChanged("FontSize");
			}
		}

		bool _fontBold;
		public bool FontBold
		{
			get { return _fontBold; }
			set
			{
				_fontBold = value;
				OnPropertyChanged("FontBold");
			}
		}

		bool _fontItalic;
		public bool FontItalic
		{
			get { return _fontItalic; }
			set
			{
				_fontItalic = value;
				OnPropertyChanged("FontItalic");
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
			ElementTextBlock.Text = Text;
			ElementTextBlock.BackgroundColor = BackgroundColor;
			ElementTextBlock.ForegroundColor = ForegroundColor;
			ElementTextBlock.BorderColor = BorderColor;
			ElementTextBlock.BorderThickness = StrokeThickness;
			ElementTextBlock.FontSize = FontSize;
			ElementTextBlock.FontBold = FontBold;
			ElementTextBlock.FontItalic = FontItalic;
			ElementTextBlock.FontFamilyName = FontFamilyName;
			ElementTextBlock.TextAlignment = TextAlignment;
			ElementTextBlock.Stretch = Stretch;
			return base.Save();
		}
	}
}