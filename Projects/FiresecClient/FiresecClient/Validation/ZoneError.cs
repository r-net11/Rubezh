using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecAPI;

namespace FiresecClient.Validation
{
    public class ZoneError : BaseError
    {
        public ZoneError(Zone zone, string error, ErrorLevel level)
            : base(error, level)
        {
            Zone = zone;
        }

        public Zone Zone { get; set; }
    }
}
