namespace FiresecClient.Validation
{
    public class BaseError
    {
        public BaseError(string error, ErrorLevel level)
        {
            Error = error;
            Level = level;
        }

        public string Error { get; set; }
        public ErrorLevel Level { get; set; }
    }
}