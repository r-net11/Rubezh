using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerFS2
{
	public class JournalItem
	{
		public int No { get; set; }
		public string Date { get; set; }
		public string EventName { get; set; }
		public int Flag { get; set; }
		public int ShleifNo { get; set; }
		public int IntType { get; set; }
        public int FirstAddress { get; set; }
		public int Address { get; set; }
		public int State { get; set; }
		public int ZoneNo { get; set; }
		public int DescriptorNo { get; set; }
		public string StringType { get; set; }
		public int EventClass { get; set; }
        public string Description { get; set; }
        public string ByteTracer { get; set; }
	    public uint IntDate { get; set; }
	}
}