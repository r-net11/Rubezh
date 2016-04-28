using FiresecAPI;
using FiresecAPI.SKD;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace SKDDriver
{
	public class PassCardTemplateTranslator : WithShortTranslator<DataAccess.PassCardTemplate, PassCardTemplate, PassCardTemplateFilter, ShortPassCardTemplate>
	{
		private DataContractSerializer _serializer;

		public PassCardTemplateTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
			_serializer = new DataContractSerializer(typeof(PassCardTemplate));
		}

		protected override OperationResult CanSave(PassCardTemplate item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			bool hasSameName = Table.Any(x => x.Name == item.Caption &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (hasSameName)
                return new OperationResult(Resources.Language.Translators.PassCardTemplateTranslator.CanSave_Error);
			return new OperationResult();
		}

		protected override OperationResult CanDelete(Guid uid)
		{
			return base.CanDelete(uid);
		}

		protected override PassCardTemplate Translate(DataAccess.PassCardTemplate tableItem)
		{
			using (var ms = new MemoryStream(tableItem.Data.ToArray()))
				return (PassCardTemplate)_serializer.ReadObject(ms);
		}

		protected override void TranslateBack(DataAccess.PassCardTemplate tableItem, PassCardTemplate apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Caption;
			tableItem.Description = apiItem.Description;
			using (var ms = new MemoryStream())
			{
				_serializer.WriteObject(ms, apiItem);
				tableItem.Data = ms.ToArray();
			}
		}

		protected override ShortPassCardTemplate TranslateToShort(DataAccess.PassCardTemplate tableItem)
		{
			var result = base.TranslateToShort(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			return result;
		}
	}
}