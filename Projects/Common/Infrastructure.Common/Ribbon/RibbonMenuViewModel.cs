using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Ribbon
{
	public class RibbonMenuViewModel : BaseViewModel
	{
		public RibbonMenuViewModel(ObservableCollection<RibbonMenuItemViewModel> items = null)
		{
			Items = items;
		}

		private ObservableCollection<RibbonMenuItemViewModel> _items;
		public ObservableCollection<RibbonMenuItemViewModel> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged(() => Items);
			}
		}
	}

}
