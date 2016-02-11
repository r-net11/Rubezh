using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Controls.Converters;
using GKWebService.Utils;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKWebService.Models
{
	public class DirectionStateClass
	{
		public string IconData { get; set; }

		public string Name { get; set; }

		public DirectionStateClass()
		{
			
		}

		public DirectionStateClass(XStateClass stateClass)
		{
			Name = stateClass.ToDescription();
			IconData = stateClass.ToString();
		}
	}
}