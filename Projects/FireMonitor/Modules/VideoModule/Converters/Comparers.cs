using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using VideoModule.ViewModels;

namespace VideoModule.Converters
{
	public class CameraViewModelNameComparer : TreeNodeComparer<CameraViewModel>
	{
		protected override int Compare(CameraViewModel x, CameraViewModel y)
		{
			if (x.Camera != null && y.Camera != null)
				return string.Compare(x.Camera.Name, y.Camera.Name);
			else 
				return Compare(x, y);
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
