using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using XFiresecAPI;

namespace PlansModule.Kursk
{
	internal class PlanMonitor
	{
		private Plan _plan;
		private Action _callBack;

		public PlanMonitor(Plan plan, Action callBack)
		{
			_plan = plan;
			_callBack = callBack;
			Initialize();
		}
		private void Initialize()
		{
		}
		public XStateClass GetState()
		{
			var result = XStateClass.No;
			return result;
		}
	}
}