using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Resurs.ViewModels
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