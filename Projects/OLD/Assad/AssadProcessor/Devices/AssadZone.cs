﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI;
using FiresecClient;

namespace AssadProcessor.Devices
{
	public class AssadZone : AssadBase
	{
		public string ZoneNo { get; set; }

		public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
		{
			if (innerDevice.param != null)
			{
				var zoneParameter = innerDevice.param.FirstOrDefault(x => x.param == "Номер зоны");
				if (zoneParameter != null)
					ZoneNo = zoneParameter.value;
			}
		}

		public override Assad.DeviceType GetStates()
		{
			var deviceType = new Assad.DeviceType();
			deviceType.deviceId = DeviceId;
			var states = new List<Assad.DeviceTypeState>();

			if (FiresecManager.Zones.Any(x => x.No.ToString() == ZoneNo))
			{
				var zone = FiresecManager.Zones.FirstOrDefault(x => x.No.ToString() == ZoneNo);

				var mainState = new Assad.DeviceTypeState()
				{
					state = "Состояние",
					value = zone.ZoneState.StateType.ToDescription()
				};
				states.Add(mainState);
				var state1 = new Assad.DeviceTypeState()
				{
					state = "Наименование",
					value = zone.Name
				};
				states.Add(state1);
				var state2 = new Assad.DeviceTypeState()
				{
					state = "Число датчиков для формирования сигнала Пожар",
					value = zone.DetectorCount.ToString()
				};
				states.Add(state2);
				var state3 = new Assad.DeviceTypeState()
				{
					state = "Примечание",
					value = zone.Description
				};
				states.Add(state3);
				var state4 = new Assad.DeviceTypeState()
				{
					state = "Назначение зоны",
					value = "Пожарная"
				};
				states.Add(state4);
			}
			else
			{
				var mainState = new Assad.DeviceTypeState()
				{
					state = "Состояние",
					value = "Отсутствует в конфигурации сервера оборудования"
				};
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

			if (FiresecManager.Zones.Any(x => x.No.ToString() == ZoneNo))
			{
				var zone = FiresecManager.Zones.FirstOrDefault(x => x.No.ToString() == ZoneNo);

				eventType.state = new Assad.CPeventTypeState[1];
				eventType.state[0] = new Assad.CPeventTypeState()
				{
					state = "Состояние",
					value = zone.ZoneState.StateType.ToDescription()
				};
			}

			NetManager.Send(eventType, null);
		}

		public override Assad.DeviceType QueryAbility()
		{
			var deviceAbility = new Assad.DeviceType();
			deviceAbility.deviceId = DeviceId;
			var abilityParameters = new List<Assad.DeviceTypeParam>();

			deviceAbility.param = abilityParameters.ToArray();
			return deviceAbility;
		}
	}
}