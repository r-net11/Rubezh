﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class AccessTemplatesViewModel : OrganisationBaseViewModel<AccessTemplate, AccessTemplateFilter, AccessTemplateViewModel, AccessTemplateDetailsViewModel>, ICardDoorsParentList<AccessTemplateViewModel>
	{
		public override void Initialize(AccessTemplateFilter filter)
		{
			base.Initialize(filter);
			_updateOrganisationDoorsEventSubscriber = new UpdateOrganisationDoorsEventSubscriber<AccessTemplateViewModel>(this);
		}

		UpdateOrganisationDoorsEventSubscriber<AccessTemplateViewModel> _updateOrganisationDoorsEventSubscriber;

		protected override IEnumerable<AccessTemplate> GetModels(AccessTemplateFilter filter)
		{
			return AccessTemplateHelper.Get(filter);
		}

		protected override void UpdateSelected()
		{
			if (SelectedItem != null && !SelectedItem.IsOrganisation && SelectedItem.CardDoorsViewModel != null && SelectedItem.CardDoorsViewModel.Doors != null)
				SelectedItem.CardDoorsViewModel.Doors.Sort(x => x.No);
		}

		protected override IEnumerable<AccessTemplate> GetModelsByOrganisation(Guid organisationUID)
		{
			return AccessTemplateHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(AccessTemplate model)
		{
			return AccessTemplateHelper.MarkDeleted(model);
		}
		protected override bool Restore(AccessTemplate model)
		{
			return AccessTemplateHelper.Restore(model);
		}
		protected override bool Add(AccessTemplate item)
		{
			return AccessTemplateHelper.Save(item, true);
		}
		protected override AccessTemplate CopyModel(AccessTemplate source)
		{
			var copy = base.CopyModel(source);
			foreach (var cardDoor in source.CardDoors)
			{
				var copyCardDoor = new CardDoor
				{
					DoorUID = cardDoor.DoorUID,
					EnterScheduleNo = cardDoor.EnterScheduleNo,
					CardUID = null,
					AccessTemplateUID = null
				};
				copy.CardDoors.Add(copyCardDoor);
			}
			copy.CardDoors.ForEach(x => x.AccessTemplateUID = copy.UID);
			return copy;
		}

		protected override bool CanPaste()
		{
			return base.CanPaste() && ParentOrganisation.Organisation.UID == _clipboard.OrganisationUID;
		}

		protected override string ItemRemovingName
		{
			get { return "шаблон доступа"; }
		}

		protected override FiresecAPI.Models.PermissionType Permission
		{
			get { return FiresecAPI.Models.PermissionType.Oper_SKD_AccessTemplates_Etit; }
		}

		public List<AccessTemplateViewModel> DoorsParents
		{
			get { return Organisations.SelectMany(x => x.Children).ToList(); }
		}
	}
}