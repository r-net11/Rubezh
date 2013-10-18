using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common.TreeList;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Services.Layout;

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

		bool flag = true;
		public void Update(Layout layout)
		{
			_layout = layout;
			foreach (var d in LayoutParts)
				d.IsPresented = flag || d.LayoutPartDescription.AllowMultiple;
			flag = !flag;
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
