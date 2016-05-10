using StrazhAPI;
using StrazhAPI.SKD;
using LinqKit;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public class AccessTemplateTranslator : OrganisationElementTranslator<DataAccess.AccessTemplate, AccessTemplate, AccessTemplateFilter>
	{
		public AccessTemplateTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override OperationResult CanSave(AccessTemplate accessTemplate)
		{
			var result = base.CanSave(accessTemplate);
			if (result.HasError)
				return result;
			bool hasSameName = Table.Any(x => x.Name == accessTemplate.Name &&
				x.OrganisationUID == accessTemplate.OrganisationUID &&
				x.UID != accessTemplate.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return new OperationResult(Resources.Language.Translators.AccessTemplateTranslator.CanSave_Error);
			else
				return new OperationResult();
		}

		protected override OperationResult CanDelete(Guid uid)
		{
			if (Context.Cards.Any(x => x.AccessTemplateUID == uid))
                return new OperationResult(Resources.Language.Translators.AccessTemplateTranslator.CanDelete_Error);
			return base.CanDelete(uid);
		}

		public override OperationResult MarkDeleted(Guid uid)
		{
			var deleteDoorsResult = DatabaseService.CardDoorTranslator.RemoveFromAccessTemplate(uid);
			if (deleteDoorsResult.HasError)
				return deleteDoorsResult;
			return base.MarkDeleted(uid);
		}

		protected override AccessTemplate Translate(DataAccess.AccessTemplate tableItem)
		{
			var result = base.Translate(tableItem);
			result.CardDoors = DatabaseService.CardDoorTranslator.GetForAccessTemplate(tableItem.UID);
			result.DeactivatingReaders = DatabaseService.AccessTemplateDeactivatingReaderTranslator.GetForAccessTemplate(tableItem.UID);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			return result;
		}

		protected override void TranslateBack(DataAccess.AccessTemplate tableItem, AccessTemplate apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
		}

		/// <summary>
		/// Сохраняет шаблон доступа
		/// </summary>
		/// <param name="item">Шаблон доступа</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public override OperationResult Save(AccessTemplate item)
		{
			// В базе "SKD" из таблицы "CardDoor" удаляем все записи ссылающиеся на данный шаблон доступа
			var updateCardDoorsResult = DatabaseService.CardDoorTranslator.RemoveFromAccessTemplate(item.UID);
			// В базе "SKD" из таблицы "AccessTemplateDeactivatingReader" удаляем все записи ссылающиеся на данный шаблон доступа
			var updateAccessTemplateDeactivatingReaderResult = DatabaseService.AccessTemplateDeactivatingReaderTranslator.RemoveFromAccessTemplate(item.UID);
			// Сохраняем шаблон доступа
			var result = base.Save(item);
			// В базе "SKD" в таблицу "CardDoor" сохраняем все записи содержащиеся в данном шаблоне доступа
			DatabaseService.CardDoorTranslator.Save(item.CardDoors);
			// В базе "SKD" в таблицу "AccessTemplateDeactivatingReader" сохраняем все записи содержащиеся в данном шаблоне доступа
			DatabaseService.AccessTemplateDeactivatingReaderTranslator.Save(item.DeactivatingReaders);

			return result;
		}

		protected override Expression<Func<DataAccess.AccessTemplate, bool>> IsInFilter(AccessTemplateFilter filter)
		{
			var result = base.IsInFilter(filter);
			var names = filter.Names;
			if (names != null && names.Count != 0)
				result = result.And(e => names.Contains(e.Name));
			return result;
		}
	}
}