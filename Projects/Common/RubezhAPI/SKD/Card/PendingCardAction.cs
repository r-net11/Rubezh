using System.ComponentModel;
namespace RubezhAPI.SKD
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