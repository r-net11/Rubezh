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
	}
}
