using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Color = System.Windows.Media.Color;

namespace GKWebService.Models
{
    public class Shape
    {
        public System.Windows.Media.Color Border
        { get; set; }

        public System.Windows.Media.Color BorderMouseOver
        { get; set; }

        public System.Windows.Media.Color Fill
        { get; set; }

        public Color FillMouseOver
        { get; set; }

        public Guid Id
        { get; set; }

        public string Name
        { get; set; }

        public string Path
        { get; set; }
    }
}