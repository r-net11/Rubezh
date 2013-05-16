using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.GK;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Common;

namespace GKModule
{
    public static class BinConfigurationWriter
    {
        public static void WriteConfig()
        {
			try
			{
				DatabaseManager.Convert();
				var result = Ping();
				if (!result)
					return;

				//SendManager.StrartLog("D:/GkLog.txt");
				foreach (var gkDatabase in DatabaseManager.GkDatabases)
				{
					var summaryObjectsCount = 4 + gkDatabase.BinaryObjects.Count;
					gkDatabase.KauDatabases.ForEach(x => { summaryObjectsCount += 3 + x.BinaryObjects.Count; });
					LoadingService.ShowProgress("", "Запись конфигурации в " + gkDatabase.RootDevice.PresentationDriverAndAddress, summaryObjectsCount);

					result = GoToTechnologicalRegime(gkDatabase.RootDevice);
					if (!result)
					{
						//MessageBoxService.ShowError("Не удалось перевести устройство в технологический режим");
						//return;
					}
					EraseDatabase(gkDatabase.RootDevice);

					foreach (var kauDatabase in gkDatabase.KauDatabases)
					{
						result = GoToTechnologicalRegime(kauDatabase.RootDevice);
						if (!result)
						{
							MessageBoxService.ShowError("Не удалось перевести устройство в технологический режим");
							return;
						}
						EraseDatabase(kauDatabase.RootDevice);
						var writeResult = WriteConfigToDevice(kauDatabase);
						if (!writeResult)
							return;
					}
					var writeResult2 = WriteConfigToDevice(gkDatabase);
					if (!writeResult2)
						return;

					foreach (var kauDatabase in gkDatabase.KauDatabases)
					{
						GoToWorkingRegime(kauDatabase.RootDevice);
					}
					GoToWorkingRegime(gkDatabase.RootDevice);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "BinConfigurationWriter.WriteConfig");
			}
			finally
			{
				LoadingService.Close();
				//SendManager.StopLog();
			}
        }

		static bool Ping()
		{
			foreach (var gkDatabase in DatabaseManager.GkDatabases)
			{
                    var sendResult = SendManager.Send(gkDatabase.RootDevice, 0, 1, 1);
					if (sendResult.HasError)
					{
						MessageBoxService.ShowError("Устройство " + gkDatabase.RootDevice.ShortPresentationAddressAndDriver + " недоступно");
						return false;
					}

				foreach (var kauDatabase in gkDatabase.KauDatabases)
				{
					sendResult = SendManager.Send(kauDatabase.RootDevice, 0, 1, 1);
					if (sendResult.HasError)
					{
						MessageBoxService.ShowError("Устройство " + kauDatabase.RootDevice.ShortPresentationAddressAndDriver + " недоступно");
						return false;
					}
				}
			}
			return true;
		}

        static bool WriteConfigToDevice(CommonDatabase commonDatabase)
        {
            foreach (var binaryObject in commonDatabase.BinaryObjects)
            {
                var progressStage = commonDatabase.RootDevice.PresentationDriverAndAddress + ": запись " +
					binaryObject.BinaryBase.GetBinaryDescription() + " " +
                    "(" + binaryObject.GetNo().ToString() + ")" +
					" из " + commonDatabase.BinaryObjects.Count.ToString();
                LoadingService.DoStep(progressStage);
                var packs = BinConfigurationWriter.CreateDescriptors(binaryObject);
                foreach (var pack in packs)
                {
                    var packBytesCount = pack.Count;
                    var sendResult = SendManager.Send(commonDatabase.RootDevice, (ushort)(packBytesCount), 17, 0, pack, true);
                    if (sendResult.HasError)
                    {
						LoadingService.Close();
						return MessageBoxService.ShowQuestion(sendResult.Error + "\n\nПперевести устройства в рабочий режим") == System.Windows.MessageBoxResult.Yes;
                    }
                }
            }
            WriteEndDescriptor(commonDatabase);
			return true;
        }

        static List<List<byte>> CreateDescriptors(BinaryObjectBase binaryObject)
        {
            var objectNo = (ushort)(binaryObject.GetNo());

            var packs = new List<List<byte>>();
            for (int packNo = 0; packNo <= binaryObject.AllBytes.Count / 256; packNo++)
            {
                int packLenght = Math.Min(256, binaryObject.AllBytes.Count - packNo * 256);
                var packBytes = binaryObject.AllBytes.Skip(packNo * 256).Take(packLenght).ToList();

                if (packBytes.Count > 0)
                {
                    var resultBytes = new List<byte>();
                    ushort binaryObjectNo = (ushort)(binaryObject.GetNo());
                    resultBytes.AddRange(BytesHelper.ShortToBytes(binaryObjectNo));
                    resultBytes.Add((byte)(packNo + 1));
                    resultBytes.AddRange(packBytes);
                    packs.Add(resultBytes);
                }
            }
            return packs;
        }

        static List<byte> CreateEndDescriptor(ushort descriptorNo)
        {
            var resultBytes = new List<byte>();
            resultBytes.AddRange(BytesHelper.ShortToBytes(descriptorNo));
            resultBytes.Add(1);
            resultBytes.Add(255);
            resultBytes.Add(255);
            return resultBytes;
        }

        public static bool GoToTechnologicalRegime(XDevice device)
        {
            LoadingService.DoStep(device.PresentationDriverAndAddress + " Переход в технологический режим");
			var sendResult = SendManager.Send(device, 0, 14, 0, null, device.Driver.DriverType == XDriverType.GK);
			if (sendResult.HasError)
			{
				return false;
			}

            for (int i = 0; i < 10; i++)
            {
				sendResult = SendManager.Send(device, 0, 1, 1, null, true, false, 2000);
                if (!sendResult.HasError)
                {
                    if (sendResult.Bytes.Count > 0)
                    {
                        var version = sendResult.Bytes[0];
                        if (version > 127)
                        {
                            return true;
                        }
                    }
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

			return false;
        }

        public static void GoToWorkingRegime(XDevice device)
        {
            LoadingService.DoStep(device.PresentationDriverAndAddress + " Переход в рабочий режим");
            SendManager.Send(device, 0, 11, 0, null, device.Driver.DriverType == XDriverType.GK);

            for (int i = 0; i < 10; i++)
            {
                var sendResult = SendManager.Send(device, 0, 1, 1);
                if (!sendResult.HasError)
                {
                    if (sendResult.Bytes.Count > 0)
                    {
                        var version = sendResult.Bytes[0];
                        if (version <= 127)
                        {
                            return;
                        }
                    }
                }
				Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            MessageBoxService.ShowError("Не удалось перевести устройство в рабочий режим в заданное время");
        }

        static void EraseDatabase(XDevice device)
        {
            LoadingService.DoStep(device.ShortPresentationAddressAndDriver + " Стирание базы данных");
			for (int i = 0; i < 3; i++)
			{
				var sendResult = SendManager.Send(device, 0, 15, 0, null, true, false, 10000);
				if (!sendResult.HasError)
				{
					return;
				}
				else
				{
					Thread.Sleep(TimeSpan.FromSeconds(1));
				}
			}
			MessageBoxService.ShowError("Не удалось стереть базу данных");
        }

        static void WriteEndDescriptor(CommonDatabase commonDatabase)
        {
            LoadingService.DoStep(commonDatabase.RootDevice.PresentationDriverAndAddress + " Запись завершающего дескриптора");
            var endBytes = BinConfigurationWriter.CreateEndDescriptor((ushort)(commonDatabase.BinaryObjects.Count + 1));
            SendManager.Send(commonDatabase.RootDevice, 5, 17, 0, endBytes, true);
        }

        static void SyncronizeTime(XDevice device)
        {
            LoadingService.DoStep(device.PresentationDriverAndAddress + " Синхронизация времени");
            DeviceBytesHelper.WriteDateTime(device);
        }
    }
}