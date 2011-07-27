
namespace FiresecClient.Models
{
    public class Category
    {
        public Category(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public bool IsEnable { get; set; }
    }
}
