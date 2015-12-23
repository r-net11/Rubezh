using System;
using System.Collections.Generic;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using SKDModule.ViewModels;
using Infrastructure;
using SKDModule.Events;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class PassCardTemplatesViewModel : OrganisationBaseViewModel<ShortPassCardTemplate, PassCardTemplateFilter, PassCardTemplateViewModel, PassCardTemplateDetailsViewModel>
	{
		public PassCardTemplatesViewModel()
			: base()
		{
			ServiceFactory.Events.GetEvent<RemoveAdditionalColumnEvent>().Unsubscribe(RemoveAdditionalColumn);
			ServiceFactory.Events.GetEvent<RemoveAdditionalColumnEvent>().Subscribe(RemoveAdditionalColumn);
		}

		protected override IEnumerable<ShortPassCardTemplate> GetModels(PassCardTemplateFilter filter)
		{
			return PassCardTemplateHelper.Get(filter);
		}
		protected override IEnumerable<ShortPassCardTemplate> GetModelsByOrganisation(Guid organisationUID)
		{
			return PassCardTemplateHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(ShortPassCardTemplate model)
		{
			return PassCardTemplateHelper.MarkDeleted(model);
		}
		protected override bool Restore(ShortPassCardTemplate model)
		{
			return PassCardTemplateHelper.Restore(model);
		}
		protected override bool Add(ShortPassCardTemplate item)
		{
			var passCardTemplate = PassCardTemplateHelper.GetDetails(_clipboardUID);
			passCardTemplate.UID = item.UID;
			passCardTemplate.Description = item.Description;
			passCardTemplate.Caption = item.Name;
			passCardTemplate.OrganisationUID = item.OrganisationUID;
			return PassCardTemplateHelper.Save(passCardTemplate, true);
		}

		protected override string ItemRemovingName
		{
			get { return "шаблон пропуска"; }
		}

		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_PassCards_Etit; }
		}

		public void RemoveAdditionalColumn(AdditionalColumnType column)
		{
			var filter = new PassCardTemplateFilter();
			var organisations = GetModels(filter);
			foreach (var item in organisations)
			{
				var PassCardTemplate = PassCardTemplateHelper.GetDetails(item.UID);
				switch (column.DataType)
				{
					case AdditionalColumnDataType.Text:
						PassCardTemplate.ElementTextProperties = PassCardTemplate.ElementTextProperties.FindAll(x => x.AdditionalColumnUID != column.UID);
						break;
					case AdditionalColumnDataType.Graphics:
						PassCardTemplate.ElementImageProperties = PassCardTemplate.ElementImageProperties.FindAll(x => x.AdditionalColumnUID != column.UID);
						break;
				}
				PassCardTemplateHelper.Save(PassCardTemplate, false);
			}
		}
	}
}