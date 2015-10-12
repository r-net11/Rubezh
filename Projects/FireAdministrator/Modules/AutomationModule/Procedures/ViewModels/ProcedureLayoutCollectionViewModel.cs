﻿using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;
using RubezhClient;
using LayoutModel = RubezhAPI.Models.Layouts.Layout;

namespace AutomationModule.ViewModels
{
	public class ProcedureLayoutCollectionViewModel : BaseViewModel
	{
		public ProcedureLayoutCollection ProcedureLayoutCollection { get; private set; }

		public ProcedureLayoutCollectionViewModel(ProcedureLayoutCollection procedureLayoutCollection)
		{
			ProcedureLayoutCollection = procedureLayoutCollection;
			LayoutItems = new ObservableCollection<ProcedureLayoutItemViewModel>();

			foreach (var layout in ClientManager.LayoutsConfiguration.Layouts)
			{
				var procedureLayoutItems = new ProcedureLayoutItemViewModel(ProcedureLayoutCollection, layout);
				LayoutItems.Add(procedureLayoutItems);
			}
		}

		public ObservableCollection<ProcedureLayoutItemViewModel> LayoutItems { get; private set; }
	}

	public class ProcedureLayoutItemViewModel : BaseViewModel
	{
		public LayoutModel Layout { get; private set; }
		public ProcedureLayoutCollection ProcedureLayoutCollection { get; private set; }

		public ProcedureLayoutItemViewModel(ProcedureLayoutCollection procedureLayoutCollection, LayoutModel layout)
		{
			Layout = layout;
			ProcedureLayoutCollection = procedureLayoutCollection;
		}

		public string Name
		{
			get { return Layout.Caption; }
		}

		public bool IsChecked
		{
			get { return ProcedureLayoutCollection.LayoutsUIDs.Contains(Layout.UID); }
			set
			{
				if (value && !ProcedureLayoutCollection.LayoutsUIDs.Contains(Layout.UID))
					ProcedureLayoutCollection.LayoutsUIDs.Add(Layout.UID);
				else if (!value)
					ProcedureLayoutCollection.LayoutsUIDs.Remove(Layout.UID);
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}