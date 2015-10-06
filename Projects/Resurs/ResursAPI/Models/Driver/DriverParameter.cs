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
			IntDefaultValue = -1;
			DoubleDefaultValue = -1;
			DateTimeMinValue = new DateTime(1900, 1, 1);
			DateTimeMaxValue = new DateTime(9000, 1, 1);
			DateTimeDefaultValue = new DateTime(1900, 1, 1);
			EnumDefaultItem = 0;
			IsWriteToDevice = true;
		}
		public string Name { get; set; }
		public string Description { get; set; }
		public int Number { get; set; }
		public bool IsReadOnly { get; set; }
		public bool IsWriteToDevice { get; set; }
		public ParameterType ParameterType { get; set; }
		public List<ParameterEnumItem> ParameterEnumItems { get; set; }
		public int? IntMinValue { get; set; }
		public int? IntMaxValue { get; set; }
		public int IntDefaultValue { get; set;}
		public double? DoubleMinValue { get; set; }
		public double? DoubleMaxValue { get; set; }
		public double DoubleDefaultValue { get; set; }
		public DateTime? DateTimeMinValue { get; set; }
		public DateTime? DateTimeMaxValue { get; set; }
		public DateTime DateTimeDefaultValue { get; set; }
		public bool BoolDefaultValue { get; set;}
		public string StringDefaultValue { get; set; }
		public string RegEx { get; set; }
		public int EnumDefaultItem { get; set; }
	}
}