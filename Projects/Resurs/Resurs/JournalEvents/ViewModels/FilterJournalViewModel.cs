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
		public FilterJournalViewModel(Filter filter )
		{
			Title = "Настройки фильтра";
			FilterEventsViewModel = new FilterEventsViewModel(filter);
			DateTimeViewModel = new DateTimeViewModel(filter);
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
