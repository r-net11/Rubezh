using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;
using JournalModule.Events;
using RubezhAPI.Journal;

namespace JournalModule.ViewModels
{
	public class ArchiveDateTimeViewModel : BaseViewModel
	{
		public ArchiveDateTimeViewModel()
		{
			ArchiveDefaultStateTypes = new ObservableCollection<ArchiveDefaultStateType>();
			foreach (ArchiveDefaultStateType item in Enum.GetValues(typeof(ArchiveDefaultStateType)))
			{
				ArchiveDefaultStateTypes.Add(item);
			}

			SelectedArchiveDefaultStateType = ClientSettings.ArchiveDefaultState.ArchiveDefaultStateType;
			Count = ClientSettings.ArchiveDefaultState.Count;
			UseDeviceDateTime = ClientSettings.ArchiveDefaultState.UseDeviceDateTime;
			IsSortAsc = ClientSettings.ArchiveDefaultState.IsSortAsc;

			if (ArchiveFirstDate <ClientSettings.ArchiveDefaultState.StartDate)
				StartDateTime = new DateTimePairViewModel(ClientSettings.ArchiveDefaultState.StartDate);
			else
				StartDateTime = new DateTimePairViewModel(ArchiveFirstDate);

			if (ArchiveFirstDate <ClientSettings.ArchiveDefaultState.EndDate)
				EndDateTime = new DateTimePairViewModel(ClientSettings.ArchiveDefaultState.EndDate);
			else
				EndDateTime = new DateTimePairViewModel(NowDate);
		}

		public DateTime ArchiveFirstDate
		{
			get { return ArchiveViewModel.ArchiveFirstDate; }
		}

		public DateTime NowDate
		{
			get { return DateTime.Now; }
		}

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

		bool useDeviceDateTime;
		public bool UseDeviceDateTime
		{
			get { return useDeviceDateTime; }
			set
			{
				useDeviceDateTime = value;
				OnPropertyChanged(() => UseDeviceDateTime);
			}
		}

		bool _IsSortAsc;
		public bool IsSortAsc
		{
			get { return _IsSortAsc; }
			set
			{
				_IsSortAsc = value;
				OnPropertyChanged(() => IsSortAsc);
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

		public ObservableCollection<ArchiveDefaultStateType> ArchiveDefaultStateTypes { get; private set; }

		ArchiveDefaultStateType _selectedArchiveDefaultStateType;
		public ArchiveDefaultStateType SelectedArchiveDefaultStateType
		{
			get { return _selectedArchiveDefaultStateType; }
			set
			{
				_selectedArchiveDefaultStateType = value;
				OnPropertyChanged(() => SelectedArchiveDefaultStateType);

				switch (value)
				{
					case ArchiveDefaultStateType.LastDays:
					case ArchiveDefaultStateType.LastHours:
						IsCountVisible = true;
						IsStartDateVisible = false;
						IsEndDateVisible = false;
						break;

					case ArchiveDefaultStateType.FromDate:
						IsCountVisible = false;
						IsStartDateVisible = true;
						IsEndDateVisible = false;
						break;

					case ArchiveDefaultStateType.RangeDate:
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

		public void Save()
		{
			ClientSettings.ArchiveDefaultState.ArchiveDefaultStateType = SelectedArchiveDefaultStateType;
			ClientSettings.ArchiveDefaultState.StartDate = StartDateTime.DateTime;
			ClientSettings.ArchiveDefaultState.EndDate = EndDateTime.DateTime;
			ClientSettings.ArchiveDefaultState.Count = Count;
			ClientSettings.ArchiveDefaultState.UseDeviceDateTime = UseDeviceDateTime;
			ClientSettings.ArchiveDefaultState.IsSortAsc = IsSortAsc;
		}
	}
}