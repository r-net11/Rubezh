using System;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardRemovalReasonViewModel : SaveCancelDialogViewModel
	{
		private bool deactivatedIsChecked;
		public bool RemoveIsChecked { get; set; }
		public bool DeactivatedIsChecked 
		{
			get { return deactivatedIsChecked; }
			set 
			{
				deactivatedIsChecked = value;
				OnPropertyChanged(()=>DeactivatedIsChecked);
			}
		}
		public CardRemovalReasonViewModel()
		{
			Title = "Удаление пропуска";
			RemovalReason = "Утерян " + DateTime.Now.ToString();
			RemoveIsChecked = true;
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

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(RemovalReason);
		}
	}
}