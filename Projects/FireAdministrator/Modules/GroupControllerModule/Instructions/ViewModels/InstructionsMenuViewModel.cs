
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class InstructionsMenuViewModel : BaseViewModel
	{
		public InstructionsMenuViewModel(InstructionsViewModel instructionsViewModel)
		{
			Context = instructionsViewModel;
		}

		public InstructionsViewModel Context { get; private set; }
	}
}