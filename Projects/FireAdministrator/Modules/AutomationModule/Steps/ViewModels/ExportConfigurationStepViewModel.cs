using StrazhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ExportConfigurationStepViewModel : BaseStepViewModel
	{
		ExportConfigurationArguments ExportConfigurationArguments { get; set; }
		public ArgumentViewModel IsExportDevices { get; private set; }
		public ArgumentViewModel IsExportZones { get; private set; }
		public ArgumentViewModel IsExportDoors { get; private set; }
		public ArgumentViewModel PathArgument { get; private set; }

		public ExportConfigurationStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ExportConfigurationArguments = stepViewModel.Step.ExportConfigurationArguments;
			IsExportDevices = new ArgumentViewModel(ExportConfigurationArguments.IsExportDevices, stepViewModel.Update, UpdateContent);
			IsExportZones = new ArgumentViewModel(ExportConfigurationArguments.IsExportZones, stepViewModel.Update, UpdateContent);
			IsExportDoors = new ArgumentViewModel(ExportConfigurationArguments.IsExportDoors, stepViewModel.Update, UpdateContent);
			PathArgument = new ArgumentViewModel(ExportConfigurationArguments.PathArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			IsExportDevices.Update(Procedure, ExplicitType.Boolean);
			IsExportZones.Update(Procedure, ExplicitType.Boolean);
			IsExportDoors.Update(Procedure, ExplicitType.Boolean);
			PathArgument.Update(Procedure, ExplicitType.String);
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
