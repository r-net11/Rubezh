using System.Collections.Generic;
using System.Linq;
using PowerCalculator.Models;

namespace PowerCalculator.Processor
{
	public static class Processor
	{
        public static List<DeviceSpecificationItem> CollectDevices(List<Line> lines)
        {
            var deviceSpecificationItems = new List<DeviceSpecificationItem>();

            foreach (Line line in lines)
                foreach (Device device in line.Devices)
                {
                    if (device.DriverType == DriverType.RSR2_KAU)
                        continue;

                    DeviceSpecificationItem existingDevice = deviceSpecificationItems.Where(x => x.DriverType == device.DriverType).FirstOrDefault();

                    if (existingDevice == null)
                    {
                        existingDevice = new DeviceSpecificationItem()
                        {
                            DriverType = device.DriverType,
                            Count = 1

                        };
                        deviceSpecificationItems.Add(existingDevice);
                    }
                    else
                        existingDevice.Count++;
                }

            return deviceSpecificationItems;
        }

        public static List<CableSpecificationItem> CollectCables(List<Line> lines)
        {
            var cableSpecificationItems = new List<CableSpecificationItem>();

            foreach (Line line in lines)
                foreach (Device device in line.Devices)
                {
                    if (device.DriverType == DriverType.RSR2_KAU)
                        continue;

                    CableSpecificationItem existingCable = cableSpecificationItems.Where(x => x.Resistivity == device.Cable.Resistivity && x.CableType == device.Cable.CableType).FirstOrDefault();

                    if (existingCable == null)
                    {
                        existingCable = new CableSpecificationItem()
                        {
                            Length = device.Cable.Length,
                            Resistivity = device.Cable.Resistivity,
                            CableType = device.Cable.CableType
                        };
                        cableSpecificationItems.Add(existingCable);
                    }
                    else
                        existingCable.Length += device.Cable.Length;
                }

            return cableSpecificationItems;
        }

        public static IEnumerable<CableSpecificationItem> GenerateFromSpecification(Configuration configuration)
        {
            return GenerateFromSpecification(configuration, configuration.DeviceSpecificationItems, configuration.CableSpecificationItems);
        }
        
        public static IEnumerable<CableSpecificationItem> GenerateFromSpecification(Configuration configuration, IList<DeviceSpecificationItem> deviceSpecificationItems, IList<CableSpecificationItem> cableSpecificationItems)
		{
            const int maxAdress = 240;
			configuration.Lines = new List<Line>();
			var totalDevicesCount = deviceSpecificationItems.Sum(x => x.Count * x.Driver.Mult);
            for (int i = 0; i <= totalDevicesCount / maxAdress; i++)
            {
                configuration.Lines.Add(new Line());
            }

            var expandedDeviceSpecificationItems = new List<DeviceSpecificationItem>();
            foreach (var deviceSpecificationItem in deviceSpecificationItems)
                for (int i = 0; i < deviceSpecificationItem.Count; i++)
                    expandedDeviceSpecificationItems.Add(new DeviceSpecificationItem() { DriverType = deviceSpecificationItem.DriverType, Count = 1 } );

            var sortedDeviceSpecificationItems = expandedDeviceSpecificationItems.OrderByDescending(x=>x.Driver.DeviceType).ThenBy(x => x.Driver.I);

            bool needAnotherLine = false;
            for (int i = 0; i < configuration.Lines.Count; i++)
			{
                uint sum = 0;
                for (int j = i; j < expandedDeviceSpecificationItems.Count; j+=configuration.Lines.Count)
                    sum += expandedDeviceSpecificationItems[j].Driver.Mult;

                if (sum > maxAdress)
                {
                    needAnotherLine = true;
                    break;
                }
            }

            if (needAnotherLine)
                configuration.Lines.Add(new Line());

            var lineNo = 0;
			foreach (var deviceSpecificationItem in sortedDeviceSpecificationItems)
			{
				var line = configuration.Lines[lineNo];
				lineNo++;
				if (lineNo >= configuration.Lines.Count)
					lineNo = 0;
                
                var device = new Device();
				device.DriverType = deviceSpecificationItem.DriverType;
                device.Cable.Length = 0;
                device.Cable.Resistivity = 0;
				line.Devices.Add(device);
				
			}
              
            //Cables
            var cableRemains = 
                cableSpecificationItems.OrderBy(x => x.Resistivity)
                .Select(x => new CableSpecificationItem() { CableType = x.CableType, Length = x.Length, Resistivity = x.Resistivity })
                .ToList();
            var cablePieces = new List<CableSpecificationItem>();
                        
            do
            {
                bool goAgain = false;
                var totalCableLength = cableRemains.Sum(x => x.Length);
                var averageCableLength = totalCableLength / (expandedDeviceSpecificationItems.Count - cablePieces.Count);
                if (averageCableLength > 1000)
                    averageCableLength = 1000;

                for (int i = cableRemains.Count - 1; i >= 0 && cablePieces.Count < expandedDeviceSpecificationItems.Count; i--)
                {
                    if (cableRemains[i].Length < averageCableLength)
                    {
                        goAgain = true;
                        cablePieces.Add(cableRemains[i]);
                        cableRemains.RemoveAt(i);
                        if (i < cableRemains.Count) 
                            i++;
                    }
                }

                if (cablePieces.Count >= expandedDeviceSpecificationItems.Count)
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
                        pc = expandedDeviceSpecificationItems.Count - cablePieces.Count;
                    else 
                        pc = cableRemains[i].Length % averageCableLength == 0 ? (int)(cableRemains[i].Length / averageCableLength) : (int)(cableRemains[i].Length / averageCableLength) + 1;
                    
                    double pieceLength = cableRemains[i].Length / pc;
                    if (pieceLength > 1000)
                        pieceLength = 1000;

                    if (pieceLength > maxPieceLength)
                    {
                        maxPieceLength = pieceLength;
                        selectedIndex = i;
                        piecesCount = pc;
                    }
                }

                for (int i = 0; i < piecesCount; i++)
                    cablePieces.Add(new CableSpecificationItem() { Length = maxPieceLength, Resistivity = cableRemains[selectedIndex].Resistivity, CableType = cableRemains[selectedIndex].CableType });

                if (selectedIndex < cableRemains.Count)
                {
                    cableRemains[selectedIndex].Length -= maxPieceLength * piecesCount;
                    if (cableRemains[selectedIndex].Length <= 0)
                        cableRemains.RemoveAt(selectedIndex);
                }

            } while (cablePieces.Count < expandedDeviceSpecificationItems.Count);


            var sortedCablePieces = cablePieces.OrderByDescending(x => x.Resistivity).ThenBy(x => x.Length).ToList();

            lineNo = 0;
            var deviceNo = 0;

            foreach (var cablePiece in cablePieces)
            {
                var line = configuration.Lines[lineNo];
                
                if (deviceNo < line.Devices.Count)
                {
                    line.Devices[deviceNo].Cable.Length = cablePiece.Length;
                    line.Devices[deviceNo].Cable.Resistivity = cablePiece.Resistivity;
                    line.Devices[deviceNo].Cable.CableType = cablePiece.CableType;
                }

                if (++lineNo >= configuration.Lines.Count)
                {
                    lineNo = 0;
                    deviceNo++;
                }    

            }

            return cableRemains;
		}

        public static IEnumerable<DeviceIndicator> CalculateLine(Line line)
        {
            var calcPower = new Algorithms.CalcPowerAlgorithm(line);
            calcPower.Calculate();

            foreach (Device device in line.Devices)
                yield return new DeviceIndicator(device, calcPower.Result[device].il, calcPower.Result[device].ud);
                        
        }       

        public static List<int> GetLinePatch(Line line)
        {
            var patch = new List<int>();
            
            Line testLine = new Line();
            testLine.IsCircular = line.IsCircular;
            testLine.Devices = line.Devices.ToList();

            int step = (int)(line.Devices.Count / 10);
            if (step == 0)
                step = 1;
            int index = 0;
            while (true)
            {
                if (!CalculateLine(testLine).Any(x => x.HasIError || x.HasUError))
                {
                    return patch;
                }

                if (testLine.MaxAdress >= 255)
                    return null;
                
                index += step;
                if (index >= testLine.Devices.Count)
                    index = testLine.Devices.Count - 1;
                InsertSupplier(testLine, index);
                
                if (CalculateLine(testLine).Any(x=>testLine.Devices.IndexOf(x.Device) < index && (x.HasIError || x.HasUError)))
                {
                    while (true)
                    {
                        testLine.Devices.RemoveAt(index);
                        index--;
                        InsertSupplier(testLine, index);
                        if (!CalculateLine(testLine).Any(x => testLine.Devices.IndexOf(x.Device) < index && (x.HasIError || x.HasUError)))
                        {
							AddPatchItem(patch, index);
                            break;
                        }
                    }
                }
                else
                {
                    if (!CalculateLine(testLine).Any(x => x.HasIError || x.HasUError))
                    {
                        AddPatchItem(patch, index);
                        return patch;
                    }
                    testLine.Devices.RemoveAt(index);
                }
            }
        }

        static void InsertSupplier(Line line, int index)
        {
            var supplier = new Device();
            supplier.DriverType = DriverType.RSR2_MP;
            supplier.Cable.CableType = line.Devices[index].Cable.CableType;
            supplier.Cable.Length = line.Devices[index].Cable.Length;
            supplier.Cable.Resistivity = line.Devices[index].Cable.Resistivity;
            line.Devices.Insert(index, supplier);
        }

        static void AddPatchItem(List<int> patch, int index)
        {
            patch.RemoveAll(x => x >= index);
            patch.Add(index);
        }
    }
}