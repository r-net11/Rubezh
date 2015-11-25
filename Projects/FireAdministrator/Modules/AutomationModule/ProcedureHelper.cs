using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.Automation;
using Infrastructure.Common.Windows;
using AutomationModule.ViewModels;
using System.Collections.ObjectModel;
using Infrustructure.Plans.Elements;
using RubezhAPI.Models;

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
			allElements.AddRange(plan.ElementPolygonGKZones);
			allElements.AddRange(plan.ElementPolygonGKGuardZones);
			allElements.AddRange(plan.ElementPolygonGKSKDZones);
			allElements.AddRange(plan.ElementPolygonGKDirections);
			allElements.AddRange(plan.ElementPolygonGKMPTs);
			allElements.AddRange(plan.ElementPolygonGKDelays);
			allElements.AddRange(plan.ElementSubPlans);
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
				var deviceSelectionViewModel = new DeviceSelectionViewModel(currentExplicitValue.Device);
				if (DialogService.ShowModalWindow(deviceSelectionViewModel))
				{
					currentExplicitValue.UidValue = deviceSelectionViewModel.SelectedDevice != null ? deviceSelectionViewModel.SelectedDevice.Device.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.Zone)
			{
				var zoneSelectionViewModel = new ZoneSelectionViewModel(currentExplicitValue.Zone);
				if (DialogService.ShowModalWindow(zoneSelectionViewModel))
				{
					currentExplicitValue.UidValue = zoneSelectionViewModel.SelectedZone != null ? zoneSelectionViewModel.SelectedZone.Zone.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.GuardZone)
			{
				var guardZoneSelectionViewModel = new GuardZoneSelectionViewModel(currentExplicitValue.GuardZone);
				if (DialogService.ShowModalWindow(guardZoneSelectionViewModel))
				{
					currentExplicitValue.UidValue = guardZoneSelectionViewModel.SelectedZone != null ? guardZoneSelectionViewModel.SelectedZone.GuardZone.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.GKDoor)
			{
				var doorSelectionViewModel = new GKDoorSelectionViewModel(currentExplicitValue.GKDoor);
				if (DialogService.ShowModalWindow(doorSelectionViewModel))
				{
					currentExplicitValue.UidValue = doorSelectionViewModel.SelectedDoor != null ? doorSelectionViewModel.SelectedDoor.GKDoor.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.Direction)
			{
				var directionSelectionViewModel = new DirectionSelectionViewModel(currentExplicitValue.Direction);
				if (DialogService.ShowModalWindow(directionSelectionViewModel))
				{
					currentExplicitValue.UidValue = directionSelectionViewModel.SelectedDirection != null ? directionSelectionViewModel.SelectedDirection.Direction.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.PumpStation)
			{
				var pumpStationSelectionViewModel = new PumpStationSelectionViewModel(currentExplicitValue.PumpStation);
				if (DialogService.ShowModalWindow(pumpStationSelectionViewModel))
				{
					currentExplicitValue.UidValue = pumpStationSelectionViewModel.SelectedPumpStation != null ? pumpStationSelectionViewModel.SelectedPumpStation.PumpStation.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.MPT)
			{
				var mptSelectionViewModel = new MPTSelectionViewModel(currentExplicitValue.MPT);
				if (DialogService.ShowModalWindow(mptSelectionViewModel))
				{
					currentExplicitValue.UidValue = mptSelectionViewModel.SelectedMPT != null ? mptSelectionViewModel.SelectedMPT.MPT.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.VideoDevice)
			{
				var cameraSelectionViewModel = new CameraSelectionViewModel(currentExplicitValue.Camera);
				if (DialogService.ShowModalWindow(cameraSelectionViewModel))
				{
					currentExplicitValue.UidValue = cameraSelectionViewModel.SelectedCamera != null ? cameraSelectionViewModel.SelectedCamera.Camera.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.Delay)
			{
				var delaySelectionViewModel = new DelaySelectionViewModel(currentExplicitValue.Delay);
				if (DialogService.ShowModalWindow(delaySelectionViewModel))
				{
					currentExplicitValue.UidValue = delaySelectionViewModel.SelectedDelay != null ? delaySelectionViewModel.SelectedDelay.Delay.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.Organisation)
			{
				var organisationSelectionViewModel = new OrganisationSelectionViewModel(currentExplicitValue.Organisation);
				if (DialogService.ShowModalWindow(organisationSelectionViewModel))
				{
					currentExplicitValue.UidValue = organisationSelectionViewModel.SelectedOrganisation != null ? organisationSelectionViewModel.SelectedOrganisation.Organisation.UID : Guid.Empty;
					return true;
				}
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