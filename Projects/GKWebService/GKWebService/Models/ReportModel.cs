using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models
{
    public class ReportModel
    {
        public DateTime? DeviceDate { get; set; }

        public DateTime? SystemDate { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public string Object { get; set; }

        public string Subsystem { get; set; }

        public string User { get; set; }
    }
}