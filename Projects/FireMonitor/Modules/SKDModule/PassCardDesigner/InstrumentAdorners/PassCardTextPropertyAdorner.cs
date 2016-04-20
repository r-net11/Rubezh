using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
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

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementPassCardTextProperty();
			element.OrganisationUID = _organisationUID;
			var propertiesViewModel = new PassCardTextPropertyViewModel(element);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			return element;
		}
	}
}