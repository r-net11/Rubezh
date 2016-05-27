using System;
using StrazhAPI.GK;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using Infrastructure.Client.Plans.Presenter;
using StrazhAPI.Plans.Elements;

namespace StrazhModule.Plans
{
	internal class PlanMonitor : StateMonitor
	{
		public PlanMonitor(Plan plan, Action callBack)
			: base(plan, callBack)
		{
			Plan.ElementSKDDevices.ForEach(item => Initialize(item));
			Plan.ElementRectangleSKDZones.ForEach(item => Initialize(item));
			Plan.ElementPolygonSKDZones.ForEach(item => Initialize(item));
			Plan.ElementDoors.ForEach(item => Initialize(item));
		}

		private void Initialize(ElementSKDDevice element)
		{
			var device = PlanPresenter.Cache.Get<SKDDevice>(element.DeviceUID);
			AddState(device);
		}
		private void Initialize(ElementDoor element)
		{
			var door = PlanPresenter.Cache.Get<SKDDoor>(element.DoorUID);
			AddState((IStateProvider)door);
		}
		private void Initialize(IElementZone element)
		{
			var zone = PlanPresenter.Cache.Get<SKDZone>(element.ZoneUID);
			AddState(zone);
		}
	}
}