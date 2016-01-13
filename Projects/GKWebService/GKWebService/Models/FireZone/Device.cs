using GKWebService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GKWebService.Models.FireZone
{
    public class Device
    {
        public String Address { get; set; }

        public String ImageSource { get; set; }

        public Tuple<string, System.Drawing.Size> ImageBloom { get; set; }

        [DataMember]
        public String ShortName { get; set; }

        public Device(string address, string imageSource, string shortName)
        {
            Address = address;
            ImageSource = imageSource;
            ShortName = shortName;
            ImageBloom = InternalConverter.GetImageResource(ImageSource);
        }
    }
}