using System;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;
using System.Collections.Generic;

namespace SKDDriver.DataClasses
{
	public class AccessTemplateTranslator : OrganisationItemTranslatorBase<AccessTemplate, API.AccessTemplate, API.AccessTemplateFilter>
	{
        public AccessTemplateTranslator(DbService context)
            : base(context)
        {
            AsyncTranslator = new AccessTemplateAsyncTranslator(this);
        }

        public AccessTemplateAsyncTranslator AsyncTranslator { get; private set; }

		public override DbSet<AccessTemplate> Table
		{
			get { return Context.AccessTemplates; }
		}

        public override IQueryable<AccessTemplate> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.CardDoors);
		}

		protected override OperationResult CanDelete(Guid uid)
		{
			if (Context.Cards.Any(x => x.AccessTemplateUID == uid))
				return new OperationResult("Невозможно удалить шаблон доступа, пока он указан у действующих карт");
			return base.CanDelete(uid);
		}

		public override API.AccessTemplate Translate(AccessTemplate tableItem)
		{
			var result = base.Translate(tableItem);
            if (result == null)
                return null;
			result.CardDoors = tableItem.CardDoors.Select(x => x.Translate()).ToList();

			return result;
		}

		public override void TranslateBack(API.AccessTemplate apiItem, AccessTemplate tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.CardDoors = apiItem.CardDoors.Select(x => new CardDoor(x)).ToList();
		}

		protected override void ClearDependentData(AccessTemplate tableItem)
		{
			Context.CardDoors.RemoveRange(tableItem.CardDoors);
		}
	}

    public class AccessTemplateAsyncTranslator : AsyncTranslator<AccessTemplate, API.AccessTemplate, API.AccessTemplateFilter>
    {
        public AccessTemplateAsyncTranslator(AccessTemplateTranslator translator) : base(translator as ITranslatorGet<AccessTemplate, API.AccessTemplate, API.AccessTemplateFilter>) { }
        public override List<API.AccessTemplate> GetCollection(DbCallbackResult callbackResult)
        {
            return callbackResult.AccessTemplates;
        }
    }
}
