using Infrastructure.Common.Windows.TreeList;
using VideoModule.ViewModels;

namespace VideoModule.Converters
{
	public class CameraViewModelNameComparer : TreeNodeComparer<CameraViewModel>
	{
		protected override int Compare(CameraViewModel x, CameraViewModel y)
		{
			return string.Compare(x.PresentationName, y.PresentationName);
		}
	}

	public class CamerasViewModelAddressComparer : TreeNodeComparer<CameraViewModel>
	{
		protected override int Compare(CameraViewModel x, CameraViewModel y)
		{
			return string.Compare(x.PresentationAddress, y.PresentationAddress);
		}
	}

}
