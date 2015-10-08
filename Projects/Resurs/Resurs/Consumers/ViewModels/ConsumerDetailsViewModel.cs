using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;

namespace Resurs.ViewModels
{
	public partial class ConsumerDetailsViewModel : SaveCancelDialogViewModel
	{
		Consumer _consumer;
		public Consumer Consumer 
		{ 
			get { return _consumer; }
			set
			{
				_consumer = value;
				if (_consumer != null)
				{
					Name = _consumer.Name;
					Address = _consumer.Address;
					Description = _consumer.Description;
					Phone = _consumer.Phone;
					Email = _consumer.Email;
					Login = _consumer.Login;
					Password = _consumer.Password;
					IsSendEmail = _consumer.IsSendEmail;
				}
			}
		}

		public ConsumerDetailsViewModel(Consumer consumer, bool isNew = false, bool isReadOnly = false)
		{
			Title = isNew ? "Создание абонента" : "Редактирование абонента";
			Consumer = consumer;
			IsReadOnly = isReadOnly;
		}

		bool _isReadOnly;
		public bool IsReadOnly
		{
			get { return _isReadOnly; }
			set
			{
				_isReadOnly = value;
				OnPropertyChanged(() => IsReadOnly);
			}
		}

		protected override bool Save()
		{
			Consumer.Name = Name;
			Consumer.Address = Address;
			Consumer.Description = Description;
			Consumer.Phone = Phone;
			Consumer.Email = Email;
			Consumer.Login = Login;
			Consumer.Password = Password;
			Consumer.IsSendEmail = IsSendEmail;

			DBCash.SaveConsumer(Consumer);

			return base.Save();
		}
	}
}