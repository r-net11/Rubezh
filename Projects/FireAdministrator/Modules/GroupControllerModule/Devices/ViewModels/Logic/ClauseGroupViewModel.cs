using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ClauseGroupViewModel : BaseViewModel
	{
		ClauseGroupViewModel Parent;
		GKBase GKBase;

		public ClauseGroupViewModel(GKBase gkBase, GKClauseGroup clauseGroup)
		{
			AddCommand = new RelayCommand(OnAdd);
			AddGroupCommand = new RelayCommand(OnAddGroup);
			RemoveCommand = new RelayCommand<ClauseViewModel>(OnRemove);
			RemoveGroupCommand2 = new RelayCommand(OnRemoveGroup2, CanRemoveGroup2);
			RemoveGroupCommand = new RelayCommand<ClauseGroupViewModel>(OnRemoveGroup);
			ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);

			GKBase = gkBase;
			Clauses = new ObservableCollection<ClauseViewModel>();
			foreach (var clause in clauseGroup.Clauses)
			{
				var clauseViewModel = new ClauseViewModel(gkBase, clause);
				Clauses.Add(clauseViewModel);
			}

			ClauseGroups = new ObservableCollection<ClauseGroupViewModel>();
			foreach (var clause in clauseGroup.ClauseGroups)
			{
				var clauseGroupViewModel = new ClauseGroupViewModel(gkBase, clause);
				clauseGroupViewModel.Parent = this;
				ClauseGroups.Add(clauseGroupViewModel);
			}

			JoinOperator = clauseGroup.ClauseJounOperationType;
		}

		public ObservableCollection<ClauseViewModel> Clauses { get; private set; }
		public ObservableCollection<ClauseGroupViewModel> ClauseGroups { get; private set; }

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var clauseViewModel = new ClauseViewModel(GKBase, new GKClause());
			Clauses.Add(clauseViewModel);
		}

		public RelayCommand AddGroupCommand { get; private set; }
		void OnAddGroup()
		{
			var clauseGroupViewModel = new ClauseGroupViewModel(GKBase, new GKClauseGroup());
			clauseGroupViewModel.Parent = this;
			ClauseGroups.Add(clauseGroupViewModel);
		}

		public RelayCommand<ClauseViewModel> RemoveCommand { get; private set; }
		void OnRemove(ClauseViewModel clauseViewModel)
		{
			Clauses.Remove(clauseViewModel);
		}

		public RelayCommand RemoveGroupCommand2 { get; private set; }
		void OnRemoveGroup2()
		{
			Parent.ClauseGroups.Remove(this);
		}
		bool CanRemoveGroup2()
		{
			return Parent != null;
		}

		public RelayCommand<ClauseGroupViewModel> RemoveGroupCommand { get; private set; }
		void OnRemoveGroup(ClauseGroupViewModel clauseGroupViewModel)
		{
			ClauseGroups.Remove(clauseGroupViewModel);
		}

		public RelayCommand ChangeJoinOperatorCommand { get; private set; }
		void OnChangeJoinOperator()
		{
			if (JoinOperator == ClauseJounOperationType.And)
				JoinOperator = ClauseJounOperationType.Or;
			else
				JoinOperator = ClauseJounOperationType.And;
		}

		ClauseJounOperationType _joinOperator;
		public ClauseJounOperationType JoinOperator
		{
			get { return _joinOperator; }
			set
			{
				_joinOperator = value;
				OnPropertyChanged(() => JoinOperator);
			}
		}

		public GKClauseGroup GetClauseGroup()
		{
			var clauseGroup = new GKClauseGroup();
			clauseGroup.ClauseJounOperationType = JoinOperator;
			foreach (var clauseGroupViewModel in ClauseGroups)
			{
				clauseGroup.ClauseGroups.Add(clauseGroupViewModel.GetClauseGroup());
			}

			foreach (var clauseViewModel in Clauses)
			{
				var clause = new GKClause()
				{
					ClauseConditionType = clauseViewModel.SelectedClauseConditionType,
					StateType = clauseViewModel.SelectedStateType.StateBit,
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

					case ClauseOperationType.AllGuardZones:
					case ClauseOperationType.AnyGuardZone:
						clause.GuardZones = clauseViewModel.GuardZones.ToList();
						clause.GuardZoneUIDs = clauseViewModel.GuardZones.Select(x => x.UID).ToList();
						break;

					case ClauseOperationType.AllDirections:
					case ClauseOperationType.AnyDirection:
						clause.Directions = clauseViewModel.Directions.ToList();
						clause.DirectionUIDs = clauseViewModel.Directions.Select(x => x.UID).ToList();
						break;

					case ClauseOperationType.AllMPTs:
					case ClauseOperationType.AnyMPT:
						clause.MPTs = clauseViewModel.MPTs.ToList();
						clause.MPTUIDs = clauseViewModel.MPTs.Select(x => x.UID).ToList();
						break;

					case ClauseOperationType.AllDelays:
					case ClauseOperationType.AnyDelay:
						clause.Delays = clauseViewModel.Delays.ToList();
						clause.DelayUIDs = clauseViewModel.Delays.Select(x => x.UID).ToList();
						break;

					case ClauseOperationType.AllDoors:
					case ClauseOperationType.AnyDoor:
						clause.Doors = clauseViewModel.Doors.ToList();
						clause.DoorUIDs = clauseViewModel.Doors.Select(x => x.UID).ToList();
						break;

					case ClauseOperationType.AnyPumpStation:
					case ClauseOperationType.AllPumpStations:
						clause.PumpStations = clauseViewModel.PumpStations.ToList();
						clause.PumpStationsUIDs = clauseViewModel.PumpStations.Select(x => x.UID).ToList();
						break;
				}
				if (clause.HasObjects())
					clauseGroup.Clauses.Add(clause);
			}
			return clauseGroup;
		}
	}
}