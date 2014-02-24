using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class MPTViewModel : BaseViewModel
    {
		public XMPT MPT { get; set; }

		public MPTViewModel(XMPT mpt)
		{
			MPT = mpt;
		}

		public void Update()
		{
			OnPropertyChanged("MPT");
		}
    }
}