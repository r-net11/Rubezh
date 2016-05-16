using Controls.Converters;
using Controls.Extentions;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Plans.Elements;
using System.Collections.Generic;
using System.Windows.Media;

namespace Infrastructure.Plans.ElementProperties.ViewModels
{
	public class TextBlockPropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		private const int MaxFontSize = 1000;
		public List<string> Fonts { get; private set; }
		public List<string> TextAlignments { get; private set; }
		public List<string> VerticalAlignments { get; private set; }
		ElementBaseRectangle ElementBaseRectangle { get; set; }
		protected IElementTextBlock ElementTextBlock { get; private set; }

		public TextBlockPropertiesViewModel(IElementTextBlock element)
		{
			Title = "Свойства фигуры: Надпись";
			ElementTextBlock = element;
			ElementBaseRectangle = element as ElementBaseRectangle;
			Left = (int)(ElementBaseRectangle.Left * _sensivityFactor);
			Top = (int)(ElementBaseRectangle.Top * _sensivityFactor);

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
		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}
		protected virtual void CopyProperties()
		{
			ElementBase.Copy(this.ElementTextBlock, this);
			StrokeThickness = ElementTextBlock.BorderThickness;
			BackgroundColor = ElementTextBlock.BackgroundColor.ToWindowsColor();
			BorderColor = ElementTextBlock.BorderColor.ToWindowsColor();
			ForegroundColor = ElementTextBlock.ForegroundColor.ToWindowsColor();
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

		bool _showTooltip;
		public bool ShowTooltip
		{
			get { return this._showTooltip; }
			set
			{
				this._showTooltip = value;
				OnPropertyChanged(() => this.ShowTooltip);
			}
		}

		protected override bool Save()
		{
			ElementBaseRectangle.Left = (double)Left / _sensivityFactor;
			ElementBaseRectangle.Top = (double)Top / _sensivityFactor;
			ElementBase.Copy(this, this.ElementTextBlock);
			var colorConverter = new ColorToSystemColorConverter();
			ElementTextBlock.BorderColor = (RubezhAPI.Color)colorConverter.ConvertBack(this.BorderColor, this.BorderColor.GetType(), null, null);
			ElementTextBlock.BackgroundColor = (RubezhAPI.Color)colorConverter.ConvertBack(this.BackgroundColor, this.BackgroundColor.GetType(), null, null);
			ElementTextBlock.ForegroundColor = (RubezhAPI.Color)colorConverter.ConvertBack(this.ForegroundColor, this.ForegroundColor.GetType(), null, null);
			ElementTextBlock.BorderThickness = StrokeThickness;
			return base.Save();
		}
	}
}