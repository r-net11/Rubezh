using Infrastructure.Common.Services.Layout;
using Infrustructure.Plans;
using RubezhAPI.Models.Layouts;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartPropertyTextPageViewModel : LayoutPartPropertyPageViewModel
	{
		private const int MaxFontSize = 1000;
		private bool _haveChanges;
		public List<FontFamily> Fonts { get; private set; }
		public List<string> HorizontalAlignments { get; private set; }
		public List<string> VerticalAlignments { get; private set; }
		public List<string> TextAlignments { get; private set; }
		public bool IsEditable { get; private set; }
		private LayoutPartTextViewModel _layoutPartViewModel;

		public LayoutPartPropertyTextPageViewModel(LayoutPartTextViewModel layoutPartViewModel, bool isEditable)
		{
			_layoutPartViewModel = layoutPartViewModel;
			IsEditable = isEditable;
			TextAlignments = new List<string>()
			{
				"По левому краю",
				"По правому краю",
				"По центру",
				"Растянуть",
			};
			HorizontalAlignments = new List<string>()
			{
				"По левому краю",
				"По центру",
				"По правому краю",
			};
			VerticalAlignments = new List<string>()
			{
				"По верхему краю",
				"По середине",
				"По нижнему краю",
			};
			Fonts = System.Windows.Media.Fonts.SystemFontFamilies.OrderBy(item => item.Source).ToList();
		}

		private string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
				_haveChanges = true;
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
				_haveChanges = true;
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
				_haveChanges = true;
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
				_haveChanges = true;
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
				_haveChanges = true;
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
				_haveChanges = true;
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
				_haveChanges = true;
			}
		}

		private FontFamily _fontFamily;
		public FontFamily FontFamily
		{
			get { return _fontFamily; }
			set
			{
				_fontFamily = value;
				OnPropertyChanged(() => FontFamily);
				_haveChanges = true;
			}
		}

		private int _horizontalAlignment;
		public int HorizontalAlignment
		{
			get { return _horizontalAlignment; }
			set
			{
				_horizontalAlignment = value;
				OnPropertyChanged(() => HorizontalAlignment);
				_haveChanges = true;
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
				_haveChanges = true;
			}
		}

		private bool _acceptReturn;
		public bool AcceptReturn
		{
			get { return _acceptReturn; }
			set
			{
				_acceptReturn = value;
				OnPropertyChanged(() => AcceptReturn);
				_haveChanges = true;
			}
		}

		private bool _acceptTab;
		public bool AcceptTab
		{
			get { return _acceptTab; }
			set
			{
				_acceptTab = value;
				OnPropertyChanged(() => AcceptTab);
				_haveChanges = true;
			}
		}

		private bool _textTrimming;
		public bool TextTrimming
		{
			get { return _textTrimming; }
			set
			{
				_textTrimming = value;
				OnPropertyChanged(() => TextTrimming);
				_haveChanges = true;
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
				_haveChanges = true;
			}
		}

		public override string Header
		{
			get { return "Формат"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartTextProperties)_layoutPartViewModel.Properties;
			if (properties != null)
			{
				AcceptReturn = properties.AcceptReturn;
				AcceptTab = properties.AcceptTab;
				BackgroundColor = properties.BackgroundColor.ToWindowsColor();
				FontBold = properties.FontBold;
				FontFamily = new FontFamily(properties.FontFamilyName);
				FontItalic = properties.FontItalic;
				FontSize = properties.FontSize;
				ForegroundColor = properties.ForegroundColor.ToWindowsColor();
				Text = properties.Text;
				TextAlignment = properties.TextAlignment;
				TextTrimming = properties.TextTrimming;
				HorizontalAlignment = properties.HorizontalAlignment;
				VerticalAlignment = properties.VerticalAlignment;
				WordWrap = properties.WordWrap;
			}
			_haveChanges = false;
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			var properties = (LayoutPartTextProperties)_layoutPartViewModel.Properties;
			if (_haveChanges)
			{
				properties.AcceptReturn = AcceptReturn;
				properties.AcceptTab = AcceptTab;
				properties.BackgroundColor = BackgroundColor.ToRubezhColor();
				properties.FontBold = FontBold;
				properties.FontFamilyName = FontFamily.Source;
				properties.FontItalic = FontItalic;
				properties.FontSize = FontSize;
				properties.ForegroundColor = ForegroundColor.ToRubezhColor();
				properties.HorizontalAlignment = HorizontalAlignment;
				properties.Text = Text;
				properties.TextAlignment = TextAlignment;
				properties.TextTrimming = TextTrimming;
				properties.VerticalAlignment = VerticalAlignment;
				properties.WordWrap = WordWrap;
				return true;
			}
			return false;
		}
	}
}
