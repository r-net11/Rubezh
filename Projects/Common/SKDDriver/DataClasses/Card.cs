using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SKDDriver.DataClasses
{
	public class Card
	{
		public Card()
		{
			PendingCards = new List<PendingCard>();
			CardDoors = new List<CardDoor>();
			GKControllerUIDs = new List<CardGKControllerUID>();
		}
		
		[Key]
		public Guid UID { get; set; }

		public Guid? EmployeeUID { get; set; }
		public Employee Employee { get; set; }

		public Guid? AccessTemplateUID { get; set; }
		public AccessTemplate AccessTemplate { get; set; }

		public Guid? PassCardTemplateUID { get; set; }
		public PassCardTemplate PassCardTemplate { get; set; }

		public ICollection<PendingCard> PendingCards { get; set; }

		public ICollection<CardDoor> CardDoors { get; set; }

		public ICollection<CardGKControllerUID> GKControllerUIDs { get; set; }

		public int Number { get; set; }

		public int CardType { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public bool IsInStopList { get; set; }

		public string StopReason { get; set; }

		public string Password { get; set; }

		public Guid? DeactivationControllerUID { get; set; }

		public int UserTime { get; set; }

		public int GKLevel { get; set; }

		public int GKLevelSchedule { get; set; }

		public int GKCardType { get; set; }

		public string ExternalKey { get; set; }

		public FiresecAPI.SKD.SKDCard Translate()
		{
			return new FiresecAPI.SKD.SKDCard
			{
				UID = UID,
				Number = (uint)Number,
				EmployeeUID = EmployeeUID,
				StartDate = StartDate,
				EndDate = EndDate.GetValueOrDefault(),
				UserTime = UserTime,
				DeactivationControllerUID = DeactivationControllerUID.GetValueOrDefault(),
				CardDoors = CardDoors.Select(x => x.Translate()).ToList(),
				PassCardTemplateUID = PassCardTemplateUID,
				CardType = (FiresecAPI.SKD.CardType)CardType,
				GKCardType = (FiresecAPI.GK.GKCardType)GKCardType,
				Password = Password,
				IsInStopList = IsInStopList,
				StopReason = StopReason,
				EmployeeName = Employee != null ? Employee.Name : null,
				OrganisationUID = Employee != null ? Employee.OrganisationUID.GetValueOrDefault() : Guid.Empty,
				GKLevel = GKLevel,
				GKLevelSchedule = GKLevelSchedule,
				GKControllerUIDs = GKControllerUIDs != null ? GKControllerUIDs.Select(x => x.UID).ToList() : new List<Guid>()
			};
		}

		public void TranslateBack(FiresecAPI.SKD.SKDCard apiItem)
		{
			Number = (int)apiItem.Number;
			EmployeeUID = apiItem.EmployeeUID;
			StartDate = apiItem.StartDate;
			EndDate = apiItem.EndDate;
			UserTime = apiItem.UserTime;
			DeactivationControllerUID = apiItem.DeactivationControllerUID;
			CardDoors = apiItem.CardDoors.Select(x => new CardDoor(x)).ToList();
			PassCardTemplateUID = apiItem.PassCardTemplateUID;
			AccessTemplateUID = apiItem.AccessTemplateUID;
			CardType = (int)apiItem.CardType;
			GKCardType = (int)apiItem.GKCardType;
			Password = apiItem.Password;
			IsInStopList = apiItem.IsInStopList;
			StopReason = apiItem.StopReason;
			GKLevel = apiItem.GKLevel;
			GKControllerUIDs = apiItem.GKControllerUIDs.Select(x => new CardGKControllerUID
			{ 
				UID = Guid.NewGuid(), 
				CardUID = UID, 
				GKControllerUID = x
			}).ToList();
		}
	}
}
