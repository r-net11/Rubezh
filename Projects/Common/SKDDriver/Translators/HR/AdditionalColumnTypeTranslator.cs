using FiresecAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class AdditionalColumnTypeTranslator : OrganisationItemTranslatorBase<AdditionalColumnType, API.AdditionalColumnType, API.AdditionalColumnTypeFilter>
	{
		DataContractSerializer _serializer;

		public AdditionalColumnTypeTranslator(DbService context)
			: base(context)
		{
			_serializer = new DataContractSerializer(typeof(API.AdditionalColumnType));
			AsyncTranslator = new AdditionalColumnTypeAsyncTranslator(this);
		}

		public AdditionalColumnTypeAsyncTranslator AsyncTranslator { get; private set; }

		public override DbSet<AdditionalColumnType> Table
		{
			get { return Context.AdditionalColumnTypes; }
		}

		public override System.Linq.IQueryable<AdditionalColumnType> GetFilteredTableItems(API.AdditionalColumnTypeFilter filter, System.Linq.IQueryable<AdditionalColumnType> tableItems)
		{
			var filteredTableItems = base.GetFilteredTableItems(filter, tableItems);
			if(filter.Type.HasValue)
			{
				filteredTableItems = filteredTableItems.Where(x => x.DataType == (int)filter.Type.Value);
			}
			return filteredTableItems;
		}

		public override void TranslateBack(API.AdditionalColumnType apiItem, AdditionalColumnType tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			using (var ms = new MemoryStream())
			{
				_serializer.WriteObject(ms, apiItem);
				tableItem.DataType = (int)apiItem.DataType;
				tableItem.PersonType = (int)apiItem.PersonType;
				tableItem.IsInGrid = apiItem.IsInGrid;
			}
		}

		protected override IEnumerable<API.AdditionalColumnType> GetAPIItems(System.Linq.IQueryable<AdditionalColumnType> tableItems)
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

	public class AdditionalColumnTypeAsyncTranslator : AsyncTranslator<AdditionalColumnType, API.AdditionalColumnType, API.AdditionalColumnTypeFilter>
	{
		public AdditionalColumnTypeAsyncTranslator(AdditionalColumnTypeTranslator translator) : base(translator as ITranslatorGet<AdditionalColumnType, API.AdditionalColumnType, API.AdditionalColumnTypeFilter>) { }
		public override List<API.AdditionalColumnType> GetCollection(DbCallbackResult callbackResult)
		{
			return callbackResult.AdditionalColumnTypes;
		}
	}
}