using DevExpress.XtraReports;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace DevExpress.CustomControls
{
	[ToolboxItem(true)]
	[XRDesigner("DevExpress.CustomControls.CustomShapeDesigner")]
	[DXDisplayName(typeof(ResFinder), "Resources.Common", "DevExpress.CustomControls.CustomShape", "Фигура")] //TODO: Add localization
	public class CustomShape : XRShape
	{
	}

	public class CustomShapeDesigner : XRShapeDesigner
	{
		protected override void RegisterActionLists(DesignerActionListCollection list)
		{
			list.Add(new XRShapeDesignerActionList1(this));
			list.Add(new XRShapeDesignerActionList2(this));
			list.Add(ShapeActionListFactory.CreateShapeActionList(XRShape.Shape, this));
		}
	}
}
