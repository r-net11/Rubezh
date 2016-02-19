using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Controls.Converters;
using GKWebService.Utils;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKWebService.Models
{
	public class StateClass
	{
		public string IconData { get; set; }

		public string Name { get; set; }

		public StateClass()
		{ 
			
		}
		public StateClass(XStateClass stateClass)
		{
			Name = stateClass.ToDescription();
			IconData = stateClass.ToString();
		}
	}
}