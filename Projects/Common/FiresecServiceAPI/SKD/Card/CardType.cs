using System.ComponentModel;
using Localization;

namespace FiresecAPI.SKD
{
	public enum CardType
	{
		//[Description("Постоянный")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.CardType), "Constant")]
		Constant,

        //[Description("Временный")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.CardType), "Temporary")]
		Temporary,

        //[Description("Разовый")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.CardType), "OneTime")]
		OneTime,

        //[Description("Заблокирован")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.CardType), "Blocked")]
		Blocked,

        //[Description("Принуждение")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.CardType), "Duress")]
		Duress
	}
}