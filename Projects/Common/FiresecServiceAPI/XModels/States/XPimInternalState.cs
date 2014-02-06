
namespace XFiresecAPI
{
	public class XPimInternalState : XBaseInternalState
	{
		public XPim Pim { get; set; }

		public XPimInternalState(XPim pim)
		{
			Pim = pim;
		}
	}
}