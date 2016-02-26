using System;
using System.Data.Entity;
using System.Linq;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public class CardSynchroniser : Synchroniser<API.ExportCard, Card>
	{
		public CardSynchroniser(DbSet<Card> table, DbService databaseService) : base(table, databaseService) { }

		public override API.ExportCard Translate(Card item)
		{
			return new API.ExportCard
			{
				Number = item.Number,
				EndDate = item.EndDate,
				IsInStopList = item.IsInStopList,
				StopReason = item.StopReason,
				
				OrganisationUID = item.Employee != null ? GetUID(item.Employee.OrganisationUID) : Guid.Empty,
				OrganisationExternalKey = item.Employee != null ? GetExternalKey(item.Employee.OrganisationUID, item.Employee.Organisation) : "-1",
				EmployeeUID = GetUID(item.EmployeeUID),
				EmployeeExternalKey = GetExternalKey(item.EmployeeUID, item.Employee)
			};
		}

		public override void TranslateBack(API.ExportCard exportItem, Card tableItem)
		{
			tableItem.Number = exportItem.Number;
			tableItem.EndDate = exportItem.EndDate;
			tableItem.IsInStopList = exportItem.IsInStopList;
			tableItem.StopReason = exportItem.StopReason;
		}

		protected override IQueryable<Card> GetFilteredItems(API.ExportFilter filter)
		{
			return base.GetFilteredItems(filter).Where(x => x.Employee != null && x.Employee.OrganisationUID == filter.OrganisationUID);
		}

		protected override IQueryable<Card> GetTableItems()
		{
			return _Table.Include(x => x.Employee);
		}

		protected override string Name
		{
			get { return "Cards"; }
		}

		protected override string XmlHeaderName
		{
			get { return "ArrayOfExportCards"; }
		}
	}
}