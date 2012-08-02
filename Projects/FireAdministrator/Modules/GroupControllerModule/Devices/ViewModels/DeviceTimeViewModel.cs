using System;
using System.Collections.Generic;
using Common.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceTimeViewModel : DialogViewModel
	{
		XDevice Device;

		public DeviceTimeViewModel(XDevice device)
		{
			Title = "Время устройства";
			ReadCommand = new RelayCommand(OnRead);
			WriteCommand = new RelayCommand(OnWrite);
			Device = device;
			OnRead();
		}

		string _time;
		public string Time
		{
			get { return _time; }
			set
			{
				_time = value;
				OnPropertyChanged("Time");
			}
		}

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			var bytes = SendManager.Send(Device, 0, 4, 6);
			var Day = bytes[0];
			var Month = bytes[1];
			var Year = bytes[2];
			var Hour = bytes[3];
			var Minute = bytes[4];
			var Second = bytes[5];
			Time = Day.ToString() + "/" + Month.ToString() + "/" + Year.ToString() + " " + Hour.ToString() + ":" + Minute.ToString() + ":" + Second.ToString();
		}

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{
			var dateTime = DateTime.Now;
			var bytes = new List<byte>();
			bytes.Add((byte)dateTime.Day);
			bytes.Add((byte)dateTime.Month);
			bytes.Add((byte)(dateTime.Year - 2000));
			bytes.Add((byte)dateTime.Hour);
			bytes.Add((byte)dateTime.Minute);
			bytes.Add((byte)dateTime.Second);
			SendManager.Send(Device, 6, 5, 0, bytes);
		}
	}
}