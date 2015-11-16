using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.GK;

namespace GKWebService.Models
{
	public class GKControllerModel
	{
		public bool IsChecked { get; set; }

		public GKDevice Device { get; private set; }

		public GKControllerModel(GKDevice device)
		{
			Device = device;
		}
	}
}