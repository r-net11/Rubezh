using FiresecAPI;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
		
		public override API.AdditionalColumnType Translate(AdditionalColumnType tableItem)
		{
			var result = base.Translate(tableItem);
            if (result == null)
                return null;
			result.DataType = (API.AdditionalColumnDataType)tableItem.DataType;
			result.PersonType = (API.PersonType)tableItem.PersonType;
			result.IsInGrid = tableItem.IsInGrid;
			return result;
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