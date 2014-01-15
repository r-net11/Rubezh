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
		public AdditionalColumnType Type { get; set; }
        public string TextData { get; set; }
        public byte[] GraphicsData { get; set; }
	}

	public enum AdditionalColumnType
	{
		Text,
		Graphics,
		Mixed
	}
}
