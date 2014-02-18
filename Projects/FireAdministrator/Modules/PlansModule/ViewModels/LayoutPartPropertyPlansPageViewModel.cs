using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Microsoft.Win32;
using FiresecAPI.Models;
using FiresecClient;

namespace PlansModule.ViewModels
{
	public class LayoutPartPropertyPlansPageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartPlansViewModel _layoutPartPlansViewModel;

		public LayoutPartPropertyPlansPageViewModel(LayoutPartPlansViewModel layoutPartPlansViewModel)
		{
			_layoutPartPlansViewModel = layoutPartPlansViewModel;
			//Plans = new ObservableCollection<Plan>(FiresecManager.SystemConfiguration.Cameras);
			CopyProperties();
			UpdateLayoutPart();
		}

		private ObservableCollection<Plan> _plans;
		public ObservableCollection<Plan> Plans
		{
			get { return _plans; }
			set
			{
				_plans = value;
				OnPropertyChanged(() => Plans);
			}
		}

		private Plan _selectedPlan;
		public Plan SelectedPlan
		{
			get { return _selectedPlan; }
			set
			{
				_selectedPlan = value;
				OnPropertyChanged(() => SelectedPlan);
			}
		}

		public override string Header
		{
			get { return "Планы"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartPlansProperties)_layoutPartPlansViewModel.Properties;
			//SelectedPlan = FiresecManager.SystemConfiguration.Plans.FirstOrDefault(item => item.UID == properties.SourceUID);
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			var properties = (LayoutPartPlansProperties)_layoutPartPlansViewModel.Properties;
			//if ((SelectedPlan == null && properties.SourceUID != Guid.Empty) || (SelectedPlan != null && properties.SourceUID != SelectedPlan.UID))
			//{
			//    properties.SourceUID = SelectedPlan == null ? Guid.Empty : SelectedPlan.UID;
			//    UpdateLayoutPart();
			//    return true;
			//}
			return false;
		}

		private void UpdateLayoutPart()
		{
			var properties = (LayoutPartPlansProperties)_layoutPartPlansViewModel.Properties;
			//_layoutPartPlansViewModel.PlansTitle = SelectedPlan == null ? _layoutPartPlansViewModel.Title : SelectedPlan.Caption;
		}
	}
}