using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public class ZoneLogicViewModel : DialogContent
    {
        public ZoneLogicViewModel()
        {
            Title = "Настройка логики зон";
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand(OnRemove);
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
                ClauseViewModel clauseViewModel = new ClauseViewModel();
                clauseViewModel.Initialize(clause);
                Clauses.Add(clauseViewModel);
            }

            if (Clauses.Count > 0)
                SelectedClause = Clauses[0];
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

        ClauseViewModel _selectedClause;
        public ClauseViewModel SelectedClause
        {
            get { return _selectedClause; }
            set
            {
                _selectedClause = value;
                OnPropertyChanged("SelectedClause");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            ClauseViewModel clauseViewModel = new ClauseViewModel();
            Clause clause = new Clause();
            clause.Operation = ZoneLogicOperation.All;
            clause.State = ZoneLogicState.AutomaticOn;
            clauseViewModel.Initialize(clause);
            Clauses.Add(clauseViewModel);
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            if (SelectedClause != null)
            {
                Clauses.Remove(SelectedClause);
            }
        }

        void Save()
        {
            ZoneLogic zoneLogic = new ZoneLogic();
            foreach (var clauseViewModel in Clauses)
            {
                if (clauseViewModel.Zones.Count > 0)
                {
                    Clause clause = new Clause();
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
