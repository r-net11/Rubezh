using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestUSB
{
	public static class DriverTypeHelper
	{
		public static List<DriverItem> DriverItems { get; private set; }

		static DriverTypeHelper()
		{
			DriverItems = new List<DriverItem>();
		}
	}

	public class DriverItem
	{
		public DriverItem(int code, string name, Guid uid)
		{
			Code = code;
			Name = name;
			UID = uid;
		}

		public int Code { get; set; }
		public string Name { get; set; }
		public Guid UID { get; set; }
	}
}