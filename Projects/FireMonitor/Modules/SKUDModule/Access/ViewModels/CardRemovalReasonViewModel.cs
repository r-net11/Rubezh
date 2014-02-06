using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;
using Infrastructure.Common;

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