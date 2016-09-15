using System;
using Localization.Common.InfrastructureCommon;

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
			Title = CommonResources.ChooseDate;
			DateTime = dateTime;
		}
	}
}