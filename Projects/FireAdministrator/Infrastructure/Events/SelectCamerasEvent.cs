using Microsoft.Practices.Prism.Events;
using System.Collections.Generic;
using RubezhAPI.Models;

namespace Infrastructure.Events
{
	public class SelectCamerasEvent : CompositePresentationEvent<SelectCamerasEventArg>
	{
	}

	public class SelectCamerasEventArg
	{
		public bool Cancel { get; set; }
		public List<Camera> Cameras { get; set; }
	}
}