using System;
using System.Collections.Generic;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypesViewModel : OrganisationBaseViewModel<AdditionalColumnType, AdditionalColumnTypeFilter, AdditionalColumnTypeViewModel, AdditionalColumnTypeDetailsViewModel>
	{
		protected override IEnumerable<AdditionalColumnType> GetModels(AdditionalColumnTypeFilter filter)
		{
			return AdditionalColumnTypeHelper.Get(filter);
		}
		protected override IEnumerable<AdditionalColumnType> GetModelsByOrganisation(Guid organisationUID)
		{
			return AdditionalColumnTypeHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(AdditionalColumnType model)
		{
			return AdditionalColumnTypeHelper.MarkDeleted(model);
		}
		protected override bool Restore(AdditionalColumnType model)
		{
			return AdditionalColumnTypeHelper.Restore(model);
		}
		protected override bool Add(AdditionalColumnType item)
		{
			var additionalColumnType = AdditionalColumnTypeHelper.GetDetails(_clipboardUID);
			additionalColumnType.UID = item.UID;
			additionalColumnType.Description = item.Description;
			additionalColumnType.Name = item.Name;
			additionalColumnType.OrganisationUID = item.OrganisationUID;
			return AdditionalColumnTypeHelper.Save(additionalColumnType, true);
		}

		protected override string ItemRemovingName
		{
			get { return "дополнительную колонку"; }
		}

		protected override void AfterRemove(AdditionalColumnType source)
		{
			ServiceFactory.Events.GetEvent<EditAdditionalColumnEvent>().Publish(null);
			ServiceFactory.Events.GetEvent<RemoveAdditionalColumnEvent>().Publish(source);
		}

		protected override void AfterRestore(AdditionalColumnType source)
		{
			ServiceFactory.Events.GetEvent<EditAdditionalColumnEvent>().Publish(null);
		}

		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_AdditionalColumns_Etit; }
		}

		protected override AdditionalColumnType CopyModel(AdditionalColumnType source)
		{
			var copy = base.CopyModel(source);
			copy.DataType = source.DataType;
			return copy;
		}
	}
}