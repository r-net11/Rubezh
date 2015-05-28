using PowerCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerCalculator.Processor
{
	public static class Processor
	{

        public static void CollectToRepository(Configuration configuration)
        {
            

            configuration.CableRepositoryItems = new List<CableRepositoryItem>();
            configuration.DeviceRepositoryItems = new List<DeviceRepositoryItem>();

            foreach (Line line in configuration.Lines)
                foreach (Device device in line.Devices)
                {
                    if (device.DriverType == DriverType.KAU)
                        continue;

                    DeviceRepositoryItem existingDevice = configuration.DeviceRepositoryItems.Where(x => x.DriverType == device.DriverType).FirstOrDefault();

                    if (existingDevice == null)
                    {
                        existingDevice = new DeviceRepositoryItem()
                        {
                            DriverType = device.DriverType,
                            Count = 1
          
                        };
                        configuration.DeviceRepositoryItems.Add(existingDevice);
                    }
                    else
                        existingDevice.Count++;

                    CableRepositoryItem existingCable = configuration.CableRepositoryItems.Where(x=>x.Resistivity == device.Cable.Resistivity).FirstOrDefault();

                    if (existingCable == null)
                    {
                        existingCable = new CableRepositoryItem()
                        {
                            Length = device.Cable.Length,
                            Resistivity = device.Cable.Resistivity
                        };
                        configuration.CableRepositoryItems.Add(existingCable);
                    }
                    else
                        existingCable.Length += device.Cable.Length;
                }
        }

        public static IEnumerable<CableRepositoryItem> GenerateFromRepository(Configuration configuration)
		{
			configuration.Lines = new List<Line>();
			var totalDevicesCount = configuration.DeviceRepositoryItems.Sum(x => x.Count * x.Driver.Mult);
            for (int i = 0; i <= totalDevicesCount / 255; i++)
            {
                configuration.Lines.Add(new Line().Init());
            }
			
			
            var expandedDeviceRepositoryItems = new List<DeviceRepositoryItem>();
            foreach (var deviceRepositoryItem in configuration.DeviceRepositoryItems)
                for (int i = 0; i < deviceRepositoryItem.Count; i++)
                    expandedDeviceRepositoryItems.Add(new DeviceRepositoryItem() { DriverType = deviceRepositoryItem.DriverType, Count = 1 } );

            
            var sortedDeviceRepositoryItems = expandedDeviceRepositoryItems.OrderByDescending(x=>x.Driver.DeviceType).ThenBy(x => x.Driver.I);

            bool needAnotherLine = false;
            for (int i = 0; i < configuration.Lines.Count; i++)
			{
                uint sum = 0;
                for (int j = i; j < expandedDeviceRepositoryItems.Count; j+=configuration.Lines.Count)
                    sum += expandedDeviceRepositoryItems[j].Driver.Mult;

                if (sum > 255)
                {
                    needAnotherLine = true;
                    break;
                }
            }

            if (needAnotherLine)
                configuration.Lines.Add(new Line());

            var lineNo = 0;
			foreach (var deviceRepositoryItem in sortedDeviceRepositoryItems)
			{
				var line = configuration.Lines[lineNo];
				lineNo++;
				if (lineNo >= configuration.Lines.Count)
					lineNo = 0;
                
                var device = new Device();
				device.DriverType = deviceRepositoryItem.DriverType;
                device.Cable.Length = 0;
                device.Cable.Resistivity = 0;
				line.Devices.Add(device);
				
			}
              
            //Cables
            var cableRemains = configuration.CableRepositoryItems.OrderBy(x=>x.Resistivity).ToList();
            var cablePieces = new List<CableRepositoryItem>();
                        
            do
            {
                bool goAgain = false;
                var totalCableLength = cableRemains.Sum(x => x.Length);
                var avarageCableLength = totalCableLength / (expandedDeviceRepositoryItems.Count - cablePieces.Count);

                for (int i = cableRemains.Count - 1; i >= 0 && cablePieces.Count < expandedDeviceRepositoryItems.Count; i--)
                {
                    if (cableRemains[i].Length < avarageCableLength)
                    {
                        goAgain = true;
                        cablePieces.Add(cableRemains[i]);
                        cableRemains.RemoveAt(i);
                        if (i < cableRemains.Count) 
                            i++;
                    }
                }

                if (cablePieces.Count >= expandedDeviceRepositoryItems.Count)
                    break;

                if (goAgain)
                    continue;

                int piecesCount = 0;
                int selectedIndex = 0;
                double maxPieceLength = -1;
                for (int i = cableRemains.Count - 1; i >= 0; i--)
                {
                    int pc;
                    if (cableRemains.Count == 1)
                        pc = expandedDeviceRepositoryItems.Count - cablePieces.Count;
                    else 
                        pc = cableRemains[i].Length % avarageCableLength == 0 ? (int)(cableRemains[i].Length / avarageCableLength) : (int)(cableRemains[i].Length / avarageCableLength) + 1;
                    
                    double pieceLength = cableRemains[i].Length / pc;

                    if (pieceLength > maxPieceLength)
                    {
                        maxPieceLength = pieceLength;
                        selectedIndex = i;
                        piecesCount = pc;
                    }
                }

                for (int i = 0; i < piecesCount; i++)
                    cablePieces.Add(new CableRepositoryItem() { Length = maxPieceLength, Resistivity = cableRemains[selectedIndex].Resistivity });
                cableRemains.RemoveAt(selectedIndex);
                    
            } while (cablePieces.Count < expandedDeviceRepositoryItems.Count);


            var sortedCablePieces = cablePieces.OrderByDescending(x => x.Resistivity).ThenBy(x => x.Length).ToList();

            lineNo = 0;
            var deviceNo = 1;

            foreach (var cablePiece in cablePieces)
            {
                var line = configuration.Lines[lineNo];
                
                if (deviceNo < line.Devices.Count)
                {
                    line.Devices[deviceNo].Cable.Length = cablePiece.Length;
                    line.Devices[deviceNo].Cable.Resistivity = cablePiece.Resistivity;
                }

                if (++lineNo >= configuration.Lines.Count)
                {
                    lineNo = 0;
                    deviceNo++;
                }    

            }
            

            return cableRemains;
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