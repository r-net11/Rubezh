using PowerCalculator.Models;
using System.Collections.Generic;

namespace PowerCalculator.Processor
{
	public static class Processor
	{
		public static void GenerateFromRepository(Configuration configuration)
		{

		}

        public static IEnumerable<LineError> CalculateLine(Line line)
        {
            var calcPower = new Algorithms.CalcPowerAlgorithm(line);
            calcPower.Calculate();
            
            foreach (Device device in line.Devices)
            {
                double scale = calcPower.Result[device].il - DriversHelper.GetDriver(device.DriverType).Imax;
                if (scale > 0)
                    yield return new LineError(device, ErrorType.Current, scale);
                scale = DriversHelper.GetDriver(device.DriverType).Umin - calcPower.Result[device].ud;
                if (scale > 0)
                    yield return new LineError(device, ErrorType.Voltage, scale);
            }
        }
	}
}