using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DepositDetailsViewModel : SaveCancelDialogViewModel
	{

		public DepositDetailsViewModel(Deposit deposit, bool isNew)
		{
			Title = isNew ? "Пополнение баланса" : "Редактирование пополнения";
			Update(deposit);
		}

		public Guid UID { get; private set; }
		public Guid ConsumerUID { get; private set; }
		public Consumer Consumer { get; private set; }
		public string Name { get; private set; }

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

		public void Update(Deposit deposit)
		{
			UID = deposit.UID;
			ConsumerUID = deposit.ConsumerUID;
			Consumer = deposit.Consumer;
			Moment = deposit.Moment;
			Amount = deposit.Amount;
			Description = deposit.Description;
			Name = deposit.Name;
		}

		public Deposit GetDeposit()
		{
			return new Deposit
			{
				UID = this.UID,
				ConsumerUID = this.ConsumerUID,
				Consumer = this.Consumer,
				Moment = this.Moment,
				Amount = this.Amount,
				Description = this.Description,
				Name = this.Name
			};
		}
	}
}
