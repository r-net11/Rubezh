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
	/// <summary>
	/// Методы Get и GetSingle не реализованы, вместо GetSingle - GetPassCardTemplate
	/// </summary>
	public class PassCardTemplateTranslator : OrganisationItemTranslatorBase<PassCardTemplate, API.PassCardTemplate, API.PassCardTemplateFilter>
	{
		DataContractSerializer _serializer;
		public PassCardTemplateShortTranslator ShortTranslator { get; private set; }

		public PassCardTemplateTranslator(DbService context)
			: base(context)
		{
			_serializer = new DataContractSerializer(typeof(API.PassCardTemplate));
			ShortTranslator = new PassCardTemplateShortTranslator(this);
			AsyncTranslator = new PassCardTemplateAsyncTranslator(ShortTranslator);
		}

		public PassCardTemplateAsyncTranslator AsyncTranslator { get; private set; }

		public override DbSet<PassCardTemplate> Table
		{
			get { return Context.PassCardTemplates; }
		}

		public OperationResult<API.PassCardTemplate> GetPassCardTemplate(Guid? uid)
		{
			try
			{
				if (uid == null)
					return new OperationResult<API.PassCardTemplate>();
				var tableItem = GetTableItems().FirstOrDefault(x => x.UID == uid.Value);
				if(tableItem == null)
					return new OperationResult<API.PassCardTemplate>();
				var result =Translate(tableItem);
				return new OperationResult<API.PassCardTemplate>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<API.PassCardTemplate>.FromError(e.Message);
			}
		}

		API.PassCardTemplate Translate(PassCardTemplate tableItem)
		{
			if (tableItem == null)
				return null;
			using (var ms = new MemoryStream(tableItem.Data.ToArray()))
			{
				var result = (API.PassCardTemplate)_serializer.ReadObject(ms);
				return result;
			}
		}

		public override void TranslateBack(API.PassCardTemplate apiItem, PassCardTemplate tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			using (var ms = new MemoryStream())
			{
				_serializer.WriteObject(ms, apiItem);
				tableItem.Data = ms.ToArray();
			}
		}

		protected override IEnumerable<API.PassCardTemplate> GetAPIItems(IQueryable<PassCardTemplate> tableItems)
		{
			throw new NotImplementedException();
		}
	}

	public class PassCardTemplateShortTranslator : OrganisationShortTranslatorBase<PassCardTemplate, API.ShortPassCardTemplate, API.PassCardTemplate, API.PassCardTemplateFilter>
	{
		public PassCardTemplateShortTranslator(PassCardTemplateTranslator translator) : base(translator) { }

		protected override IEnumerable<API.ShortPassCardTemplate> GetAPIItems(System.Linq.IQueryable<PassCardTemplate> tableItems)
		{
			return tableItems.Select(tableItem => new API.ShortPassCardTemplate
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty
			});
		}
	}

	public class PassCardTemplateAsyncTranslator : AsyncTranslator<PassCardTemplate, API.ShortPassCardTemplate, API.PassCardTemplateFilter>
	{
		public PassCardTemplateAsyncTranslator(PassCardTemplateShortTranslator translator) : base(translator as ITranslatorGet<PassCardTemplate, API.ShortPassCardTemplate, API.PassCardTemplateFilter>) { }
		public override List<API.ShortPassCardTemplate> GetCollection(DbCallbackResult callbackResult)
		{
			return callbackResult.PassCardTemplates;
		}
	}
}