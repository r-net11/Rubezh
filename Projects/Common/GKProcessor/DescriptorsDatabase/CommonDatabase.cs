using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public abstract class CommonDatabase
	{
		ushort currentChildNo = 1;
		protected List<GKDevice> Devices { get; set; }
		protected List<GKDirection> Directions { get; set; }
		protected List<GKPumpStation> PumpStations { get; set; }
		protected List<GKMPT> MPTs { get; set; }
		public List<GKDelay> Delays { get; private set; }
		public List<GKPim> Pims { get; private set; }

		protected ushort NextDescriptorNo
		{
			get { return currentChildNo++; }
		}

		public DatabaseType DatabaseType { get; protected set; }
		public GKDevice RootDevice { get; protected set; }
		public List<BaseDescriptor> Descriptors { get; set; }

		public CommonDatabase()
		{
			Devices = new List<GKDevice>();
			Directions = new List<GKDirection>();
			PumpStations = new List<GKPumpStation>();
			MPTs = new List<GKMPT>();
			Delays = new List<GKDelay>();
			Pims = new List<GKPim>();
			Descriptors = new List<BaseDescriptor>();
		}

		public void AddDelay(GKDelay delay)
		{
			if (!Delays.Contains(delay))
			{
				if (DatabaseType == DatabaseType.Gk)
				{
					delay.GKDescriptorNo = NextDescriptorNo;
					delay.GkDatabaseParent = RootDevice;
				}
				else
				{
					delay.KAUDescriptorNo = NextDescriptorNo;
					delay.KauDatabaseParent = RootDevice;
					delay.IsLogicOnKau = true;
					delay.GkDatabaseParent = RootDevice.GKParent;
				}
				Delays.Add(delay);
			}
		}

		public void AddPim(GKPim pim)
		{
			if (!Pims.Contains(pim) && pim != null)
			{
				if (DatabaseType == DatabaseType.Gk)
				{
					pim.GKDescriptorNo = NextDescriptorNo;
					pim.GkDatabaseParent = RootDevice;
				}
				else
				{
					pim.KAUDescriptorNo = NextDescriptorNo;
					pim.KauDatabaseParent = RootDevice;
					pim.GkDatabaseParent = RootDevice.GKParent;
				}
				Pims.Add(pim);
			}
		}

		public abstract void BuildObjects();
	}
}