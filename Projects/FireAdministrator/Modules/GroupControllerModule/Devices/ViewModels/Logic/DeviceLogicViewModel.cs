using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

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

			if (deviceLogic.ClausesGroup.Clauses.Count == 0)
			{
				deviceLogic.ClausesGroup.Clauses.Add(new XClause());
			}

			ClausesGroup = new ClauseGroupViewModel(device, deviceLogic.ClausesGroup);
			OffClausesGroup = new ClauseGroupViewModel(device, deviceLogic.OffClausesGroup);

			SelectedMROMessageNo = deviceLogic.ZoneLogicMROMessageNo;
			SelectedMROMessageType = deviceLogic.ZoneLogicMROMessageType;

			HasOffClause = hasOffClause;
		}

		public DeviceLogicViewModel _deviceDetailsViewModel { get; private set; }

		public ClauseGroupViewModel ClausesGroup { get; private set; }
		public ClauseGroupViewModel OffClausesGroup { get; private set; }

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
				OnPropertyChanged(() => SelectedMROMessageNo);
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
				if (Device.DeviceLogic.OffClausesGroup.Clauses == null || Device.DeviceLogic.OffClausesGroup.Clauses.Count == 0)
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
			deviceLogic.ClausesGroup = ClausesGroup.GetClauseGroup();
			deviceLogic.OffClausesGroup = OffClausesGroup.GetClauseGroup();
			deviceLogic.ZoneLogicMROMessageNo = SelectedMROMessageNo;
			deviceLogic.ZoneLogicMROMessageType = SelectedMROMessageType;
			return deviceLogic;
		}
	}
}