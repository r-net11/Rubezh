using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.Zones.Events;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Common;

namespace DevicesModule.ViewModels
{
    public class ZoneLogicViewModel : SaveCancelDialogContent
    {
        Device _device;

        public ZoneLogicViewModel()
        {
            Title = "Настройка логики зон";
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            RemoveCommand = new RelayCommand<ClauseViewModel>(OnRemove);
            ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);

            ServiceFactory.Events.GetEvent<CurrentClauseStateChangedEvent>().Subscribe(OnCurrentClauseStateChanged);
        }

        public void Initialize(Device device)
        {
            _device = device;
            Clauses = new ObservableCollection<ClauseViewModel>();
            if (device.ZoneLogic != null && device.ZoneLogic.Clauses.IsNotNullOrEmpty())
            {
                foreach (var clause in device.ZoneLogic.Clauses)
                {
                    var clauseViewModel = new ClauseViewModel();
                    clauseViewModel.Initialize(_device, clause);
                    Clauses.Add(clauseViewModel);
                }
            }
            else
            {
                device.ZoneLogic = new ZoneLogic();
            }

            if (device.ZoneLogic.Clauses.Count == 0)
            {
                var clauseViewModel = new ClauseViewModel();
                var clause = new Clause();
                clauseViewModel.Initialize(_device, clause);
                Clauses.Add(clauseViewModel);
            }

            JoinOperator = device.ZoneLogic.JoinOperator;
        }

        ZoneLogicJoinOperator _joinOperator;
        public ZoneLogicJoinOperator JoinOperator
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
            if (JoinOperator == ZoneLogicJoinOperator.And)
                JoinOperator = ZoneLogicJoinOperator.Or;
            else
                JoinOperator = ZoneLogicJoinOperator.And;
        }

        public bool ShowZoneLogicJoinOperator
        {
            get { return Clauses.Count > 1; }
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
            var clause = new Clause()
            {
                Operation = ZoneLogicOperation.All,
                State = ZoneLogicState.Fire
            };
            clauseViewModel.Initialize(_device, clause);
            Clauses.Add(clauseViewModel);
            OnPropertyChanged("ShowZoneLogicJoinOperator");
        }

        public RelayCommand<ClauseViewModel> RemoveCommand { get; private set; }
        void OnRemove(ClauseViewModel clauseViewModel)
        {
            Clauses.Remove(clauseViewModel);
            OnPropertyChanged("ShowZoneLogicJoinOperator");
        }

        protected override void Save(ref bool cancel)
        {
            var zoneLogic = new ZoneLogic();
            foreach (var clauseViewModel in Clauses)
            {
                if (clauseViewModel.Zones.Count > 0)
                {
                    var clause = new Clause()
                    {
                        State = clauseViewModel.SelectedState,
                        Operation = clauseViewModel.SelectedOperation,
                        Zones = clauseViewModel.Zones
                    };
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