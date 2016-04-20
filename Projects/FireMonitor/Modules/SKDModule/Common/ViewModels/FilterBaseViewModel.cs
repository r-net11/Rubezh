using System;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class FilterBaseViewModel<T> : SaveCancelDialogViewModel
		where T : IsDeletedFilter
	{
		public FilterBaseViewModel(T filter)
		{
			Title = "Фильтр";
			Filter = filter;

			switch (Filter.LogicalDeletationType)
			{
				case LogicalDeletationType.Deleted:
					WithDeleted = true;
					OnlyDeleted = true;
					break;
				case LogicalDeletationType.All:
					WithDeleted = true;
					OnlyDeleted = false;
					break;
				default:
					WithDeleted = false;
					OnlyDeleted = false;
					break;
			}
			RemovalDatesStart = Filter.RemovalDates.StartDate;
			RemovalDatesEnd = Filter.RemovalDates.EndDate;
		}

		T filter;
		public T Filter
		{
			get { return filter; }
			set
			{
				filter = value;
				OnPropertyChanged(() => Filter);
			}
		}

		bool withDeleted;
		public bool WithDeleted
		{
			get { return withDeleted; }
			set
			{
				withDeleted = value;
				OnPropertyChanged(() => WithDeleted);
			}
		}

		bool onlyDeleted;
		public bool OnlyDeleted
		{
			get { return onlyDeleted; }
			set
			{
				onlyDeleted = value;
				OnPropertyChanged(() => OnlyDeleted);
			}
		}

		DateTime removalDatesStart;
		public DateTime RemovalDatesStart
		{
			get { return removalDatesStart; }
			set
			{
				removalDatesStart = value;
				OnPropertyChanged(() => RemovalDatesStart);
			}
		}

		DateTime removalDatesEnd;
		public DateTime RemovalDatesEnd
		{
			get { return removalDatesEnd; }
			set
			{
				removalDatesEnd = value;
				OnPropertyChanged(() => RemovalDatesEnd);
			}
		}

		protected override bool Save()
		{
			if (OnlyDeleted && WithDeleted)
				Filter.LogicalDeletationType = LogicalDeletationType.Deleted;
			else if (WithDeleted)
				Filter.LogicalDeletationType = LogicalDeletationType.All;
			else
				Filter.LogicalDeletationType = LogicalDeletationType.Active;
			Filter.RemovalDates.StartDate = RemovalDatesStart;
			Filter.RemovalDates.EndDate = RemovalDatesEnd;
			return true;
		}
	}
}