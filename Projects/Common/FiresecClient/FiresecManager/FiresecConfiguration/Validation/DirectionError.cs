using FiresecAPI.Models;

namespace FiresecClient.Validation
{
    public class DirectionError : BaseError
    {
        public DirectionError(Direction direction, string error, ErrorLevel level)
            : base(error, level)
        {
            Direction = direction;
        }

        public Direction Direction { get; set; }
    }
}