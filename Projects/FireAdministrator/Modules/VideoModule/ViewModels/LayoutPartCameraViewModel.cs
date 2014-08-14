using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartReferenceProperties _properties;

		public LayoutPartCameraViewModel(LayoutPartReferenceProperties properties)
		{
			Title = "Камера";
			IconSource = LayoutPartDescription.IconPath + "BVideo.png";
			_сameraTitle = null;
			_properties = properties ?? new LayoutPartReferenceProperties();
			var selectedCamera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == _properties.ReferenceUID);
			UpdateLayoutPart(selectedCamera);
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return new LayoutPartPropertyCameraPageViewModel(this);
			}
		}

		private string _сameraTitle;
		public string CameraTitle
		{
			get { return _сameraTitle; }
			set
			{
				_сameraTitle = value;
				OnPropertyChanged(() => CameraTitle);
			}
		}

		public void UpdateLayoutPart(Camera selectedCamera)
		{
			CameraTitle = selectedCamera == null ? Title : selectedCamera.Name;
		}
	}
}