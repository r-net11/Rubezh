using FiresecClient.SKDHelpers;
using Infrastructure.Common.Services;
using SKDModule.Events;
using System;
using System.Collections.Generic;

namespace SKDModule
{
	public interface ICardDoorsParentList<T>
		where T : IDoorsParent
	{
		List<T> DoorsParents { get; }
	}

	public interface IDoorsParent
	{
		void UpdateCardDoors(IEnumerable<Guid> doorUIDs, Guid organisationUID);
	}

	public class UpdateOrganisationDoorsEventSubscriber<T>
		where T : IDoorsParent
	{
		readonly ICardDoorsParentList<T> _cardDoorsParentList;

		public UpdateOrganisationDoorsEventSubscriber(ICardDoorsParentList<T> cardDoorsParentList)
		{
			_cardDoorsParentList = cardDoorsParentList;
			ServiceFactoryBase.Events.GetEvent<UpdateOrganisationDoorsEvent>().Unsubscribe(OnOrganisationDoorsChanged);
			ServiceFactoryBase.Events.GetEvent<UpdateOrganisationDoorsEvent>().Subscribe(OnOrganisationDoorsChanged);
		}

		void OnOrganisationDoorsChanged(Guid organisationUID)
		{
			var doorUIDs = OrganisationHelper.GetSingle(organisationUID).DoorUIDs;
			_cardDoorsParentList.DoorsParents.ForEach(x => x.UpdateCardDoors(doorUIDs, organisationUID));
		}
	}
}
