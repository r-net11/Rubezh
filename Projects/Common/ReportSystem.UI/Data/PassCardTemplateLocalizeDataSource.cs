using System;
using DevExpress.Data;

namespace ReportSystem.UI.Data
{
	/// <summary>
	/// Класс, отвечающий за перевод полей источника данных DataSource в дизайнере  пропусков.
	/// </summary>
	public class PassCardTemplateLocalizeDataSource : PassCardTemplateSource, IDisplayNameProvider
	{
		public string GetFieldDisplayName(string[] fieldAccessors)
		{
			var fieldName = fieldAccessors[fieldAccessors.Length - 1];

			switch (fieldName)
			{
				case "Photo":
					return "Фото";
				case "OrganisationLogo":
					return "Логотип организации";
				case "DepartmentLogo":
					return "Логотип подразделения";
				case "PositionLogo":
					return "Логотип должности";
				case "Employee":
					return "Сотрудники";
				case "LastName":
					return "Фамилия";
				case "FirstName":
					return "Имя";
				case "MiddleName":
					return "Отчество";
				case "OrganisationName":
					return "Организация";
				case "DepartmentName":
					return "Подразделение";
				case "PositionName":
					return "Должность";
				case "ExcortName":
					return "Сопровождающий";
				case "ScheduleName":
					return "График работы";
				case "Description":
					return "Примечание";
				case "Phone":
					return "Телефон";
				case "CardNo":
					return "Номер пропуска";
				case "TabelNo":
					return "Табельный номер";
				case "UID":
					return null;
			}

			return fieldName;
			//	return ChangeNames(fieldName);
		}

		public string GetDataSourceDisplayName()
		{
			return "Источник данных";
		}

		//private string ChangeNames(string name)
		//{
		//	var result = string.Empty;
		//	var isPrevLow = false;

		//	foreach (var symb in name)
		//	{
		//		if (Char.IsUpper(symb) && isPrevLow)
		//		{
		//			result += " " + symb;
		//		}
		//		else
		//		{
		//			result += symb;
		//		}

		//		isPrevLow = Char.IsLower(symb);
		//	}

		//	return result;
		//}
	}
}
