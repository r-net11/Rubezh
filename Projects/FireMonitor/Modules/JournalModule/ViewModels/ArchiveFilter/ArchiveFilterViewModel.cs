using System;
using RubezhAPI.Journal;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;
using System.Collections.ObjectModel;

namespace JournalModule.ViewModels
{
	public class ArchiveFilterViewModel : DialogViewModel
	{
		public bool IsShowDateTime { get; private set; }
		public ArchiveDateTimeViewModel ArchiveDateTimeViewModel { get; private set; }
		public FilterNamesViewModel FilterNamesViewModel { get; private set; }
		public FilterObjectsViewModel FilterObjectsViewModel { get; private set; }
		
		public ArchiveFilterViewModel(JournalFilter filter, bool isShowDateTime)
		{
			Title = "Настройки фильтра";
			ClearCommand = new RelayCommand(OnClear);
			SaveCommand = new RelayCommand(OnSave);
			CancelCommand = new RelayCommand(OnCancel);

			if (filter == null)
				filter = new JournalFilter();
			IsShowDateTime = isShowDateTime;
			if(IsShowDateTime)
				ArchiveDateTimeViewModel = new ArchiveDateTimeViewModel();
			FilterNamesViewModel = new FilterNamesViewModel(filter);
			FilterObjectsViewModel = new FilterObjectsViewModel(filter);
			
		}

		void Initialize(JournalFilter filter)
		{
			FilterNamesViewModel.Initialize(filter);
			FilterObjectsViewModel.Initialize(filter);
		}

		public JournalFilter GetModel()
		{
			var archiveFilter = FilterNamesViewModel.GetModel();
			var objectsFilter = FilterObjectsViewModel.GetModel();
			archiveFilter.JournalObjectTypes = objectsFilter.JournalObjectTypes;
			archiveFilter.ObjectUIDs = objectsFilter.ObjectUIDs;
			return archiveFilter;
		}

		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
			Initialize(new JournalFilter());
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			if (IsShowDateTime)
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
			}
			Close(true);
		}
		public RelayCommand CancelCommand { get; private set; }
		void OnCancel()
		{
			Close(false);
		}
	}
}