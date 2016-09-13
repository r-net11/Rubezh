using Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;

namespace Infrastructure.Common.CheckBoxList
{
	public class CheckBoxItemList<T> : BaseViewModel, ICheckBoxItemList
		where T : ICheckBoxItem
	{
		public CheckBoxItemList()
		{
			Items = new ObservableCollection<T>();
		}

		public void Add(T item)
		{
			Items.Add(item);
			item.ItemsList = this;
		}

		public ObservableCollection<T> Items { get; private set; }

		public bool HasCheckedItems
		{
			get { return Items.Any(x => x.IsChecked); }
		}

		public bool IsSingleSelection { get; set; }

		public virtual void Update()
		{
			OnPropertyChanged(() => HasCheckedItems);
		}

		public virtual void BeforeChecked()
		{
			if (IsSingleSelection)
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