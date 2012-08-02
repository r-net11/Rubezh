using System.Collections.Generic;
using System.Text;
using Common.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceInfoViewModel : DialogViewModel
	{
		XDevice Device;

		public DeviceInfoViewModel(XDevice device)
		{
			Title = "Информация об устройстве";
			ReadCommand = new RelayCommand(OnRead);
			WriteCommand = new RelayCommand(OnWrite);
			Device = device;
			OnRead();
		}

		string _version;
		public string Version
		{
			get { return _version; }
			set
			{
				_version = value;
				OnPropertyChanged("Version");
			}
		}

		string _info;
		public string Info
		{
			get { return _info; }
			set
			{
				_info = value;
				OnPropertyChanged("Info");
			}
		}

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			var bytes = SendManager.Send(Device, 0, 1, 1);
			Version = BytesToString(bytes);

			bytes = SendManager.Send(Device, 0, 2, 8);
			Info = BytesToString(bytes);
		}

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{
			var bytes = SendManager.Send(Device, 8, 3, 0);
		}

		List<byte> StringToBytes(string stringBytes)
		{
			var bytes = new List<byte>();
			var strings = stringBytes.Split(' ');
			foreach (var str in strings)
			{
				byte b = byte.Parse(str);
				bytes.Add(b);
			}
			return bytes;
		}

		string BytesToString(List<byte> bytes)
		{
			var stringBuilder = new StringBuilder();
			foreach (var b in bytes)
			{
				stringBuilder.Append(b + " ");
			}
			return stringBuilder.ToString();
		}
	}
}