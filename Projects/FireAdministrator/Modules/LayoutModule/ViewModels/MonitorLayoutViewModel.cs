using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class MonitorLayoutViewModel : BaseViewModel
	{
		public LayoutDesignerViewModel LayoutDesignerViewModel { get; private set; }
		public LayoutElementsViewModel LayoutElementsViewModel { get; private set; }
		public LayoutUsersViewModel LayoutUsersViewModel { get; private set; }

		public MonitorLayoutViewModel()
		{
			LayoutElementsViewModel = new LayoutElementsViewModel();
			LayoutDesignerViewModel = new LayoutDesignerViewModel(LayoutElementsViewModel);
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
