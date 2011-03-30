using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections.ObjectModel;
using ServiceApi;

namespace ServiceVisualizer
{
    public class ZoneLogicViewModel : BaseViewModel
    {
        public ZoneLogicViewModel()
        {
            AddCommand = new RelayCommand(OnAddCommand);
            CancelCommand = new RelayCommand(OnCancelCommand);
            SaveCommand = new RelayCommand(OnSaveCommand);
        }

        public void SetDevice(DeviceViewModel deviceViewModel)
        {
            Firesec.ZoneLogic.expr zoneLogic = deviceViewModel.Device.ZoneLogic;

            StringZoneLogic = ZoneLogicToText.Convert(zoneLogic);

            ClauseViewModels = new ObservableCollection<ClauseViewModel>();
            foreach (Firesec.ZoneLogic.clauseType clause in zoneLogic.clause)
            {
                ClauseViewModel clauseViewModel = new ClauseViewModel();
                clauseViewModel.Initialize(clause);
                ClauseViewModels.Add(clauseViewModel);
            }
        }

        public string stringZoneLogic;
        public string StringZoneLogic
        {
            get { return stringZoneLogic; }
            set
            {
                stringZoneLogic = value;
                OnPropertyChanged("StringZoneLogic");
            }
        }

        public Action RequestClose { get; set; }
        void OnRequestClose()
        {
            if (RequestClose != null)
                RequestClose();
        }

        ObservableCollection<ClauseViewModel> clauseViewModels;
        public ObservableCollection<ClauseViewModel> ClauseViewModels
        {
            get { return clauseViewModels; }
            set
            {
                clauseViewModels = value;
                OnPropertyChanged("ClauseViewModels");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAddCommand(object obj)
        {
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancelCommand(object obj)
        {
            OnRequestClose();
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSaveCommand(object obj)
        {
            OnRequestClose();
        }
    }
}
