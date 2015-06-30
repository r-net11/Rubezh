﻿using System;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class DateSelectionViewModel : SaveCancelDialogViewModel
	{
		DateTime _dateTime;
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
			Title = "Выберите дату";
			DateTime = dateTime;
		}
	}
}