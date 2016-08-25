using DevExpress.XtraReports.Design;
using System.ComponentModel.Design;
using System.Linq;

namespace DevExpress.CustomControls.CustomActions
{
	public class XRLabelDesignerCustomActionList2 : XRLabelDesignerActionList3
	{
		public XRLabelDesignerCustomActionList2(XRTextControlDesigner designer) : base(designer)
		{
		}

		public override DesignerActionItemCollection GetSortedActionItems()
		{
			var items = base.GetSortedActionItems();
			var item = items.OfType<DesignerActionPropertyItem>().FirstOrDefault(x => x.MemberName == "Shrink");

			if (item != null)
				items.Remove(item);

			return items;
		}
	}
}
