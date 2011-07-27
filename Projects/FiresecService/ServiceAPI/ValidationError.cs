
namespace FiresecClient
{
    public class ValidationError
    {
        public ValidationError(string error, Level level)
        {
            Error = error;
            Level = level;
        }

        public string Error { get; set; }
        public Level Level { get; set; }
    }

    public enum Level
    {
        Critical,
        Normal
    }
}
