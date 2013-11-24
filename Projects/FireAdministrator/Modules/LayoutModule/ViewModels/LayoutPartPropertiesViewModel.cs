using FiresecAPI.Models.Layouts;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Services.Layout;
using System.Windows;
using System.Collections.ObjectModel;
using System;

namespace LayoutModule.ViewModels
{
	public class LayoutPartPropertiesViewModel : SaveCancelDialogViewModel
	{
		public LayoutPartSize LayoutPartSize { get; private set; }
		public LayoutPartPropertiesViewModel(LayoutPartSize layoutPartSize)
		{
			Title = "Свойства элемента";
			LayoutPartSize = layoutPartSize;
			UnitTypes = new ObservableCollection<GridUnitType>(Enum.GetValues(typeof(GridUnitType)).Cast<GridUnitType>());
			CopyProperties();
		}

		private void CopyProperties()
		{
			Height = LayoutPartSize.Height;
			HeightType = LayoutPartSize.HeightType;
			IsHeightFixed = LayoutPartSize.IsHeightFixed;
			IsWidthFixed = LayoutPartSize.IsWidthFixed;
			MinHeight = LayoutPartSize.MinHeight;
			MinWidth = LayoutPartSize.MinWidth;
			Width = LayoutPartSize.Width;
			WidthType = LayoutPartSize.WidthType;
		}

		public ObservableCollection<GridUnitType> UnitTypes { get; private set; }
		private GridUnitType _widthType;
		public GridUnitType WidthType
		{
			get { return _widthType; }
			set
			{
				_widthType = value;
				OnPropertyChanged(() => WidthType);
			}
		}
		private GridUnitType _heightType;
		public GridUnitType HeightType
		{
			get { return _heightType; }
			set
			{
				_heightType = value;
				OnPropertyChanged(() => HeightType);
			}
		}
		private double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}
		private double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}
		private double _minWidth;
		public double MinWidth
		{
			get { return _minWidth; }
			set
			{
				_minWidth = value;
				OnPropertyChanged(() => MinWidth);
			}
		}
		private double _minHeight;
		public double MinHeight
		{
			get { return _minHeight; }
			set
			{
				_minHeight = value;
				OnPropertyChanged(() => MinHeight);
			}
		}
		private bool _isWidthFixed;
		public bool IsWidthFixed
		{
			get { return _isWidthFixed; }
			set
			{
				_isWidthFixed = value;
				OnPropertyChanged(() => IsWidthFixed);
			}
		}
		private bool _isHeightFixed;
		public bool IsHeightFixed
		{
			get { return _isHeightFixed; }
			set
			{
				_isHeightFixed = value;
				OnPropertyChanged(() => IsHeightFixed);
			}
		}

		protected override bool CanSave()
		{
			return true;
		}
		protected override bool Save()
		{
			LayoutPartSize.Height = Height;
			LayoutPartSize.HeightType = HeightType;
			LayoutPartSize.IsHeightFixed = IsHeightFixed;
			LayoutPartSize.IsWidthFixed = IsWidthFixed;
			LayoutPartSize.MinHeight = MinHeight;
			LayoutPartSize.MinWidth = MinWidth;
			LayoutPartSize.Width = Width;
			LayoutPartSize.WidthType = WidthType;
			return base.Save();
		}
	}
}