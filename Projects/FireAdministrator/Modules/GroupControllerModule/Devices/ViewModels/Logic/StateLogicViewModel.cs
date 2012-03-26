using Infrastructure.Common;
using GroupControllerModule.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System;

namespace GroupControllerModule.ViewModels
{
    public class StateLogicViewModel : BaseViewModel
    {
        DeviceLogicViewModel _deviceDetailsViewModel;

        public StateLogicViewModel(DeviceLogicViewModel deviceDetailsViewModel, StateLogic stateLogic)
        {
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand<ClauseViewModel>(OnRemove);
            ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);

            DeviceStates = Enum.GetValues(typeof(XStateType)).Cast<XStateType>().ToList();
            SelectedDeviceState = stateLogic.StateType;

            _deviceDetailsViewModel = deviceDetailsViewModel;
            Clauses = new ObservableCollection<ClauseViewModel>();
            foreach (var clause in stateLogic.Clauses)
            {
                var clauseViewModel = new ClauseViewModel(this, clause);
                Clauses.Add(clauseViewModel);
            }
        }

        public List<XStateType> DeviceStates { get; private set; }

        XStateType _selectedDeviceState;
        public XStateType SelectedDeviceState
        {
            get { return _selectedDeviceState; }
            set
            {
                _selectedDeviceState = value;
                OnPropertyChanged("SelectedDeviceState");
            }
        }

        public ObservableCollection<ClauseViewModel> Clauses { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var clause = new XClause();
            var clauseViewModel = new ClauseViewModel(this, clause);
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
    }
}