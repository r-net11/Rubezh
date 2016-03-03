using Controls.Converters;
using GKWebService.Converters;
using GKWebService.Models.GK;
using RubezhAPI;
using RubezhAPI.GK;
using System.Collections.Generic;

namespace GKWebService.Models
{
	public class Delay : GKBaseModel
	{
		public int Number { get; set; }
		public string Name { get; set; }
		public string PresentationLogic { get; set; }
		public string OnDelay { get; set; }
		public string HoldDelay { get; set; }
		public string StateIcon { get; set; }
		public string StateColor { get; set; }
		public List<DelayStateClass> StateClasses { get; set; }
		public bool HasOnDelay { get; set; }
		public bool HasHoldDelay { get; set; }
		public string ControlRegimeName { get; set; }
		public string ControlRegimeIcon { get; set; }
		public bool CanSetAutomaticState { get; set; }
		public bool CanSetManualState { get; set; }
		public bool CanSetIgnoreState { get; set; }
		public bool IsControlRegime { get; set; }
		public string DelayRegimeName { get; set; }
		public ushort GkDescriptorNo { get; set; }
		public ushort DelayTime { get; set; }
		public ushort HoldTime { get; set; }

		public Delay()
		{
			
		}

		public Delay(GKDelay gkDelay)
			: base(gkDelay)
		{
			Number = gkDelay.No;
			Name = gkDelay.Name;
			PresentationLogic = GKManager.GetPresentationLogic(gkDelay.Logic);
			OnDelay = gkDelay.State.OnDelay != 0 ? string.Format("{0} сек", gkDelay.State.OnDelay) : string.Empty;
			HoldDelay = gkDelay.State.HoldDelay != 0 ? string.Format("{0} сек", gkDelay.State.HoldDelay) : string.Empty;
			StateIcon = gkDelay.State.StateClass.ToString();
			StateColor = "'#" + new XStateClassToColorConverter2().Convert(gkDelay.State.StateClass, null, null, null).ToString().Substring(3) + "'";
			StateClasses = new List<DelayStateClass>();
			gkDelay.State.StateClasses.ForEach(x => StateClasses.Add(new DelayStateClass(x)));
			HasOnDelay = gkDelay.State.StateClasses.Contains(XStateClass.TurningOn) && gkDelay.State.OnDelay > 0;
			HasHoldDelay = gkDelay.State.StateClasses.Contains(XStateClass.On) && gkDelay.State.HoldDelay > 0;
			var controlRegime = gkDelay.State.StateClasses.Contains(XStateClass.Ignore)
				? DeviceControlRegime.Ignore
				: !gkDelay.State.StateClasses.Contains(XStateClass.AutoOff) ? DeviceControlRegime.Automatic : DeviceControlRegime.Manual;
			ControlRegimeName = controlRegime.ToDescription();
			ControlRegimeIcon = (new DeviceControlRegimeToIconConverter()).Convert(controlRegime);
			CanSetAutomaticState = (controlRegime != DeviceControlRegime.Automatic);
			CanSetManualState = (controlRegime != DeviceControlRegime.Manual);
			CanSetIgnoreState = (controlRegime != DeviceControlRegime.Ignore);
			IsControlRegime = controlRegime == DeviceControlRegime.Manual;
			DelayRegimeName = gkDelay.DelayRegime.ToDescription();
			GkDescriptorNo = gkDelay.GKDescriptorNo;
			DelayTime = gkDelay.DelayTime;
			HoldTime = gkDelay.Hold;
		}
	}
}