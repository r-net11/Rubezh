using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrustructure.Plans.Presenter;

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

		public virtual StateTypeName<XStateClass> GetStateTypeName()
		{
			var stateTypeName = new StateTypeName<XStateClass>() { StateType = XStateClass.No, Name = "Нет" };
			_states.ForEach(state =>
			{
				var stateClass = state.StateType;
				if (stateClass < stateTypeName.StateType)
				{
					stateTypeName.StateType = stateClass;
					stateTypeName.Name = state.StateTypeName;
				}
			});
			return stateTypeName;
		}
	}
}