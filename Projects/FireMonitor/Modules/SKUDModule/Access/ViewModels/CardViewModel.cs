using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class CardViewModel : BaseViewModel
	{
		public SKDCard Card { get; private set; }

		public CardViewModel(SKDCard card)
		{
			Card = card;
			ID = card.IDFamily + "/" + card.IDNo;
			StartDate = card.StartDate;
			EndDate = card.EndDate;
		}

		bool _isBlocked;
		public bool IsBlocked
		{
			get { return _isBlocked; }
			set
			{
				_isBlocked = value;
				OnPropertyChanged("IsBlocked");
			}
		}

		public string ID { get; private set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }
	}
}