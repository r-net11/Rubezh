using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ClientApi
{
    public class ChangeEntities
    {
        public bool StateChanged { get; set; }
        public bool StatesChanged { get; set; }
        public bool ParameterChanged { get; set; }
        public bool VisibleParameterChanged { get; set; }

        public void Reset()
        {
            StateChanged = false;
            StatesChanged = false;
            ParameterChanged = false;
            VisibleParameterChanged = false;
        }
    }
}
