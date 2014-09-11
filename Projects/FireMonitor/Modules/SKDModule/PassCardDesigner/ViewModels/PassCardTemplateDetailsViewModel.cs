using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.ViewModels;
using FiresecAPI.SKD;
using Controls.Menu.ViewModels;
using Infrastructure.Common;
using Infrustructure.Plans.Services;
using Infrastructure.Designer.ViewModels;
using System.Collections.ObjectModel;
using Infrustructure.Plans.Designer;
using Infrastructure.Client.Plans;
using SKDModule.PassCardDesigner.InstrumentAdorners;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class PassCardTemplateDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortPassCardTemplate>
	{
		private Guid _organisationUID { get; set; }
		public PassCardTemplate PassCardTemplate { get; private set; }

		public const string PassCardImagePropertiesGroup = "PassCardImagePropertiesGroup";
		public const string PassCardTextPropertiesGroup = "PassCardTextPropertiesGroup";
		public ElementsViewModel ElementsViewModel { get; private set; }
		public PassCardDesignerViewModel PassCardDesignerViewModel { get; private set; }
		public Infrastructure.Designer.DesignerCanvas DesignerCanvas
		{
			get { return PassCardDesignerViewModel.DesignerCanvas; }
		}

		public PassCardTemplateDetailsViewModel()
		{
			LayerGroupService.Instance.RegisterGroup(PassCardTextPropertiesGroup, "Текстовые свойства", 1);
			LayerGroupService.Instance.RegisterGroup(PassCardImagePropertiesGroup, "Графические свойства", 2);
			PassCardDesignerViewModel = new PassCardDesignerViewModel();
			OnPropertyChanged(() => PassCardDesignerViewModel);
			PassCardDesignerViewModel.DesignerCanvas.ZoomChanged();
			ElementsViewModel = new ElementsViewModel(PassCardDesignerViewModel.DesignerCanvas);

			Menu = new MenuViewModel()
			{
				Items = new ObservableCollection<BaseViewModel>()
			    {
			        new MenuButtonViewModel(PassCardDesignerViewModel.CopyCommand, "/Controls;component/Images/Copy.png" , "Копировать"),
			        new MenuButtonViewModel(PassCardDesignerViewModel.CutCommand, "/Controls;component/Images/Cut.png" , "Вырезать"),
			        new MenuButtonViewModel(PassCardDesignerViewModel.PasteCommand, "/Controls;component/Images/Paste.png" , "Вставить"),
			        new MenuButtonViewModel(PassCardDesignerViewModel.UndoCommand, "/Controls;component/Images/Undo.png" , "Отменить"),
			        new MenuButtonViewModel(PassCardDesignerViewModel.RedoCommand, "/Controls;component/Images/Redo.png" , "Применить"),
			        //new MenuSeparatorViewModel(),
			        new MenuButtonViewModel(PassCardDesignerViewModel.MoveToFrontCommand, "/Controls;component/Images/MoveForward.png" , "Вверх"),
			        new MenuButtonViewModel(PassCardDesignerViewModel.SendToBackCommand, "/Controls;component/Images/MoveBackward.png" , "Вниз"),
			        new MenuButtonViewModel(PassCardDesignerViewModel.MoveForwardCommand, "/Controls;component/Images/MoveFront.png" , "Выше"),
			        new MenuButtonViewModel(PassCardDesignerViewModel.MoveBackwardCommand, "/Controls;component/Images/MoveBack.png" , "Ниже"),
			    }
			};
			DesignerCanvas.Toolbox.RegisterInstruments(GetInstruments());
		}

		public MenuViewModel Menu { get; private set; }

		private IEnumerable<IInstrument> GetInstruments()
		{
			yield return new InstrumentViewModel()
			{
				ImageSource = "/Controls;component/Images/Text.png",
				ToolTip = "Текстовое свойство",
				Adorner = new PassCardTextPropertyAdorner(DesignerCanvas),
				Index = 300,
				Autostart = true
			};
			yield return new InstrumentViewModel()
			{
				ImageSource = "/Controls;component/Images/Photo.png",
				ToolTip = "Графическое свойство",
				Adorner = new PassCardImagePropertyAdorner(DesignerCanvas),
				Index = 301,
				Autostart = true
			};
		}

		#region IDetailsViewModel<ShortPassCardTemplate> Members

		public ShortPassCardTemplate Model
		{
			get
			{
				return new ShortPassCardTemplate
				{
					UID = PassCardTemplate.UID,
					Name = PassCardTemplate.Caption,
					Description = PassCardTemplate.Description,
					OrganisationUID = PassCardTemplate.OrganisationUID
				};
			}
		}

		public void Initialize(Organisation organisation, ShortPassCardTemplate model, ViewPartViewModel parentViewModel)
		{
			_organisationUID = organisation.UID;
			if (model == null)
			{
				Title = "Создание шаблона пропусков";
				PassCardTemplate = new PassCardTemplate()
				{
					Caption = "Новая шаблон",
					OrganisationUID = _organisationUID
				};
			}
			else
			{
				//PassCardTemplate = PassCardTemplateHelper.GetDetails(model.UID);
				Title = string.Format("Шаблон пропусков: {0}", PassCardTemplate.Caption);
			}
			//CopyProperties();
		}

		#endregion
	}
}
