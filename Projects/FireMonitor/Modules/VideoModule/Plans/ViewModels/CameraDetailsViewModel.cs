using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;

namespace VideoModule.Plans.ViewModels
{
	public class CameraDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public Camera Camera { get; private set; }

		public CameraDetailsViewModel(Camera camera)
		{
			Camera = camera;
			Title = Camera.Name;
			TopMost = true;

		}

		public Brush CameraPicture
		{
			get { return PictureCacheSource.CameraPicture.GetDefaultBrush(); }
		}

		#region IWindowIdentity Members

		public string Guid
		{
			get { return Camera.UID.ToString(); }
		}

		#endregion
	}
}