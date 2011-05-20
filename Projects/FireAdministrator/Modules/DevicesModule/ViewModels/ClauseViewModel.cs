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

        public Firesec.ZoneLogic.clauseType Clause { get; private set; }

        public void Initialize(Firesec.ZoneLogic.clauseType clause)
        {
            Clause = clause;
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

        public string Zones
        {
            get
            {
                string zones = "";
                foreach (string zoneId in Clause.zone)
                {
                    zones += zoneId + ",";
                }
                if (zones.EndsWith(","))
                {
                    zones = zones.Remove(zones.Length - 1, 1);
                }
                return zones;
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

        public RelayCommand ShowZonesCommand { get; private set; }
        void OnShowZones()
        {
            ZoneLogicSectionViewModel zoneLogicSectionViewModel = new ZoneLogicSectionViewModel();
            zoneLogicSectionViewModel.Initialize(Clause);
            ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicSectionViewModel);
        }

        public void Save()
        {
            Clause.state = SelectedState;
            Clause.operation = SelectedOperation;
        }
    }
}
