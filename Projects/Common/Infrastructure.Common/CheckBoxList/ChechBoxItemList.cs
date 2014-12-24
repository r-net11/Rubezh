using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;

namespace Infrastructure.Common.CheckBoxList
{
	public class CheckBoxItemList<T> : BaseViewModel, ICheckBoxItemList
		where T : ICheckBoxItem
	{
		public CheckBoxItemList()
		{
			Items = new List<T>();
		}

		public void Add(T item)
		{
			Items.Add(item);
			item.ItemsList = this;
		}

		public List<T> Items { get; private set; }
		public bool HasCheckedItems
		{
			get { return Items.Any(x => x.IsChecked == true); }
		}

		public void Update()
		{
			OnPropertyChanged(() => HasCheckedItems);
		}
	}

	public interface ICheckBoxItemList
	{
		bool HasCheckedItems { get; }
		void Update();
	}
}