using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class DriverParameter
	{
		public DriverParameter()
		{
			ParameterEnumItems = new List<ParameterEnumItem>();
		}
		public string Name { get; set; }
		public int Number { get; set; }
		public bool IsReadOnly { get; set; }
		public ParameterType ParameterType { get; set; }
		public List<ParameterEnumItem> ParameterEnumItems { get; set; }
	}
}
