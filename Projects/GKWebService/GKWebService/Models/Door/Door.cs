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
	public class Door
	{
		public Door(GKDoor door)
		{
			UID = door.UID;
			No = door.No;
			GKDescriptorNo = door.GKDescriptorNo;
			Name = door.Name;
			DoorType = door.DoorType.ToDescription();
			FullCanControl = ClientManager.CheckPermission(PermissionType.Oper_Full_Door_Control);
			CanControl = ClientManager.CheckPermission(PermissionType.Oper_Door_Control);
			Desription = door.Description;
			ImageSource = door.ImageSource.Replace("/Controls;component/", "");
			
			State = door.State.StateClass.ToDescription();
			StateIcon = door.State.StateClass.ToString();
			StateClasses = door.State.StateClasses.Select(x => new StateClass(x)).ToList();
			StateColor = "'#" + new XStateClassToColorConverter2().Convert(door.State.StateClass, null, null, null).ToString().Substring(3) + "'";


			OnDelay = door.State.OnDelay;
			HasOnDelay = door.State.StateClasses.Contains(XStateClass.TurningOn) && door.State.OnDelay > 0;
			HasOffDelay = door.State.StateClasses.Contains(XStateClass.TurningOff) && door.State.OnDelay > 0;
			OffDelay = door.State.OffDelay;
			HasHoldDelay = door.State.StateClasses.Contains(XStateClass.Attention) && door.State.OffDelay > 0;

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
		public Guid UID { get; set; }
		public int No { get; set; }
		public string Name { get; set; }
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
		public int OnDelay { get; set; }
		public int OffDelay { get; set; }
		public bool HasOffDelay { get; set; }
		public bool HasHoldDelay { get; set; }
		public bool FullCanControl { get; set; }
		public bool CanControl { get; set; }

		public string DoorType { get; set; }

		public string Desription { get; set; }

		public string ImageSource { get; set; }
	}
}