using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Controls.Converters;
using GKWebService.Converters;
using RubezhAPI;
using GKWebService.Models.GK;

namespace GKWebService.Models.Door
{
	public class Door : GKBaseModel
	{
		public Door()
		{
			
		}

        public Door(GKDoor door)
			: base(door)
		{
			No = door.No;
			GKDescriptorNo = door.GKDescriptorNo;
			DoorTypeString = door.DoorType.ToDescription();
			DoorType = door.DoorType;
			FullCanControl = ClientManager.CheckPermission(PermissionType.Oper_Full_Door_Control);
			CanControl = ClientManager.CheckPermission(PermissionType.Oper_Door_Control);
			Desription = door.Description;
			ImageSource = door.ImageSource.Replace("/Controls;component/", "");
			if (door.EnterDevice != null)
				EnterDevice = new DoorDevice(door.EnterDevice);
			if (door.ExitDevice != null)
				ExitDevice = new DoorDevice(door.ExitDevice);
			if (door.EnterButton != null)
				EnterButton = new DoorDevice(door.EnterButton);
			if (door.ExitButton != null)
				ExitButton = new DoorDevice(door.ExitButton);
			if (door.LockDevice != null)
				LockDevice = new DoorDevice(door.LockDevice);
			if (door.LockDeviceExit != null)
				LockDeviceExit = new DoorDevice(door.LockDeviceExit);
			if (door.LockControlDevice != null)
				LockControlDevice = new DoorDevice(door.LockControlDevice);
			if (door.LockControlDeviceExit != null)
				LockControlDeviceExit = new DoorDevice(door.LockControlDeviceExit);
			var zone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.ExitZoneUID);
			if (zone != null)
				ExitZone = new Tuple<string, Guid>(zone.PresentationName, zone.UID);
			 zone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.EnterZoneUID);
			if (zone != null)
				EnterZone = new Tuple<string, Guid>(zone.PresentationName, zone.UID);
			OpenRegimeLogic = GKManager.GetPresentationLogic(door.OpenRegimeLogic.OnClausesGroup);
			NormRegimeLogic = GKManager.GetPresentationLogic(door.NormRegimeLogic.OnClausesGroup  );
			CloseRegimeLogic = GKManager.GetPresentationLogic(door.CloseRegimeLogic.OnClausesGroup);


			State = DoorStateClassToStringConverter.Converter(door.State.StateClass);
			StateIcon = door.State.StateClass.ToString();
			StateClasses = door.State.StateClasses.Select(x => new StateClass { Name = DoorStateClassToStringConverter.Converter(x), IconData = x.ToString() }).ToList();
			StateColor = "'#" + new XStateClassToColorConverter2().Convert(door.State.StateClass, null, null, null).ToString().Substring(3) + "'";

			HasHoldDelay = door.State.StateClasses.Contains(XStateClass.On) && door.State.HoldDelay > 0;
			HasOffDelay = door.State.StateClasses.Contains(XStateClass.TurningOff) && door.State.OffDelay > 0;
			OffDelay = HasOffDelay?  door.State.OffDelay.ToString(): string.Empty;
			HoldDelay = HasHoldDelay?  door.State.HoldDelay.ToString(): string.Empty;

			var controlRegime = door.State.StateClasses.Contains(XStateClass.Ignore)
				? DeviceControlRegime.Ignore
				: !door.State.StateClasses.Contains(XStateClass.AutoOff) ? DeviceControlRegime.Automatic : DeviceControlRegime.Manual;
			//ControlRegimeIcon = "data:image/gif;base64," + InternalConverter.GetImageResource(((string)new DeviceControlRegimeToIconConverter().Convert(controlRegime)) ?? string.Empty).Item1;
			ControlRegimeName = controlRegime.ToDescription();
			ControlRegimeIcon = (new DeviceControlRegimeToIconConverter()).Convert(controlRegime);
			CanSetAutomaticState = (controlRegime != DeviceControlRegime.Automatic);
			CanSetManualState = (controlRegime != DeviceControlRegime.Manual);
			CanSetIgnoreState = (controlRegime != DeviceControlRegime.Ignore);
			IsControlRegime = (controlRegime == DeviceControlRegime.Manual);

		}
		public int No { get; set; }
		public String OnClausesGroup { get; set; }
		public string StateIcon { get; set; }
		public bool CanSetAutomaticState { get; set; }
		public bool CanSetManualState { get; set; }
		public bool CanSetIgnoreState { get; set; }
		public bool IsControlRegime { get; set; }
		public string ControlRegimeName { get; set; }
		public string ControlRegimeIcon { get; set; }
		public bool HasOnDelay { get; set; }
		public ushort GKDescriptorNo { get; set; }
		public List<StateClass> StateClasses { get; set; }
		public string StateColor { get; set; }
		public string State { get; set; }
		public string HoldDelay { get; set; }
		public string  OffDelay { get; set; }
		public bool HasOffDelay { get; set; }
		public bool HasHoldDelay { get; set; }
		public bool FullCanControl { get; set; }
		public bool CanControl { get; set; }

		public string DoorTypeString { get; set; }

		public string Desription { get; set; }

		public DoorDevice EnterDevice { get; private set; }
		public DoorDevice ExitDevice { get; private set; }
		public DoorDevice EnterButton { get; private set; }
		public DoorDevice ExitButton { get; private set; }
		public DoorDevice LockDevice { get; private set; }
		public DoorDevice LockDeviceExit { get; private set; }
		public DoorDevice LockControlDevice { get; private set; }
		public DoorDevice LockControlDeviceExit { get; private set; }

		public string OpenRegimeLogic { get; set; }

		public string NormRegimeLogic { get; set; }

		public string CloseRegimeLogic { get; set; }

		public GKDoorType DoorType { get; set; }

		public Tuple<string, Guid> ExitZone { get; set; }

		public Tuple<string, Guid> EnterZone { get; set; }
	}
}