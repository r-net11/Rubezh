using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using Firesec;

namespace FiresecClient
{
    public class InnerState
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public bool AffectChildren { get; private set; }
        public int Priority { get; private set; }
        public State State { get; private set; }
        public bool IsManualReset { get; private set; }
        public bool IsAutomatic { get; private set; }
        public bool IsActive { get; set; }

        public InnerState(Firesec.Metadata.configDrvState innerState)
        {
            Id = innerState.id;
            Name = innerState.name;
            Priority = System.Convert.ToInt32(innerState.@class);
            State = new Firesec.State(Priority);
            AffectChildren = innerState.affectChildren == "1" ? true : false;
            IsManualReset = innerState.manualReset == "1" ? true : false;
            IsAutomatic = innerState.type == "Auto" ? true : false;
        }
    }
}
