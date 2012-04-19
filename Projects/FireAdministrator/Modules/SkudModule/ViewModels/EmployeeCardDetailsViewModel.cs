using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using FiresecAPI.Models.Skud;
using FiresecClient;
using SkudModule.Properties;

namespace SkudModule.ViewModels
{
	public class EmployeeCardDetailsViewModel : SaveCancelDialogContent
	{
		public EmployeeCardViewModel EmployeeCardViewModel { get; private set; }
		public EmployeeCardDetails EmployeeCardDetails { get; private set; }

		public EmployeeCardDetailsViewModel(EmployeeCardViewModel employeeCardViewModel = null)
		{
			EmployeeCardViewModel = employeeCardViewModel;
			Title = Resources.EmployeeCardTitle;
			SaveText = Resources.SaveText;
			Initialize();
		}

		private void Initialize()
		{
			EmployeeCardDetails = EmployeeCardViewModel == null ? new EmployeeCardDetails() { Id = -1 } : FiresecManager.GetEmployeeCard(EmployeeCardViewModel.EmployeeCard);
		}

		protected override void Save(ref bool cancel)
		{
			base.Save(ref cancel);
			if (cancel)
				return;
			cancel = !FiresecManager.SaveEmployeeCard(EmployeeCardDetails);
			if (!cancel)
			{
				if (EmployeeCardViewModel == null)
					EmployeeCardViewModel = new ViewModels.EmployeeCardViewModel(new EmployeeCard());
				EmployeeCardViewModel.EmployeeCard.Id = EmployeeCardDetails.Id;
				EmployeeCardViewModel.EmployeeCard.Age = EmployeeCardDetails.Age;
				EmployeeCardViewModel.EmployeeCard.Comment = EmployeeCardDetails.Comment;
				EmployeeCardViewModel.EmployeeCard.Department = EmployeeCardDetails.Department;
				EmployeeCardViewModel.EmployeeCard.FirstName = EmployeeCardDetails.FirstName;
				EmployeeCardViewModel.EmployeeCard.LastName = EmployeeCardDetails.LastName;
				EmployeeCardViewModel.EmployeeCard.PersonId = EmployeeCardDetails.PersonId;
				EmployeeCardViewModel.EmployeeCard.Position = EmployeeCardDetails.Position;
				EmployeeCardViewModel.EmployeeCard.SecondName = EmployeeCardDetails.SecondName;
			}
		}
		protected override bool CanSave()
		{
			return base.CanSave();
		}
	}
}