using Infrastructure.Common.Windows.Services.Layout;
using Infrastructure.Common.Windows.Windows;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ControlVisualStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ValueArgument { get; private set; }
		public ControlVisualStep ControlVisualStep { get; private set; }
		public ControlElementType ControlElementType { get; private set; }

		public ControlVisualStepViewModel(StepViewModel stepViewModel, ControlElementType controlElementType)
			: base(stepViewModel)
		{
			ControlVisualStep = (ControlVisualStep)stepViewModel.Step;
			ControlElementType = controlElementType;
			IsServerContext = Procedure.ContextType == ContextType.Server;
			ValueArgument = new ArgumentViewModel(ControlVisualStep.Argument, stepViewModel.Update, UpdateContent, controlElementType == ControlElementType.Set);
		}


		public ObservableCollection<LayoutViewModel> Layouts { get; private set; }
		private LayoutViewModel _selectedLayout;
		public LayoutViewModel SelectedLayout
		{
			get { return _selectedLayout; }
			set
			{
				_selectedLayout = value;
				ControlVisualStep.Layout = SelectedLayout == null ? Guid.Empty : SelectedLayout.Layout.UID;
				OnPropertyChanged(() => SelectedLayout);
				LayoutParts = new ObservableCollection<LayoutPartViewModel>(SelectedLayout == null ? Enumerable.Empty<LayoutPartViewModel>() : SelectedLayout.Layout.Parts.Select(item => new LayoutPartViewModel(item, GetDescription(item))).Where(item => item.Description != null && item.Description.Properties.Count() > 0));
				SelectedLayoutPart = LayoutParts.FirstOrDefault(x => x.LayoutPart.UID == ControlVisualStep.LayoutPart);
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
				ControlVisualStep.LayoutPart = SelectedLayoutPart == null ? Guid.Empty : SelectedLayoutPart.LayoutPart.UID;
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
				_selectedLayoutPartProperty = value;
				ControlVisualStep.Property = SelectedLayoutPartProperty == null ? null : (LayoutPartPropertyName?)SelectedLayoutPartProperty.LayoutPartProperty.Name;
				if (SelectedLayoutPartProperty != null)
				{
					var explicitTypeViewModel = PropertyTypeToExplicitType(SelectedLayoutPartProperty);
					ValueArgument.Update(Procedure, explicitTypeViewModel.ExplicitType, explicitTypeViewModel.EnumType, isList: false);
				}
				OnPropertyChanged(() => SelectedLayoutPartProperty);
			}
		}

		public bool ForAllClients
		{
			get { return ControlVisualStep.ForAllClients; }
			set
			{
				ControlVisualStep.ForAllClients = value;
				OnPropertyChanged(() => ForAllClients);
			}
		}

		public override string Description
		{
			get
			{
				return "Макет: " + (SelectedLayout != null ? SelectedLayout.Name : "<пусто>") + "; Элемент: " + (SelectedLayoutPart != null ? SelectedLayoutPart.Name : "<пусто>") +
					"; Свойство: " + (SelectedLayoutPartProperty != null ? SelectedLayoutPartProperty.Name.ToDescription() : "<пусто>") + "; Операция: " + ControlElementType.ToDescription() + "; Значение: " + ValueArgument.Description;
			}
		}

		bool _isServerContext;
		public bool IsServerContext
		{
			get { return _isServerContext; }
			set
			{
				_isServerContext = value;
				OnPropertyChanged(() => IsServerContext);
			}
		}
		public override void UpdateContent()
		{
			IsServerContext = Procedure.ContextType == ContextType.Server;
			Layouts = new ObservableCollection<LayoutViewModel>(ClientManager.LayoutsConfiguration.Layouts.Select(item => new LayoutViewModel(item)));
			SelectedLayout = Layouts.FirstOrDefault(x => x.Layout.UID == ControlVisualStep.Layout);
			OnPropertyChanged(() => Layouts);
		}

		private void UpdateProperties()
		{
			var access = ControlElementType == ControlElementType.Get ? LayoutPartPropertyAccess.Get : LayoutPartPropertyAccess.Set;
			LayoutPartProperties = new ObservableCollection<LayoutPartPropertyViewModel>(SelectedLayoutPart == null || SelectedLayoutPart.Description == null ? Enumerable.Empty<LayoutPartPropertyViewModel>() : SelectedLayoutPart.Description.Properties.Where(item => item.Access == LayoutPartPropertyAccess.GetOrSet || item.Access == access).Select(item => new LayoutPartPropertyViewModel(item)));
			SelectedLayoutPartProperty = LayoutPartProperties.FirstOrDefault(x => x.LayoutPartProperty.Name == ControlVisualStep.Property);
			OnPropertyChanged(() => LayoutPartProperties);
		}

		ExplicitTypeViewModel PropertyTypeToExplicitType(LayoutPartPropertyViewModel layoutPartPropertyViewModel)
		{
			if (layoutPartPropertyViewModel.Name == LayoutPartPropertyName.BorderThickness || layoutPartPropertyViewModel.Name == LayoutPartPropertyName.Margin)
				return new ExplicitTypeViewModel(ExplicitType.Integer);
			if (layoutPartPropertyViewModel.Name == LayoutPartPropertyName.BackgroundColor || layoutPartPropertyViewModel.Name == LayoutPartPropertyName.BorderColor)
				return new ExplicitTypeViewModel(EnumType.ColorType);
			if (layoutPartPropertyViewModel.Name == LayoutPartPropertyName.Text || layoutPartPropertyViewModel.Name == LayoutPartPropertyName.Title)
				return new ExplicitTypeViewModel(ExplicitType.String);
			return new ExplicitTypeViewModel(ExplicitType.Integer);
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