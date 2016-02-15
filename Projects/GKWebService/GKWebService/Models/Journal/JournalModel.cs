using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI;
using RubezhAPI.Journal;
using GKWebService.Controllers;
using RubezhAPI.GK;
using System.Drawing;

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
		public Guid ObjectUid { get; set; }
		public string Color { get; set; }

		public JournalModel(JournalItem journalItem)
		{
			Desc = journalItem.JournalEventDescriptionType.ToDescription();
			SystemDate = journalItem.SystemDateTime.ToString();
			Name = journalItem.JournalEventNameType.ToDescription();
			Object = journalItem.ObjectName;
			DeviceDate = journalItem.DeviceDateTime.ToString();
			Subsystem = journalItem.JournalSubsystemType.ToDescription();
			User = journalItem.UserName;
			SubsystemImage = JournalController.GetSubsystemImage(journalItem.JournalSubsystemType);
			ObjectUid = journalItem.ObjectUID;
			Color = GetStateColor(journalItem);
		}

		string GetStateColor(JournalItem journalItem)
		{
			var stateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);
			switch (stateClass)
			{
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.ConnectionLost:
				case XStateClass.TechnologicalRegime:
				case XStateClass.HasNoLicense:
					return ColorTranslator.ToHtml(System.Drawing.Color.Gray);

				case XStateClass.Fire2:
				case XStateClass.Fire1:
					return ColorTranslator.ToHtml(System.Drawing.Color.Red);

				case XStateClass.Attention:
					return ColorTranslator.ToHtml(System.Drawing.Color.Orange);

				case XStateClass.Failure:
					return ColorTranslator.ToHtml(System.Drawing.Color.Pink);

				case XStateClass.Service:
					return ColorTranslator.ToHtml(System.Drawing.Color.Yellow);

				case XStateClass.Ignore:
					return ColorTranslator.ToHtml(System.Drawing.Color.Yellow);

				case XStateClass.On:
					return ColorTranslator.ToHtml(System.Drawing.Color.LightBlue);

				case XStateClass.AutoOff:
					return ColorTranslator.ToHtml(System.Drawing.Color.Yellow);

				case XStateClass.Test:
				case XStateClass.Norm:
					return ColorTranslator.ToHtml(System.Drawing.Color.Transparent);

				default:
					return ColorTranslator.ToHtml(System.Drawing.Color.Transparent);
			}
		}
    }
}