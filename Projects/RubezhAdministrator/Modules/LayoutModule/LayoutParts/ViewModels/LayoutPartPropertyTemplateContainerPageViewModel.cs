using Common;
using Infrastructure.Client.Layout;
using Infrastructure.Common.Services.Layout;
using LayoutModule.ViewModels;
using RubezhAPI.Models.Layouts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartPropertyTemplateContainerPageViewModel : LayoutPartPropertyPageViewModel
	{
		LayoutPartTemplateContainerViewModel _layoutPartViewModel;
		bool _layoutChanged;

		public LayoutPartPropertyTemplateContainerPageViewModel(LayoutPartTemplateContainerViewModel layoutPartViewModel)
		{
			_layoutPartViewModel = layoutPartViewModel;
			Layouts = new ObservableCollection<LayoutViewModel>();
			MonitorLayoutsViewModel.Instance.Layouts.Where(item => CheckLayoutCycling(item.Layout)).ForEach(item => Layouts.Add(item));
		}

		ObservableCollection<LayoutViewModel> _layouts;
		public ObservableCollection<LayoutViewModel> Layouts
		{
			get { return _layouts; }
			set
			{
				_layouts = value;
				OnPropertyChanged(() => Layouts);
			}
		}

		LayoutViewModel _selectedLayout;
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
				var properties = (LayoutPartReferenceProperties)_layoutPartViewModel.Properties;
				properties.ReferenceUID = SelectedLayout.Layout.UID;
				_layoutPartViewModel.UpdateLayout(SelectedLayout.Layout);
				return true;
			}
			return false;
		}

		bool CheckLayoutCycling(Layout layout, List<Guid> parents = null)
		{
			if (parents == null)
				parents = new List<Guid>() { MonitorLayoutsViewModel.Instance.SelectedLayout.Layout.UID };
			if (parents.Contains(layout.UID))
				return false;
			parents.Add(layout.UID);
			foreach (var layoutPart in layout.Parts.Where(part => part.DescriptionUID == LayoutPartIdentities.TemplateContainer))
			{
				var property = (LayoutPartReferenceProperties)layoutPart.Properties;
				if (property != null)
				{
					var childLayout = MonitorLayoutsViewModel.Instance.Layouts.FirstOrDefault(item => item.Layout.UID == property.ReferenceUID);
					if (childLayout != null && !CheckLayoutCycling(childLayout.Layout, parents))
						return false;
				}
			}
			parents.Remove(layout.UID);
			return true;
		}
	}
}