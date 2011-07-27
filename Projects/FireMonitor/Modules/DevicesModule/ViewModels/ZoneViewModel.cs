using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrastructure.Common;
using DevicesModule.Events;
using FiresecClient;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public class ZoneViewModel : BaseViewModel
    {
        public ZoneViewModel(Zone zone)
        {
            SelectCommand = new RelayCommand(OnSelect);

            Zone = zone;
            ZoneState zoneState = FiresecManager.States.ZoneStates.FirstOrDefault(x => x.No == zone.No);
            State = zoneState.State;
        }

        public Zone Zone { get; private set; }

        public RelayCommand SelectCommand { get; private set; }
        void OnSelect()
        {
            ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Publish(Zone.No);
        }

        public string PresentationName
        {
            get { return Zone.No + "." + Zone.Name; }
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
