using System.Windows.Media;
using FiresecAPI.SKD.PassCardLibrary;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.ElementProperties.ViewModels;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardTemplatePropertiesViewModel : SaveCancelDialogViewModel
	{
		private const double MinSize = 10;
		public PassCardTemplate PassCardTemplate { get; private set; }
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public PassCardTemplatePropertiesViewModel(PassCardTemplate passCardTemplate)
		{
			Title = "Свойства элемента: Шаблон пропуска";
			if (passCardTemplate == null)
			{
				passCardTemplate = new PassCardTemplate();
				var width = RegistrySettingsHelper.GetDouble("Administrator.PassCardTemplate.DefaultWidth");
				var height = RegistrySettingsHelper.GetDouble("Administrator.PassCardTemplate.DefaultHeight");
				var color = RegistrySettingsHelper.GetColor("Administrator.PassCardTemplate.DefaultColor");
				if (width != 0)
					passCardTemplate.Width = width;
				if (height != 0)
					passCardTemplate.Height = height;
				passCardTemplate.BackgroundColor = color;
			}
			PassCardTemplate = passCardTemplate;
			ImagePropertiesViewModel = new ImagePropertiesViewModel(PassCardTemplate);
			CopyProperties();
		}

		private void CopyProperties()
		{
			BackgroundColor = PassCardTemplate.BackgroundColor;
			Caption = PassCardTemplate.Caption;
			Description = PassCardTemplate.Description;
			Width = PassCardTemplate.Width;
			Height = PassCardTemplate.Height;
		}

		private Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged("BackgroundColor");
			}
		}

		private string _caption;
		public string Caption
		{
			get { return _caption; }
			set
			{
				_caption = value;
				OnPropertyChanged("Caption");
			}
		}

		private string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		private double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged("Width");
			}
		}

		private double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged("Height");
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Caption);
		}

		protected override bool Save()
		{
			RegistrySettingsHelper.SetDouble("Administrator.PassCardTemplate.DefaultWidth", Width);
			RegistrySettingsHelper.SetDouble("Administrator.PassCardTemplate.DefaultHeight", Height);
			RegistrySettingsHelper.SetColor("Administrator.PassCardTemplate.DefaultColor", BackgroundColor);

			PassCardTemplate.Caption = Caption;
			PassCardTemplate.Description = Description;
			PassCardTemplate.Width = Width;
			PassCardTemplate.Height = Height;
			PassCardTemplate.BackgroundColor = BackgroundColor;
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}