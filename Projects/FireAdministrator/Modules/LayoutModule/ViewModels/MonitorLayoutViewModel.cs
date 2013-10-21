using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models.Layouts;

namespace LayoutModule.ViewModels
{
	public class MonitorLayoutViewModel : BaseViewModel
	{
		public LayoutDesignerViewModel LayoutDesignerViewModel { get; private set; }
		public LayoutElementsViewModel LayoutElementsViewModel { get; private set; }
		public LayoutUsersViewModel LayoutUsersViewModel { get; private set; }

		public MonitorLayoutViewModel()
		{
			LayoutDesignerViewModel = new LayoutDesignerViewModel();
			LayoutElementsViewModel = new LayoutElementsViewModel();
			LayoutUsersViewModel = new LayoutUsersViewModel();
		}

		private LayoutViewModel _layoutViewModel;
		public LayoutViewModel LayoutViewModel
		{
			get { return _layoutViewModel; }
			set
			{
				_layoutViewModel = value;
				OnPropertyChanged(() => LayoutViewModel);
				OnPropertyChanged(() => Layout);
				Update();
			}
		}
		public Layout Layout
		{
			get { return LayoutViewModel == null ? null : LayoutViewModel.Layout; }
		}

		public void Update()
		{
			LayoutDesignerViewModel.Update(Layout);
			LayoutUsersViewModel.Update(Layout);
			LayoutElementsViewModel.Update(Layout);
		}
	}
}