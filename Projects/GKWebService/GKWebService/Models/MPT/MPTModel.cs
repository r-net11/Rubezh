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
		public MPTModel()
		{
			MPTDevices = new List<MPTDevice>();
		}
		public Guid UID { get; set; }
		public int No { get; set; }
		public string Name { get; set; }
		public String MptLogic { get; set; }
		public List<MPTDevice> MPTDevices { get; set; }
		public int Delay { get; set; }
		public string StateIcon { get; set; }
		
	}
}
