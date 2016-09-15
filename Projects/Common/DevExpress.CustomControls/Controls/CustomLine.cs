using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using DevExpress.CustomControls.CustomActions;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;

namespace DevExpress.CustomControls
{
	[ToolboxItem(true)]
	[XRDesigner("DevExpress.CustomControls.CustomLineDesigner")]
	public class CustomLine : XRLine
	{
	}

	public class CustomLineDesigner : XRLineDesigner
	{
		protected override void RegisterActionLists(DesignerActionListCollection list)
		{
			list.Add(new XRLineDesignerActionList(this));
		}
	}
}
