using Microsoft.Practices.Prism.Events;
using RubezhAPI.Models;

namespace Infrastructure.Events
{
	public class SelectCameraEvent : CompositePresentationEvent<SelectCameraEventArg>
	{
	}

	public class SelectCameraEventArg
	{
		public bool Cancel { get; set; }
		public Camera Camera { get; set; }
	}
}