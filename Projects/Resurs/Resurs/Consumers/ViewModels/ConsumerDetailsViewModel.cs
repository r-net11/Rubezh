using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Resurs.ViewModels
{
	public partial class ConsumerDetailsViewModel : SaveCancelDialogViewModel
	{
		public ConsumerDetailsViewModel(Consumer consumer, bool isReadOnly, bool isNew = false)
		{
			Title = isNew ? "Создание абонента" : "Редактирование абонента";
			Update(consumer);
			IsReadOnly = isReadOnly;
			BillsViewModel = new BillsViewModel(consumer.Bills.ToList(), isReadOnly);
		}

		public bool IsReadOnly { get; private set; }

		public Guid Uid { get; private set; }
		public Guid? ParentUid { get; private set; } 

		public void Update(Consumer consumer)
		{
			Uid = consumer.UID;
			ParentUid = consumer.ParentUID;
			Name = consumer.Name;
			Address = consumer.Address;
			Description = consumer.Description;
			Phone = consumer.Phone;
			Email = consumer.Email;
			Login = consumer.Login;
			Password = consumer.Password;
			IsSendEmail = consumer.IsSendEmail;
			if (BillsViewModel != null)
				BillsViewModel.Update(consumer.Bills.ToList());
		}

		public Consumer GetConsumer()
		{
			return new Consumer
			{
				Address = this.Address,
				Bills = BillsViewModel == null ? null : BillsViewModel.GetBills(),
				Description = this.Description,
				Email = this.Email,
				FIO = this.FIO,
				IsFolder = false,
				IsSendEmail = this.IsSendEmail,
				Login = this.Login,
				Name = this.Name,
				ParentUID = this.ParentUid,
				Password = this.Password,
				Phone = this.Phone,
				UID = this.Uid
			};
		}

		protected override bool Save()
		{
			DBCash.SaveConsumer(GetConsumer());
			return base.Save();
		}
	}
}