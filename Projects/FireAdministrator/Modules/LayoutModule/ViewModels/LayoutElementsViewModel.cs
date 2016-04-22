using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models.Layouts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LayoutModule.ViewModels
{
	public class LayoutElementsViewModel : BaseViewModel
	{
		Layout _layout;
		Dictionary<Guid, LayoutPartDescriptionViewModel> _map;
		public LayoutElementsViewModel()
		{
			Update();
		}

		ObservableCollection<LayoutPartDescriptionGroupViewModel> _layoutParts;
		public ObservableCollection<LayoutPartDescriptionGroupViewModel> LayoutParts
		{
			get { return _layoutParts; }
			set
			{
				_layoutParts = value;
				OnPropertyChanged(() => LayoutParts);
			}
		}

		LayoutPartDescriptionGroupViewModel _selectedLayoutPart;
		public LayoutPartDescriptionGroupViewModel SelectedLayoutPart
		{
			get { return _selectedLayoutPart; }
			set
			{
				_selectedLayoutPart = value;
				OnPropertyChanged(() => SelectedLayoutPart);
			}
		}

		public void Update(Layout layout)
		{
			_layout = layout;
			var map = new Dictionary<Guid, int>();
			if (_layout != null)
				foreach (var layoutPart in _layout.Parts)
					if (map.ContainsKey(layoutPart.DescriptionUID))
						map[layoutPart.DescriptionUID]++;
					else
						map.Add(layoutPart.DescriptionUID, 1);
			foreach (var layoutPart in _map)
				if (map.ContainsKey(layoutPart.Key))
					layoutPart.Value.Count = map[layoutPart.Key];
				else
					layoutPart.Value.Count = 0;
			SelectedLayoutPart = LayoutParts.FirstOrDefault();
		}
		public void Update()
		{
			var list = new List<LayoutPartDescriptionGroupViewModel>();
			_map = new Dictionary<Guid, LayoutPartDescriptionViewModel>();
			var groups = Enum.GetValues(typeof(LayoutPartDescriptionGroup)).Cast<LayoutPartDescriptionGroup>().
				Where(item => item != LayoutPartDescriptionGroup.Root).
				ToDictionary(item => item, item => new LayoutPartDescriptionGroupViewModel(item));
			foreach (var module in ApplicationService.Modules)
			{
				var layoutDeclarationModule = module as ILayoutDeclarationModule;
				if (layoutDeclarationModule != null)
					foreach (var layoutPartDescription in layoutDeclarationModule.GetLayoutPartDescriptions())
					{
						var layoutPartDescriptionViewModel = new LayoutPartDescriptionViewModel(layoutPartDescription);
						if (layoutPartDescription.Group == LayoutPartDescriptionGroup.Root)
							list.Add(layoutPartDescriptionViewModel);
						else
							groups[layoutPartDescription.Group].AddChild(layoutPartDescriptionViewModel);
						_map.Add(layoutPartDescription.UID, layoutPartDescriptionViewModel);
					}
			}
			list.AddRange(groups.Values.Where(item => item.ChildrenCount > 0));
			list.Sort(Comparer);
			LayoutParts = new ObservableCollection<LayoutPartDescriptionGroupViewModel>(list);
		}
		public LayoutPartDescriptionViewModel GetLayoutPartDescription(Guid guid)
		{
			return _map.ContainsKey(guid) ? _map[guid] : null;
		}

		int Comparer(LayoutPartDescriptionGroupViewModel x, LayoutPartDescriptionGroupViewModel y)
		{
			var dx = x as LayoutPartDescriptionViewModel;
			var dy = y as LayoutPartDescriptionViewModel;
			return dx == null ?
				(dy == null ? string.Compare(x.GroupName, y.GroupName) : -1) :
				(dy == null ? 1 : dx.LayoutPartDescription.Index - dy.LayoutPartDescription.Index);
		}
	}
}