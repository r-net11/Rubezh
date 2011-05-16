using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Firesec;

namespace FiresecClient
{
    public class ZoneState
    {
        public string No { get; set; }
        public State State { get; set; }

        public ZoneState()
        {
            State = new State(8);
        }
    }
}
