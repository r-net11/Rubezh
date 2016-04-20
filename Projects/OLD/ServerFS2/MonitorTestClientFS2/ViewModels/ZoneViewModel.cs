using System;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using ServerFS2.Monitoring;

namespace MonitorTestClientFS2.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public Zone Zone { get; private set; }

		public ZoneViewModel(Zone zone)
		{
			SetGuardCommand = new RelayCommand(OnSetGuard, CanSetResetGuard);
			ResetGuardCommand = new RelayCommand(OnResetGuard, CanSetResetGuard);
			Zone = zone;
			Zone.ZoneState.StateChanged += new Action(ZoneState_StateChanged);
		}

		void ZoneState_StateChanged()
		{
			StateType = Zone.ZoneState.StateType;
			ZoneState = Zone.ZoneState;
			OnPropertyChanged("StateType");
			OnPropertyChanged("ZoneState");
		}

		public StateType StateType { get; private set; }
		public ZoneState ZoneState { get; private set; }

		public RelayCommand SetGuardCommand { get; private set; }
		void OnSetGuard()
		{
			MonitoringManager.AddTaskSetGuard(Zone, "Пользователь");
		}

		public RelayCommand ResetGuardCommand { get; private set; }
		void OnResetGuard()
		{
			MonitoringManager.AddTaskSetGuard(Zone, "Пользователь");
		}

		bool CanSetResetGuard()
		{
			return Zone.ZoneType == ZoneType.Guard;
		}
	}
}