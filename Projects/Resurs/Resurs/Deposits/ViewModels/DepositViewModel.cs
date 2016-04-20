using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DepositViewModel : BaseViewModel
	{
		public Deposit Deposit { get; private set; }

		public DepositViewModel(Deposit deposit)
		{
			Update(deposit);
		}

		DateTime _moment;
		public DateTime Moment
		{
			get { return _moment; }
			set
			{
				_moment = value;
				OnPropertyChanged(() => Moment);
			}
		}

		decimal _amount;
		public Decimal Amount
		{
			get { return _amount; }
			set
			{
				_amount = value;
				OnPropertyChanged(() => Amount);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public void Update(Deposit deposit)
		{
			Deposit = deposit;
			Moment = deposit.Moment;
			Amount = deposit.Amount;
			Description = deposit.Description;
			Name = deposit.Name;
		}
	}
}
