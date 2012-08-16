using System;
using System.Collections.Generic;
using FiresecAPI.Models;

namespace XFiresecAPI
{
    public class XDeviceState
    {
        public XDeviceState()
        {
            States = new List<XStateType>();
            StateType = StateType.Norm;
            Children = new List<XDeviceState>();
        }

        public XDeviceState Parent { get; set; }
        public List<XDeviceState> Children { get; set; }

        public Guid UID { get; set; }
        public XDevice Device { get; set; }
        public bool IsConnectionLost { get; private set; }
        public List<XStateType> States { get; private set; }
        public StateType StateType { get; set; }

        public void SetStates(List<XStateType> states)
        {
            if (IsConnectionLost)
            {
                States = new List<XStateType>();
                StateType = StateType.Unknown;
            }
            else
            {
                States = states;
            }
        }

        public void SetIsConnectionLost(bool value)
        {
            IsConnectionLost = true;
            if (value)
            {
                States = new List<XStateType>();
                StateType = StateType.Unknown;
            }
        }

        public event Action StateChanged;
        public void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged();
        }
    }
}