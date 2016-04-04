using AutomationModule.ViewModels;
using Infrastructure;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using RubezhAPI.Automation;
using RubezhAPI.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule
{
	public static class ProcedureHelper
	{
		public static ObservableCollection<ElementViewModel> GetAllElements(Plan plan)
		{
			var elements = new ObservableCollection<ElementViewModel>();
			var allElements = new List<ElementBase>(plan.ElementRectangles);
			allElements.AddRange(plan.ElementEllipses);
			allElements.AddRange(plan.ElementPolylines);
			allElements.AddRange(plan.ElementTextBlocks);
			allElements.AddRange(plan.ElementPolygons);
			allElements.AddRange(plan.ElementExtensions);
			allElements.AddRange(plan.ElementGKDevices);
			allElements.AddRange(plan.ElementRectangleGKZones);
			allElements.AddRange(plan.ElementRectangleGKGuardZones);
			allElements.AddRange(plan.ElementRectangleGKSKDZones);
			allElements.AddRange(plan.ElementRectangleGKDirections);
			allElements.AddRange(plan.ElementRectangleGKMPTs);
			allElements.AddRange(plan.ElementRectangleGKDelays);
			allElements.AddRange(plan.ElementRectangleGKPumpStations);
			allElements.AddRange(plan.ElementPolygonGKZones);
			allElements.AddRange(plan.ElementPolygonGKGuardZones);
			allElements.AddRange(plan.ElementPolygonGKSKDZones);
			allElements.AddRange(plan.ElementPolygonGKDirections);
			allElements.AddRange(plan.ElementPolygonGKMPTs);
			allElements.AddRange(plan.ElementPolygonGKDelays);
			allElements.AddRange(plan.ElementPolygonGKPumpStations);
			allElements.AddRange(plan.ElementSubPlans);
			allElements.AddRange(plan.ElementPolygonSubPlans);
			foreach (var elementRectangle in allElements)
			{
				elements.Add(new ElementViewModel(elementRectangle));
			}
			return elements;
		}

		public static bool SelectObject(ObjectType objectType, ExplicitValueViewModel currentExplicitValue)
		{
			if (objectType == ObjectType.Device)
			{
				var selectGKDeviceEventArg = new SelectGKDeviceEventArg { Device = currentExplicitValue.Device };
				ServiceFactory.Events.GetEvent<SelectGKDeviceEvent>().Publish(selectGKDeviceEventArg);
				if (!selectGKDeviceEventArg.Cancel)
					currentExplicitValue.ObjectReferenceValue = selectGKDeviceEventArg.Device == null ?
						new ObjectReference() :
						new ObjectReference { UID = selectGKDeviceEventArg.Device.UID, ObjectType = objectType };
				return true;
			}

			if (objectType == ObjectType.Zone)
			{
				var selectGKZoneEventArg = new SelectGKZoneEventArg { Zone = currentExplicitValue.Zone };
				ServiceFactory.Events.GetEvent<SelectGKZoneEvent>().Publish(selectGKZoneEventArg);
				if (!selectGKZoneEventArg.Cancel)
					currentExplicitValue.ObjectReferenceValue = selectGKZoneEventArg.Zone == null ?
						new ObjectReference() :
						new ObjectReference { UID = selectGKZoneEventArg.Zone.UID, ObjectType = objectType };
				return true;
			}

			if (objectType == ObjectType.GuardZone)
			{
				var selectGKGuardZoneEventArg = new SelectGKGuardZoneEventArg { GuardZone = currentExplicitValue.GuardZone };
				ServiceFactory.Events.GetEvent<SelectGKGuardZoneEvent>().Publish(selectGKGuardZoneEventArg);
				if (!selectGKGuardZoneEventArg.Cancel)
					currentExplicitValue.ObjectReferenceValue = selectGKGuardZoneEventArg.GuardZone == null ?
						new ObjectReference() :
						new ObjectReference { UID = selectGKGuardZoneEventArg.GuardZone.UID, ObjectType = objectType };
				return true;
			}

			if (objectType == ObjectType.GKDoor)
			{
				var selectGKDoorEventArg = new SelectGKDoorEventArg { Door = currentExplicitValue.GKDoor };
				ServiceFactory.Events.GetEvent<SelectGKDoorEvent>().Publish(selectGKDoorEventArg);
				if (!selectGKDoorEventArg.Cancel)
					currentExplicitValue.ObjectReferenceValue = selectGKDoorEventArg.Door == null ?
						new ObjectReference() :
						new ObjectReference { UID = selectGKDoorEventArg.Door.UID, ObjectType = objectType };
				return true;
			}

			if (objectType == ObjectType.Direction)
			{
				var selectGKDirectionEventArg = new SelectGKDirectionEventArg { Direction = currentExplicitValue.Direction };
				ServiceFactory.Events.GetEvent<SelectGKDirectionEvent>().Publish(selectGKDirectionEventArg);
				if (!selectGKDirectionEventArg.Cancel)
					currentExplicitValue.ObjectReferenceValue = selectGKDirectionEventArg.Direction == null ?
						new ObjectReference() :
						new ObjectReference { UID = selectGKDirectionEventArg.Direction.UID, ObjectType = objectType };
				return true;
			}

			if (objectType == ObjectType.PumpStation)
			{
				var selectGKPumpStationEventArg = new SelectGKPumpStationEventArg { PumpStation = currentExplicitValue.PumpStation };
				ServiceFactory.Events.GetEvent<SelectGKPumpStationEvent>().Publish(selectGKPumpStationEventArg);
				if (!selectGKPumpStationEventArg.Cancel)
					currentExplicitValue.ObjectReferenceValue = selectGKPumpStationEventArg.PumpStation == null ?
						new ObjectReference() :
						new ObjectReference { UID = selectGKPumpStationEventArg.PumpStation.UID, ObjectType = objectType };
				return true;
			}

			if (objectType == ObjectType.MPT)
			{
				var selectGKMPTEventArg = new SelectGKMPTEventArg { MPT = currentExplicitValue.MPT };
				ServiceFactory.Events.GetEvent<SelectGKMPTEvent>().Publish(selectGKMPTEventArg);
				if (!selectGKMPTEventArg.Cancel)
					currentExplicitValue.ObjectReferenceValue = selectGKMPTEventArg.MPT == null ?
						new ObjectReference() :
						new ObjectReference { UID = selectGKMPTEventArg.MPT.UID, ObjectType = objectType };
				return true;
			}

			if (objectType == ObjectType.VideoDevice)
			{
				var selectCameraEventArg = new SelectCameraEventArg { Camera = currentExplicitValue.Camera };
				ServiceFactory.Events.GetEvent<SelectCameraEvent>().Publish(selectCameraEventArg);
				if (!selectCameraEventArg.Cancel)
					currentExplicitValue.ObjectReferenceValue = selectCameraEventArg.Camera == null ?
						new ObjectReference() :
						new ObjectReference { UID = selectCameraEventArg.Camera.UID, ObjectType = objectType };
				return true;
			}

			if (objectType == ObjectType.Delay)
			{
				var selectGKDelayEventArg = new SelectGKDelayEventArg { Delay = currentExplicitValue.Delay };
				ServiceFactory.Events.GetEvent<SelectGKDelayEvent>().Publish(selectGKDelayEventArg);
				if (!selectGKDelayEventArg.Cancel)
					currentExplicitValue.ObjectReferenceValue = selectGKDelayEventArg.Delay == null ?
						new ObjectReference() :
						new ObjectReference { UID = selectGKDelayEventArg.Delay.UID, ObjectType = objectType };
				return true;
			}

			if (objectType == ObjectType.Organisation)
			{
				var selectGKOrganisationEventArg = new SelectOrganisationEventArg { Organisation = currentExplicitValue.Organisation };
				ServiceFactory.Events.GetEvent<SelectOrganisationEvent>().Publish(selectGKOrganisationEventArg);
				if (!selectGKOrganisationEventArg.Cancel)
					currentExplicitValue.ObjectReferenceValue = selectGKOrganisationEventArg.Organisation == null ?
						new ObjectReference() :
						new ObjectReference { UID = selectGKOrganisationEventArg.Organisation.UID, ObjectType = objectType };
				return true;
			}
			return false;
		}

		public static bool SelectObjects(ObjectType objectType, ref List<ExplicitValueViewModel> currentExplicitValues)
		{
			if (objectType == ObjectType.Device)
			{
				var selectGKDevicesEventArg = new SelectGKDevicesEventArg { Devices = currentExplicitValues == null ? null : currentExplicitValues.Select(x => x.Device).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKDevicesEvent>().Publish(selectGKDevicesEventArg);
				currentExplicitValues = selectGKDevicesEventArg.Cancel ?
					null :
					selectGKDevicesEventArg.Devices.Select(x =>
						new ExplicitValueViewModel(new ExplicitValue
						{
							ExplicitType = ExplicitType.Object,
							ObjectReferenceValue = new ObjectReference { UID = x.UID, ObjectType = objectType }
						})).ToList();
				return true;
			}

			if (objectType == ObjectType.Zone)
			{
				var selectGKZonesEventArg = new SelectGKZonesEventArg { Zones = currentExplicitValues == null ? null : currentExplicitValues.Select(x => x.Zone).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKZonesEvent>().Publish(selectGKZonesEventArg);
				currentExplicitValues = selectGKZonesEventArg.Cancel ?
					null :
					selectGKZonesEventArg.Zones.Select(x => new ExplicitValueViewModel(new ExplicitValue
					{
						ExplicitType = ExplicitType.Object,
						ObjectReferenceValue = new ObjectReference { UID = x.UID, ObjectType = objectType }
					})).ToList();
				return true;
			}

			if (objectType == ObjectType.GuardZone)
			{
				var selectGKGuardZonesEventArg = new SelectGKGuardZonesEventArg { GuardZones = currentExplicitValues == null ? null : currentExplicitValues.Select(x => x.GuardZone).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKGuardZonesEvent>().Publish(selectGKGuardZonesEventArg);
				currentExplicitValues = selectGKGuardZonesEventArg.Cancel ?
					null :
					selectGKGuardZonesEventArg.GuardZones.Select(x => new ExplicitValueViewModel(new ExplicitValue
					{
						ExplicitType = ExplicitType.Object,
						ObjectReferenceValue = new ObjectReference { UID = x.UID, ObjectType = objectType }
					})).ToList();
				return true;
			}

			if (objectType == ObjectType.GKDoor)
			{
				var selectGKDoorsEventArg = new SelectGKDoorsEventArg { Doors = currentExplicitValues == null ? null : currentExplicitValues.Select(x => x.GKDoor).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKDoorsEvent>().Publish(selectGKDoorsEventArg);
				currentExplicitValues = selectGKDoorsEventArg.Cancel ?
					null :
					selectGKDoorsEventArg.Doors.Select(x => new ExplicitValueViewModel(new ExplicitValue
					{
						ExplicitType = ExplicitType.Object,
						ObjectReferenceValue = new ObjectReference { UID = x.UID, ObjectType = objectType }
					})).ToList();
				return true;
			}

			if (objectType == ObjectType.Direction)
			{
				var selectGKDirectionsEventArg = new SelectGKDirectionsEventArg { Directions = currentExplicitValues == null ? null : currentExplicitValues.Select(x => x.Direction).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKDirectionsEvent>().Publish(selectGKDirectionsEventArg);
				currentExplicitValues = selectGKDirectionsEventArg.Cancel ?
					null :
					selectGKDirectionsEventArg.Directions.Select(x => new ExplicitValueViewModel(new ExplicitValue
					{
						ExplicitType = ExplicitType.Object,
						ObjectReferenceValue = new ObjectReference { UID = x.UID, ObjectType = objectType }
					})).ToList();
				return true;
			}

			if (objectType == ObjectType.PumpStation)
			{
				var selectGKPumpStationsEventArg = new SelectGKPumpStationsEventArg { PumpStations = currentExplicitValues == null ? null : currentExplicitValues.Select(x => x.PumpStation).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKPumpStationsEvent>().Publish(selectGKPumpStationsEventArg);
				currentExplicitValues = selectGKPumpStationsEventArg.Cancel ?
					null :
					selectGKPumpStationsEventArg.PumpStations.Select(x => new ExplicitValueViewModel(new ExplicitValue
					{
						ExplicitType = ExplicitType.Object,
						ObjectReferenceValue = new ObjectReference { UID = x.UID, ObjectType = objectType }
					})).ToList();
				return true;
			}

			if (objectType == ObjectType.MPT)
			{
				var selectGKMPTsEventArg = new SelectGKMPTsEventArg { MPTs = currentExplicitValues == null ? null : currentExplicitValues.Select(x => x.MPT).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKMPTsEvent>().Publish(selectGKMPTsEventArg);
				currentExplicitValues = selectGKMPTsEventArg.Cancel ?
					null :
					selectGKMPTsEventArg.MPTs.Select(x => new ExplicitValueViewModel(new ExplicitValue
					{
						ExplicitType = ExplicitType.Object,
						ObjectReferenceValue = new ObjectReference { UID = x.UID, ObjectType = objectType }
					})).ToList();
				return true;
			}

			if (objectType == ObjectType.VideoDevice)
			{
				var selectCamerasEventArg = new SelectCamerasEventArg { Cameras = currentExplicitValues == null ? null : currentExplicitValues.Select(x => x.Camera).ToList() };
				ServiceFactory.Events.GetEvent<SelectCamerasEvent>().Publish(selectCamerasEventArg);
				currentExplicitValues = selectCamerasEventArg.Cancel ?
					null :
					selectCamerasEventArg.Cameras.Select(x => new ExplicitValueViewModel(new ExplicitValue
					{
						ExplicitType = ExplicitType.Object,
						ObjectReferenceValue = new ObjectReference { UID = x.UID, ObjectType = objectType }
					})).ToList();
				return true;
			}

			if (objectType == ObjectType.Delay)
			{
				var selectGKDelaysEventArg = new SelectGKDelaysEventArg { Delays = currentExplicitValues == null ? null : currentExplicitValues.Select(x => x.Delay).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKDelaysEvent>().Publish(selectGKDelaysEventArg);
				currentExplicitValues = selectGKDelaysEventArg.Cancel ?
					null :
					selectGKDelaysEventArg.Delays.Select(x => new ExplicitValueViewModel(new ExplicitValue
					{
						ExplicitType = ExplicitType.Object,
						ObjectReferenceValue = new ObjectReference { UID = x.UID, ObjectType = objectType }
					})).ToList();
				return true;
			}

			if (objectType == ObjectType.Organisation)
			{
				var selectOrganisationsEventArg = new SelectOrganisationsEventArg { Organisations = currentExplicitValues == null ? null : currentExplicitValues.Select(x => x.Organisation).ToList() };
				ServiceFactory.Events.GetEvent<SelectOrganisationsEvent>().Publish(selectOrganisationsEventArg);
				currentExplicitValues = selectOrganisationsEventArg.Cancel ?
					null :
					selectOrganisationsEventArg.Organisations.Select(x => new ExplicitValueViewModel(new ExplicitValue
					{
						ExplicitType = ExplicitType.Object,
						ObjectReferenceValue = new ObjectReference { UID = x.UID, ObjectType = objectType }
					})).ToList();
				return true;
			}
			return false;
		}

		public static List<ExplicitTypeViewModel> BuildExplicitTypes(List<ExplicitType> explicitTypes, List<EnumType> enumTypes, List<ObjectType> objectTypes)
		{
			var ExplicitTypes = new List<ExplicitTypeViewModel>();
			if (explicitTypes != null)
				ExplicitTypes.AddRange(explicitTypes.Select(explicitType => new ExplicitTypeViewModel(explicitType)));
			foreach (var enumType in enumTypes)
			{
				var explicitTypeViewModel = new ExplicitTypeViewModel(enumType);
				var parent = ExplicitTypes.FirstOrDefault(x => x.ExplicitType == ExplicitType.Enum);
				if (parent != null)
				{
					parent.AddChild(explicitTypeViewModel);
				}
			}
			foreach (var objectType in objectTypes)
			{
				var explicitTypeViewModel = new ExplicitTypeViewModel(objectType);
				var parent = ExplicitTypes.FirstOrDefault(x => x.ExplicitType == ExplicitType.Object);
				if (parent != null)
				{
					parent.AddChild(explicitTypeViewModel);
				}
			}
			return ExplicitTypes;
		}
	}
}