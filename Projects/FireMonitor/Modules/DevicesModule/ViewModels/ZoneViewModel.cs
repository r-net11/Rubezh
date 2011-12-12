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
        public Zone Zone { get; private set; }
        public ZoneState ZoneState { get; private set; }

        public ZoneViewModel(Zone zone)
        {
            SelectCommand = new RelayCommand(OnSelect);

            Zone = zone;
            ZoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zone.No);
            StateType = ZoneState.StateType;
        }

        public RelayCommand SelectCommand { get; private set; }
        void OnSelect()
        {
            ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Publish(Zone.No);
        }

        public string PresentationName
        {
            get { return Zone.PresentationName; }
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