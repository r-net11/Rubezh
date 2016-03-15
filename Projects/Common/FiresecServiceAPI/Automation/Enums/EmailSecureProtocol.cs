using System.ComponentModel;

namespace FiresecAPI.Automation.Enums
{
	public enum EmailSecureProtocol
	{
		[Description("Не используется")]
		None,

		[Description("SSL")]
		Ssl,

		[Description("TLS")]
		Tls
	}
}