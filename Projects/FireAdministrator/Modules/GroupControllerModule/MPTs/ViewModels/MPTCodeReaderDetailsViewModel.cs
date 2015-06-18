using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class MPTCodeReaderDetailsViewModel : SaveCancelDialogViewModel
	{
		public string CodeName { get; private set; }
		public GKMPTDeviceType MPTDeviceType { get; private set; }
	}
}