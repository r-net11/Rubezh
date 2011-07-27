
namespace FiresecClient.Models
{
    public class Event
    {
        public Event(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public bool IsEnable { get; set; }
    }
}
