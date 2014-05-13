using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ClauseGroupViewModel : BaseViewModel
	{
		XDevice Device;

		public ClauseGroupViewModel(XDevice device, XClauseGroup clauseGroup)
		{
			Device = device;
			AddCommand = new RelayCommand(OnAdd);
			AddGroupCommand = new RelayCommand(OnAddGroup);
			RemoveCommand = new RelayCommand<ClauseViewModel>(OnRemove);
			RemoveGroupCommand = new RelayCommand<ClauseGroupViewModel>(OnRemoveGroup);
			ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);

			Clauses = new ObservableCollection<ClauseViewModel>();
			foreach (var clause in clauseGroup.Clauses)
			{
				var clauseViewModel = new ClauseViewModel(device, clause);
				Clauses.Add(clauseViewModel);
			}

			ClauseGroups = new ObservableCollection<ClauseGroupViewModel>();
			foreach (var clause in clauseGroup.ClauseGroups)
			{
				var clauseGroupViewModel = new ClauseGroupViewModel(device, clause);
				ClauseGroups.Add(clauseGroupViewModel);
			}

			JoinOperator = clauseGroup.ClauseJounOperationType;

			UpdateJoinOperatorVisibility();
		}

		public ObservableCollection<ClauseViewModel> Clauses { get; private set; }
		public ObservableCollection<ClauseGroupViewModel> ClauseGroups { get; private set; }

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var clauseViewModel = new ClauseViewModel(Device, new XClause());
			Clauses.Add(clauseViewModel);
			UpdateJoinOperatorVisibility();
		}

		public RelayCommand AddGroupCommand { get; private set; }
		void OnAddGroup()
		{
			var clauseGroupViewModel = new ClauseGroupViewModel(Device, new XClauseGroup());
			ClauseGroups.Add(clauseGroupViewModel);
			UpdateJoinOperatorVisibility();
		}

		public RelayCommand<ClauseViewModel> RemoveCommand { get; private set; }
		void OnRemove(ClauseViewModel clauseViewModel)
		{
			Clauses.Remove(clauseViewModel);
			UpdateJoinOperatorVisibility();
		}

		public RelayCommand<ClauseGroupViewModel> RemoveGroupCommand { get; private set; }
		void OnRemoveGroup(ClauseGroupViewModel clauseGroupViewModel)
		{
			ClauseGroups.Remove(clauseGroupViewModel);
			UpdateJoinOperatorVisibility();
		}

		public void UpdateJoinOperatorVisibility()
		{
			ShowJoinOperator = Clauses.Count > 1;
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
				OnPropertyChanged("JoinOperator");
			}
		}

		bool _showJoinOperator;
		public bool ShowJoinOperator
		{
			get { return _showJoinOperator; }
			set
			{
				_showJoinOperator = value;
				OnPropertyChanged("ShowJoinOperator");
			}
		}

		public XClauseGroup GetClauseGroup()
		{
			var clauseGroup = new XClauseGroup();
			clauseGroup.ClauseJounOperationType = JoinOperator;
			foreach (var clauseGroupViewModel in ClauseGroups)
			{
				clauseGroup.ClauseGroups.Add(clauseGroupViewModel.GetClauseGroup());
			}

			foreach (var clauseViewModel in Clauses)
			{
				var clause = new XClause()
				{
					ClauseConditionType = clauseViewModel.SelectedClauseConditionType,
					StateType = clauseViewModel.SelectedStateType,
					ClauseOperationType = clauseViewModel.SelectedClauseOperationType
				};
				switch (clause.ClauseOperationType)
				{
					case ClauseOperationType.AllDevices:
					case ClauseOperationType.AnyDevice:
						clause.Devices = clauseViewModel.Devices.ToList();
						clause.DeviceUIDs = clauseViewModel.Devices.Select(x => x.BaseUID).ToList();
						break;

					case ClauseOperationType.AllZones:
					case ClauseOperationType.AnyZone:
						clause.Zones = clauseViewModel.Zones.ToList();
						clause.ZoneUIDs = clauseViewModel.Zones.Select(x => x.BaseUID).ToList();
						break;

					case ClauseOperationType.AllDirections:
					case ClauseOperationType.AnyDirection:
						clause.Directions = clauseViewModel.Directions.ToList();
						clause.DirectionUIDs = clauseViewModel.Directions.Select(x => x.BaseUID).ToList();
						break;

					case ClauseOperationType.AllMPTs:
					case ClauseOperationType.AnyMPT:
						clause.MPTs = clauseViewModel.MPTs.ToList();
						clause.MPTUIDs = clauseViewModel.MPTs.Select(x => x.BaseUID).ToList();
						break;

					case ClauseOperationType.AllDelays:
					case ClauseOperationType.AnyDelay:
						clause.Delays = clauseViewModel.Delays.ToList();
						clause.DelayUIDs = clauseViewModel.Delays.Select(x => x.BaseUID).ToList();
						break;
				}
				if (clause.HasObjects())
					clauseGroup.Clauses.Add(clause);
			}
			return clauseGroup;
		}
	}
}