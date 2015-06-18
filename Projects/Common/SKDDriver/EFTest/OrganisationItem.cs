
namespace SKDDriver.DataClasses
{
	public class OrganisationItem<TApiItem, TTableItem>
		where TApiItem : FiresecAPI.SKD.IOrganisationElement, new()
		where TTableItem : IOrganisationItem, new()
	{
		TTableItem _tableItem;

		public OrganisationItem(TTableItem tableItem)
		{
			_tableItem = tableItem;
		}

		public TApiItem Translate()
		{
			return new TApiItem
			{
				UID = _tableItem.UID,
				Name = _tableItem.Name,
				Description = _tableItem.Description,
				IsDeleted = _tableItem.IsDeleted,
				RemovalDate = _tableItem.RemovalDate.GetValueOrDefault(),
				OrganisationUID = _tableItem.OrganisationUID.GetValueOrDefault()
			};
		}

		public void TranslateBack(TApiItem apiItem, TTableItem tableItem)
		{
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.IsDeleted = apiItem.IsDeleted;
			tableItem.RemovalDate = apiItem.RemovalDate;
			tableItem.OrganisationUID = apiItem.OrganisationUID;
		}

		public TTableItem Initialize(TApiItem item)
		{
			var result = new TTableItem();
			result.UID = item.UID;
			TranslateBack(item, result);
			return result;
		}
	}
}
