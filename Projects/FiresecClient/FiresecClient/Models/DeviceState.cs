using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Firesec;

namespace FiresecClient
{
    public class DeviceState
    {
        public string Id { get; set; }
        public string PlaceInTree { get; set; }
        public List<InnerState> InnerStates { get; set; }
        public State State { get; set; }
        public List<string> States { get; set; }
        public List<Parameter> Parameters { get; set; }
        public ChangeEntities ChangeEntities { get; set; }
        public List<string> SelfStates { get; set; }
        public List<InnerState> ParentInnerStates { get; set; }
        public List<string> ParentStringStates { get; set; }
        public int MinPriority { get; set; }
        public string SourceState { get; set; }

        public DeviceState()
        {
            State = new State(8);
        }

        public event Action StateChanged;
        public void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged();
        }
    }
}
