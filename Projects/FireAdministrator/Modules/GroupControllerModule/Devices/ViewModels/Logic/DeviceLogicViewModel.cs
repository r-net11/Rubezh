using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DeviceLogicViewModel : SaveCancelDialogViewModel
	{
		public XDevice Device { get; private set; }

		public DeviceLogicViewModel(XDevice device)
		{
			Title = "Настройка логики устройства " + device.PresentationDriverAndAddress;
			Device = device;

			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ClauseViewModel>(OnRemove);
			ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);

			if (device.DeviceLogic.Clauses.Count == 0)
			{
				device.DeviceLogic.Clauses.Add(new XClause());
			}
			Clauses = new ObservableCollection<ClauseViewModel>();
			foreach (var clause in device.DeviceLogic.Clauses)
			{
				var clauseViewModel = new ClauseViewModel(clause, device);
				Clauses.Add(clauseViewModel);
				JoinOperator = clause.ClauseJounOperationType;
			}
			UpdateJoinOperatorVisibility();

			SelectedMROMessageNo = device.DeviceLogic.ZoneLogicMROMessageNo;
			SelectedMROMessageType = device.DeviceLogic.ZoneLogicMROMessageType;
		}

		public DeviceLogicViewModel _deviceDetailsViewModel { get; private set; }
		public ObservableCollection<ClauseViewModel> Clauses { get; private set; }

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var clause = new XClause();
			var clauseViewModel = new ClauseViewModel(clause, Device);
			Clauses.Add(clauseViewModel);
			UpdateJoinOperatorVisibility();
		}

		public void UpdateJoinOperatorVisibility()
		{
			foreach (var clause in Clauses)
				clause.ShowJoinOperator = false;

			if (Clauses.Count > 1)
			{
				foreach (var clause in Clauses)
					clause.ShowJoinOperator = true;

				Clauses.Last().ShowJoinOperator = false;
			}
		}

		ClauseJounOperationType _joinOperator;
		public ClauseJounOperationType JoinOperator
		{
			get { return _joinOperator; }
			set
			{
				_joinOperator = value;
				OnPropertyChanged("JoinOperator");
			}
		}

		public RelayCommand ChangeJoinOperatorCommand { get; private set; }
		void OnChangeJoinOperator()
		{
			if (JoinOperator == ClauseJounOperationType.And)
				JoinOperator = ClauseJounOperationType.Or;
			else
				JoinOperator = ClauseJounOperationType.And;
		}

		public RelayCommand<ClauseViewModel> RemoveCommand { get; private set; }
		void OnRemove(ClauseViewModel clauseViewModel)
		{
			Clauses.Remove(clauseViewModel);
			UpdateJoinOperatorVisibility();
		}

		#region IsMRO_2M
		public bool IsSonar
		{
			get { return Device.Driver.DriverType == XDriverType.MRO_2; }
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

		protected override bool Save()
		{
			var deviceLogic = new XDeviceLogic();
			foreach (var clauseViewModel in Clauses)
			{
				var clause = new XClause()
				{
					ClauseConditionType = clauseViewModel.SelectedClauseConditionType,
					StateType = clauseViewModel.SelectedStateType,
					ClauseJounOperationType = JoinOperator,
					ClauseOperationType = clauseViewModel.SelectedClauseOperationType
				};
				switch (clause.ClauseOperationType)
				{
					case ClauseOperationType.AllDevices:
					case ClauseOperationType.AnyDevice:
						clause.Devices = clauseViewModel.Devices.ToList();
						clause.DeviceUIDs = clauseViewModel.Devices.Select(x => x.UID).ToList();
						break;

					case ClauseOperationType.AllZones:
					case ClauseOperationType.AnyZone:
						clause.Zones = clauseViewModel.Zones.ToList();
						clause.ZoneUIDs = clauseViewModel.Zones.Select(x => x.UID).ToList();
						break;

					case ClauseOperationType.AllDirections:
					case ClauseOperationType.AnyDirection:
						clause.Directions = clauseViewModel.Directions.ToList();
						clause.DirectionUIDs = clauseViewModel.Directions.Select(x => x.UID).ToList();
						break;
				}
				if (clause.ZoneUIDs.Count > 0 || clause.DeviceUIDs.Count > 0 || clause.DirectionUIDs.Count > 0)
					deviceLogic.Clauses.Add(clause);
				deviceLogic.ZoneLogicMROMessageNo = SelectedMROMessageNo;
				deviceLogic.ZoneLogicMROMessageType = SelectedMROMessageType;
			}

			XManager.ChangeDeviceLogic(Device, deviceLogic);
			return base.Save();
		}
	}
}