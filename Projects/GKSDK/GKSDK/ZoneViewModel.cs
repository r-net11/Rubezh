using System;
using System.Windows;
using FiresecAPI.Models;
using XFiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace GKSDK
{
	public class ZoneViewModel : BaseViewModel
	{
		public ZoneViewModel(XState state)
		{
			ZoneState = state;
			_stateClass = state.StateClass;
			state.StateChanged += new Action(OnStateChanged);
			No = state.Zone.No;
			Name = state.Zone.Name;
		}

		public void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

		void OnStateChanged()
		{
			StateClass = ZoneState.StateClass;
		}

		public XState ZoneState { get; private set; }
		public int No { get; private set; }
		public string Name { get; private set; }

		XStateClass _stateClass;
		public XStateClass StateClass
		{
			get { return _stateClass; }
			set
			{
				_stateClass = value;
				OnPropertyChanged("StateClass");
			}
		}
	}
}