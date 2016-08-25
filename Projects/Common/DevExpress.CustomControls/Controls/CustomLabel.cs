using DevExpress.CustomControls.CustomActions;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace DevExpress.CustomControls
{
	[ToolboxItem(true)]
	[XRDesigner("DevExpress.CustomControls.CustomLabelDesigner")]
	[DXDisplayName(typeof(ResFinder), "Resources.Common", "DevExpress.CustomControls.CustomLabel", "Текст")] //TODO: Add localization
	public class CustomLabel : XRLabel
	{
		public string CustomProperty { get; set; }
	}

	internal class CustomLabelDesigner : XRLabelDesigner
	{
		protected override void RegisterActionLists(DesignerActionListCollection list)
		{
			list.Add(new XRLabelDesignerCustomActionList1(this));
			list.Add(new XRLabelDesignerActionList2(this));
			list.Add(new XRLabelDesignerCustomActionList2(this));
		}
	}
}
