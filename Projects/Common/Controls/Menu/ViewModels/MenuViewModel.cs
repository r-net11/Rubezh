using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Controls.Menu.ViewModels
{
	public class MenuViewModel : BaseViewModel
	{
		public MenuViewModel()
		{
			Items = new ObservableCollection<BaseViewModel>();
		}
		public MenuViewModel(IEnumerable<BaseViewModel> items)
		{
			Items = new ObservableCollection<BaseViewModel>(items);
		}

		private ObservableCollection<BaseViewModel> _items;
		public ObservableCollection<BaseViewModel> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged("Items");
			}
		}
	}
}