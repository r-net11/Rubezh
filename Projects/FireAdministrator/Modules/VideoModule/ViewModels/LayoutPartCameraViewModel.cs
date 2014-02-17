using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Services.Layout;
using FiresecAPI.Models.Layouts;
using Infrastructure.Client.Layout.ViewModels;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartCameraProperties _properties;
		private LayoutPartPropertyPageViewModel _imagePage;

		public LayoutPartCameraViewModel(LayoutPartCameraProperties properties)
		{
			Title = "Камера";
			IconSource = LayoutPartDescription.IconPath + "BVideo.png";
			_сameraTitle = null;
			_properties = properties ?? new LayoutPartCameraProperties();
			_imagePage = new LayoutPartPropertyCameraPageViewModel(this);
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return _imagePage;
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
	}
}