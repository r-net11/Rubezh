using Infrastructure;
using Infrastructure.Common;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Events;
using RubezhAPI.Plans.Elements;
using RubezhAPI.SKD;
using SKDModule.PassCardDesigner.Designer;
using SKDModule.PassCardDesigner.Painter;
using System.Collections.Generic;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class PassCardDesignerViewModel : Infrastructure.Designer.ViewModels.PlanDesignerViewModel
	{
		public const string PassCardImagePropertiesGroup = "PassCardImagePropertiesGroup";
		public const string PassCardTextPropertiesGroup = "PassCardTextPropertiesGroup";

		public PassCardTemplate PassCardTemplate { get; private set; }
		public PassCardDesignerViewModel()
		{
			DesignerCanvas = new DesignerCanvas(this);
			DesignerCanvas.Toolbox.IsRightPanel = false;
			AllowScalePoint = false;
			FullScreenSize = false;
			CanCollapse = false;
		}

		public void Initialize(PassCardTemplate passCardTemplate)
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);
			PassCardTemplate = passCardTemplate;
			IsNotEmpty = PassCardTemplate != null;
			using (new WaitWrapper())
			{
				((DesignerCanvas)DesignerCanvas).Initialize(PassCardTemplate);
				if (PassCardTemplate != null)
				{
					foreach (var elementBase in EnumerateElements())
						DesignerCanvas.Create(elementBase);
					DesignerCanvas.UpdateZIndex();
					Update();
				}
			}
			ResetHistory();
		}
		private IEnumerable<ElementBase> EnumerateElements()
		{
			foreach (var elementTextProperty in PassCardTemplate.ElementTextProperties)
				yield return elementTextProperty;
			foreach (var elementImageProperty in PassCardTemplate.ElementImageProperties)
				yield return elementImageProperty;
			foreach (var elementRectangle in PassCardTemplate.ElementRectangles)
				yield return elementRectangle;
			foreach (var elementEllipse in PassCardTemplate.ElementEllipses)
				yield return elementEllipse;
			foreach (var elementTextBlock in PassCardTemplate.ElementTextBlocks)
				yield return elementTextBlock;
			foreach (var elementPolygon in PassCardTemplate.ElementPolygons)
				yield return elementPolygon;
			foreach (var elementPolyline in PassCardTemplate.ElementPolylines)
				yield return elementPolyline;
		}
		public override void RegisterDesignerItem(DesignerItem designerItem)
		{
			base.RegisterDesignerItem(designerItem);
			if (designerItem.Element is ElementPassCardImageProperty)
			{
				designerItem.Group = PassCardDesignerViewModel.PassCardImagePropertiesGroup;
				designerItem.Title = ((ElementPassCardImageProperty)designerItem.Element).Text.Replace('\n', ' ');
				designerItem.UpdateProperties += DesignerItemPropertyChanged;
			}
			else if (designerItem.Element is ElementPassCardTextProperty)
			{
				designerItem.Group = PassCardDesignerViewModel.PassCardTextPropertiesGroup;
				designerItem.Title = ((ElementPassCardTextProperty)designerItem.Element).Text.Replace('\n', ' ');
				designerItem.UpdateProperties += DesignerItemPropertyChanged;
			}
		}

		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			if (e.Element is ElementPassCardImageProperty)
			{
				var elementPassCardImageProperty = (ElementPassCardImageProperty)e.Element;
				elementPassCardImageProperty.OrganisationUID = PassCardTemplate.OrganisationUID;
				e.PropertyViewModel = new PassCardImagePropertyViewModel(elementPassCardImageProperty);
			}
			else if (e.Element is ElementPassCardTextProperty)
			{
				var elementPassCardTextProperty = (ElementPassCardTextProperty)e.Element;
				elementPassCardTextProperty.OrganisationUID = PassCardTemplate.OrganisationUID;
				e.PropertyViewModel = new PassCardTextPropertyViewModel(elementPassCardTextProperty);
			}
		}
		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			if (args.DesignerCanvas != DesignerCanvas)
				return;
			var elementPassCardImageProperty = args.Element as ElementPassCardImageProperty;
			if (elementPassCardImageProperty != null)
				args.Painter = new PassCardImagePropertyPainter(DesignerCanvas, elementPassCardImageProperty);
		}

		private void DesignerItemPropertyChanged(CommonDesignerItem designerItem)
		{
			if (designerItem.Element is ElementPassCardImageProperty)
				designerItem.Title = ((ElementPassCardImageProperty)designerItem.Element).Text.Replace('\n', ' ');
			else if (designerItem.Element is ElementPassCardTextProperty)
				designerItem.Title = ((ElementPassCardTextProperty)designerItem.Element).Text.Replace('\n', ' ');
		}
	}
}