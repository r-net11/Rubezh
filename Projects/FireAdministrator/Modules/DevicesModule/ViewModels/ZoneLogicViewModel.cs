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
            RemoveCommand = new RelayCommand(OnRemove);
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public void Initialize(Firesec.ZoneLogic.expr zoneLogic)
        {
            Clauses = new ObservableCollection<ClauseViewModel>();
            if (zoneLogic != null)
            {
                foreach (Firesec.ZoneLogic.clauseType clause in zoneLogic.clause)
                {
                    ClauseViewModel clauseViewModel = new ClauseViewModel();
                    clauseViewModel.Initialize(clause);
                    Clauses.Add(clauseViewModel);
                }
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
            Firesec.ZoneLogic.clauseType clause = new Firesec.ZoneLogic.clauseType();
            clause.operation = "and";
            clause.state = "0";
            clause.zone = new string[0];
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

        public Firesec.ZoneLogic.expr Save()
        {
            Firesec.ZoneLogic.expr zoneLogic = new Firesec.ZoneLogic.expr();
            List<Firesec.ZoneLogic.clauseType> clauses = new List<Firesec.ZoneLogic.clauseType>();
            foreach (var clauseViewModel in Clauses)
            {
                var clause = clauseViewModel.Save();
                clauses.Add(clause);
            }
            zoneLogic.clause = clauses.ToArray();
            return zoneLogic;
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
