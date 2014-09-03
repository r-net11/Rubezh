using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class FSShleifViewModel : BaseViewModel
	{
		public FSShleifViewModel()
		{
			KAUDevices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				if (device.Driver.IsKauOrRSR2Kau)
					KAUDevices.Add(device);
			}

			KAUShleifNos = new List<int>();
			KAUShleifNos.Add(1);
			KAUShleifNos.Add(2);
			KAUShleifNos.Add(3);
			KAUShleifNos.Add(4);
			KAUShleifNos.Add(5);
			KAUShleifNos.Add(6);
			KAUShleifNos.Add(7);
			KAUShleifNos.Add(8);
		}

		Device _fsDevice;
		public Device FSDevice
		{
			get { return _fsDevice; }
			set
			{
				_fsDevice = value;
				OnPropertyChanged(() => FSDevice);
			}
		}

		int _fsShleifNo;
		public int FSShleifNo
		{
			get { return _fsShleifNo; }
			set
			{
				_fsShleifNo = value;
				OnPropertyChanged(() => FSShleifNo);
			}
		}

		XDevice _kauDevice;
		public XDevice KAUDevice
		{
			get { return _kauDevice; }
			set
			{
				_kauDevice = value;
				OnPropertyChanged(() => KAUDevice);
			}
		}

		int _kauShleifNo;
		public int KAUShleifNo
		{
			get { return _kauShleifNo; }
			set
			{
				_kauShleifNo = value;
				OnPropertyChanged(() => KAUShleifNo);
			}
		}

		public List<XDevice> KAUDevices { get; private set; }
		public List<int> KAUShleifNos { get; private set; }
	}
}