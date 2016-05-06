using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.Models.Layouts;
using StrazhAPI.Automation;
using FiresecClient;
using Infrastructure.Common;
using System.Windows.Media;
using System.Windows;

namespace AutomationModule.ViewModels
{
	public class LayoutProcedurePartViewModel : BaseViewModel
	{
		public LayoutProcedurePartViewModel(LayoutPartProcedureProperties properties)
		{
			Procedure = FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(item => item.Uid == properties.ReferenceUID);
			if (Procedure != null)
				Text = Procedure.Name;
			RunProcedureCommand = new RelayCommand(OnRunProcedure, CanRunProcedure);

			UseCustomStyle = properties.UseCustomStyle;
			if (UseCustomStyle)
			{
				BackgroundBrush = new SolidColorBrush(properties.BackgroundColor);
				ForegroundBrush = new SolidColorBrush(properties.ForegroundColor);
				BorderBrush = new SolidColorBrush(properties.BorderColor);
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
