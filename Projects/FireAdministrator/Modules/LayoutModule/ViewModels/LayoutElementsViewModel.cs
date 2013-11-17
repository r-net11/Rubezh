using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutElementsViewModel : BaseViewModel
	{
		private Layout _layout;
		public LayoutElementsViewModel()
		{
			Update();
		}

		private ObservableCollection<LayoutPartDescriptionViewModel> _layoutParts;
		public ObservableCollection<LayoutPartDescriptionViewModel> LayoutParts
		{
			get { return _layoutParts; }
			set
			{
				_layoutParts = value;
				OnPropertyChanged(() => LayoutParts);
			}
		}

		private LayoutPartDescriptionViewModel _selectedLayoutPart;
		public LayoutPartDescriptionViewModel SelectedLayoutPart
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
			foreach (var layoutPart in LayoutParts)
				if (map.ContainsKey(layoutPart.LayoutPartDescription.UID))
					layoutPart.Count = map[layoutPart.LayoutPartDescription.UID];
				else
					layoutPart.Count = 0;
			SelectedLayoutPart = LayoutParts.FirstOrDefault();
		}
		public void Update()
		{
			var list = new List<LayoutPartDescriptionViewModel>();
			foreach (var module in ApplicationService.Modules)
			{
				var layoutDeclarationModule = module as ILayoutDeclarationModule;
				if (layoutDeclarationModule != null)
					foreach (var layoutPartDescription in layoutDeclarationModule.GetLayoutPartDescriptions())
						list.Add(new LayoutPartDescriptionViewModel(layoutPartDescription));
			}
			list.Sort((x, y) => x.LayoutPartDescription.Index - y.LayoutPartDescription.Index);
			LayoutParts = new ObservableCollection<LayoutPartDescriptionViewModel>(list);
		}
	}
}
