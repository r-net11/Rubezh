using System.Windows.Media;
using StrazhAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.ElementProperties.ViewModels;

namespace PlansModule.ViewModels
{
	public class DesignerPropertiesViewModel : SaveCancelDialogViewModel
	{
		private const double MinSize = 10;
		public Plan Plan { get; private set; }
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public DesignerPropertiesViewModel(Plan plan)
		{
			Title = "Свойства элемента: План";
			if (plan == null)
			{
				plan = new Plan();
				var width = RegistrySettingsHelper.GetDouble("Administrator.Plans.DefaultWidth");
				var height = RegistrySettingsHelper.GetDouble("Administrator.Plans.DefaultHeight");
				var color = RegistrySettingsHelper.GetColor("Administrator.Plans.DefaultColor");
				if (width != 0)
					plan.Width = width;
				if (height != 0)
					plan.Height = height;
				plan.BackgroundColor = color;
			}
			Plan = plan;
			ImagePropertiesViewModel = new ImagePropertiesViewModel(Plan);
			CopyProperties();
		}

		void CopyProperties()
		{
			BackgroundColor = Plan.BackgroundColor;
			Caption = Plan.Caption;
			Description = Plan.Description;
			Width = Plan.Width;
			Height = Plan.Height;
		}

		Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged(() => BackgroundColor);
			}
		}

		string _caption;
		public string Caption
		{
			get { return _caption; }
			set
			{
				_caption = value;
				OnPropertyChanged(() => Caption);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}

		double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Caption);
		}

		protected override bool Save()
		{
			RegistrySettingsHelper.SetDouble("Administrator.Plans.DefaultWidth", Width);
			RegistrySettingsHelper.SetDouble("Administrator.Plans.DefaultHeight", Height);
			RegistrySettingsHelper.SetColor("Administrator.Plans.DefaultColor", BackgroundColor);

			Plan.Caption = Caption;
			Plan.Description = Description;
			Plan.Width = Width;
			Plan.Height = Height;
			Plan.BackgroundColor = BackgroundColor;
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}