using System.Linq;
using DevicesModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using System;

namespace DevicesModule.ViewModels
{
    public class ZoneViewModel : BaseViewModel
    {
        public Zone Zone { get; private set; }
        public ZoneState ZoneState { get; private set; }

        public ZoneViewModel(Zone zone)
        {
            SelectCommand = new RelayCommand(OnSelect);
            ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);

            Zone = zone;
            ZoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zone.No);
            ZoneState.StateChanged += new System.Action(OnStateChanged);
            OnStateChanged();
        }

        void OnStateChanged()
        {
            StateType = ZoneState.StateType;
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

        public RelayCommand SelectCommand { get; private set; }
        void OnSelect()
        {
            ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Publish(Zone.No);
        }

        public bool CanShowOnPlan()
        {
            foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
            {
                if (plan.ElementPolygonZones.Any(x => (x.ZoneNo.HasValue) && (x.ZoneNo.Value == Zone.No)))
                {
                    return true;
                }
            }
            return false;
        }

        public RelayCommand ShowOnPlanCommand { get; private set; }
        void OnShowOnPlan()
        {
            ServiceFactory.Events.GetEvent<ShowZoneOnPlanEvent>().Publish(Zone.No);
        }
    }
}