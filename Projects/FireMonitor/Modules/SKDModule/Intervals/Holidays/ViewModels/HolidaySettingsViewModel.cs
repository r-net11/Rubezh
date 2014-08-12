using System;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class HolidaySettingsViewModel : SaveCancelDialogViewModel
	{
		public HolidaySettingsViewModel()
		{
			Title = "Настройки праздничных дней";
		}

		TimeSpan _nightStartTime;
		public TimeSpan NightStartTime
		{
			get { return _nightStartTime; }
			set
			{
				_nightStartTime = value;
				OnPropertyChanged(() => NightStartTime);
			}
		}

		TimeSpan _nightEndTime;
		public TimeSpan NightEndTime
		{
			get { return _nightEndTime; }
			set
			{
				_nightEndTime = value;
				OnPropertyChanged(() => NightEndTime);
			}
		}

		TimeSpan _eveningStartTime;
		public TimeSpan EveningStartTime
		{
			get { return _eveningStartTime; }
			set
			{
				_eveningStartTime = value;
				OnPropertyChanged(() => EveningStartTime);
			}
		}

		TimeSpan _eveningEndTime;
		public TimeSpan EveningEndTime
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