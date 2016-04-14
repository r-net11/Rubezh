using AutomationModule.ViewModels;
using Infrastructure;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.SKD;
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
				{
					currentExplicitValue.ExplicitValue.Value = selectGKDeviceEventArg.Device;
					currentExplicitValue.Initialize();
				}
				return true;
			}

			if (objectType == ObjectType.Zone)
			{
				var selectGKZoneEventArg = new SelectGKZoneEventArg { Zone = currentExplicitValue.Zone };
				ServiceFactory.Events.GetEvent<SelectGKZoneEvent>().Publish(selectGKZoneEventArg);
				if (!selectGKZoneEventArg.Cancel)
				{
					currentExplicitValue.ExplicitValue.Value = selectGKZoneEventArg.Zone;
					currentExplicitValue.Initialize();
				}
				return true;
			}

			if (objectType == ObjectType.GuardZone)
			{
				var selectGKGuardZoneEventArg = new SelectGKGuardZoneEventArg { GuardZone = currentExplicitValue.GuardZone };
				ServiceFactory.Events.GetEvent<SelectGKGuardZoneEvent>().Publish(selectGKGuardZoneEventArg);
				if (!selectGKGuardZoneEventArg.Cancel)
				{
					currentExplicitValue.ExplicitValue.Value = selectGKGuardZoneEventArg.GuardZone;
					currentExplicitValue.Initialize();
				}
				return true;
			}

			if (objectType == ObjectType.GKDoor)
			{
				var selectGKDoorEventArg = new SelectGKDoorEventArg { Door = currentExplicitValue.GKDoor };
				ServiceFactory.Events.GetEvent<SelectGKDoorEvent>().Publish(selectGKDoorEventArg);
				if (!selectGKDoorEventArg.Cancel)
				{
					currentExplicitValue.ExplicitValue.Value = selectGKDoorEventArg.Door;
					currentExplicitValue.Initialize();
				}
				return true;
			}

			if (objectType == ObjectType.Direction)
			{
				var selectGKDirectionEventArg = new SelectGKDirectionEventArg { Direction = currentExplicitValue.Direction };
				ServiceFactory.Events.GetEvent<SelectGKDirectionEvent>().Publish(selectGKDirectionEventArg);
				if (!selectGKDirectionEventArg.Cancel)
				{
					currentExplicitValue.ExplicitValue.Value = selectGKDirectionEventArg.Direction;
					currentExplicitValue.Initialize();
				}
				return true;
			}

			if (objectType == ObjectType.PumpStation)
			{
				var selectGKPumpStationEventArg = new SelectGKPumpStationEventArg { PumpStation = currentExplicitValue.PumpStation };
				ServiceFactory.Events.GetEvent<SelectGKPumpStationEvent>().Publish(selectGKPumpStationEventArg);
				if (!selectGKPumpStationEventArg.Cancel)
				{
					currentExplicitValue.ExplicitValue.Value = selectGKPumpStationEventArg.PumpStation;
					currentExplicitValue.Initialize();
				}
				return true;
			}

			if (objectType == ObjectType.MPT)
			{
				var selectGKMPTEventArg = new SelectGKMPTEventArg { MPT = currentExplicitValue.MPT };
				ServiceFactory.Events.GetEvent<SelectGKMPTEvent>().Publish(selectGKMPTEventArg);
				if (!selectGKMPTEventArg.Cancel)
				{
					currentExplicitValue.ExplicitValue.Value = selectGKMPTEventArg.MPT == null;
					currentExplicitValue.Initialize();
				}
				return true;
			}

			if (objectType == ObjectType.VideoDevice)
			{
				var selectCameraEventArg = new SelectCameraEventArg { Camera = currentExplicitValue.Camera };
				ServiceFactory.Events.GetEvent<SelectCameraEvent>().Publish(selectCameraEventArg);
				if (!selectCameraEventArg.Cancel)
				{
					currentExplicitValue.ExplicitValue.Value = selectCameraEventArg.Camera;
					currentExplicitValue.Initialize();
				}
				return true;
			}

			if (objectType == ObjectType.Delay)
			{
				var selectGKDelayEventArg = new SelectGKDelayEventArg { Delay = currentExplicitValue.Delay };
				ServiceFactory.Events.GetEvent<SelectGKDelayEvent>().Publish(selectGKDelayEventArg);
				if (!selectGKDelayEventArg.Cancel)
				{
					currentExplicitValue.ExplicitValue.Value = selectGKDelayEventArg.Delay;
					currentExplicitValue.Initialize();
				}
				return true;
			}

			if (objectType == ObjectType.Organisation)
			{
				var selectGKOrganisationEventArg = new SelectOrganisationEventArg { Organisation = currentExplicitValue.Organisation };
				ServiceFactory.Events.GetEvent<SelectOrganisationEvent>().Publish(selectGKOrganisationEventArg);
				if (!selectGKOrganisationEventArg.Cancel)
				{
					currentExplicitValue.ExplicitValue.Value = selectGKOrganisationEventArg.Organisation;
					currentExplicitValue.Initialize();
				}
				return true;
			}
			return false;
		}

		public static bool SelectObjects(ObjectType objectType, ref List<ObjectReference> currentReferences)
		{
			if (objectType == ObjectType.Device)
			{
				var selectGKDevicesEventArg = new SelectGKDevicesEventArg { Devices = currentReferences == null ? null : currentReferences.Select(x => (GKDevice)ExplicitValue.GetObjectValue(x)).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKDevicesEvent>().Publish(selectGKDevicesEventArg);
				currentReferences = selectGKDevicesEventArg.Cancel ?
					null :
					selectGKDevicesEventArg.Devices.Select(x => new ObjectReference { UID = x.UID, ObjectType = objectType }).ToList();
				return true;
			}

			if (objectType == ObjectType.Zone)
			{
				var selectGKZonesEventArg = new SelectGKZonesEventArg { Zones = currentReferences == null ? null : currentReferences.Select(x => (GKZone)ExplicitValue.GetObjectValue(x)).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKZonesEvent>().Publish(selectGKZonesEventArg);
				currentReferences = selectGKZonesEventArg.Cancel ?
					null :
					selectGKZonesEventArg.Zones.Select(x => new ObjectReference { UID = x.UID, ObjectType = objectType }).ToList();
				return true;
			}

			if (objectType == ObjectType.GuardZone)
			{
				var selectGKGuardZonesEventArg = new SelectGKGuardZonesEventArg { GuardZones = currentReferences == null ? null : currentReferences.Select(x => (GKGuardZone)ExplicitValue.GetObjectValue(x)).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKGuardZonesEvent>().Publish(selectGKGuardZonesEventArg);
				currentReferences = selectGKGuardZonesEventArg.Cancel ?
					null :
					selectGKGuardZonesEventArg.GuardZones.Select(x => new ObjectReference { UID = x.UID, ObjectType = objectType }).ToList();
				return true;
			}

			if (objectType == ObjectType.GKDoor)
			{
				var selectGKDoorsEventArg = new SelectGKDoorsEventArg { Doors = currentReferences == null ? null : currentReferences.Select(x => (GKDoor)ExplicitValue.GetObjectValue(x)).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKDoorsEvent>().Publish(selectGKDoorsEventArg);
				currentReferences = selectGKDoorsEventArg.Cancel ?
					null :
					selectGKDoorsEventArg.Doors.Select(x => new ObjectReference { UID = x.UID, ObjectType = objectType }).ToList();
				return true;
			}

			if (objectType == ObjectType.Direction)
			{
				var selectGKDirectionsEventArg = new SelectGKDirectionsEventArg { Directions = currentReferences == null ? null : currentReferences.Select(x => (GKDirection)ExplicitValue.GetObjectValue(x)).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKDirectionsEvent>().Publish(selectGKDirectionsEventArg);
				currentReferences = selectGKDirectionsEventArg.Cancel ?
					null :
					selectGKDirectionsEventArg.Directions.Select(x => new ObjectReference { UID = x.UID, ObjectType = objectType }).ToList();
				return true;
			}

			if (objectType == ObjectType.PumpStation)
			{
				var selectGKPumpStationsEventArg = new SelectGKPumpStationsEventArg { PumpStations = currentReferences == null ? null : currentReferences.Select(x => (GKPumpStation)ExplicitValue.GetObjectValue(x)).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKPumpStationsEvent>().Publish(selectGKPumpStationsEventArg);
				currentReferences = selectGKPumpStationsEventArg.Cancel ?
					null :
					selectGKPumpStationsEventArg.PumpStations.Select(x => new ObjectReference { UID = x.UID, ObjectType = objectType }).ToList();
				return true;
			}

			if (objectType == ObjectType.MPT)
			{
				var selectGKMPTsEventArg = new SelectGKMPTsEventArg { MPTs = currentReferences == null ? null : currentReferences.Select(x => (GKMPT)ExplicitValue.GetObjectValue(x)).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKMPTsEvent>().Publish(selectGKMPTsEventArg);
				currentReferences = selectGKMPTsEventArg.Cancel ?
					null :
					selectGKMPTsEventArg.MPTs.Select(x => new ObjectReference { UID = x.UID, ObjectType = objectType }).ToList();
				return true;
			}

			if (objectType == ObjectType.VideoDevice)
			{
				var selectCamerasEventArg = new SelectCamerasEventArg { Cameras = currentReferences == null ? null : currentReferences.Select(x => (Camera)ExplicitValue.GetObjectValue(x)).ToList() };
				ServiceFactory.Events.GetEvent<SelectCamerasEvent>().Publish(selectCamerasEventArg);
				currentReferences = selectCamerasEventArg.Cancel ?
					null :
					selectCamerasEventArg.Cameras.Select(x => new ObjectReference { UID = x.UID, ObjectType = objectType }).ToList();
				return true;
			}

			if (objectType == ObjectType.Delay)
			{
				var selectGKDelaysEventArg = new SelectGKDelaysEventArg { Delays = currentReferences == null ? null : currentReferences.Select(x => (GKDelay)ExplicitValue.GetObjectValue(x)).ToList() };
				ServiceFactory.Events.GetEvent<SelectGKDelaysEvent>().Publish(selectGKDelaysEventArg);
				currentReferences = selectGKDelaysEventArg.Cancel ?
					null :
					selectGKDelaysEventArg.Delays.Select(x => new ObjectReference { UID = x.UID, ObjectType = objectType }).ToList();
				return true;
			}

			if (objectType == ObjectType.Organisation)
			{
				var selectOrganisationsEventArg = new SelectOrganisationsEventArg { Organisations = currentReferences == null ? null : currentReferences.Select(x => (Organisation)ExplicitValue.GetObjectValue(x)).ToList() };
				ServiceFactory.Events.GetEvent<SelectOrganisationsEvent>().Publish(selectOrganisationsEventArg);
				currentReferences = selectOrganisationsEventArg.Cancel ?
					null :
					selectOrganisationsEventArg.Organisations.Select(x => new ObjectReference { UID = x.UID, ObjectType = objectType }).ToList();
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