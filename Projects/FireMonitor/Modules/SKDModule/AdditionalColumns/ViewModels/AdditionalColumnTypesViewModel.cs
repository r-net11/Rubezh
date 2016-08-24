using System;
using System.Collections.Generic;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypesViewModel : OrganisationBaseViewModel<ShortAdditionalColumnType, AdditionalColumnTypeFilter, AdditionalColumnTypeViewModel, AdditionalColumnTypeDetailsViewModel>
	{
		protected override IEnumerable<ShortAdditionalColumnType> GetModels(AdditionalColumnTypeFilter filter)
		{
			return AdditionalColumnTypeHelper.Get(filter);
		}
		protected override IEnumerable<ShortAdditionalColumnType> GetModelsByOrganisation(Guid organisationUID)
		{
			return AdditionalColumnTypeHelper.GetShortByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(ShortAdditionalColumnType model)
		{
			return AdditionalColumnTypeHelper.MarkDeleted(model);
		}
		protected override bool Restore(ShortAdditionalColumnType model)
		{
			return AdditionalColumnTypeHelper.Restore(model);
		}
		protected override bool Add(ShortAdditionalColumnType item)
		{
			var additionalColumnType = AdditionalColumnTypeHelper.GetDetails(ClipboardUID);
			additionalColumnType.UID = item.UID;
			additionalColumnType.Description = item.Description;
			additionalColumnType.Name = item.Name;
			additionalColumnType.OrganisationUID = item.OrganisationUID;
			return AdditionalColumnTypeHelper.Save(additionalColumnType, true);
		}

		protected override string ItemRemovingName
		{
			get { return CommonViewModels.AdditionalColumn; }
		}

		protected override void AfterRemove(ShortAdditionalColumnType source)
		{
			ServiceFactory.Events.GetEvent<EditAdditionalColumnEvent>().Publish(null);
		}

		protected override void AfterRestore(ShortAdditionalColumnType source)
		{
			ServiceFactory.Events.GetEvent<EditAdditionalColumnEvent>().Publish(null);
		}

		protected override StrazhAPI.Models.PermissionType Permission
		{
			get { return StrazhAPI.Models.PermissionType.Oper_SKD_AdditionalColumns_Etit; }
		}

		protected override ShortAdditionalColumnType CopyModel(ShortAdditionalColumnType source)
		{
			var copy = base.CopyModel(source);
			copy.DataType = source.DataType;
			return copy;
		}
	}
}