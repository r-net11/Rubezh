using RubezhAPI.SKD;
using Infrastructure.Common.Windows;
using Infrastructure;

namespace SKDModule
{
	public static class DetailsValidateHelper
	{
		public static bool Validate(IOrganisationElement item)
		{
			if (item.Name != null && item.Name.Length > 50)
			{
				ServiceFactory.MessageBoxService.Show("Значение поля 'Название' не может быть длиннее 50 символов");
				return false;
			}
			if (item.Description != null && item.Description.Length > 50)
			{
				ServiceFactory.MessageBoxService.Show("Значение поля 'Примечание' не может быть длиннее 50 символов");
				return false;
			}
			return true;
		}
	}
}