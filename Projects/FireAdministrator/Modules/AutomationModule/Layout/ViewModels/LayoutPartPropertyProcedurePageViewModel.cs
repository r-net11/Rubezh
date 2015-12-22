using Infrastructure.Common.Services.Layout;
using RubezhAPI.Automation;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System.Collections.ObjectModel;
using System.Linq;

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
			var properties = _layoutPartViewModel.Properties as LayoutPartProcedureProperties;
			if (properties != null)
			{
				properties.ReferenceUID = SelectedProcedure.Uid;
				properties.Text = _layoutPartViewModel.Title;
				_layoutPartViewModel.UpdateLayoutPart();
				return true;
			}
			return false;
		}
	}
}