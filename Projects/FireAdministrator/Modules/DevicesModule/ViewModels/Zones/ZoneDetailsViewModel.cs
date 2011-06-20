using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public class ZoneDetailsViewModel : DialogContent
    {
        public ZoneDetailsViewModel(Zone zone)
        {
            Title = "Свойства зоны";
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);

            _zone = zone;
            Name = zone.Name;
            No = zone.No;
            Description = zone.Description;
            DetectorCount = zone.DetectorCount;
            EvacuationTime = zone.EvacuationTime;
        }

        Zone _zone;

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        string _no;
        public string No
        {
            get { return _no; }
            set
            {
                _no = value;
                OnPropertyChanged("No");
            }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        string _detectorCount;
        public string DetectorCount
        {
            get { return _detectorCount; }
            set
            {
                _detectorCount = value;
                OnPropertyChanged("DetectorCount");
            }
        }

        string _evacuationTime;
        public string EvacuationTime
        {
            get { return _evacuationTime; }
            set
            {
                _evacuationTime = value;
                OnPropertyChanged("EvacuationTime");
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            _zone.Name = Name;
            _zone.No = No;
            _zone.Description = Description;
            _zone.DetectorCount = DetectorCount;
            _zone.EvacuationTime = EvacuationTime;
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
