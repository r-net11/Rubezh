using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.XModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;
using System.Diagnostics;
using System;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public SKDJournalItem JournalItem { get; private set; }
		
		public JournalItemViewModel(SKDJournalItem journalItem)
		{
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowInTree);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			JournalItem = journalItem;
		}

		public bool CanShow
		{
			get { return CanShowInTree() || CanShowOnPlan(); }
		}

		public RelayCommand ShowObjectOrPlanCommand { get; private set; }
		void OnShowObjectOrPlan()
		{
			if (CanShowOnPlan())
				OnShowOnPlan();
			else if (CanShowInTree())
				OnShowObject();
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
		}
		bool CanShowProperties()
		{
			return false;
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
		}

		bool CanShowInTree()
		{
			return false;
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
		}
		bool CanShowOnPlan()
		{
			return false;
		}
	}
}