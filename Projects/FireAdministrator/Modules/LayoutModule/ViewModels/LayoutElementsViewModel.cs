using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common.TreeList;
using FiresecAPI.Models.Layouts;

namespace LayoutModule.ViewModels
{
	public class LayoutElementsViewModel : BaseViewModel
	{
		private Layout _layout;
		public LayoutElementsViewModel()
		{
			Update();
		}

		private ObservableCollection<TreeNodeViewModel> _items;
		public ObservableCollection<TreeNodeViewModel> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged(() => Items);
			}
		}

		private TreeNodeViewModel _selectedItem;
		public TreeNodeViewModel SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				OnPropertyChanged(() => SelectedItem);
			}
		}

		public void Update(Layout layout)
		{
		}
		public void Update()
		{
		}
	}
}
