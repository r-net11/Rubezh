using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
using FiresecAPI.Models;
using FiresecClient;
using FiresecAPI;
using XFiresecAPI;

namespace SKDModule.ViewModels
{
	public class LayoutPartPropertyVerificationPageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartVerificationViewModel _layoutPartVerificationViewModel;

		public LayoutPartPropertyVerificationPageViewModel(LayoutPartVerificationViewModel layoutPartFilterViewModel)
		{
			_layoutPartVerificationViewModel = layoutPartFilterViewModel;

			Devices = new ObservableCollection<SKDDevice>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.DriverType == SKDDriverType.Reader)
				{
					Devices.Add(device);
				}
			}
		}

		public override string Header
		{
			get { return "Настройка верификации"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartSKDVerificationProperties)_layoutPartVerificationViewModel.Properties;

			SelectedDevice = Devices.FirstOrDefault(x => x.UID == properties.ReaderDeviceUID);
			ShowEmployeeCardID = properties.ShowEmployeeCardID;
			ShowEmployeeName = properties.ShowEmployeeName;
			ShowEmployeePassport = properties.ShowEmployeePassport;
			ShowEmployeeTime = properties.ShowEmployeeTime;
			ShowEmployeeNo = properties.ShowEmployeeNo;
			ShowEmployeePosition = properties.ShowEmployeePosition;
			ShowEmployeeShedule = properties.ShowEmployeeShedule;
			ShowEmployeeDepartment = properties.ShowEmployeeDepartment;
			ShowGuestCardID = properties.ShowGuestCardID;
			ShowGuestName = properties.ShowGuestName;
			ShowGuestWhere = properties.ShowGuestWhere;
			ShowGuestConvoy = properties.ShowGuestConvoy;
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

		public override bool CanSave()
		{
			return SelectedDevice != null;
		}
		public override bool Save()
		{
			var properties = (LayoutPartSKDVerificationProperties)_layoutPartVerificationViewModel.Properties;

			properties.ReaderDeviceUID = SelectedDevice.UID;
			properties.ShowEmployeeCardID = ShowEmployeeCardID;
			properties.ShowEmployeeName = ShowEmployeeName;
			properties.ShowEmployeePassport = ShowEmployeePassport;
			properties.ShowEmployeeTime = ShowEmployeeTime;
			properties.ShowEmployeeNo = ShowEmployeeNo;
			properties.ShowEmployeePosition = ShowEmployeePosition;
			properties.ShowEmployeeShedule = ShowEmployeeShedule;
			properties.ShowEmployeeDepartment = ShowEmployeeDepartment;
			properties.ShowGuestCardID = ShowGuestCardID;
			properties.ShowGuestName = ShowGuestName;
			properties.ShowGuestWhere = ShowGuestWhere;
			properties.ShowGuestConvoy = ShowGuestConvoy;

			_layoutPartVerificationViewModel.UpdateLayoutPart();
			return true;
		}
	}
}