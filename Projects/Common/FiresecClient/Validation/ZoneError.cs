using FiresecAPI.Models;

namespace FiresecClient.Validation
{
    public class ZoneError : BaseError
    {
        public ZoneError(Zone zone, string error, ErrorLevel level)
            : base(error, level)
        {
            Zone = zone;
        }

        public Zone Zone { get; set; }
    }
}