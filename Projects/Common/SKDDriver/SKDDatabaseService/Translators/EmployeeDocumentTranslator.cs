using FiresecAPI;

namespace SKDDriver
{
	public class EmployeeDocumentTranslator : IsDeletedTranslator<DataAccess.EmployeeDocument, EmployeeDocument, EmployeeFilter>
	{
		public EmployeeDocumentTranslator(DataAccess.SKDDataContext context) 
			: base(context) { }

		protected override EmployeeDocument Translate(DataAccess.EmployeeDocument tableItem)
		{
			var result = base.Translate(tableItem);
			result.BirthDate = tableItem.BirthDate;
			result.BirthPlace = tableItem.BirthPlace;
			result.GivenBy = tableItem.GivenBy;
			result.GivenDate = tableItem.GivenDate;
			result.ValidTo = tableItem.ValidTo;
			result.Gender = (Gender)tableItem.Gender;
			result.DepartmentCode = tableItem.DepartmentCode;
			result.Citizenship = tableItem.Citizenship;
			result.Type = (EmployeeDocumentType)tableItem.Type;
			return result;
		}

		protected override void TranslateBack(DataAccess.EmployeeDocument tableItem, EmployeeDocument apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.BirthDate = apiItem.BirthDate;
			tableItem.BirthPlace = apiItem.BirthPlace;
			tableItem.GivenBy = apiItem.GivenBy;
			tableItem.GivenDate = apiItem.GivenDate;
			tableItem.ValidTo = apiItem.ValidTo;
			tableItem.Gender = (int)apiItem.Gender;
			tableItem.DepartmentCode = apiItem.DepartmentCode;
			tableItem.Citizenship = apiItem.Citizenship;
			tableItem.Type = (int)apiItem.Type;
		}
	}
}
