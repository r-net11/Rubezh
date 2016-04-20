using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using System.Text;
using Infrastructure.Common.Windows.Windows;
using Infrastructure;
using SKDModule.Events;

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
				var copyCardDoor = new CardDoor();
				copyCardDoor.DoorUID = cardDoor.DoorUID;
				copyCardDoor.EnterScheduleNo = cardDoor.EnterScheduleNo;
				copyCardDoor.ExitScheduleNo = cardDoor.ExitScheduleNo;
				copyCardDoor.CardUID = null;
				copyCardDoor.AccessTemplateUID = null;
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

		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_AccessTemplates_Etit; }
		}

		public List<AccessTemplateViewModel> DoorsParents
		{
			get { return Organisations.SelectMany(x => x.Children).Where(x => x.CardDoorsViewModel != null).ToList(); }
		}

		protected override void UpdateSelected()
		{
			foreach (var item in Organisations.SelectMany(x => x.GetAllChildren()))
			{
				item.CardDoorsViewModel = null;
			}
			if (SelectedItem != null)
			{
				SelectedItem.InitializeDoors();
			}
		}

		protected override bool ShowRemovingQuestion()
		{
			var cards = CardHelper.GetOrganisationCards(ParentOrganisation.UID);
			if(cards == null)
				return false;
			var linkedCards = cards.Where(x => x.AccessTemplateUID == SelectedItem.Model.UID);
			if (linkedCards.Count() > 0)
			{
				var numbers = linkedCards.Select(x => x.Number).OrderBy(x => x);
				var numbersSting = string.Join(",", numbers);
				var message = string.Format("Шаблон привязан к пропускам номер {0}. При удалении шаблона указанные в нём точки доступа будут убраны из привязаных пропусков. Вы уверены, что хотите удалить шаблон?", 
					numbersSting);
				return MessageBoxService.ShowQuestion(message);
			}
			else 
				return base.ShowRemovingQuestion();
		}

		protected override void AfterRemove(AccessTemplate model)
		{
			base.AfterRemove(model);
			ServiceFactory.Events.GetEvent<UpdateAccessTemplateEvent>().Publish(model.UID);
		}
	}	
}