using Infrastructure.Common.Windows;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class JournalEventViewModel
	{
		public JournalEventViewModel(Journal journal)
		{
			ShowCommand = new RelayCommand(OnShow);
			Journal = journal;
		}
		public Journal Journal { get; private set; }

		public bool IsUser { get { return Journal.UserName != null; } }

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			switch (Journal.ClassType)
			{ 
				case ClassType.IsDevice:
					if (Journal.ObjectUID.HasValue)
					Bootstrapper.MainViewModel.DevicesViewModel.Select(Journal.ObjectUID.Value);
					break;
				case ClassType.IsConsumer:
					if (Journal.ObjectUID.HasValue)
						Bootstrapper.MainViewModel.ConsumersViewModel.Select(Journal.ObjectUID.Value);
					break;
				case ClassType.IsUser:
					if (Journal.ObjectUID.HasValue)
						Bootstrapper.MainViewModel.UsersViewModel.Select(Journal.ObjectUID.Value);
					break;
				case ClassType.IsTariff:
					if (Journal.ObjectUID.HasValue)
						Bootstrapper.MainViewModel.TariffsViewModel.Select(Journal.ObjectUID.Value);
					break;
			}
		}
	}
}
