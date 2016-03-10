using Controls.Converters;
using GKWebService.Converters;
using GKWebService.Models.GK;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKWebService.Models.GuardZones
{
	public class GuardZone : GKBaseModel
	{
		public GuardZone()
		{
			
		}

		public GuardZone(GKGuardZone guardZone)
			: base(guardZone)
		{ 
			No = guardZone.No;
			GKDescriptorNo = guardZone.GKDescriptorNo;
			Name = guardZone.Name;
			AlarmDelay = guardZone.AlarmDelay;
			ResetDelay = guardZone.ResetDelay;
			SetDelay = guardZone.SetDelay;

			State = GuardZoneStateClassToStringConverter.Converter(guardZone.State.StateClass);  // guardZone.State.StateClass.ToDescription();
			StateIcon = guardZone.State.StateClass.ToString();
			StateClasses = guardZone.State.StateClasses.Select(x => new StateClass{ Name = GuardZoneStateClassToStringConverter.Converter(x), IconData = x.ToString() }).ToList();
			StateColor = "'#" + new XStateClassToColorConverter2().Convert(guardZone.State.StateClass, null, null, null).ToString().Substring(3) + "'";

		
			OnDelay = guardZone.State.OnDelay;
			HasOnDelay = guardZone.State.StateClasses.Contains(XStateClass.TurningOn) && guardZone.State.OnDelay > 0;
			HasOffDelay = guardZone.State.StateClasses.Contains(XStateClass.TurningOff) && guardZone.State.OnDelay > 0;
			OffDelay = guardZone.State.OffDelay;
			HasHoldDelay = guardZone.State.StateClasses.Contains(XStateClass.Attention) && guardZone.State.OffDelay > 0;

			var controlRegime = guardZone.State.StateClasses.Contains(XStateClass.Ignore)
				? DeviceControlRegime.Ignore
				: !guardZone.State.StateClasses.Contains(XStateClass.AutoOff) ? DeviceControlRegime.Automatic : DeviceControlRegime.Manual;
			//ControlRegimeIcon = "data:image/gif;base64," + InternalConverter.GetImageResource(((string)new DeviceControlRegimeToIconConverter().Convert(controlRegime)) ?? string.Empty).Item1;
			ControlRegimeName = controlRegime.ToDescription();
			ControlRegimeIcon = (new DeviceControlRegimeToIconConverter()).Convert(controlRegime);
			CanSetAutomaticState = (controlRegime != DeviceControlRegime.Automatic);
			CanSetManualState = (controlRegime != DeviceControlRegime.Manual);
			CanSetIgnoreState = (controlRegime != DeviceControlRegime.Ignore);
			IsControlRegime = (controlRegime == DeviceControlRegime.Manual);

		}
		public int No { get; set; }
		public string Name { get; set; }
		public String OnClausesGroup { get; set; }
		public int AlarmDelay { get; set; }
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

		public int SetDelay { get; set; }
		public string StateColor { get; set; }

		public int ResetDelay { get; set; }

		public string State { get; set; }

		public int OnDelay { get; set; }

		public int OffDelay { get; set; }

		public bool HasOffDelay { get; set; }


		public bool HasHoldDelay { get; set; }

	}
}