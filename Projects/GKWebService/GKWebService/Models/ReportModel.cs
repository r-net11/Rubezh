using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models
{
    public class JournalModel
    {
		public string DeviceDate { get; set; }

		public string SystemDate { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public string Object { get; set; }

        public string Subsystem { get; set; }

		public string SubsystemImage { get; set; }

        public string User { get; set; }
    }
}