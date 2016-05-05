using System.ComponentModel;
using LocalizationConveters;

namespace FiresecAPI.Automation.Enums
{
	public enum EmailSecureProtocol
	{
		//[Description("Не используется")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.EmailSecureProtocol), "None")]
		None,

		//[Description("SSL")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.EmailSecureProtocol), "Ssl")]
		Ssl,

		//[Description("TLS")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.EmailSecureProtocol), "Tls")]
		Tls
	}
}