using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrastructure.Common;
using DevicesModule.Events;
using FiresecClient;
using Firesec;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public class ZoneViewModel : BaseViewModel
    {
        public ZoneViewModel()
        {
            SelectCommand = new RelayCommand(OnSelect);
        }

        Zone zone;

        public void Initialize(Zone zone)
        {
            this.zone = zone;
            ZoneState zoneState = FiresecManager.States.ZoneStates.FirstOrDefault(x => x.No == zone.No);
            State = zoneState.State;
        }

        public RelayCommand SelectCommand { get; private set; }
        void OnSelect()
        {
            ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Publish(zone.No);
        }

        public string Name
        {
            get { return zone.Name; }
        }

        public string No
        {
            get { return zone.No; }
        }

        public string Description
        {
            get { return zone.Description; }
        }

        public string DetectorCount
        {
            get { return zone.DetectorCount; }
        }

        public string EvacuationTime
        {
            get { return zone.EvacuationTime; }
        }

        public string PresentationName
        {
            get { return zone.No + "." + zone.Name; }
        }

        State state;
        public State State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }
    }
}
