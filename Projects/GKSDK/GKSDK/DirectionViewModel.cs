using System;
using System.Windows;
using FiresecAPI.Models;
using XFiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace GKSDK
{
	public class DirectionViewModel : BaseViewModel
	{
		public DirectionViewModel(XDirectionState directionState)
		{
			DirectionState = directionState;
			_stateClass = directionState.StateClass;
			directionState.StateChanged += new Action(OnStateChanged);
			No = directionState.Direction.No;
			Name = directionState.Direction.Name;
		}

		public void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

		void OnStateChanged()
		{
			StateClass = DirectionState.StateClass;
		}

		public XDirectionState DirectionState { get; private set; }
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