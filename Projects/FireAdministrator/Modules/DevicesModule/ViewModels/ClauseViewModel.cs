using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using DevicesModule.Views;

namespace DevicesModule.ViewModels
{
    public class ClauseViewModel : BaseViewModel
    {
        public ClauseViewModel()
        {
            ShowZonesCommand = new RelayCommand(OnShowZones);
            ClauseStates = new ObservableCollection<ClauseStateViewModel>();
            ClauseStates.Add(new ClauseStateViewModel() { Name = "Включение автоматики", Id = "0" });
            ClauseStates.Add(new ClauseStateViewModel() { Name = "Тревога", Id = "1" });
            ClauseStates.Add(new ClauseStateViewModel() { Name = "Пожар", Id = "2" });
            ClauseStates.Add(new ClauseStateViewModel() { Name = "Внимание", Id = "5" });
            ClauseStates.Add(new ClauseStateViewModel() { Name = "Включение модуля пожаротушения", Id = "6" });

            ClauseOperations = new ObservableCollection<ClauseOperationViewModel>();
            ClauseOperations.Add(new ClauseOperationViewModel() { Name = "во всех зонах из", Id = "and" });
            ClauseOperations.Add(new ClauseOperationViewModel() { Name = "в любой зонах из", Id = "or" });
        }

        Firesec.ZoneLogic.clauseType clause;

        public RelayCommand ShowZonesCommand { get; private set; }
        void OnShowZones()
        {
            ZoneLogicSectionView zoneLogicSectionView = new ZoneLogicSectionView();
            ZoneLogicSectionViewModel zoneLogicSectionViewModel = new ZoneLogicSectionViewModel();
            zoneLogicSectionViewModel.RequestClose += delegate { zoneLogicSectionView.Close(); };
            zoneLogicSectionViewModel.Initialize(clause);
            zoneLogicSectionView.DataContext = zoneLogicSectionViewModel;
            zoneLogicSectionView.ShowDialog();
        }

        public void Initialize(Firesec.ZoneLogic.clauseType clause)
        {
            this.clause = clause;
            SelectedClauseState = ClauseStates.FirstOrDefault(x => x.Id == clause.state);
            SelectedClauseOperation = ClauseOperations.FirstOrDefault(x => x.Id == clause.operation);
            string zonesIds = "";
            foreach (string zoneId in clause.zone)
            {
                zonesIds += zoneId + ",";
            }
            Zones = zonesIds;
        }

        public ObservableCollection<ClauseStateViewModel> ClauseStates { get; private set; }
        public ObservableCollection<ClauseOperationViewModel> ClauseOperations { get; private set; }

        ClauseStateViewModel selectedClauseState;
        public ClauseStateViewModel SelectedClauseState
        {
            get { return selectedClauseState; }
            set
            {
                selectedClauseState = value;
                clause.state = selectedClauseState.Id;
                OnPropertyChanged("SelectedClauseState");
            }
        }

        ClauseOperationViewModel selectedClauseOperation;
        public ClauseOperationViewModel SelectedClauseOperation
        {
            get { return selectedClauseOperation; }
            set
            {
                selectedClauseOperation = value;
                clause.operation = selectedClauseOperation.Id;
                OnPropertyChanged("SelectedClauseOperation");
            }
        }

        string zones;
        public string Zones
        {
            get { return zones; }
            set
            {
                zones = value;
                OnPropertyChanged("Zones");
            }
        }
    }

    public class ClauseStateViewModel : BaseViewModel
    {
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
    }

    public class ClauseOperationViewModel : BaseViewModel
    {
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
    }
}
