using RubezhAPI;
using RubezhAPI.SKD;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class CardViewModel : TreeNodeViewModel<CardViewModel>
	{
		public SKDCard Card { get; private set; }
		public Organisation Organisation { get; private set; }
		public bool IsCard { get; private set; }
		public bool IsOrganisation { get; private set; }
		public bool IsDeactivatedRootItem { get; private set; }
		public string Number 
		{
			get
			{
				if (IsDeactivatedRootItem)
					return "Деактивированные";
				if (IsOrganisation)
					return Organisation.Name;
                if (Card != null)
                    return Card.Number.ToString();
                return "";
			}
		}
		public string CardType
		{
			get{ return IsCard ? Card.GKCardType.ToDescription() : ""; }
		}

		public bool IsInStopList
		{
			get { return IsCard && Card.IsInStopList; }
		}

		public string EmployeeName
		{
			get { return IsCard ? Card.EmployeeName : ""; }
		}

		public string StopReason
		{
			get { return IsCard ? Card.StopReason : ""; }
		}
		
		public CardViewModel() { }
		
		public CardViewModel(SKDCard card)
		{
			Card = card;
			IsCard = true;
		}

		public CardViewModel(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			IsExpanded = true;
		}

		public void Update(SKDCard card)
		{
			Card = card;
			OnPropertyChanged(() => Card);
			OnPropertyChanged(() => Number);
			OnPropertyChanged(() => IsInStopList);
			OnPropertyChanged(() => EmployeeName);
			OnPropertyChanged(() => StopReason);
		}

		public void Update(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			OnPropertyChanged(() => Card);
			OnPropertyChanged(() => Number);
			OnPropertyChanged(() => IsInStopList);
			OnPropertyChanged(() => EmployeeName);
			OnPropertyChanged(() => StopReason);
		}

		public static CardViewModel DeactivatedRootItem
		{
			get { return new CardViewModel { IsDeactivatedRootItem = true }; } 
		}
	}
}