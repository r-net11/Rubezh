namespace StrazhDAL
{
	//public class CardSynchroniser : Synchroniser<ExportCard, DataAccess.Card>
	//{
	//	public CardSynchroniser(Table<DataAccess.Card> table, SKDDatabaseService databaseService) : base(table, databaseService) { }

	//	public override ExportCard Translate(DataAccess.Card item)
	//	{
	//		return new ExportCard
	//		{
	//			Number = item.Number,
	//			CardType = item.CardType != null ? item.CardType.Value : -1,
	//			StartDate = item.StartDate,
	//			EndDate = item.EndDate,
	//			IsInStopList = item.IsInStopList,
	//			StopReason = item.StopReason,
	//			Password = item.Password,
	//			UserTime = item.UserTime,

	//			OrganisationUID = item.Employee != null ? GetUID(item.Employee.OrganisationUID) : Guid.Empty,
	//			OrganisationExternalKey = item.Employee != null ? GetExternalKey(item.Employee.OrganisationUID, item.Employee.Organisation) : "-1",
	//			EmployeeUID = GetUID(item.EmployeeUID),
	//			EmployeeExternalKey = GetExternalKey(item.EmployeeUID, item.Employee)
	//		};
	//	}

	//	public override void TranslateBack(ExportCard exportItem, DataAccess.Card tableItem)
	//	{
	//		tableItem.Number = exportItem.Number;
	//		tableItem.CardType = exportItem.CardType;
	//		tableItem.StartDate = exportItem.StartDate;
	//		tableItem.EndDate = exportItem.EndDate;
	//		tableItem.IsInStopList = exportItem.IsInStopList;
	//		tableItem.StopReason = exportItem.StopReason;
	//		tableItem.Password = exportItem.Password;
	//		tableItem.UserTime = exportItem.UserTime;
	//	}

	//	protected override Expression<Func<DataAccess.Card, bool>> IsInFilter(Guid uid)
	//	{
	//		return base.IsInFilter(uid).And(x => x.Employee != null && x.Employee.OrganisationUID == uid);
	//	}

	//	protected override string Name
	//	{
	//		get { return "Cards"; }
	//	}
	//}
}