using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class AccessTemplatesViewModel : OrganisationBaseViewModel<AccessTemplate, AccessTemplateFilter, AccessTemplateViewModel, AccessTemplateDetailsViewModel>, ICardDoorsParentList<AccessTemplateViewModel>
	{
		public RelayCommand ApplyToUserGroupCommand { get; private set; }
		private void OnApplyToUserGroup()
		{
			var organisationID = Guid.Empty;
			var accessTemplateID = Guid.Empty;

			if (SelectedItem != null)
			{
				organisationID = SelectedItem.OrganisationUID;
				if (!SelectedItem.IsOrganisation)
					accessTemplateID = SelectedItem.UID;
			}
			var dialog = new ApplyAccessTemplateToUserGroupViewModel(organisationID, accessTemplateID);
			if (DialogService.ShowModalWindow(dialog))
			{
			}
		}
		private bool CanApplyToUserGroup()
		{
			return SelectedItem != null;
		}

		public override void Initialize(AccessTemplateFilter filter)
		{
			base.Initialize(filter);
			_updateOrganisationDoorsEventSubscriber = new UpdateOrganisationDoorsEventSubscriber<AccessTemplateViewModel>(this);
			ApplyToUserGroupCommand = new RelayCommand(OnApplyToUserGroup, CanApplyToUserGroup);
		}

		private UpdateOrganisationDoorsEventSubscriber<AccessTemplateViewModel> _updateOrganisationDoorsEventSubscriber;

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

		protected override void AfterRemove(AccessTemplate model)
		{
			SelectedItem.CardDoorsViewModel.UpdateDoors(new List<Guid>());
			SelectedItem.CardDoorsViewModel.Update(null);
			model.CardDoors = new List<CardDoor>();
		}

		protected override bool Restore(AccessTemplate model)
		{
			return AccessTemplateHelper.Restore(model);
		}

		protected override void AfterRestore(AccessTemplate model)
		{
			base.AfterRestore(model);
			model.IsDeleted = false;
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
			return base.CanPaste() && ParentOrganisation.Organisation.UID == Clipboard.OrganisationUID;
		}

		protected override string ItemRemovingName
		{
			get { return "шаблон доступа"; }
		}

		protected override StrazhAPI.Models.PermissionType Permission
		{
			get { return StrazhAPI.Models.PermissionType.Oper_SKD_AccessTemplates_Etit; }
		}

		public List<AccessTemplateViewModel> DoorsParents
		{
			get { return Organisations.SelectMany(x => x.Children).ToList(); }
		}
	}
}