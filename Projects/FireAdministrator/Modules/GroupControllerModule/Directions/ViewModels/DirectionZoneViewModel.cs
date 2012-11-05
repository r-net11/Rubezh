using System.Collections.Generic;
using Infrastructure;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class DirectionZoneViewModel
    {
        public DirectionZoneViewModel(XDirectionZone directionZone)
        {
            DirectionZone = directionZone;
        }

        public XDirectionZone DirectionZone { get; private set; }

        public XStateType StateType
        {
            get { return DirectionZone.StateType; }
            set
            {
                DirectionZone.StateType = value;
                ServiceFactory.SaveService.GKChanged = true;
            }
        }

        public List<XStateType> StateTypes
        {
            get
            {
                var stateTypes = new List<XStateType>();
                stateTypes.Add(XStateType.Attention);
                stateTypes.Add(XStateType.Fire1);
                stateTypes.Add(XStateType.Fire2);
                return stateTypes;
            }
        }
    }
}