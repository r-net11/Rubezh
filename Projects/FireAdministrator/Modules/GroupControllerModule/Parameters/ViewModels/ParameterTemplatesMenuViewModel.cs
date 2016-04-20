using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ParameterTemplatesMenuViewModel : BaseViewModel
	{
		public ParameterTemplatesMenuViewModel(ParameterTemplatesViewModel context)
		{
			Context = context;
		}

		public ParameterTemplatesViewModel Context { get; private set; }
	}
}