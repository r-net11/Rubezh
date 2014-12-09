using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public abstract class CommonDatabase
	{
		ushort currentChildNo = 1;
		protected List<GKDevice> Devices { get; set; }
		protected List<GKZone> Zones { get; set; }
		protected List<GKGuardZone> GuardZones { get; set; }
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
			Zones = new List<GKZone>();
			GuardZones = new List<GKGuardZone>();
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
				delay.GKDescriptorNo = NextDescriptorNo;
				delay.GkDatabaseParent = RootDevice;
				Delays.Add(delay);
			}
		}

		public void AddPim(GKPim pim)
		{
			if (!Pims.Contains(pim))
			{
				pim.GKDescriptorNo = NextDescriptorNo;
				pim.GkDatabaseParent = RootDevice;
				Pims.Add(pim);
			}
		}

		public abstract void BuildObjects();
	}
}