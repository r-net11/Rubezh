using System;
using FiresecAPI.Models;

namespace Infrastructure.Client.Plans
{
	public interface IPlanDesignerViewModel
	{
		event EventHandler Updated;

		object Toolbox { get; }
		object Canvas { get; }
		Plan Plan { get; }

		void ChangeZoom(double zoom);
		void ChangeDeviceZoom(double deviceZoom);
	}
}
