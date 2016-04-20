using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class PDUDeviceViewModel : BaseViewModel
	{
		public Device Device { get; private set; }

		public void Initialize(Device device)
		{
			Device = device;
		}

		public void Initialize(PDUGroupDevice pDUGroupDevice)
		{
			Device = FiresecManager.Devices.FirstOrDefault(x => x.UID == pDUGroupDevice.DeviceUID);

			IsInversion = pDUGroupDevice.IsInversion;
			OnDelay = pDUGroupDevice.OnDelay;
			OffDelay = pDUGroupDevice.OffDelay;
		}

		bool _isInversion;
		public bool IsInversion
		{
			get { return _isInversion; }
			set
			{
				_isInversion = value;
				OnPropertyChanged(() => IsInversion);
			}
		}

		int _onDelay;
		public int OnDelay
		{
			get { return _onDelay; }
			set
			{
				_onDelay = value;
				OnPropertyChanged(() => OnDelay);
			}
		}

		int _offDelay;
		public int OffDelay
		{
			get { return _offDelay; }
			set
			{
				_offDelay = value;
				OnPropertyChanged(() => OffDelay);
			}
		}
	}
}