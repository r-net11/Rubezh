using Infrastructure.Automation;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ElementViewModel : BaseViewModel
	{
		public string Name { get; private set; }
		ElementBase ElementBase { get; set; }
		public Type ElementType { get; private set; }

		public ElementViewModel(ElementBase elementBase)
		{
			ElementBase = elementBase;
			ElementType = ElementBase.GetType();
		}

		public string PresentationName
		{
			get
			{
				if (ElementType == typeof(ElementGKDevice))
				{
					var item = GKManager.Devices.FirstOrDefault(x => x.UID == ((ElementGKDevice)ElementBase).DeviceUID);
					if (item != null)
						return "Устройство: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementRectangleGKZone))
				{
					var item = GKManager.Zones.FirstOrDefault(x => x.UID == ((ElementRectangleGKZone)ElementBase).ZoneUID);
					if (item != null)
						return "Пожарная зона: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementRectangleGKGuardZone))
				{
					var item = GKManager.GuardZones.FirstOrDefault(x => x.UID == ((ElementRectangleGKGuardZone)ElementBase).ZoneUID);
					if (item != null)
						return "Охранная зона: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementRectangleGKSKDZone))
				{
					var item = GKManager.SKDZones.FirstOrDefault(x => x.UID == ((ElementRectangleGKSKDZone)ElementBase).ZoneUID);
					if (item != null)
						return "Зона СКД: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementRectangleGKDirection))
				{
					var item = GKManager.Directions.FirstOrDefault(x => x.UID == ((ElementRectangleGKDirection)ElementBase).DirectionUID);
					if (item != null)
						return "Направление: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementRectangleGKMPT))
				{
					var item = GKManager.MPTs.FirstOrDefault(x => x.UID == ((ElementRectangleGKMPT)ElementBase).MPTUID);
					if (item != null)
						return "МПТ: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementRectangleGKDelay))
				{
					var item = GKManager.Delays.FirstOrDefault(x => x.UID == ((ElementRectangleGKDelay)ElementBase).DelayUID);
					if (item != null)
						return "Задержка: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementPolygonGKZone))
				{
					var item = GKManager.Zones.FirstOrDefault(x => x.UID == ((ElementPolygonGKZone)ElementBase).ZoneUID);
					if (item != null)
						return "Пожарная зона: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementRectangleGKPumpStation))
				{
					var item = GKManager.PumpStations.FirstOrDefault(x => x.UID == ((ElementRectangleGKPumpStation)ElementBase).PumpStationUID);
					if (item != null)
						return "Насосная станция: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementPolygonGKPumpStation))
				{
					var item = GKManager.PumpStations.FirstOrDefault(x => x.UID == ((ElementPolygonGKPumpStation)ElementBase).PumpStationUID);
					if (item != null)
						return "Насосная станция: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementPolygonGKGuardZone))
				{
					var item = GKManager.GuardZones.FirstOrDefault(x => x.UID == ((ElementPolygonGKGuardZone)ElementBase).ZoneUID);
					if (item != null)
						return "Охранная зона: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementPolygonGKSKDZone))
				{
					var item = GKManager.SKDZones.FirstOrDefault(x => x.UID == ((ElementPolygonGKSKDZone)ElementBase).ZoneUID);
					if (item != null)
						return "Зона СКД: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementPolygonGKDirection))
				{
					var item = GKManager.Directions.FirstOrDefault(x => x.UID == ((ElementPolygonGKDirection)ElementBase).DirectionUID);
					if (item != null)
						return "Направление: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementPolygonGKMPT))
				{
					var item = GKManager.MPTs.FirstOrDefault(x => x.UID == ((ElementPolygonGKMPT)ElementBase).MPTUID);
					if (item != null)
						return "МПТ: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementPolygonGKDelay))
				{
					var item = GKManager.Delays.FirstOrDefault(x => x.UID == ((ElementPolygonGKDelay)ElementBase).DelayUID);
					if (item != null)
						return "Задержка: " + item.PresentationName;
				}
				if (ElementType == typeof(ElementRectangleSubPlan))
				{
					var item = GetPlan(ClientManager.PlansConfiguration.Plans, ((ElementRectangleSubPlan)ElementBase).PlanUID);
					if (item != null)
						return "План: " + item.Caption;
				}
				if (ElementType == typeof(ElementPolygonSubPlan))
				{
					var item = GetPlan(ClientManager.PlansConfiguration.Plans, ((ElementPolygonSubPlan)ElementBase).PlanUID);
					if (item != null)
						return "План: " + item.Caption;
				}
				if (ElementType == typeof(ElementProcedure))
					return "Процедура: " + AutomationHelper.GetProcedureName(((ElementProcedure)ElementBase).ProcedureUID);

				return ElementBase.PresentationName;
			}
		}

		Plan GetPlan(List<Plan> list, Guid planUid)
		{
			if (list == null)
				return null;
			var plan = list.FirstOrDefault(x => x.UID == planUid);
			if (plan != null)
				return plan;
			foreach (var item in list)
			{
				plan = GetPlan(item.Children, planUid);
				if (plan != null)
					return plan;
			}
			return null;
		}

		public Guid Uid
		{
			get { return ElementBase.UID; }
		}
	}
}
