using System;
using System.Collections.Generic;

namespace ServerFS2
{
	public class Response
	{
		public uint Id { get; set; }
		public List<byte> Data { get; set; }
		public TimeSpan TimeSpan { get; set; }
	}
}