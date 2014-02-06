using System;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardRemovalReasonViewModel : SaveCancelDialogViewModel
	{
		public CardRemovalReasonViewModel()
		{
			Title = "Причина перемещения в СТОП-лист";
			CardRemovalReason = "Утеряна " + DateTime.Now.ToString();
		}

		string _cardRemovalReason;
		public string CardRemovalReason
		{
			get { return _cardRemovalReason; }
			set
			{
				_cardRemovalReason = value;
				OnPropertyChanged("_cardRemovalReason");
			}
		}
	}
}