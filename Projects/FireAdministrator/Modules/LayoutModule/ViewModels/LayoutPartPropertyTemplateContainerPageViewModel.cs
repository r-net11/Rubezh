using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Microsoft.Win32;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.Windows.Data;
using FiresecClient;
using Common;

namespace LayoutModule.ViewModels
{
	public class LayoutPartPropertyTemplateContainerPageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartTemplateContainerViewModel _layoutPartViewModel;
		private bool _layoutChanged;

		public LayoutPartPropertyTemplateContainerPageViewModel(LayoutPartTemplateContainerViewModel layoutPartViewModel)
		{
			_layoutPartViewModel = layoutPartViewModel;
			Layouts = new ObservableCollection<LayoutViewModel>();
			MonitorLayoutsViewModel.Instance.Layouts.Where(item=>item != MonitorLayoutsViewModel.Instance.SelectedLayout).ForEach(item => Layouts.Add(item));
		}

		private ObservableCollection<LayoutViewModel> _layouts;
		public ObservableCollection<LayoutViewModel> Layouts
		{
			get { return _layouts; }
			set
			{
				_layouts = value;
				OnPropertyChanged(() => Layouts);
			}
		}

		private LayoutViewModel _selectedLayout;
		public LayoutViewModel SelectedLayout
		{
			get { return _selectedLayout; }
			set
			{
				if (_selectedLayout != value)
					_layoutChanged = true;
				_selectedLayout = value;
				OnPropertyChanged(() => SelectedLayout);
			}
		}

		public override string Header
		{
			get { return "Макет"; }
		}
		public override void CopyProperties()
		{
			SelectedLayout = _layoutPartViewModel.Layout == null ? null : Layouts.FirstOrDefault(item => item.Layout.UID == _layoutPartViewModel.Layout.UID);
			_layoutChanged = false;
		}
		public override bool CanSave()
		{
			return SelectedLayout != null;
		}
		public override bool Save()
		{
			if (_layoutChanged)
			{
				var properties = (LayoutPartTemplateContainerProperties)_layoutPartViewModel.Properties;
				properties.SourceUID = SelectedLayout.Layout.UID;
				_layoutPartViewModel.UpdateLayout(SelectedLayout.Layout);
				return true;
			}
			return false;
		}
	}
}
