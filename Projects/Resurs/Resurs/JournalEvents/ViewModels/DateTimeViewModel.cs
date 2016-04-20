using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DateTimeViewModel : BaseViewModel
	{
		public DateTimeViewModel(Filter filter)
		{
			StateTypes = new ObservableCollection<StateType>(Enum.GetValues(typeof(StateType)).Cast<StateType>());
			SelectedStateType = filter.StateType;
			Count = filter.Count;
			IsSortAsc = filter.IsSortAsc;
			PageSize = filter.PageSize;
			StartDateTime = new DateTimePairViewModel(filter.StartDate);
			EndDateTime = new DateTimePairViewModel(filter.EndDate);
		}

		public ObservableCollection<StateType> StateTypes { get; private set; }

		DateTimePairViewModel _startDateTime;
		public DateTimePairViewModel StartDateTime
		{
			get { return _startDateTime; }
			set
			{
				_startDateTime = value;
				OnPropertyChanged(() => StartDateTime);
			}
		}

		DateTimePairViewModel _endDateTime;
		public DateTimePairViewModel EndDateTime
		{
			get { return _endDateTime; }
			set
			{
				_endDateTime = value;
				OnPropertyChanged(() => EndDateTime);
			}
		}

		int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged(() => Count);
			}
		}

		StateType _selectedStateType;
		public StateType SelectedStateType
		{
			get { return _selectedStateType; }
			set
			{
				_selectedStateType = value;
				OnPropertyChanged(() => SelectedStateType);

				switch (value)
				{
					case StateType.LastDays:
					case StateType.LastHours:
						IsCountVisible = true;
						IsStartDateVisible = false;
						IsEndDateVisible = false;
						break;

					case StateType.FromDate:
						IsCountVisible = false;
						IsStartDateVisible = true;
						IsEndDateVisible = false;
						break;

					case StateType.RangeDate:
						IsCountVisible = false;
						IsStartDateVisible = true;
						IsEndDateVisible = true;
						break;
				}
			}
		}

		bool _isCountVisible;
		public bool IsCountVisible
		{
			get { return _isCountVisible; }
			set
			{
				_isCountVisible = value;
				OnPropertyChanged(() => IsCountVisible);
			}
		}

		bool _isStartDateVisible;
		public bool IsStartDateVisible
		{
			get { return _isStartDateVisible; }
			set
			{
				_isStartDateVisible = value;
				OnPropertyChanged(() => IsStartDateVisible);
			}
		}


		bool _isEndDateVisible;
		public bool IsEndDateVisible
		{
			get { return _isEndDateVisible; }
			set
			{
				_isEndDateVisible = value;
				OnPropertyChanged(() => IsEndDateVisible);
			}
		}

		bool _isSortAsc;
		public bool IsSortAsc
		{
			get { return _isSortAsc; }
			set
			{
				_isSortAsc = value;
				OnPropertyChanged(() => IsSortAsc);
			}
		}

		int _pageSize;
		public int PageSize
		{
			get { return _pageSize; }
			set
			{
				_pageSize = value;
				OnPropertyChanged(() => PageSize);
			}
		}

		public void Save(Filter filter)
		{
			switch (SelectedStateType)
			{
				case StateType.LastHours:
					filter.StartDate = DateTime.Now.AddHours(-Count);
					break;

				case StateType.LastDays:
					filter.StartDate = DateTime.Now.AddDays(-Count);
					break;

				case StateType.FromDate:
					filter.StartDate = StartDateTime.DateTime;
					break;

				case StateType.RangeDate:
					filter.StartDate = StartDateTime.DateTime;
					filter.EndDate = EndDateTime.DateTime;
					break;
			}
			filter.StateType = SelectedStateType;
			filter.Count = Count;
			filter.IsSortAsc = IsSortAsc;
			filter.PageSize = PageSize;
		}
	}
}