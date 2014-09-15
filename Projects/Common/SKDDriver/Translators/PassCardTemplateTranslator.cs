using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using System.Runtime.Serialization;
using System.IO;

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

		protected override OperationResult CanSave(PassCardTemplate PassCardTemplate)
		{
			bool hasSameName = Table.Any(x => x.Name == PassCardTemplate.Caption &&
				x.OrganisationUID == PassCardTemplate.OrganisationUID &&
				x.UID != PassCardTemplate.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return new OperationResult("Попытка добавления шаблон пропуска с совпадающим наименованием");
			return base.CanSave(PassCardTemplate);
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
			var shortPassCardTemplate = new ShortPassCardTemplate
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				OrganisationUID = tableItem.OrganisationUID.HasValue ? tableItem.OrganisationUID.Value : Guid.Empty
			};
			return shortPassCardTemplate;
		}
	}
}