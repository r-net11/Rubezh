using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Common.GK;
using System.Diagnostics;
using Infrastructure.Common.Windows;
using System.Collections;

namespace GKModule
{
	public static class BinConfigurationReader
	{
		public static void ReadConfiguration(XDevice device)
		{
			BinConfigurationWriter.GoToTechnologicalRegime(device);
			var descriptorAddersses = GetDescriptorAddresses(device);
			foreach (var descriptorAdderss in descriptorAddersses)
			{
				GetDescriptorInfo(device, descriptorAdderss);
			}
			BinConfigurationWriter.GoToWorkingRegime(device);
		}

		static void GetDescriptorInfo(XDevice device, int descriptorAdderss)
		{
			var descriptorAdderssesBytes = new List<byte>(BitConverter.GetBytes(descriptorAdderss));

			var data = new List<byte>(descriptorAdderssesBytes);
			var sendResult = SendManager.Send(device, 4, 31, 256, data);
			var bytes = sendResult.Bytes;
			if (bytes.Count != 256)
			{
				MessageBoxService.ShowError("bytes.Count != 256");
				return;
			}
			var deviceType = BytesHelper.SubstructShort(bytes, 0);
			var address = BytesHelper.SubstructShort(bytes, 2);
			var parametersOffset = BytesHelper.SubstructShort(bytes, 4);
			var inputDependensesCount = BytesHelper.SubstructShort(bytes, 6);
			for (int i = 0; i < inputDependensesCount; i++)
			{
				var inputDependensyNo = BytesHelper.SubstructShort(bytes, 8 + i * 2);
			}
			while (true)
			{
				var formula = BytesHelper.SubstructShort(bytes, 8 + inputDependensesCount * 2);
				if (formula == 0x1F)
				{
					break;
				}
			}
            Trace.WriteLine(descriptorAdderss + " " + BytesHelper.BytesToString(descriptorAdderssesBytes) +
                deviceType.ToString() + " " + address.ToString() + " " + parametersOffset.ToString() + " " + inputDependensesCount.ToString() + " "
                );
		}

		static List<int> GetDescriptorAddresses(XDevice device)
		{
			var descriptorAddersses = new List<int>();
			int startaddress = 0x078000;

			while (true)
			{
				byte[] startAddressBytes = BitConverter.GetBytes(startaddress);
				startaddress += 256;

				var data = new List<byte>(startAddressBytes);
				var sendResult = SendManager.Send(device, 4, 31, 256, data);
				if (sendResult.Bytes.Count != 256)
				{
					MessageBoxService.ShowError("bytes.Count != 256");
					return descriptorAddersses;
				}
				for (int i = 0; i < 256 / 4; i++)
				{
					var descriptorAdderss = BytesHelper.SubstructInt(sendResult.Bytes, i * 4);
					if (descriptorAdderss == -1)
					{
						return descriptorAddersses;
					}
                    descriptorAddersses.Add(descriptorAdderss);
				}
			}
		}
	}
}