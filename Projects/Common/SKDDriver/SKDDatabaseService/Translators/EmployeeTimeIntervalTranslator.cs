using System.Linq;
using FiresecAPI;

namespace SKDDriver.Translators
{
	class EmployeeTimeIntervalTranslator : OrganizationElementTranslator<DataAccess.NamedInterval, EmployeeTimeInterval, EmployeeTimeIntervalFilter>
	{
		public EmployeeTimeIntervalTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override OperationResult CanSave(EmployeeTimeInterval item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganizationUID == item.OrganizationUID &&
				x.UID != item.UID &&
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Именнованный интервал с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override EmployeeTimeInterval Translate(DataAccess.NamedInterval tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			return result;
		}

		protected override void TranslateBack(DataAccess.NamedInterval tableItem, EmployeeTimeInterval apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
		}
	}
}
