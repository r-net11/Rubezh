using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public abstract class CommonDatabase
	{
		ushort currentChildNo = 1;
		protected List<GKDevice> Devices { get; set; }

		protected ushort NextDescriptorNo
		{
			get { return currentChildNo++; }
		}

		public DatabaseType DatabaseType { get; protected set; }
		public GKDevice RootDevice { get; protected set; }

		public CommonDatabase()
		{
			Devices = new List<GKDevice>();
		}

		public abstract void BuildObjects();
	}
}