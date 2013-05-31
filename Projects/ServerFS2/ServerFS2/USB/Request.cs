using System;
using System.Collections.Generic;

namespace ServerFS2
{
	public class Request
	{
		public DateTime StartTime = DateTime.Now;
		public uint Id { get; set; }
		public List<byte> Data { get; set; }
	}
}