using System;
using Infrastructure.Common.Windows.ViewModels;
using Localization.SKD.Common;
using Localization.SKD.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardRemovalReasonViewModel : SaveCancelDialogViewModel
	{
		public CardRemovalReasonViewModel()
		{
			Title = CommonResources.DeactivationReason;
			RemovalReason = string.Format(CommonViewModels.Lost, DateTime.Now);
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