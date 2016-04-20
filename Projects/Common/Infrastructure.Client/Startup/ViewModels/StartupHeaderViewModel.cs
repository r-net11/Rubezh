using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Client.Startup.ViewModels
{
	public class StartupHeaderViewModel : BaseViewModel
	{
		public StartupHeaderViewModel(StartupViewModel content)
		{
			Content = content;
		}

		public StartupViewModel Content { get; private set; }
	}
}