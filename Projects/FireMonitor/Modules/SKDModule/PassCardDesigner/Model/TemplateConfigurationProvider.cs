using System.Threading.Tasks;
using Infrastructure.Common;
using System;
using Infrustructure.Plans;
using Color = StrazhAPI.Color;

namespace SKDModule.PassCardDesigner.Model
{
	public class TemplateConfigurationProvider
	{
		#region Fields
		private const string PathForDefaultWidth = "Administrator.PassCardTemplate.DefaultWidth";
		private const string PathForDefaultHeight = "Administrator.PassCardTemplate.DefaultHeight";
		private const string PathForDefaultBackgroundColorFront = "Administrator.PassCardTemplate.DefaultColorFront";
		private const string PathForDefaultBorderThcknessFront = "Administrator.PassCardTemplate.DefaultBorderFront";
		private const string PathForDefaultBorderColorFront = "Administrator.PassCardTemplate.DefaultBorderColorFront";
		private const string PathForDefaultBackgroundColorBack = "Administrator.PassCardTemplate.DefaultColorBack";
		private const string PathForDefaultBorderThcknessBack = "Administrator.PassCardTemplate.DefaultBorderBack";
		private const string PathForDefaultBorderColorBack = "Administrator.PassCardTemplate.DefaultBorderColorBack";
		private const string PathForDefaultDualTemplateValue = "Administrator.PassCardTemplate.DefaultDualTemplateValue";
		private const int DefaultWidth = 100;
		private const int DefaultHeight = 100;
		#endregion

		#region Properties
		public double Width { get; private set; }
		public double Height { get; private set; }
		public Color FrontBackgroundColor { get; private set; }
		public Color BackBackgroundColor { get; private set; }
		public Color FrontBorderColor { get; private set; }
		public Color BackBorderColor { get; private set; }
		public double FrontBorderThickness { get; private set; }
		public double BackBorderThickness { get; private set; }
		public bool IsDualTemplateEnabled { get; private set; }
		#endregion

		private static double GetDefaultSize(double size, double defaultSize)
		{
			const double tolerance = 0.0000001;
			return Math.Abs(size) > tolerance ? size : defaultSize;
		}

		private Task LoadDefaultProperties()
		{
			return Task.Factory.StartNew(() =>
			{
				Width = GetDefaultSize(RegistrySettingsHelper.GetDouble(PathForDefaultWidth), DefaultWidth);
				Height = GetDefaultSize(RegistrySettingsHelper.GetDouble(PathForDefaultHeight), DefaultHeight);

				FrontBackgroundColor = RegistrySettingsHelper.GetColor(PathForDefaultBackgroundColorFront);
				FrontBorderColor = RegistrySettingsHelper.GetColor(PathForDefaultBorderColorFront);
				FrontBorderThickness = RegistrySettingsHelper.GetDouble(PathForDefaultBorderThcknessFront);

				BackBackgroundColor = RegistrySettingsHelper.GetColor(PathForDefaultBackgroundColorBack);
				BackBorderColor = RegistrySettingsHelper.GetColor(PathForDefaultBorderColorBack);
				BackBorderThickness = RegistrySettingsHelper.GetDouble(PathForDefaultBorderThcknessBack);

				if (RegistrySettingsHelper.GetBool(PathForDefaultDualTemplateValue))
					IsDualTemplateEnabled = true;
			});
		}

		public async void InitializeWithDefaults(Template template)
		{
			await LoadDefaultProperties();

			template.Width = Width;
			template.Height = Height;
			template.Front.PreviewImage.BackgroundColor = FrontBackgroundColor.ToWindowsColor();
			template.Front.PreviewImage.BorderColor = FrontBorderColor.ToWindowsColor();
			template.Front.PreviewImage.BorderThickness = FrontBorderThickness;
			template.Back.PreviewImage.BackgroundColor = BackBackgroundColor.ToWindowsColor();
			template.Back.PreviewImage.BorderColor = BackBorderColor.ToWindowsColor();
			template.Back.PreviewImage.BorderThickness = BackBorderThickness;
			template.IsDualTemplateEnabled = IsDualTemplateEnabled;
		}

		public Task SaveDefaultPropertiesFrom(Template passcardTemplate)
		{
			return Task.Factory.StartNew(() =>
			{
				RegistrySettingsHelper.SetBool(PathForDefaultDualTemplateValue, passcardTemplate.IsDualTemplateEnabled);
				RegistrySettingsHelper.SetDouble(PathForDefaultWidth, passcardTemplate.Width);
				RegistrySettingsHelper.SetDouble(PathForDefaultHeight, passcardTemplate.Height);
				RegistrySettingsHelper.SetColor(PathForDefaultBackgroundColorFront, passcardTemplate.Front.PreviewImage.BackgroundColor.ToStruzhColor());
				RegistrySettingsHelper.SetDouble(PathForDefaultBorderThcknessFront, passcardTemplate.Front.PreviewImage.BorderThickness);
				RegistrySettingsHelper.SetColor(PathForDefaultBorderColorFront, passcardTemplate.Front.PreviewImage.BorderColor.ToStruzhColor());
				RegistrySettingsHelper.SetColor(PathForDefaultBackgroundColorBack, passcardTemplate.Back.PreviewImage.BackgroundColor.ToStruzhColor());
				RegistrySettingsHelper.SetDouble(PathForDefaultBorderThcknessBack, passcardTemplate.Back.PreviewImage.BorderThickness);
				RegistrySettingsHelper.SetColor(PathForDefaultBorderColorBack, passcardTemplate.Back.PreviewImage.BorderColor.ToStruzhColor());
			});
		}
	}
}
