using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerFS2
{
	public class FSInternalJournal
	{
		public int PanelAddress { get; set; }
		public int EventCode { get; set; }
		public int TimeBytes { get; set; }
		public int AdditionalEventCode { get; set; }
		public int ShleifNo { get; set; }
		public int DeviceType { get; set; }
		public int AddressOnShleif { get; set; }
		public int State { get; set; }
		public int ZoneNo { get; set; }
		public int DescriptorNo { get; set; }
	}
}