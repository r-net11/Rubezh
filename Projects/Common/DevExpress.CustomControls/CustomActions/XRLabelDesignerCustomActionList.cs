using DevExpress.XtraReports.Design;
using System.ComponentModel.Design;
using System.Linq;

namespace DevExpress.CustomControls.CustomActions
{
	public class XRLabelDesignerCustomActionList1 : XRControlTextDesignerActionList
	{
		public XRLabelDesignerCustomActionList1(XRControlDesigner designer) : base(designer)
		{
		}

		public override DesignerActionItemCollection GetSortedActionItems()
		{
			var items = base.GetSortedActionItems();
			var item = items.OfType<DesignerActionPropertyItem>().FirstOrDefault(x => x.MemberName == "FormatString");

			if (item != null)
				items.Remove(item);

			return items;
		}
	}
}
