using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ZoneLogicViewModel : SaveCancelDialogViewModel
	{
		Device _device;

		public ZoneLogicViewModel(Device device)
		{
			Title = "Настройка логики исполнительного устройства по состоянию зон. " + device.PresentationAddressAndName;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<ClauseViewModel>(OnRemove);
			ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);

			Initialize(device);
		}

		void Initialize(Device device)
		{
			_device = device;
			Clauses = new ObservableCollection<ClauseViewModel>();
			if (device.ZoneLogic != null)
			{
				foreach (var clause in device.ZoneLogic.Clauses)
				{
					var clauseViewModel = new ClauseViewModel(this, _device, clause);
					Clauses.Add(clauseViewModel);
				}
			}
			else
			{
				device.ZoneLogic = new ZoneLogic();
			}

			if (device.ZoneLogic.Clauses.Count == 0)
			{
				var clauseViewModel = new ClauseViewModel(this, _device, new Clause());
				Clauses.Add(clauseViewModel);
			}

			JoinOperator = device.ZoneLogic.JoinOperator;
			UpdateJoinOperatorVisibility();
		}

		ZoneLogicJoinOperator _joinOperator;
		public ZoneLogicJoinOperator JoinOperator
		{
			get { return _joinOperator; }
			set
			{
				_joinOperator = value;
				OnPropertyChanged(() => JoinOperator);
			}
		}

		public RelayCommand ChangeJoinOperatorCommand { get; private set; }
		void OnChangeJoinOperator()
		{
			if (JoinOperator == ZoneLogicJoinOperator.And)
				JoinOperator = ZoneLogicJoinOperator.Or;
			else
				JoinOperator = ZoneLogicJoinOperator.And;
		}

		public bool IsRm
		{
			get { return _device.Driver.DriverType == DriverType.RM_1; }
		}

		public bool IsRmWithTablo
		{
			get { return _device.IsRmAlarmDevice; }
			set
			{
				_device.IsRmAlarmDevice = value;
			}
		}

		public ObservableCollection<ClauseViewModel> Clauses { get; private set; }

		bool _isBlocked = false;
		public void OnCurrentClauseStateChanged(ZoneLogicState zoneLogicState)
		{
			_isBlocked = ((zoneLogicState == ZoneLogicState.Lamp) || (zoneLogicState == ZoneLogicState.PCN));
			if (_isBlocked)
			{
				var selectedClause = Clauses.FirstOrDefault(x => x.SelectedState == zoneLogicState);
				Clauses.Clear();
				Clauses.Add(selectedClause);
			}
		}

		public bool CanAdd()
		{
			return _isBlocked != true;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var clause = new Clause()
			{
				Operation = ZoneLogicOperation.All,
				State = ZoneLogicState.Fire
			};
			var clauseViewModel = new ClauseViewModel(this, _device, clause);
			Clauses.Add(clauseViewModel);
			UpdateJoinOperatorVisibility();
		}

		public RelayCommand<ClauseViewModel> RemoveCommand { get; private set; }
		void OnRemove(ClauseViewModel clauseViewModel)
		{
			Clauses.Remove(clauseViewModel);
			UpdateJoinOperatorVisibility();
		}

		void UpdateJoinOperatorVisibility()
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

		protected override bool Save()
		{
			var zoneLogic = new ZoneLogic();
			zoneLogic.JoinOperator = JoinOperator;

			foreach (var clauseViewModel in Clauses)
			{
				switch (clauseViewModel.SelectedState)
				{
					case ZoneLogicState.AM1TOn:
					case ZoneLogicState.ShuzOn:
						if (clauseViewModel.SelectedDevices.Count > 0)
						{
							var clause = new Clause()
							{
								State = clauseViewModel.SelectedState,
								Operation = clauseViewModel.SelectedOperation,
							};
							foreach (var device in clauseViewModel.SelectedDevices)
							{
								clause.DeviceUIDs.Add(device.UID);
								clause.Devices.Add(device);
							}

							zoneLogic.Clauses.Add(clause);
						}
						break;

					case ZoneLogicState.Failure:
					case ZoneLogicState.DoubleFire:
						{
							var clause = new Clause()
							{
								State = clauseViewModel.SelectedState,
							};
							zoneLogic.Clauses.Add(clause);
						}
						break;

					default:
						if (clauseViewModel.Zones.Count > 0)
						{
							var clause = new Clause()
							{
								State = clauseViewModel.SelectedState,
								Operation = clauseViewModel.SelectedOperation,
								ZoneUIDs = clauseViewModel.Zones,
								ZoneLogicMROMessageNo = clauseViewModel.SelectedMROMessageNo,
								ZoneLogicMROMessageType = clauseViewModel.SelectedMROMessageType
							};
							zoneLogic.Clauses.Add(clause);
						}
						break;
				}
			}
			FiresecManager.FiresecConfiguration.SetDeviceZoneLogic(_device, zoneLogic);
			return base.Save();
		}
	}
}