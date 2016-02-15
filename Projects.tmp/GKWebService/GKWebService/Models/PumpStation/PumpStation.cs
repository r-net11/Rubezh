using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.GK;
using RubezhAPI;
//using Controls.Converters;
using GKWebService.Converters;
using GKWebService.Models.GK;

namespace GKWebService.Models.PumpStation
{
	public class PumpStation
	{
		public PumpStation(GKPumpStation pumpStation)
		{
			UID = pumpStation.UID;
			No = pumpStation.No;
			GKDescriptorNo = pumpStation.GKDescriptorNo;
			Name = pumpStation.Name;
			StartLogic = GKManager.GetPresentationLogic(pumpStation.StartLogic);
			StopLogic = GKManager.GetPresentationLogic(pumpStation.StopLogic);
			AutomaticOffLogic = GKManager.GetPresentationLogic(pumpStation.AutomaticOffLogic);
			Delay = pumpStation.Delay;
			Hold = pumpStation.Hold;
			DelayRegime = pumpStation.DelayRegime.ToDescription();
			NSPumpsCount = pumpStation.NSPumpsCount;
			NSDeltaTime = pumpStation.NSDeltaTime;

			State = pumpStation.State.StateClass.ToDescription();
			StateIcon = pumpStation.State.StateClass.ToString();
			StateClasses = pumpStation.State.StateClasses.Select(x => new DirectionStateClass(x)).ToList();
			//StateColor = "'#" + new XStateClassToColorConverter2().Convert(pumpStation.State.StateClass, null, null, null).ToString().Substring(3) + "'";

			HasOnDelay = pumpStation.State.StateClasses.Contains(XStateClass.TurningOn) && pumpStation.State.OnDelay > 0;
			OnDelay = pumpStation.State.OnDelay;
			HoldDelay = pumpStation.State.HoldDelay;
			HasHoldDelay = pumpStation.State.StateClasses.Contains(XStateClass.On) && pumpStation.State.HoldDelay > 0;

			var controlRegime = pumpStation.State.StateClasses.Contains(XStateClass.Ignore)
				? DeviceControlRegime.Ignore
				: !pumpStation.State.StateClasses.Contains(XStateClass.AutoOff) ? DeviceControlRegime.Automatic : DeviceControlRegime.Manual;
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
		public String StartLogic { get; set; }
		public int Delay { get; set; }
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

		public int Hold { get; set; }

		public string State { get; set; }

		public int OnDelay { get; set; }

		public int HoldDelay { get; set; }

		public bool HasHoldDelay { get; set; }

		public string StopLogic { get; set; }

		public string AutomaticOffLogic { get; set; }

		public int NSPumpsCount { get; set; }

		public int NSDeltaTime { get; set; }
	}
}