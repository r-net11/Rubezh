using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeDictionaryItemViewModel<T> : BaseViewModel
	{
		public T Item { get; private set; }

		public EmployeeDictionaryItemViewModel(T item)
		{
			Item = item;
		}

		public void Update()
		{
			OnPropertyChanged("Item");
		}
	}
}