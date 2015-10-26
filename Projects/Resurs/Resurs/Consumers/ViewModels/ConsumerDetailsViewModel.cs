﻿using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Resurs.ViewModels
{
	public partial class ConsumerDetailsViewModel : SaveCancelDialogViewModel
	{
		public ConsumerDetailsViewModel(Consumer consumer, bool isReadOnly, bool isNew = false)
		{
			Title = isNew ? "Создание абонента" : "Редактирование абонента";
			Update(consumer);
			InitializeBill(consumer);
			IsReadOnly = isReadOnly;
		}

		public bool IsReadOnly { get; private set; }

		public Guid Uid { get; private set; }
		public Guid? ParentUid { get; private set; }

		int _selectedTabIndex;
		public int SelectedTabIndex
		{
			get { return _selectedTabIndex; }
			set
			{
				_selectedTabIndex = value;
				OnPropertyChanged(() => SelectedTabIndex);
			}
		}

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
			Number = consumer.Number;
			Balance = consumer.Balance;
			Devices = new ObservableCollection<DeviceViewModel>(consumer.Devices.Select(x => new DeviceViewModel(x)));
		}

		public Consumer GetConsumer()
		{
			return new Consumer
			{
				Address = this.Address,
				Description = this.Description,
				Email = this.Email,
				IsFolder = false,
				IsSendEmail = this.IsSendEmail,
				Login = this.Login,
				Name = this.Name,
				ParentUID = this.ParentUid,
				Password = this.Password,
				Phone = this.Phone,
				UID = this.Uid,
				Number = this.Number,
				Balance = this.Balance,
				Devices = this.Devices.Select(x => x.Device).ToList()
			};
		}

		protected override bool Save()
		{
			var consumer = GetConsumer();
			DBCash.SaveConsumer(consumer);
			
			return base.Save();
		}
	}
}