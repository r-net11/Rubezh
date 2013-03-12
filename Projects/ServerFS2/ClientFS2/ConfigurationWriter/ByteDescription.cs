using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientFS2.ConfigurationWriter
{
	public class ByteDescription
	{
		public int Offset { get; set; }
		public int Value { get; set; }
		public string Description { get; set; }
		public ByteDescription AddressReference { get; set; }
	}
}