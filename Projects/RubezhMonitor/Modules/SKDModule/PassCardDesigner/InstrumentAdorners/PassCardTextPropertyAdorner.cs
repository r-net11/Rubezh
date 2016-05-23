using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Plans.Elements;
using RubezhAPI.SKD;
using SKDModule.PassCardDesigner.ViewModels;
using System;

namespace SKDModule.PassCardDesigner.InstrumentAdorners
{
	public class PassCardTextPropertyAdorner : BaseRectangleAdorner
	{
		Guid _organisationUID;

		public PassCardTextPropertyAdorner(CommonDesignerCanvas designerCanvas, Guid organisationUID)
			: base(designerCanvas)
		{
			_organisationUID = organisationUID;
		}

		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			var element = new ElementPassCardTextProperty() { Left = left, Top = top };
			element.OrganisationUID = _organisationUID;
			var propertiesViewModel = new PassCardTextPropertyViewModel(element, DesignerCanvas);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}