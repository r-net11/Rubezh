using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI;
using RubezhAPI.Plans.Elements;
using RubezhAPI.SKD;
using SKDModule.PassCardDesigner.ViewModels;
using System;

namespace SKDModule.PassCardDesigner.InstrumentAdorners
{
	public class PassCardImagePropertyAdorner : BaseRectangleAdorner
	{
		public PassCardImagePropertyAdorner(CommonDesignerCanvas designerCanvas, Guid organisationUID)
			: base(designerCanvas)
		{
			_organisationUID = organisationUID;
		}

		Guid _organisationUID;

		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			var element = new ElementPassCardImageProperty() { BackgroundColor = Colors.Transparent, Left = left, Top = top };
			element.OrganisationUID = _organisationUID;
			var propertiesViewModel = new PassCardImagePropertyViewModel(element, DesignerCanvas);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}