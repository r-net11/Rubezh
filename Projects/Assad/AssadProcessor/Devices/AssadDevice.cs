using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI;
using FiresecClient;

namespace AssadProcessor.Devices
{
	public class AssadDevice : AssadBase
	{
		public string Address { get; set; }
		public string Id { get; set; }
		public string DriverId { get; set; }

		public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
		{
			if (innerDevice.param == null)
				return;

			var addressParameter = innerDevice.param.FirstOrDefault(x => x.param == "Адрес");
			if (addressParameter != null)
				Address = addressParameter.value;
			else
				Address = null;

			var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == new Guid(DriverId));
			switch (driver.DriverType)
			{
				case DriverType.Computer:
				case DriverType.PumpStation:
				case DriverType.JokeyPump:
				case DriverType.Compressor:
				case DriverType.DrenazhPump:
				case DriverType.CompensationPump:
					Address = "0";
					break;

				case DriverType.MS_1:
				case DriverType.MS_2:
					string serialNo = null;
					if (innerDevice.param.Any(x => x.param == "Серийный номер"))
						serialNo = innerDevice.param.FirstOrDefault(x => x.param == "Серийный номер").value;

					if (string.IsNullOrEmpty(serialNo))
					{
						Address = "0";
					}
					else
					{
						Address = serialNo;
					}
					break;
			}

			SetPath();
		}

		public override Assad.DeviceType GetStates()
		{
			var deviceType = new Assad.DeviceType();
			deviceType.deviceId = DeviceId;
			var states = new List<Assad.DeviceTypeState>();

			var deviceState = ConfigurationHelper.GetDeviceState(Id);
			if (deviceState != null)
			{
				var mainState = new Assad.DeviceTypeState();
				mainState.state = "Состояние";
				mainState.value = deviceState.StateType.ToDescription();
				states.Add(mainState);
				string str = " ";
				switch (mainState.value)
				{
					case "Тревога":
					case "Внимание (предтревожное)":
					case "Неисправность":
					case "Требуется обслуживание":
					case "Норма(*)":
						str = mainState.value;
						break;
				}

				if (str != " ")
				{
					FireEvent(str);
				}

				foreach (var parameter in deviceState.Parameters)
				{
					if (parameter.Visible)
					{
						var parameterState = new Assad.DeviceTypeState();
						parameterState.state = parameter.Caption;
						parameterState.value = parameter.Value;

						if ((string.IsNullOrEmpty(parameter.Value)) || (parameter.Value == "<NULL>"))
						{
							parameterState.value = " ";
						}

						states.Add(parameterState);
					}
				}

				if (FiresecManager.Devices.Any(x => x.PathId == Id))
				{
					var device = FiresecManager.Devices.FirstOrDefault(x => x.PathId == Id);

					var state0 = new Assad.DeviceTypeState();
					state0.state = "Примечание";
					if ((string.IsNullOrEmpty(device.Description)) || (device.Description == "<NULL>"))
					{
						device.Description = " ";
					}
					state0.value = device.Description;
					states.Add(state0);

					if (device.Driver.IsZoneDevice)
					{
						var state1 = new Assad.DeviceTypeState();
						state1.state = "Зона";

						if (device.Zone != null)
							state1.value = device.Zone.No.ToString();
						else
							state1.value = " ";
						states.Add(state1);
					}
					else
					{
						if (device.Driver.IsZoneLogicDevice)
						{
							var state2 = new Assad.DeviceTypeState();
							state2.state = "Настройка включения по состоянию зон";
							string zonelogicstring = device.ZoneLogic.ToString();
							state2.value = zonelogicstring;
							states.Add(state2);
						}
					}
					foreach (var propinfo in device.Driver.Properties)
					{
						var loopState = new Assad.DeviceTypeState();
						string name = propinfo.Name;
						string value = propinfo.Default;
						loopState.state = propinfo.Caption;

						if (propinfo.Caption == "Адрес")
						{
							loopState.state = "Адрес USB устройства в сети RS-485";
						}

						if (device.Properties.Any(x => x.Name == name))
						{
							var property = device.Properties.FirstOrDefault(x => x.Name == name);
							value = property.Value;

							if (string.IsNullOrEmpty(property.Value))
							{
								value = propinfo.Default;
							}
						}

						var parameter = propinfo.Parameters.FirstOrDefault(x => x.Value == value);
						if (parameter != null)
						{
							value = parameter.Name;
						}

						loopState.value = value;

						if (propinfo.Visible)
							states.Add(loopState);
					}
				}
			}
			else
			{
				var mainState = new Assad.DeviceTypeState();
				mainState.state = "Состояние";
				mainState.value = "Отсутствует в конфигурации сервера оборудования";
				states.Add(mainState);
			}

			deviceType.state = states.ToArray();
			return deviceType;
		}

		public override void FireEvent(string eventName)
		{
			var eventType = new Assad.CPeventType();

			eventType.deviceId = DeviceId;
			eventType.eventTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
			eventType.eventId = eventName;
			eventType.alertLevel = "0";

			var deviceState = ConfigurationHelper.GetDeviceState(Id);
			if (deviceState != null)
			{
				var states = new List<Assad.CPeventTypeState>();

				var mainState = new Assad.CPeventTypeState();
				mainState.state = "Состояние";
				mainState.value = deviceState.StateType.ToDescription();
				states.Add(mainState);

				foreach (var parameter in deviceState.Parameters)
				{
					if (parameter.Visible)
					{
						var parameterState = new Assad.CPeventTypeState();
						parameterState.state = parameter.Name;
						parameterState.value = parameter.Value;

						if ((string.IsNullOrEmpty(parameter.Value)) || (parameter.Value == "<NULL>"))
						{
							parameterState.value = " ";
						}
						states.Add(parameterState);
					}
				}
				eventType.state = states.ToArray();
			}
			NetManager.Send(eventType, null);
		}

		public override Assad.DeviceType QueryAbility()
		{
			var deviceAbility = new Assad.DeviceType();
			deviceAbility.deviceId = DeviceId;
			var abilityParameters = new List<Assad.DeviceTypeParam>();

			var deviceState = ConfigurationHelper.GetDeviceState(Id);
			if (deviceState != null)
			{
				foreach (var parameter in deviceState.Parameters)
				{
					if (!(string.IsNullOrEmpty(parameter.Value)) && (parameter.Value != "<NULL>"))
					{
						var abilityParameter = new Assad.DeviceTypeParam();
						abilityParameter.name = parameter.Caption;
						abilityParameter.value = parameter.Value;
						abilityParameters.Add(abilityParameter);
					}
				}

				foreach (var state in deviceState.States)
				{
					var stateParameter = new Assad.DeviceTypeParam();
					stateParameter.name = state.DriverState.Name;
					stateParameter.value = " ";
					abilityParameters.Add(stateParameter);
				}
			}

			deviceAbility.param = abilityParameters.ToArray();
			return deviceAbility;
		}

		void SetPath()
		{
			string currentPath = DriverId + ":" + Address;
			if (Parent != null)
			{
				Id = (Parent as AssadDevice).Id + @"/" + currentPath;
			}
			else
			{
				Id = currentPath;
			}
		}
	}
}