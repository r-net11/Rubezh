using System.Collections.ObjectModel;
using FiresecAPI;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using System;
using System.Collections.Generic;
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

				LayoutParts = new ObservableCollection<LayoutPartViewModel>(SelectedLayout == null ? Enumerable.Empty<LayoutPartViewModel>() : SelectedLayout.Layout.Parts.Select(item => new LayoutPartViewModel(item, GetDescription(item))).Where(item => item.Description != null && item.Description.Properties.Count() > 0));
				SelectedLayoutPart = LayoutParts.FirstOrDefault(x => x.LayoutPart.UID == ControlVisualArguments.LayoutPart);
				OnPropertyChanged(() => LayoutParts);
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

				UpdateProperties();
			}
		}

		public ObservableCollection<LayoutPartPropertyViewModel> LayoutPartProperties { get; private set; }
		private LayoutPartPropertyViewModel _selectedLayoutPartProperty;
		public LayoutPartPropertyViewModel SelectedLayoutPartProperty
		{
			get { return _selectedLayoutPartProperty; }
			set
			{
				var isChanged = SelectedLayoutPartProperty != value && (SelectedLayoutPartProperty == null || value == null || SelectedLayoutPartProperty.Name != value.Name);
				_selectedLayoutPartProperty = value;
				ControlVisualArguments.Property = SelectedLayoutPartProperty == null ? null : (LayoutPartPropertyName?)SelectedLayoutPartProperty.LayoutPartProperty.Name;
				OnPropertyChanged(() => SelectedLayoutPartProperty);
				if (isChanged)
					UpdateContent();
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
				UpdateProperties();
			}
		}

		public bool ForAllClients
		{
			get { return ControlVisualArguments.ForAllClients; }
			set
			{
				ControlVisualArguments.ForAllClients = value;
				OnPropertyChanged(() => ForAllClients);
			}
		}

		public override string Description
		{
			get
			{
				var address = "<пусто>";
				if (SelectedLayout != null || SelectedLayoutPart != null || SelectedLayoutPartProperty != null)
					address = string.Format("<{0}|{1}|{2}>", SelectedLayout == null ? null : SelectedLayout.Name, SelectedLayoutPart == null ? null : SelectedLayoutPart.Name, SelectedLayoutPartProperty == null ? null : SelectedLayoutPartProperty.Name.ToDescription());
				var template = string.Empty;
				switch (SelectedControlVisualType)
				{
					case ControlVisualType.Get:
						template = "{0} = {1}";
						break;
					case ControlVisualType.Set:
						template = "{1} = {0}";
						break;
				}
				return string.Format(template, Argument.Description, address);
			}
		}
		public override void UpdateContent()
		{
			Layouts = new ObservableCollection<LayoutViewModel>(FiresecManager.LayoutsConfiguration.Layouts.Select(item => new LayoutViewModel(item)));
			SelectedLayout = Layouts.FirstOrDefault(x => x.Layout.UID == ControlVisualArguments.Layout);
			SelectedControlVisualType = ControlVisualTypes.FirstOrDefault(item => item == SelectedControlVisualType);
			if (SelectedLayoutPartProperty == null)
				Argument.Update(Procedure);
			else
				Argument.Update(Procedure, GetExplicitType());
		}

		private void UpdateProperties()
		{
			var access = SelectedControlVisualType == ControlVisualType.Get ? LayoutPartPropertyAccess.Get : LayoutPartPropertyAccess.Set;
			LayoutPartProperties = new ObservableCollection<LayoutPartPropertyViewModel>(SelectedLayoutPart == null || SelectedLayoutPart.Description == null ? Enumerable.Empty<LayoutPartPropertyViewModel>() : SelectedLayoutPart.Description.Properties.Where(item => item.Access == LayoutPartPropertyAccess.GetOrSet || item.Access == access).Select(item => new LayoutPartPropertyViewModel(item)));
			SelectedLayoutPartProperty = LayoutPartProperties.FirstOrDefault(x => x.LayoutPartProperty.Name == ControlVisualArguments.Property);
			OnPropertyChanged(() => LayoutPartProperties);
		}
		private ExplicitType GetExplicitType()
		{
			switch (SelectedLayoutPartProperty.Type)
			{
				case LayoutPartPropertyType.Boolean:
					return ExplicitType.Boolean;
				case LayoutPartPropertyType.DateTime:
					return ExplicitType.DateTime;
				case LayoutPartPropertyType.Double:
				case LayoutPartPropertyType.Integer:
					return ExplicitType.Integer;
				case LayoutPartPropertyType.String:
				case LayoutPartPropertyType.Object:
					return ExplicitType.String;
			}
			return ExplicitType.String;
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