﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.Models.GK;
using RubezhAPI.GK;
using RubezhAPI;
using Controls.Converters;
using GKWebService.Utils;
using GKWebService.Converters;

namespace GKWebService.Models
{
	public class Direction
	{
		public Direction(GKDirection gkDirection)
		{
			var stateClass = gkDirection.State.StateClass;
			var stateClasses = gkDirection.State.StateClasses;

			UID = gkDirection.UID;
			No = gkDirection.No;
			Name = gkDirection.Name;
			Delay = gkDirection.Delay;
			Hold = gkDirection.Hold;
			DelayRegime = gkDirection.DelayRegime.ToDescription();
			Logic = GKManager.GetPresentationLogic(gkDirection.Logic);
			GKDescriptorNo = gkDirection.GKDescriptorNo;

			State = stateClass.ToDescription();
			StateIcon = stateClass.ToString();
			StateColor = "'#" + new XStateClassToColorConverter2().Convert(stateClass, null, null, null).ToString().Substring(3) + "'";
			StateClasses = stateClasses.Select(x => new DirectionStateClass(x)).ToList();

			OnDelay = gkDirection.State.OnDelay;
			HoldDelay = gkDirection.State.HoldDelay;
			HasOnDelay = stateClasses.Contains(XStateClass.TurningOn) && gkDirection.State.OnDelay > 0;
			HasHoldDelay = stateClasses.Contains(XStateClass.On) && gkDirection.State.HoldDelay > 0;

			var controlRegime = stateClasses.Contains(XStateClass.Ignore)
				? DeviceControlRegime.Ignore
				: !stateClasses.Contains(XStateClass.AutoOff) ? DeviceControlRegime.Automatic : DeviceControlRegime.Manual;
			ControlRegimeIcon = (new DeviceControlRegimeToIconConverter()).Convert(controlRegime);
			ControlRegimeName = controlRegime.ToDescription();
			CanSetAutomaticState = (controlRegime != DeviceControlRegime.Automatic);
			CanSetManualState = (controlRegime != DeviceControlRegime.Manual);
			CanSetIgnoreState = (controlRegime != DeviceControlRegime.Ignore);
			IsControlRegime = (controlRegime == DeviceControlRegime.Manual);
		}

		public Guid UID { get; set; }
		public int No { get; set; }
		public string Name { get; set; }
		public ushort GKDescriptorNo { get; set; }
		public ushort Delay { get; set; }
		public ushort Hold { get; set; }
		public string DelayRegime { get; set; }
		public string Logic { get; set; }

		public string State { get; set; }
		public string StateIcon { get; set; }
		public string StateColor { get; set; }
		public List<DirectionStateClass> StateClasses { get; set; }

		public int OnDelay { get; set; }
		public int HoldDelay { get; set; }
		public bool HasOnDelay { get; set; }
		public bool HasHoldDelay { get; set; }

		public string ControlRegimeName { get; set; }
		public string ControlRegimeIcon { get; set; }
		public bool CanSetAutomaticState { get; set; }
		public bool CanSetManualState { get; set; }
		public bool CanSetIgnoreState { get; set; }
		public bool IsControlRegime { get; set; }
	}
}