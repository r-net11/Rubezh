using StrazhAPI.Journal;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;

namespace JournalModule.ViewModels
{
	public class ArchiveFilterViewModel : DialogViewModel
	{
		public ArchiveDateTimeViewModel ArchiveDateTimeViewModel { get; private set; }
		public FilterNamesViewModel FilterNamesViewModel { get; private set; }
		public FilterObjectsViewModel FilterObjectsViewModel { get; private set; }

		public ArchiveFilterViewModel(ArchiveFilter filter)
		{
			Title = "Настройки фильтра";
			ClearCommand = new RelayCommand(OnClear);
			SaveCommand = new RelayCommand(OnSave);
			CancelCommand = new RelayCommand(OnCancel);

			if (filter == null)
				filter = new ArchiveFilter();
			ArchiveDateTimeViewModel = new ArchiveDateTimeViewModel();
			FilterNamesViewModel = new FilterNamesViewModel(filter);
			FilterObjectsViewModel = new FilterObjectsViewModel(filter);
			Initialize(filter);
		}

		void Initialize(ArchiveFilter filter)
		{
			FilterNamesViewModel.Initialize(filter);
			FilterObjectsViewModel.Initialize(filter);
		}

		public ArchiveFilter GetModel()
		{
			var archiveFilter = FilterNamesViewModel.GetModel();
			var objectsFilter = FilterObjectsViewModel.GetModel();
			foreach (var journalSubsystemTypes in objectsFilter.JournalSubsystemTypes)
			{
				if (!archiveFilter.JournalSubsystemTypes.Contains(journalSubsystemTypes))
					archiveFilter.JournalSubsystemTypes.Add(journalSubsystemTypes);
			}
			archiveFilter.JournalObjectTypes = objectsFilter.JournalObjectTypes;
			archiveFilter.ObjectUIDs = objectsFilter.ObjectUIDs;
			return archiveFilter;
		}

		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
			Initialize(new ArchiveFilter());
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			if (ArchiveDateTimeViewModel.SelectedArchiveDefaultStateType == ArchiveDefaultStateType.RangeDate)
			{
				if (ArchiveDateTimeViewModel.StartDateTime.DateTime > ArchiveDateTimeViewModel.EndDateTime.DateTime)
				{
					MessageBoxService.ShowWarning("Начальная дата должна быть меньше конечной");
					return;
				}
			}
			ArchiveDateTimeViewModel.Save();
			Close(true);
		}
		public RelayCommand CancelCommand { get; private set; }
		void OnCancel()
		{
			Close(false);
		}
	}
}