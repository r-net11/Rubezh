using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;

namespace GKImitator.ViewModels
{
	public class GuardZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		public GKGuardZone GuardZone { get; private set; }

		public GuardZonePropertiesViewModel(GKGuardZone guardZone)
		{
			Title = "Параметры охранной зоны";
			GuardZone = guardZone;

			SetDelay = GuardZone.SetDelay;
			ResetDelay = GuardZone.ResetDelay;
			AlarmDelay = GuardZone.AlarmDelay;
		}


		int _setDelay;
		public int SetDelay
		{
			get { return _setDelay; }
			set
			{
				_setDelay = value;
				OnPropertyChanged(() => SetDelay);
			}
		}

		int _resetDelay;
		public int ResetDelay
		{
			get { return _resetDelay; }
			set
			{
				_resetDelay = value;
				OnPropertyChanged(() => ResetDelay);
			}
		}

		int _alarmDelay;
		public int AlarmDelay
		{
			get { return _alarmDelay; }
			set
			{
				_alarmDelay = value;
				OnPropertyChanged(() => AlarmDelay);
			}
		}

		protected override bool Save()
		{
			GuardZone.SetDelay = SetDelay;
			GuardZone.ResetDelay = ResetDelay;
			GuardZone.AlarmDelay = AlarmDelay;
			return base.Save();
		}
	}
}