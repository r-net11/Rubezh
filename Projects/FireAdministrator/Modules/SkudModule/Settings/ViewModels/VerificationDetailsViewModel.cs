using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;
using XFiresecAPI;

namespace SKDModule.ViewModels
{
	public class VerificationDetailsViewModel : SaveCancelDialogViewModel
	{
		public VerificationDetailsViewModel()
		{
			Title = "Настройка поста верификации";
			Devices = new ObservableCollection<SKDDevice>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.DriverType == SKDDriverType.Reader)
				{
					Devices.Add(device);
				}
			}
			SelectedDevice = Devices.FirstOrDefault(x => x.UID == SKDManager.SKDConfiguration.SKDSystemConfiguration.ReaderDeviceUID);

			ShowEmployeeCardID = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeCardID;
			ShowEmployeeName = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeName;
			ShowEmployeePassport = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeePassport;
			ShowEmployeeTime = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeTime;
			ShowEmployeeNo = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeNo;
			ShowEmployeePosition = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeePosition;
			ShowEmployeeShedule = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeShedule;
			ShowEmployeeDepartment = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeDepartment;
			ShowGuestCardID = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowGuestCardID;
			ShowGuestName = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowGuestName;
			ShowGuestWhere = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowGuestWhere;
			ShowGuestConvoy = SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowGuestConvoy;
		}

		public ObservableCollection<SKDDevice> Devices { get; private set; }

		SKDDevice _selectedDevice;
		public SKDDevice SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		bool _showEmployeeCardID;
		public bool ShowEmployeeCardID
		{
			get { return _showEmployeeCardID; }
			set
			{
				_showEmployeeCardID = value;
				OnPropertyChanged("ShowEmployeeCardID");
			}
		}

		bool _showEmployeeName;
		public bool ShowEmployeeName
		{
			get { return _showEmployeeName; }
			set
			{
				_showEmployeeName = value;
				OnPropertyChanged("ShowEmployeeName");
			}
		}

		bool _showEmployeePassport;
		public bool ShowEmployeePassport
		{
			get { return _showEmployeePassport; }
			set
			{
				_showEmployeePassport = value;
				OnPropertyChanged("ShowEmployeePassport");
			}
		}

		bool _showEmployeeTime;
		public bool ShowEmployeeTime
		{
			get { return _showEmployeeTime; }
			set
			{
				_showEmployeeTime = value;
				OnPropertyChanged("ShowEmployeeTime");
			}
		}

		bool _showEmployeeNo;
		public bool ShowEmployeeNo
		{
			get { return _showEmployeeNo; }
			set
			{
				_showEmployeeNo = value;
				OnPropertyChanged("ShowEmployeeNo");
			}
		}

		bool _showEmployeePosition;
		public bool ShowEmployeePosition
		{
			get { return _showEmployeePosition; }
			set
			{
				_showEmployeePosition = value;
				OnPropertyChanged("ShowEmployeePosition");
			}
		}

		bool _showEmployeeShedule;
		public bool ShowEmployeeShedule
		{
			get { return _showEmployeeShedule; }
			set
			{
				_showEmployeeShedule = value;
				OnPropertyChanged("ShowEmployeeShedule");
			}
		}

		bool _showEmployeeDepartment;
		public bool ShowEmployeeDepartment
		{
			get { return _showEmployeeDepartment; }
			set
			{
				_showEmployeeDepartment = value;
				OnPropertyChanged("ShowEmployeeDepartment");
			}
		}

		bool _showGuestCardID;
		public bool ShowGuestCardID
		{
			get { return _showGuestCardID; }
			set
			{
				_showGuestCardID = value;
				OnPropertyChanged("ShowGuestCardID");
			}
		}

		bool _showGuestName;
		public bool ShowGuestName
		{
			get { return _showGuestName; }
			set
			{
				_showGuestName = value;
				OnPropertyChanged("ShowGuestName");
			}
		}

		bool _showGuestWhere;
		public bool ShowGuestWhere
		{
			get { return _showGuestWhere; }
			set
			{
				_showGuestWhere = value;
				OnPropertyChanged("ShowGuestWhere");
			}
		}

		bool _showGuestConvoy;
		public bool ShowGuestConvoy
		{
			get { return _showGuestConvoy; }
			set
			{
				_showGuestConvoy = value;
				OnPropertyChanged("ShowGuestConvoy");
			}
		}

		protected override bool Save()
		{
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ReaderDeviceUID = SelectedDevice.UID;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeCardID = ShowEmployeeCardID;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeName = ShowEmployeeName;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeePassport = ShowEmployeePassport;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeTime = ShowEmployeeTime;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeNo = ShowEmployeeNo;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeePosition = ShowEmployeePosition;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeShedule = ShowEmployeeShedule;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowEmployeeDepartment = ShowEmployeeDepartment;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowGuestCardID = ShowGuestCardID;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowGuestName = ShowGuestName;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowGuestWhere = ShowGuestWhere;
			SKDManager.SKDConfiguration.SKDSystemConfiguration.ShowGuestConvoy = ShowGuestConvoy;
			return true;
		}
	}
}