using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using DevicesModule.Views;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public class ClauseViewModel : BaseViewModel
    {
        public ClauseViewModel()
        {
            ShowZonesCommand = new RelayCommand(OnShowZones);
        }

        List<string> _zones;

        public void Initialize(Firesec.ZoneLogic.clauseType clause)
        {
            _zones = new List<string>();
            if (clause.zone != null)
            {
                foreach (var zone in clause.zone)
                    _zones.Add(zone);
            }
            SelectedState = clause.state;
            SelectedOperation = clause.operation;
        }

        public ObservableCollection<string> States
        {
            get
            {
                ObservableCollection<string> _states = new ObservableCollection<string>();
                _states.Add("0");
                _states.Add("1");
                _states.Add("2");
                _states.Add("5");
                _states.Add("6");
                return _states;
            }
        }

        public ObservableCollection<string> Operations
        {
            get
            {
                ObservableCollection<string> _operations = new ObservableCollection<string>();
                _operations.Add("and");
                _operations.Add("or");
                return _operations;
            }
        }

        public string PresenrationZones
        {
            get
            {
                string presenrationZones = "";
                foreach (var zone in _zones)
                {
                    presenrationZones += zone + ",";
                }
                if (presenrationZones.EndsWith(","))
                {
                    presenrationZones = presenrationZones.Remove(presenrationZones.Length - 1, 1);
                }
                return presenrationZones;
            }
        }

        string _selectedState;
        public string SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                OnPropertyChanged("SelectedState");
            }
        }

        string _selectedOperation;
        public string SelectedOperation
        {
            get { return _selectedOperation; }
            set
            {
                _selectedOperation = value;
                OnPropertyChanged("SelectedOperation");
            }
        }

        void Update()
        {
            OnPropertyChanged("PresenrationZones");
        }

        public RelayCommand ShowZonesCommand { get; private set; }
        void OnShowZones()
        {
            ZonesSelectionViewModel zonesSelectionViewModel = new ZonesSelectionViewModel();
            zonesSelectionViewModel.Initialize(_zones);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(zonesSelectionViewModel);
            if (result)
            {
                _zones = zonesSelectionViewModel.Zones;
                Update();
            }
        }

        public Firesec.ZoneLogic.clauseType Save()
        {
            Firesec.ZoneLogic.clauseType clause = new Firesec.ZoneLogic.clauseType();
            clause.state = SelectedState;
            clause.operation = SelectedOperation;
            clause.zone = _zones.ToArray();
            return clause;
        }
    }
}
