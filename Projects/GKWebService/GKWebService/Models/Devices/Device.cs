﻿using Controls.Converters;
using GKWebService.Converters;
using GKWebService.Models.GK;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.Models;
using RubezhClient;

namespace GKWebService.Models
{
	public class Device
	{
		public string MPTDeviceType { get; set; }

		public string Address { get; set; }

		public string Description { get; set; }

		public Guid UID { get; set; }

		public Guid? ParentUID { get; set; }

		public int No { get; set; }

		public string Name { get; set; }

		public string StateIcon { get; set; }

		public bool CanSetAutomaticState { get; set; }

		public bool CanSetManualState { get; set; }

		public bool CanSetIgnoreState { get; set; }

		public bool IsControlRegime { get; set; }

		public string ControlRegimeName { get; set; }

		public string ControlRegimeIcon { get; set; }

		public bool HasOnDelay { get; set; }

		public ushort GKDescriptorNo { get; set; }

		public List<DirectionStateClass> StateClasses { get; set; }

		public string DelayRegime { get; set; }

		public string StateColor { get; set; }

		public string ImageSource { get; set; }

		public string State { get; set; }

		public int OnDelay { get; set; }

		public int HoldDelay { get; set; }

		public bool HasHoldDelay { get; set; }

		public int Level { get; set; }

		public List<GKProperty> Properties { get; set; }

		public List<GKDriverProperty> DriverProperties { get; set; }

		public bool IsZoneOrLogic { get; set; }

		public string PresentationZone { get; set; }

		public bool IsFireAndGuard { get; set; }

		public string GuardPresentationZone { get; set; }

		public string Logic { get; set; }

		public List<GKMeasureParameter> MeasureParameters { get; set; }

		public string NsLogic { get; set; }

		public Boolean IsBiStateControl { get; set; }

		public Boolean IsTriStateControl { get; set; }

		public Boolean HasReset { get; set; }

		public Device(GKDevice device)
		{
			UID = device.UID;
			ParentUID = device.Parent != null ? device.Parent.UID : (Guid?)null;
			GKDescriptorNo = device.GKDescriptorNo;
			Name = device.PresentationName;
			ImageSource = device.ImageSource.Replace("/Controls;component/", "");
			Address = device.DottedPresentationAddress;
			Description = device.Description;
			Logic = GKManager.GetPresentationLogic(device.Logic);
			NsLogic = GKManager.GetPresentationLogic(device.NSLogic);

			State = device.State.StateClass.ToDescription();
			StateIcon = device.State.StateClass.ToString();
			StateClasses = device.State.StateClasses.Select(x => new DirectionStateClass(x)).ToList();
			StateColor = "'#" + new XStateClassToColorConverter2().Convert(device.State.StateClass, null, null, null).ToString().Substring(3) + "'";

			HasOnDelay = device.State.StateClasses.Contains(XStateClass.TurningOn) && device.State.OnDelay > 0;
			OnDelay = device.State.OnDelay;
			HoldDelay = device.State.HoldDelay;
			HasHoldDelay = device.State.StateClasses.Contains(XStateClass.On) && device.State.HoldDelay > 0;

			IsFireAndGuard = device.Driver.HasZone && device.Driver.HasGuardZone;

			var isInPumpStation = (device.DriverType == GKDriverType.RSR2_Bush_Drenazh || device.DriverType == GKDriverType.RSR2_Bush_Fire
				|| device.DriverType == GKDriverType.RSR2_Bush_Jokey) && device.OutputDependentElements.Any(x => x as GKPumpStation != null);

			var canShowZones = device.Driver.HasZone || device.Driver.HasGuardZone;
			var canShowLogic = device.Driver.HasLogic && !device.IsInMPT && !isInPumpStation;

			IsZoneOrLogic = !device.IsInMPT && isInPumpStation && (canShowZones || canShowLogic || device.Driver.HasMirror);

			PresentationZone = GKManager.GetPresentationZoneAndGuardZoneOrLogic(device);
			GuardPresentationZone = GKManager.GetPresentationGuardZone(device);

			var controlRegime = device.State.StateClasses.Contains(XStateClass.Ignore)
				? DeviceControlRegime.Ignore
				: !device.State.StateClasses.Contains(XStateClass.AutoOff) ? DeviceControlRegime.Automatic : DeviceControlRegime.Manual;
			//ControlRegimeIcon = "data:image/gif;base64," + InternalConverter.GetImageResource(((string)new DeviceControlRegimeToIconConverter().Convert(controlRegime)) ?? string.Empty).Item1;
			ControlRegimeName = controlRegime.ToDescription();
			ControlRegimeIcon = (new DeviceControlRegimeToIconConverter()).Convert(controlRegime);
			CanSetAutomaticState = (controlRegime != DeviceControlRegime.Automatic);
			CanSetManualState = (controlRegime != DeviceControlRegime.Manual);
			CanSetIgnoreState = (controlRegime != DeviceControlRegime.Ignore);
			IsControlRegime = (controlRegime == DeviceControlRegime.Manual);
			Properties = device.Properties;
			DriverProperties = device.Driver.Properties;
			MeasureParameters = device.Driver.MeasureParameters;

			IsTriStateControl = device.Driver.IsControlDevice && ClientManager.CheckPermission(PermissionType.Oper_Device_Control);
			IsBiStateControl = device.Driver.IsDeviceOnShleif && !device.Driver.IsControlDevice 
				&& ClientManager.CheckPermission(PermissionType.Oper_Device_Control);
			HasReset = device.DriverType == GKDriverType.RSR2_MAP4;
		}
	}
}