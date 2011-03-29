﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ServiceApi
{
    public class State
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool AffectChildren { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
    }
}
