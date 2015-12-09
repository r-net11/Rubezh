using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GKWebService.Models.FireZone
{
    public class FireZone
    {
        public int Fire1Count { get; set; }

        public int Fire2Count { get; set; }

        public List<Device> devicesList;

        public FireZone()
        {
            devicesList = new List<Device>();
        }
    }
}