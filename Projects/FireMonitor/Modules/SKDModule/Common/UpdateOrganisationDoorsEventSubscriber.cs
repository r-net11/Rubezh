using System;
using System.Collections.Generic;
using RubezhClient.SKDHelpers;
using Infrastructure;
using SKDModule.Events;

namespace SKDModule
{
	public interface ICardDoorsParentList<T>
		where T : IDoorsParent
	{
		List<T> DoorsParents { get; }
	}

	public interface IDoorsParent
	{
		void UpdateCardDoors(IEnumerable<Guid> doorUIDs);
	}
	
	public class UpdateOrganisationDoorsEventSubscriber<T>
		where T : IDoorsParent
	{
		ICardDoorsParentList<T> _cardDoorsParentList;

		public UpdateOrganisationDoorsEventSubscriber(ICardDoorsParentList<T> cardDoorsParentList)
		{
			_cardDoorsParentList = cardDoorsParentList;
			ServiceFactory.Events.GetEvent<UpdateOrganisationDoorsEvent>().Unsubscribe(OnOrganisationDoorsChanged);
			ServiceFactory.Events.GetEvent<UpdateOrganisationDoorsEvent>().Subscribe(OnOrganisationDoorsChanged);
		}

		void OnOrganisationDoorsChanged(Guid organisationUID)
		{
			var doorUIDs = OrganisationHelper.GetSingle(organisationUID).DoorUIDs;
			_cardDoorsParentList.DoorsParents.ForEach(x => x.UpdateCardDoors(doorUIDs));
		}
	}
}
