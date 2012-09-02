using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.GK
{
    public class XArchiveFilter
    {
        public XArchiveFilter()
        {
        }

        public bool UseSystemDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}