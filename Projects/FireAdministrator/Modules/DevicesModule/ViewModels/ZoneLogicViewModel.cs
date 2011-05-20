using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Windows;

namespace DevicesModule.ViewModels
{
    public class ZoneLogicViewModel : DialogContent
    {
        public ZoneLogicViewModel()
        {
            Title = "Настройка логики зон";
            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
            SaveCommand = new RelayCommand(OnSave);
        }

        public void Initialize(DeviceViewModel deviceViewModel)
        {
            Firesec.ZoneLogic.expr zoneLogic = deviceViewModel._device.ZoneLogic;

            Clauses = new ObservableCollection<ClauseViewModel>();
            if (zoneLogic != null)
                foreach (Firesec.ZoneLogic.clauseType clause in zoneLogic.clause)
                {
                    ClauseViewModel clauseViewModel = new ClauseViewModel();
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
            ClauseViewModel clauseViewModel = new ClauseViewModel();
            clauseViewModel.Initialize(null);
            Clauses.Add(clauseViewModel);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            foreach (var clause in Clauses)
            {
                clause.Save();
                var x1 = clause.Clause.operation;
                var x2 = clause.Clause.state;
            }
            Close(true);
        }
    }
}
