//using Controls.Converters;
using GKWebService.Converters;
using GKWebService.Models.GK;
using GKWebService.Utils;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Windows.Documents;

namespace GKWebService.Models
{
	public class MPTModel
	{
		public MPTModel(GKMPT mpt)
		{
			UID = mpt.UID;
			No = mpt.No;
			GKDescriptorNo = mpt.GKDescriptorNo;
			Name = mpt.Name;
			OnClausesGroup = GKManager.GetPresentationLogic(mpt.MptLogic.OnClausesGroup);
			StopClausesGroup = GKManager.GetPresentationLogic(mpt.MptLogic.StopClausesGroup);
			OffClausesGroup = GKManager.GetPresentationLogic(mpt.MptLogic.OffClausesGroup);
			Delay = mpt.Delay;
			Hold = mpt.Hold;
 			DelayRegime = mpt.DelayRegime.ToDescription();

			State = mpt.State.StateClass.ToDescription();
			StateIcon = mpt.State.StateClass.ToString();
			StateClasses = mpt.State.StateClasses.Select(x => new DirectionStateClass(x)).ToList();
			//StateColor = "'#" + new XStateClassToColorConverter2().Convert(mpt.State.StateClass, null, null, null).ToString().Substring(3) + "'";

			HasOnDelay = mpt.State.StateClasses.Contains(XStateClass.TurningOn) && mpt.State.OnDelay > 0;
			OnDelay = mpt.State.OnDelay;
			HoldDelay = mpt.State.HoldDelay;
			HasHoldDelay = mpt.State.StateClasses.Contains(XStateClass.On) && mpt.State.HoldDelay > 0;

			var controlRegime = mpt.State.StateClasses.Contains(XStateClass.Ignore)
				? DeviceControlRegime.Ignore
				: !mpt.State.StateClasses.Contains(XStateClass.AutoOff) ? DeviceControlRegime.Automatic : DeviceControlRegime.Manual;
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

		public string StopClausesGroup { get; set; }

		public string OffClausesGroup { get; set; }
	}
}
