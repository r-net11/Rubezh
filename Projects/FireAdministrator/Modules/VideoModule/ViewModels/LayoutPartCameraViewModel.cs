using System.Collections.Generic;
using System.Linq;
using Localization.Video.Common;
using StrazhAPI.Models;
using StrazhAPI.Models.Layouts;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Events;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartReferenceProperties _properties;

		public LayoutPartCameraViewModel(LayoutPartReferenceProperties properties)
		{
			Title = CommonResources.Camera;
			IconSource = LayoutPartDescription.IconPath + "BVideo.png";
			_cameraTitle = null;
			_properties = properties ?? new LayoutPartReferenceProperties();
			var selectedCamera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == _properties.ReferenceUID);
			UpdateLayoutPart(selectedCamera);
			
			// Следим за удалением из конфигурации зарегистрированной камеры
			ServiceFactory.Events.GetEvent<CameraDeletedEvent>().Subscribe(guid =>
			{
				if (_properties.ReferenceUID == guid)
					UpdateLayoutPart(null);
			});
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

        private string _cameraTitle;
		public string CameraTitle
		{
			get { return _cameraTitle; }
			set
			{
				_cameraTitle = value;
				OnPropertyChanged(() => CameraTitle);
			}
		}

		public void UpdateLayoutPart(Camera selectedCamera)
		{
			CameraTitle = selectedCamera == null ? Title : selectedCamera.Name;
		}
	}
}