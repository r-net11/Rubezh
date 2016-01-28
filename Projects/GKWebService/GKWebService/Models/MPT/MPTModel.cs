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
			Name = mpt.Name;
			MptLogic = GKManager.GetPresentationLogic(mpt.MptLogic);
			Delay = mpt.Delay;
			StateIcon = mpt.State.StateClass.ToString();

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
		
	}
}
