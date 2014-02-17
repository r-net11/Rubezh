using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using VideoModule.Plans.Designer;
using Infrustructure.Plans.Elements;
using XFiresecAPI;
using System.Linq;

namespace VideoModule.Plans
{
	internal class PlanMonitor
	{
		private Plan _plan;
		private Action _callBack;
		private List<XStateClass> _cameraStates;

		public PlanMonitor(Plan plan, Action callBack)
		{
			_plan = plan;
			_callBack = callBack;
			_cameraStates = new List<XStateClass>();
			Initialize();
		}
		private void Initialize()
		{
			foreach (var elementCamera in _plan.ElementExtensions.OfType<ElementCamera>())
			{
				var camera = Helper.GetCamera(elementCamera);
				if (camera != null)
				{
					_cameraStates.Add(camera.StateClass);
					//camera.StateClass.StateChanged += _callBack;
				}
			}
		}

		public XStateClass GetState()
		{
			var result = XStateClass.No;
			foreach (var cameraState in _cameraStates)
				if (cameraState < result)
					result = cameraState;
			return result;
		}
	}
}