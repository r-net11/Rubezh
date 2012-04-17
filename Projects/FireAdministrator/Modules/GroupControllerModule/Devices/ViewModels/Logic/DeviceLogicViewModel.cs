using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using GKModule.Models;
using Infrastructure.Common;
using Controls.MessageBox;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class DeviceLogicViewModel : SaveCancelDialogContent
    {
        XDevice Device;

        public DeviceLogicViewModel(XDevice device)
        {
            Title = "Настройка логики устройства";
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand<StateLogicViewModel>(OnRemove);

            Device = device;

            StateLogics = new ObservableCollection<StateLogicViewModel>();
            foreach (var stateLogic in device.DeviceLogic.StateLogics)
            {
                var stateLogicViewModel = new StateLogicViewModel(this, stateLogic);
                StateLogics.Add(stateLogicViewModel);
            }
        }

        public ObservableCollection<StateLogicViewModel> StateLogics { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var stateLogic = new StateLogic();
            var stateLogicViewModel = new StateLogicViewModel(this, stateLogic);
            StateLogics.Add(stateLogicViewModel);
        }

        public RelayCommand<StateLogicViewModel> RemoveCommand { get; private set; }
        void OnRemove(StateLogicViewModel stateLogicViewModel)
        {
            StateLogics.Remove(stateLogicViewModel);
        }

        protected override void Save(ref bool cancel)
        {
            var deviceLogic = new XDeviceLogic();
            foreach (var stateLogicViewModel in StateLogics)
            {
                var stateLogic = new StateLogic();
                stateLogic.StateType = stateLogicViewModel.SelectedStateType;
                foreach (var clauseViewModel in stateLogicViewModel.Clauses)
                {
                    var clause = new XClause()
                    {
                        StateType = clauseViewModel.SelectedStateType,
                        Devices = clauseViewModel.Devices.ToList(),
                        Zones = clauseViewModel.Zones.ToList(),
                        ClauseJounOperationType = clauseViewModel.SelectedClauseJounOperationType,
                        ClauseOperandType = clauseViewModel.SelectedClauseOperandType,
                        ClauseOperationType = clauseViewModel.SelectedClauseOperationType
                    };
                    if ((clause.Devices.Count > 0) || (clause.Zones.Count > 0))
                        stateLogic.Clauses.Add(clause);
                }
                if (deviceLogic.StateLogics.Any(x=>x.StateType == stateLogic.StateType))
                {
                    MessageBoxService.ShowWarning("Логика для состояние " + stateLogic.StateType.ToDescription() + " дублируется");
                    cancel = true;
                    return;
                }
                if (stateLogic.Clauses.Count > 0)
                    deviceLogic.StateLogics.Add(stateLogic);

                Device.DeviceLogic = deviceLogic;
            }
        }
    }
}