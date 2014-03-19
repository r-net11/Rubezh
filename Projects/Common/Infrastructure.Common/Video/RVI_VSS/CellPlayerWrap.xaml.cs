using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using Entities.DeviceOriented;
using FiresecAPI.Models;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public partial class CellPlayerWrap
	{
		public CellPlayerWrap()
		{
			InitializeComponent();
		}

		public void InitializeCamera(Camera camera)
		{
			if (String.IsNullOrEmpty(camera.Address))
				FormsPlayer.StopVideo();
			else
				FormsPlayer.StartVideo(camera);
		}
	}
}