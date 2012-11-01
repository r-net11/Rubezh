using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
    public class DeviceLogicViewModel : SaveCancelDialogViewModel
    {
        public XDevice Device { get; private set; }

        public DeviceLogicViewModel(XDevice device)
        {
            Title = "Настройка логики устройства " + device.ShortPresentationAddressAndDriver;
            Device = device;

            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand<ClauseViewModel>(OnRemove);
            ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);

			if(device.DeviceLogic.Clauses.Count == 0)
			{
				device.DeviceLogic.Clauses.Add(new XClause());
			}
            Clauses = new ObservableCollection<ClauseViewModel>();
            foreach (var clause in device.DeviceLogic.Clauses)
            {
                var clauseViewModel = new ClauseViewModel(clause);
                Clauses.Add(clauseViewModel);
				JoinOperator = clause.ClauseJounOperationType;
            }
			UpdateJoinOperatorVisibility();
        }

        public DeviceLogicViewModel _deviceDetailsViewModel { get; private set; }
        public ObservableCollection<ClauseViewModel> Clauses { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var clause = new XClause();
            var clauseViewModel = new ClauseViewModel(clause);
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

        protected override bool Save()
        {
            var deviceLogic = new XDeviceLogic();
            foreach (var clauseViewModel in Clauses)
            {
                var clause = new XClause()
                {
                    ClauseConditionType = clauseViewModel.SelectedClauseConditionType,
                    StateType = clauseViewModel.SelectedStateType,
					Zones = clauseViewModel.Zones.ToList(),
                    Devices = clauseViewModel.Devices.ToList(),
					Directions = clauseViewModel.Directions.ToList(),
					ZoneUIDs = clauseViewModel.Zones.Select(x => x.UID).ToList(),
                    DeviceUIDs = clauseViewModel.Devices.Select(x => x.UID).ToList(),
					DirectionUIDs = clauseViewModel.Directions.Select(x => x.UID).ToList(),
                    //ClauseJounOperationType = clauseViewModel.SelectedClauseJounOperationType,
					ClauseJounOperationType = JoinOperator,
                    ClauseOperationType = clauseViewModel.SelectedClauseOperationType
                };
                if (clause.ZoneUIDs.Count > 0 || clause.DeviceUIDs.Count > 0 || clause.DirectionUIDs.Count > 0)
                    deviceLogic.Clauses.Add(clause);
            }

			XManager.ChangeDeviceLogic(Device, deviceLogic);
            return base.Save();
        }
    }
}