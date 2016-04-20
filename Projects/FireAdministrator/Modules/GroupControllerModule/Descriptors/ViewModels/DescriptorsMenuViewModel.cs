using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DescriptorsMenuViewModel : BaseViewModel
	{
		public DescriptorsMenuViewModel(DescriptorsViewModel context)
		{
			Context = context;
		}

		public DescriptorsViewModel Context { get; private set; }
	}
}