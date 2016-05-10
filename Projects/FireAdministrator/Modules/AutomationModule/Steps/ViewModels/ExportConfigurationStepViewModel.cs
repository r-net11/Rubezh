using StrazhAPI.Automation;
using Localization.Automation.ViewModels;

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
				var result = StepCommonViewModel.ExportConfiguration;
				if (IsExportDevices.ExplicitValue.BoolValue)
                    result += StepCommonViewModel.ExportConfiguration_Device;
				if (IsExportZones.ExplicitValue.BoolValue)
                    result += StepCommonViewModel.ExportConfiguration_Zone;
				if (IsExportDoors.ExplicitValue.BoolValue)
                    result += StepCommonViewModel.ExportConfiguration_Door;
				if (!PathArgument.IsEmpty)
                    result += StepCommonViewModel.In + PathArgument.Description;
				return result;
			}
		}
	}
}
