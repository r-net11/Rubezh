using System;
using FiresecAPI.Models;

namespace Infrastructure.Client.Plans
{
	public interface IPlanDesignerViewModel
	{
		event EventHandler Updated;

		bool AlwaysShowScroll { get; }
		object Toolbox { get; }
		object Canvas { get; }
		Plan Plan { get; }

		void ResetZoom(double zoom, double deviceZoom);
		void ChangeZoom(double zoom);
		void ChangeDeviceZoom(double deviceZoom);
		bool HasPermissionsToScale { get; }

	}
}