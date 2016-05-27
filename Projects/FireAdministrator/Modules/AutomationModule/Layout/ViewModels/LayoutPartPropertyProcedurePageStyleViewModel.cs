using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Services.Layout;
using System.Collections.ObjectModel;
using Infrustructure.Plans;
using StrazhAPI.Automation;
using FiresecClient;
using StrazhAPI.Models.Layouts;
using System.Windows.Media;
using System.Windows.Controls;

namespace AutomationModule.Layout.ViewModels
{
	public class LayoutPartPropertyProcedurePageStyleViewModel : LayoutPartPropertyPageViewModel
	{
		private const int MaxFontSize = 1000;
		private bool _haveChanges;
		public List<FontFamily> Fonts { get; private set; }
		public List<string> TextAlignments { get; private set; }
		private LayoutPartProcedureViewModel _layoutPartViewModel;

		public LayoutPartPropertyProcedurePageStyleViewModel(LayoutPartProcedureViewModel layoutPartViewModel)
		{
			_layoutPartViewModel = layoutPartViewModel;
			TextAlignments = new List<string>()
			{
				"По левому краю",
				"По правому краю",
				"По центру",
			};
			Fonts = System.Windows.Media.Fonts.SystemFontFamilies.OrderBy(item => item.Source).ToList();
		}

		private bool _useCustomStyle;
		public bool UseCustomStyle
		{
			get { return _useCustomStyle; }
			set
			{
				_useCustomStyle = value;
				OnPropertyChanged(() => UseCustomStyle);
			}
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

		public override string Header
		{
			get { return "Стиль"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartProcedureProperties)_layoutPartViewModel.Properties;
			if (properties != null)
			{
				UseCustomStyle = properties.UseCustomStyle;
				Text = properties.Text;
				BackgroundColor = properties.BackgroundColor.ToWindowsColor();
				ForegroundColor = properties.ForegroundColor.ToWindowsColor();
				BorderColor = properties.BorderColor.ToWindowsColor();
				BorderThickness = properties.BorderThickness;
				FontSize = properties.FontSize;
				FontItalic = properties.FontItalic;
				FontBold = properties.FontBold;
				FontFamily = new FontFamily(properties.FontFamilyName);
				Stretch = properties.Stretch;
				TextAlignment = properties.TextAlignment;
			}
			_haveChanges = false;
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			var properties = (LayoutPartProcedureProperties)_layoutPartViewModel.Properties;
			if (_haveChanges)
			{
				properties.UseCustomStyle = UseCustomStyle;
				properties.Text = Text;
				properties.BackgroundColor = BackgroundColor.ToStruzhColor();
				properties.ForegroundColor = ForegroundColor.ToStruzhColor();
				properties.BorderColor = BorderColor.ToStruzhColor();
				properties.BorderThickness = BorderThickness;
				properties.FontSize = FontSize;
				properties.FontBold = FontBold;
				properties.FontItalic = FontItalic;
				properties.FontFamilyName = FontFamily.Source;
				properties.TextAlignment = TextAlignment;
				properties.Stretch = Stretch;
				return true;
			}
			return false;
		}
	}
}