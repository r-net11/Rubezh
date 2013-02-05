using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecClient;

namespace Common.GK
{
	public static class PumpStationCreator
	{
		public static void Create(GkDatabase gkDatabase, XDevice pumpStatioDevice)
		{
			if (pumpStatioDevice.PumpStationProperty == null)
				return;

			var delayTime = pumpStatioDevice.PumpStationProperty.DelayTime;
			var pumpsCount = pumpStatioDevice.PumpStationProperty.PumpsCount;

			var delays = new List<XDelay>();

			var pumpDevices = new List<XDevice>();
			foreach (var pumpDeviceUID in pumpStatioDevice.PumpStationProperty.DeviceUIDs)
			{
				var pumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == pumpDeviceUID);
				if (pumpDevice != null)
				{
					var addressOnShleif = pumpDevice.IntAddress % 256;
					if (addressOnShleif <= 8)
					{
						pumpDevices.Add(pumpDevice);
					}
				}
			}

			var mainDelay = new XDelay()
			{
				Name = "Задержка пуска НС",
				DelayTime = (ushort)(0),
				SetTime = 1,
				InitialState = false,
			};
			//mainDelay.InputObjects.Add(pumpDevice);
			//pumpDevice.OutputObjects.Add(delay);
			delays.Add(mainDelay);

			int pumpIndex = 1;
			foreach (var pumpDevice in pumpDevices)
			{
				var delay = new XDelay()
				{
					Name = "Задержка пуска ШУН " + pumpIndex.ToString(),
					DelayTime = (ushort)(pumpIndex * delayTime),
					SetTime = 1,
					InitialState = false,
				};
				delay.InputObjects.Add(pumpDevice);
				pumpDevice.OutputObjects.Add(delay);
				delays.Add(delay);
			}
			pumpIndex++;

			foreach (var pumpDevice in pumpDevices)
			{
				var pumpBinaryObject = gkDatabase.BinaryObjects.FirstOrDefault(x => x.Device != null && x.Device.UID == pumpDevice.UID);
				if (pumpBinaryObject != null)
				{
					var formula = new FormulaBuilder();
					var inputPumpsCount = 0;
					foreach (var otherPumpDevice in pumpDevices)
					{
						if (otherPumpDevice.UID == pumpDevice.UID)
							continue;

						formula.AddGetBit(XStateType.TurningOn, otherPumpDevice);
						formula.AddGetBit(XStateType.On, otherPumpDevice);
						formula.Add(FormulaOperationType.OR);
						if (inputPumpsCount > 0)
						{
							formula.Add(FormulaOperationType.ADD); // почситать количество включеных насосов
						}
						inputPumpsCount++;
					}
					formula.Add(FormulaOperationType.CONST, 0, pumpsCount, "Количество основных пожарных насосов");
					formula.Add(FormulaOperationType.LT);
					formula.AddGetBit(XStateType.Norm, pumpDevice);
					formula.Add(FormulaOperationType.AND); // бит дежурный у самого насоса

					formula.AddGetBit(XStateType.On, mainDelay);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(XStateType.TurnOn, pumpDevice); // включить насос

					formula.AddGetBit(XStateType.On, mainDelay);
					formula.Add(FormulaOperationType.COM);
					formula.AddGetBit(XStateType.Norm, pumpDevice);
					formula.Add(FormulaOperationType.AND); // бит дежурный у самого насоса
					formula.AddPutBit(XStateType.TurnOff, pumpDevice); // выключить насос

					pumpBinaryObject.Formula.Add(FormulaOperationType.END);
					pumpBinaryObject.Formula = formula;
					pumpBinaryObject.FormulaBytes = pumpBinaryObject.Formula.GetBytes();
				}
			}

			foreach (var pumpDevice in pumpDevices)
			{
				foreach (var delay in delays)
				{
					pumpDevice.InputObjects.Add(delay);
					delay.OutputObjects.Add(pumpDevice);
				}
			}

			foreach (var delay in delays)
			{
				gkDatabase.AddDelay(delay);
				var deviceBinaryObject = new DelayBinaryObject(delay);
				gkDatabase.BinaryObjects.Add(deviceBinaryObject);
			}
		}
	}
}