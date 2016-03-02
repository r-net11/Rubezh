using Infrastructure.Common.Windows.ViewModels;
using RviCommonClient;

namespace VideoModule.ViewModels
{
	public class DeviceViewModel : BaseViewModel
	{
		public DeviceViewModel(IRviDevice device, IRviChannel channel, int streamNo, bool isEnabled)
		{
			Device = device;
			Channel = channel;
			StreamNo = streamNo;

			DeviceName = device.Name + " (" + "канал " + "\"" + channel.Name + "\"" + ", поток " + StreamNo + ")";
			DeviceIP = device.Ip;
			ChannalNumber = channel.Number;
			ChannalName = channel.Name;
			IsEnabled = isEnabled;
		}

		public int StreamNo { get; private set; }
		public IRviDevice Device { get; private set; }
		public IRviChannel Channel { get; private set; }

		public string DeviceName { get; private set; }
		public string DeviceIP { get; private set; }
		public int ChannalNumber { get; private set; }
		public string ChannalName { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		public bool IsEnabled { get; private set; }
	}
}