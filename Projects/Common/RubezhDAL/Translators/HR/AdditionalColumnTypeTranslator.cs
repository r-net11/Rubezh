using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public class AdditionalColumnTypeTranslator : OrganisationItemTranslatorBase<AdditionalColumnType, API.AdditionalColumnType, API.AdditionalColumnTypeFilter>
	{
		public AdditionalColumnTypeTranslator(DbService context)
			: base(context)
		{
		}

		public override DbSet<AdditionalColumnType> Table
		{
			get { return Context.AdditionalColumnTypes; }
		}

		public override IQueryable<AdditionalColumnType> GetFilteredTableItems(API.AdditionalColumnTypeFilter filter, IQueryable<AdditionalColumnType> tableItems)
		{
			var filteredTableItems = base.GetFilteredTableItems(filter, tableItems);
			if(filter.Type.HasValue)
				filteredTableItems = filteredTableItems.Where(x => x.DataType == (int)filter.Type.Value);
			if(filter.IsInGrid != null)
				filteredTableItems = filteredTableItems.Where(x => x.IsInGrid);
			return filteredTableItems;
		}

		public override void TranslateBack(API.AdditionalColumnType apiItem, AdditionalColumnType tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
		}

		protected override IEnumerable<API.AdditionalColumnType> GetAPIItems(IQueryable<AdditionalColumnType> tableItems)
		{
			return tableItems.Select(tableItem => new API.AdditionalColumnType
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty,
				DataType = (API.AdditionalColumnDataType)tableItem.DataType,
				PersonType = (API.PersonType)tableItem.PersonType,
				IsInGrid = tableItem.IsInGrid
			});
		}
	}
}