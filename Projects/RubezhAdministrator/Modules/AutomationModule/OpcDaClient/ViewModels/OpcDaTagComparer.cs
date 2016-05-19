using RubezhAPI.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationModule.ViewModels
{
	public class OpcDaTagComparer: IEqualityComparer<OpcDaTag>
	{
		public bool Equals(OpcDaTag x, OpcDaTag y)
		{
			return x.Uid == y.Uid;
		}

		public int GetHashCode(OpcDaTag obj)
		{
			return obj.GetHashCode();
		}
	}
}