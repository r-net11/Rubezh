using PowerCalculator.Models;
using System.Collections.Generic;
using System.Linq;

namespace PowerCalculator.Processor
{
	public static class Processor
	{
		public static void GenerateFromRepository(Configuration configuration)
		{
			configuration.Lines = new List<Line>();
			var totalDevicesCount = configuration.DeviceRepositoryItems.Sum(x => x.Count * x.Driver.Mult);
			for (int i = 0; i <= totalDevicesCount / 255; i++)
			{
				configuration.Lines.Add(new Line());
			}

			var lineNo = 0;
			var sortedDeviceRepositoryItems = configuration.DeviceRepositoryItems.OrderBy(x => x.Driver.I);
			foreach (var deviceRepositoryItem in sortedDeviceRepositoryItems)
			{
				for (int i = 0; i < deviceRepositoryItem.Count; i++)
				{
					var line = configuration.Lines[lineNo];
					lineNo++;
					if (lineNo >= configuration.Lines.Count)
						lineNo = 0;

					var device = new Device();
					device.DriverType = deviceRepositoryItem.DriverType;
					line.Devices.Add(device);
				}
			}

			//var totalCableLenght = configuration.CableRepositoryItems.Sum(x => x.Lenght);
			//var avarageCableLenght = totalCableLenght / totalDevicesCount;
			//foreach (var cableRepositoryItem in configuration.CableRepositoryItems)
			//{
			//    cableRepositoryItem.PercentsOfTotalLenght = cableRepositoryItem.Lenght / totalCableLenght;
			//}

			//foreach (var line in configuration.Lines)
			//{
			//    for (int i = 0; i < line.Devices.Count; i++)
			//    {
			//        var device = line.Devices[i];
			//    }
			//}
		}

        public static IEnumerable<LineError> CalculateLine(Line line)
        {
            var calcPower = new Algorithms.CalcPowerAlgorithm(line);
            calcPower.Calculate();
            
            foreach (Device device in line.Devices)
            {
                double scale = calcPower.Result[device].il - device.Driver.Imax;
                if (scale > 0)
                    yield return new LineError(device, ErrorType.Current, scale);
                scale = device.Driver.Umin - calcPower.Result[device].ud;
                if (scale > 0)
                    yield return new LineError(device, ErrorType.Voltage, scale);
            }
        }
	}
}