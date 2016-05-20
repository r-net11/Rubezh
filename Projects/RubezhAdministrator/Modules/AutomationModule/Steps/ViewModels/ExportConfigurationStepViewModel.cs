using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ExportConfigurationStepViewModel : BaseStepViewModel
	{
		ExportConfigurationStep ExportConfigurationStep { get; set; }
		public ArgumentViewModel IsExportDevices { get; private set; }
		public ArgumentViewModel IsExportZones { get; private set; }
		public ArgumentViewModel IsExportDoors { get; private set; }
		public ArgumentViewModel PathArgument { get; private set; }

		public ExportConfigurationStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ExportConfigurationStep = (ExportConfigurationStep)stepViewModel.Step;
			IsExportDevices = new ArgumentViewModel(ExportConfigurationStep.IsExportDevices, stepViewModel.Update, UpdateContent);
			IsExportZones = new ArgumentViewModel(ExportConfigurationStep.IsExportZones, stepViewModel.Update, UpdateContent);
			IsExportDoors = new ArgumentViewModel(ExportConfigurationStep.IsExportDoors, stepViewModel.Update, UpdateContent);
			PathArgument = new ArgumentViewModel(ExportConfigurationStep.PathArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			IsExportDevices.Update(Procedure, ExplicitType.Boolean, isList: false);
			IsExportZones.Update(Procedure, ExplicitType.Boolean, isList: false);
			IsExportDoors.Update(Procedure, ExplicitType.Boolean, isList: false);
			PathArgument.Update(Procedure, ExplicitType.String, isList: false);
		}

		public override string Description
		{
			get
			{
				var result = "Экспортировать конфигурацию ";
				if (IsExportDevices.ExplicitValue.BoolValue)
					result += " устройства ";
				if (IsExportZones.ExplicitValue.BoolValue)
					result += " зоны ";
				if (IsExportDoors.ExplicitValue.BoolValue)
					result += " точки доступа ";
				if (!PathArgument.IsEmpty)
					result += "в " + PathArgument.Description;
				return result;
			}
		}
	}
}
