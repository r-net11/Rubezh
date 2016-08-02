using System.ComponentModel;

namespace StrazhAPI.Automation.Enums
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