using System;
using System.Linq;
using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class HolidaySettingsViewModel : SaveCancelDialogViewModel
	{
		public HolidaySettingsViewModel()
		{
			Title = "Настройки праздничных дней";
		}

		private DateTime _nightStartTime;
		public DateTime NightStartTime
		{
			get { return _nightStartTime; }
			set
			{
				_nightStartTime = value;
				OnPropertyChanged(() => NightStartTime);
			}
		}

		private DateTime _nightEndTime;
		public DateTime NightEndTime
		{
			get { return _nightEndTime; }
			set
			{
				_nightEndTime = value;
				OnPropertyChanged(() => NightEndTime);
			}
		}

		private DateTime _eveningStartTime;
		public DateTime EveningStartTime
		{
			get { return _eveningStartTime; }
			set
			{
				_eveningStartTime = value;
				OnPropertyChanged(() => EveningStartTime);
			}
		}

		private DateTime _eveningEndTime;
		public DateTime EveningEndTime
		{
			get { return _eveningEndTime; }
			set
			{
				_eveningEndTime = value;
				OnPropertyChanged(() => EveningEndTime);
			}
		}

		protected override bool Save()
		{
			return true;
		}
	}
}