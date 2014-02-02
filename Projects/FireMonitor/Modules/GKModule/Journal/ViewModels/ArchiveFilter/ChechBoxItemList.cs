using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class CheckBoxItemList : BaseViewModel
	{
		public CheckBoxItemList()
		{
			Items = new List<ICheckBoxItem>();
		}

		public CheckBoxItemList(List<ICheckBoxItem> items)
		{
			Items = items;
			Items.ForEach(x => x.ItemsList = this);
		}

		public void Add(ICheckBoxItem item)
		{
			Items.Add(item);
			item.ItemsList = this;
		}

		public List<ICheckBoxItem> Items { get; private set; }
		public bool HasCheckedItems
		{
			get { return Items.Any(x => x.IsChecked == true); }
		}
	}
}