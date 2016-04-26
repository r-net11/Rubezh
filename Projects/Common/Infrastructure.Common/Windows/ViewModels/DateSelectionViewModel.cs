using System;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class DateSelectionViewModel : SaveCancelDialogViewModel
	{
		private DateTime _dateTime;

		public DateTime DateTime
		{
			get { return _dateTime; }
			set
			{
				_dateTime = value;
				OnPropertyChanged(() => DateTime);
			}
		}

		public DateSelectionViewModel(DateTime dateTime)
		{
			Title = Resources.Language.DateSelectionViewModel.DateSelectionViewModel_Title;
			DateTime = dateTime;
		}
	}
}