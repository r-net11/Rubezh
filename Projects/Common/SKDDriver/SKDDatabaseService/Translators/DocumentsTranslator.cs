using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using System.Data.Linq;
using Common;
using System.Windows;
using System.Linq.Expressions;

namespace SKDDriver
{
	public class DocumentsTranslator : TranslatorBase<DataAccess.Document, Document, DocumentFilter>
	{
		public DocumentsTranslator(Table<DataAccess.Document> table)
			: base(table)
		{
			;
		}

		protected override void Verify(Document item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name && x.OrganizationUid == item.OrganizationUid && x.Uid != item.UID);
			if(sameName)
				throw new Exception("Попытка добавления документа с совпадающим именем");
			bool sameNo = Table.Any(x => x.No == item.No && x.OrganizationUid == item.OrganizationUid && x.Uid != item.UID);
			if (sameNo)
				throw new Exception("Попытка добавления документа с совпадающим номером");
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

		protected override DataAccess.Document TranslateBack(Document apiItem)
		{
			var result = base.TranslateBack(apiItem);
			result.Name = apiItem.Name;
			result.Description = apiItem.Description;
			result.IssueDate = apiItem.IssueDate;
			result.LaunchDate = apiItem.LaunchDate;
			result.No = apiItem.No;
			return result;
		}

		protected override bool IsInFilter(DataAccess.Document tableItem, DocumentFilter filter)
		{
			bool isInBase = base.IsInFilter(tableItem, filter);
			bool isInNos = IsInList<int>(tableItem.No, filter.Nos);
			bool isInNames = IsInList<string>(tableItem.Name, filter.Names);
			bool isInIsuueDates = IsInDateTimePeriod(tableItem.IssueDate, filter.IssueDate);
			bool isInLaunchDates = IsInDateTimePeriod(tableItem.LaunchDate, filter.LaunchDate);
			return isInBase && isInNos && isInNames && isInIsuueDates && isInLaunchDates;
		}

		protected override void Update(DataAccess.Document tableItem, Document apiItem)
		{
			base.Update(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.IssueDate = apiItem.IssueDate;
			tableItem.LaunchDate = apiItem.LaunchDate;
			tableItem.No = apiItem.No;
			Translator.Update(tableItem, apiItem);
		}
	}
}


