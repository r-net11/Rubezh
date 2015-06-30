using System;
using Infrustructure.Plans.Designer;

namespace Infrastructure.Client.Plans
{
	public interface IPlanDesignerViewModel
	{
		event EventHandler Updated;

		bool AlwaysShowScroll { get; }
		object Toolbox { get; }
		CommonDesignerCanvas Canvas { get; }
		bool IsNotEmpty { get; }

		void ResetZoom(double zoom, double deviceZoom);
		void ChangeZoom(double zoom);
		void ChangeDeviceZoom(double deviceZoom);
		bool HasPermissionsToScale { get; }
		bool CanCollapse { get; }
		bool IsCollapsed { get; set; }
		bool AllowScalePoint { get; }
		bool FullScreenSize { get; }
	}
}