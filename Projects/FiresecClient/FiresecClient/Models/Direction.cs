using System.Collections.Generic;

namespace FiresecClient.Models
{
    public class Direction
    {
        public Direction()
        {
            Zones = new List<string>();
        }

        public int Id { get; set; }
        public string Gid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Zones { get; set; }
        public string DeviceRm { get; set; }
        public string DeviceButton { get; set; }
    }
}
