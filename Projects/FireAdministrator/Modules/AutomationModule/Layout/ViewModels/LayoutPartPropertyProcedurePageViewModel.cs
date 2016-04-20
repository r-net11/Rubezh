using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Services.Layout;
using System.Collections.ObjectModel;
using RubezhAPI.Automation;
using RubezhClient;
using RubezhAPI.Models.Layouts;

namespace AutomationModule.Layout.ViewModels
{
	public class LayoutPartPropertyProcedurePageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartProcedureViewModel _layoutPartViewModel;

		public LayoutPartPropertyProcedurePageViewModel(LayoutPartProcedureViewModel layoutPartViewModel)
		{
			_layoutPartViewModel = layoutPartViewModel;
			Procedures = new ObservableCollection<Procedure>(ClientManager.SystemConfiguration.AutomationConfiguration.Procedures);
		}

		private ObservableCollection<Procedure> _procedures;
		public ObservableCollection<Procedure> Procedures
		{
			get { return _procedures; }
			set
			{
				_procedures = value;
				OnPropertyChanged(() => Procedures);
			}
		}

		private Procedure _selectedProcedure;
		public Procedure SelectedProcedure
		{
			get { return _selectedProcedure; }
			set
			{
				_selectedProcedure = value;
				OnPropertyChanged(() => SelectedProcedure);
			}
		}

		public override string Header
		{
			get { return "Процедура"; }
		}
		public override void CopyProperties()
		{
			SelectedProcedure = Procedures.FirstOrDefault(item => item.Uid == ((LayoutPartReferenceProperties)_layoutPartViewModel.Properties).ReferenceUID);
		}
		public override bool CanSave()
		{
			return SelectedProcedure != null;
		}
		public override bool Save()
		{
			var properties = (LayoutPartReferenceProperties)_layoutPartViewModel.Properties;
			if (properties.ReferenceUID != SelectedProcedure.Uid)
			{
				properties.ReferenceUID = SelectedProcedure.Uid;
				_layoutPartViewModel.UpdateLayoutPart();
				return true;
			}
			return false;
		}
	}
}