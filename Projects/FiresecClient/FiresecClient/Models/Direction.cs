using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecClient.Models
{
    public class Direction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> Zones { get; set; }
    }
}
