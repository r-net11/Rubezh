using DevExpress.XtraReports;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using ResFinder = DevExpress.XtraReports.ResFinder;

namespace DevExpress.CustomControls
{
	[ToolboxItem(true)]
	[XRDesigner("DevExpress.CustomControls.CustomImageDesigner")]
	[DXDisplayName(typeof(ResFinder), "Resources.Common", "DevExpress.CustomControls.CustomImage", "Изображение")] //TODO: Add localization
	public class CustomImage : XRPictureBox
	{
		public string CustomProperty { get; set; }
	}

	internal class CustomImageDesigner : XRPictureBoxDesigner
	{
		protected override void RegisterActionLists(DesignerActionListCollection list)
		{
			list.Add(new XRPictureBoxDesignerActionList1(this));
			list.Add(new XRPictureBoxDesignerActionList3(this));
		}
	}
}
