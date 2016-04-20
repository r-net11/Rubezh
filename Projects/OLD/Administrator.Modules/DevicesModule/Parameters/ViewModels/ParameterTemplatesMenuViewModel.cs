using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
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