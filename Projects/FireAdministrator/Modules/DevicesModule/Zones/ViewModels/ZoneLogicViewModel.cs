using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZoneLogicViewModel : DialogContent
    {
        public ZoneLogicViewModel()
        {
            Title = "Настройка логики зон";
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand<ClauseViewModel>(OnRemove);
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        Device _device;

        public void Initialize(Device device)
        {
            _device = device;
            Clauses = new ObservableCollection<ClauseViewModel>();
            foreach (var clause in device.ZoneLogic.Clauses)
            {
                var clauseViewModel = new ClauseViewModel();
                clauseViewModel.Initialize(clause);
                Clauses.Add(clauseViewModel);
            }
        }

        ObservableCollection<ClauseViewModel> _clauses;
        public ObservableCollection<ClauseViewModel> Clauses
        {
            get { return _clauses; }
            set
            {
                _clauses = value;
                OnPropertyChanged("Clauses");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var clauseViewModel = new ClauseViewModel();
            var clause = new Clause();
            clause.Operation = ZoneLogicOperation.All;
            clause.State = ZoneLogicState.Fire;
            clauseViewModel.Initialize(clause);
            Clauses.Add(clauseViewModel);
        }

        public RelayCommand<ClauseViewModel> RemoveCommand { get; private set; }
        void OnRemove(ClauseViewModel clauseViewModel)
        {
            Clauses.Remove(clauseViewModel);
        }

        void Save()
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
                    zoneLogic.Clauses.Add(clause);
                }
            }
            _device.ZoneLogic = zoneLogic;
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Save();
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}