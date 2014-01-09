using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class AdditionalColumn
	{
		public Guid Uid;
		public string Name { get; set; }
		public string Description { get; set; }
		public AdditionalColumnData Data { get; set; }
	}

	public class AdditionalColumnData
	{
		public DataType Type { get; set; }
		public object Data { get; set; }
	}

	public enum DataType
	{
		Text,
		Graphics,
		Mixed
	}
}
