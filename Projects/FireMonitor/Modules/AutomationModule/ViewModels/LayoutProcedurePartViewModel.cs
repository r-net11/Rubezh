using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
using RubezhAPI.Automation;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace AutomationModule.ViewModels
{
	public class LayoutProcedurePartViewModel : BaseViewModel
	{
		public LayoutProcedurePartViewModel(LayoutPartProcedureProperties properties)
		{
			Procedure = ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(item => item.Uid == properties.ReferenceUID);
			if (Procedure != null)
				Text = Procedure.Name;
			RunProcedureCommand = new RelayCommand(OnRunProcedure, CanRunProcedure);

			UseCustomStyle = properties.UseCustomStyle;
			if (UseCustomStyle)
			{
				BackgroundBrush = new SolidColorBrush(properties.BackgroundColor.ToWindowsColor());
				ForegroundBrush = new SolidColorBrush(properties.ForegroundColor.ToWindowsColor());
				BorderBrush = new SolidColorBrush(properties.BorderColor.ToWindowsColor());
				BorderThickness = properties.BorderThickness;
				FontSize = properties.FontSize;
				FontStyle = properties.FontItalic ? FontStyles.Italic : FontStyles.Normal;
				FontWeight = properties.FontBold ? FontWeights.Bold : FontWeights.Normal;
				FontFamily = new FontFamily(properties.FontFamilyName);
				TextAlignment = (TextAlignment)properties.TextAlignment;
				Stretch = properties.Stretch ? Stretch.Fill : Stretch.None;
			}
			if (!string.IsNullOrWhiteSpace(properties.Text))
				Text = properties.Text;
		}

		public Procedure Procedure { get; private set; }

		public bool UseCustomStyle { get; private set; }
		public string Text { get; private set; }
		public Brush BackgroundBrush { get; private set; }
		public Brush ForegroundBrush { get; private set; }
		public Brush BorderBrush { get; private set; }
		public double BorderThickness { get; private set; }
		public double FontSize { get; private set; }
		public FontStyle FontStyle { get; private set; }
		public FontWeight FontWeight { get; private set; }
		public FontFamily FontFamily { get; private set; }
		public TextAlignment TextAlignment { get; private set; }
		public Stretch Stretch { get; private set; }

		public RelayCommand RunProcedureCommand { get; private set; }
		private void OnRunProcedure()
		{
			ProcedureArgumentsViewModel.Run(Procedure);
		}
		private bool CanRunProcedure()
		{
			return Procedure != null;
		}
	}
}
