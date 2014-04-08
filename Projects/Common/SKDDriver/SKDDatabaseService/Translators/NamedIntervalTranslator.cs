using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	class NamedIntervalTranslator : OrganizationElementTranslator<DataAccess.NamedInterval, NamedInterval, NamedIntervalFilter>
	{
		public NamedIntervalTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override OperationResult CanSave(NamedInterval item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganizationUID == item.OrganizationUID &&
				x.UID != item.UID &&
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Именнованный интервал с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override NamedInterval Translate(DataAccess.NamedInterval tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			return result;
		}

		protected override void TranslateBack(DataAccess.NamedInterval tableItem, NamedInterval apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
		}
	}
}
