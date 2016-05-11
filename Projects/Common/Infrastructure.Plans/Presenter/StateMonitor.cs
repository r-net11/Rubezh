using Infrastructure.Plans.Presenter;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;

namespace Infrastructure.Plans.Presenter
{
	public class StateMonitor : BaseMonitor<Plan>
	{
		List<IDeviceState> _states;

		public StateMonitor(Plan plan, Action callBack)
			: base(plan, callBack)
		{
			_states = new List<IDeviceState>();
		}

		public void AddState(IStateProvider provider)
		{
			AddState(provider == null ? null : provider.StateClass);
		}
		public void AddState(IDeviceState state)
		{
			if (state != null)
			{
				_states.Add(state);
				state.StateChanged += CallBack;
			}
		}

		public virtual NamedStateClass GetNamedStateClass()
		{
			var namedStateClass = new NamedStateClass();
			_states.ForEach(state =>
			{
				var stateClass = state.StateClass;
				if (stateClass < namedStateClass.StateClass)
				{
					namedStateClass.StateClass = stateClass;
					namedStateClass.Name = state.Name;
				}
			});
			return namedStateClass;
		}
	}
}