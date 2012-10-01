using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public class DeviceState
	{
		public Device Device { get; set; }
		public List<DeviceDriverState> States { get; set; }
		public List<ParentDeviceState> ParentStates { get; set; }
		public List<ChildDeviceState> ChildStates { get; set; }
		public List<Parameter> Parameters { get; set; }

		public DeviceState()
		{
			States = new List<DeviceDriverState>();
			ParentStates = new List<ParentDeviceState>();
			ChildStates = new List<ChildDeviceState>();
			Parameters = new List<Parameter>();
		}

		public StateType StateType
		{
			get
			{
				var stateTypes = new List<StateType>() { StateType.Norm };

				foreach (var deviceDriverState in States)
				{
					if (deviceDriverState.DriverState != null)
					{
						stateTypes.Add(deviceDriverState.DriverState.StateType);
					}
				}

				foreach (var parentDeviceState in ParentStates)
				{
					if (parentDeviceState.DriverState != null)
					{
						stateTypes.Add(parentDeviceState.DriverState.StateType);
					}
				}

				foreach (var childDeviceState in ChildStates)
				{
					stateTypes.Add(childDeviceState.StateType);
				}

				return stateTypes.Min();
			}
		}

		public List<string> StringStates
		{
			get
			{
				var stringStates = new List<string>();
				foreach (var state in States)
				{
					stringStates.Add(state.DriverState.Name);
				}
				return stringStates;
			}
		}

		public List<string> ParentStringStates
		{
			get
			{
				var parentStringStates = new List<string>();
				foreach (var parentDeviceState in ParentStates)
				{
					parentStringStates.Add(parentDeviceState.ParentDevice.Driver.ShortName + " - " + parentDeviceState.DriverState.Name);
				}
				return parentStringStates;
			}
		}

		public bool IsDisabled
		{
			get { return States.Any(x => x.DriverState.StateType == StateType.Off); }
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}

		public event Action ParametersChanged;
		public void OnParametersChanged()
		{
			if (ParametersChanged != null)
				ParametersChanged();
		}
	}
}