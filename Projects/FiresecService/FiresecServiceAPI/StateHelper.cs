using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecAPI
{
    public static class StateHelper
    {
        public static List<StateType> AllStates
        {
            get
            {
                List<StateType> states = new List<StateType>();
                states.Add(StateType.Attention);
                states.Add(StateType.Failure);
                states.Add(StateType.Fire);
                states.Add(StateType.Info);
                states.Add(StateType.No);
                states.Add(StateType.Norm);
                states.Add(StateType.Off);
                states.Add(StateType.Service);
                states.Add(StateType.Unknown);
                return states;
            }
        }
    }
}
