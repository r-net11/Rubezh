using System;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public class DocumentTranslator : OrganizationElementTranslator<DataAccess.Document, Document, DocumentFilter>
	{
		public DocumentTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override OperationResult CanSave(Document item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganizationUID == item.OrganizationUID &&
				x.UID != item.UID &&
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Документ с таким же именем уже содержится в базе данных");
			if (item.No <= 0)
				return new OperationResult("Номер добавляемого документа должен быть положительным числом");
			bool sameNo = Table.Any(x => x.No == item.No &&
				x.OrganizationUID == item.OrganizationUID &&
				x.UID != item.UID &&
				x.IsDeleted == false);
			if (sameNo)
				return new OperationResult("Документ с таким же номером уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override Document Translate(DataAccess.Document tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.IssueDate = tableItem.IssueDate;
			result.LaunchDate = tableItem.LaunchDate;
			result.No = tableItem.No;
			return result;
		}

		protected override void TranslateBack(DataAccess.Document tableItem, Document apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.IssueDate = CheckDate(apiItem.IssueDate);
			tableItem.LaunchDate = CheckDate(apiItem.LaunchDate);
			tableItem.No = apiItem.No;
		}

		protected override Expression<Func<DataAccess.Document, bool>> IsInFilter(DocumentFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Document>();
			result = result.And(base.IsInFilter(filter));
			var nos = filter.Nos;
			if (nos != null && nos.Count != 0)
				result = result.And(e => nos.Contains(e.No));
			var names = filter.Names;
			if (names != null && names.Count != 0)
				result = result.And(e => names.Contains(e.Name));
			var issueDates = filter.IssueDate;
			if (issueDates != null)
				result = result.And(e => e.RemovalDate >= issueDates.StartDate && e.RemovalDate <= issueDates.EndDate);
			var launchDates = filter.LaunchDate;
			if (launchDates != null)
				result = result.And(e => e.RemovalDate >= launchDates.StartDate && e.RemovalDate <= launchDates.EndDate);
			return result;
		}

	}
}