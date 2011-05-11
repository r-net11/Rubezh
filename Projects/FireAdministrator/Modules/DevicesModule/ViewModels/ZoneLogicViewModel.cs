using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Collections.ObjectModel;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class ZoneLogicViewModel : BaseViewModel
    {
        public ZoneLogicViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
            SaveCommand = new RelayCommand(OnSave);
        }

        public void SetDevice(DeviceViewModel deviceViewModel)
        {
            Firesec.ZoneLogic.expr zoneLogic = deviceViewModel._device.ZoneLogic;

            StringZoneLogic = ZoneLogicToText.Convert(zoneLogic);

            ClauseViewModels = new ObservableCollection<ClauseViewModel>();
            if (zoneLogic != null)
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
        void OnAdd()
        {
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            OnRequestClose();
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            OnRequestClose();
        }
    }
}
