using System.ComponentModel;

namespace RubezhAPI.Automation
{
	public enum ContextType
	{
		[Description("На клиенте")]
		Client,
		[Description("На сервере")]
		Server
	}
}
