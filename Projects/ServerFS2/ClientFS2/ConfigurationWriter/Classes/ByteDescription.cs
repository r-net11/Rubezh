using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace ClientFS2.ConfigurationWriter
{
	public class ByteDescription : TreeItemViewModel<ByteDescription>
	{
		public TableBase TableHeader { get; set; }

		public int RelativeOffset { get; set; }
		public int Offset { get; set; }
		public int Value { get; set; }
		public string Description { get; set; }
		public string GroupName { get; set; }
		public string RealValue { get; set; }

		public ByteDescription AddressReference { get; set; }
		public TableBase TableBaseReference { get; set; }

		public bool IsBold { get; set; }
		public bool IsHeader { get; set; }
		public bool HasNoOffset { get; set; }

		public bool IsNotEqualToOriginal { get; set; }
        public int OriginalValue { get; set; }
        public bool IsReadOnly { get; set; }
	}
}