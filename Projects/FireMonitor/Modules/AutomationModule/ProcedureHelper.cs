using AutomationModule.ViewModels;
using Infrastructure.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using RubezhAPI.Automation;
using RubezhAPI.Models;
using RubezhClient;
using System.Collections.Generic;
using System.Threading;

namespace AutomationModule
{
	public static class ProcedureHelper
	{
		public static bool SelectObject(ObjectType objectType, ExplicitValueViewModel currentExplicitValue)
		{
			if (objectType == ObjectType.Device)
			{
				var deviceSelectationViewModel = new DeviceSelectionViewModel(currentExplicitValue.Device != null ? currentExplicitValue.Device : null);
				if (DialogService.ShowModalWindow(deviceSelectationViewModel))
				{
					currentExplicitValue.ObjectReferenceValue = deviceSelectationViewModel.SelectedDevice == null ?
						new ObjectReference() :
						new ObjectReference { UID = deviceSelectationViewModel.SelectedDevice.Device.UID, ObjectType = objectType };
					return true;
				}
			}

			if (objectType == ObjectType.Zone)
			{
				var zoneSelectationViewModel = new ZoneSelectionViewModel(currentExplicitValue.Zone != null ? currentExplicitValue.Zone : null);
				if (DialogService.ShowModalWindow(zoneSelectationViewModel))
				{
					currentExplicitValue.ObjectReferenceValue = zoneSelectationViewModel.SelectedZone.Zone == null ?
						new ObjectReference() :
						new ObjectReference { UID = zoneSelectationViewModel.SelectedZone.Zone.UID, ObjectType = objectType };
					return true;
				}
			}

			if (objectType == ObjectType.GuardZone)
			{
				var guardZoneSelectationViewModel = new GuardZoneSelectionViewModel(currentExplicitValue.GuardZone != null ? currentExplicitValue.GuardZone : null);
				if (DialogService.ShowModalWindow(guardZoneSelectationViewModel))
				{
					currentExplicitValue.ObjectReferenceValue = guardZoneSelectationViewModel.SelectedZone.GuardZone == null ?
						new ObjectReference() :
						new ObjectReference { UID = guardZoneSelectationViewModel.SelectedZone.GuardZone.UID, ObjectType = objectType };
					return true;
				}
			}

			if (objectType == ObjectType.Direction)
			{
				var directionSelectationViewModel = new DirectionSelectionViewModel(currentExplicitValue.Direction != null ? currentExplicitValue.Direction : null);
				if (DialogService.ShowModalWindow(directionSelectationViewModel))
				{
					currentExplicitValue.ObjectReferenceValue = directionSelectationViewModel.SelectedDirection.Direction == null ?
						new ObjectReference() :
						new ObjectReference { UID = directionSelectationViewModel.SelectedDirection.Direction.UID, ObjectType = objectType };
					return true;
				}
			}

			if (objectType == ObjectType.VideoDevice)
			{
				var cameraSelectionViewModel = new CameraSelectionViewModel(currentExplicitValue.Camera != null ? currentExplicitValue.Camera : null);
				if (DialogService.ShowModalWindow(cameraSelectionViewModel))
				{
					currentExplicitValue.ObjectReferenceValue = cameraSelectionViewModel.SelectedCamera == null ?
						new ObjectReference() :
						new ObjectReference { UID = cameraSelectionViewModel.SelectedCamera.Camera.UID, ObjectType = objectType };
					return true;
				}
			}

			if (objectType == ObjectType.Delay)
			{
				var delaySelectionViewModel = new DelaySelectionViewModel(currentExplicitValue.Delay);
				if (DialogService.ShowModalWindow(delaySelectionViewModel))
				{
					currentExplicitValue.ObjectReferenceValue = delaySelectionViewModel.SelectedDelay == null ?
						new ObjectReference() :
						new ObjectReference { UID = delaySelectionViewModel.SelectedDelay.Delay.UID, ObjectType = objectType };
					return true;
				}
			}
			return false;
		}

		public static void Run(Procedure procedure, List<Argument> args = null, User user = null)
		{
			if (args == null)
				args = new List<Argument>();
			using (new WaitWrapper())
			{
				var thread = new Thread(() =>
					{
						if (procedure.ContextType == ContextType.Client)
							AutomationProcessor.RunProcedure(procedure, args, null, user, null, FiresecServiceFactory.UID);
						else
							ClientManager.FiresecService.RunProcedure(procedure.Uid, args);
					}
					)
				{
					Name = "Run Procedure",
				};
				thread.Start();
				while (!thread.Join(50))
					ApplicationService.DoEvents();
			}
		}
	}
}