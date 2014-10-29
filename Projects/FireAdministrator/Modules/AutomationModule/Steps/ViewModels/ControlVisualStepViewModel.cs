using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using System;
using System.Collections.Generic;
using AutomationModule.Steps.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Services.Layout;
using FiresecAPI.Models.Layouts;

namespace AutomationModule.ViewModels
{
	public class ControlVisualStepViewModel : BaseStepViewModel
	{
		public ControlVisualArguments ControlVisualArguments { get; private set; }

		public ControlVisualStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlVisualArguments = stepViewModel.Step.ControlVisualArguments;
			ControlVisualTypes = new ObservableCollection<ControlVisualType>(ProcedureHelper.GetEnumList<ControlVisualType>());
			Argument = new ArgumentViewModel(ControlVisualArguments.Argument, stepViewModel.Update, UpdateContent);
		}

		public ArgumentViewModel Argument { get; set; }

		public ObservableCollection<LayoutViewModel> Layouts { get; private set; }
		private LayoutViewModel _selectedLayout;
		public LayoutViewModel SelectedLayout
		{
			get { return _selectedLayout; }
			set
			{
				_selectedLayout = value;
				ControlVisualArguments.Layout = SelectedLayout == null ? Guid.Empty : SelectedLayout.Layout.UID;
				OnPropertyChanged(() => SelectedLayout);

				LayoutParts = new ObservableCollection<LayoutPartViewModel>(SelectedLayout == null ? Enumerable.Empty<LayoutPartViewModel>() : SelectedLayout.Layout.Parts.Select(item => new LayoutPartViewModel(item, GetDescription(item))));
				OnPropertyChanged(() => LayoutParts);
				SelectedLayoutPart = LayoutParts.FirstOrDefault(x => x.LayoutPart.UID == ControlVisualArguments.LayoutPart);
			}
		}

		public ObservableCollection<LayoutPartViewModel> LayoutParts { get; private set; }
		private LayoutPartViewModel _selectedLayoutPart;
		public LayoutPartViewModel SelectedLayoutPart
		{
			get { return _selectedLayoutPart; }
			set
			{
				_selectedLayoutPart = value;
				ControlVisualArguments.LayoutPart = SelectedLayoutPart == null ? Guid.Empty : SelectedLayoutPart.LayoutPart.UID;
				OnPropertyChanged(() => SelectedLayoutPart);

				LayoutPartProperties = new ObservableCollection<LayoutPartPropertyViewModel>(SelectedLayoutPart == null || SelectedLayoutPart.Description == null ? Enumerable.Empty<LayoutPartPropertyViewModel>() : SelectedLayoutPart.Description.Properties.Select(item => new LayoutPartPropertyViewModel(item)));
				OnPropertyChanged(() => LayoutPartProperties);
				SelectedLayoutPartProperty = LayoutPartProperties.FirstOrDefault(x => x.LayoutPartProperty.Name == ControlVisualArguments.Property);
			}
		}

		public ObservableCollection<LayoutPartPropertyViewModel> LayoutPartProperties { get; private set; }
		private LayoutPartPropertyViewModel _selectedLayoutPartProperty;
		public LayoutPartPropertyViewModel SelectedLayoutPartProperty
		{
			get { return _selectedLayoutPartProperty; }
			set
			{
				_selectedLayoutPartProperty = value;
				ControlVisualArguments.Property = SelectedLayoutPartProperty == null ? null : (LayoutPartPropertyName?)SelectedLayoutPartProperty.LayoutPartProperty.Name;
				OnPropertyChanged(() => SelectedLayoutPartProperty);
			}
		}

		public ObservableCollection<ControlVisualType> ControlVisualTypes { get; private set; }
		public ControlVisualType SelectedControlVisualType
		{
			get { return ControlVisualArguments.Type; }
			set
			{
				ControlVisualArguments.Type = value;
				OnPropertyChanged(() => SelectedControlVisualType);
			}
		}

		public override string Description
		{
			get { return ""; }
		}
		public override void UpdateContent()
		{
			Argument.Update(Procedure);
			SelectedControlVisualType = ControlVisualTypes.FirstOrDefault(item => item == SelectedControlVisualType);
			Layouts = new ObservableCollection<LayoutViewModel>(FiresecManager.LayoutsConfiguration.Layouts.Select(item => new LayoutViewModel(item)));
			SelectedLayout = Layouts.FirstOrDefault(x => x.Layout.UID == ControlVisualArguments.Layout);
		}

		private static Dictionary<Guid, ILayoutPartDescription> _map;
		public static void Initialize()
		{
			_map = new Dictionary<Guid, ILayoutPartDescription>();
			foreach (var module in ApplicationService.Modules)
			{
				var layoutDeclarationModule = module as ILayoutDeclarationModule;
				if (layoutDeclarationModule != null)
					foreach (var layoutPartDescription in layoutDeclarationModule.GetLayoutPartDescriptions())
						_map.Add(layoutPartDescription.UID, layoutPartDescription);
			}
		}
		private static ILayoutPartDescription GetDescription(LayoutPart layoutPart)
		{
			return layoutPart == null || !_map.ContainsKey(layoutPart.DescriptionUID) ? null : _map[layoutPart.DescriptionUID];
		}
	}
}