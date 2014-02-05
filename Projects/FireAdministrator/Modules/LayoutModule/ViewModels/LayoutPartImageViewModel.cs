using System.Collections.Generic;
using System.Windows.Media;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;

namespace LayoutModule.ViewModels
{
	public class LayoutPartImageViewModel : BaseLayoutPartViewModel
	{
		private LayoutPartImageProperties _properties;
		private LayoutPartPropertyPageViewModel _imagePage;
		public LayoutPartImageViewModel(LayoutPartImageProperties properties)
		{
			_imageSource = null;
			_properties = properties ?? new LayoutPartImageProperties();
			_imagePage = new LayoutPartPropertyImagePageViewModel(this);
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

		private Stretch _stretch;
		public Stretch Stretch
		{
			get { return _stretch; }
			set
			{
				_stretch = value;
				OnPropertyChanged(() => Stretch);
			}
		}
		private ImageSource _imageSource;
		public ImageSource ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged(() => ImageSource);
			}
		}
	}
}
