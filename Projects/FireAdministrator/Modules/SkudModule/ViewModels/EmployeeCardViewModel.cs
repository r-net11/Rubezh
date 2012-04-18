using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using FiresecAPI.Models.Skud;
using FiresecClient;

namespace SkudModule.ViewModels
{
	public class EmployeeCardViewModel : SaveCancelDialogContent
	{
		public EmployeeCardIndex EmployeeCardIndex { get; private set; }
		public EmployeeCard EmployeeCard { get; private set; }

		public EmployeeCardViewModel(EmployeeCardIndex employeeCardIndex = null)
		{
			EmployeeCardIndex = employeeCardIndex;
		}

		public void Update()
		{
			OnPropertyChanged("EmployeeCardIndex");
		}

		public void Initialize()
		{
			EmployeeCard = EmployeeCardIndex == null ? new EmployeeCard() { Id = -1 } : FiresecManager.GetEmployeeCard(EmployeeCardIndex);
		}

		protected override void Save(ref bool cancel)
		{
			cancel = !FiresecManager.SaveEmployeeCard(EmployeeCard);
			if (!cancel)
			{
				if (EmployeeCardIndex == null)
					EmployeeCardIndex = new EmployeeCardIndex();
				EmployeeCardIndex.Id = EmployeeCard.Id;
				EmployeeCardIndex.Age = EmployeeCard.Age;
				EmployeeCardIndex.Comment = EmployeeCard.Comment;
				EmployeeCardIndex.Department = EmployeeCard.Department;
				EmployeeCardIndex.FirstName = EmployeeCard.FirstName;
				EmployeeCardIndex.LastName = EmployeeCard.LastName;
				EmployeeCardIndex.PersonId = EmployeeCard.PersonId;
				EmployeeCardIndex.Position = EmployeeCard.Position;
				EmployeeCardIndex.SecondName = EmployeeCard.SecondName;
				Update();
			}
			base.Save(ref cancel);
		}
		protected override bool CanSave()
		{
			return base.CanSave();
		}
	}
}