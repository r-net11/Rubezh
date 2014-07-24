using System;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardRemovalReasonViewModel : SaveCancelDialogViewModel
	{
		public CardRemovalReasonViewModel()
		{
			Title = "Причина деактивации";
			RemovalReason = "Утеряна " + DateTime.Now.ToString();
		}

		string _removalReason;
		public string RemovalReason
		{
			get { return _removalReason; }
			set
			{
				_removalReason = value;
				OnPropertyChanged(() => RemovalReason);
			}
		}
	}
}