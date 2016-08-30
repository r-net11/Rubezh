using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data;

namespace ReportSystem.DataSets
{
	public class PassCardTemplateLocalizeDataSource : PassCardTemplateDataSource, IDisplayNameProvider
	{
		public string GetFieldDisplayName(string[] fieldAccessors)
		{
			//Get a field name form the data member's name.
			var fieldName = fieldAccessors[fieldAccessors.Length - 1];

			//Hide the data member if its name ends with 'LastName'
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

			return ChangeNames(fieldName);
		}

		public string GetDataSourceDisplayName()
		{
			return "Источник данных";
		}

		private string ChangeNames(string name)
		{
			var result = string.Empty;
			var isPrevLow = false;

			foreach (var symb in name)
			{
				//Check if a character is of upper case.
				//To avoid spaces inside abbreviations,
				//check if the previous character is of upper case, too.
				if (Char.IsUpper(symb) && isPrevLow)
				{
					result += " " + symb;
				}
				else
				{
					result += symb;
				}

				isPrevLow = Char.IsLower(symb);
			}

			return result;
		}
	}
}
