using System;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient.Itv;
using Infrastructure.Common.Windows.ViewModels;

namespace ItvIntegration
{
	public class ZoneViewModel : BaseViewModel
	{
        public ZoneViewModel(ZoneState zoneState)
        {
            ZoneState = zoneState;
            _stateType = zoneState.StateType;
			ItvManager.ZoneStateChanged += new Action<ZoneState>((x) => { SafeCall(() => { OnZoneStateChanged(x); }); });
            No = zoneState.Zone.No;
            Name = zoneState.Zone.Name;
        }

		public void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

        void OnZoneStateChanged(ZoneState zoneState)
		{
            if (ZoneState == zoneState)
			{
				StateType = ZoneState.StateType;
			}
		}

		public ZoneState ZoneState { get; private set; }
        public int No { get; private set; }
		public string Name { get; private set; }

		StateType _stateType;
		public StateType StateType
		{
			get { return _stateType; }
			set
			{
				_stateType = value;
				OnPropertyChanged("StateType");
			}
		}
	}
}