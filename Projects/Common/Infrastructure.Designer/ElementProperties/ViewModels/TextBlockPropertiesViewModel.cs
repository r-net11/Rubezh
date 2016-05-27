using System.Collections.Generic;
using System.Windows.Media;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
using StrazhAPI.Plans.Elements;

namespace Infrastructure.Designer.ElementProperties.ViewModels
{
	public class TextBlockPropertiesViewModel : SaveCancelDialogViewModel
	{
		private const int MaxFontSize = 1000;
		public List<string> Fonts { get; private set; }
		public List<string> TextAlignments { get; private set; }
		public List<string> VerticalAlignments { get; private set; }
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
			VerticalAlignments = new List<string>()
			{
				"По верхнему краю",
				"По середине",
				"По нижнему краю",
			};
		}

		protected virtual void CopyProperties()
		{
			Text = ElementTextBlock.Text;
			BackgroundColor = ElementTextBlock.BackgroundColor.ToWindowsColor();
			ForegroundColor = ElementTextBlock.ForegroundColor.ToWindowsColor();
			BorderColor = ElementTextBlock.BorderColor.ToWindowsColor();
			StrokeThickness = ElementTextBlock.BorderThickness;
			FontSize = ElementTextBlock.FontSize;
			FontItalic = ElementTextBlock.FontItalic;
			FontBold = ElementTextBlock.FontBold;
			FontFamilyName = ElementTextBlock.FontFamilyName;
			Stretch = ElementTextBlock.Stretch;
			TextAlignment = ElementTextBlock.TextAlignment;
			VerticalAlignment = ElementTextBlock.VerticalAlignment;
			WordWrap = ElementTextBlock.WordWrap;
			PresentationName = ElementTextBlock.PresentationName;
		}

		private string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}

		private Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged(() => BackgroundColor);
			}
		}

		private Color _foregroundColor;
		public Color ForegroundColor
		{
			get { return _foregroundColor; }
			set
			{
				_foregroundColor = value;
				OnPropertyChanged(() => ForegroundColor);
			}
		}

		private Color _borderColor;
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				_borderColor = value;
				OnPropertyChanged(() => BorderColor);
			}
		}

		string _presentationName;
		public string PresentationName
		{
			get { return _presentationName; }
			set
			{
				_presentationName = value;
				OnPropertyChanged(() => PresentationName);
			}
		}

		private double _strokeThickness;
		public double StrokeThickness
		{
			get { return _strokeThickness; }
			set
			{
				_strokeThickness = value;
				OnPropertyChanged(() => StrokeThickness);
			}
		}

		private double _fontSize;
		public double FontSize
		{
			get { return _fontSize; }
			set
			{
				_fontSize = value;
				if (_fontSize > MaxFontSize)
					_fontSize = MaxFontSize;
				OnPropertyChanged(() => FontSize);
			}
		}

		private bool _fontBold;
		public bool FontBold
		{
			get { return _fontBold; }
			set
			{
				_fontBold = value;
				OnPropertyChanged(() => FontBold);
			}
		}

		private bool _fontItalic;
		public bool FontItalic
		{
			get { return _fontItalic; }
			set
			{
				_fontItalic = value;
				OnPropertyChanged(() => FontItalic);
			}
		}

		private bool _stretch;
		public bool Stretch
		{
			get { return _stretch; }
			set
			{
				_stretch = value;
				OnPropertyChanged(() => Stretch);
			}
		}

		private int _textAlignment;
		public int TextAlignment
		{
			get { return _textAlignment; }
			set
			{
				_textAlignment = value;
				OnPropertyChanged(() => TextAlignment);
			}
		}

		private int _verticalAlignment;
		public int VerticalAlignment
		{
			get { return _verticalAlignment; }
			set
			{
				_verticalAlignment = value;
				OnPropertyChanged(() => VerticalAlignment);
			}
		}

		private bool _wordWrap;
		public bool WordWrap
		{
			get { return _wordWrap; }
			set
			{
				_wordWrap = value;
				OnPropertyChanged(() => WordWrap);
			}
		}


		private string _fontFamilyName;
		public string FontFamilyName
		{
			get { return _fontFamilyName; }
			set
			{
				_fontFamilyName = value;
				OnPropertyChanged(() => FontFamilyName);
			}
		}

		protected override bool Save()
		{
			ElementTextBlock.Text = Text;
			ElementTextBlock.BackgroundColor = BackgroundColor.ToStruzhColor();
			ElementTextBlock.ForegroundColor = ForegroundColor.ToStruzhColor();
			ElementTextBlock.BorderColor = BorderColor.ToStruzhColor();
			ElementTextBlock.BorderThickness = StrokeThickness;
			ElementTextBlock.FontSize = FontSize;
			ElementTextBlock.FontBold = FontBold;
			ElementTextBlock.FontItalic = FontItalic;
			ElementTextBlock.FontFamilyName = FontFamilyName;
			ElementTextBlock.TextAlignment = TextAlignment;
			ElementTextBlock.VerticalAlignment = VerticalAlignment;
			ElementTextBlock.WordWrap = WordWrap;
			ElementTextBlock.Stretch = Stretch;
			ElementTextBlock.PresentationName = PresentationName;
			return base.Save();
		}
	}
}