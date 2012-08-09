using FiresecClient.Validation;
using XFiresecAPI;

namespace FiresecClient.XModelsValidator
{
	public class ZoneError : BaseError
	{
		public ZoneError(XZone zone, string error, ErrorLevel level)
			: base(error, level)
		{
			Zone = zone;
		}

		public XZone Zone { get; set; }
	}
}