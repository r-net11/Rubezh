using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Common
{
	public class ObservableCollectionAdv<T> : ObservableCollection<T>
	{
		public void RemoveRange(int index, int count)
		{
			CheckReentrancy();
			((List<T>)Items).RemoveRange(index, count);
			OnReset();
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			CheckReentrancy();
			((List<T>)Items).InsertRange(index, collection);
			OnReset();
		}

		private void OnReset()
		{
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		private void OnPropertyChanged(string propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}
	}
}