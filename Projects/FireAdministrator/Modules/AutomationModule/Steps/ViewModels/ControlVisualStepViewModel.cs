using System.Collections.ObjectModel;
using StrazhAPI;
using System.Linq;
using StrazhAPI.Automation;
using FiresecClient;
using System;
using System.Collections.Generic;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Services.Layout;
using StrazhAPI.Models.Layouts;

namespace AutomationModule.ViewModels
{
	public class ControlVisualStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ValueArgument { get; private set; }
		public ControlVisualArguments ControlVisualArguments { get; private set; }
		public ControlElementType ControlElementType { get; private set; }

		public ControlVisualStepViewModel(StepViewModel stepViewModel, ControlElementType controlElementType) : base(stepViewModel)
		{
			ControlVisualArguments = stepViewModel.Step.ControlVisualArguments;
			ControlElementType = controlElementType;
			ValueArgument = new ArgumentViewModel(ControlVisualArguments.Argument, stepViewModel.Update, UpdateContent, controlElementType == ControlElementType.Set);
		}


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
				LayoutParts = new ObservableCollection<LayoutPartViewModel>(SelectedLayout == null
					? Enumerable.Empty<LayoutPartViewModel>()
					: SelectedLayout.Layout.Parts.Select(item => new LayoutPartViewModel(item, GetDescription(item))).Where(item => item.Description != null && item.Description.Properties.Any()));
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
				_selectedLayoutPartProperty = value;
				ControlVisualArguments.Property = SelectedLayoutPartProperty == null ? null : (LayoutPartPropertyName?)SelectedLayoutPartProperty.LayoutPartProperty.Name;
				if (SelectedLayoutPartProperty != null)
				{
					var explicitTypeViewModel = PropertyTypeToExplicitType(SelectedLayoutPartProperty);
					ValueArgument.Update(Procedure, explicitTypeViewModel.ExplicitType, explicitTypeViewModel.EnumType);
				}
				OnPropertyChanged(() => SelectedLayoutPartProperty);
			}
		}

		public bool StoreOnServer
		{
			get { return ControlVisualArguments.StoreOnServer; }
			set
			{
				ControlVisualArguments.StoreOnServer = value;
				OnPropertyChanged(() => StoreOnServer);
			}
		}

		public bool CanStoreOnServer
		{
			get { return ControlElementType == ControlElementType.Set && ForAllClients; }
		}

		public bool ForAllClients
		{
			get { return ControlVisualArguments.ForAllClients; }
			set
			{
				ControlVisualArguments.ForAllClients = value;
				if (value == false)
					StoreOnServer = false;
				OnPropertyChanged(() => ForAllClients);
				OnPropertyChanged(() => CanStoreOnServer);
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

		public override void UpdateContent()
		{
			Layouts = new ObservableCollection<LayoutViewModel>(FiresecManager.LayoutsConfiguration.Layouts.Select(item => new LayoutViewModel(item)));
			SelectedLayout = Layouts.FirstOrDefault(x => x.Layout.UID == ControlVisualArguments.Layout);
			OnPropertyChanged(() => Layouts);
		}

		private void UpdateProperties()
		{
			var access = ControlElementType == ControlElementType.Get ? LayoutPartPropertyAccess.Get : LayoutPartPropertyAccess.Set;
			LayoutPartProperties = new ObservableCollection<LayoutPartPropertyViewModel>(SelectedLayoutPart == null || SelectedLayoutPart.Description == null ? Enumerable.Empty<LayoutPartPropertyViewModel>() : SelectedLayoutPart.Description.Properties.Where(item => item.Access == LayoutPartPropertyAccess.GetOrSet || item.Access == access).Select(item => new LayoutPartPropertyViewModel(item)));
			SelectedLayoutPartProperty = LayoutPartProperties.FirstOrDefault(x => x.LayoutPartProperty.Name == ControlVisualArguments.Property);
			OnPropertyChanged(() => LayoutPartProperties);
			OnPropertyChanged(() => StoreOnServer);
			OnPropertyChanged(() => CanStoreOnServer);
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