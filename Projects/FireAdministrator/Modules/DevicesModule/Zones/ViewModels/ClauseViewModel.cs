using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using System;
using System.Linq;

namespace DevicesModule.ViewModels
{
    public class ClauseViewModel : BaseViewModel
    {
        public ClauseViewModel()
        {
            ShowZonesCommand = new RelayCommand(OnShowZones);
        }

        public List<string> Zones { get; set; }

        public void Initialize(Clause clause)
        {
            Zones = new List<string>();
            foreach (var zone in clause.Zones)
                Zones.Add(zone);

            SelectedState = clause.State;
            SelectedOperation = clause.Operation;
        }

        public List<ZoneLogicState> States
        {
            get { return Enum.GetValues(typeof(ZoneLogicState)).Cast<ZoneLogicState>().ToList(); }
        }

        public ObservableCollection<ZoneLogicOperation> Operations
        {
            get
            {
                ObservableCollection<ZoneLogicOperation> _operations = new ObservableCollection<ZoneLogicOperation>();
                _operations.Add(ZoneLogicOperation.All);
                _operations.Add(ZoneLogicOperation.Any);
                return _operations;
            }
        }

        public string PresenrationZones
        {
            get
            {
                string presenrationZones = "";
                foreach (var zone in Zones)
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

        ZoneLogicState _selectedState;
        public ZoneLogicState SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                OnPropertyChanged("SelectedState");
            }
        }

        ZoneLogicOperation _selectedOperation;
        public ZoneLogicOperation SelectedOperation
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
            var zonesSelectionViewModel = new ZonesSelectionViewModel();
            zonesSelectionViewModel.Initialize(Zones);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(zonesSelectionViewModel);
            if (result)
            {
                Zones = zonesSelectionViewModel.GuardLevelNames;
                Update();
            }
        }
    }
}