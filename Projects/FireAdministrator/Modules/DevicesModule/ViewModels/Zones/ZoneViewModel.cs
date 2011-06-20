using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class ZoneViewModel : BaseViewModel
    {
        public Zone Zone { get; private set; }

        public ZoneViewModel(Zone zone)
        {
            Zone = zone;
        }

        public string Name
        {
            get { return Zone.Name; }
        }

        public string No
        {
            get { return Zone.No; }
        }

        public string Description
        {
            get { return Zone.Description; }
        }

        public string DetectorCount
        {
            get { return Zone.DetectorCount; }
        }

        public string EvacuationTime
        {
            get { return Zone.EvacuationTime; }
        }

        public string PresentationName
        {
            get { return No + "." + Name; }
        }

        public void Update()
        {
            OnPropertyChanged("Name");
            OnPropertyChanged("No");
            OnPropertyChanged("Description");
            OnPropertyChanged("DetectorCount");
            OnPropertyChanged("EvacuationTime");
            OnPropertyChanged("PresentationName");
        }
    }
}
