using System;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class FilterBaseViewModel<T> : SaveCancelDialogViewModel
		where T : FilterBase
	{
		public FilterBaseViewModel(T filter)
		{
			Title = "Фильтр";
			Filter = filter;
			Initialize();
			Update();
		}

		protected virtual void Initialize()
		{
			;
		}

		protected virtual void Update()
		{
			switch (Filter.WithDeleted)
			{
				case DeletedType.Deleted:
					WithDeleted = true;
					OnlyDeleted = true;
					break;
				case DeletedType.All:
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


		protected override bool CanSave()
		{
			return true;
		}

		protected override bool Save()
		{
			if (OnlyDeleted && WithDeleted)
				Filter.WithDeleted = DeletedType.Deleted;
			else if (WithDeleted)
				Filter.WithDeleted = DeletedType.All;
			else
				Filter.WithDeleted = DeletedType.Not;
			Filter.RemovalDates.StartDate = RemovalDatesStart;
			Filter.RemovalDates.EndDate = RemovalDatesEnd;
			return true;
		}
	}
}
