using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI;
using RubezhAPI.Journal;
using GKWebService.Controllers;

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

		public JournalModel(JournalItem x)
		{
			Desc = x.JournalEventDescriptionType.ToDescription();
			SystemDate = x.SystemDateTime.ToString();
			Name = x.JournalEventNameType.ToDescription();
			Object = x.JournalObjectType.ToDescription();
			DeviceDate = x.DeviceDateTime.ToString();
			Subsystem = x.JournalSubsystemType.ToDescription();
			User = x.UserName;
			SubsystemImage = JournalController.GetSubsystemImage(x.JournalSubsystemType);
		}
    }
}