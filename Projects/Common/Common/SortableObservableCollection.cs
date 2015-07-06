using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Common
{
	public class SortableObservableCollection<T> : ObservableCollection<T>
	{
		public SortableObservableCollection()
			: base()
		{
		}

		public SortableObservableCollection(List<T> list)
			: base(list)
		{
		}

		public SortableObservableCollection(IEnumerable<T> list)
			: base(list)
		{
		}

		#region Sorting

		public void Sort<TKey>(Func<T, TKey> keySelector)
		{
			InternalSort(Items.OrderBy(keySelector));
		}

		public void SortDescending<TKey>(Func<T, TKey> keySelector)
		{
			InternalSort(Items.OrderByDescending(keySelector));
		}

		public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
		{
			InternalSort(Items.OrderBy(keySelector, comparer));
		}

		private void InternalSort(IEnumerable<T> sortedItems)
		{
			int index = 0;
			foreach (var item in sortedItems)
			{
				Move(IndexOf(item), index);
				index++;
			}
		}

		#endregion Sorting
	}
}