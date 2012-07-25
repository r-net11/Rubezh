using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans;
using Infrustructure.Plans.Services;
using GKModule.Plans.ViewModels;

namespace GKModule
{
	class GKPlanExtension : IPlanExtension
	{
		public GKPlanExtension()
		{
			TabPage = new DevicesViewModel();
		}

		#region IPlanExtension Members

		public int Index
		{
			get { return 1; }
		}

		public string Alias
		{
			get { return "GK"; }
		}

		public string Title
		{
			get { return "ГК"; }
		}

		public object TabPage { get; private set; }

		#endregion
	}
}
