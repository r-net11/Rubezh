using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class DeviceCustomFunctionConverter
    {
        public static List<DeviceCustomFunction> Convert(Firesec.DeviceCustomFunctions.functions functions)
        {
            var deviceCustomFunctions = new List<DeviceCustomFunction>();
            if ((functions != null) && (functions.Items != null))
            {
                foreach (var function in functions.Items)
                {
                    var deviceCustomFunction = new DeviceCustomFunction()
                    {
                        Code = function.code,
                        Description = function.desc,
                        Name = function.name
                    };
                    deviceCustomFunctions.Add(deviceCustomFunction);
                }
                return deviceCustomFunctions;
            }

            return null;
        }
    }
}
