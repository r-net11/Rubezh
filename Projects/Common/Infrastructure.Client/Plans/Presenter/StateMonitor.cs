using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Presenter;
using FiresecAPI.Models;
using FiresecAPI.GK;
using Common;

namespace Infrastructure.Client.Plans.Presenter
{
	public class StateMonitor : BaseMonitor<Plan>
	{
		private List<IDeviceState<XStateClass>> _states;

		public StateMonitor(Plan plan, Action callBack)
			: base(plan, callBack)
		{
			_states = new List<IDeviceState<XStateClass>>();
		}

		public void AddState(IStateProvider provider)
		{
			AddState(provider == null ? null : provider.StateClass);
		}
		public void AddState(IDeviceState<XStateClass> state)
		{
			if (state != null)
			{
				_states.Add(state);
				state.StateChanged += CallBack;
			}
		}

		public virtual XStateClass GetState()
		{
			var result = XStateClass.No;
			_states.ForEach(state =>
			{
				var stateClass = state.StateType;
				var xstate = state as XState;
				if (xstate != null && xstate.Device != null && xstate.Device.DriverType == XDriverType.AM1_T && stateClass == XStateClass.Fire2)
					stateClass = XStateClass.Info;
				if (stateClass < result)
					result = stateClass;
			});
			return result;
		}
	}
}
