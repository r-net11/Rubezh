using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class ByteDescription
	{
		public ByteDescription()
		{
			Children = new List<ByteDescription>();
		}

		public TableBase TableHeader { get; set; }

		public int Offset { get; set; }
		public int Value { get; set; }
		public string Description { get; set; }
		public string GroupName { get; set; }
		public bool IsBold { get; set; }
		public int ByteIndex { get; set; }

		public ByteDescription AddressReference { get; set; }
		public TableBase TableBaseReference { get; set; }

		public List<ByteDescription> Children { get; set; }
	}
}