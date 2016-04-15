using Common;
using Infrastructure.Common.Services.Layout;
//using System.Windows.Media;
using RubezhAPI.Models.Layouts;
using System.Collections.Generic;
using System.Linq;

namespace FireAdministrator.ViewModels
{
	public class LayoutPartPropertyTimePageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartTimeViewModel _layoutPartTimeViewModel;
		private const int MaxFontSize = 1000;
		private bool _haveChanges;
		public List<System.Windows.Media.FontFamily> Fonts { get; private set; }
		public List<string> HorizontalAlignments { get; private set; }
		public List<string> VerticalAlignments { get; private set; }
		public List<string> Formats { get; private set; }

		public LayoutPartPropertyTimePageViewModel(LayoutPartTimeViewModel layoutPartTimeViewModel)
		{
			_layoutPartTimeViewModel = layoutPartTimeViewModel;
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
			Formats = new List<string>()
			{
				"dd.MM.yyyy H:mm:ss",
				"dd.MM.yyyy HH:mm:ss",
				"dd MMMM yyyy H:mm:ss",
				"dd MMMM yyyy HH:mm:ss",
				"dd.MM H:mm:ss",
				"dd.MM HH:mm:ss",
				"dd MMMM H:mm:ss",
				"dd MMMM HH:mm:ss",
				"H:mm:ss",
				"HH:mm:ss",
				"H:mm",
				"HH:mm",
			};
			Fonts = System.Windows.Media.Fonts.SystemFontFamilies.OrderBy(item => item.Source).ToList();
		}

		private string _format;
		public string Format
		{
			get { return _format; }
			set
			{
				_format = value;
				OnPropertyChanged(() => Format);
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

		private Color _borderColor;
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				_borderColor = value;
				OnPropertyChanged(() => BorderColor);
				_haveChanges = true;
			}
		}

		private double _borderThickness;
		public double BorderThickness
		{
			get { return _borderThickness; }
			set
			{
				_borderThickness = value;
				OnPropertyChanged(() => BorderThickness);
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

		private bool _stretch;
		public bool Stretch
		{
			get { return _stretch; }
			set
			{
				_stretch = value;
				OnPropertyChanged(() => Stretch);
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

		private System.Windows.Media.FontFamily _fontFamily;
		public System.Windows.Media.FontFamily FontFamily
		{
			get { return _fontFamily; }
			set
			{
				_fontFamily = value;
				OnPropertyChanged(() => FontFamily);
				_haveChanges = true;
			}
		}

		public override string Header
		{
			get { return "Стиль"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartTimeProperties)_layoutPartTimeViewModel.Properties;
			if (properties != null)
			{
				Format = properties.Format;
				BackgroundColor = properties.BackgroundColor;
				ForegroundColor = properties.ForegroundColor;
				BorderColor = properties.BorderColor;
				BorderThickness = properties.BorderThickness;
				FontSize = properties.FontSize;
				FontItalic = properties.FontItalic;
				FontBold = properties.FontBold;
				FontFamily = new System.Windows.Media.FontFamily(properties.FontFamilyName);
				Stretch = properties.Stretch;
				HorizontalAlignment = properties.HorizontalAlignment;
				VerticalAlignment = properties.VerticalAlignment;
			}
			_haveChanges = false;
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			var properties = (LayoutPartTimeProperties)_layoutPartTimeViewModel.Properties;
			if (_haveChanges)
			{
				properties.Format = Format;
				properties.BackgroundColor = BackgroundColor;
				properties.ForegroundColor = ForegroundColor;
				properties.BorderColor = BorderColor;
				properties.BorderThickness = BorderThickness;
				properties.FontSize = FontSize;
				properties.FontBold = FontBold;
				properties.FontItalic = FontItalic;
				properties.FontFamilyName = FontFamily.Source;
				properties.Stretch = Stretch;
				properties.HorizontalAlignment = HorizontalAlignment;
				properties.VerticalAlignment = VerticalAlignment;
				return true;
			}
			return false;
		}
	}
}