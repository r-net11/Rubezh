using System.Collections.Generic;
using Common;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using SKDModule.PassCardDesigner.Designer;
using SKDModule.PassCardDesigner.Painter;

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
			using (new TimeCounter("\tPassCardDesignerViewModel.Initialize: {0}"))
			using (new WaitWrapper())
			{
				using (new TimeCounter("\t\tDesignerCanvas.Initialize: {0}"))
					((DesignerCanvas)DesignerCanvas).Initialize(PassCardTemplate);
				if (PassCardTemplate != null)
				{
					using (new TimeCounter("\t\tDesignerItem.Create: {0}"))
					{
						foreach (var elementBase in EnumerateElements())
							DesignerCanvas.Create(elementBase);
						DesignerCanvas.UpdateZIndex();
					}
					using (new TimeCounter("\t\tPassCardDesignerViewModel.OnUpdated: {0}"))
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
				e.PropertyViewModel = new PassCardImagePropertyViewModel((ElementPassCardImageProperty)e.Element);
			else if (e.Element is ElementPassCardTextProperty)
				e.PropertyViewModel = new PassCardTextPropertyViewModel((ElementPassCardTextProperty)e.Element);
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