using System.Linq;
using DevicesModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZoneViewModel : BaseViewModel
    {
        public ZoneViewModel(Zone zone)
        {
            SelectCommand = new RelayCommand(OnSelect);

            Zone = zone;
            var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zone.No);
            StateType = zoneState.StateType;
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

        StateType _stateType;
        public StateType StateType
        {
            get { return _stateType; }
            set
            {
                _stateType = value;
                OnPropertyChanged("StateType");
            }
        }
    }
}