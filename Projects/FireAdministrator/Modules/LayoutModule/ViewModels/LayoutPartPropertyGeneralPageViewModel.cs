using Infrastructure.Common.Services.Layout;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LayoutModule.ViewModels
{
	public class LayoutPartPropertyGeneralPageViewModel : LayoutPartPropertyPageViewModel
	{
		LayoutPartSize _layoutPartSize;
		LayoutPartViewModel _layoutPartViewModel;
		bool _initialized;
		public LayoutPartPropertyGeneralPageViewModel(LayoutPartViewModel layoutPartViewModel, LayoutPartSize layoutPartSize)
		{
			_initialized = false;
			_layoutPartSize = layoutPartSize;
			_layoutPartViewModel = layoutPartViewModel;
			UnitTypes = new ObservableCollection<GridUnitType>(Enum.GetValues(typeof(GridUnitType)).Cast<GridUnitType>());
			_initialized = true;
		}

		public ObservableCollection<GridUnitType> UnitTypes { get; private set; }

		GridUnitType _widthType;
		public GridUnitType WidthType
		{
			get { return _widthType; }
			set
			{
				if (WidthType != value)
				{
					_widthType = value;
					OnPropertyChanged(() => WidthType);
					if (_initialized && WidthType == GridUnitType.Auto)
						Width = _layoutPartSize.PreferedSize.Width;
				}
			}
		}

		GridUnitType _heightType;
		public GridUnitType HeightType
		{
			get { return _heightType; }
			set
			{
				if (HeightType != value)
				{
					_heightType = value;
					OnPropertyChanged(() => HeightType);
					if (_initialized && HeightType == GridUnitType.Auto)
						Height = _layoutPartSize.PreferedSize.Height;
				}
			}
		}

		double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}

		double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}

		double _minWidth;
		public double MinWidth
		{
			get { return _minWidth; }
			set
			{
				_minWidth = value;
				OnPropertyChanged(() => MinWidth);
			}
		}

		double _minHeight;
		public double MinHeight
		{
			get { return _minHeight; }
			set
			{
				_minHeight = value;
				OnPropertyChanged(() => MinHeight);
			}
		}

		bool _isWidthFixed;
		public bool IsWidthFixed
		{
			get { return _isWidthFixed; }
			set
			{
				_isWidthFixed = value;
				OnPropertyChanged(() => IsWidthFixed);
			}
		}

		bool _isHeightFixed;
		public bool IsHeightFixed
		{
			get { return _isHeightFixed; }
			set
			{
				_isHeightFixed = value;
				OnPropertyChanged(() => IsHeightFixed);
			}
		}

		int _margin;
		public int Margin
		{
			get { return _margin; }
			set
			{
				_margin = value;
				OnPropertyChanged(() => Margin);
			}
		}

		Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged(() => BackgroundColor);
			}
		}

		Color _borderColor;
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				_borderColor = value;
				OnPropertyChanged(() => BorderColor);
			}
		}

		int _borderThickness;
		public int BorderThickness
		{
			get { return _borderThickness; }
			set
			{
				_borderThickness = value;
				OnPropertyChanged(() => BorderThickness);
			}
		}

		string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged(() => Title);
			}
		}

		public override string Header
		{
			get { return "Общее"; }
		}
		public override void CopyProperties()
		{
			Height = _layoutPartSize.Height;
			HeightType = _layoutPartSize.HeightType;
			IsHeightFixed = _layoutPartSize.IsHeightFixed;
			IsWidthFixed = _layoutPartSize.IsWidthFixed;
			MinHeight = _layoutPartSize.MinHeight;
			MinWidth = _layoutPartSize.MinWidth;
			Width = _layoutPartSize.Width;
			WidthType = _layoutPartSize.WidthType;
			Margin = _layoutPartSize.Margin;
			BackgroundColor = _layoutPartSize.BackgroundColor;
			BorderColor = _layoutPartSize.BorderColor;
			BorderThickness = _layoutPartSize.BorderThickness;
			Title = _layoutPartViewModel.Title;
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			if (_layoutPartSize.Height != Height || _layoutPartSize.HeightType != HeightType || _layoutPartSize.IsHeightFixed != IsHeightFixed || _layoutPartSize.IsWidthFixed != IsWidthFixed || _layoutPartSize.MinHeight != MinHeight || _layoutPartSize.MinWidth != MinWidth || _layoutPartSize.Width != Width || _layoutPartSize.WidthType != WidthType || _layoutPartSize.Margin != Margin || _layoutPartSize.BackgroundColor != BackgroundColor || _layoutPartSize.BorderColor != BorderColor || _layoutPartSize.BorderThickness != BorderThickness || (_layoutPartViewModel.Title != Title))
			{
				_layoutPartSize.Height = Height;
				_layoutPartSize.HeightType = HeightType;
				_layoutPartSize.IsHeightFixed = IsHeightFixed;
				_layoutPartSize.IsWidthFixed = IsWidthFixed;
				_layoutPartSize.MinHeight = MinHeight;
				_layoutPartSize.MinWidth = MinWidth;
				_layoutPartSize.Width = Width;
				_layoutPartSize.WidthType = WidthType;
				_layoutPartSize.Margin = Margin;
				_layoutPartSize.BackgroundColor = BackgroundColor;
				_layoutPartSize.BorderColor = BorderColor;
				_layoutPartSize.BorderThickness = BorderThickness;
				_layoutPartViewModel.LayoutPart.Title = string.IsNullOrEmpty(Title) ? null : Title;
				return true;
			}
			return false;
		}
	}
}