using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
using RubezhAPI.Models.Layouts;
using System.Collections.Generic;
using System.Windows.Media;

namespace LayoutModule.ViewModels
{
	public class LayoutPropertiesViewModel : SaveCancelDialogViewModel
	{
		public class IPObject
		{
			public string IP { get; set; }
		}
		public Layout Layout { get; private set; }
		public LayoutUsersViewModel LayoutUsersViewModel { get; private set; }
		public IPFilterViewModel IPFilterViewModel { get; private set; }
		List<string> otherCaptions;
		public LayoutPropertiesViewModel(Layout layout, LayoutUsersViewModel layoutUsersViewModel, List<string> otherCaptions)
		{
			Title = "Свойства элемента: Макет интерфейса ОЗ";
			Layout = layout ?? new Layout(otherCaptions);
			LayoutUsersViewModel = layoutUsersViewModel;
			this.otherCaptions = otherCaptions;
			LayoutUsersViewModel.Update(Layout);
			IPFilterViewModel = new IPFilterViewModel(Layout.HostNameOrAddressList);
			CopyProperties();
		}

		private void CopyProperties()
		{
			Caption = Layout.Caption;
			Description = Layout.Description;
			SplitterColor = Layout.SplitterColor.ToWindowsColor();
			SplitterSize = Layout.SplitterSize;
			BorderColor = Layout.BorderColor.ToWindowsColor();
			BorderThickness = Layout.BorderThickness;
			BackgroundColor = Layout.BackgroundColor.ToWindowsColor();
			Padding = Layout.Padding;
			IsRibbonEnabled = Layout.IsRibbonEnabled;
		}

		private string _caption;
		public string Caption
		{
			get { return _caption; }
			set
			{
				_caption = value;
				OnPropertyChanged(() => Caption);
			}
		}

		private string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		private Color _splitterColor;
		public Color SplitterColor
		{
			get { return _splitterColor; }
			set
			{
				_splitterColor = value;
				OnPropertyChanged(() => SplitterColor);
			}
		}
		private int _splitterSize;
		public int SplitterSize
		{
			get { return _splitterSize; }
			set
			{
				_splitterSize = value;
				OnPropertyChanged(() => SplitterSize);
			}
		}

		private int _borderThickness;
		public int BorderThickness
		{
			get { return _borderThickness; }
			set
			{
				_borderThickness = value;
				OnPropertyChanged(() => BorderThickness);
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

		private int _padding;
		public int Padding
		{
			get { return _padding; }
			set
			{
				_padding = value;
				OnPropertyChanged(() => Padding);
			}
		}

		private bool _isRibbonEnabled;
		public bool IsRibbonEnabled
		{
			get { return _isRibbonEnabled; }
			set
			{
				_isRibbonEnabled = value;
				OnPropertyChanged(() => IsRibbonEnabled);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Caption);
		}
		protected override bool Save()
		{
			if (IsValidate())
			{
				Layout.Caption = Caption;
				Layout.Description = Description;
				LayoutUsersViewModel.Save();
				Layout.HostNameOrAddressList = IPFilterViewModel.GetModel();
				Layout.SplitterColor = SplitterColor.ToRubezhColor();
				Layout.SplitterSize = SplitterSize;
				Layout.BorderColor = BorderColor.ToRubezhColor();
				Layout.BorderThickness = BorderThickness;
				Layout.BackgroundColor = BackgroundColor.ToRubezhColor();
				Layout.Padding = Padding;
				Layout.IsRibbonEnabled = IsRibbonEnabled;
				return base.Save();
			}
			return false;
		}
		bool IsValidate()
		{
			if (otherCaptions.Contains(Caption))
			{
				MessageBoxService.Show("Макет с таким именем уже существует.");
				return false;
			}
			return true;
		}
	}
}