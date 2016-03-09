using RubezhAPI.GK;
using RubezhAPI;

namespace GKWebService.Models.Door
{
	public class DoorStateClassToStringConverter
	{
		public static string Converter(XStateClass satate)
		{
			switch (satate)
			{
				case XStateClass.On:
					return "Открыто";

				case XStateClass.Off:
					return "Закрыто";

				case XStateClass.TurningOff:
					return "Закрывается";

				case XStateClass.Fire1:
					return "Тревога";
			}
			return satate.ToDescription();
		}
	}
}