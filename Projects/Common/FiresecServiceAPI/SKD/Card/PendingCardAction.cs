using System.ComponentModel;
namespace FiresecAPI.SKD
{
	public enum PendingCardAction
	{
		[Description("Добавление")]
		Add,

		[Description("Редактирование")]
		Edit,

		[Description("Удаление")]
		Delete
	}
}