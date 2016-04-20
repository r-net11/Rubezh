using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Ribbon
{
	public class RibbonMenuViewModel : BaseViewModel
	{
		public RibbonMenuViewModel(ObservableCollection<RibbonMenuItemViewModel> items = null)
		{
			Items = items ?? new ObservableCollection<RibbonMenuItemViewModel>();
			if (Items.Count > 0)
			{
				int index = 1;
				foreach (var item in Items)
					if (item.Order == default(int))
					{
						item.Order = index;
						index++;
					}
			}
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
