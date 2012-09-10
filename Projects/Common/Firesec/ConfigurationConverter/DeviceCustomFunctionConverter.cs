using System.Collections.Generic;
using FiresecAPI.Models;

namespace Firesec
{
	public static class DeviceCustomFunctionConverter
	{
		public static List<DeviceCustomFunction> Convert(Firesec.Models.Functions.functions functions)
		{
			var deviceCustomFunctions = new List<DeviceCustomFunction>();
			if (functions != null && functions.Items != null)
			{
				foreach (var function in functions.Items)
				{
					deviceCustomFunctions.Add(new DeviceCustomFunction()
					{
						Code = function.code,
						Description = function.desc,
						Name = function.name
					});
				}

				return deviceCustomFunctions;
			}
			return null;
		}
	}
}