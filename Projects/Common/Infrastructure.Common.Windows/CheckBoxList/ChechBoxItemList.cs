using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Infrastructure.Common.Windows.CheckBoxList
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

		public bool IsSingleSelection { get; set; }

		public virtual void Update()
		{
			OnPropertyChanged(() => HasCheckedItems);
		}

		public virtual void BeforeChecked()
		{
			if(IsSingleSelection)
				Items.ForEach(x => x.SetFromParent(false));
		}
	}

	public interface ICheckBoxItemList
	{
		bool HasCheckedItems { get; }
		void BeforeChecked();
		void Update();
	}
}