using FiresecAPI.Journal;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace JournalModule.ViewModels
{
	public class SortTypeViewModel : BaseViewModel
	{
		public SortTypeViewModel(ArchiveFilter filter)
		{
			SortTypes = new ObservableCollection<ArchiveSortType>(Enum.GetValues(typeof(ArchiveSortType)).OfType<ArchiveSortType>()); ;
			SelectedSortType = filter.SortType;
			IsSortDesc = filter.IsSortDesc;
		}

		public ObservableCollection<ArchiveSortType> SortTypes { get; private set; }

		ArchiveSortType _SelectedSortType;
		public ArchiveSortType SelectedSortType
		{
			get { return _SelectedSortType; }
			set
			{
				_SelectedSortType = value;
				OnPropertyChanged(() => SelectedSortType);
			}
		}

		bool _IsSortDesc;
		public bool IsSortDesc
		{
			get { return _IsSortDesc; }
			set
			{
				_IsSortDesc = value;
				OnPropertyChanged(() => IsSortDesc);
			}
		}
	}
}
