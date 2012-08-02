using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class DirectionViewModel : BaseViewModel
    {
        public XDirection XDirection { get; set; }

		public DirectionViewModel(XDirection xDirection)
        {
			XDirection = xDirection;
        }

        public void Update()
        {
			OnPropertyChanged("XDirection");
        }
    }
}