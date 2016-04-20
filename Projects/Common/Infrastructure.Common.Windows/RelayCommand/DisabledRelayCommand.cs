
namespace Infrastructure.Common.Windows
{
	public static class DisabledRelayCommand
	{
		public static RelayCommand Instance { get; private set; }
		static DisabledRelayCommand()
		{
			Instance = new RelayCommand(() => { }, () => false);
		}
	}
}
