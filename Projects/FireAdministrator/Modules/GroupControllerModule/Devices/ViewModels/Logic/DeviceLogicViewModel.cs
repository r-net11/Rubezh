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
            Title = "Настройка логики устройства";
            Device = device;

            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand<ClauseViewModel>(OnRemove);
            ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);

            Clauses = new ObservableCollection<ClauseViewModel>();
            foreach (var clause in device.DeviceLogic.Clauses)
            {
                var clauseViewModel = new ClauseViewModel(clause);
                Clauses.Add(clauseViewModel);
            }
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
                    Devices = clauseViewModel.Devices.ToList(),
                    Zones = clauseViewModel.Zones.ToList(),
                    DeviceUIDs = clauseViewModel.Devices.Select(x => x.UID).ToList(),
                    ZoneUIDs = clauseViewModel.Zones.Select(x => x.UID).ToList(),
                    ClauseJounOperationType = clauseViewModel.SelectedClauseJounOperationType,
                    ClauseOperationType = clauseViewModel.SelectedClauseOperationType
                };
                foreach (var deviceUID in clause.DeviceUIDs)
                {
                    var decvice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
                    clause.Devices.Add(decvice);
                }
                foreach (var zoneUID in clause.ZoneUIDs)
                {
                    var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
                    clause.Zones.Add(zone);
                }
                if ((clause.DeviceUIDs.Count > 0) || (clause.ZoneUIDs.Count > 0))
                    deviceLogic.Clauses.Add(clause);
            }

            Device.DeviceLogic = deviceLogic;
            return base.Save();
        }
    }
}