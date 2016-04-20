using Common;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
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

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementPassCardImageProperty() { BackgroundColor = Colors.Transparent };
			element.OrganisationUID = _organisationUID;
			var propertiesViewModel = new PassCardImagePropertyViewModel(element);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			return element;
		}
	}
}