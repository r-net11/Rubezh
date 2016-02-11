using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.GK;

namespace GKWebService.Models
{
	public class GKControllerModel
	{
		public Guid UID { get; set; }

		public bool IsChecked { get; set; }

		public string PresentationName { get; set; }

		public GKControllerModel()
		{
		}

		public GKControllerModel(Guid UID, bool isChecked, string presentationName)
		{
			this.UID = UID;
			IsChecked = isChecked;
			PresentationName = presentationName;
		}
	}
}