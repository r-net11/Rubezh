using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class Driver
	{
		public Driver()
		{
			Children = new List<DriverType>();
			DriverParameters = new List<DriverParameter>();
		}

		public DriverType DriverType { get; set; }
		public List<DriverType> Children { get; set; }
		public List<DriverParameter> DriverParameters { get; set; }

		public void AddParameter(string name, ParameterType parameterType, bool isReadOnly = false, List<ParameterEnumItem> parameterEnumItems = null) 
		{
			var driverParameter = new DriverParameter
			{
				Name = name,
				Number = DriverParameters.Count,
				ParameterType = parameterType,
				IsReadOnly = isReadOnly,
			};
			if (parameterEnumItems != null)
				driverParameter.ParameterEnumItems = parameterEnumItems;
			DriverParameters.Add(driverParameter);
		}
	}
}