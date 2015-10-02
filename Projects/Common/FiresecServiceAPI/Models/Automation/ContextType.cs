using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum ContextType
	{
		[Description("На клиенте")]
		Client,
		[Description("На сервере")]
		Server
	}
}
