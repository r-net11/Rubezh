using RubezhAPI.GK;
using RubezhAPI;

namespace GKWebService.Models.GuardZones
{
	public class GuardZoneStateClassToStringConverter
	{
		public static string Converter(XStateClass satate)
		{
			switch (satate)
			{
				case XStateClass.On:
					return "На Охране";

				case XStateClass.Off:
					return "Снята с охраны";

				case XStateClass.TurningOn:
					return "Ставится на охрану";

				case XStateClass.TurningOff:
					return "Снимается с охраны";

				case XStateClass.Fire1:
					return "Тревога";
			}
			return satate.ToDescription();
		}
	}
}