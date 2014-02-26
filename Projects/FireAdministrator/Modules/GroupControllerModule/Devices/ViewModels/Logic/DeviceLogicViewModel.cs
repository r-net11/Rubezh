using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DeviceLogicViewModel : SaveCancelDialogViewModel
	{
		public XDevice Device { get; private set; }

		public DeviceLogicViewModel(XDevice device, XDeviceLogic deviceLogic, bool hasOffClause = true)
		{
			if (device.DriverType == XDriverType.System)
				Title = "Настройка логики";
			else
				Title = "Настройка логики устройства " + device.PresentationName;
			Device = device;

			if (deviceLogic.Clauses.Count == 0)
			{
				deviceLogic.Clauses.Add(new XClause());
			}

			OnLogicViewModel = new LogicViewModel(device, deviceLogic.Clauses);
			OffLogicViewModel = new LogicViewModel(device, deviceLogic.OffClauses);

			SelectedMROMessageNo = deviceLogic.ZoneLogicMROMessageNo;
			SelectedMROMessageType = deviceLogic.ZoneLogicMROMessageType;

			HasOffClause = hasOffClause;
		}

		public DeviceLogicViewModel _deviceDetailsViewModel { get; private set; }

		public LogicViewModel OnLogicViewModel { get; private set; }
		public LogicViewModel OffLogicViewModel { get; private set; }

		#region IsMRO_2M
		public bool IsMRO_2M
		{
			get { return Device.DriverType == XDriverType.MRO_2; }
		}

		public List<ZoneLogicMROMessageNo> AvailableMROMessageNos
		{
			get { return Enum.GetValues(typeof(ZoneLogicMROMessageNo)).Cast<ZoneLogicMROMessageNo>().ToList(); }
		}

		ZoneLogicMROMessageNo _selectedMROMessageNo;
		public ZoneLogicMROMessageNo SelectedMROMessageNo
		{
			get { return _selectedMROMessageNo; }
			set
			{
				_selectedMROMessageNo = value;
				OnPropertyChanged("SelectedMROMessageNo");
			}
		}

		public List<ZoneLogicMROMessageType> AvailableMROMessageTypes
		{
			get { return Enum.GetValues(typeof(ZoneLogicMROMessageType)).Cast<ZoneLogicMROMessageType>().ToList(); }
		}

		ZoneLogicMROMessageType _selectedMROMessageType;
		public ZoneLogicMROMessageType SelectedMROMessageType
		{
			get { return _selectedMROMessageType; }
			set
			{
				_selectedMROMessageType = value;
				OnPropertyChanged("SelectedMROMessageType");
			}
		}
		#endregion

		public bool HasOffClause { get; private set; }

		public string OffClauseName
		{
			get
			{
				if (Device.DeviceLogic.OffClauses == null || Device.DeviceLogic.OffClauses.Count == 0)
					return "Условие выключения противоположно условию включения";
				else
					return "Условие выключения";
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}

		public XDeviceLogic GetModel()
		{
			var deviceLogic = new XDeviceLogic();
			deviceLogic.Clauses = OnLogicViewModel.GetClauses();
			deviceLogic.OffClauses = OffLogicViewModel.GetClauses();
			deviceLogic.ZoneLogicMROMessageNo = SelectedMROMessageNo;
			deviceLogic.ZoneLogicMROMessageType = SelectedMROMessageType;
			return deviceLogic;
		}
	}
}