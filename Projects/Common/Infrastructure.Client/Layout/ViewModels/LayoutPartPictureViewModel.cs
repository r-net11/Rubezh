using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Client.Layout.ViewModels
{
	public class LayoutPartPictureViewModel : BaseViewModel
	{
		string _imageSource;
		public string ImageSource
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
