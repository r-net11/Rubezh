using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Метод ввода охранной зоны
	/// </summary>
	public enum GKGuardZoneEnterMethod
	{
		[Description("Только по глобальному паролю")]
		GlobalOnly,

		[Description("Только по индивидуальному паролю")]
		UserOnly,

		[Description("По любому паролю")]
		Both
	}
}