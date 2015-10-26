using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System.Collections.ObjectModel;

namespace Resurs.ViewModels
{
	public class FilterJournalViewModel : SaveCancelDialogViewModel
	{
		public FilterEventsViewModel FilterEventsViewModel { get; set; }
		public DateTimeViewModel DateTimeViewModel { get; set; }
		public FilterConsumersViewModel FilterConsumersViewModel { get; set; }
		public FilterDevicesViewModel FilterDevicesViewModel { get; set; }
		public FilterTariffsViewModel FilterTariffsViewModel { get; set; }
		public FilterUsersViewModel FilterUsersViewModel { get; set; }
		public FilterJournalViewModel(Filter filter )
		{
			ClearCommand = new RelayCommand(OnClear);

			Title = "Настройки фильтра";
			DateTimeViewModel = new DateTimeViewModel(filter);
			FilterEventsViewModel = new FilterEventsViewModel(filter);
			FilterConsumersViewModel = new FilterConsumersViewModel(filter);
			FilterDevicesViewModel = new FilterDevicesViewModel(filter);
			FilterTariffsViewModel = new FilterTariffsViewModel(filter);
			FilterUsersViewModel = new FilterUsersViewModel(filter);
		}

		protected override bool Save()
		{
			if (DateTimeViewModel.SelectedStateType == StateType.RangeDate)
			{
				if (DateTimeViewModel.StartDateTime.DateTime > DateTimeViewModel.EndDateTime.DateTime)
				{
					MessageBoxService.ShowWarning("Начальная дата должна быть меньше конечной");
					return false;
				}
			}
			return base.Save();
		}
		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
			DateTimeViewModel = new DateTimeViewModel(new Filter());
			FilterEventsViewModel = new FilterEventsViewModel(new Filter());
			FilterConsumersViewModel = new FilterConsumersViewModel(new Filter());
			FilterDevicesViewModel = new FilterDevicesViewModel(new Filter());
			FilterTariffsViewModel = new FilterTariffsViewModel(new Filter());
			FilterUsersViewModel = new FilterUsersViewModel(new Filter());
			OnPropertyChanged(() => DateTimeViewModel);
			OnPropertyChanged(() => FilterEventsViewModel);
			OnPropertyChanged(() => FilterConsumersViewModel);
			OnPropertyChanged(() => FilterDevicesViewModel);
			OnPropertyChanged(() => FilterTariffsViewModel);
			OnPropertyChanged(() => FilterUsersViewModel);
		}
	}
}
