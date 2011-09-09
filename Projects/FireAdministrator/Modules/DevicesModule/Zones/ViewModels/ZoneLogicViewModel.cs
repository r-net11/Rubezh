using System;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.Zones.Events;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZoneLogicViewModel : SaveCancelDialogContent
    {
        public ZoneLogicViewModel()
        {
            Title = "Настройка логики зон";
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            RemoveCommand = new RelayCommand<ClauseViewModel>(OnRemove);

            ServiceFactory.Events.GetEvent<CurrentClauseStateChangedEvent>().Subscribe(OnCurrentClauseStateChanged);
        }

        Device _device;

        public void Initialize(Device device)
        {
            _device = device;
            Clauses = new ObservableCollection<ClauseViewModel>();
            foreach (var clause in device.ZoneLogic.Clauses)
            {
                var clauseViewModel = new ClauseViewModel();
                clauseViewModel.Initialize(_device, clause);
                Clauses.Add(clauseViewModel);
            }
            if (device.ZoneLogic.Clauses.Count == 0)
            {
                var clauseViewModel = new ClauseViewModel();
                var clause = new Clause();
                clauseViewModel.Initialize(_device, clause);
                Clauses.Add(clauseViewModel);
            }
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
        void OnCurrentClauseStateChanged(ZoneLogicState zoneLogicState)
        {
            _isBlocked = ((zoneLogicState == ZoneLogicState.Lamp) || (zoneLogicState == ZoneLogicState.PCN));
            var selectedClause = Clauses.FirstOrDefault(x => x.SelectedState == zoneLogicState);
            Clauses.Clear();
            Clauses.Add(selectedClause);
        }

        public bool CanAdd()
        {
            return _isBlocked != true;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var clauseViewModel = new ClauseViewModel();
            var clause = new Clause();
            clause.Operation = ZoneLogicOperation.All;
            clause.State = ZoneLogicState.Fire;
            clauseViewModel.Initialize(_device, clause);
            Clauses.Add(clauseViewModel);
        }

        public RelayCommand<ClauseViewModel> RemoveCommand { get; private set; }
        void OnRemove(ClauseViewModel clauseViewModel)
        {
            Clauses.Remove(clauseViewModel);
        }

        protected override void Save(ref bool cancel)
        {
            var zoneLogic = new ZoneLogic();
            foreach (var clauseViewModel in Clauses)
            {
                if (clauseViewModel.Zones.Count > 0)
                {
                    var clause = new Clause();
                    clause.State = clauseViewModel.SelectedState;
                    clause.Operation = clauseViewModel.SelectedOperation;
                    clause.Zones = clauseViewModel.Zones;
                    if (clauseViewModel.SelectedDevice != null)
                    {
                        clause.DeviceUID = clauseViewModel.SelectedDevice.UID;
                    }
                    zoneLogic.Clauses.Add(clause);
                }
            }
            _device.ZoneLogic = zoneLogic;
        }
    }
}