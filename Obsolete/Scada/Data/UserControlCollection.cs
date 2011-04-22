using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlBase;

namespace Data
{
    [Serializable]
    public class UserControlCollection
    {

        public List<ControlBase.UserControlBase> Controls { get; set; }
    }
}
