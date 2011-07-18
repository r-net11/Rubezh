using System.Collections.Generic;

namespace FiresecClient.Models
{
    public class Direction
    {
        public Direction()
        {
            Zones = new List<int>();
        }

        public int Id { get; set; }
        public string Gid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> Zones { get; set; }
    }
}
