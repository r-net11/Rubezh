using Controls.Converters;
using GKWebService.Converters;
using GKWebService.Models.GK;
using GKWebService.Utils;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Documents;

namespace GKWebService.Models
{
	public class MPTModel
	{
		public MPTModel(GKMPT mpt)
		{
			MPTDevices = new List<MPTDevice>();
			UID = mpt.UID;
			No = mpt.No;
			GKDescriptorNo = mpt.GKDescriptorNo;
			Name = mpt.Name;
			MptLogic = GKManager.GetPresentationLogic(mpt.MptLogic);
			Delay = mpt.Delay;
			StateIcon = mpt.State.StateClass.ToString();
			StateClasses = mpt.State.StateClasses.Select(x => new DirectionStateClass(x)).ToList();
			StateColor = "'#" + new XStateClassToColorConverter2().Convert(mpt.State.StateClass, null, null, null).ToString().Substring(3) + "'";
			HasOnMPT = mpt.State.StateClasses.Contains(XStateClass.TurningOn) && Delay > 0;
			var controlRegime = mpt.State.StateClasses.Contains(XStateClass.Ignore)
				? DeviceControlRegime.Ignore
				: !mpt.State.StateClasses.Contains(XStateClass.AutoOff) ? DeviceControlRegime.Automatic : DeviceControlRegime.Manual;
			//ControlRegimeIcon = "data:image/gif;base64," + InternalConverter.GetImageResource(((string)new DeviceControlRegimeToIconConverter().Convert(controlRegime)) ?? string.Empty).Item1;
			ControlRegimeName = controlRegime.ToDescription();
			CanSetAutomaticState = (controlRegime != DeviceControlRegime.Automatic);
			CanSetManualState = (controlRegime != DeviceControlRegime.Manual);
			CanSetIgnoreState = (controlRegime != DeviceControlRegime.Ignore);
			IsControlRegime = (controlRegime == DeviceControlRegime.Manual);

		}
		public Guid UID { get; set; }
		public int No { get; set; }
		public string Name { get; set; }
		public String MptLogic { get; set; }
		public List<MPTDevice> MPTDevices { get; set; }
		public int Delay { get; set; }
		public string StateIcon { get; set; }
		public bool CanSetAutomaticState { get; set; }
		public bool CanSetManualState { get; set; }
		public bool CanSetIgnoreState { get; set; }
		public bool IsControlRegime { get; set; }
		public string ControlRegimeName { get; set; }
		public string ControlRegimeIcon { get; set; }
		public bool HasOnMPT { get; set; }
		public ushort GKDescriptorNo { get; set; }
		public List<DirectionStateClass> StateClasses { get; set; }

		public string StateColor { get; set; }
	}
}
